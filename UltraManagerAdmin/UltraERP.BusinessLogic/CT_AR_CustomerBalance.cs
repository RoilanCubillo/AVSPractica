using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_AR_CustomerBalance
    {
        #region Variables
        DT_AR_CustomerBalance oDT_AR_CustomerBalance = new DT_AR_CustomerBalance();
        #endregion
        #region Constructors
        public CT_AR_CustomerBalance()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_AR_CustomerBalance> GetAll()
        {
            return oDT_AR_CustomerBalance.GetAll();
        }
        #endregion
    }
}
