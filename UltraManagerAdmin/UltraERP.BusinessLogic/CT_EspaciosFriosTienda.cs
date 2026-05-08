using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
   public  class CT_EspaciosFriosTienda
    {
        #region Variables


        DT_EspaciosFriosTienda oDT_EspFrio = new DT_EspaciosFriosTienda();

        #endregion

        #region Constructors

        public CT_EspaciosFriosTienda()
        {
        }

        #endregion

        #region Methods


        /// <summary>
        /// Guarda un nuevo registro o actualizar datos en la Entidad EN_EspaciosFriosTienda
        /// </summary>
        public Respuesta Save(EN_EspaciosFriosTienda espFrio)
        {
            return oDT_EspFrio.Save(espFrio);
        }

        /// <summary>
        /// Selects all records from the espacios frios x tienda table.
        /// </summary>
        public List<EN_EspaciosFriosTienda> GetAll()
        {
            return oDT_EspFrio.GetAll();
        }

        #endregion
    }
}
