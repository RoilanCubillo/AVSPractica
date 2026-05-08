using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ExtCentral_WizardList : DT
    {
        #region Variables
        #endregion

        #region Constructores
        public DT_ExtCentral_WizardList() : base() { }
        #endregion

        #region Methods
        public List<EN_ExtCentral_WizardList> GetAll()
        {
            List<EN_ExtCentral_WizardList> list = (
                from i in db.UEP_WS_EXTCENTRAL_WIZARDLIST_GETALL()
                select new EN_ExtCentral_WizardList() { Codigo = i.Codigo, Descripcion = i.Descripcion, Estado = i.Estado }
            ).ToList();

            return list;
        }
        #endregion
    }
}
