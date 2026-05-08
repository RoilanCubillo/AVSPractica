using Security.DataAccess.DataAccessIntegration;
using Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Logic
{
    public class CT_SC_User
    {
        public (EN_SC_User, int, string) ValidateUser(string account, int autoID, int systemID)
        {
            return new DT_SC_User().ValidateUser(account, autoID, systemID);
        }

        public bool validateUserSession(int iD, int autoID, int systemID)
        {
            return new DT_SC_User().validateUserSession(iD, autoID, systemID);
        }
    }
}
