using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_AR_PaymentTerms
    {
        #region Variables
        DT_AR_PaymentTerms oDT_AR_PaymentTerms = new DT_AR_PaymentTerms();
        #endregion
        #region Constructors
        public CT_AR_PaymentTerms()
        {
            
        }
        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_AR_PaymentTerms> GetAll()
        {
            return oDT_AR_PaymentTerms.GetAll();
        }
        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public Respuesta Save(EN_AR_PaymentTerms paymentTerms)
        {
           return oDT_AR_PaymentTerms.SavePaymentTerms(paymentTerms);
        }
        #endregion
    }
}
