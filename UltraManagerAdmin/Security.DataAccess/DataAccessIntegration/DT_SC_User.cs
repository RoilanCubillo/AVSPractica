using Security.EntitiesAVS;
using System.Collections.Generic;
using System.Linq;

namespace Security.DataAccess.DataAccessIntegration
{
    public class DT_SC_User : DT_SC
    {
        public DT_SC_User() : base() { }

        public (EN_SC_User, int, string) ValidateUser(string account, int autoID, int systemID)
        {
            EN_SC_User user = null;

            try
            {
                foreach (var u in db.SC_USER_VALIDATE(account, autoID, systemID))
                {
                    user = new EN_SC_User()
                    {
                        Account = u.Account,
                        AutoID = u.AutoID,
                        Enable = u.Enable,
                        EnableCloseSession = u.EnCloseSession,
                        ID = u.ID,
                        ID_Company = 0,
                        Name = u.Name,
                        SystemID = u.SystemID
                    };
                }

                return (user, (user == null ? -1 : 1), "");
            }
            catch (System.Exception e)
            {
                return (user, 0, e.Message);
            }
        }

        public bool validateUserSession(int iD, int autoID, int systemID)
        {
            foreach (var u in db.SC_USER_VALIDATE_SESSION(iD, autoID, systemID))
            {
                return true;
            }

            return false;
        }

        public List<EN_SC_Company> GetCompaniesByUser(string account, int autoID, int systemID)
        {
            return (
                from i in db.SC_COMPANY_GET_BY_USER(account, autoID, systemID)
                select new EN_SC_Company()
                {
                    ID_Company = i.ID_Company,
                    Code = i.Code,
                    Name = i.Name,
                    Connection_String = i.Connection_String,
                    Enable = i.Enable
                }
            ).ToList();
        }

        public (EN_SC_User, int, string) ValidateUserCompany(string account, int autoID, int systemID, int idCompany)
        {
            EN_SC_User user = null;

            try
            {
                foreach (var u in db.SC_USER_VALIDATE_COMPANY(account, autoID, systemID, idCompany))
                {
                    user = new EN_SC_User()
                    {
                        Account = u.Account,
                        AutoID = u.AutoID,
                        Enable = u.Enable,
                        EnableCloseSession = u.EnCloseSession,
                        ID = u.ID,
                        ID_Company = u.ID_Company,
                        Name = u.Name,
                        SystemID = u.SystemID
                    };
                }

                return (user, (user == null ? -1 : 1), "");
            }
            catch (System.Exception e)
            {
                return (user, 0, e.Message);
            }
        }

        public bool ValidateUserSessionCompany(int id, int autoID, int systemID, int idCompany)
        {
            return db.SC_USER_VALIDATE_SESSION_COMPANY(id, autoID, systemID, idCompany).Any();
        }
    }
}
