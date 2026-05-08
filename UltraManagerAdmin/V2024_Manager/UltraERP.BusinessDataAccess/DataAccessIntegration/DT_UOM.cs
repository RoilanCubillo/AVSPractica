using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_UOM : DT
    {
        public DT_UOM() : base() { }

        public List<EN_UOM> GetAllByInactive(bool inactive)
        {
            List<EN_UOM> list = (
                from i in db.UEP_UOM_GETALL_BY_INACTIVE(inactive)
                select new EN_UOM() { ID = i.ID, Code = i.Code, Name = i.Name, Inactive = i.Inactive }
            ).ToList();

            return list;
        }
    }
}
