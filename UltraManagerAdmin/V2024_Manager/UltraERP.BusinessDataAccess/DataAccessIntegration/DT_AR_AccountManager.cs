using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_AR_AccountManager : DT
    {
        #region Variables
        protected EN_AR_AccountManager oEN_AR_AccountManage = new EN_AR_AccountManager();
        #endregion
        #region Constructors
        public DT_AR_AccountManager() : base() { }
        #endregion
        #region Methods
        public List<EN_AR_AccountManager> GetAll()
        {
            return(from p in db.UEP_AR_ACCOUNTMANAGER_GETALL("", true, 0)
                   select new EN_AR_AccountManager()
                   {
                       ID = p.ID,
                       Code = p.Code,
                       Name = p.Name,
                       ExtCode = p.ExtCode,
                       Inactive = p.Inactive,
                       Title = p.Title,
                       FirstName = p.FirstName,
                       MiddleName = p.MiddleName,
                       LastName = p.LastName,
                       Suffix = p.Suffix,
                       PhoneNumber = p.PhoneNumber,
                       MobileNumber = p.MobileNumber,
                       EMail = p.EMail,
                       LoginID = p.LoginID.Value
                   }).ToList();
        }
        #endregion
    }
}
