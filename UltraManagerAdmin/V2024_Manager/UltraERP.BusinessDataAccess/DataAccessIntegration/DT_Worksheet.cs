using CentralWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Worksheet : DT
    {
        private ClsData data = new ClsData();
        public static readonly string TOTAL_FILTERED = "TOTAL_FILTERED";
        public static readonly string TOTAL_RECORDS = "TOTAL_RECORDS";

        public DT_Worksheet() : base() { }

        public enum WorksheetStyles
        {
            UpdateItemPrice = 251,
            UpdateItemTax = 320,
            DownloadItem = 261
        }

        public List<EN_Worksheet> GetAll(string storesID, string tasksCode, string hqUsersID, string searchValue, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            List<EN_Worksheet> list = (
                from i in db.UEP_WORKSHEET_GETALL(storesID, tasksCode, hqUsersID, searchValue, orderColumn, orderDirection, skip, take, fromDate, toDate)
                select new EN_Worksheet()
                {
                    DateApplied = i.DateApplied,
                    EffectiveDate = i.EffectiveDate,
                    ID = i.ID,
                    Notes = i.Notes,
                    Status = i.Status ?? 0,
                    StoresName = i.StoresName,
                    StroresID = i.StoresID,
                    Style = i.Style ?? 0,
                    TaskCode = i.TaskCode,
                    TaskDescription = i.TaskDescription,
                    Title = i.Title,
                    UserID = i.UserID,
                    UserName = i.UserName,
                    WorksheetContentID = i.WorksheetContentID
                }
            ).ToList();

            return list;
        }

        public EN_Worksheet Get(string storesID, string tasksCode, string users, int worksheetID)
        {
            EN_Worksheet w = (
                from i in db.UEP_WORKSHEET_GET(storesID, tasksCode, users, worksheetID)
                select new EN_Worksheet()
                {
                    DateApplied = i.DateApplied,
                    EffectiveDate = i.EffectiveDate,
                    ID = i.ID,
                    Notes = i.Notes,
                    Status = i.Status ?? 0,
                    StoresName = i.StoresName,
                    StroresID = i.StoresID,
                    Style = i.Style ?? 0,
                    TaskCode = i.WizardTaskCode,
                    TaskDescription = i.WizardTaskDescription,
                    Title = i.Title,
                    UserID = i.UserID,
                    UserName = i.UserName
                }
            ).Single();

            return w;
        }

        public Dictionary<string, int> GetCountRecord(string storesID, string tasksCode, string hqUsersID, string searchValue, string fromDate, string toDate)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            foreach(var i in db.UEP_WORKSHEET_GET_COUNT_RECORDS(storesID, tasksCode, hqUsersID, searchValue, fromDate, toDate))
            {
                dictionary.Add(TOTAL_FILTERED, i.TOTAL_FILTERED ?? 0);
                dictionary.Add(TOTAL_RECORDS, i.TOTAL_RECORDS ?? 0);
            }

            return dictionary;
        }

        public List<EN_Worksheet.WorksheetStore> GetStores(string storesID, string tasksCode, string users, int worksheetID)
        {
            List<EN_Worksheet.WorksheetStore> list = (
                from i in db.UEP_WORKSHEET_STORE_GETALL(storesID, tasksCode, users, worksheetID)
                select new EN_Worksheet.WorksheetStore()
                {
                    DateProcessed = i.DateProcessed,
                    ID = i.ID,
                    Status = i.Status ?? 0,
                    StoreID = i.StoreID ?? 0,
                    WorksheetID = i.WorksheetID ?? 0,
                    StoreName = i.StoreName
                }
            ).ToList();

            return list;
        }

        public List<EN_Worksheet.WorksheetHistory> GetHistories(string storesID, string tasksCode, string users, int worksheetID)
        {
            List<EN_Worksheet.WorksheetHistory> list = (
                from i in db.UEP_WORKSHEET_HISTORY_GETALL(storesID, tasksCode, users, worksheetID)
                select new EN_Worksheet.WorksheetHistory()
                {
                    Comment = i.Comment,
                    HistoryDate = i.HistoryDate,
                    ID = i.ID,
                    Status = i.Status ?? 0,
                    StoreID = i.StoreID ?? 0,
                    WorksheetID = i.WorksheetID ?? 0,
                    StoreName = i.StoreName
                }
            ).ToList();

            return list;
        }

        //public List<EN_Worksheet.WorksheetUpdateItemPrice> GetAll_WorksheetUpdateItemPrice(string storesID, string tasksCode, string users, int worksheetID)
        //{
        //    List<EN_Worksheet.WorksheetUpdateItemPrice> list = (
        //        from i in db.UEP_WORKSHEET_DETAIL_251_GETALL(storesID, tasksCode, users, worksheetID)
        //        select new EN_Worksheet.WorksheetUpdateItemPrice()
        //        {
        //            BuydownPrice = i.BuydownPrice ?? 0,
        //            BuydownQuantity = i.BuydownQuantity ?? 0,
        //            ID = i.ID,
        //            ItemTaxDescription = i.ItemTaxDescription,
        //            LowerBound = i.LowerBound ?? 0,
        //            SaleEndDate = i.SaleEndDate,
        //            SalePrice = i.SalePrice ?? 0,
        //            SaleStartDate = i.SaleStartDate,
        //            StoreID = i.StoreID ?? 0,
        //            StoreName = i.StoreName,
        //            UpperBound = i.UpperBound ?? 0,
        //            WorksheetID = i.WorksheetID ?? 0,
        //            Description = i.Description,
        //            ExtendedDescription = i.ExtendedDescription,
        //            ItemID = i.ItemID ?? 0,
        //            ItemLookupcode = i.ItemLookupCode,
        //            StoreAvailibity = i.StoreAvailability ?? false
        //        }
        //    ).ToList();

        //    return list;
        //}
        public List<EN_Worksheet.WorksheetUpdateItemPrice> GetAll_WorksheetUpdateItemPrice(string storesID, string tasksCode, string users, int worksheetID)
        {
            List<EN_Worksheet.WorksheetUpdateItemPrice> list = (
                from i in db.UEP_WORKSHEET_DETAIL_251_GETALL(storesID, tasksCode, users, worksheetID)
                select new EN_Worksheet.WorksheetUpdateItemPrice()
                {
                    BuydownPrice = i.BuydownPrice ?? 0,
                    BuydownQuantity = i.BuydownQuantity ?? 0,
                    ID = i.ID,
                    ItemTaxDescription = i.ItemTaxDescription ?? string.Empty,
                    LowerBound = i.LowerBound ?? 0,
                    SaleEndDate = i.SaleEndDate ?? DateTime.Now,
                    SalePrice = i.SalePrice ?? 0,
                    SaleStartDate = i.SaleStartDate,
                    StoreID = i.StoreID ?? 0,
                    StoreName = i.StoreName ?? string.Empty,
                    UpperBound = i.UpperBound ?? 0,
                    WorksheetID = i.WorksheetID ?? 0,
                    Description = i.Description ?? string.Empty,
                    ExtendedDescription = i.ExtendedDescription ?? string.Empty,
                    ItemID = i.ItemID ?? 0,
                    ItemLookupcode = i.ItemLookupCode ?? string.Empty,
                    StoreAvailibity = true//i.StoreAvailability.GetValueOrDefault() == true

                }
            ).ToList();

            return list;
        }


        public List<EN_Worksheet.Worksheet_ItemUpdate> GetAll_Worksheet_ItemUpdate(string storesID, string tasksCode, string users, int worksheetID)
        {
            List<EN_Worksheet.Worksheet_ItemUpdate> list = (
                from i in db.UEP_WORKSHEET_DETAIL_261_GETALL(storesID, tasksCode, users, worksheetID)
                select new EN_Worksheet.Worksheet_ItemUpdate()
                {
                    Description = i.Description,
                    ExtendedDescription = i.ExtendedDescription,
                    ItemID = i.ItemID ?? 0,
                    ItemLookupcode = i.ItemLookupCode,
                    StoreAvailibity = i.StoreAvailability ?? false,
                    ID = i.ID,
                    StoreID = i.StoreID ?? 0,
                    StoreName = i.StoreName,
                    WorksheetID = i.WorksheetID ?? 0
                }
            ).ToList();

            return list;
        }

        public List<EN_Worksheet.Worksheet_ItemTax> GetAll_Worksheet_ItemTax(string storesID, string tasksCode, string users, int worksheetID)
        {
            List<EN_Worksheet.Worksheet_ItemTax> list = (
                from i in db.UEP_WORKSHEET_DETAIL_320_GETALL(storesID, tasksCode, users, worksheetID)
                select new EN_Worksheet.Worksheet_ItemTax()
                {
                    Description = i.Description,
                    ExtendedDescription = i.ExtendedDescription,
                    ItemID = i.ItemID ?? 0,
                    ItemLookupcode = i.ItemLookupCode,
                    StoreAvailibity = i.StoreAvailability ?? false,
                    ID = i.ID,
                    StoreID = i.StoreID ?? 0,
                    StoreName = i.StoreName,
                    WorksheetID = i.WorksheetID ?? 0,
                    ItemTaxDescription = i.ItemTaxDescription
                }
            ).ToList();

            return list;
        }

        /**
         * Este método permite aprobar (status = 2) desaprobar (status = 1) y eliminar (status = 0) hojas de trabajo
         */
        public Respuesta Change_Status(string stores, string tasks, string users, int hqUserID, int worksheetID, int status)
        {
            Respuesta respuesta = new Respuesta("Proedimiento no ejecutado", "error_reg_actualizado", null, false);
            
            try
            {
                respuesta = (from i in db.UEP_WORKSHEET_CHANGE_STATUS(stores, tasks, users, hqUserID, worksheetID, status)
                             select new Respuesta()
                             {
                                 InternalMessage = i.ERROR_MESSAGE,
                                 Message = i.RESPUESTA,
                                 Result = null,
                                 Status = Convert.ToBoolean(i.STATUS)
                             }
                ).Single();

            }
            catch (Exception ex)
            {
                respuesta.InternalMessage = ex.Message;
            }

            return respuesta;
        }

        public Respuesta Save_WorksheetContent(EN_Worksheet.WorksheetContent cont)
        {
            Respuesta respuesta = new Respuesta("Proedimiento no ejecutado", "No se pudo guardar la bitácora", null, false);

            try
            {
                respuesta = (from i in db.UEP_WORKSHEET_CONTENT_INSERT(cont.HQUserID, cont.TaskCode, cont.FileName, cont.ContentType, cont.ContentLenght, cont.ContentData)
                             select new Respuesta()
                             {
                                 InternalMessage = "",
                                 Message = "",
                                 Result = i.SCOPE,
                                 Status = i.SCOPE > 0
                             }
                ).Single();
            }
            catch (Exception e)
            {
                respuesta.InternalMessage = e.Message;
            }

            return respuesta;
        }

        public EN_Worksheet.WorksheetContent Get_WorksheetContent(int worksheetContentID, int worksheetID, string stores, string tasks, string users)
        {
            EN_Worksheet.WorksheetContent en = null;

            foreach(var i in db.UEP_WORKSHEET_CONTENT_GET(worksheetContentID, worksheetID, stores, tasks, users))
            {
                en = new EN_Worksheet.WorksheetContent()
                {
                    ContentData = i.ContentData.ToArray(),
                    ContentLenght = i.ContentLenght,
                    ContentType = i.ContentType,
                    DateApplied = i.DateApplied,
                    FileName = i.FileName,
                    HQUserID = i.HQUserID,
                    ID = i.ID,
                    TaskCode = i.TaskCode
                };
            }

            return en;
        }
    }
    
}
