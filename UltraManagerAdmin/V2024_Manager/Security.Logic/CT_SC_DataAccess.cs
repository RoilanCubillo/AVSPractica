using Security.DataAccess.DataAccessIntegration;
using Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Logic
{
    public class CT_SC_DataAccess
    {
        public List<EN_SC_DataAccess> ValidateDataAccess(int userID, string systemCode, string dataCode)
        {
            return new DT_SC_DataAccess().ValidateDataAccess(userID, systemCode, dataCode);
        }
    }
}
