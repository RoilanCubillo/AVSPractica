using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_AR_StatementType
    {
        #region Variables
        DT_AR_StatementType oDT_AR_StatementType = new DT_AR_StatementType();
        #endregion
        #region Constructors
        public CT_AR_StatementType()
        {

        }
        #endregion
        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_AR_StatementType> GetAll()
        {
            return oDT_AR_StatementType.GetAll();
        }
        #endregion
    }
}
