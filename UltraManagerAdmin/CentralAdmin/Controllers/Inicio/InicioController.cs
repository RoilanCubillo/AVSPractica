using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.UltraERPServiceClient;
using CentralAdmin.wcfSC_Security;
using Security.EntitiesAVS;
using System;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Inicio
{
    public class InicioController : Controller
    {
        private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();
        private readonly SecurityServicesClient _objSCService = new SecurityServicesClient();
        private readonly int SystemID = Convert.ToInt32(WebConfigurationManager.AppSettings["SystemID"]);
        private readonly string SystemCode = WebConfigurationManager.AppSettings["SystemCode"];

        [HttpGet]
        public ActionResult Login(string ReturnURL = null)
        {
            ViewBag.ReturnURL = ReturnURL;
            return View("~/Views/Login/Login.cshtml");
        }

        [Authorize]
        [CustomAuthorize]
        [SystemAuthorize]
        public ActionResult Manager()
        {
            return View("~/Views/Manager/Inicio.cshtml");
        }

        [HttpPost]
        public ActionResult ValidateSession()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }

            int userId = Convert.ToInt32(Session["USER_ID"]);
            int userAutoId = Convert.ToInt32(Session["USER_AUTOID"]);
            int companyId = Convert.ToInt32(Session["USER_COMPANY_ID"] ?? 0);

            bool isValid = companyId > 0
                ? _objSCService.User_ValidateUserSessionCompany(userId, userAutoId, SystemID, companyId)
                : _objSCService.User_ValidateUserSession(userId, userAutoId, SystemID);

            if (isValid)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }

            LogoutSession();
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string ReturnURL, string remember, int? companyId)
        {
            try
            {
                if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                {
                    ViewBag.ErrorLogin = Resources.messages.error_loggin_credenciales;
                    return Login(ReturnURL);
                }

                (EN_User, int) result = _objService.EN_User_ValidateUser(username, password);

                if (result.Item1 == null || result.Item2 != 0)
                {
                    ViewBag.ErrorLogin = result.Item2 == -1
                        ? Resources.messages.error_loggin
                        : Resources.messages.error_loggin_credenciales;
                    return Login(ReturnURL);
                }

                EN_SC_Company[] companies = _objSCService.User_GetCompanies(username, result.Item1.ID, SystemID) ?? new EN_SC_Company[0];

                if (companies.Length == 0)
                {
                    ViewBag.ErrorLogin = $"SC. {Resources.messages.error_loggin_unauthorized}";
                    return Login(ReturnURL);
                }

                int selectedCompanyId;
                if (companyId.HasValue && companies.Any(c => c.ID_Company == companyId.Value))
                {
                    selectedCompanyId = companyId.Value;
                }
                else if (companies.Length == 1)
                {
                    selectedCompanyId = companies[0].ID_Company;
                }
                else
                {
                    PrepareCompanySelectionView(username, ReturnURL, remember, companies, companyId);
                    ViewBag.ErrorLogin = "Seleccione una empresa y vuelva a ingresar la contrasena.";
                    return View("~/Views/Login/Login.cshtml");
                }

                (EN_SC_User, int, string) scResult = _objSCService.User_ValidateUserCompany(username, result.Item1.ID, SystemID, selectedCompanyId);

                if (scResult.Item1 != null && scResult.Item2 == 1 && scResult.Item3 == "")
                {
                    FormsAuthentication.SetAuthCookie(username, !String.IsNullOrEmpty(remember) && remember == "on");
                    SetUserSession(result.Item1, scResult.Item1, companies.First(c => c.ID_Company == selectedCompanyId));

                    if (!String.IsNullOrEmpty(ReturnURL))
                    {
                        return Redirect(ReturnURL);
                    }

                    return RedirectToAction("Manager", "Inicio");
                }

                ViewBag.ErrorLogin = scResult.Item2 == 0 && !String.IsNullOrEmpty(scResult.Item3)
                    ? $"SC. {Resources.messages.error_loggin}"
                    : $"SC. {Resources.messages.error_loggin_unauthorized}";

                PrepareCompanySelectionView(username, ReturnURL, remember, companies, selectedCompanyId);
                return View("~/Views/Login/Login.cshtml");
            }
            catch (Exception)
            {
                ViewBag.ErrorLogin = Resources.messages.error_loggin;
                return Login(ReturnURL);
            }
        }

        [HttpGet]
        public void Logout()
        {
            LogoutSession();
            Response.Redirect(FormsAuthentication.LoginUrl);
        }

        private void PrepareCompanySelectionView(string username, string returnUrl, string remember, EN_SC_Company[] companies, int? selectedCompanyId)
        {
            ViewBag.ReturnURL = returnUrl;
            ViewBag.Companies = companies;
            ViewBag.Username = username;
            ViewBag.Remember = remember;
            ViewBag.SelectedCompanyId = selectedCompanyId;
        }

        private void LogoutSession()
        {
            FormsAuthentication.SignOut();
            ClearUserSessions();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
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
            Session["USER_MODULES"] = _objSCService.Module_ValidateModules(scUser.ID, SystemCode);
            Session["USER_VIEWS"] = _objSCService.View_ValidateViews(scUser.ID, SystemCode);
            Session["USER_DATAACCESS"] = _objSCService.DataAccess_ValidateDataAccess(scUser.ID, SystemCode, "%");
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

            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
        }
    }
}
