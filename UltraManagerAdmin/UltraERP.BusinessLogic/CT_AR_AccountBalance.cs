using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_AR_AccountBalance
    {
        #region Variables
        DT_AR_AccountBalance oDT_AR_AccountBalance = new DT_AR_AccountBalance();
        #endregion
        #region Constructors
        public CT_AR_AccountBalance()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_AR_AccountBalance> GetAll()
        {
            return oDT_AR_AccountBalance.GetAll();
        }
        #endregion
    }
}
