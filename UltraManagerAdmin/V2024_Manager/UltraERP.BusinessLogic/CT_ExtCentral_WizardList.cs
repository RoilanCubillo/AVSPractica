using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ExtCentral_WizardList
    {
        private DT_ExtCentral_WizardList dtObj = new DT_ExtCentral_WizardList();

        public CT_ExtCentral_WizardList() { }

        public List<EN_ExtCentral_WizardList> GetAll()
        {
            return dtObj.GetAll();
        }
    }
}
