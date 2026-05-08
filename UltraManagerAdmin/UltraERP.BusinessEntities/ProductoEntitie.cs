using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class ProductoEntitie
    {
        public int intIdProducto { get; set; }
        public int intIdCliente { get; set; }
        public string strCodigo { get; set; }
        public string strCodCabys { get; set; }

        public string strDescripCabys { get; set; }
        public string strNombre { get; set; }
        public string strNombreExt { get; set; }
        public string strUnidadMedida { get; set; }
        public string strCodImpuesto { get; set; }
        public string strCodTarifa { get; set; }

        public decimal decPrecioCol { get; set; }
        public decimal decPrecioDol { get; set; }
        public int intEstado { get; set; }
        public int id { get; set; }
        public string strdescripcion { get; set; }
        public string strSimboloUM { get; set; }

        public double? TarifaIVA { get; set; }

    }
}
