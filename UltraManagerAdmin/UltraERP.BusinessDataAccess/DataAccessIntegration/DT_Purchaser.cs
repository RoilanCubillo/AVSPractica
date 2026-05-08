using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Purchaser : DT
    {
        #region Variables
        #endregion

        #region Constructor
        public DT_Purchaser() : base() { }
        #endregion

        #region Métodos
        public List<EN_Purchaser> GetAll()
        {
            List<EN_Purchaser> list =
                (from p in db.UEP_PURCHASER_GETALL()
                 select new EN_Purchaser() { ID = p.ID, Code = p.Code, Name = p.Name, Inactive = p.Inactive, EmailAddress = p.EmailAddress, ExtCode = p.ExtCode, Telephone = p.Telephone, LastUpdated = p.LastUpdated }
                ).ToList();
            
            return list;
        }

        public List<EN_Purchaser> GetAllByInactive(bool inactive)
        {
            List<EN_Purchaser> list =
                (from p in db.UEP_PURCHASER_GETALL_BY_INACTIVE(inactive)
                 select new EN_Purchaser() { ID = p.ID, Code = p.Code, Name = p.Name, Inactive = p.Inactive, EmailAddress = p.EmailAddress, ExtCode = p.ExtCode, Telephone = p.Telephone, LastUpdated = p.LastUpdated }
                ).ToList();

            return list;
        }

        public Respuesta Save(EN_Purchaser pur)
        {
            Respuesta respuesta = new Respuesta("NO ACCIONADO", "error_guardar", null, false);
            
            foreach(var i in db.UEP_PURCHASER_INSERT_UPDATE(pur.ID, pur.Code, pur.ExtCode, pur.Name, pur.EmailAddress, pur.Telephone, pur.Inactive))
                respuesta = new Respuesta("", i.RESPUESTA, i.SCOPE, !i.RESPUESTA.Contains("error"));

            return respuesta;
        }
        #endregion
    }
}
