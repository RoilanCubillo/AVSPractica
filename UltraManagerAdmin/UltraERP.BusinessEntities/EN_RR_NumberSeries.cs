using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_RR_NumberSeries
    {
        #region Fields
        private int Id;
        private string dbTimeStamp;
        private string code;
        private string name;
        private bool inactive;
        private string prefix;
        private int noOfDigit;
        private int lastUsed;
        private Guid syncGuid;
        #endregion
        #region Constructors
        public EN_RR_NumberSeries()
        {
            
        }
        public EN_RR_NumberSeries(int ID, string DbTimeStamp, string Code, string Name, bool Inactive,
            string Prefix, int NoOfDigit, int LastUsed, Guid SyncGuid)
        {
            this.Id = ID;
            this.dbTimeStamp = DbTimeStamp;
            this.code = Code;
            this.name = Name;
            this.inactive = Inactive;
            this.prefix = Prefix;
            this.noOfDigit = NoOfDigit;
            this.lastUsed = LastUsed;
            this.syncGuid = SyncGuid;
            
        }
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the ID value.
        /// </summary>
        /// 
        public virtual int ID
        {
            get { return Id; }
            set { Id = value; }
        }
        public virtual string DbTimeStamp
        {
            get { return dbTimeStamp; }
            set { dbTimeStamp = value; }
        }
        public virtual string Code
        {
            get { return code; }
            set { code = value; }
        }
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
        public virtual bool Inactive
        {
            get { return inactive; }
            set { inactive = value; }
        }
        public virtual string Prefix
        {

            get { return prefix; }
            set { prefix = value; }
        }
        public virtual int NoOfDigit
        {

            get { return noOfDigit; }
            set { noOfDigit = value; }
        }
        public virtual int LastUsed
        {

            get { return lastUsed; }
            set { lastUsed = value; }
        }
        public virtual Guid SyncGuid
        {
            get { return syncGuid; }
            set { syncGuid = value; }
        }
        #endregion
    }
}
