using Security.DataAccess.DataAccessIntegration;
using Security.EntitiesAVS;
using System.Collections.Generic;

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
