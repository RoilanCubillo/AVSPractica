using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ExtCentral_Customer
    {
        #region Fields
        private int customerID;
        private int accountGroupID;
        private string prefix;
        #endregion


        #region Constructors
        public EN_ExtCentral_Customer()
        {
            
        }
        public EN_ExtCentral_Customer(int CustomerID, int AccountGroupID, string Prefix)
        {
            this.customerID = CustomerID;
            this.accountGroupID = AccountGroupID;
            this.prefix = Prefix;
        }
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the ID value.
        /// </summary>
        /// 
        public virtual int CustomerID
        {
            get { return customerID; }
            set { customerID = value; }
        }
        public virtual int AccountGroupID
        {
            get { return accountGroupID; }
            set { accountGroupID = value; }
        }
        public virtual string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }
        #endregion
    }
}
