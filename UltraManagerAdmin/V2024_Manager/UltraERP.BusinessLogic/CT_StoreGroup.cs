using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_StoreGroup
    {
        private DT_StoreGroup dtObj = new DT_StoreGroup();

        public List<EN_StoreGroup> GetAll(string stores_Id)
        {
            return dtObj.GetAll(stores_Id);
        }
    }
}