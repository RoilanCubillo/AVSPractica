using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_AR_AccountGroup
    {
        #region Fields
        private int Id;
        private string dbTimeStamp;
        private string code;
        private string name;
        private string extCode;
        private bool inactive;
        private string mlText;
        private int payTermsId;
        private int finChargeID;
        private int currencyId;
        private int managerId;
        private int countryId;
        private int regionId;
        private int locationId;
        private decimal creditLimit;
        private int creditLimitCheck;
        private int statementType;
        private int applicationMethod;
        private int numberSeries;
        private Guid syncGuid;
        #endregion
        #region Constructors
        public EN_AR_AccountGroup()
        {
            
        }

        public EN_AR_AccountGroup(int ID, string DbTimeStamp, string Code, string Name, string ExtCode, bool Inactive, string MlText,
            int PayTermsId, int FinChargeID, int CurrencyId, int ManagerId, int CountryId, int RegionId, int LocationId, decimal CreditLimit,
            int CreditLimitCheck, int StatementType, int ApplicationMethod, int NumberSeries, Guid SyncGuid)
        {
            this.Id = ID;
            this.dbTimeStamp = DbTimeStamp;
            this.code = Code;
            this.name = Name;
            this.extCode = ExtCode;
            this.inactive = Inactive;
            this.mlText = MlText;
            this.payTermsId = PayTermsId;
            this.finChargeID = FinChargeID;
            this.currencyId = CurrencyId;
            this.managerId = ManagerId;
            this.countryId = CountryId;
            this.regionId = RegionId;
            this.locationId = LocationId;
            this.creditLimit = CreditLimit;
            this.creditLimitCheck = CreditLimitCheck;
            this.statementType = StatementType;
            this.applicationMethod = ApplicationMethod;
            this.numberSeries = NumberSeries;
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

        public virtual int PayTermsId
        {
            get { return payTermsId; }
            set { payTermsId = value; }
        }
        public virtual int FinChargeID
        {
            get { return finChargeID; }
            set { finChargeID = value; }
        }
        public virtual int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }
        public virtual int ManagerId
        {
            get { return managerId; }
            set { managerId = value; }
        }
        public virtual int CountryId
        {
            get { return countryId; }
            set { countryId = value; }
        }
        public virtual int RegionId
        {
            get { return regionId; }
            set { regionId = value; }
        }
        public virtual int LocationId
        {
            get { return locationId; }
            set { locationId = value; }
        }
        public virtual decimal CreditLimit
        {
            get { return creditLimit; }
            set { creditLimit = value; }
        }
        public virtual int CreditLimitCheck
        {
            get { return creditLimitCheck; }
            set { creditLimitCheck = value; }
        }
        public virtual int StatementType
        {
            get { return statementType; }
            set { statementType = value; }
        }
        public virtual int ApplicationMethod
        {
            get { return applicationMethod; }
            set { applicationMethod = value; }
        }
        public virtual int NumberSeries
        {
            get { return numberSeries; }
            set { numberSeries = value; }
        }
        public virtual Guid SyncGuid
        {
            get { return syncGuid; }
            set { syncGuid = value; }
        }
        #endregion
    }
}
