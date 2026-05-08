using Security.DataAccess.DataAccessIntegration;
using Security.EntitiesAVS;
using System.Collections.Generic;

namespace Security.Logic
{
   public class CT_SC_View
    {
        public List<EN_SC_View> ValidateViews(int userID, string systemCode)
        {
            return new DT_SC_View().ValidateViews(userID, systemCode);
        }
    }
}
