using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
   public  class CT_EspaciosFrios
    {

        #region Variables


        DT_EspaciosFrios oDT_EspacioFrio = new DT_EspaciosFrios();

        #endregion

        #region Constructors

        public CT_EspaciosFrios()
        {
        }

        #endregion

        #region Methods

       
        /// <summary>
        /// Guarda un nuevo registro o actualizar datos en la Entidad espacios frios
        /// </summary>
        public Respuesta Save(EN_EspaciosFrios espFrio)
        {
            return oDT_EspacioFrio.Save(espFrio);
        }

        /// <summary>
        /// Selects all records from the espacios frios table.
        /// </summary>
        public List<EN_EspaciosFrios> GetAll()
        {
            return oDT_EspacioFrio.GetAll();
        }

        public List<EN_EspaciosFrios> GetAllEspaciosFriosxTienda(int idStore)
        {
            return oDT_EspacioFrio.GetAll_EspaciosXtienda(idStore);
        }


        #endregion
    }
}
