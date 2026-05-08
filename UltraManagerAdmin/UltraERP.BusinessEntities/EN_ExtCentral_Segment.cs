using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ExtCentral_Segment
    {
        #region Fields
        private int iD;
        private int subCategoryID;
        private string code;
        private string description;
        #endregion

        #region Constructores
        public EN_ExtCentral_Segment() { }
        #endregion

        #region Properties
        public virtual int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        public virtual int SubCategoryID
        {
            get { return subCategoryID; }
            set { subCategoryID = value; }
        }
        public virtual string Code
        {
            get { return code; }
            set { code = value; }
        }
        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }
        #endregion
    }
}
