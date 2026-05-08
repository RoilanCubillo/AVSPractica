using CentralAdmin.Infraestructure;
using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(CentralAdmin.Startup))]

namespace CentralAdmin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory(new MultiLanguageControllerActivator()));
            // Para obtener más información sobre cómo configurar la aplicación, visite https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
