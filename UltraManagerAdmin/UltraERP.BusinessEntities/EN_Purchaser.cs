using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_Purchaser
    {
        #region Fields
        private int iD;
        private string code;
        private string extCode;
        private string name;
        private string emailAddress;
        private string telephone;
        private bool inactive;
        private DateTime lastUpdated;
        #endregion

        #region Properties
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        public string ExtCode
        {
            get { return extCode; }
            set { extCode = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }
        public bool Inactive
        {
            get { return inactive; }
            set { inactive = value; }
        }
        public DateTime LastUpdated
        {
            get { return lastUpdated; }
            set { lastUpdated = value; }
        }
        public string LastUpdatedAux
        {
            get { return lastUpdated != null ? lastUpdated.ToString("dd/MM/yyyy hh:mm tt") : ""; }
        }
        #endregion
    }
}
