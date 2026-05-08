using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Proveedores
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize(Permissions = "cxc-esps-isla-side")]
    public class IslaSideController : Controller
    {
        // GET: islas

        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Proveedores/IslaSide.cshtml");
        }

        
        public JsonResult GetInitialData()
        {
            var supp = _objService.GetAllSupplier("%", "", 0, 0);
            var st = _objService.GetAllStore(StoreAuthorizationAttribute.StoresAvailable(Session), 0, 0);
            var impuesto = _objService.GetAllTax("", 0, 0);
            var islasSide = _objService.GetAllIslasSideKick();  



            return Json(new { store = st, supplier = supp, tax = impuesto, islaSide = islasSide }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAll()
        {
            try
            {
                using (_objService)
                {
                    
                    var supp = _objService.GetAllSupplier("%", "", 0, 0).ToList();
                    var st = _objService.GetAllStore(StoreAuthorizationAttribute.StoresAvailable(Session), 0, 0).ToList();
                    var impuesto = _objService.GetAllTax("", 0, 0).ToList();
                    var islasSide = _objService.GetAllIslasSideKick().ToList();
                    return Json(new Respuesta("", "", new { store = st, supplier = supp, tax = impuesto, islaSide = islasSide }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetRefresh()
        {
            try
            {
                using (_objService)
                {
                    var islasSide = _objService.GetAllIslasSideKick().ToList();
                    return Json(new Respuesta("", "", new { islaSide = islasSide }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [WebMethod]
        public JsonResult GetIslaSide(int ID)
        {
            try
            {
                Respuesta respuesta = _objService.GetIslasSideKick(ID);
                if (!respuesta.Status) respuesta.Message = Resources.messages.ResourceManager.GetString(respuesta.Message);
                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_reg, null, false), JsonRequestBehavior.AllowGet);
            }
        }


       
        [HttpPost]
        public JsonResult SaveIslasSide(EN_IslasSideKick islasSide)
        {
            try
            {
               
                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta r = _objService.SaveIslasSideKick(islasSide);
                        r.Message = Resources.messages.ResourceManager.GetString(r.Message);
                        return Json(r, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        _objService.Abort();
                        return Json(new Respuesta(System.ServiceModel.CommunicationState.Faulted.ToString(), Resources.messages.error_interno + Resources.messages.ejec_abort, null, false), JsonRequestBehavior.AllowGet);
                    }
                
               // return Json(new Respuesta("Requeridos: " + validate, Resources.messages.validacion_campos_requeridos, null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno + Resources.messages.error_intento_guardar, null, false), JsonRequestBehavior.AllowGet);
            }
        }


        private string validateEN_IslasSideKick(EN_IslasSideKick islasSide)
        {
            if (islasSide != null)
            {
                string resp = "";

                if (islasSide.IDStore <= 0) resp += "[Tienda],";
                if (islasSide.IDSupplier <= 0) resp += "[Proveedor],";               
                if (String.IsNullOrEmpty(islasSide.Dinamica)) resp += "[Dinamica],";             
                return resp;
            }
            else return "[islaside nulo]";
        }


       
    }


}
