using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ExtCentral_AR_AccountGroup : DT
    {
        #region Variables
        protected EN_ExtCentral_AR_AccountGroup oEN_ExtCentral_AR_AccountGroup = new EN_ExtCentral_AR_AccountGroup();
        #endregion
        #region Constructores
        public DT_ExtCentral_AR_AccountGroup() : base() { }
        #endregion
        #region Métodos
        public List<EN_ExtCentral_AR_AccountGroup> GetAll()
        {
            return(from p in db.UEP_EXTCENTRAL_AR_ACCOUNTGROUP_GETALL()
                   select new EN_ExtCentral_AR_AccountGroup()
                   {
                       AccountGroupID = p.AccountGroupID,
                       StoreID = p.StoreID,
                       Prefix = p.Prefix
                   }).ToList();
        }
        #endregion
    }
}
