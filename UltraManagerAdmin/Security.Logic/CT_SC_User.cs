using Security.DataAccess.DataAccessIntegration;
using Security.EntitiesAVS;
using System.Collections.Generic;

namespace Security.Logic
{
    public class CT_SC_User
    {
        public (EN_SC_User, int, string) ValidateUser(string account, int autoID, int systemID)
        {
            return new DT_SC_User().ValidateUser(account, autoID, systemID);
        }

        public bool validateUserSession(int iD, int autoID, int systemID)
        {
            return new DT_SC_User().validateUserSession(iD, autoID, systemID);
        }

        public List<EN_SC_Company> GetCompaniesByUser(string account, int autoID, int systemID)
        {
            return new DT_SC_User().GetCompaniesByUser(account, autoID, systemID);
        }

        public (EN_SC_User, int, string) ValidateUserCompany(string account, int autoID, int systemID, int idCompany)
        {
            return new DT_SC_User().ValidateUserCompany(account, autoID, systemID, idCompany);
        }

        public bool ValidateUserSessionCompany(int id, int autoID, int systemID, int idCompany)
        {
            return new DT_SC_User().ValidateUserSessionCompany(id, autoID, systemID, idCompany);
        }
    }
}
