using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analitic.Entities;

namespace Analitic.DataAccess.DataAccessIntegration
{
    public class DT_DocumentosERP : DT
    {
        #region Variables
        public static readonly string TOTAL_FILTERED = "TOTAL_FILTERED";
        public static readonly string TOTAL_RECORDS = "TOTAL_RECORDS";

        #endregion

        #region Constructors

        public DT_DocumentosERP() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Obtener documentos de la tabla Integrafast_01
        /// </summary>
        public List<EN_DocumentosERP> GetAll(string storesID, string hqUsersID, string searchValue, string tipoDocumento, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            List<EN_DocumentosERP> list = (
                from i in db.UEP_DOCUMENTOSERP_GETALL(storesID, hqUsersID, searchValue, tipoDocumento, orderColumn, orderDirection, skip, take, fromDate, toDate)
                select new EN_DocumentosERP()
                {
                    //ID = i.DateApplied,
                    StoreId = i.StoreID,
                    StoreName = i.Name,
                    Documento = i.Documento.ToString(),
                    Fecha_Envio = (DateTime)(i.FechaEnvio == null ? new DateTime(1900, 01, 01) : i.FechaEnvio),
                    Respuesta_Envio = i.RespuestaEnvio,
                    Detalles_Envio = i.DetallesEnvio,
                    Status = i.Status,
                    Tipo = i.Tipo
                }
            ).ToList();

            return list;
        }

        public Dictionary<string, int> GetCountRecordDocsERP(string storesID, string hqUsersID, string searchValue, string tipoDocumento, string fromDate, string toDate)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            foreach (var i in db.UEP_DOCUMENTOSERP_GET_COUNT_RECORDS(storesID, hqUsersID, searchValue, tipoDocumento, fromDate, toDate))
            {
                dictionary.Add(TOTAL_FILTERED, i.TOTAL_FILTERED ?? 0);
                dictionary.Add(TOTAL_RECORDS, i.TOTAL_RECORDS ?? 0);
            }

            return dictionary;
        }

        #endregion
    }
}
