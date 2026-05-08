using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_POC_ItemPrice : DT
    {
        public DT_POC_ItemPrice() : base() { }

        public List<EN_POC_ItemPrice> GetAllByItemID(int itemID, string storesID)
        {
            List<EN_POC_ItemPrice> list = (
                from i in db.UEP_POC_ITEMPRICE_GETALL_BY_ITEM(itemID, storesID)
                select new EN_POC_ItemPrice()
                {
                    ID = i.ID,
                    CurrencyCode = i.CurrencyCode,
                    CurrencyID = i.CurrencyID,
                    EndingDate = i.EndingDate,
                    ItemDescription = i.ItemDescription,
                    ItemID = i.ItemID,
                    ItemLookupCode = i.ItemLookupCode,
                    MinQuantity = Convert.ToDecimal(i.MinQuantity),
                    StartingDate = i.StartingDate,
                    SupplierCode = i.SupplierCode,
                    SupplierID = i.SupplierID,
                    SupplierName = i.SupplierName,
                    UnitCost = i.UnitCost,
                    UOMID = i.UOMID
                }
            ).ToList();

            return list;
        }
    }
}
