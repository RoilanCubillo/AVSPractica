using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ExtCentral_Family
    {

		#region Fields
		private int iD;
		private string name;
		private string code;
        #endregion

        #region Constructores
        public EN_ExtCentral_Family() { }
        #endregion

        #region Properties
        public virtual int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
        public virtual string Code
        {
            get { return code; }
            set { code = value; }
        }
        #endregion

    }
}
