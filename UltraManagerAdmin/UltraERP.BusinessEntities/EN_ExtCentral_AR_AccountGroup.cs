using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ExtCentral_AR_AccountGroup
    {
        #region Fields
        private int accountGroupID;
        private string storeID;
        private string prefix;
        #endregion

        #region Constructores
        public EN_ExtCentral_AR_AccountGroup(){ }
        #endregion

        #region Properties
        public virtual int AccountGroupID
        {
            get { return accountGroupID; }
            set { accountGroupID = value; }
        }
        public virtual string StoreID
        {
            get { return storeID; }
            set { storeID = value; }
        }
        public virtual string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }
        #endregion
    }
}
