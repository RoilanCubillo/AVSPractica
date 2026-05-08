using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.UltraERPServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Cobros
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize(Permissions = "cxc-cobs-proveedores")]
    public class CobroProveedoresController : Controller
    {

         private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();

        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Proveedores/CobroProveedores.cshtml");
        }

        public JsonResult GetInitData()
        {
            try
            {
                var provs = _objService.GetAllSupplier("%", "", 0, 0);
                var casas = _objService.GetAllPurchasersByInactive(false);
                var cxc_fr = _objService.GetAll_CXC_FormaRebajos().ToList();
                var cxc_provs = _objService.GetAll_CXC_Proveedor().ToList();
                var iDCs = _objService.GetAll_CXC_IDCTipo();
                var iICs = _objService.GetAll_CXC_IICTipo();
                var frecuencias = _objService.GetAll_CXC_FrecuenciasPago();
                var negociaciones = _objService.GetAll_CXC_Negociaciones();

                return Json(new Respuesta("", "", new
                {
                    cxc_proveedores = cxc_provs,
                    cxc_formas = cxc_fr,
                    proveedores = provs,
                    casas = casas,
                    iDCs = iDCs,
                    iICs = iICs,
                    frecuencias = frecuencias,
                    negociaciones = negociaciones
                }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllFichasProveedor()
        {
            try
            {
                var cxc_provs = _objService.GetAll_CXC_Proveedor().ToList();

                return Json(new Respuesta("", "", new { cxc_proveedores = cxc_provs }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDetalles(int fichaID)
        {
            try
            {
                var IICs = _objService.GetAll_CXC_IIC(fichaID);
                var IDCs = _objService.GetAll_CXC_IDC(fichaID);

                return Json(new Respuesta("", "", new { iic = IICs, idc = IDCs }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveFicha(EN_CXC_Proveedor ficha, List<EN_CXC_IDC> IDCs, List<EN_CXC_IIC> IICs)
        {
            try
            {
                if (ficha.Estado == "01")
                {
                    bool flag = true;
                    Dictionary<string, object> result_ficha = new Dictionary<string, object>();
                    result_ficha.Add("RESULT", "succ_insertado");
                    result_ficha.Add("SCOPE", ficha.ID);

                    if (ficha.ID == 0) result_ficha = _objService.Save_CXC_Proveedor(ficha);

                    List<object> idc = new List<object>(), iic = new List<object>();

                    if (((string)result_ficha["RESULT"]) == "succ_insertado")
                    {
                        int cxc_provID = ((int)result_ficha["SCOPE"]);

                        if (IDCs != null && IDCs.Count != 0)
                        {
                            foreach (EN_CXC_IDC i in IDCs)
                            {
                                Respuesta resp = i.Estado == 0 ? _objService.Save_CXC_IDC(i, cxc_provID) : (
                                    i.Estado == -1 ? _objService.Delete_CXC_IDC(i.ID) : (
                                    i.Estado == 1 ? new Respuesta("", "", i.ID, true) : new Respuesta("", "", null, false)));
                                if (!resp.Status) flag = false;
                                idc.Add(new { scope = resp.Result, idAux = i.IDAux, status = resp.Status });
                            }
                        }

                        if (IICs != null && IICs.Count != 0)
                        {
                            foreach (EN_CXC_IIC i in IICs)
                            {
                                Respuesta resp = i.Estado == 0 ? _objService.Save_CXC_IIC(i, ((int)result_ficha["SCOPE"])) : (
                                    i.Estado == -1 ? _objService.Delete_CXC_IIC(i.ID) : (
                                    i.Estado == 1 ? new Respuesta("", "", i.ID, true) : new Respuesta("", "", null, false)));
                                if (!resp.Status) flag = false;
                                iic.Add(new { scope = resp.Result, idAux = i.IDAux, status = resp.Status });
                            }
                        }

                        return Json(new Respuesta((flag ? "" : "ALGUNOS DETALLES NO FUERON PROCESADOS."),
                            (flag ? "Datos guardados exitosamente." : "Algunos detalles no se lograron guardar."),
                            new
                            {
                                result_ficha = result_ficha,
                                result_idc = idc,
                                result_iic = iic
                            },
                            flag), JsonRequestBehavior.AllowGet);
                    }

                    return Json(new Respuesta("ALGUNOS DETALLES NO SE GUARDARON",
                        Resources.messages.error_interno + " " + Resources.messages.ResourceManager.GetString((string)result_ficha["RESULT"]),
                        null, false), JsonRequestBehavior.AllowGet);
                }

                return Json(new Respuesta(Resources.messages.error_interno,
                        "NO ES PERMITIDO ALTERAR LA INFORMACIÓN",
                        null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ChangeStatusProveedor(int proveedorID, string status)
        {
            try
            {
                Respuesta respuesta = _objService.ChangeStatus_CXC_Proveedor(proveedorID, status);

                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
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
    }
}
