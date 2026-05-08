using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ExtCentral_Customer
    {
        #region Variables
        DT_ExtCentral_Customer oDT_ExtCentral_Customers = new DT_ExtCentral_Customer();
        #endregion
        #region Constructors
        public CT_ExtCentral_Customer()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_ExtCentral_Customer> GetAll()
        {
            return oDT_ExtCentral_Customers.GetAll();
        }
        #endregion
    }
}
