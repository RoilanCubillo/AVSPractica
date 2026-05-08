using System;

namespace UltraERP.BusinessEntities
{
    public class EN_QuantityDiscount
    {
        #region Fields

        private string description;
        private int hQID;
        private int iD;
        private bool discountOddItems;
        private float quantity1;
        private decimal price1;
        private decimal price1A;
        private decimal price1B;
        private decimal price1C;
        private float quantity2;
        private decimal price2;
        private decimal price2A;
        private decimal price2B;
        private decimal price2C;
        private float quantity3;
        private decimal price3;
        private decimal price3A;
        private decimal price3B;
        private decimal price3C;
        private float quantity4;
        private decimal price4;
        private decimal price4A;
        private decimal price4B;
        private decimal price4C;
        private int type;
        private float percentOffPrice1;
        private float percentOffPrice1A;
        private float percentOffPrice1B;
        private float percentOffPrice1C;
        private float percentOffPrice2;
        private float percentOffPrice2A;
        private float percentOffPrice2B;
        private float percentOffPrice2C;
        private float percentOffPrice3;
        private float percentOffPrice3A;
        private float percentOffPrice3B;
        private float percentOffPrice3C;
        private float percentOffPrice4;
        private float percentOffPrice4A;
        private float percentOffPrice4B;
        private float percentOffPrice4C;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the EN_QuantityDiscount class.
        /// </summary>
        public EN_QuantityDiscount()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EN_QuantityDiscount class.
        /// </summary>
        public EN_QuantityDiscount(string description, int hQID, bool discountOddItems, int type,
            float quantity1, decimal price1, decimal price1A, decimal price1B, decimal price1C,
            float quantity2, decimal price2, decimal price2A, decimal price2B, decimal price2C,
            float quantity3, decimal price3, decimal price3A, decimal price3B, decimal price3C,
            float quantity4, decimal price4, decimal price4A, decimal price4B, decimal price4C,
            float percentOffPrice1, float percentOffPrice1A, float percentOffPrice1B, float percentOffPrice1C,
            float percentOffPrice2, float percentOffPrice2A, float percentOffPrice2B, float percentOffPrice2C,
            float percentOffPrice3, float percentOffPrice3A, float percentOffPrice3B, float percentOffPrice3C,
            float percentOffPrice4, float percentOffPrice4A, float percentOffPrice4B, float percentOffPrice4C)
        {
            this.description = description;
            this.hQID = hQID;
            this.discountOddItems = discountOddItems;
            this.quantity1 = quantity1;
            this.price1 = price1;
            this.price1A = price1A;
            this.price1B = price1B;
            this.price1C = price1C;
            this.quantity2 = quantity2;
            this.price2 = price2;
            this.price2A = price2A;
            this.price2B = price2B;
            this.price2C = price2C;
            this.quantity3 = quantity3;
            this.price3 = price3;
            this.price3A = price3A;
            this.price3B = price3B;
            this.price3C = price3C;
            this.quantity4 = quantity4;
            this.price4 = price4;
            this.price4A = price4A;
            this.price4B = price4B;
            this.price4C = price4C;
            this.type = type;
            this.percentOffPrice1 = percentOffPrice1;
            this.percentOffPrice1A = percentOffPrice1A;
            this.percentOffPrice1B = percentOffPrice1B;
            this.percentOffPrice1C = percentOffPrice1C;
            this.percentOffPrice2 = percentOffPrice2;
            this.percentOffPrice2A = percentOffPrice2A;
            this.percentOffPrice2B = percentOffPrice2B;
            this.percentOffPrice2C = percentOffPrice2C;
            this.percentOffPrice3 = percentOffPrice3;
            this.percentOffPrice3A = percentOffPrice3A;
            this.percentOffPrice3B = percentOffPrice3B;
            this.percentOffPrice3C = percentOffPrice3C;
            this.percentOffPrice4 = percentOffPrice4;
            this.percentOffPrice4A = percentOffPrice4A;
            this.percentOffPrice4B = percentOffPrice4B;
            this.percentOffPrice4C = percentOffPrice4C;
        }

        /// <summary>
        /// Initializes a new instance of the EN_QuantityDiscount class.
        /// </summary>
        public EN_QuantityDiscount(string description, int hQID, int iD, bool discountOddItems, int type,
            float quantity1, decimal price1, decimal price1A, decimal price1B, decimal price1C,
            float quantity2, decimal price2, decimal price2A, decimal price2B, decimal price2C,
            float quantity3, decimal price3, decimal price3A, decimal price3B, decimal price3C,
            float quantity4, decimal price4, decimal price4A, decimal price4B, decimal price4C,
            float percentOffPrice1, float percentOffPrice1A, float percentOffPrice1B, float percentOffPrice1C,
            float percentOffPrice2, float percentOffPrice2A, float percentOffPrice2B, float percentOffPrice2C,
            float percentOffPrice3, float percentOffPrice3A, float percentOffPrice3B, float percentOffPrice3C,
            float percentOffPrice4, float percentOffPrice4A, float percentOffPrice4B, float percentOffPrice4C)
        {
            this.description = description;
            this.hQID = hQID;
            this.iD = iD;
            this.discountOddItems = discountOddItems;
            this.quantity1 = quantity1;
            this.price1 = price1;
            this.price1A = price1A;
            this.price1B = price1B;
            this.price1C = price1C;
            this.quantity2 = quantity2;
            this.price2 = price2;
            this.price2A = price2A;
            this.price2B = price2B;
            this.price2C = price2C;
            this.quantity3 = quantity3;
            this.price3 = price3;
            this.price3A = price3A;
            this.price3B = price3B;
            this.price3C = price3C;
            this.quantity4 = quantity4;
            this.price4 = price4;
            this.price4A = price4A;
            this.price4B = price4B;
            this.price4C = price4C;
            this.type = type;
            this.percentOffPrice1 = percentOffPrice1;
            this.percentOffPrice1A = percentOffPrice1A;
            this.percentOffPrice1B = percentOffPrice1B;
            this.percentOffPrice1C = percentOffPrice1C;
            this.percentOffPrice2 = percentOffPrice2;
            this.percentOffPrice2A = percentOffPrice2A;
            this.percentOffPrice2B = percentOffPrice2B;
            this.percentOffPrice2C = percentOffPrice2C;
            this.percentOffPrice3 = percentOffPrice3;
            this.percentOffPrice3A = percentOffPrice3A;
            this.percentOffPrice3B = percentOffPrice3B;
            this.percentOffPrice3C = percentOffPrice3C;
            this.percentOffPrice4 = percentOffPrice4;
            this.percentOffPrice4A = percentOffPrice4A;
            this.percentOffPrice4B = percentOffPrice4B;
            this.percentOffPrice4C = percentOffPrice4C;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Description value.
        /// </summary>
        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Gets or sets the HQID value.
        /// </summary>
        public virtual int HQID
        {
            get { return hQID; }
            set { hQID = value; }
        }

        /// <summary>
        /// Gets or sets the ID value.
        /// </summary>
        public virtual int ID
        {
            get { return iD; }
            set { iD = value; }
        }

        /// <summary>
        /// Gets or sets the DiscountOddItems value.
        /// </summary>
        public virtual bool DiscountOddItems
        {
            get { return discountOddItems; }
            set { discountOddItems = value; }
        }

        /// <summary>
        /// Gets or sets the Quantity1 value.
        /// </summary>
        public virtual float Quantity1
        {
            get { return quantity1; }
            set { quantity1 = value; }
        }

        /// <summary>
        /// Gets or sets the Price1 value.
        /// </summary>
        public virtual decimal Price1
        {
            get { return price1; }
            set { price1 = value; }
        }

        /// <summary>
        /// Gets or sets the Price1A value.
        /// </summary>
        public virtual decimal Price1A
        {
            get { return price1A; }
            set { price1A = value; }
        }

        /// <summary>
        /// Gets or sets the Price1B value.
        /// </summary>
        public virtual decimal Price1B
        {
            get { return price1B; }
            set { price1B = value; }
        }

        /// <summary>
        /// Gets or sets the Price1C value.
        /// </summary>
        public virtual decimal Price1C
        {
            get { return price1C; }
            set { price1C = value; }
        }

        /// <summary>
        /// Gets or sets the Quantity2 value.
        /// </summary>
        public virtual float Quantity2
        {
            get { return quantity2; }
            set { quantity2 = value; }
        }

        /// <summary>
        /// Gets or sets the Price2 value.
        /// </summary>
        public virtual decimal Price2
        {
            get { return price2; }
            set { price2 = value; }
        }

        /// <summary>
        /// Gets or sets the Price2A value.
        /// </summary>
        public virtual decimal Price2A
        {
            get { return price2A; }
            set { price2A = value; }
        }

        /// <summary>
        /// Gets or sets the Price2B value.
        /// </summary>
        public virtual decimal Price2B
        {
            get { return price2B; }
            set { price2B = value; }
        }

        /// <summary>
        /// Gets or sets the Price2C value.
        /// </summary>
        public virtual decimal Price2C
        {
            get { return price2C; }
            set { price2C = value; }
        }

        /// <summary>
        /// Gets or sets the Quantity3 value.
        /// </summary>
        public virtual float Quantity3
        {
            get { return quantity3; }
            set { quantity3 = value; }
        }

        /// <summary>
        /// Gets or sets the Price3 value.
        /// </summary>
        public virtual decimal Price3
        {
            get { return price3; }
            set { price3 = value; }
        }

        /// <summary>
        /// Gets or sets the Price3A value.
        /// </summary>
        public virtual decimal Price3A
        {
            get { return price3A; }
            set { price3A = value; }
        }

        /// <summary>
        /// Gets or sets the Price3B value.
        /// </summary>
        public virtual decimal Price3B
        {
            get { return price3B; }
            set { price3B = value; }
        }

        /// <summary>
        /// Gets or sets the Price3C value.
        /// </summary>
        public virtual decimal Price3C
        {
            get { return price3C; }
            set { price3C = value; }
        }

        /// <summary>
        /// Gets or sets the Quantity4 value.
        /// </summary>
        public virtual float Quantity4
        {
            get { return quantity4; }
            set { quantity4 = value; }
        }

        /// <summary>
        /// Gets or sets the Price4 value.
        /// </summary>
        public virtual decimal Price4
        {
            get { return price4; }
            set { price4 = value; }
        }

        /// <summary>
        /// Gets or sets the Price4A value.
        /// </summary>
        public virtual decimal Price4A
        {
            get { return price4A; }
            set { price4A = value; }
        }

        /// <summary>
        /// Gets or sets the Price4B value.
        /// </summary>
        public virtual decimal Price4B
        {
            get { return price4B; }
            set { price4B = value; }
        }

        /// <summary>
        /// Gets or sets the Price4C value.
        /// </summary>
        public virtual decimal Price4C
        {
            get { return price4C; }
            set { price4C = value; }
        }

        /// <summary>
        /// Gets or sets the Type value.
        /// </summary>
        public virtual int Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice1 value.
        /// </summary>
        public virtual float PercentOffPrice1
        {
            get { return percentOffPrice1; }
            set { percentOffPrice1 = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice1A value.
        /// </summary>
        public virtual float PercentOffPrice1A
        {
            get { return percentOffPrice1A; }
            set { percentOffPrice1A = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice1B value.
        /// </summary>
        public virtual float PercentOffPrice1B
        {
            get { return percentOffPrice1B; }
            set { percentOffPrice1B = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice1C value.
        /// </summary>
        public virtual float PercentOffPrice1C
        {
            get { return percentOffPrice1C; }
            set { percentOffPrice1C = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice2 value.
        /// </summary>
        public virtual float PercentOffPrice2
        {
            get { return percentOffPrice2; }
            set { percentOffPrice2 = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice2A value.
        /// </summary>
        public virtual float PercentOffPrice2A
        {
            get { return percentOffPrice2A; }
            set { percentOffPrice2A = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice2B value.
        /// </summary>
        public virtual float PercentOffPrice2B
        {
            get { return percentOffPrice2B; }
            set { percentOffPrice2B = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice2C value.
        /// </summary>
        public virtual float PercentOffPrice2C
        {
            get { return percentOffPrice2C; }
            set { percentOffPrice2C = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice3 value.
        /// </summary>
        public virtual float PercentOffPrice3
        {
            get { return percentOffPrice3; }
            set { percentOffPrice3 = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice3A value.
        /// </summary>
        public virtual float PercentOffPrice3A
        {
            get { return percentOffPrice3A; }
            set { percentOffPrice3A = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice3B value.
        /// </summary>
        public virtual float PercentOffPrice3B
        {
            get { return percentOffPrice3B; }
            set { percentOffPrice3B = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice3C value.
        /// </summary>
        public virtual float PercentOffPrice3C
        {
            get { return percentOffPrice3C; }
            set { percentOffPrice3C = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice4 value.
        /// </summary>
        public virtual float PercentOffPrice4
        {
            get { return percentOffPrice4; }
            set { percentOffPrice4 = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice4A value.
        /// </summary>
        public virtual float PercentOffPrice4A
        {
            get { return percentOffPrice4A; }
            set { percentOffPrice4A = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice4B value.
        /// </summary>
        public virtual float PercentOffPrice4B
        {
            get { return percentOffPrice4B; }
            set { percentOffPrice4B = value; }
        }

        /// <summary>
        /// Gets or sets the PercentOffPrice4C value.
        /// </summary>
        public virtual float PercentOffPrice4C
        {
            get { return percentOffPrice4C; }
            set { percentOffPrice4C = value; }
        }

        #endregion
    }
}
