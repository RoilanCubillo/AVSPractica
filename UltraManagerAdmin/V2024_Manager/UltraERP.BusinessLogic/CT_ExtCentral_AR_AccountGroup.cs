using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ExtCentral_AR_AccountGroup
    {
        #region Variables
        DT_ExtCentral_AR_AccountGroup oDT_ExtCentral_AR_AccountGroup = new DT_ExtCentral_AR_AccountGroup();
        #endregion
        #region Constructors
        public CT_ExtCentral_AR_AccountGroup()
        {
            
        }
        #endregion
        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_ExtCentral_AR_AccountGroup> GetAll()
        {
            return oDT_ExtCentral_AR_AccountGroup.GetAll();
        }
        #endregion
    }
}
