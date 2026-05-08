using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ExtCentral_AR_AccountDynamic : DT
    {
        #region Constructors
        public DT_ExtCentral_AR_AccountDynamic() : base() { }
        #endregion

        #region Methods
        public List<EN_ExtCentral_AR_AccountDynamic> GetAll()
        {
            return(from p in db.UEP_EXTCENTRAL_AR_ACCOUNTDYNAMIC_GETALL(null, true, 0)
                   select new EN_ExtCentral_AR_AccountDynamic()
                   {
                        AccountID = p.AccountID,
                        StoreID = p.StoreID,
                        Status = p.Status,
                        CreditLimit = p.CreditLimit,
                        LastUpdated = p.LastUpdated
                   }).ToList();
        }
        public List<EN_ExtCentral_AR_AccountDynamic> GetStoreBalance(int accountID, int groupID)
        {
            var result = (from p in db.fnAR_AccountStoreBalance(accountID, groupID)
                          select new EN_ExtCentral_AR_AccountDynamic()
                          {
                              AccountID = accountID,
                              StoreID = groupID,
                              Balance = (p.Balance != null) ? (decimal)p.Balance : 0, // Validación para Balance
                              BalanceDue = (p.Saldo_Vencido != null) ? (decimal)p.Saldo_Vencido : 0 // Validación para Saldo_Vencido
                          }).ToList();

            return result;
        }
        #endregion
    }
}
