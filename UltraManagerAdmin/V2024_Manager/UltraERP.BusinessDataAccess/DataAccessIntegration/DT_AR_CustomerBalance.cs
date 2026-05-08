using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_AR_CustomerBalance : DT
    {

        #region Constructors
        public DT_AR_CustomerBalance() : base() { }
        #endregion

        #region Methods
        public List<EN_AR_CustomerBalance> GetAll()
        {
            return (from p in db.UEP_AR_CUSTOMERBALANCE_GETALL(null, true, 0)
                    select new EN_AR_CustomerBalance()
                    {
                        ID = p.ID,
                        Number = (int)p.Number,
                        Amount = p.Amount,
                        AmountLCY = p.AmountLCY
                    }).ToList();
        }
        #endregion
    }
}
