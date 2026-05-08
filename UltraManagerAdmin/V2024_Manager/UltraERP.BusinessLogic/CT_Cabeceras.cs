using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
   public  class CT_Cabeceras
    {
        #region Variables

        
        DT_Cabeceras oDT_Cabecera = new DT_Cabeceras();

        #endregion

        #region Constructors

        public CT_Cabeceras()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        public Respuesta Get(int iD)
        {
            return oDT_Cabecera.Get(iD);
        }
        /// <summary>
        /// Guarda un nuevo registro o actualizar datos en la Entidad EN_Category
        /// </summary>
        public Respuesta Save(EN_Cabeceras cabecera)
        {
            return oDT_Cabecera.Save(cabecera);
        }

        /// <summary>
        /// Selects all records from the Category table.
        /// </summary>
        public List<EN_Cabeceras> GetAll()
        {
            return oDT_Cabecera.GetAll();
        }

        public List<EN_Cabeceras> GetAllCabeceraxTienda(int idStore)
        {
            return oDT_Cabecera.GetAll_TiendaxCabeceras(idStore);
        }

        #endregion
    }
}
