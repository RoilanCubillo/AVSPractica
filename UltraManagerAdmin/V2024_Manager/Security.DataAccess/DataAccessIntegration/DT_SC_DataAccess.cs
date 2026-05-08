using Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.DataAccess.DataAccessIntegration
{
    public class DT_SC_DataAccess : DT_SC
    {
        public DT_SC_DataAccess() : base() { }

        public List<EN_SC_DataAccess> ValidateDataAccess(int userID, string systemCode, string dataCode)
        {
            List<EN_SC_DataAccess> list = (
                from i in db.SC_DATAACCESS_VALIDATE(dataCode, userID, systemCode)
                select new EN_SC_DataAccess()
                {
                    ID = i.DataAccessID,
                    Code = i.DataCode,
                    DataID = i.DataID,
                    DataIDs = i.DataAccessDataIDs,
                    Enable = i.DataEnable,
                    EnableAll = i.DataAccessEnAll,
                    Name = i.DataAccessDescription,
                    SystemID = i.DataSystemID,
                    TableName = i.DataTableName,
                    TablePKName = i.DataTablePKName
                }
            ).ToList();

            return list;
        }
    }
}
