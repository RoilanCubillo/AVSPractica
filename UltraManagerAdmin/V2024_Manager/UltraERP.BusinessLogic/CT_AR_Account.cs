using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_AR_Account
    {
        #region Variables
        DT_AR_Account oDT_AR_Account = new DT_AR_Account();
        #endregion
        #region Constructors
        public CT_AR_Account()
        {

        }
        #endregion
        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_AR_Account> GetAll()
        {
            return oDT_AR_Account.GetAll();
        }
        public Respuesta Save(EN_AR_Account account, EN_ExtCentral_AR_AccountDynamic accountDynamic, EN_AR_AccountLink accountLink)
        {
            return oDT_AR_Account.SaveAccount(account, accountDynamic, accountLink);
        }
        #endregion
    }
}
