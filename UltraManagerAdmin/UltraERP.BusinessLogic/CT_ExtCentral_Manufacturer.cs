using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ExtCentral_Manufacturer
    {
        public CT_ExtCentral_Manufacturer() { }

        public List<EN_ExtCentral_Manufacturer> GetAll()
        {
            return new DT_ExtCentral_Manufacturer().GetAll();
        }

        public Respuesta Save(int id, string description)
        {
            return new DT_ExtCentral_Manufacturer().Save(id, description);
        }
    }
}
