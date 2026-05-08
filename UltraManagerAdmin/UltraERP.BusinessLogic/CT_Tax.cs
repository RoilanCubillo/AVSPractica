using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_Tax
    {
        #region Variables


        DT_Tax oDT_Tax = new DT_Tax();

        #endregion

        #region Constructors

        public CT_Tax()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public EN_Tax Get(int iD)
        {
            return oDT_Tax.Get(iD);
        }

       

        /// <summary>
        /// Selects all records from the Category table.
        /// </summary>
        public List<EN_Tax> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return oDT_Tax.GetAll(busqueda, estado, cantidad);
        }

        #endregion
    }
}
