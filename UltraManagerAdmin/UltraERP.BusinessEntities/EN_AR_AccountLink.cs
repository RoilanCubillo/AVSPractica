using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_AR_AccountLink
    {
        #region Fields
        private int linkID;
        private int linkType;
        private int accountID;
        private int storeID;
        private Guid syncGuid;
        #endregion
        #region Constructors
        public EN_AR_AccountLink()
        {
            
        }
        public EN_AR_AccountLink(int LinkID, int LinkType, int AccountID, int StoreID, Guid SyncGuid)
        {
            this.linkID = LinkID;
            this.linkType = LinkType;
            this.accountID = AccountID;
            this.storeID = StoreID;
            this.syncGuid = SyncGuid;
        }
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the ID value.
        /// </summary>
        ///
        public virtual int LinkID
        {
            get { return linkID; }
            set { linkID = value; }
        }
        public virtual int LinkType
        {
            get { return linkType; }
            set { linkType = value; }
        }
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
        public virtual Guid SyncGuid
        {
            get { return syncGuid; }
            set { syncGuid = value; }
        }
        #endregion
    }
}
