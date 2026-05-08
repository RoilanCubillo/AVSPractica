using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ExtCentral_SubCategory
    {

        #region Fields
        private int iD;
        private string description;
        private string code;
        private int categoryID;
        #endregion

        #region Constructores
        public EN_ExtCentral_SubCategory() { }
        #endregion

        #region Properties
        public virtual int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }
        public virtual string Code
        {
            get { return code; }
            set { code = value; }
        }
        public virtual int CategoryID
        {
            get { return categoryID; }
            set { categoryID = value; }
        }
        #endregion
    }
}
