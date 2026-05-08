using Security.DataAccess.DataAccessIntegration;
using Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Logic
{
    public class CT_SC_View
    {
        public List<EN_SC_View> ValidateViews(int userID, string systemCode)
        {
            return new DT_SC_View().ValidateViews(userID, systemCode);
        }
    }
}
