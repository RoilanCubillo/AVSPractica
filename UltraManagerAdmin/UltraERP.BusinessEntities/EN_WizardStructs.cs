using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_WizardStructs
    {
        public struct TablaItemSub1
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public string Description { get; set; }
            public string OldSubDescription1 { get; set; }
            public string SubDescription1 { get; set; }
        }

        public struct TablaItemSub2
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public string Description { get; set; }
            public string OldSubDescription2 { get; set; }
            public string SubDescription2 { get; set; }
        }
        
        public struct TablaItemSub3
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public string Description { get; set; }
            public string OldCabys { get; set; }
            public string SubDescription3 { get; set; }
        }

        public struct TablaCost
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public string Description { get; set; }
            public string MontoAnterior { get; set; }
            public string NuevoMonto { get; set; }
        }
        
        public struct TablaEstado
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public string Description { get; set; }
            public string AnteriorEstado { get; set; }
            public string NuevoEstado { get; set; }
        }

        public struct TablaItemTax
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public string Description { get; set; }
            public string TaxPerAnterior { get; set; }
            public string TaxPercentage { get; set; }
        }

        public struct TablaItemDes
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public string Description { get; set; }
            public string DesAnterior { get; set; }
            public string NuevaDes { get; set; }
        }

        public struct TablaItemProveedor
        {
            public int ID { get; set; }
            public string SupplierCode { get; set; }
            public string ItemLookupCode { get; set; }
            public string Cost { get; set; }
            public string Utility { get; set; }
            public string InvoiceDiscount { get; set; }
            public string CustomerDiscount { get; set; }
            public string StartDate { get; set; }
        }

        public struct TablaItemPriceDynamic
        {
            public int ID { get; set; }
            public string SupplierCode { get; set; }
            public string ItemLookupCode { get; set; }
            public string InvoiceDiscount { get; set; }
            public string CustomerDiscount { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string SalePrice { get; set; }
            public string Quantity { get; set; }
        }

        public struct TablaItemQuantityDiscount
        {
            public int ID { get; set; }
            public string SupplierCode { get; set; }
            public string ItemLookupCode { get; set; }
            public string InvoiceDiscount { get; set; }
            public string CustomerDiscount { get; set; }
        }

        public struct TablaItemPriceRegular
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public string InvoiceDiscount { get; set; }
            public string CustomerDiscount { get; set; }
            public string Cost { get; set; }
            public string Utility { get; set; }
        }

        public struct TablaItem
        {
            public string ItemLookupCode { get; set; }
            public string Description { get; set; }
            public string DescriptionExtended { get; set; }
            public string Subdescription3 { get; set; }
            public string Subdescription4 { get; set; }
            public string Subdescription5 { get; set; }
            public string Subdescription6 { get; set; }
            public string Subdescription7 { get; set; }
            public string Subdescription8 { get; set; }
            public string Subdescription9{ get; set; }
            public string Subdescription10 { get; set; }
            public string UOM { get; set; }
            public string FamilyCode { get; set; }
            public string DepartmentCode { get; set; }
            public string CategoryCode { get; set; }
            public string SubCategoryCode { get; set; }
            public string SegmentCode { get; set; }
            public int ManufacturerCode { get; set; }
            public int BrandCode { get; set; }
            public string PurchaserCode { get; set; }
            public string SupplierCode { get; set; }
            public string Cost { get; set; }
            public string MSRP { get; set; }
            public string Utility { get; set; }
            public string InvoiceDiscount { get; set; }
            public string CustomerDiscount { get; set; }
            public int TaxID { get; set; }
        }

        public struct TablaMargenUtility
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public string Utility { get; set; }
        }

        public struct TablaItemActivate
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public int SupplierID { get; set; }
            public string SupplierCode { get; set; }
            public string GrossCost { get; set; }
            public string Utility { get; set; }
            public string InvoiceDiscount { get; set; }
            public string CustomerDiscount { get; set; }
        }

        public struct TablaItemProperties
        {
            public int ID { get; set; }
            public string ItemLookupCode { get; set; }
            public string Properties { get; set; }
        }
    }
}
