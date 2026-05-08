using CentralAdmin.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CentralAdmin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["ID_CLIENTE_ORIGEN"] = 0;
            Session["USER_ID"] = 0;
            Session["USER_AUTOID"] = 0;
            Session["USER_NAME"] = "";
            Session["USER_EMAIL"] = "";
            Session["USER_ACCOUNT"] = "";
            Session["USER_MODULES"] = null;
            Session["USER_VIEWS"] = null;
            Session["USER_DATAACCESS"] = null;
            Session["WORKSHEET_CONTENT_ID"] = null;
        }
    }
}
