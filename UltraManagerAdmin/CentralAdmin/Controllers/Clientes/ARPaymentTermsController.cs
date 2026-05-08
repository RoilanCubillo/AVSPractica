using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.UltraERPServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Clientes
{
    [Authorize]
    
    public class ARPaymentTermsController : Controller
    {
      private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();
        // GET: ARPaymentTerms
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Clientes/ARPaymentTerms.cshtml");
        }

        public JsonResult GetAll()
        {
            try
            {
                using (_objService)
                {
                    List<EN_AR_PaymentTerms> list = _objService.AR_PaymentTerms_GetAll().ToList();
                    return Json(new Respuesta("", "", list, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(EN_AR_PaymentTerms paymentTerms)
        {
            try
            {
                Respuesta r = _objService.EN_AR_PAYMENTTERMS_SAVE(paymentTerms);
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