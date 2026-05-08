using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_Purchaser
    {
        #region Variables
        private EN_Purchaser eN_Purchaser = new EN_Purchaser();
        private DT_Purchaser dT_Purchaser = new DT_Purchaser();
        #endregion

        #region Constructors
        public CT_Purchaser() { }
        #endregion

        #region Methods
        public List<EN_Purchaser> GetAll()
        {
            return dT_Purchaser.GetAll();
        }

        public List<EN_Purchaser> GetAllByInactive(bool inactive)
        {
            return dT_Purchaser.GetAllByInactive(inactive);
        }

        public Respuesta Save(EN_Purchaser pur)
        {
            return dT_Purchaser.Save(pur);
        }
        #endregion
    }
}
