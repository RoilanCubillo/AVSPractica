using System;
using System.Collections.Generic;
using System.Linq;

namespace UltraERP.Models
{
    public class MonitoreoDatosViewModel
    {
        public IList<MonitoreoTiendaViewModel> Tiendas { get; set; }
        public IList<MonitoreoDocumentoViewModel> DocumentosMH { get; set; }
        public IList<MonitoreoDocumentoViewModel> DocumentosERP { get; set; }
        public IList<MonitoreoAsientoViewModel> AsientosERP { get; set; }
        public IList<string> Alertas { get; set; }
        public string FechaDesdeDefault { get; set; }
        public string FechaHastaDefault { get; set; }

        public MonitoreoDatosViewModel()
        {
            Tiendas = new List<MonitoreoTiendaViewModel>();
            DocumentosMH = new List<MonitoreoDocumentoViewModel>();
            DocumentosERP = new List<MonitoreoDocumentoViewModel>();
            AsientosERP = new List<MonitoreoAsientoViewModel>();
            Alertas = new List<string>();
        }

        public int TotalDocumentos
        {
            get { return (DocumentosMH == null ? 0 : DocumentosMH.Count) + (DocumentosERP == null ? 0 : DocumentosERP.Count); }
        }

        public int TotalAceptados
        {
            get
            {
                return (DocumentosMH == null ? 0 : DocumentosMH.Count(x => x.EstadoID == 1)) +
                    (DocumentosERP == null ? 0 : DocumentosERP.Count(x => x.EstadoID == 1));
            }
        }

        public int TotalRechazados
        {
            get
            {
                return (DocumentosMH == null ? 0 : DocumentosMH.Count(x => x.EstadoID == 2)) +
                    (DocumentosERP == null ? 0 : DocumentosERP.Count(x => x.EstadoID == 2));
            }
        }

        public int TotalAsientosPendientes
        {
            get { return AsientosERP == null ? 0 : AsientosERP.Count(x => x.EstadoID == 0); }
        }
    }

    public class MonitoreoTiendaViewModel
    {
        public string ID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }

        public string Texto
        {
            get { return Codigo + " - " + Nombre; }
        }
    }

    public class MonitoreoDocumentoViewModel
    {
        public int ID { get; set; }
        public string Origen { get; set; }
        public string TiendaID { get; set; }
        public string Tienda { get; set; }
        public string Consecutivo { get; set; }
        public string Clave { get; set; }
        public string ComprobanteTipo { get; set; }
        public string Cliente { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaSincronizacion { get; set; }
        public decimal Total { get; set; }
        public int EstadoID { get; set; }
        public string Mensaje { get; set; }

        public string FechaTexto
        {
            get { return Fecha.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string FechaFiltro
        {
            get { return Fecha.ToString("yyyy-MM-dd"); }
        }

        public string FechaSincronizacionTexto
        {
            get { return FechaSincronizacion.HasValue ? FechaSincronizacion.Value.ToString("dd/MM/yyyy HH:mm") : "-"; }
        }

        public string EstadoTexto
        {
            get
            {
                switch (EstadoID)
                {
                    case 1: return "Aceptado";
                    case 2: return "Rechazado";
                    case 3: return "Pendiente";
                    default: return "Enviado";
                }
            }
        }

        public string EstadoClase
        {
            get
            {
                switch (EstadoID)
                {
                    case 1: return "received";
                    case 2: return "danger";
                    case 3: return "draft";
                    default: return "sent";
                }
            }
        }
    }

    public class MonitoreoAsientoViewModel
    {
        public int ID { get; set; }
        public string TiendaID { get; set; }
        public string Tienda { get; set; }
        public string NumeroAsiento { get; set; }
        public string Referencia { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaAsiento { get; set; }
        public DateTime? FechaSincronizacion { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public int EstadoID { get; set; }
        public string Mensaje { get; set; }

        public string FechaAsientoTexto
        {
            get { return FechaAsiento.ToString("dd/MM/yyyy"); }
        }

        public string FechaFiltro
        {
            get { return FechaAsiento.ToString("yyyy-MM-dd"); }
        }

        public string FechaSincronizacionTexto
        {
            get { return FechaSincronizacion.HasValue ? FechaSincronizacion.Value.ToString("dd/MM/yyyy HH:mm") : "-"; }
        }

        public string EstadoTexto
        {
            get
            {
                switch (EstadoID)
                {
                    case 1: return "Enviado";
                    case 2: return "Con error";
                    default: return "Sin enviar";
                }
            }
        }

        public string EstadoClase
        {
            get
            {
                switch (EstadoID)
                {
                    case 1: return "received";
                    case 2: return "danger";
                    default: return "draft";
                }
            }
        }
    }
}
