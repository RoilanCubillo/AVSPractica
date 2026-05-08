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
    [ViewAuthorize(Permissions = "inv-depart")]
    public class DepartamentosController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();
        // GET: Departamentos
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Inventario/Departamentos.cshtml");
        }

        #region"CRUD Departamentos"
        [WebMethod]
        public JsonResult GetDepartments()
        {
            try
            {
                using (_objService)
                {
                    List<EN_Department> departs = _objService.GetAllDepartments("", 0, 0).ToList();
                    List<EN_ExtCentral_Family> fams = _objService.GetAllFamilies("", 0, 0).ToList();

                    return Json(new Respuesta("", "", new { departments = departs, families = fams }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno.", null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [WebMethod]
        public JsonResult GetDepartment(int id)
        {
            try
            {
                using (_objService)
                {
                    return Json(_objService.GetDepartment(id), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message });
            }
        }

        [WebMethod]
        public JsonResult SaveDepartment(EN_Department department)
        {
            try
            {
                string validate = validateEN_Department(department);
                if (String.IsNullOrEmpty(validate))
                {
                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta r = _objService.SaveDepartment(department);
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
        #endregion

        private string validateEN_Department(EN_Department department)
        {
            if (department != null)
            {
                string resp = "";
                if (String.IsNullOrEmpty(department.Name)) resp += "[Descripción],";
                if (String.IsNullOrEmpty(department.Code)) resp += "[Código],";
                return resp;
            }
            else return "[Departamento nulo]";
        }
    }
}
