using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_Supplier
    {
        #region Variables

        EN_Supplier oEN_Supplier = new EN_Supplier();
        DT_Supplier oDT_Supplier = new DT_Supplier();

        #endregion

        #region Constructors

        public CT_Supplier()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public EN_Supplier Get(int iD, string stores_ID)
        {
            return oDT_Supplier.Get(iD, stores_ID);
        }

        /// <summary>
        /// Guarda un nuevo registro o actualizar datos en la Entidad EN_Category
        /// </summary>

        public List<EN_Supplier> GetAll(string stores_ID, string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return oDT_Supplier.GetAll(stores_ID, busqueda, estado, cantidad);
        }


        #endregion
    }
}
