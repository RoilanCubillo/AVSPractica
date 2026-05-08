using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;
using UltraERP.BusinessDataAccess.DataAccessIntegration;

namespace UltraERP.BusinessLogic
{
    public class CT_POC_ItemPrice
    {
        public CT_POC_ItemPrice() { }

        public List<EN_POC_ItemPrice> GetAllByItemID(int itemID, string storesID)
        {
            return new DT_POC_ItemPrice().GetAllByItemID(itemID, storesID);
        }
    }
}
