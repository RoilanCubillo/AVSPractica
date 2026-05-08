using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ExtCentral_Brand
    {
        public CT_ExtCentral_Brand() { }

        public List<EN_ExtCentral_Brand> GetAll()
        {
            return new DT_ExtCentral_Brand().GetAll();
        }

        public Respuesta Save(int id, string description)
        {
            return new DT_ExtCentral_Brand().Save(id, description);
        }
    }
}
