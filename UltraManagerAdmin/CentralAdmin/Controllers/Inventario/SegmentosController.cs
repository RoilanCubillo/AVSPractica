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
    [ViewAuthorize(Permissions = "inv-segs")]
    public class SegmentosController : Controller
    {
         private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();

        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Inventario/Segmentos.cshtml");
        }

        public JsonResult GetInitData()
        {
            try
            {
                using (_objService)
                {
                    List<EN_ExtCentral_Segment> list_segs = _objService.GetAll_ExtCentral_Segment().ToList();
                    List<EN_ExtCentral_SubCategory> list_subCat = _objService.GetAllSubCategories("", 0, 0).ToList();

                    return Json(new Respuesta("", "", new { list_subCategories = list_subCat, list_segments = list_segs }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSegmentos()
        {
            try
            {
                using (_objService)
                {
                    List<EN_ExtCentral_Segment> list_segs = _objService.GetAll_ExtCentral_Segment().ToList();

                    return Json(new Respuesta("", "", new { list_segments = list_segs }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetBySubCategoryID(int subCategoryID)
        {
            try
            {
                List<EN_ExtCentral_Segment> list = _objService.GetAllSegment_By_SubCategoryID(subCategoryID).ToList();

                return Json(new Respuesta("", "", list, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetBySubCategories(int[] subCategoriesID)
        {
            try
            {
                using (_objService)
                {
                    List<EN_ExtCentral_Segment> list_segs = _objService.GetAll_ExtCentral_Segment().ToList(), new_list = new List<EN_ExtCentral_Segment>();

                    new_list = list_segs.Where(x => subCategoriesID.Contains(x.SubCategoryID)).ToList();

                    return Json(new Respuesta("", "", new { list_segments = new_list }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(EN_ExtCentral_Segment segment)
        {
            try
            {
                Respuesta r = _objService.Save_ExtCentral_Segment(segment);
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