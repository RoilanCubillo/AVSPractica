using System.Web.Mvc;

namespace UltraERP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Inicio", "DocumentosInventario");

            return RedirectToAction("Login", "Account");
        }
    }
}
