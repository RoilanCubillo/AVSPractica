using Security.Entities;
using System;

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
                        Name = u.Name,
                        SystemID = u.SystemID
                    };
                }

                return (user, (user == null ? -1 : 1), "");
            } catch (Exception e)
            {
                return (user, 0, e.Message);
            }
        }

        public bool validateUserSession(int iD, int autoID, int systemID)
        {
            foreach (var u in db.SC_USER_VALIDATE_SESSION(iD, autoID, systemID)) return true;

            return false;
        }
    }
}
