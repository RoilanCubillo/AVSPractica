using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_DocumentosMH
    {
        public int ID { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int TransactionNumber { get; set; }
        public string Clave50 { get; set; }
        public string Clave20 { get; set; }
        public string Cliente { get; set; }
        public string Cod_Cliente { get; set; }
        public DateTime? Fecha_Transac { get; set; }
        public string Fecha_TransacString { get { return Fecha_Transac == null ? "" : Fecha_Transac.Value.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public DateTime? Fecha_Hacienda { get; set; }
        public string Fecha_HaciendaString { get { return Fecha_Hacienda == null ? "" : Fecha_Hacienda.Value.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public int NC_Referencia { get; set; }
        public DateTime? NC_Referencia_Fecha { get; set; }
        public string NC_Referencia_FechaString { get { return NC_Referencia_Fecha == null ? "" : NC_Referencia_Fecha.Value.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public int Comprobante_Tipo { get; set; }
        public string Observaciones { get; set; }
        public int Estado_Hacienda { get; set; }
        public string XML_Respuesta { get; set; }
    }
}
