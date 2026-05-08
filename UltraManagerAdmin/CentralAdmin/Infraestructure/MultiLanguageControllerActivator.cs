using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CentralAdmin.Infraestructure
{
    public class MultiLanguageControllerActivator : IControllerActivator
    {
        private string FallBackLanguage = "es-CR";
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            HttpCookie cookie = requestContext.HttpContext.Request.Cookies.Get("ultraerp-language");

            if (cookie == null) cookie = new HttpCookie("ultraerp-language");

            if (String.IsNullOrEmpty(cookie.Value)) cookie.Value = FallBackLanguage;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(cookie.Value);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cookie.Value);

            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}