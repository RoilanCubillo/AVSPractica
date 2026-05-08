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
    [ViewAuthorize(Permissions = "inv-subcategs")]
    public class SubCategoriasController : Controller
    {
         private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();

        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Inventario/SubCategorias.cshtml");
        }

        public JsonResult GetAll()
        {
            try
            {
                using (_objService)
                {
                    List<EN_ExtCentral_SubCategory> list_subcategories = _objService.GetAllSubCategories("", 0, 0).ToList();
                    List<EN_Category> list_categories = _objService.GetAllCategories("", 0, 0).ToList();
                    return Json(new Respuesta("", "", new { list_categories=list_categories, list_subCategories=list_subcategories }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetListByCategories(int[] categories)
        {
            List<EN_ExtCentral_SubCategory> list_subcategories = _objService.GetAllSubCategories("", 0, 0).ToList();

            var new_list = list_subcategories.Where(x => categories.Contains(x.CategoryID));

            return Json(new { subCategories = new_list }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetByCategoryID(int categoryID)
        {
            try
            {
                List<EN_ExtCentral_SubCategory> list = _objService.GetAllSubCategory_By_CategoryID(categoryID).ToList();

                return Json(new Respuesta("", "", list, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(EN_ExtCentral_SubCategory subCategory)
        {
            try
            {
                string validate = validateEntity(subCategory);
                if (String.IsNullOrEmpty(validate))
                {
                    if (_objService.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        Respuesta r = _objService.SaveSubCategories(subCategory);
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

        private string validateEntity(EN_ExtCentral_SubCategory entity)
        {
            string validate = "";
            if (entity != null)
            {
                if (String.IsNullOrEmpty(entity.Description)) validate += "[Description]";
                if (String.IsNullOrEmpty(entity.Code)) validate += "[Code]";
                if (entity.CategoryID <= 0) validate += "[Category]";
            }
            else validate = "[SubCategory]";
            return validate;
        }
    }
}