using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Compra
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize(Permissions = "comp-casasc")]
    public class CasasComercialesController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();

        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Compra/CasasComerciales.cshtml");
        }

        public JsonResult GetInitData()
        {
            try
            {

                List<EN_Purchaser> list = _objService.GetAllPurchasers().ToList();

                return Json(new Respuesta("", "", new { purchasers = list }, true), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(EN_Purchaser purchaser)
        {
            try
            {
                Respuesta r = _objService.SavePurchaser(purchaser);
                r.Message = Resources.messages.ResourceManager.GetString(r.Message);
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno + Resources.messages.error_intento_guardar, null, false), JsonRequestBehavior.AllowGet);
            }
        }
    }
}