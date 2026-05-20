using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace UltraERP.Services
{
    public static class ErrorLogService
    {
        private const int MaxFieldLength = 4000;

        public static void Log(Exception exception, string screen, string action, object data)
        {
            if (exception == null)
                return;

            try
            {
                if (TryLogToSql(exception, screen, action, data))
                    return;
            }
            catch
            {
            }

            try
            {
                LogToFile(exception, screen, action, data);
            }
            catch
            {
            }
        }

        public static void LogMessage(string message, string screen, string action, object data)
        {
            if (String.IsNullOrWhiteSpace(message))
                return;

            Log(new Exception(message), screen, action, data);
        }

        private static bool TryLogToSql(Exception exception, string screen, string action, object data)
        {
            if (ShouldSkipSqlLogging(exception))
                return false;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];
            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return false;

            using (var connection = new SqlConnection(BuildFastFailConnectionString(settings.ConnectionString)))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 3;
                command.CommandText = @"
IF OBJECT_ID('dbo.UEP_ERROR_LOG', 'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.UEP_ERROR_LOG
    (
        UserID,
        UserName,
        UserAccount,
        Screen,
        ActionName,
        ErrorMessage,
        ErrorDetail,
        RequestData,
        Url,
        CreatedAt
    )
    VALUES
    (
        @UserID,
        @UserName,
        @UserAccount,
        @Screen,
        @ActionName,
        @ErrorMessage,
        @ErrorDetail,
        @RequestData,
        @Url,
        GETDATE()
    );
    SELECT CAST(1 AS BIT);
END
ELSE
BEGIN
    SELECT CAST(0 AS BIT);
END";

                command.Parameters.AddWithValue("@UserID", GetCurrentUserID());
                command.Parameters.AddWithValue("@UserName", Limit(GetCurrentUserName(), 150));
                command.Parameters.AddWithValue("@UserAccount", Limit(GetCurrentUserAccount(), 80));
                command.Parameters.AddWithValue("@Screen", Limit(screen, 120));
                command.Parameters.AddWithValue("@ActionName", Limit(action, 120));
                command.Parameters.AddWithValue("@ErrorMessage", Limit(exception.GetBaseException().Message, MaxFieldLength));
                command.Parameters.AddWithValue("@ErrorDetail", Limit(exception.ToString(), MaxFieldLength));
                command.Parameters.AddWithValue("@RequestData", Limit(SerializeData(data), MaxFieldLength));
                command.Parameters.AddWithValue("@Url", Limit(GetCurrentUrl(), 500));

                connection.Open();
                object result = command.ExecuteScalar();
                return result != null && Convert.ToBoolean(result);
            }
        }

        private static bool ShouldSkipSqlLogging(Exception exception)
        {
            string message = exception == null ? "" : exception.ToString();
            return message.IndexOf("connection from the pool", StringComparison.OrdinalIgnoreCase) >= 0
                || message.IndexOf("max pool size", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string BuildFastFailConnectionString(string connectionString)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    ConnectTimeout = 3
                };

                return builder.ConnectionString;
            }
            catch
            {
                return connectionString;
            }
        }

        private static void LogToFile(Exception exception, string screen, string action, object data)
        {
            HttpContext context = HttpContext.Current;
            string folder = context == null
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "ErrorLogs")
                : context.Server.MapPath("~/App_Data/ErrorLogs");

            Directory.CreateDirectory(folder);

            string file = Path.Combine(folder, DateTime.Now.ToString("yyyyMMdd") + ".log");
            var builder = new StringBuilder();
            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.AppendLine("Usuario: " + GetCurrentUserName() + " (" + GetCurrentUserAccount() + ")");
            builder.AppendLine("Pantalla: " + (screen ?? ""));
            builder.AppendLine("Accion: " + (action ?? ""));
            builder.AppendLine("Url: " + GetCurrentUrl());
            builder.AppendLine("Datos: " + SerializeData(data));
            builder.AppendLine("Error: " + exception);

            File.AppendAllText(file, builder.ToString(), Encoding.UTF8);
        }

        private static string SerializeData(object data)
        {
            if (data == null)
                return "";

            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch
            {
                return Convert.ToString(data);
            }
        }

        private static int GetCurrentUserID()
        {
            HttpContext context = HttpContext.Current;
            if (context == null || context.Session == null || context.Session["USER_AUTOID"] == null)
                return 0;

            int id;
            return Int32.TryParse(Convert.ToString(context.Session["USER_AUTOID"]), out id) ? id : 0;
        }

        private static string GetCurrentUserName()
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

        private static string GetCurrentUserAccount()
        {
            HttpContext context = HttpContext.Current;
            if (context == null || context.Session == null || context.Session["USER_ACCOUNT"] == null)
                return "";

            return Convert.ToString(context.Session["USER_ACCOUNT"]).Trim();
        }

        private static string GetCurrentUrl()
        {
            HttpContext context = HttpContext.Current;
            if (context == null || context.Request == null || context.Request.Url == null)
                return "";

            return context.Request.Url.ToString();
        }

        private static string Limit(string value, int maxLength)
        {
            value = value ?? "";
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
