using Security.EntitiesAVS;
using Security.Logic;
using System.Collections.Generic;
using System.Linq;

namespace Security.WebServices
{
    public class SecurityServices : ISecurityServices
    {
        public (EN_SC_User, int, string) User_ValidateUser(string account, int autoID, int systemID)
        {
            return new CT_SC_User().ValidateUser(account, autoID, systemID);
        }

        public bool User_ValidateUserSession(int iD, int autoID, int systemID)
        {
            return new CT_SC_User().validateUserSession(iD, autoID, systemID);
        }

        public List<EN_SC_Company> User_GetCompanies(string account, int autoID, int systemID)
        {
            return new CT_SC_User().GetCompaniesByUser(account, autoID, systemID);
        }

        public (EN_SC_User, int, string) User_ValidateUserCompany(string account, int autoID, int systemID, int idCompany)
        {
            return new CT_SC_User().ValidateUserCompany(account, autoID, systemID, idCompany);
        }

        public bool User_ValidateUserSessionCompany(int iD, int autoID, int systemID, int idCompany)
        {
            return new CT_SC_User().ValidateUserSessionCompany(iD, autoID, systemID, idCompany);
        }

        public (SC_Enum.ESystemValidateStatus, bool) System_ValidateSystem(int userID, string systemCode)
        {
            return new CT_SC_System().ValidateSystem(userID, systemCode);
        }

        public EN_SC_Module[] Module_ValidateModules(int userID, string systemCode)
        {
            return new CT_SC_Module().ValidateModules(userID, systemCode).ToArray();
        }

        public List<EN_SC_View> View_ValidateViews(int userID, string systemCode)
        {
            return new CT_SC_View().ValidateViews(userID, systemCode);
        }

        public List<EN_SC_DataAccess> DataAccess_ValidateDataAccess(int userID, string systemCode, string dataCode)
        {
            return new CT_SC_DataAccess().ValidateDataAccess(userID, systemCode, dataCode);
        }
    }
}
