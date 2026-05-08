using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_Tax
    {
        #region Fields

        private int id;
        private string name;
        private float percentage;
        #endregion

        #region Constructors
        public EN_Tax() { }
        public EN_Tax(int id, string name, float percentage)
        {

            this.id = id;
            this.name = name;
            this.percentage = percentage;
        }
        #endregion

        #region Properties
        public virtual int ID
        {
            get { return id; }
            set { id = value; }
        }
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
        public virtual float Percentage
        {
            get { return percentage; }
            set { percentage = value; }
        }
        #endregion
    }
}
