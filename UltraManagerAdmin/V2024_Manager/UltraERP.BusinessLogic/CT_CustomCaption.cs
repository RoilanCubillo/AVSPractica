using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_CustomCaption
    {
        #region Variables
        DT_CustomCaption oDT_CustomCaption = new DT_CustomCaption();
        #endregion
        #region Constructors
        public CT_CustomCaption()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public List<EN_CustomCaption> GetAll()
        {
            return oDT_CustomCaption.GetAll();
        }
        #endregion
    }
}
