using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_POC_ItemPrice
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int SupplierID { get; set; }
        public int CurrencyID { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }
        public string StartingDateAux { get { return StartingDate == null ? "" : ((DateTime)StartingDate).ToString("dd/MM/yyyy"); } }
        public string EndingDateAux { get { return EndingDate == null ? "" : ((DateTime)EndingDate).ToString("dd/MM/yyyy"); } }
        public int UOMID { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public string ItemLookupCode { get; set; }
        public string ItemDescription { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string CurrencyCode { get; set; }
    }
}
