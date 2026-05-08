using CentralAdmin.AuthorizationManager.AuthorizationUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using static Security.Entities.SC_Enum;

namespace CentralAdmin.AuthorizationManager.AuthorizationAttributes
{
    public class SystemAuthorizeAttribute : ACustomAttribute
    {
        protected override void OnAuthorizationValidate(AuthorizationContext filterContext)
        {
            var result = _scService.System_ValidateSystem((int)Data["UserID"], WebConfigurationManager.AppSettings["SystemCode"]);

            AuthorizationStatus.Instance.SystemValidateStatus = result.Item1;

            switch (result.Item1)
            {
                case ESystemValidateStatus.ALLOW_ACCESS: break;
                default: filterContext.Result = AuthorizationSystemView("AuthorizationManagerView"); break;
            }

            return;
        }
    }
}