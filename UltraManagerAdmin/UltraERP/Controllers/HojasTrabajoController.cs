using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class HojasTrabajoController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<HojaTrabajoViewModel> HojasTrabajo = new List<HojaTrabajoViewModel>
        {
            CreateWorksheet(
                1854,
                251,
                1,
                "Actualizacion de precio promocional cafe CR",
                "Se aplicara nueva liquidacion para temporada alta en tiendas GAM.",
                "251",
                "Actualizar Inventario - Precio de Producto",
                "Laura Solis",
                new DateTime(2026, 5, 10, 8, 15, 0),
                new DateTime(2026, 5, 16, 22, 0, 0),
                null,
                9011,
                new List<HojaTrabajoTiendaViewModel>
                {
                    CreateStore(4, "Sucursal Escazu", 1, null),
                    CreateStore(7, "Sucursal Sabana", 1, null),
                    CreateStore(9, "San Pedro", 1, null)
                },
                new List<HojaTrabajoHistorialViewModel>
                {
                    CreateHistory(1, "Oficina Central", new DateTime(2026, 5, 10, 8, 15, 0), "Hoja creada por cambios masivos."),
                    CreateHistory(1, "Oficina Central", new DateTime(2026, 5, 10, 8, 17, 0), "Pendiente de aprobacion.")
                },
                new List<HojaTrabajoContenidoViewModel>
                {
                    CreateContent("Sucursal Escazu", true, "CAF-1820-500", "Cafe 1820 molido 500 g", "Cafe tostado nacional para retail.", 2995m, new DateTime(2026, 5, 16), new DateTime(2026, 5, 31), 6m, 24m, 2795m, 12m, ""),
                    CreateContent("Sucursal Sabana", true, "CAF-BRT-340", "Cafe Britt clasico 340 g", "Empaque premium para autoservicio.", 3845m, new DateTime(2026, 5, 16), new DateTime(2026, 5, 31), 4m, 18m, 3595m, 8m, ""),
                    CreateContent("San Pedro", false, "CAF-TRR-250", "Cafe Tarrazu 250 g", "Presentacion gourmet para tiendas de conveniencia.", 0m, null, null, null, null, null, null, "")
                }),
            CreateWorksheet(
                1855,
                261,
                2,
                "Descarga de articulos descontinuados",
                "Se retiraran articulos de baja rotacion del surtido temporal.",
                "261",
                "Descarga de Productos",
                "Mario Rojas",
                new DateTime(2026, 5, 9, 14, 40, 0),
                new DateTime(2026, 5, 15, 6, 0, 0),
                null,
                0,
                new List<HojaTrabajoTiendaViewModel>
                {
                    CreateStore(1, "San Jose Centro", 2, null),
                    CreateStore(4, "Sucursal Escazu", 2, null)
                },
                new List<HojaTrabajoHistorialViewModel>
                {
                    CreateHistory(1, "Oficina Central", new DateTime(2026, 5, 9, 14, 40, 0), "Hoja creada por depuracion de catalogo."),
                    CreateHistory(2, "Gerencia Comercial", new DateTime(2026, 5, 10, 9, 20, 0), "Aprobada para ejecucion.")
                },
                new List<HojaTrabajoContenidoViewModel>
                {
                    CreateContent("San Jose Centro", true, "GAL-POZ-6U", "Galleta surtida 6 u", "Producto con baja demanda en canal urbano.", null, null, null, null, null, null, null, ""),
                    CreateContent("Sucursal Escazu", true, "JUG-CRT-1L", "Jugo tropical 1 L", "Linea temporal para temporada anterior.", null, null, null, null, null, null, null, "")
                }),
            CreateWorksheet(
                1856,
                320,
                4,
                "Ajuste de impuesto categoria limpieza",
                "Actualizacion por reforma tributaria para productos de aseo.",
                "320",
                "Ajuste de Impuestos",
                "Paola Brenes",
                new DateTime(2026, 5, 4, 10, 0, 0),
                new DateTime(2026, 5, 12, 0, 0, 0),
                new DateTime(2026, 5, 12, 2, 15, 0),
                9013,
                new List<HojaTrabajoTiendaViewModel>
                {
                    CreateStore(1, "San Jose Centro", 4, new DateTime(2026, 5, 12, 2, 10, 0)),
                    CreateStore(3, "Cartago", 4, new DateTime(2026, 5, 12, 2, 12, 0)),
                    CreateStore(5, "Alajuela", 4, new DateTime(2026, 5, 12, 2, 14, 0))
                },
                new List<HojaTrabajoHistorialViewModel>
                {
                    CreateHistory(1, "Oficina Central", new DateTime(2026, 5, 4, 10, 0, 0), "Hoja creada por area fiscal."),
                    CreateHistory(2, "Gerencia Financiera", new DateTime(2026, 5, 5, 11, 5, 0), "Aprobada para aplicarse el 12 de mayo."),
                    CreateHistory(3, "Motor de integracion", new DateTime(2026, 5, 12, 2, 0, 0), "Proceso iniciado."),
                    CreateHistory(4, "Motor de integracion", new DateTime(2026, 5, 12, 2, 15, 0), "Proceso completado con exito.")
                },
                new List<HojaTrabajoContenidoViewModel>
                {
                    CreateContent("San Jose Centro", true, "DET-IRX-1K", "Detergente Irex 1 kg", "Presentacion polvo hogar.", null, null, null, null, null, null, null, "IVA 13%"),
                    CreateContent("Cartago", true, "CLN-MUL-900", "Limpiador multiuso 900 ml", "Linea institucional.", null, null, null, null, null, null, null, "IVA 13%"),
                    CreateContent("Alajuela", true, "JAB-LIQ-500", "Jabon liquido 500 ml", "Envase surtido para anaquel.", null, null, null, null, null, null, null, "IVA 13%")
                }),
            CreateWorksheet(
                1857,
                261,
                8,
                "Depuracion de referencias sin movimiento",
                "No se encontraron articulos para ejecutar en las tiendas seleccionadas.",
                "261",
                "Descarga de Productos",
                "Andres Montero",
                new DateTime(2026, 5, 3, 16, 20, 0),
                new DateTime(2026, 5, 11, 23, 0, 0),
                new DateTime(2026, 5, 11, 23, 5, 0),
                0,
                new List<HojaTrabajoTiendaViewModel>
                {
                    CreateStore(2, "Heredia", 8, new DateTime(2026, 5, 11, 23, 4, 0)),
                    CreateStore(6, "Liberia", 8, new DateTime(2026, 5, 11, 23, 4, 0))
                },
                new List<HojaTrabajoHistorialViewModel>
                {
                    CreateHistory(1, "Oficina Central", new DateTime(2026, 5, 3, 16, 20, 0), "Hoja creada."),
                    CreateHistory(2, "Gerencia Comercial", new DateTime(2026, 5, 4, 8, 30, 0), "Aprobada."),
                    CreateHistory(8, "Motor de integracion", new DateTime(2026, 5, 11, 23, 5, 0), "Completada sin cambios.")
                },
                new List<HojaTrabajoContenidoViewModel>())
        };

        public ActionResult Inicio()
        {
            List<HojaTrabajoViewModel> model;
            lock (SyncRoot)
            {
                model = HojasTrabajo.Select(Clone).OrderByDescending(x => x.ID).ToList();
            }

            return View(model);
        }

        public static HojaTrabajoViewModel RegistrarHoja(HojaTrabajoViewModel hoja)
        {
            if (hoja == null)
                return null;

            lock (SyncRoot)
            {
                var registrada = Clone(hoja);
                registrada.ID = HojasTrabajo.Count == 0 ? 1 : HojasTrabajo.Max(x => x.ID) + 1;
                registrada.FechaCreacion = registrada.FechaCreacion == default(DateTime) ? DateTime.Now : registrada.FechaCreacion;
                registrada.FechaEfectiva = registrada.FechaEfectiva == default(DateTime) ? registrada.FechaCreacion.Date.AddDays(1).AddHours(22) : registrada.FechaEfectiva;
                registrada.ArchivoID = registrada.ArchivoID <= 0 ? 9000 + registrada.ID : registrada.ArchivoID;
                registrada.TiendasTexto = registrada.Tiendas == null ? "" : String.Join(", ", registrada.Tiendas.Select(x => x.TiendaNombre));

                HojasTrabajo.Add(registrada);
                return Clone(registrada);
            }
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id, int estado)
        {
            lock (SyncRoot)
            {
                var hoja = HojasTrabajo.FirstOrDefault(x => x.ID == id);
                if (hoja == null)
                    return Json(new JsonResponse("Hoja no encontrada.", "No se encontro la hoja de trabajo seleccionada.", null, false));

                string validation = ValidateTransition(hoja, estado);
                if (!String.IsNullOrWhiteSpace(validation))
                    return Json(new JsonResponse("Cambio de estado no permitido.", validation, null, false));

                if (estado == 2 && (hoja.Contenido == null || hoja.Contenido.Count == 0))
                    estado = 8;

                hoja.EstadoID = estado;
                if (estado == 4 || estado == 8)
                    hoja.FechaAplicacion = DateTime.Now;

                hoja.Historial.Add(new HojaTrabajoHistorialViewModel
                {
                    EstadoID = estado,
                    Tienda = "Oficina Central",
                    FechaHora = DateTime.Now,
                    Comentario = BuildStatusComment(estado)
                });

                return Json(new JsonResponse("", "Estado actualizado correctamente.", Clone(hoja), true));
            }
        }

        public ActionResult DescargarArchivo(int id)
        {
            HojaTrabajoViewModel hoja;
            lock (SyncRoot)
            {
                hoja = HojasTrabajo.Select(Clone).FirstOrDefault(x => x.ID == id);
            }

            if (hoja == null || hoja.ArchivoID <= 0)
                return HttpNotFound();

            var builder = new StringBuilder();
            builder.AppendLine("Hoja de Trabajo");
            builder.AppendLine("Referencia: " + hoja.ID);
            builder.AppendLine("Titulo: " + hoja.Titulo);
            builder.AppendLine("Estilo: " + hoja.EstiloTexto);
            builder.AppendLine("Estado: " + hoja.EstadoTexto);
            builder.AppendLine("Fecha efectiva: " + hoja.FechaEfectivaTexto);
            builder.AppendLine("Tiendas: " + hoja.TiendasTexto);
            builder.AppendLine();
            builder.AppendLine("Contenido");
            foreach (var item in hoja.Contenido)
            {
                builder.AppendLine(item.Tienda + " | " + item.CodigoArticulo + " | " + item.Descripcion + " | " + (String.IsNullOrWhiteSpace(item.Impuesto) ? "-" : item.Impuesto));
            }

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            var fileName = "worksheet-" + hoja.ID + ".txt";
            return File(bytes, "text/plain", fileName);
        }

        private static string ValidateTransition(HojaTrabajoViewModel hoja, int estado)
        {
            if (hoja == null)
                return "No se encontro la hoja de trabajo.";

            if (estado == 1)
            {
                if (hoja.EstadoID != 2)
                    return "Solo una hoja Aprobada puede volver a Sin aprobar.";

                return "";
            }

            if (estado == 2)
            {
                if (hoja.EstadoID != 1)
                    return "Solo una hoja Sin aprobar puede aprobarse.";

                return "";
            }

            if (estado == 0)
            {
                if (hoja.EstadoID != 1 && hoja.EstadoID != 2)
                    return "Solo una hoja Sin aprobar o Aprobada puede suspenderse.";

                return "";
            }

            return "El estado solicitado no es valido.";
        }

        private static string BuildStatusComment(int estado)
        {
            switch (estado)
            {
                case 0: return "Hoja suspendida manualmente.";
                case 1: return "Hoja regresada a revision.";
                case 2: return "Hoja aprobada para proceso.";
                case 8: return "Hoja completada sin cambios por no tener contenido aplicable.";
                default: return "Estado actualizado.";
            }
        }

        private static HojaTrabajoViewModel CreateWorksheet(int id, int estiloID, int estadoID, string titulo, string notas, string tareaCodigo, string tareaDescripcion, string usuario, DateTime fechaCreacion, DateTime fechaEfectiva, DateTime? fechaAplicacion, int archivoID, List<HojaTrabajoTiendaViewModel> tiendas, List<HojaTrabajoHistorialViewModel> historial, List<HojaTrabajoContenidoViewModel> contenido)
        {
            return new HojaTrabajoViewModel
            {
                ID = id,
                EstiloID = estiloID,
                EstadoID = estadoID,
                Titulo = titulo,
                Notas = notas,
                TareaCodigo = tareaCodigo,
                TareaDescripcion = tareaDescripcion,
                Usuario = usuario,
                FechaCreacion = fechaCreacion,
                FechaEfectiva = fechaEfectiva,
                FechaAplicacion = fechaAplicacion,
                ArchivoID = archivoID,
                Tiendas = tiendas,
                Historial = historial,
                Contenido = contenido,
                TiendasTexto = tiendas == null ? "" : String.Join(", ", tiendas.Select(x => x.TiendaNombre))
            };
        }

        private static HojaTrabajoTiendaViewModel CreateStore(int id, string nombre, int estadoID, DateTime? fechaProcesado)
        {
            return new HojaTrabajoTiendaViewModel
            {
                TiendaID = id,
                TiendaNombre = nombre,
                EstadoID = estadoID,
                FechaProcesado = fechaProcesado
            };
        }

        private static HojaTrabajoHistorialViewModel CreateHistory(int estadoID, string tienda, DateTime fecha, string comentario)
        {
            return new HojaTrabajoHistorialViewModel
            {
                EstadoID = estadoID,
                Tienda = tienda,
                FechaHora = fecha,
                Comentario = comentario
            };
        }

        private static HojaTrabajoContenidoViewModel CreateContent(string tienda, bool disponible, string codigo, string descripcion, string descripcionExtendida, decimal? precioLiquidacion, DateTime? inicioLiquidacion, DateTime? finLiquidacion, decimal? limiteInferior, decimal? limiteSuperior, decimal? precioCantidad, decimal? cantidadLiquidacion, string impuesto)
        {
            return new HojaTrabajoContenidoViewModel
            {
                Tienda = tienda,
                Disponible = disponible,
                CodigoArticulo = codigo,
                Descripcion = descripcion,
                DescripcionExtendida = descripcionExtendida,
                PrecioLiquidacion = precioLiquidacion,
                InicioLiquidacion = inicioLiquidacion,
                FinLiquidacion = finLiquidacion,
                LimiteInferior = limiteInferior,
                LimiteSuperior = limiteSuperior,
                PrecioCantidad = precioCantidad,
                CantidadLiquidacion = cantidadLiquidacion,
                Impuesto = impuesto
            };
        }

        private static HojaTrabajoViewModel Clone(HojaTrabajoViewModel source)
        {
            if (source == null)
                return null;

            return new HojaTrabajoViewModel
            {
                ID = source.ID,
                EstiloID = source.EstiloID,
                EstadoID = source.EstadoID,
                Titulo = source.Titulo,
                Notas = source.Notas,
                TareaCodigo = source.TareaCodigo,
                TareaDescripcion = source.TareaDescripcion,
                Usuario = source.Usuario,
                FechaCreacion = source.FechaCreacion,
                FechaEfectiva = source.FechaEfectiva,
                FechaAplicacion = source.FechaAplicacion,
                TiendasTexto = source.TiendasTexto,
                ArchivoID = source.ArchivoID,
                Tiendas = source.Tiendas.Select(x => new HojaTrabajoTiendaViewModel
                {
                    TiendaID = x.TiendaID,
                    TiendaNombre = x.TiendaNombre,
                    EstadoID = x.EstadoID,
                    FechaProcesado = x.FechaProcesado
                }).ToList(),
                Historial = source.Historial.Select(x => new HojaTrabajoHistorialViewModel
                {
                    EstadoID = x.EstadoID,
                    Tienda = x.Tienda,
                    FechaHora = x.FechaHora,
                    Comentario = x.Comentario
                }).ToList(),
                Contenido = source.Contenido.Select(x => new HojaTrabajoContenidoViewModel
                {
                    Tienda = x.Tienda,
                    Disponible = x.Disponible,
                    CodigoArticulo = x.CodigoArticulo,
                    Descripcion = x.Descripcion,
                    DescripcionExtendida = x.DescripcionExtendida,
                    PrecioLiquidacion = x.PrecioLiquidacion,
                    InicioLiquidacion = x.InicioLiquidacion,
                    FinLiquidacion = x.FinLiquidacion,
                    LimiteInferior = x.LimiteInferior,
                    LimiteSuperior = x.LimiteSuperior,
                    PrecioCantidad = x.PrecioCantidad,
                    CantidadLiquidacion = x.CantidadLiquidacion,
                    Impuesto = x.Impuesto
                }).ToList()
            };
        }
    }
}
