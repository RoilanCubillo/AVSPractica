using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_StoreGroup : DT
    {
        #region Variables
        #endregion

        #region constructor
        public DT_StoreGroup() : base() { }
        #endregion

        #region Methods
        public List<EN_StoreGroup> GetAll(string stores_Id)
        {
            List<EN_StoreGroup> list = (
                from i in db.UEP_STOREGROUP_GETALL(stores_Id)
                select new EN_StoreGroup() { ID = i.ID, Code = i.Code, Description = i.Description }
            ).ToList();

            return list;
        }
        #endregion
    }
}
