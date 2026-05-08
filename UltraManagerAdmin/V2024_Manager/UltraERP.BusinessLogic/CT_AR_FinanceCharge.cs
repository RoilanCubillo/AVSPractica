using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_AR_FinanceCharge
    {
        #region Variables
        DT_AR_FinanceCharge oDT_AR_FinanceChargep = new DT_AR_FinanceCharge();
        #endregion
        #region Constructors
        public CT_AR_FinanceCharge()
        {
            
        }
        #endregion
        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_AR_FinanceCharge> GetAll()
        {
            return oDT_AR_FinanceChargep.GetAll();
        }
        #endregion
    }
}
