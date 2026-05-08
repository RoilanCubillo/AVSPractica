using System;
using System.Collections.Generic;

namespace UltraERP.Models
{
    public class DocumentoInventarioViewModel
    {
        public int ID { get; set; }
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public string Proveedor { get; set; }
        public string Producto { get; set; }
        public string FacturaRef { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public DateTime? FechaAplicacion { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }
        public string PersonaSolicita { get; set; }
        public string TipoSalida { get; set; }
        public string JustificacionSalida { get; set; }
        public string JustificacionEntrada { get; set; }
        public List<DocumentoDetalleLineaViewModel> DetalleLineas { get; set; }
        public int CantidadLineasDetalle { get; set; }
        public List<DocumentoLineaEspecialViewModel> LineasEspeciales { get; set; }
        public int CantidadLineasEspeciales { get; set; }
        public decimal TotalLineasEspeciales { get; set; }
        public string ResumenAuditoria { get; set; }
    }
}
