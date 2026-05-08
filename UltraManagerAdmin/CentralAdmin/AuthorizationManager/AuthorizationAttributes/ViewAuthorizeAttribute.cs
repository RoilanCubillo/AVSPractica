using CentralAdmin.AuthorizationManager.AuthorizationUtil;
using Security.EntitiesAVS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CentralAdmin.AuthorizationManager.AuthorizationAttributes
{
    public class ViewAuthorizeAttribute : ACustomAttribute
    {
        protected override void OnAuthorizationValidate(AuthorizationContext filterContext)
        {
            if (!String.IsNullOrEmpty(Permissions))
            {
                EN_SC_View[] views = (EN_SC_View[])filterContext.RequestContext.HttpContext.Session["USER_VIEWS"];
                EN_SC_Module[] modules = (EN_SC_Module[])filterContext.RequestContext.HttpContext.Session["USER_MODULES"];

                if (Array.Exists(views, v => v.Code == Permissions && Array.Exists(modules, m => m.ID == v.ModuleID)))
                    return;
            }

            filterContext.Result = AuthorizationView("Unauthorized");

            AuthorizationStatus.Instance.Clear();
            AuthorizationStatus.Instance.Set(true, filterContext.Result);
        }
    }
}