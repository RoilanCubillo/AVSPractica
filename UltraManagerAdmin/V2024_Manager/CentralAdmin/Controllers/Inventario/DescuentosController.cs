using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Inventario
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize(Permissions = "inv-desc")]
    public class DescuentosController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();

        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Inventario/Descuentos.cshtml");
        }

        [WebMethod]
        public JsonResult GetAll()
        {
            try
            {
                using (_objService)
                {
                    List<EN_QuantityDiscount> list_discounts = _objService.GetAllQuantityDiscounts(0, 0).ToList();
                    return Json(new Respuesta("", "", list_discounts, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(EN_QuantityDiscount discount)
        {
            try
            {
                string validate = validateEN_QuantityDiscount(discount);
                if (String.IsNullOrEmpty(validate))
                {
                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta r = _objService.SaveQuantityDiscount(discount);
                        r.Message = Resources.messages.ResourceManager.GetString(r.Message);
                        return Json(r, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        _objService.Abort();
                        return Json(new Respuesta(System.ServiceModel.CommunicationState.Faulted.ToString(), Resources.messages.error_interno + Resources.messages.ejec_abort, null, false), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new Respuesta("Requeridos: " + validate, Resources.messages.validacion_campos_requeridos, null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno + Resources.messages.error_intento_guardar, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        private string validateEN_QuantityDiscount(EN_QuantityDiscount discount)
        {
            string validate = "";
            if (discount != null)
            {
                if (String.IsNullOrEmpty(discount.Description)) validate += "[Descripción]";
                if (discount.Type == 2 || discount.Type == 4)
                {
                    if (discount.Quantity1 <= 0) validate += "[Cantidad a Comprar a precio completo]";
                    if (discount.Quantity2 <= 0) validate += "[Cantidad a Obtener con descuento]";
                }
            }
            else validate = "[Descuento]";
            return validate;
        }
    }
}
