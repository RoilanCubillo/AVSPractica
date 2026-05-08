using Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.DataAccess.DataAccessIntegration
{
    public class DT_SC_Module : DT_SC
    {
        public DT_SC_Module() : base() { }

        public List<EN_SC_Module> ValidateModules(int userID, string systemCode)
        {
            List<EN_SC_Module> list = (
                from i in db.SC_MODULE_VALIDATE_GET(systemCode, userID)
                select new EN_SC_Module()
                {
                    Code = i.Code,
                    Enable = i.Enable,
                    ID = i.ID,
                    Name = i.Name,
                    SystemID = i.SystemID
                }
            ).ToList();

            return list;
        }
    }
}
