using System.Collections.Generic;
using System.Linq;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ItemTax : DT
    {
        public DT_ItemTax() : base() { }

        public List<EN_ItemTax> GetAll()
        {
            List<EN_ItemTax> list = (
                from i in db.UEP_ITEMTAX_GETALL()
                select new EN_ItemTax()
                {
                    ID = i.ID,
                    TaxID1 = i.TaxID01,
                    TaxID2 = i.TaxID02,
                    TaxID3 = i.TaxID03,
                    Code = i.Code,
                    Description = i.Description
                }
            ).ToList();

            return list;
        }
    }
}
