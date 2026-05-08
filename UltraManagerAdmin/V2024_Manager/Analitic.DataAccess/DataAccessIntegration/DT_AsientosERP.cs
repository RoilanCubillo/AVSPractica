using Analitic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analitic.DataAccess.DataAccessIntegration
{
    
    public class DT_AsientosERP : DT
    {
        #region Variables
        public static readonly string TOTAL_FILTERED = "TOTAL_FILTERED";
        public static readonly string TOTAL_RECORDS = "TOTAL_RECORDS";

        #endregion

        #region Constructors

        public DT_AsientosERP() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Obtener documentos de la tabla API_ASIENTO_ENC_SYNC
        /// </summary>
        public List<EN_Asientos> GetAllAsientos(string storesID, string hqUsersID, string searchValue, string estadoAsiento, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            List<EN_Asientos> list = (
                from i in db.UEP_ASIENTOS_ERP_GETALL(storesID, hqUsersID, searchValue, estadoAsiento, orderColumn, orderDirection, skip, take, fromDate, toDate)
                select new EN_Asientos()
                {
                    ID = i.ID,
                    StoreId = i.Codigo_negocio,
                    StoreName = i.Nombre_negocio,
                    Descripcion = i.Descripcion,
                    Referencia = i.Referencia,
                    Fecha_Asiento = i.Fecha_asiento,
                    Fecha_Sync = i.Fecha_Sync,
                    Detalle_Sync = i.Detalle_Sync,
                    Estado_Sync = Convert.ToInt32(i.Estado_Sync)
                }
            ).ToList();

            return list;
        }

        public Dictionary<string, int> GetCountRecordAsientosERP(string storesID, string hqUsersID, string searchValue, string estadoAsiento, string fromDate, string toDate)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            foreach (var i in db.UEP_ASIENTOS_ERP_GET_COUNT_RECORDS(storesID, hqUsersID, searchValue, estadoAsiento, fromDate, toDate))
            {
                dictionary.Add(TOTAL_FILTERED, i.TOTAL_FILTERED ?? 0);
                dictionary.Add(TOTAL_RECORDS, i.TOTAL_RECORDS ?? 0);
            }

            return dictionary;
        }

        #endregion
    }
}
