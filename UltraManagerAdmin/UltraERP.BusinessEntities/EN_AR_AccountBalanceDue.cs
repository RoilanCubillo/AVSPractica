using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_AR_AccountBalanceDue
    {
        #region Fields
        private int iD;
        private int number;
        private DateTime dueDate;
        private DateTime postingDate;
        private decimal amount;
        private decimal amountLCY;
        #endregion
        #region Constructors
        public EN_AR_AccountBalanceDue()
        {
            
        }
        public EN_AR_AccountBalanceDue(int ID, int Number, DateTime DueDate, DateTime PostingDate, decimal Amount, decimal AmountLCY)
        {
            this.iD = ID;
            this.number = Number;
            this.dueDate = DueDate;
            this.postingDate = PostingDate;
            this.amount = Amount;
            this.amountLCY = AmountLCY;
        }
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the ID value.
        /// </summary>
        ///
        public virtual int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        public virtual int Number
        {
            get { return number; }
            set { number = value; }
        }
        public virtual DateTime DueDate
        {
            get { return dueDate; }
            set { dueDate = value; }
        }
        public virtual DateTime PostingDate
        {
            get { return postingDate; }
            set { postingDate = value; }
        }
        public virtual decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        public virtual decimal AmountLCY
        {
            get { return amountLCY; }
            set { amountLCY = value; }
        }
        #endregion
    }
}
