using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_UOM
    {
        public List<EN_UOM> GetAllByInactive(bool inactive)
        {
            return new DT_UOM().GetAllByInactive(inactive);
        }
    }
}
