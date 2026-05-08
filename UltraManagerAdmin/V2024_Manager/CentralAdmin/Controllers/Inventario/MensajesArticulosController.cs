
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
    [ViewAuthorize(Permissions = "inv-msj-articulos")]
    public class MensajesArticulosController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();
        // GET: MensajesArticulos
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Inventario/MensajesArticulos.cshtml");
        }

        [WebMethod]
        public JsonResult GetAll()
        {
            try
            {
                var resp = _objService.GetAllItemMessage("", 0, 0);
                if (resp != null) return Json(new Respuesta("","",resp, true), JsonRequestBehavior.AllowGet);
                else return Json(new Respuesta("RESULTADO NULLO", Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(EN_ItemMessage itemMessage)
        {
            try
            {
                string validate = validateEN_ItemMessage(itemMessage);
                if (String.IsNullOrEmpty(validate))
                {
                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta r = _objService.SaveItemMessage(itemMessage);
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

        private string validateEN_ItemMessage(EN_ItemMessage itemMessage)
        {
            string validate = "";
            if (itemMessage != null)
            {
                if (String.IsNullOrEmpty(itemMessage.Title)) validate += "[TITULO],";
                if (String.IsNullOrEmpty(itemMessage.Message)) validate += "[MENSAJE],";
                if (itemMessage.AgeLimit <= 0) validate += "[LIMITE DE EDAD]";
            }
            else validate += "[MENSAJE DE ARTÍCULO]";
            return validate;
        }
    }
}