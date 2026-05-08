using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_DocumentosMH
    {
        public List<EN_DocumentosMH> GetAllDocsMH(string storesID, string hqUsersID, string searchValue, string estadoHacienda, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            return new DT_DocumentosMH().GetAll(storesID, hqUsersID, searchValue, estadoHacienda, orderColumn, orderDirection, skip, take, fromDate, toDate);
        }

        public Dictionary<string, int> GetCountRecordDocsMH(string storesID, string hqUsersID, string searchValue, string estadoHacienda, string fromDate, string toDate)
        {
            return new DT_DocumentosMH().GetCountRecordDocsMH(storesID, hqUsersID, searchValue, estadoHacienda, fromDate, toDate);
        }
    }
}
