using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ItemStore
    {
        #region Fields
        private int itemID;
        private int storeID;
        private int supplierID;

        private string storeName;
        private string supplierName;

        private decimal quantity;
        private decimal cost;
        private decimal utility;
        private decimal invoiceDiscount;
        private decimal customerDiscount;

        public decimal GrossCost { get; set; }

        private DateTime lastReceived;
        #endregion

        #region Properties
        public int ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }
        public int StoreID
        {
            get { return storeID; }
            set { storeID = value; }
        }
        public int SupplierID
        {
            get { return supplierID; }
            set { supplierID = value; }
        }

        public string StoreName
        {
            get { return storeName; }
            set { storeName = value; }
        }
        public string SupplierName
        {
            get { return supplierName; }
            set { supplierName = value; }
        }

        public decimal Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public decimal Cost
        {
            get { return cost; }
            set { cost = value; }
        }
        public decimal Utility
        {
            get { return utility; }
            set { utility = value; }
        }
        public decimal InvoiceDiscount
        {
            get { return invoiceDiscount; }
            set { invoiceDiscount = value; }
        }
        public decimal CustomerDiscount
        {
            get { return customerDiscount; }
            set { customerDiscount = value; }
        }

        public DateTime LastReceived
        {
            get { return lastReceived; }
            set { lastReceived = value; }
        }

        public string LastReceivedAux
        {
            get { return lastReceived != null && lastReceived.Year != 1900? lastReceived.ToString("dd/MM/yyyy") : ""; }
        }
        #endregion
    }
}
