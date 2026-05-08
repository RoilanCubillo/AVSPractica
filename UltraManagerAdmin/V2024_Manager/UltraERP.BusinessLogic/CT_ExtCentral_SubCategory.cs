using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ExtCentral_SubCategory
    {

        #region Variables
        EN_ExtCentral_SubCategory oEN_ExtCentral_SubCategory = new EN_ExtCentral_SubCategory();
        DT_ExtCentral_SubCategory oDT_ExtCentral_SubCategory = new DT_ExtCentral_SubCategory();
        #endregion

        #region Constructors
        public CT_ExtCentral_SubCategory() { }
        #endregion

        #region Methods
        public Respuesta Get(int iD)
        {
            return oDT_ExtCentral_SubCategory.Get(iD);
        }
        public Respuesta Save(EN_ExtCentral_SubCategory family)
        {
            return oDT_ExtCentral_SubCategory.Save(family);
        }
        public List<EN_ExtCentral_SubCategory> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return oDT_ExtCentral_SubCategory.GetAll(busqueda, estado, cantidad);
        }
        public List<EN_ExtCentral_SubCategory> GetAll_By_CategoryID(int categoryID)
        {
            return oDT_ExtCentral_SubCategory.GetAll_By_CategoryID(categoryID);
        }
        #endregion

    }
}
