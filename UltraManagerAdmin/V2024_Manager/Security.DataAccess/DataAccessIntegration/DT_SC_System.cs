using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Security.Entities.SC_Enum;

namespace Security.DataAccess.DataAccessIntegration
{
    public class DT_SC_System : DT_SC
    {
        public DT_SC_System() : base() { }

        public (ESystemValidateStatus, bool) ValidateSystem(int userID, string systemCode)
        {
            foreach(var r in db.SC_SYSTEM_VALIDATE(userID, systemCode))
                return ((ESystemValidateStatus)r.RESULT, (bool)r.RESULT_STATUS);

            return (ESystemValidateStatus.NOT_VERIFIED, false);
        }
    }
}
