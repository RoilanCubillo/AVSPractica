using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_AR_AccountGroup
    {
        #region Variables
        DT_AR_AccountGroup oDT_AR_AccountGroup = new DT_AR_AccountGroup();
        #endregion
        #region Constructors
        public CT_AR_AccountGroup()
        {
            
        }
        #endregion
        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_AR_AccountGroup> GetAll()
        {
            return oDT_AR_AccountGroup.GetAll();
        }

        public Respuesta Save(EN_AR_AccountGroup accountGroup, EN_ExtCentral_AR_AccountGroup extCentral_AR_AccountGroup)
        {
            return oDT_AR_AccountGroup.SaveAccountGroup(accountGroup, extCentral_AR_AccountGroup);
        }
        #endregion
    }
}
