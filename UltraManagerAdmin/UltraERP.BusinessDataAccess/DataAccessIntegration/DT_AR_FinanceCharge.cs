using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_AR_FinanceCharge : DT
    {
        #region Variables
        protected EN_AR_FinanceCharge oEN_AR_FinanceCharge = new EN_AR_FinanceCharge();
        #endregion
        #region Constructors
        public DT_AR_FinanceCharge() : base() { }
        #endregion
        #region Methods
        public List<EN_AR_FinanceCharge> GetAll()
        {
            return(from p in db.UEP_AR_FINANCECHARGE_GETALL("", true, 0)
                   select new EN_AR_FinanceCharge()
                   {
                       ID = p.ID,
                       Code = p.Code,
                       Name = p.Name,
                       ExtCode = p.ExtCode,
                       Inactive = p.Inactive,
                       MlText = p.MLText,
                       MinimumCharge = p.MinimumCharge,
                       AnnuallnterestRate = p.AnnualInterestRate,
                       ApplyToFinCharge = p.ApplyToFinCharge
                   }).ToList();
        }
        #endregion
    }
}
