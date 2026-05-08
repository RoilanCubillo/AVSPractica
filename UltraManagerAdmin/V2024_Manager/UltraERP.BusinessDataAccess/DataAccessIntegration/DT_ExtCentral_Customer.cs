using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ExtCentral_Customer : DT
    {
        #region Constructors
        public DT_ExtCentral_Customer() : base() { }
        #endregion

        #region Methods
        public List<EN_ExtCentral_Customer> GetAll()
        {
            return (from p in db.UEP_EXTCENTRAL_CUSTOMER_GETALL(0)
                   select new EN_ExtCentral_Customer()
                   {
                       CustomerID = p.CustomerID.Value,
                       AccountGroupID = p.AccountGroupID.Value,
                       Prefix = p.Prefix
                   }).ToList();
        }
        #endregion
    }
}
