using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_Customer
    {
        #region Variables
        DT_Customer oDT_Customer = new DT_Customer();
        #endregion
        #region Constructors
        public CT_Customer()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_Customer> GetAll()
        {
            return oDT_Customer.GetAll();
        }
        public Respuesta Save(string prefijo, int accountGroupID, EN_Customer customer)
        {
            return oDT_Customer.SaveCustomer(prefijo, accountGroupID, customer);
        }
        #endregion
    }
}
