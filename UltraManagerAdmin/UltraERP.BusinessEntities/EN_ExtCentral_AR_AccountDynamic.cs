using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ExtCentral_AR_AccountDynamic
    {
        #region Fields
        private int accountID;
        private int storeID;
        private int status;
        private decimal creditLimit;
        private decimal balance;
        private decimal balanceDue;
        private DateTime? lastUpdated;
        #endregion
        #region Constructors
        public EN_ExtCentral_AR_AccountDynamic()
        {
            
        }
        public EN_ExtCentral_AR_AccountDynamic(int AccountID, int StoreID, int Status, decimal CreditLimit, decimal Balance, decimal BalanceDue, DateTime? LastUpdated)
        {
            this.accountID = AccountID;
            this.storeID = StoreID;
            this.status = Status;
            this.creditLimit = CreditLimit;
            this.balance = Balance;
            this.balanceDue = BalanceDue;
            this.lastUpdated = LastUpdated;
        }
        #endregion
        #region Properties
        public virtual int AccountID
        {
            get { return accountID; }
            set { accountID = value; }
        }
        public virtual int StoreID
        {
            get { return storeID; }
            set { storeID = value; }
        }
        public virtual int Status
        {
            get { return status; }
            set { status = value; }
        }
        public virtual decimal CreditLimit
        {
            get { return creditLimit; }
            set { creditLimit = value; }
        }
        public virtual decimal Balance
        {

            get { return balance; }
            set { balance = value; }
        }
        public virtual decimal BalanceDue
        {

            get { return balanceDue; }
            set { balanceDue = value; }
        }
        public virtual DateTime? LastUpdated
        {
            get { return lastUpdated; }
            set { lastUpdated = value; }
        }
        #endregion
    }
}
