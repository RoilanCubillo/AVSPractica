using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_Category
    {
        #region Variables

        EN_Category oEN_Category = new EN_Category();
        DT_Category oDT_Category = new DT_Category();

        #endregion

        #region Constructors

        public CT_Category()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public Respuesta Get(int iD)
        {
            return oDT_Category.Get(iD);
        }

        /// <summary>
        /// Guarda un nuevo registro o actualizar datos en la Entidad EN_Category
        /// </summary>
        public Respuesta Save(EN_Category category)
        {
            return oDT_Category.Save(category);
        }

        /// <summary>
        /// Selects all records from the Category table.
        /// </summary>
        public List<EN_Category> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return oDT_Category.GetAll(busqueda, estado, cantidad);
        }

        public List<EN_Category> GetAll_Simple(int supplierID)
        {
            return oDT_Category.GetAll_Simple(supplierID);
        }

        public List<EN_Category> GetAll_ByDepartmentID(int departmentID)
        {
            return oDT_Category.GetAll_ByDepartmentID(departmentID);
        }

        #endregion
    }
}
