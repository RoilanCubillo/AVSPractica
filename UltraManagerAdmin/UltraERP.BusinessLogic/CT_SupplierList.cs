using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;
using UltraERP.BusinessDataAccess.DataAccessIntegration;

namespace UltraERP.BusinessLogic
{
    public class CT_SupplierList
    {
        public CT_SupplierList() { }

        public List<EN_SupplierList> GetAllByItemID(int itemID, string storesID)
        {
            return new DT_SupplierList().GetAllByItemID(itemID, storesID);
        }
    }
}
