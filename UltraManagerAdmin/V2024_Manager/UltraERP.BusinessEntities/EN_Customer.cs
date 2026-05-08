using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_Customer
    {
        #region Fields
        private string accountNumber;
        private int accountTypeID;
        private string address2;
        private bool assessFinanceCharges;
        private string company;
        private string country;
        private DateTime? customDate1;
        private DateTime? customDate2;
        private DateTime? customDate3;
        private DateTime? customDate4;
        private DateTime? customDate5;
        private float customNumber1;
        private float customNumber2;
        private float customNumber3;
        private float customNumber4;
        private float customNumber5;
        private string customText1;
        private string customText2;
        private string customText3;
        private string customText4;
        private string customText5;
        private bool globalCustomer;
        private int hQID;
        private DateTime? lastStartingDate;
        private DateTime? lastClosingDate;
        private DateTime? lastUpdated;
        private bool limitPurchase;
        private decimal lastClosingBalance;
        private int primaryShipToID;
        private string state;
        private int storeID;
        private int iD;
        private bool layawayCustomer;
        private bool employee;
        private string firstName;
        private string lastName;
        private string address;
        private string city;
        private string zip;
        private decimal accountBalance;
        private decimal creditLimit;
        private decimal totalSales;
        private DateTime? accountOpened;
        private DateTime? lastVisit;
        private int totalVisits;
        private decimal totalSavings;
        private float currentDiscount;
        private int priceLevel;
        private bool taxExempt;
        private string notes;
        private string title;
        private string emailAddress;
        private string dBTimeStamp;
        private string taxNumber;
        private string pictureName;
        private int defaultShippingServiceID;
        private int autoID;
        private string phoneNumber;
        private string faxNumber;
        private int cashierID;
        private int salesRepID;
        private decimal vouchers;
        private Guid syncGuid;
        #endregion


        #region Constructors
        public EN_Customer()
        {
            
        }

        public EN_Customer(string AccountNumber, int AccountTypeID, string Address2, bool AssessFinanceCharges, string Company,
            string Country, DateTime? CustomDate1, DateTime? CustomDate2, DateTime? CustomDate3, DateTime? CustomDate4, DateTime? CustomDate5,
            float CustomNumber1, float CustomNumber2, float CustomNumber3, float CustomNumber4, float CustomNumber5, string CustomText1,
            string CustomText2, string CustomText3, string CustomText4, string CustomText5, bool GlobalCustomer, int HQID, DateTime? LastStartingDate,
            DateTime? LastClosingDate, DateTime? LastUpdated, bool LimitPurchase, decimal LastClosingBalance, int PrimaryShipToID, string State, 
           int StoreID, int ID, bool LayawayCustomer, bool Employee, string FirstName, string LastName, string Address, string City, string Zip,
           decimal AccountBalance, decimal CreditLimit, decimal TotalSales, DateTime? AccountOpened, DateTime? LastVisit, int TotalVisits,
           decimal TotalSavings, float CurrentDiscount, int PriceLevel, bool TaxExempt, string Notes, string Title, string EmailAddress, string DBTimeStamp,
           string TaxNumber, string PictureName, int DefaultShippingServiceID, int AutoID, string PhoneNumber, string FaxNumber, int CashierID, int SalesRepID,
           decimal Vouchers, Guid SyncGuid)
        {
            this.accountNumber = AccountNumber;
            this.accountTypeID = AccountTypeID;
            this.address2 = Address2;
            this.assessFinanceCharges = AssessFinanceCharges;
            this.company = Company;
            this.country = Country;
            this.customDate1 = CustomDate1;
            this.customDate2 = CustomDate2;
            this.customDate3 = CustomDate3;
            this.customDate4 = CustomDate4;
            this.customDate5 = CustomDate5;
            this.customNumber1 = CustomNumber1;
            this.customNumber2 = CustomNumber2;
            this.customNumber3 = CustomNumber3;
            this.customNumber4 = CustomNumber4;
            this.customNumber5 = CustomNumber5;
            this.customText1 = CustomText1;
            this.customText2 = CustomText2;
            this.customText3 = CustomText3;
            this.customText4 = CustomText4;
            this.customText5 = CustomText5;
            this.globalCustomer = GlobalCustomer;
            this.hQID = HQID;
            this.lastStartingDate = LastStartingDate;
            this.lastClosingDate = LastClosingDate;
            this.lastUpdated = LastUpdated;
            this.limitPurchase = LimitPurchase;
            this.lastClosingBalance = LastClosingBalance;
            this.primaryShipToID = PrimaryShipToID;
            this.state = State;
            this.storeID = StoreID;
            this.iD = ID;
            this.layawayCustomer = LayawayCustomer;
            this.employee = Employee;
            this.firstName = FirstName;
            this.lastName = LastName;
            this.address = Address;
            this.city = City;
            this.zip = Zip;
            this.accountBalance = AccountBalance;
            this.creditLimit = CreditLimit;
            this.totalSales = TotalSales;
            this.accountOpened = AccountOpened;
            this.lastVisit = LastVisit;
            this.totalVisits = TotalVisits;
            this.totalSavings = TotalSavings;
            this.currentDiscount = CurrentDiscount;
            this.priceLevel = PriceLevel;
            this.taxExempt = TaxExempt;
            this.notes = Notes;
            this.title = Title;
            this.emailAddress = EmailAddress;
            this.dBTimeStamp = DBTimeStamp;
            this.taxNumber = TaxNumber;
            this.pictureName = PictureName;
            this.defaultShippingServiceID = DefaultShippingServiceID;
            this.autoID = AutoID;
            this.phoneNumber = PhoneNumber;
            this.faxNumber = FaxNumber;
            this.cashierID = CashierID;
            this.salesRepID = SalesRepID;
            this.vouchers = Vouchers;
            this.syncGuid = SyncGuid;
        }

        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the ID value.
        /// </summary>
        /// 
        public virtual string AccountNumber
        {

            get { return accountNumber; }
            set { accountNumber = value; }
        }
        public virtual int AccountTypeID
        {
            get { return accountTypeID; }
            set { accountTypeID = value; }
        }
        public virtual string Address2
        {
            get { return address2; }
            set { address2 = value; }
        }
        public virtual bool AssessFinanceCharges
        {
            get { return assessFinanceCharges; }
            set { assessFinanceCharges = value; }
        }
        public virtual string Company
        {
            get { return company; }
            set { company = value; }
        }
        public virtual string Country
        {
            get { return country; }
            set { country = value; }
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
        public virtual bool GlobalCustomer
        {
            get { return globalCustomer; }
            set { globalCustomer = value; }
        }
        public virtual int HQID
        {
            get { return hQID; }
            set { hQID = value; }
        }
        public virtual DateTime? LastStartingDate
        {
            get { return lastStartingDate; }
            set { lastStartingDate = value; }
        }
        public virtual DateTime? LastClosingDate
        {
            get { return lastClosingDate; }
            set { lastClosingDate = value; }
        }
        public virtual DateTime? LastUpdated
        {
            get { return lastUpdated; }
            set { lastUpdated = value; }
        }
        public virtual bool LimitPurchase
        {
            get { return limitPurchase; }
            set { limitPurchase = value; }
        }
        public virtual decimal LastClosingBalance
        {
            get { return lastClosingBalance; }
            set { lastClosingBalance = value; }
        }
        public virtual int PrimaryShipToID
        {
            get { return primaryShipToID; }
            set { primaryShipToID = value; }
        }
        public virtual string State
        {
            get { return state; }
            set { state = value; }
        }
        public virtual int StoreID
        {
            get { return storeID; }
            set { storeID = value; }
        }
        public virtual int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        public virtual bool LayawayCustomer
        {
            get { return layawayCustomer; }
            set { layawayCustomer = value; }
        }
        public virtual bool Employee
        {
            get { return employee; }
            set { employee = value; }
        }
        public virtual string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        public virtual string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        public virtual string Address
        {
            get { return address; }
            set { address = value; }
        }
        public virtual string City
        {
            get { return city; }
            set { city = value; }
        }
        public virtual string Zip
        {
            get { return zip; }
            set { zip = value; }
        }
        public virtual decimal AccountBalance
        {
            get { return accountBalance; }
            set { accountBalance = value; }
        }
        public virtual decimal CreditLimit
        {
            get { return creditLimit; }
            set { creditLimit = value; }
        }
        public virtual decimal TotalSales
        {
            get { return totalSales; }
            set { totalSales = value; }
        }
        public virtual DateTime? AccountOpened
        {
            get { return accountOpened; }
            set { accountOpened = value; }
        }
        public virtual DateTime? LastVisit
        {
            get { return lastVisit; }
            set { lastVisit = value; }
        }
        public virtual int TotalVisits
        {
            get { return totalVisits; }
            set { totalVisits = value; }
        }
        public virtual decimal TotalSavings
        {
            get { return totalSavings; }
            set { totalSavings = value; }
        }
        public virtual float CurrentDiscount
        {
            get { return currentDiscount; }
            set { currentDiscount = value; }
        }
        public virtual int PriceLevel
        {
            get { return priceLevel; }
            set { priceLevel = value; }
        }
        public virtual bool TaxExempt
        {
            get { return taxExempt; }
            set { taxExempt = value; }
        }
        public virtual string Notes
        {
            get { return notes; }
            set { notes = value; }
        }
        public virtual string Title
        {
            get { return title; }
            set { title = value; }
        }
        public virtual string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }
        public virtual string DBTimeStamp
        {
            get { return dBTimeStamp; }
            set { dBTimeStamp = value; }
        }
        public virtual string TaxNumber
        {
            get { return taxNumber; }
            set { taxNumber = value; }
        }
        public virtual string PictureName
        {
            get { return pictureName; }
            set { pictureName = value; }
        }
        public virtual int DefaultShippingServiceID
        {
            get { return defaultShippingServiceID; }
            set { defaultShippingServiceID = value; }
        }
        public virtual int AutoID
        {
            get { return autoID; }
            set { autoID = value; }
        }
        public virtual string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        public virtual string FaxNumber
        {
            get { return faxNumber; }
            set { faxNumber = value; }
        }
        public virtual int CashierID
        {
            get { return cashierID; }
            set { cashierID = value; }
        }
        public virtual int SalesRepID
        {
            get { return salesRepID; }
            set { salesRepID = value; }
        }
        public virtual decimal Vouchers
        {
            get { return vouchers; }
            set { vouchers = value; }
        }
        public virtual Guid SyncGuid
        {
            get { return syncGuid; }
            set { syncGuid = value; }
        }
        #endregion


    }
}
