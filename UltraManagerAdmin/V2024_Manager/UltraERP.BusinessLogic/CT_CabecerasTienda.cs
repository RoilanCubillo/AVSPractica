using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_CabecerasTienda
    {
        #region Variables


        DT_CabecerasTienda oDT_Cabecera = new DT_CabecerasTienda();

        #endregion

        #region Constructors

        public CT_CabecerasTienda()
        {
        }

        #endregion

        #region Methods


        /// <summary>
        /// Guarda un nuevo registro o actualizar datos en la Entidad EN_Category
        /// </summary>
        public Respuesta Save(EN_CabecerasTienda cabecera)
        {
            return oDT_Cabecera.Save(cabecera);
        }

        /// <summary>
        /// Selects all records from the Category table.
        /// </summary>
        public List<EN_CabecerasTienda> GetAll()
        {
            return oDT_Cabecera.GetAll();
        }

        #endregion
    }
}
