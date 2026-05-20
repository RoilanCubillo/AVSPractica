using Security.EntitiesAVS;
using System.Collections.Generic;
using System.Linq;

namespace Security.DataAccess.DataAccessIntegration
{
   public class DT_SC_View : DT_SC
    {
        public DT_SC_View() : base() { }

        public List<EN_SC_View> ValidateViews(int userID, string systemCode)
        {
            using (var localDb = new Data.SecurityDBDataContext())
            {
                return (
                    from i in localDb.SC_VIEW_VALIDATE_GET(userID, systemCode)
                    select new EN_SC_View()
                    {
                        Code = i.Code,
                        Enable = i.Enable,
                        ID = i.ID,
                        ModuleID = i.MuduleID,
                        Name = i.Name
                    }
                ).ToList();
            }
        }
    }
}
