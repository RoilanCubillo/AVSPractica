using Analitic.DataAccess.DataAccessIntegration;
using Analitic.Entities;
using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfAnalitica;
using CentralAdmin.UltraERPServiceClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Analitica
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize(Permissions = "conta-analitica")]
    public class MonitoreoDatosController : Controller
    {
        private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();
        private readonly AnaliticaServiceClient _objServiceAnalitic = new AnaliticaServiceClient();

        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Analitica/MonitoreoDatos.cshtml");
        }

        [HttpPost]
        public JsonResult GetAll_DocsMH(string searchValue, string comprobanteTipo, string fromDate, string toDate, string idStore)
        {
            try
            {

                string draw = Request.Form.GetValues("draw").FirstOrDefault(); // Draw del DataTable
                int orderColumn = Convert.ToInt32(Request.Form.GetValues("order[0][column]").FirstOrDefault()); // Columna por la que se está ordenando
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault(); // La dirección en la que se está ordenando
                string start = Request.Form.GetValues("start").FirstOrDefault(); // Valor de posición de donde se empieza a mostrar registros
                string length = Request.Form.GetValues("length").FirstOrDefault(); // Valor de la cantidad de registros a mostrar
                int take = Convert.ToInt32(length); // Valor de registros a omitir a int
                int skip = Convert.ToInt32(start); // Valor de registros a mostrar a int

                string dateFrom = fromDate == "" ? fromDate : DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
                string dateTo = toDate == "" ? toDate : DateTime.Parse(toDate).ToString("yyyy-MM-dd");

                List<EN_DocumentosMH> list = _objService.GetAllDocsMH(idStore, "1",
                    searchValue, comprobanteTipo.TrimStart('0'), orderColumn, sortColumnDir, skip, take,
                    dateFrom, dateTo
                ).ToList();

                Dictionary<string, int> records = _objService.GetCountRecordDocsMH(idStore,
                    WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session), searchValue, comprobanteTipo.TrimStart('0'),
                    dateFrom, dateTo
                );

                return Json(
                    new
                    {
                        draw = draw,
                        recordsFiltered = records[DT_DocumentosMH.TOTAL_FILTERED],
                        recordsTotal = records[DT_DocumentosMH.TOTAL_RECORDS],
                        data = list
                    }
                );

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult GetAll_DocsERP(string searchValue, string comprobanteTipo, string fromDate, string toDate, string idStore)
        {
            try
            {
                string draw = Request.Form.GetValues("draw").FirstOrDefault(); // Draw del DataTable
                int orderColumn = Convert.ToInt32(Request.Form.GetValues("order[0][column]").FirstOrDefault()); // Columna por la que se está ordenando
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault(); // La dirección en la que se está ordenando
                string start = Request.Form.GetValues("start").FirstOrDefault(); // Valor de posición de donde se empieza a mostrar registros
                string length = Request.Form.GetValues("length").FirstOrDefault(); // Valor de la cantidad de registros a mostrar
                int take = Convert.ToInt32(length); // Valor de registros a omitir a int
                int skip = Convert.ToInt32(start); // Valor de registros a mostrar a int

                string dateFrom = fromDate == "" ? fromDate : DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
                string dateTo = toDate == "" ? toDate : DateTime.Parse(toDate).ToString("yyyy-MM-dd");

                List<Analitic.Entities.EN_DocumentosERP> list = _objServiceAnalitic.GetAllDocsERP(idStore, "1",
                    searchValue, comprobanteTipo.TrimStart('0'), orderColumn, sortColumnDir, skip, take,
                    dateFrom, dateTo
                ).ToList();

                Dictionary<string, int> records = _objServiceAnalitic.GetCountRecordDocsERP(idStore,
                    WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session), searchValue, comprobanteTipo.TrimStart('0'),
                    dateFrom, dateTo
                );

                return Json(
                    new
                    {
                        draw = draw,
                        recordsFiltered = records[DT_DocumentosERP.TOTAL_FILTERED],
                        recordsTotal = records[DT_DocumentosERP.TOTAL_RECORDS],
                        data = list
                    }
                );

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult GetAll_Asientos(string searchValue, string estadoAsiento, string comprobanteTipo, string fromDate, string toDate, string idStore)
        {
            try
            {
                string storeId = (idStore == "0") ? "%" : idStore;
                string draw = Request.Form.GetValues("draw").FirstOrDefault(); // Draw del DataTable
                int orderColumn = Convert.ToInt32(Request.Form.GetValues("order[0][column]").FirstOrDefault()); // Columna por la que se está ordenando
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault(); // La dirección en la que se está ordenando
                string start = Request.Form.GetValues("start").FirstOrDefault(); // Valor de posición de donde se empieza a mostrar registros
                string length = Request.Form.GetValues("length").FirstOrDefault(); // Valor de la cantidad de registros a mostrar
                int take = Convert.ToInt32(length); // Valor de registros a omitir a int
                int skip = Convert.ToInt32(start); // Valor de registros a mostrar a int
                string dateFrom = "", dateTo = "";

                if (fromDate != null)
                {
                    var fdate = fromDate.Substring(4, 11);
                    dateFrom = DateTime.ParseExact(fdate, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    string tdate = toDate.Substring(4, 11);
                    dateTo = DateTime.ParseExact(tdate, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                }

                using (_objServiceAnalitic)
                {
                    //List<EN_Graphics> list_Graphics = _objService.GetAllGraphics(15, "", "", "", dateFrom.ToString(), dateTo.ToString(), "").ToList();
                    //int totDocAceptados = list_Graphics.Sum(item => item.Doc_Aceptados);
                    //int totDocRechazados = list_Graphics.Sum(item => item.Doc_Rechazados);
                    //list_Graphics[0].Tot_Doc_Aceptados = totDocAceptados;
                    //list_Graphics[0].Tot_Doc_Rechazados = totDocRechazados;

                    List<EN_Asientos> list = _objServiceAnalitic.GetAllAsientos(storeId, "1", searchValue, estadoAsiento, orderColumn, sortColumnDir, skip, take, dateFrom, dateTo).ToList();

                    Dictionary<string, int> records = _objServiceAnalitic.GetCountRecordAsientosERP(StoreAuthorizationAttribute.StoresAvailable(Session),
                        WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session), searchValue, estadoAsiento, dateFrom, dateTo);

                    // return Json(new Respuesta("", "", new { draw = draw, recordsFiltered = 10, recordsTotal = 10, graphics = list_Graphics, data = list }, true), JsonRequestBehavior.AllowGet);

                    return Json(
                    new
                    {
                        draw = draw,
                        recordsFiltered = records[DT_AsientosERP.TOTAL_FILTERED],
                        recordsTotal = records[DT_AsientosERP.TOTAL_RECORDS],
                        data = list
                    });
                }

            }
            catch (Exception e)
            {
                return Json(new UltraERP.BusinessEntities.Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [WebMethod]
        public JsonResult GetAll_Graphics_DocsMH(string fromDate, string toDate, string idStore)
        {
            try
            {
                string storeId = (idStore == "0") ? "%" : idStore;
                string dateFrom = "", dateTo = "";
                if (fromDate != null)
                {
                    var fdate = fromDate.Substring(4, 11);
                    dateFrom = DateTime.ParseExact(fdate, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    string tdate = toDate.Substring(4, 11);
                    dateTo = DateTime.ParseExact(tdate, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                }

                using (_objService)
                {
                    List<UltraERP.BusinessEntities.EN_Graphics> list_Graphics = _objService.GetAllGraphics(storeId, "", "", "", dateFrom.ToString(), dateTo.ToString(), "").ToList();
                    int totDocAceptados = list_Graphics.Sum(item => item.Doc_Aceptados);
                    int totDocRechazados = list_Graphics.Sum(item => item.Doc_Rechazados);
                    list_Graphics[0].Tot_Doc_Aceptados = totDocAceptados;
                    list_Graphics[0].Tot_Doc_Rechazados = totDocRechazados;

                    return Json(new UltraERP.BusinessEntities.Respuesta("", "", new { graphics = list_Graphics }, true), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new UltraERP.BusinessEntities.Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [WebMethod]
        public JsonResult GetAll_Graphics_DocsERP(string fromDate, string toDate, string idStore)
        {
            try
            {
                string storeId = (idStore == "0") ? "%" : idStore;
                string dateFrom = "", dateTo = "";
                if (fromDate != null)
                {
                    var fdate = fromDate.Substring(4, 11);
                    dateFrom = DateTime.ParseExact(fdate, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    string tdate = toDate.Substring(4, 11);
                    dateTo = DateTime.ParseExact(tdate, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                }

                using (_objServiceAnalitic)
                {
                    List<Analitic.Entities.EN_Graphics> list_Graphics = _objServiceAnalitic.GetAllGraphics(storeId, "", "", "", dateFrom.ToString(), dateTo.ToString(), "Docs_ERP").ToList();
                    int totDocAceptados = list_Graphics.Sum(item => item.Doc_Aceptados);
                    int totDocRechazados = list_Graphics.Sum(item => item.Doc_Rechazados);
                    list_Graphics[0].Tot_Doc_Aceptados = totDocAceptados;
                    list_Graphics[0].Tot_Doc_Rechazados = totDocRechazados;

                    return Json(new UltraERP.BusinessEntities.Respuesta("", "", new { graphics = list_Graphics }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new UltraERP.BusinessEntities.Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAll_Graphics_AsientosERP(string fromDate, string toDate, string idStore)
        {
            try
            {
                string storeId = (idStore == "0") ? "%" : idStore;
                string dateFrom = "", dateTo = "";
                if (fromDate != null)
                {
                    var fdate = fromDate.Substring(4, 11);
                    dateFrom = DateTime.ParseExact(fdate, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    string tdate = toDate.Substring(4, 11);
                    dateTo = DateTime.ParseExact(tdate, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                }

                using (_objServiceAnalitic)
                {
                    List<Analitic.Entities.EN_Graphics> list_Graphics = _objServiceAnalitic.GetAllGraphics(storeId, "", "", "", dateFrom.ToString(), dateTo.ToString(), "Asientos_ERP").ToList();

                    return Json(new UltraERP.BusinessEntities.Respuesta("", "", new { graphics = list_Graphics }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new UltraERP.BusinessEntities.Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [WebMethod]
        public JsonResult GetAllStore()
        {
            try
            {
                using (_objService)
                {
                    // return Json(_objService.GetAllStore("%", 0, 0).ToList(), JsonRequestBehavior.AllowGet);

                    var data = _objService.GetAllStore("%", 0, 0).ToList();

                    List<object> list = new List<object>();

                    data.ForEach(x => list.Add(new { ID = x.IDS, Description = x.NameS }));

                    return Json(new { store = list }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message });
            }
        }

    }
}