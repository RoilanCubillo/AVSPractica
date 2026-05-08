using CentralAdmin.AuthorizationManager.AuthorizationUtil;
using CentralAdmin.wcfSC_Security;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace CentralAdmin.AuthorizationManager.AuthorizationAttributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            object d1 = filterContext.HttpContext.Session["USER_AUTOID"], d2 = filterContext.HttpContext.Session["USER_ID"];

            base.OnAuthorization(filterContext);
            
            AuthorizationStatus.Instance.Clear();

            if (filterContext.Result != null)
                AuthorizationStatus.Instance.Set(true, filterContext.Result);
            else
            {
                if (Convert.ToInt32(d1) <= 0 || Convert.ToInt32(d2) <= 0)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    AuthorizationStatus.Instance.Set(true, filterContext.Result);
                }
            }

            return;
        }
    }
}