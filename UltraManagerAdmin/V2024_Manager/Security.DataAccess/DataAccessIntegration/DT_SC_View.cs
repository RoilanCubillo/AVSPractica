using Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.DataAccess.DataAccessIntegration
{
    public class DT_SC_View : DT_SC
    {
        public DT_SC_View() : base() { }

        public List<EN_SC_View> ValidateViews(int userID, string systemCode)
        {
            List<EN_SC_View> list = (
                from i in db.SC_VIEW_VALIDATE_GET(userID, systemCode)
                select new EN_SC_View()
                {
                    Code = i.Code,
                    Enable = i.Enable,
                    ID = i.ID,
                    ModuleID = i.MuduleID,
                    Name = i.Name
                }
            ).ToList();

            return list;
        }
    }
}
