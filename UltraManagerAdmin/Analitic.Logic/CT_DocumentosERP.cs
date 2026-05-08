using Analitic.DataAccess.DataAccessIntegration;
using Analitic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analitic.Logic
{
    public class CT_DocumentosERP
    {
        public List<EN_DocumentosERP> GetAllDocsERP(string storesID, string hqUsersID, string searchValue, string tipoDocumento, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            return new DT_DocumentosERP().GetAll(storesID, hqUsersID, searchValue, tipoDocumento, orderColumn, orderDirection, skip, take, fromDate, toDate);
        }

        public Dictionary<string, int> GetCountRecordDocsMH(string storesID, string hqUsersID, string searchValue, string tipoDocumento, string fromDate, string toDate)
        {
            return new DT_DocumentosERP().GetCountRecordDocsERP(storesID, hqUsersID, searchValue, tipoDocumento, fromDate, toDate);
        }
    }
}
