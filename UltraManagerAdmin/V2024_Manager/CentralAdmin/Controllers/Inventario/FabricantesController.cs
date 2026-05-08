using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Inventario
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize(Permissions = "inv-fabricantes")]
    public class FabricantesController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();

        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Inventario/Fabricantes.cshtml");
        }

        public JsonResult GetInitData()
        {
            try
            {
                using (_objService)
                {
                    List<EN_ExtCentral_Manufacturer> list = _objService.GetAllManufacturers().ToList();

                    return Json(new Respuesta("", "", new { manufacturers = list }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(EN_ExtCentral_Manufacturer manufacturer)
        {
            try
            {
                Respuesta r = _objService.SaveManufacturer(manufacturer.ID, manufacturer.Description);
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