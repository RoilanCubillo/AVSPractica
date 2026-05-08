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
    [ViewAuthorize(Permissions = "cxc-esps-cabeceras")]
    public class CabecerasController : Controller
    {
        // GET: Cabeceras
         private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Proveedores/Cabeceras.cshtml");
        }


        [WebMethod]
        public JsonResult GetListStore()
        {
            try
            {
                using (_objService)
                {
                    return Json(_objService.GetAllStore ("", 0, 0).ToList(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message });
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
        public JsonResult GetListCabeceras(int id)
        {
            try
            {
                using (_objService)
                {
                    var data = _objService.GetAllCabecerasXTienda(id).ToList();

                    List<object> list = new List<object>();

                    data.ForEach(x => list.Add(new { ID = x.ID, Num = x.NumCab, Precio = x.Monto}));

                    return Json(new { cabeceras = list }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message });
            }
        }

        [WebMethod]
        public JsonResult GetListSupplier()
        {
            try
            {
                using (_objService)
                {
                    return Json(_objService.GetAllSupplier("%", "", 0, 0).ToList(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message });
            }
        }


        public JsonResult GetInitialData()
        {
            var supp = _objService.GetAllSupplier("%", "", 0, 0);
            var st = _objService.GetAllStore(StoreAuthorizationAttribute.StoresAvailable(Session), 0, 0);
            var impuesto = _objService.GetAllTax("", 0, 0);
            var cab = _objService.GetAllCabeceras();
            var cabS = _objService.GetAllCabecerasTienda();
            var cat = new List<EN_Category>();
            var fam = _objService.GetAllFamilies("", 0, 0);

            return Json(new { store = st, supplier = supp, tax = impuesto, cabeceras = cab, cabS = cabS, cats = cat, fams = fam}, JsonRequestBehavior.AllowGet);
        }

        [WebMethod]
        public JsonResult GetStore(int ID)
        {
            try
            {
                Respuesta respuesta = _objService.GetStore(ID);
                if (!respuesta.Status) respuesta.Message = Resources.messages.ResourceManager.GetString(respuesta.Message);
                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_reg, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllCabeceras()
        {
            var cab = _objService.GetAllCabeceras();
            return Json(new {cabeceras = cab}, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetAllCabecerasXTienda()
        {
            var cab = _objService.GetAllCabecerasTienda();
            return Json(new {cabeceras = cab}, JsonRequestBehavior.AllowGet);
        }

        [WebMethod]
        public JsonResult SaveCabecera(EN_Cabeceras cabecera)
        {
            try
            {
                string validate = validateEN_Cabecera(cabecera);
                if (String.IsNullOrEmpty(validate))

                {

                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta r = _objService.SaveCabecera(cabecera);
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

        [WebMethod]
        public JsonResult SaveCabeceraTienda(EN_CabecerasTienda cabecera)
        {
            try
            {
                string validate = validateEN_CabeceraTienda(cabecera);
                if (String.IsNullOrEmpty(validate))
                {
                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta r = _objService.SaveCabeceraTienda(cabecera);
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


        private string validateEN_Cabecera(EN_Cabeceras cabecera)
        {
            if (cabecera != null)
            {
                string resp = "";
               
                if (cabecera.IDTienda <= 0) resp += "[Tienda],";
                if (String.IsNullOrEmpty(cabecera.NumCab)) resp += "[Cabecera],";
                if (String.IsNullOrEmpty(cabecera.Tipo)) resp += "[Tipo],";
                if (String.IsNullOrEmpty(cabecera.Ubicacion)) resp += "[Ubicacion],";
               // if (String.IsNullOrEmpty(cabecera.Monto )) resp += "[Monto],";
               // if (cabecera.Electricidad ) resp += "[Electricidad],";
                return resp;
            }
            else return "[Cabecera nula]";
        }


        private string validateEN_CabeceraTienda(EN_CabecerasTienda cabecera)
        {
            if (cabecera != null)
            {
                string resp = "";
              
                if (cabecera.IDSupplier <= 0) resp += "[ProveedorT],";
                if (cabecera.IDCabecera <= 0) resp += "[CabeceraT],";
                if (String.IsNullOrEmpty(cabecera.Dinamica)) resp += "[Dinamica],";
                // if (String.IsNullOrEmpty(cabecera.ListaProd)) resp += "[Productos],";
                // if (DateTime.VIsNullOrEmpty(cabecera.FechaIni)) resp += "[FechaIni],";
                // if (cabecera.IDTax <= 0) resp += "[Cabecera],";
                // if (String.IsNullOrEmpty(cabecera.Monto )) resp += "[Monto],";
                // if (cabecera.Electricidad ) resp += "[Electricidad],";
                return resp;
            }
            else return "[Cabecera nula]";
        }



        private Respuesta respuesta(string InternalMessage, string Message, object result, bool status)
        {
            Respuesta res = new Respuesta();
            res.InternalMessage = InternalMessage;
            res.Message = Message;
            res.Result = result;
            res.Status = status;
            return res;
        }


        public JsonResult GetAll_BY_Supplier(int supplierID)
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

        public JsonResult GetAll()
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
    }


}
