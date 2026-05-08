using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ExtCentral_Family
    {

        #region Variables
        EN_ExtCentral_Family oEN_ExtCentral_Family = new EN_ExtCentral_Family();
        DT_ExtCentral_Family oDT_ExtCentral_Family = new DT_ExtCentral_Family();
        #endregion

        #region Constructors
        public CT_ExtCentral_Family(){ }
        #endregion

        #region Methods
        public Respuesta Get(int iD)
        {
            return oDT_ExtCentral_Family.Get(iD);
        }
        public Respuesta Save(EN_ExtCentral_Family family)
        {
            return oDT_ExtCentral_Family.Save(family);
        }
        public List<EN_ExtCentral_Family> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return oDT_ExtCentral_Family.GetAll(busqueda, estado, cantidad);
        }
        #endregion

    }
}
