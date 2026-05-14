using System;
using System.Collections.Generic;

namespace UltraERP.Models
{
    public class HojaTrabajoViewModel
    {
        public int ID { get; set; }
        public int EstiloID { get; set; }
        public int EstadoID { get; set; }
        public string Titulo { get; set; }
        public string Notas { get; set; }
        public string TareaCodigo { get; set; }
        public string TareaDescripcion { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaEfectiva { get; set; }
        public DateTime? FechaAplicacion { get; set; }
        public string TiendasTexto { get; set; }
        public int ArchivoID { get; set; }
        public List<HojaTrabajoTiendaViewModel> Tiendas { get; set; }
        public List<HojaTrabajoHistorialViewModel> Historial { get; set; }
        public List<HojaTrabajoContenidoViewModel> Contenido { get; set; }

        public HojaTrabajoViewModel()
        {
            Tiendas = new List<HojaTrabajoTiendaViewModel>();
            Historial = new List<HojaTrabajoHistorialViewModel>();
            Contenido = new List<HojaTrabajoContenidoViewModel>();
        }

        public string EstiloTexto
        {
            get
            {
                switch (EstiloID)
                {
                    case 251: return "Actualizar Inventario - Precio de Producto";
                    case 261: return "Descarga de Productos";
                    case 320: return "Ajuste de Impuestos";
                    default: return "Hoja de trabajo";
                }
            }
        }

        public string EstadoTexto
        {
            get
            {
                switch (EstadoID)
                {
                    case 0: return "Suspendido";
                    case 1: return "Sin aprobar";
                    case 2: return "Aprobado";
                    case 3: return "En proceso";
                    case 4: return "Completado con exito";
                    case 5: return "Completado con errores";
                    case 6: return "Completado con advertencias";
                    case 7: return "Reconocido y grabado";
                    case 8: return "Completado sin cambios";
                    default: return "Desconocido";
                }
            }
        }

        public string EstadoClase
        {
            get
            {
                switch (EstadoID)
                {
                    case 0: return "closed";
                    case 1: return "draft";
                    case 2: return "sent";
                    case 3: return "warning";
                    case 4: return "received";
                    case 5: return "danger";
                    case 6: return "neutral";
                    case 7: return "info";
                    case 8: return "success";
                    default: return "neutral";
                }
            }
        }

        public string FechaCreacionTexto
        {
            get { return FechaCreacion.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string FechaCreacionFiltro
        {
            get { return FechaCreacion.ToString("yyyy-MM-dd"); }
        }

        public string FechaEfectivaTexto
        {
            get { return FechaEfectiva.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string FechaEfectivaFiltro
        {
            get { return FechaEfectiva.ToString("yyyy-MM-dd"); }
        }

        public string FechaAplicacionTexto
        {
            get { return FechaAplicacion.HasValue ? FechaAplicacion.Value.ToString("dd/MM/yyyy HH:mm") : "-"; }
        }

        public int CantidadTiendas
        {
            get { return Tiendas == null ? 0 : Tiendas.Count; }
        }

        public int CantidadContenido
        {
            get { return Contenido == null ? 0 : Contenido.Count; }
        }
    }

    public class HojaTrabajoTiendaViewModel
    {
        public int TiendaID { get; set; }
        public string TiendaNombre { get; set; }
        public int EstadoID { get; set; }
        public DateTime? FechaProcesado { get; set; }

        public string EstadoTexto
        {
            get
            {
                switch (EstadoID)
                {
                    case 0: return "Suspendido";
                    case 1: return "Sin aprobar";
                    case 2: return "Aprobado";
                    case 3: return "En proceso";
                    case 4: return "Completado con exito";
                    case 5: return "Completado con errores";
                    case 6: return "Completado con advertencias";
                    case 7: return "Reconocido y grabado";
                    case 8: return "Completado sin cambios";
                    default: return "Desconocido";
                }
            }
        }

        public string FechaProcesadoTexto
        {
            get { return FechaProcesado.HasValue ? FechaProcesado.Value.ToString("dd/MM/yyyy HH:mm") : "-"; }
        }
    }

    public class HojaTrabajoHistorialViewModel
    {
        public int EstadoID { get; set; }
        public string Tienda { get; set; }
        public DateTime FechaHora { get; set; }
        public string Comentario { get; set; }

        public string EstadoTexto
        {
            get
            {
                switch (EstadoID)
                {
                    case 0: return "Suspendido";
                    case 1: return "Sin aprobar";
                    case 2: return "Aprobado";
                    case 3: return "En proceso";
                    case 4: return "Completado con exito";
                    case 5: return "Completado con errores";
                    case 6: return "Completado con advertencias";
                    case 7: return "Reconocido y grabado";
                    case 8: return "Completado sin cambios";
                    default: return "Desconocido";
                }
            }
        }

        public string FechaHoraTexto
        {
            get { return FechaHora.ToString("dd/MM/yyyy HH:mm:ss"); }
        }
    }

    public class HojaTrabajoContenidoViewModel
    {
        public string Tienda { get; set; }
        public bool Disponible { get; set; }
        public string CodigoArticulo { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionExtendida { get; set; }
        public decimal? PrecioLiquidacion { get; set; }
        public DateTime? InicioLiquidacion { get; set; }
        public DateTime? FinLiquidacion { get; set; }
        public decimal? LimiteInferior { get; set; }
        public decimal? LimiteSuperior { get; set; }
        public decimal? PrecioCantidad { get; set; }
        public decimal? CantidadLiquidacion { get; set; }
        public string Impuesto { get; set; }

        public string InicioLiquidacionTexto
        {
            get { return InicioLiquidacion.HasValue ? InicioLiquidacion.Value.ToString("dd/MM/yyyy") : "-"; }
        }

        public string FinLiquidacionTexto
        {
            get { return FinLiquidacion.HasValue ? FinLiquidacion.Value.ToString("dd/MM/yyyy") : "-"; }
        }
    }
}
