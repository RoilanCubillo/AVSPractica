using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.CambiosMasivos
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize(Permissions = "camb-masivos-worksheet")]
    public class HojasTrabajoController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();

        // GET: HojasTrabajo
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/CambiosMasivos/HojasTrabajo.cshtml");
        }

        [HttpPost]
        public JsonResult GetAll_Worksheets(string searchValue, string fromDate, string toDate)
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

            List<EN_Worksheet> list = _objService.EN_Worksheet_GetAll(StoreAuthorizationAttribute.StoresAvailable(Session),
                WizardAuthorizationAttribute.TasksAvailable(Session),
                WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session),
                searchValue, orderColumn, sortColumnDir, skip, take,
                dateFrom, dateTo
            ).ToList();

            Dictionary<string, int> records = _objService.EN_Worksheet_GetCountRecord(StoreAuthorizationAttribute.StoresAvailable(Session),
                WizardAuthorizationAttribute.TasksAvailable(Session),
                WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session), searchValue,
                dateFrom, dateTo
            );

            return Json(
                new
                {
                    draw = draw,
                    recordsFiltered = records[DT_Worksheet.TOTAL_FILTERED],
                    recordsTotal = records[DT_Worksheet.TOTAL_RECORDS],
                    data = list
                }
            );
        }

        [HttpGet]
        public JsonResult Get_Worksheet(int workSheetID)
        {
            try
            {
                EN_Worksheet worksheet = _objService.EN_Worksheet_Get(StoreAuthorizationAttribute.StoresAvailable(Session),
                WizardAuthorizationAttribute.TasksAvailable(Session),
                WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session), workSheetID);

                List<EN_Worksheet.WorksheetStore> worksheetStores = _objService.EN_Worksheet_GetStores(StoreAuthorizationAttribute.StoresAvailable(Session),
                    WizardAuthorizationAttribute.TasksAvailable(Session),
                    WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session), workSheetID).ToList();

                List<EN_Worksheet.WorksheetHistory> worksheetHistory = _objService.EN_Worksheet_GetHistories(StoreAuthorizationAttribute.StoresAvailable(Session),
                    WizardAuthorizationAttribute.TasksAvailable(Session),
                    WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session), workSheetID).ToList();

                object worksheetContents = null;

                switch (worksheet.Style)
                {
                    case (int)DT_Worksheet.WorksheetStyles.UpdateItemPrice:
                        worksheetContents = _objService.EN_Worksheet_GetAll_WorksheetUpdateItemPrice(StoreAuthorizationAttribute.StoresAvailable(Session),
                            WizardAuthorizationAttribute.TasksAvailable(Session),
                            WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session), workSheetID).ToList();
                        break;
                    case (int)DT_Worksheet.WorksheetStyles.UpdateItemTax:
                        worksheetContents = _objService.EN_Worksheet_GetAll_Worksheet_ItemTax(StoreAuthorizationAttribute.StoresAvailable(Session),
                            WizardAuthorizationAttribute.TasksAvailable(Session),
                            WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session), workSheetID).ToList();
                        break;
                    default:
                        worksheetContents = _objService.EN_Worksheet_GetAll_Worksheet_ItemUpdate(StoreAuthorizationAttribute.StoresAvailable(Session),
                            WizardAuthorizationAttribute.TasksAvailable(Session),
                            WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session), workSheetID).ToList();
                        break;
                }

                return Json(new Respuesta("", "",
                    new
                    {
                        worksheet = worksheet,
                        worksheetStores = worksheetStores,
                        worksheetHistory = worksheetHistory,
                        worksheetContents = worksheetContents
                    }, true),
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Change_Status(int worksheetID, int status)
        {
            Respuesta respuesta = _objService.EN_Worksheet_Change_Status(
                StoreAuthorizationAttribute.StoresAvailable(Session),
                WizardAuthorizationAttribute.TasksAvailable(Session),
                WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session),
                Convert.ToInt32(Session["USER_AUTOID"]), worksheetID, status);

            return Json(respuesta);
        }

        [HttpGet]
        public ActionResult DownloadFile(int worksheetContentID, int worksheetID)
        {
            EN_Worksheet.WorksheetContent content = _objService.EN_Worksheet_Get_WorksheetContent(
                worksheetContentID,
                worksheetID,
                StoreAuthorizationAttribute.StoresAvailable(Session),
                WizardAuthorizationAttribute.TasksAvailable(Session),
                WizardAuthorizationAttribute.WorkSheetUsersAvailable(Session)
            );

            if (content != null)
            {
                string fileName = $"T{content.TaskCode}-{worksheetID}-{worksheetContentID}-{content.DateAppliedStringFile}-{content.FileName}";

                return File(content.ContentData, content.ContentType, fileName);
            }

            return View("~/Views/Errors/NotFoundError.cshtml");
        }
    }
}