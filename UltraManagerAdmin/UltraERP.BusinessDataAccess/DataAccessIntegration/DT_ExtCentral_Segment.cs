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
    public class DT_ExtCentral_Segment : DT
    {
        #region Variables
        #endregion

        #region Constructores
        public DT_ExtCentral_Segment() : base() { }
        #endregion

        #region Métodos
        public List<EN_ExtCentral_Segment> GetAll()
        {
            List<EN_ExtCentral_Segment> list = (
                from i in db.UEP_EXTCENTRAL_SEGMENT_GETALL()
                select new EN_ExtCentral_Segment()
                { ID = i.ID, SubCategoryID = i.SubCategoryID, Code = i.Code, Description = i.Description }
            ).ToList();

            return list;
        }

        public List<EN_ExtCentral_Segment> GetAll_By_SubCategoryID(int subCategoryID)
        {
            List<EN_ExtCentral_Segment> list = (
                from i in db.UEP_EXTCENTRAL_SEGMENT_GETALL_BY_SUBCATEGORYID(subCategoryID)
                select new EN_ExtCentral_Segment()
                { ID = i.ID, SubCategoryID = i.SubCategoryID, Code = i.Code, Description = i.Description }
            ).ToList();

            return list;
        }

        public Respuesta Save(EN_ExtCentral_Segment seg)
        {
            try
            {
                var result = db.UEP_EXTCENTRAL_SEGMENT_INSERT_UPDATE(seg.ID, seg.SubCategoryID, seg.Code, seg.Description);

                Respuesta respuesta = new Respuesta("PROCEDIMIENTO NO LEIDO", "error_guardar", null, false);

                foreach (var i in result)
                    respuesta = new Respuesta("", i.RESPUESTA, i.SCOPE, !i.RESPUESTA.ToUpper().Contains("ERROR"));

                return respuesta;
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }
        }
        #endregion
    }
}
