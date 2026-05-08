using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
   public  class CT_IslasSideKick
    {
        #region Variables


        DT_IslasSideKick oDT_IslasSideKick = new DT_IslasSideKick();

        #endregion

        #region Constructors

        public CT_IslasSideKick()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Selects a single record from the islassidekick table.
        /// </summary>
        public Respuesta Get(int iD)
        {
            return oDT_IslasSideKick.Get(iD);
        }
        /// <summary>
        /// Guarda un nuevo registro o actualizar datos en la Entidad EN_IslasSideKick
        /// </summary>
        public Respuesta Save(EN_IslasSideKick islasSideKick)
        {
            return oDT_IslasSideKick.Save(islasSideKick);
        }

        /// <summary>
        /// Selects all records from the IslasSideKick table.
        /// </summary>
        public List<EN_IslasSideKick> GetAll()
        {
            return oDT_IslasSideKick.GetAll();
        }

       
        #endregion
    }
}

