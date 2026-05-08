using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_SupplierList : DT
    {
        public DT_SupplierList() : base() { }

        public List<EN_SupplierList> GetAllByItemID(int itemID, string storesID)
        {
            List<EN_SupplierList> list = (
                from i in db.UEP_SUPPLIERLIST_GETALL_BY_ITEM(itemID, storesID)
                select new EN_SupplierList()
                {
                    ID = i.ID,
                    ItemID = i.ItemID,
                    SupplierID = i.SupplierID,
                    Cost = i.Cost,
                    IsPrimary = i.IsPrimary == true,
                    ItemDescription = i.ItemDescription,
                    ItemLookupCode = i.ItemLookupCode,
                    MasterPackQuantity = i.MasterPackQuantity,
                    MinimumOrder = Convert.ToDecimal(i.MinimumOrder),
                    ReorderNumber = i.ReorderNumber,
                    SupplierCode = i.SupplierCode,
                    SupplierName = i.SupplierName,
                    TaxRate = Convert.ToDecimal(i.TaxRate)
                }
            ).ToList();

            return list;
        }
    }
}
