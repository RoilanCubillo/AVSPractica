using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ExtCentral_Manufacturer : DT
    {
        public DT_ExtCentral_Manufacturer() : base() { }

        public List<EN_ExtCentral_Manufacturer> GetAll()
        {
            List<EN_ExtCentral_Manufacturer> list = (
                from i in db.UEP_EXTCENTRAL_MANUFACTURER_GETALL()
                select new EN_ExtCentral_Manufacturer() { ID = i.ID, Description = i.Description }
            ).ToList();

            return list;
        }

        public Respuesta Save(int iD, string description)
        {
            try
            {
                Respuesta respuesta = new Respuesta("NO ACCIONADO", "error_guardar", null, false);

                foreach (var i in db.UEP_EXTCENTRAL_MANUFACTURER_INSERT_UPDATE(iD, description))
                    respuesta = new Respuesta("", i.RESPUESTA, i.SCOPE, i.RESPUESTA.IndexOf("error", StringComparison.OrdinalIgnoreCase) < 0);

                return respuesta;
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }
        }
    }
}
