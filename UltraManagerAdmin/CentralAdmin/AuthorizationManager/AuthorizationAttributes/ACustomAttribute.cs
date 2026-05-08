using CentralAdmin.AuthorizationManager.AuthorizationUtil;
using CentralAdmin.wcfSC_Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CentralAdmin.AuthorizationManager.AuthorizationAttributes
{
    public abstract class ACustomAttribute : FilterAttribute, IAuthorizationFilter
    {
        protected readonly SecurityServicesClient _scService = new SecurityServicesClient();
        protected readonly Dictionary<string, object> Data = new Dictionary<string, object>();

        public string Permissions { get; set; }

        public void GetDataOnAuthorization(AuthorizationContext filterContext)
        {
            Data.Remove("UserIdentity");
            Data.Remove("UserID");
            Data.Add("UserIdentity", filterContext.RequestContext.HttpContext.User.Identity);
            Data.Add("UserID", Convert.ToInt32(filterContext.RequestContext.HttpContext.Session["USER_ID"]));
        }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            GetDataOnAuthorization(filterContext);
            object d1 = filterContext.HttpContext.Session["USER_AUTOID"], d2 = filterContext.HttpContext.Session["USER_ID"];

            if (AuthorizationStatus.Instance.LastStatus)
            {
                filterContext.Result = AuthorizationStatus.Instance.Result;
                return;
            } else if (Convert.ToInt32(d1) <= 0 || Convert.ToInt32(d2) <= 0)
            {
                filterContext.Result = new HttpUnauthorizedResult();
                AuthorizationStatus.Instance.Clear();
                AuthorizationStatus.Instance.Set(true, filterContext.Result);
                return;
            }

            OnAuthorizationValidate(filterContext);
        }

        protected ViewResult AuthorizationSystemView(string viewName)
        {
            return new ViewResult() { ViewName = $"~/Views/AuthorizationManager/SystemAuthorization/View/{viewName}.cshtml" };
        }

        protected ViewResult AuthorizationView(string viewName)
        {
            return new ViewResult() { ViewName = $"~/Views/AuthorizationManager/ViewAuthorization/{viewName}.cshtml" };
        }

        protected abstract void OnAuthorizationValidate(AuthorizationContext filterContext);
    }
}