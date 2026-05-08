using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_DocumentosMH : DT
    {
        #region Variables
        public static readonly string TOTAL_FILTERED = "TOTAL_FILTERED";
        public static readonly string TOTAL_RECORDS = "TOTAL_RECORDS";

        #endregion

        #region Constructors

        public DT_DocumentosMH() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Obtener documentos de la tabla Integrafast_01
        /// </summary>
        public List<EN_DocumentosMH> GetAll(string storesID, string hqUsersID, string searchValue, string estadoHacienda, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            List<EN_DocumentosMH> list = (
                from i in db.UEP_DOCUMENTOSMH_GETALL(storesID, hqUsersID, searchValue, estadoHacienda, orderColumn, orderDirection, skip, take, fromDate, toDate)
                select new EN_DocumentosMH()
                {
                    //ID = i.DateApplied,
                    StoreId = i.STORE_ID,
                    StoreName = i.Name,
                    TransactionNumber = Convert.ToInt32(i.TRANSACTIONNUMBER),
                    Clave50 = i.CLAVE50,
                    Clave20 = i.CLAVE20,
                    Cliente = i.NOMBRE_CLIENTE,
                    Cod_Cliente = i.COD_CLIENTE,
                    Fecha_Transac = (DateTime)(i.FECHA_TRANSAC == null ? new DateTime(1900, 01, 01) : i.FECHA_TRANSAC),
                    Fecha_Hacienda = (DateTime)(i.FECHA_HACIENDA == null ? new DateTime(1900, 01, 01) : i.FECHA_HACIENDA),
                    NC_Referencia = Convert.ToInt32((String.IsNullOrEmpty(i.NC_REFERENCIA) ? "0" : i.NC_REFERENCIA)),
                    NC_Referencia_Fecha = (DateTime)(i.NC_REFERENCIA_FECHA == null ? new DateTime(1900, 01, 01) : i.NC_REFERENCIA_FECHA),
                    Comprobante_Tipo = Convert.ToInt32(i.Comprobante_Tipo),
                    Estado_Hacienda = Convert.ToInt32(i.Estado_Hacienda),
                    XML_Respuesta = i.XML_RESPUESTA
                }
            ).ToList();

            return list;
        }

        public Dictionary<string, int> GetCountRecordDocsMH(string storesID, string hqUsersID, string searchValue, string estadoHacienda, string fromDate, string toDate)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            foreach (var i in db.UEP_DOCUMENTOSMH_GET_COUNT_RECORDS(storesID, hqUsersID, searchValue, estadoHacienda, fromDate, toDate))
            {
                dictionary.Add(TOTAL_FILTERED, i.TOTAL_FILTERED ?? 0);
                dictionary.Add(TOTAL_RECORDS, i.TOTAL_RECORDS ?? 0);
            }

            return dictionary;
        }

        #endregion
    }
}
