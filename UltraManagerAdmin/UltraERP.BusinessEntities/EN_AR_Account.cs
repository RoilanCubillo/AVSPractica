using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_AR_Account
    {
        #region Fields
        private int iD;
        private DateTime? lastUpdated;
        private string number;
        private string name;
        private string extCode;
        private string title;
        private string firstName;
        private string middleName;
        private string lastName;
        private string suffix;
        private string company;
        private string jobTitle;
        private int type;
        private int role;
        private int status;
        private int billingAccount;
        private int groupID;
        private int payTermsID;
        private int finChargeID;
        private int currencyID;
        private int managerID;
        private int countryID;
        private int regionID;
        private int locationID;
        private string address1;
        private string address2;
        private string city;
        private string state;
        private string zip;
        private string country;
        private string phoneNumber;
        private string movileNumber;
        private string faxNumber;
        private string eMail;
        private string homePage;
        private string iMAddress;
        private decimal creditLimit;
        private int creditLimitCheck;
        private int statementType;
        private int statementDelivery;
        private string statementAddress;
        private int applicationMethod;
        private DateTime? closingDate;
        private decimal closingBalance;
        private int lastStatement;
        private int customCode1;
        private int customCode2;
        private int customCode3;
        private int customCode4;
        private int customCode5;
        private string customText1;
        private string customText2;
        private string customText3;
        private string customText4;
        private string customText5;
        private float customNumber1;
        private float customNumber2;
        private float customNumber3;
        private float customNumber4;
        private float customNumber5;
        private DateTime? customDate1;
        private DateTime? customDate2;
        private DateTime? customDate3;
        private DateTime? customDate4;
        private DateTime? customDate5;
        private byte[] picture;
        private string notes;
        private string extData;
        private DateTime? dateOpened;
        private DateTime? lastActivity;
        private int storeID;
        private Guid syncGuid;
        #endregion
        #region Constructors
        public EN_AR_Account()
        {
            
        }

        public EN_AR_Account(int Id, DateTime? LastUpdated, string Number, string Name, string ExtCode, string Title, string FirstName, string MiddleName,
            string LastName, string Suffix, string Company, string JobTitle, int Type, int Role, int Status, int BillingAccount, int GroupID, int PayTermsID,
            int FinChargeID, int CurrencyID, int ManagerID, int CountryID, int RegionID, int LocationID, string Address1, string Address2, string City,
            string State, string Zip, string Country, string PhoneNumber, string MovileNumber, string FaxNumber, string EMail, string HomePage, string IMAddress,
            decimal CreditLimit, int CreditLimitCheck, int StatementType, int StatementDelivery, string StatementAddress, int ApplicationMethod, DateTime? ClosingDate,
            decimal ClosingBalance, int LastStatement, int CustomCode1, int CustomCode2, int CustomCode3, int CustomCode4, int CustomCode5, string CustomText1,
            string CustomText2, string CustomText3, string CustomText4, string CustomText5, float CustomNumber1, float CustomNumber2, float CustomNumber3,
            float CustomNumber4, float CustomNumber5, DateTime? CustomDate1, DateTime? CustomDate2, DateTime? CustomDate3, DateTime? CustomDate4, DateTime? CustomDate5,
            byte[] Picture, string Notes, string ExtData, DateTime? DateOpened, DateTime? LastActivity, int StoreID, Guid SyncGuid)
        {
            this.iD = Id;
            this.lastUpdated = LastUpdated;
            this.number = Number;
            this.name = Name;
            this.extCode = ExtCode;
            this.title = Title;
            this.firstName = FirstName;
            this.middleName = MiddleName;
            this.lastName = LastName;
            this.suffix = Suffix;
            this.company = Company;
            this.jobTitle = JobTitle;
            this.type = Type;
            this.role = Role;
            this.status = Status;
            this.billingAccount = BillingAccount;
            this.groupID = GroupID;
            this.payTermsID = PayTermsID;
            this.finChargeID = FinChargeID;
            this.currencyID = CurrencyID;
            this.managerID = ManagerID;
            this.countryID = CountryID;
            this.regionID = RegionID;
            this.locationID = LocationID;
            this.address1 = Address1;
            this.address2 = Address2;
            this.city = City;
            this.state = State;
            this.zip = Zip;
            this.country = Country;
            this.phoneNumber = PhoneNumber;
            this.movileNumber = MovileNumber;
            this.faxNumber = FaxNumber;
            this.eMail = EMail;
            this.homePage = HomePage;
            this.iMAddress = IMAddress;
            this.creditLimit = CreditLimit;
            this.creditLimitCheck = CreditLimitCheck;
            this.statementType = StatementType;
            this.statementDelivery = StatementDelivery;
            this.statementAddress = StatementAddress;
            this.applicationMethod = ApplicationMethod;
            this.closingDate = ClosingDate;
            this.closingBalance = ClosingBalance;
            this.lastStatement = LastStatement;
            this.customCode1 = CustomCode1;
            this.customCode2 = CustomCode2;
            this.customCode3 = CustomCode3;
            this.customCode4 = CustomCode4;
            this.customCode5 = CustomCode5;
            this.customText1 = CustomText1;
            this.customText2 = CustomText2;
            this.customText3 = CustomText3;
            this.customText4 = CustomText4;
            this.customText5 = CustomText5;
            this.customNumber1 = CustomNumber1;
            this.customNumber2 = CustomNumber2;
            this.customNumber3 = CustomNumber3;
            this.customNumber4 = CustomNumber4;
            this.customNumber5 = CustomNumber5;
            this.customDate1 = CustomDate1;
            this.customDate2 = CustomDate2;
            this.customDate3 = CustomDate3;
            this.customDate4 = CustomDate4;
            this.customDate5 = CustomDate5;
            this.picture = Picture;
            this.notes = Notes;
            this.extData = ExtData;
            this.dateOpened = DateOpened;
            this.lastActivity = LastActivity;
            this.storeID = StoreID;
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
        public virtual DateTime? LastUpdated
        {
            get { return lastUpdated; }
            set { lastUpdated = value; }
        }
        public virtual string Number
        {
            get { return number; }
            set { number = value; }
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
        public virtual string Title
        {
            get { return title; }
            set { title = value; }
        }
        public virtual string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        public virtual string MiddleName
        {
            get { return middleName; }
            set { middleName = value; }
        }
        public virtual string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        public virtual string Suffix
        {
            get { return suffix; }
            set { suffix = value; }
        }
        public virtual string Company
        {
            get { return company; }
            set { company = value; }
        }
        public virtual string JobTitle
        {
            get { return jobTitle; }
            set { jobTitle = value; }
        }
        public virtual int Type
        {
            get { return type; }
            set { type = value; }
        }
        public virtual int Role
        {
            get { return role; }
            set { role = value; }
        }
        public virtual int Status
        {
            get { return status; }
            set { status = value; }
        }
        public virtual int BillingAccount
        {
            get { return billingAccount; }
            set { billingAccount = value; }
        }
        public virtual int GroupID
        {
            get { return groupID; }
            set { groupID = value; }
        }
        public virtual int PayTermsID
        {
            get { return payTermsID; }
            set { payTermsID = value; }
        }
        public virtual int FinChargeID
        {
            get { return finChargeID; }
            set { finChargeID = value; }
        }
        public virtual int CurrencyID
        {
            get { return currencyID; }
            set { currencyID = value; }
        }
        public virtual int ManagerID
        {
            get { return managerID; }
            set { managerID = value; }
        }
        public virtual int CountryID
        {
            get { return countryID; }
            set { countryID = value; }
        }
        public virtual int RegionID
        {
            get { return regionID; }
            set { regionID = value; }
        }
        public virtual int LocationID
        {
            get { return locationID; }
            set { locationID = value; }
        }
        public virtual string Address1
        {
            get { return address1; }
            set { address1 = value; }
        }
        public virtual string Address2
        {
            get { return address2; }
            set { address2 = value; }
        }
        public virtual string City
        {
            get { return city; }
            set { city = value; }
        }
        public virtual string State
        {
            get { return state; }
            set { state = value; }
        }
        public virtual string Zip
        {
            get { return zip; }
            set { zip = value; }
        }
        public virtual string Country
        {
            get { return country; }
            set { country = value; }
        }
        public virtual string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        public virtual string MovileNumber
        {
            get { return movileNumber; }
            set { movileNumber = value; }
        }
        public virtual string FaxNumber
        {
            get { return faxNumber; }
            set { faxNumber = value; }
        }
        public virtual string EMail
        {
            get { return eMail; }
            set { eMail = value; }
        }
        public virtual string HomePage
        {
            get { return homePage; }
            set { homePage = value; }
        }
        public virtual string IMAddress
        {
            get { return iMAddress; }
            set { iMAddress = value; }
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
        public virtual int StatementDelivery
        {
            get { return statementDelivery; }
            set { statementDelivery = value; }
        }
        public virtual string StatementAddress
        {
            get { return statementAddress; }
            set { statementAddress = value; }
        }
        public virtual int ApplicationMethod
        {
            get { return applicationMethod; }
            set { applicationMethod = value; }
        }
        public virtual DateTime? ClosingDate
        {
            get { return closingDate; }
            set { closingDate = value; }
        }
        public virtual decimal ClosingBalance
        {
            get { return closingBalance; }
            set { closingBalance = value; }
        }
        public virtual int LastStatement
        {
            get { return lastStatement; }
            set { lastStatement = value; }
        }
        public virtual int CustomCode1
        {
            get { return customCode1; }
            set { customCode1 = value; }
        }
        public virtual int CustomCode2
        {
            get { return customCode2; }
            set { customCode2 = value; }
        }
        public virtual int CustomCode3
        {
            get { return customCode3; }
            set { customCode3 = value; }
        }
        public virtual int CustomCode4
        {
            get { return customCode4; }
            set { customCode4 = value; }
        }
        public virtual int CustomCode5
        {
            get { return customCode5; }
            set { customCode5 = value; }
        }
        public virtual string CustomText1
        {

            get { return customText1; }
            set { customText1 = value; }
        }
        public virtual string CustomText2
        {

            get { return customText2; }
            set { customText2 = value; }
        }
        public virtual string CustomText3
        {

            get { return customText3; }
            set { customText3 = value; }
        }
        public virtual string CustomText4
        {

            get { return customText4; }
            set { customText4 = value; }
        }
        public virtual string CustomText5
        {
            get { return customText5; }
            set { customText5 = value; }
        }
        public virtual float CustomNumber1
        {

            get { return customNumber1; }
            set { customNumber1 = value; }
        }
        public virtual float CustomNumber2
        {

            get { return customNumber2; }
            set { customNumber2 = value; }
        }
        public virtual float CustomNumber3
        {

            get { return customNumber3; }
            set { customNumber3 = value; }
        }
        public virtual float CustomNumber4
        {

            get { return customNumber4; }
            set { customNumber4 = value; }
        }
        public virtual float CustomNumber5
        {
            get { return customNumber5; }
            set { customNumber5 = value; }
        }
        public virtual DateTime? CustomDate1
        {

            get { return customDate1; }
            set { customDate1 = value; }
        }
        public virtual DateTime? CustomDate2
        {

            get { return customDate2; }
            set { customDate2 = value; }
        }
        public virtual DateTime? CustomDate3
        {

            get { return customDate3; }
            set { customDate3 = value; }
        }
        public virtual DateTime? CustomDate4
        {

            get { return customDate4; }
            set { customDate4 = value; }
        }
        public virtual DateTime? CustomDate5
        {
            get { return customDate5; }
            set { customDate5 = value; }
        }
        public virtual byte[] Picture
        {
            get { return picture; }
            set { picture = value; }
        }
        public virtual string Notes
        {
            get { return notes; }
            set { notes = value; }
        }
        public virtual string ExtData
        {
            get { return extData; }
            set { extData = value; }
        }
        public virtual DateTime? DateOpened
        {
            get { return dateOpened; }
            set { dateOpened = value; }
        }
        public virtual DateTime? LastActivity
        {
            get { return lastActivity; }
            set { lastActivity = value; }
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
