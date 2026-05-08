using Security.DataAccess.DataAccessIntegration;
using Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Logic
{
    public class CT_SC_Module
    {
        public List<EN_SC_Module> ValidateModules(int userID, string systemCode)
        {
            return new DT_SC_Module().ValidateModules(userID, systemCode);
        }
    }
}
