using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_User
    {
        public CT_User() { }

        //public (EN_User, int) ValidateUser(string userName, string password)
        //{
        //    return new DT_User().ValidateUser(userName, password);
        //}

        public EN_User_ValidateUserResponse ValidateUser(string userName, string password)
        {
            return new DT_User().ValidateUser(userName, password);
        }

    }
}
