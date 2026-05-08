using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ExtCentral_Segment
    {
        #region Variables
        private DT_ExtCentral_Segment dtObject = new DT_ExtCentral_Segment();
        #endregion

        #region Constructors
        public CT_ExtCentral_Segment() { }
        #endregion

        #region Methods
        public Respuesta Save(EN_ExtCentral_Segment seg)
        {
            return dtObject.Save(seg);
        }
        public List<EN_ExtCentral_Segment> GetAll()
        {
            return dtObject.GetAll();
        }
        public List<EN_ExtCentral_Segment> GetAll_By_SubCategoryID(int subCategoryID)
        {
            return dtObject.GetAll_By_SubCategoryID(subCategoryID);
        }
        #endregion
    }
}
