using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_AR_StatementType
    {
        #region Fields
        private int Id;
        private string dbTimeStamp;
        private string code;
        private string name;
        private string extCode;
        private bool inactive;
        private string mlText;
        private int dataSet;
        private string template;
        private string options;
        private Guid syncGuid;
        #endregion
        #region Constructors
        public EN_AR_StatementType()
        {
            
        }
        public EN_AR_StatementType(int ID, string DBTimeStamp, string Code, string Name, string ExtCode, bool Inactive, string MLText,
            int DataSet, string Template, string Options, Guid SyncGuid)
        {
            this.Id = ID;
            this.dbTimeStamp = DBTimeStamp;
            this.code = Code;
            this.name = Name;
            this.extCode = ExtCode;
            this.inactive = Inactive;
            this.mlText = MLText;
            this.dataSet = DataSet;
            this.template = Template;
            this.options = Options;
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
        public virtual string DBTimesStamp
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
        public virtual string ExtCode
        {
            get { return extCode; }
            set { extCode = value; }
        }
        public virtual bool Inactive
        {
            get { return inactive; }
            set { inactive = value; }
        }
        public virtual string MLText
        {
            get { return mlText; }
            set { mlText = value; }
        }
        public virtual int DataSet
        {

            get { return dataSet; }
            set { dataSet = value; }
        }
        public virtual string Template
        {

            get { return template; }
            set { template = value; }
        }
        public virtual string Options
        {

            get { return options; }
            set { options = value; }
        }
        public virtual Guid SyncGuid
        {
            get { return syncGuid; }
            set { syncGuid = value; }
        }
        #endregion
    }
}
