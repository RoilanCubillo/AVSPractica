using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_AR_AccountManager
    {
        #region Variables
        DT_AR_AccountManager oDT_AR_AccountManager = new DT_AR_AccountManager();
        #endregion
        #region Constructors
        public CT_AR_AccountManager()
        {

        }
        #endregion
        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_AR_AccountManager> GetAll()
        {
            return oDT_AR_AccountManager.GetAll();
        }
        #endregion
    }
}
