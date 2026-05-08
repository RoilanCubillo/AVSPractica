using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_CustomCaption
    {
        #region Fields
        private int iD;
        private int hQID;
        private int style;
        private string caption;
        private string dBTimeStamp;
        private Guid syncGuid;
        #endregion
        #region Constructors
        public EN_CustomCaption()
        {
            
        }
        public EN_CustomCaption(int ID, int HQID, int Style, string Caption, string DBTimeStamp, Guid SyncGuid)
        {
            this.iD = ID;
            this.hQID = HQID;
            this.style = Style;
            this.caption = Caption;
            this.dBTimeStamp = DBTimeStamp;
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
            get { return iD; }
            set { iD = value; }
        }
        public virtual int HQID
        {
            get { return hQID; }
            set { hQID = value; }
        }
        public virtual int Style
        {
            get { return style; }
            set { style = value; }
        }
        public virtual string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
        public virtual string DBTimeStamp
        {
            get { return dBTimeStamp; }
            set { dBTimeStamp = value; }
        }
        public virtual Guid SyncGuid
        {
            get { return syncGuid; }
            set { syncGuid = value; }
        }
        #endregion
    }
}
