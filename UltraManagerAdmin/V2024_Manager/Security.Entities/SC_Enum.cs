using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Entities
{
    public class SC_Enum
    {
        public enum ESystemValidateStatus
        {
            SYSTEM_NOT_CONFIGURED = 404,
            SYSTEM_DISABLED = 403,
            SYSTEM_UPDATING = 0,
            USER_UNAUTHORIZED = 401,
            ALLOW_ACCESS = 200,
            NOT_VERIFIED = -1,
            NONE = 0
        }
    }
}
