using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_SupplierList
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int SupplierID { get; set; }
        public decimal MinimumOrder { get; set; }
        public decimal Cost { get; set; }
        public string ReorderNumber { get; set; }
        public int MasterPackQuantity { get; set; }
        public decimal TaxRate { get; set; }
        public bool IsPrimary { get; set; }
        public string ItemLookupCode { get; set; }
        public string ItemDescription { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }
}
