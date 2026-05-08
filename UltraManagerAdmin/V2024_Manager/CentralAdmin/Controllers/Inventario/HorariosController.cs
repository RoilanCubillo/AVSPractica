using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Inventario
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize]
    public class HorariosController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();
        // GET: Horarios
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Inventario/Horarios.cshtml");
        }

        public JsonResult GetAll()
        {
            try
            {
                var resp = _objService.GetAllSchedules("", 0, 0);
                var resp2 = _objService.GetAllScheduleSegment();
                if (resp != null)
                {
                    Resources.messages.ResourceManager.GetString("");
                    if (resp2 != null) return Json(new Respuesta("", "", new { schedules = resp, segments = resp2 }, true), JsonRequestBehavior.AllowGet);
                    else return Json(new Respuesta("RESULTADO SEGMENTOS NULL", Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("RESULTADO HORARIOS NULL", Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(EN_Schedule schedule, List<EN_ScheduleSegment> segments)
        {
            try
            {
                Respuesta r = _objService.SaveSchedule(schedule, segments.ToArray());
                r.Message = Resources.messages.ResourceManager.GetString(r.Message);
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_intento_guardar, null, false), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
