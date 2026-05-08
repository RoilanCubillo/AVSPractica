using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analitic.Entities
{
    public class EN_Graphics
    {
        public string Meses { get; set; }
        public int Doc_Aceptados { get; set; }
        public int Doc_Rechazados { get; set; }
        public int Tot_Doc_Aceptados { get; set; }
        public int Tot_Doc_Rechazados { get; set; }


        public int Total_Ventas { get; set; }
        public int Total_Ventas_Sync { get; set; }
        public int Total_NcVentas { get; set; }
        public int Total_NcVentas_Sync { get; set; }
        public int Total_Compras { get; set; }
        public int Total_Compras_Sync { get; set; }
        public int Total_NcCompras { get; set; }
        public int Total_NcCompras_Sync { get; set; }
        public int Total_Depositos { get; set; }
        public int Total_Depositos_Sync { get; set; }
        public int Total_Docs_NoSync { get; set; }

        public int Total_Asientos { get; set; }
        public int Total_Asientos_Sync { get; set; }
        public int Total_Asientos_No_Sync { get; set; }
        public int Total_Asientos_Error { get; set; }
    }
}
