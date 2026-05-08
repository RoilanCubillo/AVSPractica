using Security.DataAccess.DataAccessIntegration;
using Security.EntitiesAVS;
using System.Collections.Generic;

namespace Security.Logic
{
   public class CT_SC_Module
    {
        public List<EN_SC_Module> ValidateModules(int userID, string systemCode)
        {
            return new DT_SC_Module().ValidateModules(userID, systemCode);
        }
    }
}
