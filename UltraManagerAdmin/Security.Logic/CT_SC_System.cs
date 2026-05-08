using Security.DataAccess.DataAccessIntegration;
using static Security.EntitiesAVS.SC_Enum;

namespace Security.Logic
{
   public class CT_SC_System
    {
        public (ESystemValidateStatus, bool) ValidateSystem(int userID, string systemCode)
        {
            return new DT_SC_System().ValidateSystem(userID, systemCode);
        }
    }
}
