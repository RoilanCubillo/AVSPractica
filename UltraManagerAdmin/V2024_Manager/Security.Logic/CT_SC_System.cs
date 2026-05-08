using Security.DataAccess.DataAccessIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Security.Entities.SC_Enum;

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
