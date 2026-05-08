using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.UltraERPServiceClient;
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
    [ViewAuthorize(Permissions = "inv-familias")]
    public class FamiliasController : Controller
    {
         private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();
        
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Inventario/Familias.cshtml");
        }

        public JsonResult GetAll()
        {
            try
            {
                using (_objService)
                {
                    List<EN_ExtCentral_Family> list = _objService.GetAllFamilies("", 0, 0).ToList();
                    return Json(new Respuesta("", "", list, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(EN_ExtCentral_Family family)
        {
            try
            {
                string validate = validateEntity(family);
                if (String.IsNullOrEmpty(validate))
                {
                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta r = _objService.SaveFamily(family);
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

        private string validateEntity(EN_ExtCentral_Family entity)
        {
            string validate = "";
            if (entity != null)
            {
                if (String.IsNullOrEmpty(entity.Name)) validate += "[Name]";
                if (String.IsNullOrEmpty(entity.Code)) validate += "[Code]";
            }
            else validate = "[Family]";
            return validate;
        }
    }
}