using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Security.EntitiesAVS;
using Security.Logic;
using UltraERP.BusinessEntities;
using UltraERP.BusinessLogic;
using UltraERP.Models;
using UltraERP.Services;

namespace UltraERP.Controllers

{
    public class AccountController : Controller
    {
        private int SystemID
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["SystemID"] ?? "1"); }
        }

        private string SystemCode
        {
            get { return ConfigurationManager.AppSettings["SystemCode"] ?? "UltraERP"; }
        }

        public ActionResult Login(string returnUrl)
        {
            DisableResponseCache();

            if (Request.IsAuthenticated)
                return RedirectToAction("Inicio", "DocumentosInventario");

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
                LoginMessage = String.Equals(Request.QueryString["reason"], "token", StringComparison.OrdinalIgnoreCase)
                    ? "La sesion o el formulario anterior ya no eran validos. Ingrese de nuevo para continuar."
                    : ""
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.UserName = (model.UserName ?? "").Trim();
                var userLogic = new CT_User();
                var securityUserLogic = new CT_SC_User();

                if (String.IsNullOrWhiteSpace(model.UserName) || String.IsNullOrWhiteSpace(model.Password))
                {
                    AddLoginFailure(model, "Campos requeridos", "Ingrese usuario y contrasena.");
                    return View(model);
                }

                var userResult = userLogic.ValidateUser(model.UserName, model.Password);
                if (userResult.Item1 == null || userResult.Item2 != 0)
                {
                    AddLoginFailure(model, userResult.Item2 == 2 ? "Credenciales invalidas" : "Usuario no encontrado", "Usuario o contrasena incorrectos.");
                    return View(model);
                }

                EN_SC_Company[] companies = (securityUserLogic.GetCompaniesByUser(model.UserName, userResult.Item1.ID, SystemID) ?? new System.Collections.Generic.List<EN_SC_Company>()).ToArray();
                if (companies.Length == 0)
                {
                    AddLoginFailure(model, "Sin empresas autorizadas", "El usuario existe, pero no tiene empresas autorizadas para UltraERP.");
                    return View(model);
                }

                int selectedCompanyId;
                if (model.CompanyId.HasValue && companies.Any(c => c.ID_Company == model.CompanyId.Value))
                {
                    selectedCompanyId = model.CompanyId.Value;
                }
                else if (companies.Length == 1)
                {
                    selectedCompanyId = companies[0].ID_Company;
                }
                else
                {
                    PrepareCompanySelection(model, companies, "Seleccione una empresa para continuar.");
                    return View(model);
                }

                var securityResult = securityUserLogic.ValidateUserCompany(model.UserName, userResult.Item1.ID, SystemID, selectedCompanyId);
                if (securityResult.Item1 != null && securityResult.Item2 == 1 && String.IsNullOrEmpty(securityResult.Item3))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    SetUserSession(userResult.Item1, securityResult.Item1, companies.First(c => c.ID_Company == selectedCompanyId));

                    if (!String.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);

                    return RedirectToAction("Inicio", "DocumentosInventario");
                }

                AddLoginFailure(model, "Empresa no autorizada", "El usuario no esta autorizado para la empresa seleccionada.");
                PrepareCompanySelection(model, companies, "El usuario no esta autorizado para la empresa seleccionada.");
                return View(model);
            }
            catch (Exception e)
            {
                ErrorLogService.Log(e, "Login", "Validar usuario", new { Usuario = model == null ? "" : model.UserName });
                ModelState.AddModelError("", GetFriendlyLoginError(e));
                return View(model);
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            ClearUserSessions();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        private static void PrepareCompanySelection(LoginViewModel model, EN_SC_Company[] companies, string message)
        {
            model.Password = "";
            model.RequireCompanySelection = true;
            model.LoginMessage = message;
            model.Companies = companies
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.ID_Company.ToString(),
                    Selected = model.CompanyId == c.ID_Company
                })
                .ToArray();
        }

        private void AddLoginFailure(LoginViewModel model, string reason, string message)
        {
            string userName = model == null ? "" : (model.UserName ?? "").Trim();
            ErrorLogService.LogMessage("Intento fallido de login: " + reason, "Login", "Intento fallido", new
            {
                Usuario = userName,
                Motivo = reason,
                Empresa = model == null ? null : model.CompanyId
            });

            ModelState.AddModelError("", message);
        }

        private static string GetFriendlyLoginError(Exception exception)
        {
            Exception baseException = exception == null ? null : exception.GetBaseException();
            string message = baseException == null ? "" : baseException.Message;

            if (baseException is SqlException || message.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0)
                return "No se pudo conectar con la base de seguridad. Intente de nuevo en unos segundos.";

            if (message.IndexOf("network-related", StringComparison.OrdinalIgnoreCase) >= 0 ||
                message.IndexOf("server was not found", StringComparison.OrdinalIgnoreCase) >= 0)
                return "La base de seguridad no esta disponible en este momento.";

            if (message.IndexOf("connection from the pool", StringComparison.OrdinalIgnoreCase) >= 0 ||
                message.IndexOf("max pool size", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Hay demasiadas conexiones abiertas en este momento. Intente de nuevo en unos segundos.";

            return "No se pudo validar el usuario. Revise sus credenciales o intente de nuevo.";
        }

        private void SetUserSession(EN_User user, EN_SC_User scUser, EN_SC_Company company)
        {
            Session["USER_ID"] = scUser.ID;
            Session["USER_AUTOID"] = user.ID;
            Session["USER_NAME"] = user.Name;
            Session["USER_EMAIL"] = user.EmailAddress;
            Session["USER_ACCOUNT"] = user.Account;
            Session["USER_COMPANY_ID"] = scUser.ID_Company;
            Session["USER_COMPANY_CODE"] = company.Code;
            Session["USER_COMPANY_NAME"] = company.Name;
            Session["USER_MODULES"] = new CT_SC_Module().ValidateModules(scUser.ID, SystemCode).ToArray();
            Session["USER_VIEWS"] = new CT_SC_View().ValidateViews(scUser.ID, SystemCode).ToArray();
            Session["USER_DATAACCESS"] = new CT_SC_DataAccess().ValidateDataAccess(scUser.ID, SystemCode, "%").ToArray();
            Session["USER_ROLE"] = "";
            Session["USER_ROLE_SOURCE"] = "";
            Session["USER_SECURITY_ROLES"] = "";
        }

        private void ClearUserSessions()
        {
            Session["USER_ID"] = 0;
            Session["USER_AUTOID"] = 0;
            Session["USER_NAME"] = "";
            Session["USER_EMAIL"] = "";
            Session["USER_ACCOUNT"] = "";
            Session["USER_COMPANY_ID"] = 0;
            Session["USER_COMPANY_CODE"] = "";
            Session["USER_COMPANY_NAME"] = "";
            Session["USER_MODULES"] = null;
            Session["USER_VIEWS"] = null;
            Session["USER_DATAACCESS"] = null;
            Session["USER_ROLE"] = "";
            Session["USER_ROLE_SOURCE"] = "";
            Session["USER_SECURITY_ROLES"] = "";

            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
        }

        private void DisableResponseCache()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        }
    }
}
