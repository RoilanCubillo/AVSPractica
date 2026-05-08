using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using CentralAdmin.wcfSC_Security;
using Security.Entities;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Inicio
{
    public class InicioController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();
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
            if (User.Identity.IsAuthenticated)
            {
                if (_objSCService.User_ValidateUserSession(Convert.ToInt32(Session["USER_ID"]), Convert.ToInt32(Session["USER_AUTOID"]), SystemID))
                    return Json(1, JsonRequestBehavior.AllowGet);
                
                LogoutSession();

                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(-1, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult Login(string username, string password, string ReturnURL, string remember)
        //{
        //    try
        //    {
        //        if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
        //        {
        //            (EN_User, int) result = _objService.EN_User_ValidateUser(username, password);

        //            if (result.Item1 != null && result.Item2 == 0)
        //            {
        //                (EN_SC_User, int, string) scResult = _objSCService.User_ValidateUser(username, result.Item1.ID, SystemID);

        //                if (scResult.Item1 != null && scResult.Item2 == 1 && scResult.Item3 == "") // Verificar acceso según mandato de seguridad
        //                {
        //                    FormsAuthentication.SetAuthCookie(username, !String.IsNullOrEmpty(remember) && remember == "on");

        //                    setUserSession(result.Item1, scResult.Item1);

        //                    if (!String.IsNullOrEmpty(ReturnURL)) return Redirect(ReturnURL);

        //                    return RedirectToAction("Manager", "Inicio");
        //                }
        //                else if (scResult.Item2 == 0 && scResult.Item3 != "") // En Caso de error por parte del servicio
        //                {
        //                    ViewBag.ErrorLogin = $"SC. {Resources.messages.error_loggin}";
        //                    return Login(ReturnURL);
        //                }
        //                else // Usuario no configurado o no tiene los permisos correspondiente para acceder al sistema
        //                {
        //                    ViewBag.ErrorLogin = $"SC. {Resources.messages.error_loggin_unauthorized}";
        //                    return Login(ReturnURL);
        //                }
        //            }
        //            else if (result.Item2 == -1) ViewBag.ErrorLogin = Resources.messages.error_loggin;
        //            else ViewBag.ErrorLogin = Resources.messages.error_loggin_credenciales;
        //        }
        //        else ViewBag.ErrorLogin = Resources.messages.error_loggin_credenciales;

        //        return Login(ReturnURL);
        //    }
        //    catch (Exception e)
        //    {
        //        ViewBag.ErrorLogin = Resources.messages.error_loggin;

        //        return Login(ReturnURL);
        //    }
        //}

        [HttpPost]
        public ActionResult Login(string username, string password, string ReturnURL, string remember)
        {
            try
            {
                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {
                    var response = _objService.EN_User_ValidateUser(username, password);

                    EN_User resultUser = response.User;
                    int resultStatus = response.Status;

                    if (resultUser != null && resultStatus == 0)
                    {
                        (EN_SC_User, int, string) scResult = _objSCService.User_ValidateUser(username, resultUser.ID, SystemID);

                        if (scResult.Item1 != null && scResult.Item2 == 1 && scResult.Item3 == "")
                        {
                            FormsAuthentication.SetAuthCookie(username, !String.IsNullOrEmpty(remember) && remember == "on");

                            setUserSession(resultUser, scResult.Item1);

                            if (!String.IsNullOrEmpty(ReturnURL)) return Redirect(ReturnURL);

                            return RedirectToAction("Manager", "Inicio");
                        }
                        else if (scResult.Item2 == 0 && scResult.Item3 != "")
                        {
                            ViewBag.ErrorLogin = $"SC. {Resources.messages.error_loggin}";
                            return Login(ReturnURL);
                        }
                        else
                        {
                            ViewBag.ErrorLogin = $"SC. {Resources.messages.error_loggin_unauthorized}";
                            return Login(ReturnURL);
                        }
                    }
                    else if (resultStatus == -1)
                    {
                        ViewBag.ErrorLogin = Resources.messages.error_loggin;
                    }
                    else
                    {
                        ViewBag.ErrorLogin = Resources.messages.error_loggin_credenciales;
                    }
                }
                else
                {
                    ViewBag.ErrorLogin = Resources.messages.error_loggin_credenciales;
                }

                return Login(ReturnURL);
            }
            catch (Exception e)
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

        private void LogoutSession()
        {
            FormsAuthentication.SignOut();

            clearUserSessions();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
        }

        private void setUserSession(EN_User user, EN_SC_User scUser)
        {
            Session["USER_ID"] = scUser.ID;
            Session["USER_AUTOID"] = user.ID;
            Session["USER_NAME"] = user.Name;
            Session["USER_EMAIL"] = user.EmailAddress;
            Session["USER_ACCOUNT"] = user.Account;
            Session["USER_MODULES"] = _objSCService.Module_ValidateModules(scUser.ID, SystemCode);
            Session["USER_VIEWS"] = _objSCService.View_ValidateViews(scUser.ID, SystemCode);
            Session["USER_DATAACCESS"] = _objSCService.DataAccess_ValidateDataAccess(scUser.ID, SystemCode, "%");
        }

        private void clearUserSessions()
        {
            Session["USER_ID"] = 0;
            Session["USER_AUTOID"] = 0;
            Session["USER_NAME"] = "";
            Session["USER_EMAIL"] = "";
            Session["USER_ACCOUNT"] = "";
            Session["USER_MODULES"] = null;
            Session["USER_VIEWS"] = null;

            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
        }
    }
}