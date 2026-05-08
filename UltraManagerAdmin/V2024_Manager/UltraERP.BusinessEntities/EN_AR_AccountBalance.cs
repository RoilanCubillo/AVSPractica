using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_AR_AccountBalance
    {
        #region Fields
        private int iD;
        private int number;
        private decimal? amount;
        private decimal? amountLCY;
        #endregion
        #region Constructors
        public EN_AR_AccountBalance()
        {
            
        }
        public EN_AR_AccountBalance(int ID, int Number, decimal? Amount, decimal? AmountLCY)
        {
            this.iD = ID;
            this.number = Number;
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
        public virtual decimal? Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        public virtual decimal? AmountLCY
        {
            get { return amountLCY; }
            set { amountLCY = value; }
        }
        #endregion
    }
}
