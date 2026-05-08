using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ExtCentral_Brand : DT
    {

        public DT_ExtCentral_Brand() : base() { }

        public List<EN_ExtCentral_Brand> GetAll()
        {
            List<EN_ExtCentral_Brand> list = (
                from i in db.UEP_EXTCENTRAL_BRAND_GETALL()
                select new EN_ExtCentral_Brand() { ID = i.ID, Description = i.Description }
            ).ToList();

            return list;
        }

        public Respuesta Save(int iD, string description)
        {
            Respuesta respuesta = new Respuesta("NO ACCIONADO", "error_guardar", null, false);

            foreach(var i in db.UEP_EXTCENTRAL_BRAND_INSERT_UPDATE(iD, description))
                respuesta = new Respuesta("", i.RESPUESTA, i.SCOPE, !i.RESPUESTA.Contains("error"));

            return respuesta;
        }
    }
}
