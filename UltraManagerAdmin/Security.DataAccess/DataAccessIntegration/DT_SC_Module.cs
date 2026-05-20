using Security.EntitiesAVS;
using System.Collections.Generic;
using System.Linq;

namespace Security.DataAccess.DataAccessIntegration
{
   public class DT_SC_Module : DT_SC
    {
        public DT_SC_Module() : base() { }

        public List<EN_SC_Module> ValidateModules(int userID, string systemCode)
        {
            using (var localDb = new Data.SecurityDBDataContext())
            {
                return (
                    from i in localDb.SC_MODULE_VALIDATE_GET(systemCode, userID)
                    select new EN_SC_Module()
                    {
                        Code = i.Code,
                        Enable = i.Enable,
                        ID = i.ID,
                        Name = i.Name,
                        SystemID = i.SystemID
                    }
                ).ToList();
            }
        }
    }
}
