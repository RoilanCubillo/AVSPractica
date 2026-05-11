using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace UltraERP.Models
{
    public class DocumentoInventarioRegistroViewModel
    {
        public int? DocumentoId { get; set; }

        [Display(Name = "Tipo de Documento")]
        [Required(ErrorMessage = "Seleccione el tipo de documento.")]
        public string TipoDocumento { get; set; }

        [Display(Name = "Num. Orden / Documento")]
        public string NumeroDocumento { get; set; }

        public string Estado { get; set; }

        [Display(Name = "Fecha Solicitud")]
        [Required(ErrorMessage = "Ingrese la fecha de solicitud.")]
        public DateTime FechaSolicitud { get; set; }

        [Display(Name = "Fecha Entrega")]
        public DateTime? FechaEntrega { get; set; }

        [Display(Name = "Fecha Aplicacion")]
        public DateTime? FechaAplicacion { get; set; }

        [Display(Name = "Proveedor")]
        [Required(ErrorMessage = "Seleccione un proveedor.")]
        public string ProveedorBusqueda { get; set; }

        [Display(Name = "Codigo proveedor")]
        public string CodigoProveedor { get; set; }

        [Display(Name = "Nombre proveedor")]
        public string NombreProveedor { get; set; }

        [Display(Name = "Plazo credito")]
        public int PlazoCredito { get; set; }

        [Display(Name = "Clave Hacienda")]
        public string ClaveHacienda { get; set; }

        public string Consecutivo { get; set; }

        public string Emisor { get; set; }

        [Display(Name = "Fecha Emision")]
        public DateTime? FechaEmision { get; set; }

        [Display(Name = "Total Factura")]
        public decimal? TotalFactura { get; set; }

        [Display(Name = "Estado FacturaMeCR")]
        public string EstadoFacturaMeCR { get; set; }

        [Display(Name = "Numero de Factura Ref.")]
        public string FacturaRef { get; set; }

        [Display(Name = "Persona Solicita")]
        [Required(ErrorMessage = "Ingrese la persona que solicita.")]
        public string PersonaSolicita { get; set; }

        [Display(Name = "Tipo Salida")]
        public string TipoSalida { get; set; }

        [Display(Name = "Justificacion Salida")]
        public string JustificacionSalida { get; set; }

        [Display(Name = "Justificacion Entrada")]
        public string JustificacionEntrada { get; set; }

        public string Comentario { get; set; }
        public string LineasEspecialesJson { get; set; }
        public string DetalleLineasJson { get; set; }

        public IEnumerable<SelectListItem> TiposDocumento { get; set; }
        public IEnumerable<SelectListItem> TiposSalida { get; set; }
        public IEnumerable<SelectListItem> JustificacionesSalida { get; set; }
        public IEnumerable<SelectListItem> JustificacionesEntrada { get; set; }
    }
}
