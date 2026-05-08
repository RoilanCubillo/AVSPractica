using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.UltraERPServiceClient;
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
    [ViewAuthorize(Permissions = "cxc-esps-frios")]
    public class EspaciosfriosController : Controller
    {
        // GET: Espacios_frios
         private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Proveedores/Espaciosfrios.cshtml");
        }

        public JsonResult GetAll()
        {
            try
            {
                using (_objService)
                {

                    var supp = _objService.GetAllSupplier("%", "", 0, 0).ToList();
                    var st = _objService.GetAllStore(StoreAuthorizationAttribute.StoresAvailable(Session), 0, 0).ToList();
                    var espfriosTienda = _objService.GetAllEspaciosFriosTienda().ToList();
                   // var espfriosTienda = new EN_EspaciosFriosTienda();
                    var espfrios = _objService.GetAllEspaciosFrios().ToList();
                    //var impuesto = new EN_Tax();
                    var impuesto = _objService.GetAllTax("", 0, 0);
                    //var espfrios = new EN_EspaciosFrios();
                    return Json(new Respuesta("", "", new { store = st, supplier = supp, espFrios = espfrios, espTienda = espfriosTienda, tax = impuesto }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [WebMethod]
        public JsonResult GetRefresh()
        {
            try
            {
                using (_objService)
                {
                    var espfriosTienda = _objService.GetAllEspaciosFriosTienda().ToList();
                    var espfrios = _objService.GetAllEspaciosFrios().ToList();
                    return Json(new Respuesta("", "", new { espFrios = espfrios, espTienda = espfriosTienda }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [WebMethod]
        public JsonResult GetListItem(int id)
        {
            try
            {
                using (_objService)
                {
                    var data = _objService.GetAllItem_Simple(id).ToList();

                    List<object> list = new List<object>();

                    data.ForEach(x => list.Add(new { ID = x.ID, Description = x.Description }));

                    return Json(new { items = list }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message });
            }
        }

        [WebMethod]
        public JsonResult GetListEspaciosFrios(int id)
        {
            try
            {
                using (_objService)
                {
                    var data = _objService.GetAllEspaciosFriosXTienda(id).ToList();

                    List<object> list = new List<object>();

                    data.ForEach(x => list.Add(new { ID = x.ID, Camara = x.Camara, Puerta = x.Puerta, NumParrilla = x.NumParrillas, Monto = x.Monto }));

                    return Json(new { espacios = list }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message });
            }
        }


        [HttpPost]
        public JsonResult SaveEspaciosFrios(EN_EspaciosFrios espfrio)
        {
            try
            {
                string validate = validateEN_EspaciosFrios(espfrio);
                if (String.IsNullOrEmpty(validate))
                {
                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta r = _objService.SaveEspaciosFrios(espfrio);
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

        [HttpPost]
        public JsonResult SaveEspaciosFriosTienda(EN_EspaciosFriosTienda espfrio)
        {
            try
            {
                string validate = validateEN_EspaciosFriosXTienda(espfrio);
                if (String.IsNullOrEmpty(validate))
                {
                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta r = _objService.SaveEspaciosFriosTienda(espfrio);
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

        public JsonResult GetAllcategories()
        {
            try
            {
                using (_objService)
                {
                    List<EN_Category> list_categories = _objService.GetAllCategories("", 0, 0).ToList();
                    List<EN_Department> list_departments = _objService.GetAllDepartments("", 0, 0).ToList();
                    return Json(new Respuesta("", "", new { categories = list_categories, departments = list_departments }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllcategories_BY_Supplier(int supplierID)
        {
            try
            {
                using (_objService)
                {
                    List<EN_Category> list_categories = _objService.GetAllCategory_Simple(supplierID).ToList();
                    return Json(new Respuesta("", "", list_categories, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        private string validateEN_EspaciosFrios(EN_EspaciosFrios espFrio)
        {
            if (espFrio != null)
            {
                string resp = "";

                if (espFrio.StoreId <= 0) resp += "[Tienda],";                
                if (String.IsNullOrEmpty(espFrio.Camara)) resp += "[Camara],";
                if (espFrio.Puerta <= 0) resp += "[Puerta],";
                if (espFrio.NumParrillas <= 0) resp += "[Parrillas],";
                if (String.IsNullOrEmpty(espFrio.Dimension)) resp += "[Dimensiones],";
                if (String.IsNullOrEmpty(espFrio.Ubicacion)) resp += "[Ubicacion],";
                return resp;
            }
            else return "[EspacioFrio vacio]";
        }

        private string validateEN_EspaciosFriosXTienda(EN_EspaciosFriosTienda espFrio)
        {
            if (espFrio != null)
            {
                string resp = "";

                if (espFrio.EspacioId <= 0) resp += "[EspacioFrio],";
                if (espFrio.SupplierId <= 0) resp += "[Proveedor],";               
                if (espFrio.Categoria <= 0) resp += "[Categoria],";              
               
                return resp;
            }
            else return "[EspacioFrio vacio]";
        }


    }
}
