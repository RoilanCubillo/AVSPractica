using Analitic.DataAccess.DataAccessIntegration;
using Analitic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analitic.Logic
{
    public class CT_Graphics
    {
        #region Variables

        EN_Graphics oEN_Graphics = new EN_Graphics();
        DT_Graphics oDT_Graphics = new DT_Graphics();

        #endregion

        #region Constructors

        public CT_Graphics()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Obtener informacion de que se muestra en los graficos.
        /// </summary>
        public List<EN_Graphics> GetAll(string storeID = "", string codSucursal = "", string usersID = "", string busqueda = "", string fromDate = "", string toDate = "", string tipo = "")
        {
            return oDT_Graphics.GetAll(storeID, codSucursal, usersID, busqueda, fromDate, toDate, tipo);
        }


        #endregion
    }
}
