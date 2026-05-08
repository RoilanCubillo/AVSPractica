using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_Worksheet
    {
        public List<EN_Worksheet> GetAll(string storesID, string tasksCode, string hqUsersID, string searchValue, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            return new DT_Worksheet().GetAll(storesID, tasksCode, hqUsersID, searchValue, orderColumn, orderDirection, skip, take, fromDate, toDate);
        }

        public EN_Worksheet Get(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new DT_Worksheet().Get(storesID, tasksCode, users, worksheetID);
        }

        public Dictionary<string, int> GetCountRecord(string storesID, string tasksCode, string hqUsersID, string searchValue, string fromDate, string toDate)
        {
            return new DT_Worksheet().GetCountRecord(storesID, tasksCode, hqUsersID, searchValue, fromDate, toDate);
        }

        public List<EN_Worksheet.WorksheetStore> GetStores(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new DT_Worksheet().GetStores(storesID, tasksCode, users, worksheetID);
        }

        public List<EN_Worksheet.WorksheetHistory> GetHistories(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new DT_Worksheet().GetHistories(storesID, tasksCode, users, worksheetID);
        }

        public List<EN_Worksheet.WorksheetUpdateItemPrice> GetAll_WorksheetUpdateItemPrice(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new DT_Worksheet().GetAll_WorksheetUpdateItemPrice(storesID, tasksCode, users, worksheetID);
        }

        public List<EN_Worksheet.Worksheet_ItemUpdate> GetAll_Worksheet_ItemUpdate(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new DT_Worksheet().GetAll_Worksheet_ItemUpdate(storesID, tasksCode, users, worksheetID);
        }

        public List<EN_Worksheet.Worksheet_ItemTax> GetAll_Worksheet_ItemTax(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new DT_Worksheet().GetAll_Worksheet_ItemTax(storesID, tasksCode, users, worksheetID);
        }

        public Respuesta Change_Status(string stores, string tasks, string users, int hqUserID, int worksheetID, int status)
        {
            return new DT_Worksheet().Change_Status(stores, tasks, users, hqUserID, worksheetID, status);
        }

        public Respuesta Save_WorksheetContent(EN_Worksheet.WorksheetContent cont)
        {
            return new DT_Worksheet().Save_WorksheetContent(cont);
        }

        public EN_Worksheet.WorksheetContent Get_WorksheetContent(int worksheetContentID, int worksheetID, string stores, string tasks, string users)
        {
            return new DT_Worksheet().Get_WorksheetContent(worksheetContentID, worksheetID, stores, tasks, users);
        }
    }
}
