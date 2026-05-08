using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_AR_AccountManager
    {
        #region Fields
        private int Id;
        private string dbTimeStamp;
        private string code;
        private string name;
        private string extCode;
        private bool inactive;
        private string title;
        private string firstName;
        private string middleName;
        private string lastName;
        private string suffix;
        private string phoneNumber;
        private string mobileNumber;
        private string eMail;
        private int loginID;
        private Guid syncGuid;
        #endregion
        #region Constructors
        public EN_AR_AccountManager()
        {
            
        }
        public EN_AR_AccountManager(int ID, string DbTimeStamp, string Code, string Name, string ExtCode, bool Inactive,
            string Title, string FirstName, string MiddleName, string LastName, string Suffix, string PhoneNumber, string MobileNumber,
            string EMail, int LoginID, Guid SyncGuid)
        {
            this.Id = ID;
            this.dbTimeStamp = DbTimeStamp;
            this.code = Code;
            this.name = Name;
            this.extCode = ExtCode;
            this.inactive = Inactive;
            this.title = Title;
            this.firstName = FirstName;
            this.middleName = MiddleName;
            this.lastName = LastName;
            this.suffix = Suffix;
            this.phoneNumber = PhoneNumber;
            this.mobileNumber = MobileNumber;
            this.eMail = EMail;
            this.loginID = LoginID;
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
        public virtual string PhoneNumber
        {

            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        public virtual string MobileNumber
        {

            get { return mobileNumber; }
            set { mobileNumber = value; }
        }
        public virtual string EMail
        {
            get { return eMail; }
            set { eMail = value; }
        }
        public virtual int LoginID
        {

            get { return loginID; }
            set { loginID = value; }
        }
        public virtual Guid SyncGuid
        {
            get { return syncGuid; }
            set { syncGuid = value; }
        }
        #endregion
    }
}
