using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace UltraERP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            HttpAntiForgeryException antiForgeryException = exception as HttpAntiForgeryException;

            if (antiForgeryException == null && exception != null)
                antiForgeryException = exception.GetBaseException() as HttpAntiForgeryException;

            if (antiForgeryException == null)
                return;

            try
            {
                FormsAuthentication.SignOut();

                if (Session != null)
                {
                    Session.Clear();
                    Session.RemoveAll();
                    Session.Abandon();
                }

                Response.Clear();
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            }
            finally
            {
                Server.ClearError();
                Response.Redirect("~/Account/Login?reason=token", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}
