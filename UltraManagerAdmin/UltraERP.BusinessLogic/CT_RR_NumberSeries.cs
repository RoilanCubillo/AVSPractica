using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_RR_NumberSeries
    {
        #region Variables
        DT_RR_NumberSeries oDT_RR_NumberSeries = new DT_RR_NumberSeries();
        #endregion
        #region Constructors
        public CT_RR_NumberSeries()
        {

        }
        #endregion
        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_RR_NumberSeries> GetAll()
        {
            return oDT_RR_NumberSeries.GetAll();
        }
        #endregion
    }
}
