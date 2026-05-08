using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;
using RetailHero.POS.Core.Shared;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_User : DT
    {
        #region Vars
        #endregion

        #region Constructor
        public DT_User() : base() { }
        #endregion

        #region Methods
        //public (EN_User, int) ValidateUser(string userName, string password)
        //{
        //    int count = 0;

        //    EN_User user = null;
        //    string userPassword = "";

        //    foreach (var u in db.UEP_USER_GET_BY_ACCOUNT(userName))
        //    {
        //        user = new EN_User()
        //        {
        //            ID = u.ID,
        //            Account = u.Account,
        //            EmailAddress = u.EmailAddress,
        //            Name = u.UserName,
        //            SecurityLevel = Convert.ToInt32(u.SecurityLevel),
        //            UserPrivileges = Convert.ToInt32(u.UserPrivileges)
        //        };

        //        userPassword = u.Password;

        //        count++;
        //    }

        //    if (count == 1)
        //    {
        //        if (Cryptographer.Decrypt(userPassword) == password) return (user, 0);
        //        else return (null, 2);
        //    }
        //    else return (null, 1);
        //}

        public EN_User_ValidateUserResponse ValidateUser(string userName, string password)
        {
            int count = 0;

            EN_User user = null;
            string userPassword = "";

            foreach (var u in db.UEP_USER_GET_BY_ACCOUNT(userName))
            {
                user = new EN_User()
                {
                    ID = u.ID,
                    Account = u.Account,
                    EmailAddress = u.EmailAddress,
                    Name = u.UserName,
                    SecurityLevel = Convert.ToInt32(u.SecurityLevel),
                    UserPrivileges = Convert.ToInt32(u.UserPrivileges)
                };

                userPassword = u.Password;
                count++;
            }

            int status;

            if (count == 1)
            {
                status = (Cryptographer.Decrypt(userPassword) == password) ? 0 : 2;
            }
            else
            {
                status = 1;
            }

            return new EN_User_ValidateUserResponse
            {
                User = (status == 0) ? user : null,
                Status = status
            };
        }

        #endregion
    }
}
