using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_AR_FinanceCharge
    {
        #region Fields
        private int Id;
        private string dbTimeStamp;
        private string code;
        private string name;
        private string extCode;
        private bool inactive;
        private string mlText;
        private decimal minimumCharge;
        private double annuallnterestRate;
        private bool applyToFinCharge;
        private Guid syncGuid;
        #endregion
        #region Constructors
        public EN_AR_FinanceCharge()
        {
            
        }
        public EN_AR_FinanceCharge(int ID, string DbTimeStamp, string Code, string Name, string ExtCode, bool Inactive, string MlText, decimal MinimumCharge,
            double AnnuallnterestRate, bool ApplyToFinCharge, Guid SyncGuid)
        {
            this.Id = ID;
            this.dbTimeStamp = DbTimeStamp;
            this.code = Code;
            this.name = Name;
            this.extCode = ExtCode;
            this.inactive = Inactive;
            this.mlText = MlText;
            this.minimumCharge = MinimumCharge;
            this.annuallnterestRate = AnnuallnterestRate;
            this.applyToFinCharge = ApplyToFinCharge;
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
        public virtual string MlText
        {
            get { return mlText; }
            set { mlText = value; }
        }
        public virtual decimal MinimumCharge
        { 
            get { return minimumCharge; }
            set { minimumCharge = value; }
        }
        public virtual double AnnuallnterestRate
        {
            get { return annuallnterestRate; }
            set {  annuallnterestRate = value; }
        }
        public virtual bool ApplyToFinCharge
        {
            get { return applyToFinCharge; }
            set { applyToFinCharge = value; }
        }
        public virtual Guid SyncGuid
        {
            get { return syncGuid; }
            set { syncGuid = value; }
        }
        #endregion

    }
}
