using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analitic.Entities
{
    public class EN_DocumentosERP
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string Documento { get; set; }
        public DateTime? Fecha_Envio { get; set; }
        public string Fecha_EnvioString { get { return Fecha_Envio == null ? "" : Fecha_Envio.Value.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public string Respuesta_Envio { get; set; }
        public string Detalles_Envio { get; set; }
        public int Status { get; set; }
        public string Tipo { get; set; }
    }
}
