using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ExtCentral_AR_AccountDynamic
    {
        #region Variables
        DT_ExtCentral_AR_AccountDynamic oDT_ExtCentral_AR_AccountDynamic = new DT_ExtCentral_AR_AccountDynamic();
        #endregion
        #region Constructors
        public CT_ExtCentral_AR_AccountDynamic()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_ExtCentral_AR_AccountDynamic> GetAll()
        {
            return oDT_ExtCentral_AR_AccountDynamic.GetAll();
        }
        public List<EN_ExtCentral_AR_AccountDynamic> GetStoreBalance(int accountID, int groupID)
        {
            return oDT_ExtCentral_AR_AccountDynamic.GetStoreBalance(accountID, groupID);
        }
        #endregion
    }
}
