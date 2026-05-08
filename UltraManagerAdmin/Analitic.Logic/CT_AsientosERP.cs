using Analitic.DataAccess.DataAccessIntegration;
using Analitic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analitic.Logic
{
    public class CT_AsientosERP
    {
        public List<EN_Asientos> GetAllAsientos(string storesID, string hqUsersID, string searchValue, string estadoAsiento, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            return new DT_AsientosERP().GetAllAsientos(storesID, hqUsersID, searchValue, estadoAsiento, orderColumn, orderDirection, skip, take, fromDate, toDate);
        }

        public Dictionary<string, int> GetCountRecordAsientosERP(string storesID, string hqUsersID, string searchValue, string estadoAsiento, string fromDate, string toDate)
        {
            return new DT_AsientosERP().GetCountRecordAsientosERP(storesID, hqUsersID, searchValue, estadoAsiento, fromDate, toDate);
        }
    }

}
