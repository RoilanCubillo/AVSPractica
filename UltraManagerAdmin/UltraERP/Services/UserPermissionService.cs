using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Security.EntitiesAVS;
using UltraERP.BusinessLogic;

namespace UltraERP.Services
{
    public enum UserRoleLevel
    {
        Usuario = 0,
        Soporte = 1,
        Administrador = 2
    }

    public static class UserPermissionService
    {
        public static UserRoleLevel GetCurrentRole()
        {
            UserRoleLevel? securityRole = GetCurrentRoleFromSecurity();
            if (securityRole.HasValue)
                return securityRole.Value;

            if (IsProcessAuthorizationAccount())
                return UserRoleLevel.Administrador;

            if (IsSupportAccount())
                return UserRoleLevel.Soporte;

            return UserRoleLevel.Usuario;
        }

        public static bool IsAdmin()
        {
            return GetCurrentRole() == UserRoleLevel.Administrador;
        }

        public static bool IsSupport()
        {
            return GetCurrentRole() == UserRoleLevel.Soporte;
        }

        public static bool CanExecuteProcessAction(string password)
        {
            if (IsAdmin())
                return true;

            if (!IsSupport())
                return false;

            return IsProcessPasswordValid(password);
        }

        public static bool CanAccessView(string viewCode)
        {
            if (IsAdmin())
                return true;

            if (String.IsNullOrWhiteSpace(viewCode))
                return true;

            HttpContext context = HttpContext.Current;
            EN_SC_View[] views = context == null || context.Session == null
                ? null
                : context.Session["USER_VIEWS"] as EN_SC_View[];

            return views != null && views.Any(x => String.Equals((x.Code ?? "").Trim(), viewCode.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public static string GetCurrentRoleName()
        {
            return GetCurrentRole().ToString();
        }

        public static string GetCurrentRoleSource()
        {
            HttpContext context = HttpContext.Current;
            return context != null && context.Session != null && context.Session["USER_ROLE_SOURCE"] != null
                ? Convert.ToString(context.Session["USER_ROLE_SOURCE"])
                : "";
        }

        public static string GetCurrentAccount()
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["USER_ACCOUNT"] != null)
                return Convert.ToString(context.Session["USER_ACCOUNT"]).Trim();

            return context != null && context.User != null && context.User.Identity != null
                ? (context.User.Identity.Name ?? "").Trim()
                : "";
        }

        public static string GetCurrentUserName()
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Session != null && context.Session["USER_NAME"] != null)
            {
                string sessionName = Convert.ToString(context.Session["USER_NAME"]);
                if (!String.IsNullOrWhiteSpace(sessionName))
                    return sessionName.Trim();
            }

            return context != null && context.User != null && context.User.Identity != null
                ? (context.User.Identity.Name ?? "").Trim()
                : "";
        }

        private static UserRoleLevel? GetCurrentRoleFromSecurity()
        {
            HttpContext context = HttpContext.Current;
            if (context == null || context.Session == null)
                return null;

            string cachedRole = Convert.ToString(context.Session["USER_ROLE"]);
            string cachedSource = Convert.ToString(context.Session["USER_ROLE_SOURCE"]);
            if (!String.IsNullOrWhiteSpace(cachedRole) &&
                String.Equals(cachedSource, "AVS_SECURITY_DEV", StringComparison.OrdinalIgnoreCase))
            {
                UserRoleLevel parsedRole;
                if (Enum.TryParse(cachedRole, true, out parsedRole))
                    return parsedRole;
            }

            List<SecurityRoleInfo> roles = GetSecurityRoles();
            if (roles == null || roles.Count == 0)
                return null;

            UserRoleLevel role = ResolveRoleLevel(roles);
            context.Session["USER_ROLE"] = role.ToString();
            context.Session["USER_ROLE_SOURCE"] = "AVS_SECURITY_DEV";
            context.Session["USER_SECURITY_ROLES"] = String.Join(",", roles.Select(x => x.Code).Where(x => !String.IsNullOrWhiteSpace(x)));

            return role;
        }

        private static List<SecurityRoleInfo> GetSecurityRoles()
        {
            var roles = new List<SecurityRoleInfo>();
            HttpContext context = HttpContext.Current;
            if (context == null || context.Session == null)
                return roles;

            int userID = GetSessionInt("USER_ID");
            int autoID = GetSessionInt("USER_AUTOID");
            int companyID = GetSessionInt("USER_COMPANY_ID");
            int systemID = Convert.ToInt32(ConfigurationManager.AppSettings["SystemID"] ?? "1");

            if (userID <= 0 || autoID <= 0)
                return roles;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["Security.DataAccess.Properties.Settings.AVS_SECURITYConnectionString1"];
            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return roles;

            try
            {
                using (var connection = new SqlConnection(BuildFastFailConnectionString(settings.ConnectionString)))
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 5;
                    command.CommandText = @"
SELECT DISTINCT
    R.ID,
    ISNULL(R.Code, '') AS Code,
    ISNULL(R.Description, '') AS Description,
    ISNULL(R.EnAdmin, 0) AS EnAdmin,
    ISNULL(R.EnAllControls, 0) AS EnAllControls,
    ISNULL(R.EnAllAccessForms, 0) AS EnAllAccessForms
FROM dbo.SC_User U
INNER JOIN dbo.SC_UserRole UR
    ON UR.UserID = U.ID
   AND UR.ID_Company = U.ID_Company
INNER JOIN dbo.SC_Role R
    ON R.ID = UR.RoleID
   AND R.SystemID = U.SystemID
   AND R.ID_Company = UR.ID_Company
WHERE U.ID = @UserID
  AND U.AutoID = @AutoID
  AND U.SystemID = @SystemID
  AND (@CompanyID = 0 OR U.ID_Company = @CompanyID)
  AND ISNULL(U.Enable, 0) = 1
  AND ISNULL(R.Enable, 0) = 1;";

                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                    command.Parameters.Add("@AutoID", SqlDbType.Int).Value = autoID;
                    command.Parameters.Add("@SystemID", SqlDbType.Int).Value = systemID;
                    command.Parameters.Add("@CompanyID", SqlDbType.Int).Value = companyID;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new SecurityRoleInfo
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Code = Convert.ToString(reader["Code"]),
                                Description = Convert.ToString(reader["Description"]),
                                EnAdmin = Convert.ToBoolean(reader["EnAdmin"]),
                                EnAllControls = Convert.ToBoolean(reader["EnAllControls"]),
                                EnAllAccessForms = Convert.ToBoolean(reader["EnAllAccessForms"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogService.Log(ex, "Permisos", "Leer roles AVS_SECURITY_DEV", new { userID, autoID, companyID, systemID });
            }

            return roles;
        }

        private static UserRoleLevel ResolveRoleLevel(IEnumerable<SecurityRoleInfo> roles)
        {
            if (roles.Any(x => x.EnAdmin))
                return UserRoleLevel.Administrador;

            if (roles.Any(x => ContainsRoleText(x, "soporte") || ContainsRoleText(x, "support")))
                return UserRoleLevel.Soporte;

            if (IsSupportAccount())
                return UserRoleLevel.Soporte;

            return UserRoleLevel.Usuario;
        }

        private static bool ContainsRoleText(SecurityRoleInfo role, string text)
        {
            return role != null &&
                   ((role.Code ?? "").IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (role.Description ?? "").IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static bool IsProcessPasswordValid(string password)
        {
            if (String.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                string account = ConfigurationManager.AppSettings["ProcessAuthorizationUserAccount"] ?? "100";
                var result = new CT_User().ValidateUser(account, password.Trim());
                return result.Item1 != null && result.Item2 == 0;
            }
            catch (Exception ex)
            {
                ErrorLogService.Log(ex, "Permisos", "Validar clave de proceso", new { Account = ConfigurationManager.AppSettings["ProcessAuthorizationUserAccount"] ?? "100" });
                return false;
            }
        }

        private static bool IsProcessAuthorizationAccount()
        {
            string account = ConfigurationManager.AppSettings["ProcessAuthorizationUserAccount"] ?? "100";
            return String.Equals(GetCurrentAccount(), account, StringComparison.OrdinalIgnoreCase) ||
                   String.Equals(GetCurrentUserName(), account, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSupportAccount()
        {
            string account = GetCurrentAccount();
            string userName = GetCurrentUserName();

            return String.Equals(account, "soporte", StringComparison.OrdinalIgnoreCase) ||
                   String.Equals(account, "SoporteAVS", StringComparison.OrdinalIgnoreCase) ||
                   userName.IndexOf("soporte", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static int GetSessionInt(string key)
        {
            HttpContext context = HttpContext.Current;
            if (context == null || context.Session == null || context.Session[key] == null)
                return 0;

            int value;
            return Int32.TryParse(Convert.ToString(context.Session[key]), out value) ? value : 0;
        }

        private static string BuildFastFailConnectionString(string connectionString)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    ConnectTimeout = 5
                };

                return builder.ConnectionString;
            }
            catch
            {
                return connectionString;
            }
        }

        private class SecurityRoleInfo
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public bool EnAdmin { get; set; }
            public bool EnAllControls { get; set; }
            public bool EnAllAccessForms { get; set; }
        }
    }
}
