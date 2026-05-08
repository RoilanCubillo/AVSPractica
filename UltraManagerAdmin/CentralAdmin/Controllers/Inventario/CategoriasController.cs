using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.UltraERPServiceClient;
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
    [ViewAuthorize(Permissions = "inv-categs")]
    public class CategoriasController : Controller
    {
        private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();

        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Inventario/Categorias.cshtml");
        }

        [WebMethod]
        public JsonResult GetAll()
        {
            try
            {
                using (_objService)
                {
                    List<EN_Category> list_categories = _objService.GetAllCategories("", 0, 0).ToList();
                    List<EN_Department> list_departments = _objService.GetAllDepartments("", 0, 0).ToList();
                    return Json(new Respuesta("","", new { categories = list_categories, departments = list_departments }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
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

        [WebMethod]
        public JsonResult Get(int ID)
        {
            try
            {
                Respuesta respuesta = _objService.GetCategory(ID);
                if (!respuesta.Status) respuesta.Message = Resources.messages.ResourceManager.GetString(respuesta.Message);
                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_reg, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetListByDepartments(int[] departments)
        {
            List<EN_Category> list_categories = _objService.GetAllCategories("", 0, 0).ToList();

            var new_list = list_categories.Where(x => departments.Contains(x.DepartmentID));

            return Json(new { categories = new_list }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetByDepartmentID(int departmentID)
        {
            try
            {
                List<EN_Category> list = _objService.GetAllCategories_ByDepartmentID(departmentID).ToList();

                return Json(new Respuesta("", "", list, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [WebMethod]
        public JsonResult Save(EN_Category category)
        {
            try
            {

                string validate = validateEN_Category(category);
                if (String.IsNullOrEmpty(validate))
                {
                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta result = _objService.SaveCategory(category);
                        result.Message = Resources.messages.ResourceManager.GetString(result.Message);
                        return Json(result, JsonRequestBehavior.AllowGet);
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

        private string validateEN_Category(EN_Category category)
        {
            if (category != null)
            {
                string resp = "";
                if (String.IsNullOrEmpty(category.Name)) resp += "[Nombre],";
                if (String.IsNullOrEmpty(category.Code)) resp += "[Código],";
                if (category.DepartmentID <= 0) resp += "[Departamento]";
                return resp;
            }
            else return "[Categoría nula]";
        }
    }
}