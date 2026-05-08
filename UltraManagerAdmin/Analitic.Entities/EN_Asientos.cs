using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analitic.Entities
{
    public class EN_Asientos
    {
        public int ID { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string Descripcion { get; set; }
        public string Referencia { get; set; }
        public DateTime? Fecha_Asiento { get; set; }
        public string Fecha_AsientoString { get { return Fecha_Asiento == null ? "" : Fecha_Asiento.Value.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public DateTime? Fecha_Sync { get; set; }
        public string Fecha_AsientoSyncString { get { return Fecha_Sync == null ? "" : Fecha_Sync.Value.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public string Detalle_Sync { get; set; }
        public int Estado_Sync { get; set; }
    }
}
