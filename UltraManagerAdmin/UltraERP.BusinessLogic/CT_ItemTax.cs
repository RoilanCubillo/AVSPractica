using System.Collections.Generic;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ItemTax
    {
        public List<EN_ItemTax> GetAll()
        {
            return new DT_ItemTax().GetAll();
        }
    }
}
