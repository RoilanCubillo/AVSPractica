using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class DocumentosInventarioController : Controller
    {
        private static readonly object SyncRoot = new object();
        private static readonly List<ProveedorCatalogo> Proveedores = new List<ProveedorCatalogo>
        {
            new ProveedorCatalogo("00129", "CONEJO DORADO S.R.L.", 30),
            new ProveedorCatalogo("00210", "Distribuidora Central", 15),
            new ProveedorCatalogo("00345", "Comercial La Union", 45)
        };

        private static readonly List<DocumentoInventarioViewModel> Documentos = new List<DocumentoInventarioViewModel>
        {
            CreateDocumento(
                1,
                "DOC-000124",
                "Compra",
                "Distribuidora Central",
                "FAC-85412",
                new DateTime(2026, 5, 1),
                new DateTime(2026, 5, 4),
                null,
                "Borrador",
                "Maria Lopez",
                "",
                "",
                "",
                new List<DocumentoDetalleLineaViewModel>
                {
                    CreateDetalle("EMP-001", "Insumos de empaque", "CJ", 15m, 4520.50m, 0m, 0m, 13m, false, ""),
                    CreateDetalle("FIL-014", "Film termico", "UND", 24m, 1295.75m, 0m, 0m, 13m, false, "")
                }),
            CreateDocumento(
                2,
                "DOC-000125",
                "Entrada de Inventario",
                "Bodega Principal",
                "TR-7781",
                new DateTime(2026, 5, 2),
                new DateTime(2026, 5, 3),
                new DateTime(2026, 5, 3),
                "Recibida",
                "Carlos Mendez",
                "",
                "",
                "Compra a proveedor",
                new List<DocumentoDetalleLineaViewModel>
                {
                    CreateDetalle("ARR-220", "Arroz precocido", "KG", 40m, 2750m, 0m, 0m, 1m, false, "")
                }),
            CreateDocumento(
                3,
                "DOC-000126",
                "Salida de Inventario",
                "Sucursal Escazu",
                "",
                new DateTime(2026, 5, 5),
                null,
                null,
                "Enviada",
                "Ana Vargas",
                "Est\u00e1ndar",
                "Uso interno",
                "",
                new List<DocumentoDetalleLineaViewModel>
                {
                    CreateDetalle("ACE-015", "Aceite vegetal", "UND", 12m, 2590m, 0m, 0m, 13m, false, "Solicitud operativa")
                }),
            CreateDocumento(
                4,
                "DOC-000127",
                "Compra",
                "Comercial La Union",
                "FAC-99103",
                new DateTime(2026, 4, 25),
                new DateTime(2026, 4, 29),
                new DateTime(2026, 4, 30),
                "Parcial",
                "Jose Ramirez",
                "",
                "",
                "",
                new List<DocumentoDetalleLineaViewModel>
                {
                    CreateDetalle("SAL-441", "Salsa base", "GAL", 18m, 74850m, 3m, 0m, 13m, false, "")
                }),
            CreateDocumento(
                5,
                "DOC-000128",
                "Entrada de Inventario",
                "Proveedor Demo",
                "FAC-12845",
                new DateTime(2026, 5, 6),
                null,
                new DateTime(2026, 5, 6),
                "Cerrada",
                "Laura Solis",
                "",
                "",
                "Ajuste positivo",
                new List<DocumentoDetalleLineaViewModel>
                {
                    CreateDetalle("HAR-112", "Harina preparada", "QQ", 8m, 10900m, 0m, 0m, 1m, false, "")
                }),
            CreateDocumento(
                6,
                "SA00000112",
                "Salida de Inventario",
                "Sucursal Sabana",
                "AJ-55820",
                new DateTime(2026, 4, 28),
                new DateTime(2026, 4, 28),
                new DateTime(2026, 4, 29),
                "Cerrada",
                "Mario Rojas",
                "Proveedor",
                "Devoluci\u00f3n a proveedor",
                "",
                new List<DocumentoDetalleLineaViewModel>
                {
                    CreateDetalle("CAF-009", "Cafe molido", "BQ", 10m, 4521m, 0m, 0m, 13m, false, ""),
                    CreateDetalle("PROMO-1", "Producto de cortesia", "UND", 4m, 0m, 0m, 0m, 0m, true, "Linea de regalia auditada")
                })
        };

        public ActionResult Inicio()
        {
            return View();
        }

        public ActionResult Historico()
        {
            return View();
        }

        public ActionResult Registro(string tipoDocumento)
        {
            var tipo = NormalizeTipoDocumento(tipoDocumento);
            var today = DateTime.Today;

            var model = new DocumentoInventarioRegistroViewModel
            {
                TipoDocumento = tipo,
                NumeroDocumento = GetNextNumero(tipo),
                Estado = "Borrador",
                FechaSolicitud = today,
                FechaEntrega = today.AddDays(7),
                FechaAplicacion = today,
                PersonaSolicita = User.Identity.Name,
                PlazoCredito = 30,
                EstadoFacturaMeCR = "Pendiente",
                TipoSalida = "Est\u00e1ndar",
                JustificacionSalida = "",
                JustificacionEntrada = "",
                LineasEspecialesJson = "[]",
                DetalleLineasJson = "[]",
                TiposDocumento = GetTiposDocumento(tipo),
                TiposSalida = GetTiposSalida("Est\u00e1ndar"),
                JustificacionesSalida = GetJustificacionesSalida(null),
                JustificacionesEntrada = GetJustificacionesEntrada(null)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GuardarEncabezado(DocumentoInventarioRegistroViewModel model, string accion)
        {
            try
            {
                if (model == null)
                    return Json(new JsonResponse("Datos invalidos.", "No se recibio el encabezado del documento.", null, false));

                var validationMessage = ValidateEncabezado(model, accion);
                if (!String.IsNullOrWhiteSpace(validationMessage))
                {
                    return Json(new JsonResponse("Validacion de encabezado.", validationMessage, null, false));
                }

                lock (SyncRoot)
                {
                    var detalleLineas = ParseDetalleLineas(model.DetalleLineasJson);
                    var detalleValidationMessage = ValidateDetalleLineas(detalleLineas);
                    if (!String.IsNullOrWhiteSpace(detalleValidationMessage))
                    {
                        return Json(new JsonResponse("Validacion de detalle.", detalleValidationMessage, null, false));
                    }

                    var lineasEspeciales = BuildLineasEspeciales(detalleLineas);
                    var nextId = Documentos.Count == 0 ? 1 : Documentos.Max(x => x.ID) + 1;

                    var documento = new DocumentoInventarioViewModel
                    {
                        ID = nextId,
                        Numero = GetNextNumero(model.TipoDocumento),
                        Tipo = ToListTipo(model.TipoDocumento),
                        Proveedor = !String.IsNullOrWhiteSpace(model.NombreProveedor) ? model.NombreProveedor : model.ProveedorBusqueda,
                        Producto = "",
                        FacturaRef = model.FacturaRef,
                        FechaSolicitud = model.FechaSolicitud == DateTime.MinValue ? DateTime.Today : model.FechaSolicitud,
                        FechaEntrega = model.FechaEntrega,
                        FechaAplicacion = model.FechaAplicacion,
                        Estado = "Borrador",
                        Total = detalleLineas.Any() ? detalleLineas.Sum(x => x.TotalLinea) : (model.TotalFactura ?? 0m),
                        PersonaSolicita = model.PersonaSolicita,
                        TipoSalida = model.TipoSalida,
                        JustificacionSalida = model.JustificacionSalida,
                        JustificacionEntrada = model.JustificacionEntrada,
                        DetalleLineas = detalleLineas,
                        CantidadLineasDetalle = detalleLineas.Count,
                        LineasEspeciales = lineasEspeciales,
                        CantidadLineasEspeciales = lineasEspeciales.Count,
                        TotalLineasEspeciales = lineasEspeciales.Sum(x => x.Cantidad * x.Costo),
                        ResumenAuditoria = BuildResumenAuditoria(lineasEspeciales)
                    };

                    var transitionMessage = ApplyEstadoFromAccion(documento, accion);
                    if (!String.IsNullOrWhiteSpace(transitionMessage))
                    {
                        return Json(new JsonResponse("Cambio de estado no permitido.", transitionMessage, null, false));
                    }

                    documento.Producto = BuildResumenProducto(detalleLineas, lineasEspeciales);

                    Documentos.Add(documento);
                    return Json(new JsonResponse("", "Documento guardado correctamente.", ToGrid(documento), true));
                }
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(e.Message, "No se pudo guardar el encabezado.", null, false));
            }
        }

        public JsonResult GetDocumentos(string tipoDocumento, string estado, string fechaDesde, string fechaHasta, string proveedor, string numeroDocumento, string facturaRef, string personaSolicita)
        {
            try
            {
                DateTime? desde = ParseDate(fechaDesde);
                DateTime? hasta = ParseDate(fechaHasta);
                List<DocumentoInventarioViewModel> documentos;

                lock (SyncRoot)
                {
                    documentos = Documentos.Select(Clone).ToList();
                }

                var result = documentos.Where(x =>
                        IsTipoMatch(tipoDocumento, x.Tipo) &&
                        IsMatch(estado, "Todos", x.Estado) &&
                        (!desde.HasValue || x.FechaSolicitud.Date >= desde.Value.Date) &&
                        (!hasta.HasValue || x.FechaSolicitud.Date <= hasta.Value.Date) &&
                        Contains(x.Proveedor, proveedor) &&
                        Contains(x.Numero, numeroDocumento) &&
                        Contains(x.FacturaRef, facturaRef) &&
                        Contains(x.PersonaSolicita, personaSolicita))
                    .OrderByDescending(x => x.FechaSolicitud)
                    .ThenByDescending(x => x.ID)
                    .Select(ToGrid)
                    .ToList();

                return Json(new JsonResponse("", "", result, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(e.Message, "No se pudo obtener la lista de documentos.", null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetHistoricoAjustes(string tipoDocumento, string fechaAplicacion, string proveedor, string usuario, string facturaRef, string producto)
        {
            try
            {
                DateTime? aplicacion = ParseDate(fechaAplicacion);
                List<DocumentoInventarioViewModel> documentos;

                lock (SyncRoot)
                {
                    documentos = Documentos.Select(Clone).ToList();
                }

                var result = documentos.Where(x =>
                        String.Equals(x.Estado, "Cerrada", StringComparison.OrdinalIgnoreCase) &&
                        IsTipoMatch(tipoDocumento, x.Tipo) &&
                        (!aplicacion.HasValue || (x.FechaAplicacion.HasValue && x.FechaAplicacion.Value.Date == aplicacion.Value.Date)) &&
                        Contains(x.Proveedor, proveedor) &&
                        Contains(x.PersonaSolicita, usuario) &&
                        Contains(x.FacturaRef, facturaRef) &&
                        Contains(x.Producto, producto))
                    .OrderByDescending(x => x.FechaAplicacion ?? DateTime.MinValue)
                    .ThenByDescending(x => x.ID)
                    .Select(ToGrid)
                    .ToList();

                return Json(new JsonResponse("", "", result, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(e.Message, "No se pudo obtener el historico de ajustes.", null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Duplicar(int id)
        {
            try
            {
                lock (SyncRoot)
                {
                    var source = Documentos.FirstOrDefault(x => x.ID == id);
                    if (source == null)
                        return Json(new JsonResponse("Documento no encontrado.", "No se encontro el documento seleccionado.", null, false), JsonRequestBehavior.AllowGet);

                    var nextId = Documentos.Max(x => x.ID) + 1;
                    var copy = Clone(source);
                    copy.ID = nextId;
                    copy.Numero = GetNextNumero(copy.Tipo);
                    copy.Estado = "Borrador";
                    copy.FacturaRef = "";
                    copy.FechaSolicitud = DateTime.Today;
                    copy.FechaEntrega = null;
                    copy.FechaAplicacion = null;
                    copy.LineasEspeciales = BuildLineasEspeciales(copy.DetalleLineas);
                    copy.CantidadLineasDetalle = copy.DetalleLineas == null ? 0 : copy.DetalleLineas.Count;
                    copy.CantidadLineasEspeciales = copy.LineasEspeciales.Count;
                    copy.TotalLineasEspeciales = copy.LineasEspeciales.Sum(x => x.Cantidad * x.Costo);
                    copy.ResumenAuditoria = BuildResumenAuditoria(copy.LineasEspeciales);
                    copy.Producto = BuildResumenProducto(copy.DetalleLineas, copy.LineasEspeciales);
                    Documentos.Add(copy);

                    return Json(new JsonResponse("", "Documento duplicado como borrador. Revise factura y fechas antes de enviarlo.", ToGrid(copy), true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(e.Message, "No se pudo duplicar el documento.", null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EjecutarAccion(int id, string accion)
        {
            try
            {
                lock (SyncRoot)
                {
                    var documento = Documentos.FirstOrDefault(x => x.ID == id);
                    if (documento == null)
                        return Json(new JsonResponse("Documento no encontrado.", "No se encontro el documento seleccionado.", null, false), JsonRequestBehavior.AllowGet);

                    var transitionMessage = ApplyEstadoFromAccion(documento, accion);
                    if (!String.IsNullOrWhiteSpace(transitionMessage))
                    {
                        return Json(new JsonResponse("Cambio de estado no permitido.", transitionMessage, null, false), JsonRequestBehavior.AllowGet);
                    }

                    return Json(new JsonResponse("", "Documento actualizado correctamente.", ToGrid(documento), true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(e.Message, "No se pudo actualizar el documento.", null, false), JsonRequestBehavior.AllowGet);
            }
        }

        private static DocumentoInventarioViewModel CreateDocumento(
            int id,
            string numero,
            string tipo,
            string proveedor,
            string facturaRef,
            DateTime fechaSolicitud,
            DateTime? fechaEntrega,
            DateTime? fechaAplicacion,
            string estado,
            string personaSolicita,
            string tipoSalida,
            string justificacionSalida,
            string justificacionEntrada,
            List<DocumentoDetalleLineaViewModel> detalleLineas)
        {
            detalleLineas = detalleLineas ?? new List<DocumentoDetalleLineaViewModel>();
            var lineasEspeciales = BuildLineasEspeciales(detalleLineas);

            return new DocumentoInventarioViewModel
            {
                ID = id,
                Numero = numero,
                Tipo = tipo,
                Proveedor = proveedor,
                Producto = BuildResumenProducto(detalleLineas, lineasEspeciales),
                FacturaRef = facturaRef,
                FechaSolicitud = fechaSolicitud,
                FechaEntrega = fechaEntrega,
                FechaAplicacion = fechaAplicacion,
                Estado = estado,
                Total = detalleLineas.Sum(x => x.TotalLinea),
                PersonaSolicita = personaSolicita,
                TipoSalida = tipoSalida,
                JustificacionSalida = justificacionSalida,
                JustificacionEntrada = justificacionEntrada,
                DetalleLineas = detalleLineas.Select(CloneDetalleLinea).ToList(),
                CantidadLineasDetalle = detalleLineas.Count,
                LineasEspeciales = lineasEspeciales,
                CantidadLineasEspeciales = lineasEspeciales.Count,
                TotalLineasEspeciales = lineasEspeciales.Sum(x => x.Cantidad * x.Costo),
                ResumenAuditoria = BuildResumenAuditoria(lineasEspeciales)
            };
        }

        private static DocumentoDetalleLineaViewModel CreateDetalle(
            string codigo,
            string descripcion,
            string unidad,
            decimal cantidad,
            decimal costoUnitario,
            decimal descuentoPorcentaje,
            decimal descuentoMonto,
            decimal impuestoPorcentaje,
            bool regalia,
            string observacion)
        {
            return new DocumentoDetalleLineaViewModel
            {
                Codigo = codigo,
                Descripcion = descripcion,
                Unidad = unidad,
                Cantidad = cantidad,
                CostoUnitario = costoUnitario,
                DescuentoPorcentaje = descuentoPorcentaje,
                DescuentoMonto = descuentoMonto,
                ImpuestoPorcentaje = impuestoPorcentaje,
                TotalLinea = CalculateTotalLinea(cantidad, costoUnitario, descuentoPorcentaje, descuentoMonto, impuestoPorcentaje),
                Regalia = regalia,
                Observacion = observacion
            };
        }

        private static bool IsMatch(string filter, string allValue, string value)
        {
            return String.IsNullOrWhiteSpace(filter) ||
                   String.Equals(filter, allValue, StringComparison.OrdinalIgnoreCase) ||
                   String.Equals(filter, value, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsTipoMatch(string filter, string value)
        {
            if (String.IsNullOrWhiteSpace(filter) || String.Equals(filter, "Todos", StringComparison.OrdinalIgnoreCase))
                return true;

            return String.Equals(filter, value, StringComparison.OrdinalIgnoreCase) ||
                   (String.Equals(filter, "Entrada", StringComparison.OrdinalIgnoreCase) && String.Equals(value, "Entrada de Inventario", StringComparison.OrdinalIgnoreCase)) ||
                   (String.Equals(filter, "Salida", StringComparison.OrdinalIgnoreCase) && String.Equals(value, "Salida de Inventario", StringComparison.OrdinalIgnoreCase));
        }

        private static bool Contains(string value, string filter)
        {
            return String.IsNullOrWhiteSpace(filter) ||
                   (value ?? "").IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static DateTime? ParseDate(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            DateTime date;
            var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy" };
            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return date;

            if (DateTime.TryParse(value, out date))
                return date;

            return null;
        }

        private static DocumentoInventarioViewModel Clone(DocumentoInventarioViewModel source)
        {
            return new DocumentoInventarioViewModel
            {
                ID = source.ID,
                Numero = source.Numero,
                Tipo = source.Tipo,
                Proveedor = source.Proveedor,
                Producto = source.Producto,
                FacturaRef = source.FacturaRef,
                FechaSolicitud = source.FechaSolicitud,
                FechaEntrega = source.FechaEntrega,
                FechaAplicacion = source.FechaAplicacion,
                Estado = source.Estado,
                Total = source.Total,
                PersonaSolicita = source.PersonaSolicita,
                TipoSalida = source.TipoSalida,
                JustificacionSalida = source.JustificacionSalida,
                JustificacionEntrada = source.JustificacionEntrada,
                DetalleLineas = (source.DetalleLineas ?? new List<DocumentoDetalleLineaViewModel>())
                    .Select(CloneDetalleLinea)
                    .ToList(),
                CantidadLineasDetalle = source.CantidadLineasDetalle,
                LineasEspeciales = (source.LineasEspeciales ?? new List<DocumentoLineaEspecialViewModel>())
                    .Select(CloneLineaEspecial)
                    .ToList(),
                CantidadLineasEspeciales = source.CantidadLineasEspeciales,
                TotalLineasEspeciales = source.TotalLineasEspeciales,
                ResumenAuditoria = source.ResumenAuditoria
            };
        }

        private static object ToGrid(DocumentoInventarioViewModel documento)
        {
            return new
            {
                documento.ID,
                documento.Numero,
                documento.Tipo,
                documento.Proveedor,
                documento.Producto,
                documento.FacturaRef,
                FechaSolicitud = FormatDate(documento.FechaSolicitud),
                FechaEntrega = FormatDate(documento.FechaEntrega),
                FechaAplicacion = FormatDate(documento.FechaAplicacion),
                documento.Estado,
                Total = documento.Total,
                documento.PersonaSolicita,
                documento.TipoSalida,
                documento.JustificacionSalida,
                documento.JustificacionEntrada,
                documento.CantidadLineasDetalle,
                documento.CantidadLineasEspeciales,
                documento.TotalLineasEspeciales,
                documento.ResumenAuditoria,
                DetalleLineas = (documento.DetalleLineas ?? new List<DocumentoDetalleLineaViewModel>())
                    .Select(x => new
                    {
                        x.Codigo,
                        x.Descripcion,
                        x.Unidad,
                        x.Cantidad,
                        x.CostoUnitario,
                        x.DescuentoPorcentaje,
                        x.DescuentoMonto,
                        x.ImpuestoPorcentaje,
                        x.TotalLinea,
                        x.Regalia,
                        x.Observacion
                    }),
                LineasEspeciales = (documento.LineasEspeciales ?? new List<DocumentoLineaEspecialViewModel>())
                    .Select(x => new
                    {
                        x.CodigoProducto,
                        x.Descripcion,
                        x.Cantidad,
                        x.Costo,
                        x.TipoLinea,
                        x.RequiereAuditoria
                    })
            };
        }

        private static DocumentoDetalleLineaViewModel CloneDetalleLinea(DocumentoDetalleLineaViewModel source)
        {
            return new DocumentoDetalleLineaViewModel
            {
                Codigo = source.Codigo,
                Descripcion = source.Descripcion,
                Unidad = source.Unidad,
                Cantidad = source.Cantidad,
                CostoUnitario = source.CostoUnitario,
                DescuentoPorcentaje = source.DescuentoPorcentaje,
                DescuentoMonto = source.DescuentoMonto,
                ImpuestoPorcentaje = source.ImpuestoPorcentaje,
                TotalLinea = source.TotalLinea,
                Regalia = source.Regalia,
                Observacion = source.Observacion
            };
        }

        private static DocumentoLineaEspecialViewModel CloneLineaEspecial(DocumentoLineaEspecialViewModel source)
        {
            return new DocumentoLineaEspecialViewModel
            {
                CodigoProducto = source.CodigoProducto,
                Descripcion = source.Descripcion,
                Cantidad = source.Cantidad,
                Costo = source.Costo,
                TipoLinea = source.TipoLinea,
                RequiereAuditoria = source.RequiereAuditoria
            };
        }

        private static List<DocumentoDetalleLineaViewModel> ParseDetalleLineas(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
                return new List<DocumentoDetalleLineaViewModel>();

            try
            {
                var lineas = JsonConvert.DeserializeObject<List<DocumentoDetalleLineaViewModel>>(json) ?? new List<DocumentoDetalleLineaViewModel>();
                return lineas
                    .Where(x => !String.IsNullOrWhiteSpace(x.Codigo) || !String.IsNullOrWhiteSpace(x.Descripcion))
                    .Select(x =>
                    {
                        var cantidad = x.Cantidad <= 0 ? 1 : x.Cantidad;
                        var costoUnitario = x.CostoUnitario < 0 ? 0 : x.CostoUnitario;
                        var descuentoPorcentaje = x.DescuentoPorcentaje < 0 ? 0 : x.DescuentoPorcentaje;
                        var descuentoMonto = x.DescuentoMonto < 0 ? 0 : x.DescuentoMonto;
                        var impuestoPorcentaje = x.ImpuestoPorcentaje < 0 ? 0 : x.ImpuestoPorcentaje;
                        var subtotal = cantidad * costoUnitario;
                        descuentoPorcentaje = Math.Min(descuentoPorcentaje, 100m);
                        if (descuentoMonto <= 0 && descuentoPorcentaje > 0)
                        {
                            descuentoMonto = subtotal * (descuentoPorcentaje / 100m);
                        }

                        descuentoMonto = Math.Min(descuentoMonto, subtotal);
                        return new DocumentoDetalleLineaViewModel
                        {
                            Codigo = (x.Codigo ?? "").Trim(),
                            Descripcion = (x.Descripcion ?? "").Trim(),
                            Unidad = String.IsNullOrWhiteSpace(x.Unidad) ? "UND" : x.Unidad.Trim(),
                            Cantidad = cantidad,
                            CostoUnitario = costoUnitario,
                            DescuentoPorcentaje = descuentoPorcentaje,
                            DescuentoMonto = descuentoMonto,
                            ImpuestoPorcentaje = impuestoPorcentaje,
                            TotalLinea = CalculateTotalLinea(cantidad, costoUnitario, descuentoPorcentaje, descuentoMonto, impuestoPorcentaje),
                            Regalia = x.Regalia,
                            Observacion = (x.Observacion ?? "").Trim()
                        };
                    })
                    .ToList();
            }
            catch
            {
                return new List<DocumentoDetalleLineaViewModel>();
            }
        }

        private static string ValidateDetalleLineas(List<DocumentoDetalleLineaViewModel> detalleLineas)
        {
            if (detalleLineas == null || detalleLineas.Count == 0)
                return "Agregue al menos una linea de producto.";

            for (var i = 0; i < detalleLineas.Count; i++)
            {
                var linea = detalleLineas[i];
                var rowLabel = "Linea " + (i + 1).ToString(CultureInfo.InvariantCulture) + ": ";

                if (String.IsNullOrWhiteSpace(linea.Codigo) || String.IsNullOrWhiteSpace(linea.Descripcion))
                    return rowLabel + "complete codigo y descripcion.";

                if (linea.Cantidad <= 0)
                    return rowLabel + "la cantidad debe ser mayor a cero.";

                if (!linea.Regalia && linea.CostoUnitario <= 0)
                    return rowLabel + "el costo unitario debe ser mayor a cero.";

                if (linea.DescuentoPorcentaje < 0 || linea.DescuentoPorcentaje > 100)
                    return rowLabel + "el descuento porcentual debe estar entre 0 y 100.";

                var subtotal = linea.Cantidad * linea.CostoUnitario;
                if (linea.DescuentoMonto < 0 || linea.DescuentoMonto > subtotal)
                    return rowLabel + "el descuento monto no puede superar el subtotal.";

                if (linea.ImpuestoPorcentaje < 0 || linea.ImpuestoPorcentaje > 100)
                    return rowLabel + "el impuesto debe estar entre 0 y 100.";

                if (linea.Regalia && String.IsNullOrWhiteSpace(linea.Observacion))
                    return rowLabel + "las regalias requieren observacion para auditoria.";
            }

            return "";
        }

        private static List<DocumentoLineaEspecialViewModel> BuildLineasEspeciales(List<DocumentoDetalleLineaViewModel> detalleLineas)
        {
            return (detalleLineas ?? new List<DocumentoDetalleLineaViewModel>())
                .Where(x => x.Regalia)
                .Select(x => new DocumentoLineaEspecialViewModel
                {
                    CodigoProducto = x.Codigo,
                    Descripcion = x.Descripcion,
                    Cantidad = x.Cantidad,
                    Costo = x.CostoUnitario,
                    TipoLinea = "Regalia",
                    RequiereAuditoria = true
                })
                .ToList();
        }

        private static string BuildResumenAuditoria(List<DocumentoLineaEspecialViewModel> lineas)
        {
            if (lineas == null || lineas.Count == 0)
                return "";

            return lineas.Count == 1
                ? "1 linea especial auditada"
                : lineas.Count.ToString(CultureInfo.InvariantCulture) + " lineas especiales auditadas";
        }

        private static string BuildResumenProducto(List<DocumentoDetalleLineaViewModel> detalleLineas, List<DocumentoLineaEspecialViewModel> lineasEspeciales)
        {
            if (detalleLineas != null && detalleLineas.Count > 0)
                return String.Join("; ", detalleLineas.Select(x => x.Codigo + " - " + x.Descripcion));

            if (lineasEspeciales != null && lineasEspeciales.Count > 0)
                return String.Join("; ", lineasEspeciales.Select(x => x.CodigoProducto + " - " + x.Descripcion));

            return "";
        }

        private static decimal CalculateTotalLinea(decimal cantidad, decimal costoUnitario, decimal descuentoPorcentaje, decimal descuentoMonto, decimal impuestoPorcentaje)
        {
            var subtotal = cantidad * costoUnitario;
            var descuentoCalculado = descuentoMonto > 0 ? descuentoMonto : subtotal * (descuentoPorcentaje / 100m);
            var baseImponible = Math.Max(0m, subtotal - descuentoCalculado);
            var impuesto = baseImponible * (impuestoPorcentaje / 100m);
            return Math.Round(baseImponible + impuesto, 2);
        }

        private static string ValidateEncabezado(DocumentoInventarioRegistroViewModel model, string accion)
        {
            if (String.IsNullOrWhiteSpace(model.TipoDocumento))
                return "Seleccione el tipo de documento.";

            if (model.FechaSolicitud == DateTime.MinValue)
                return "Ingrese una fecha de solicitud valida.";

            if (model.FechaEntrega.HasValue && model.FechaEntrega.Value.Date < model.FechaSolicitud.Date)
                return "La fecha de entrega no puede ser menor a la fecha de solicitud.";

            if (model.FechaAplicacion.HasValue && model.FechaAplicacion.Value.Date < model.FechaSolicitud.Date)
                return "La fecha de aplicacion no puede ser menor a la fecha de solicitud.";

            if (String.IsNullOrWhiteSpace(model.PersonaSolicita))
                return "Ingrese la persona que solicita el documento.";

            var proveedor = FindProveedor(model);
            if (proveedor == null)
                return "Seleccione un proveedor valido desde la busqueda.";

            model.CodigoProveedor = proveedor.Codigo;
            model.NombreProveedor = proveedor.Nombre;
            model.PlazoCredito = proveedor.PlazoCredito;

            if (String.Equals(model.TipoDocumento, "Salida de Inventario", StringComparison.OrdinalIgnoreCase) &&
                (String.IsNullOrWhiteSpace(model.TipoSalida) || String.IsNullOrWhiteSpace(model.JustificacionSalida)))
                return "Complete tipo y justificacion de salida.";

            if (String.Equals(model.TipoDocumento, "Entrada de Inventario", StringComparison.OrdinalIgnoreCase) &&
                String.IsNullOrWhiteSpace(model.JustificacionEntrada))
                return "Complete la justificacion de entrada.";

            var normalizedAction = NormalizeAccion(accion);
            if ((normalizedAction == "Enviar" || normalizedAction == "Recibir" || normalizedAction == "Cerrar") &&
                String.IsNullOrWhiteSpace(model.FacturaRef))
                return "Ingrese el numero de factura o referencia antes de avanzar el documento.";

            return "";
        }

        private static ProveedorCatalogo FindProveedor(DocumentoInventarioRegistroViewModel model)
        {
            var codigo = NormalizeLookup(model.CodigoProveedor);
            var nombre = NormalizeLookup(model.NombreProveedor);
            var busqueda = NormalizeLookup(model.ProveedorBusqueda);

            return Proveedores.FirstOrDefault(x =>
                NormalizeLookup(x.Codigo) == codigo ||
                NormalizeLookup(x.Nombre) == nombre ||
                NormalizeLookup(x.Key) == busqueda ||
                NormalizeLookup(x.Codigo) == busqueda ||
                NormalizeLookup(x.Nombre) == busqueda);
        }

        private static string NormalizeLookup(string value)
        {
            return (value ?? "").Trim().ToUpperInvariant();
        }

        private static string ApplyEstadoFromAccion(DocumentoInventarioViewModel documento, string accion)
        {
            var normalizedAction = NormalizeAccion(accion);
            var estadoActual = documento.Estado ?? "Borrador";

            if (normalizedAction == "Borrador")
            {
                if (!String.Equals(estadoActual, "Borrador", StringComparison.OrdinalIgnoreCase))
                    return "Solo los documentos en Borrador pueden guardarse como borrador.";

                documento.Estado = "Borrador";
                return "";
            }

            if (normalizedAction == "Enviar")
            {
                if (!String.Equals(estadoActual, "Borrador", StringComparison.OrdinalIgnoreCase))
                    return "Solo los documentos en Borrador pueden enviarse.";

                documento.Estado = "Enviada";
                return "";
            }

            if (normalizedAction == "Recibir")
            {
                if (!String.Equals(estadoActual, "Enviada", StringComparison.OrdinalIgnoreCase) &&
                    !String.Equals(estadoActual, "Parcial", StringComparison.OrdinalIgnoreCase))
                    return "Solo los documentos Enviados o Parciales pueden recibirse.";

                documento.Estado = "Recibida";
                documento.FechaEntrega = documento.FechaEntrega ?? DateTime.Today;
                return "";
            }

            if (normalizedAction == "Cerrar")
            {
                if (!String.Equals(estadoActual, "Recibida", StringComparison.OrdinalIgnoreCase))
                    return "Solo los documentos Recibidos pueden cerrarse.";

                documento.Estado = "Cerrada";
                documento.FechaAplicacion = documento.FechaAplicacion ?? DateTime.Today;
                return "";
            }

            return "La accion solicitada no es valida.";
        }

        private static string NormalizeAccion(string accion)
        {
            switch ((accion ?? "").Trim().ToLowerInvariant())
            {
                case "enviar":
                    return "Enviar";
                case "recibir":
                case "aplicar":
                    return "Recibir";
                case "cerrar":
                    return "Cerrar";
                case "borrador":
                case "":
                    return "Borrador";
                default:
                    return "";
            }
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd/MM/yyyy") : "";
        }

        private static string NormalizeTipoDocumento(string tipoDocumento)
        {
            if (String.Equals(tipoDocumento, "Entrada de Inventario", StringComparison.OrdinalIgnoreCase))
                return "Entrada de Inventario";

            if (String.Equals(tipoDocumento, "Salida de Inventario", StringComparison.OrdinalIgnoreCase))
                return "Salida de Inventario";

            return "Compra";
        }

        private static string GetNextNumero(string tipoDocumento)
        {
            var prefix = GetNumeroPrefix(tipoDocumento);
            var seed = GetNumeroSeed(tipoDocumento);
            var max = seed - 1;

            lock (SyncRoot)
            {
                foreach (var numero in Documentos.Select(x => x.Numero))
                {
                    if (String.IsNullOrWhiteSpace(numero) ||
                        !numero.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    int value;
                    if (Int32.TryParse(numero.Substring(prefix.Length), out value) && value > max)
                    {
                        max = value;
                    }
                }
            }

            return prefix + (max + 1).ToString("00000000", CultureInfo.InvariantCulture);
        }

        private static string GetNumeroPrefix(string tipoDocumento)
        {
            if (String.Equals(tipoDocumento, "Entrada de Inventario", StringComparison.OrdinalIgnoreCase))
                return "EN";

            if (String.Equals(tipoDocumento, "Salida de Inventario", StringComparison.OrdinalIgnoreCase))
                return "SA";

            return "PO";
        }

        private static int GetNumeroSeed(string tipoDocumento)
        {
            if (String.Equals(tipoDocumento, "Compra", StringComparison.OrdinalIgnoreCase))
                return 40337;

            return 125;
        }

        private static string ToListTipo(string tipoDocumento)
        {
            if (String.Equals(tipoDocumento, "Entrada", StringComparison.OrdinalIgnoreCase))
                return "Entrada de Inventario";

            if (String.Equals(tipoDocumento, "Salida", StringComparison.OrdinalIgnoreCase))
                return "Salida de Inventario";

            return NormalizeTipoDocumento(tipoDocumento);
        }

        private static IEnumerable<SelectListItem> GetTiposDocumento(string selected)
        {
            var tipos = new[] { "Compra", "Entrada de Inventario", "Salida de Inventario" };
            return tipos.Select(x => new SelectListItem
            {
                Text = x,
                Value = x,
                Selected = String.Equals(x, selected, StringComparison.OrdinalIgnoreCase)
            });
        }

        private static IEnumerable<SelectListItem> GetTiposSalida(string selected)
        {
            var tipos = new[] { "Est\u00e1ndar", "Proveedor" };
            return tipos.Select(x => new SelectListItem
            {
                Text = x,
                Value = x,
                Selected = String.Equals(x, selected, StringComparison.OrdinalIgnoreCase)
            });
        }

        private static IEnumerable<SelectListItem> GetJustificacionesSalida(string selected)
        {
            var opciones = new[]
            {
                "Devoluci\u00f3n a proveedor",
                "Producto da\u00f1ado",
                "Producto vencido",
                "Ajuste administrativo",
                "Uso interno",
                "Regal\u00eda / cortes\u00eda",
                "Merma",
                "Traslado no facturable"
            };

            return opciones.Select(x => new SelectListItem
            {
                Text = x,
                Value = x,
                Selected = String.Equals(x, selected, StringComparison.OrdinalIgnoreCase)
            });
        }

        private static IEnumerable<SelectListItem> GetJustificacionesEntrada(string selected)
        {
            var opciones = new[]
            {
                "Compra a proveedor",
                "Ajuste positivo",
                "Devoluci\u00f3n de cliente",
                "Inventario inicial",
                "Producto recuperado",
                "Regal\u00eda recibida",
                "Correcci\u00f3n administrativa"
            };

            return opciones.Select(x => new SelectListItem
            {
                Text = x,
                Value = x,
                Selected = String.Equals(x, selected, StringComparison.OrdinalIgnoreCase)
            });
        }

        private class ProveedorCatalogo
        {
            public ProveedorCatalogo(string codigo, string nombre, int plazoCredito)
            {
                Codigo = codigo;
                Nombre = nombre;
                PlazoCredito = plazoCredito;
            }

            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public int PlazoCredito { get; private set; }
            public string Key { get { return Codigo + " - " + Nombre; } }
        }
    }
}
