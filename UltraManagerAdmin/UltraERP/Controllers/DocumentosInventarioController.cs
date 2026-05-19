using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using UltraERP.BusinessEntities;
using UltraERP.BusinessLogic;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class DocumentosInventarioController : Controller
    {
        private static readonly object SyncRoot = new object();
        private const int CustomDocumentClientIdOffset = 1000000000;
        private static bool DocumentSqlSchemaEnsured;

        public ActionResult Inicio()
        {
            ViewBag.DocumentProcessAdmin = IsCurrentProcessAdmin();
            return View();
        }

        public ActionResult Historico()
        {
            return View();
        }

        public ActionResult Registro(int? id, string tipoDocumento)
        {
            PrepareRegistroCatalogs();
            ViewBag.DocumentProcessAdmin = IsCurrentProcessAdmin();

            if (id.HasValue)
            {
                DocumentoInventarioViewModel documento;
                if (IsCustomDocumentClientId(id.Value))
                {
                    documento = GetCustomDocumentoByClientId(id.Value);
                    if (documento != null)
                        return View(ToRegistroModel(documento));
                }

                if (IsCurrentProcessAdmin())
                {
                    documento = GetSqlDocumentoForOperation(id.Value);
                    if (documento != null)
                        return View(ToRegistroModel(documento));
                }
            }

            return View(CreateRegistroModel(tipoDocumento));
        }

        [HttpGet]
        public JsonResult BuscarProveedores(string q, int take = 10)
        {
            var data = GetProveedoresSafe(q, take)
                .Select(ToProveedorJson)
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BuscarProductos(string q, int take = 20)
        {
            var data = GetProductosSafe(q, take)
                .Select(ToProductoJson)
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GuardarEncabezado(DocumentoInventarioRegistroViewModel model, string accion)
        {
            try
            {
                if (model == null)
                    return Json(new JsonResponse("Datos invalidos.", "No se recibio el encabezado del documento.", null, false));

                var normalizedAction = NormalizeAccion(accion);
                var isReceptionSave = model.DocumentoId.HasValue && (normalizedAction == "Parcial" || normalizedAction == "Recibir");
                if (!isReceptionSave)
                {
                    var validationMessage = ValidateEncabezado(model, accion);
                    if (!String.IsNullOrWhiteSpace(validationMessage))
                    {
                        return Json(new JsonResponse("Validacion de encabezado.", validationMessage, null, false));
                    }
                }

                var detalleLineas = ParseDetalleLineas(model.DetalleLineasJson);
                var detalleValidationMessage = ValidateDetalleLineas(detalleLineas);
                if (!String.IsNullOrWhiteSpace(detalleValidationMessage))
                    return Json(new JsonResponse("Validacion de detalle.", detalleValidationMessage, null, false));

                bool isUpdate = model.DocumentoId.HasValue && model.DocumentoId.Value > 0;
                bool isCustomUpdate = isUpdate && IsCustomDocumentClientId(model.DocumentoId.Value);
                if (isUpdate && !isCustomUpdate)
                    return Json(new JsonResponse("Documento historico.", "Este documento viene del historico SQL y no se edita desde esta pantalla. Use Duplicar para crear un borrador editable.", null, false));

                int? databaseID = isCustomUpdate ? DecodeCustomDocumentClientId(model.DocumentoId.Value) : (int?)null;
                var documento = isCustomUpdate ? GetCustomDocumentoByDatabaseId(databaseID.Value) : null;
                if (isCustomUpdate && documento == null)
                    return Json(new JsonResponse("Documento no encontrado.", "No se encontro el documento que intenta editar.", null, false));

                var isRecepcionPermitida = isCustomUpdate &&
                    (String.Equals(documento.Estado, "Enviada", StringComparison.OrdinalIgnoreCase) ||
                     String.Equals(documento.Estado, "Parcial", StringComparison.OrdinalIgnoreCase)) &&
                    (normalizedAction == "Parcial" || normalizedAction == "Recibir");

                if (isCustomUpdate &&
                    !String.Equals(documento.Estado, "Borrador", StringComparison.OrdinalIgnoreCase) &&
                    !isRecepcionPermitida &&
                    !IsCurrentProcessAdmin())
                    return Json(new JsonResponse("Edicion no permitida.", "Solo Borrador permite editar encabezado y detalle. En Parcial solo se actualizan cantidades recibidas.", null, false));

                if (!isCustomUpdate)
                {
                    documento = new DocumentoInventarioViewModel
                    {
                        Numero = GetNextNumero(model.TipoDocumento),
                        Estado = "Borrador",
                        EventosAuditoria = new List<DocumentoAuditoriaEventoViewModel>()
                    };
                }

                if (isRecepcionPermitida)
                {
                    UpdateRecepcionFromRegistro(documento, detalleLineas);
                }
                else
                {
                    UpdateDocumentoFromRegistro(documento, model, detalleLineas);
                    if (!isCustomUpdate)
                        documento.PersonaSolicita = GetCurrentUserName();

                    AddAuditoria(documento, isCustomUpdate ? "Editado" : "Creado", GetCurrentUserName(), isCustomUpdate ? "Documento actualizado desde registro." : "Documento creado en SQL.");
                }

                var transitionMessage = ApplyEstadoFromAccion(documento, accion, GetCurrentUserName(), null, IsCurrentProcessAdmin());
                if (!String.IsNullOrWhiteSpace(transitionMessage))
                    return Json(new JsonResponse("Cambio de estado no permitido.", transitionMessage, null, false));

                documento = SaveCustomDocumento(documento, databaseID);

                var message = isCustomUpdate ? "Documento actualizado correctamente en SQL." : "Documento guardado correctamente en SQL.";
                return Json(new JsonResponse("", message, ToGrid(documento), true));
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
                var documentos = GetHistoricoAjustesFromDatabase(tipoDocumento, "", proveedor, personaSolicita, facturaRef, "")
                    .Where(x =>
                        IsTipoMatch(tipoDocumento, x.Tipo) &&
                        IsMatch(estado, "Todos", x.Estado) &&
                        (!desde.HasValue || x.FechaSolicitud.Date >= desde.Value.Date) &&
                        (!hasta.HasValue || x.FechaSolicitud.Date <= hasta.Value.Date) &&
                        Contains(x.Proveedor, proveedor) &&
                        Contains(x.Numero, numeroDocumento) &&
                        Contains(x.FacturaRef, facturaRef) &&
                        Contains(x.PersonaSolicita, personaSolicita))
                    .ToList();

                var customDocuments = GetCustomDocumentosFromDatabase(tipoDocumento, estado, fechaDesde, fechaHasta, "", proveedor, personaSolicita, facturaRef, "", false);
                var customNumbers = new HashSet<string>(customDocuments.Select(x => NormalizeLookup(x.Numero)), StringComparer.OrdinalIgnoreCase);
                documentos = documentos
                    .Where(x => !customNumbers.Contains(NormalizeLookup(x.Numero)))
                    .ToList();

                documentos = customDocuments
                    .Concat(documentos)
                    .OrderByDescending(x => x.FechaSolicitud)
                    .ThenByDescending(x => x.FechaAplicacion ?? DateTime.MinValue)
                    .ThenByDescending(x => x.ID)
                    .ToList();

                var result = documentos
                    .Select(x => ToGrid(x, !IsCustomDocumentClientId(x.ID)))
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
                var databaseDocuments = GetHistoricoAjustesFromDatabase(tipoDocumento, fechaAplicacion, proveedor, usuario, facturaRef, producto);
                var customDocuments = GetCustomDocumentosFromDatabase(tipoDocumento, "Cerrada", "", "", fechaAplicacion, proveedor, usuario, facturaRef, producto, true);
                var databaseNumbers = new HashSet<string>(
                    databaseDocuments.Select(x => NormalizeLookup(x.Numero)),
                    StringComparer.OrdinalIgnoreCase);

                var result = databaseDocuments
                    .Concat(customDocuments.Where(x => !databaseNumbers.Contains(NormalizeLookup(x.Numero))))
                    .OrderByDescending(x => x.FechaAplicacion ?? DateTime.MinValue)
                    .ThenByDescending(x => x.ID)
                    .Select(x => ToGrid(x))
                    .ToList();

                return Json(new JsonResponse("", "", result, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(e.Message, "No se pudo obtener el historico de ajustes.", null, false), JsonRequestBehavior.AllowGet);
            }
        }

        private List<DocumentoInventarioViewModel> GetHistoricoAjustesFromDatabase(string tipoDocumento, string fechaAplicacion, string proveedor, string usuario, string facturaRef, string producto, int? receiptID = null)
        {
            var documentos = new Dictionary<int, DocumentoInventarioViewModel>();
            DateTime? aplicacion = ParseDate(fechaAplicacion);
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];

            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return documentos.Values.ToList();

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
SELECT TOP 500
    R.ID,
    R.Number,
    R.Reference,
    R.SupplierDocNo,
    R.DatePosted,
    R.OrderDate,
    R.RequiredDate,
    R.TotalAmount,
    R.PostingComment,
    ISNULL(S.SupplierName, '') AS SupplierName,
    ISNULL(S.Code, '') AS SupplierCode,
    ISNULL(CA.Name, '') AS UserName,
    E.ID AS LineID,
    ISNULL(I.ItemLookupCode, '') AS ItemCode,
    ISNULL(E.Description, I.Description) AS ItemDescription,
    ISNULL(I.UnitOfMeasure, '') AS UnitOfMeasure,
    ISNULL(E.Quantity, 0) AS Quantity,
    ISNULL(E.QtyInvoiced, 0) AS QtyInvoiced,
    ISNULL(E.UnitCost, 0) AS UnitCost,
    ISNULL(E.LineDiscRate, 0) AS LineDiscRate,
    ISNULL(E.Comment, '') AS LineComment
FROM dbo.POD_Receipt R
LEFT JOIN dbo.Supplier S ON S.ID = R.SupplierID
LEFT JOIN dbo.Cashier CA ON CA.ID = R.UserID
LEFT JOIN dbo.POD_ReceiptEntry E ON E.ReceiptID = R.ID
LEFT JOIN dbo.Item I ON I.ID = E.EntryID
WHERE (@TipoDocumento = '' OR @TipoDocumento = 'Todos' OR @TipoDocumento = 'Compra')
  AND (@ReceiptID IS NULL OR R.ID = @ReceiptID)
  AND (@FechaAplicacion IS NULL OR CONVERT(date, R.DatePosted) = @FechaAplicacion)
  AND (@Proveedor = '' OR ISNULL(S.SupplierName, '') LIKE @ProveedorLike OR ISNULL(S.Code, '') LIKE @ProveedorLike)
  AND (@Usuario = '' OR ISNULL(CA.Name, '') LIKE @UsuarioLike OR ((ISNULL(CA.Name, '') = '' OR ISNULL(CA.Name, '') = 'SQL') AND 'Usuario no encontrado' LIKE @UsuarioLike))
  AND (@FacturaRef = '' OR ISNULL(R.Reference, '') LIKE @FacturaRefLike OR ISNULL(R.SupplierDocNo, '') LIKE @FacturaRefLike OR ISNULL(R.Number, '') LIKE @FacturaRefLike)
  AND (@Producto = '' OR ISNULL(I.ItemLookupCode, '') LIKE @ProductoLike OR ISNULL(E.Description, I.Description) LIKE @ProductoLike)
ORDER BY R.DatePosted DESC, R.ID DESC, E.LineNumber ASC;";

                command.Parameters.AddWithValue("@TipoDocumento", (tipoDocumento ?? "").Trim());
                command.Parameters.Add("@ReceiptID", SqlDbType.Int).Value = receiptID.HasValue ? (object)receiptID.Value : DBNull.Value;
                command.Parameters.Add("@FechaAplicacion", SqlDbType.Date).Value = aplicacion.HasValue ? (object)aplicacion.Value.Date : DBNull.Value;
                AddLikeParameter(command, "@Proveedor", proveedor);
                AddLikeParameter(command, "@Usuario", usuario);
                AddLikeParameter(command, "@FacturaRef", facturaRef);
                AddLikeParameter(command, "@Producto", producto);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = ReadInt(reader, "ID");
                        DocumentoInventarioViewModel documento;
                        if (!documentos.TryGetValue(id, out documento))
                        {
                            string numero = ReadString(reader, "Number");
                            string supplier = ReadString(reader, "SupplierName");
                            string supplierCode = ReadString(reader, "SupplierCode");
                            string userName = ReadString(reader, "UserName");
                            string displayUserName = GetDisplayUserName(userName);
                            DateTime postedDate = ReadDateTime(reader, "DatePosted");

                            documento = new DocumentoInventarioViewModel
                            {
                                ID = id,
                                Numero = numero,
                                Tipo = "Compra",
                                Proveedor = JoinCodeName(supplierCode, supplier),
                                FacturaRef = FirstNonEmpty(ReadString(reader, "Reference"), ReadString(reader, "SupplierDocNo"), numero),
                                FechaSolicitud = ReadDateTime(reader, "OrderDate"),
                                FechaEntrega = ReadNullableDateTime(reader, "RequiredDate"),
                                FechaAplicacion = postedDate,
                                Estado = "Cerrada",
                                Total = ReadDecimal(reader, "TotalAmount"),
                                PersonaSolicita = displayUserName,
                                JustificacionEntrada = ReadString(reader, "PostingComment"),
                                DetalleLineas = new List<DocumentoDetalleLineaViewModel>(),
                                LineasEspeciales = new List<DocumentoLineaEspecialViewModel>(),
                                EventosAuditoria = new List<DocumentoAuditoriaEventoViewModel>
                                {
                                    new DocumentoAuditoriaEventoViewModel
                                    {
                                        Evento = "Aplicado",
                                        Usuario = displayUserName,
                                        FechaHora = postedDate,
                                        Comentario = "Documento cargado desde POD_Receipt."
                                    }
                                }
                            };

                            documentos.Add(id, documento);
                        }

                        int lineId = ReadInt(reader, "LineID");
                        if (lineId > 0)
                        {
                            decimal quantity = ReadDecimal(reader, "Quantity");
                            decimal cost = ReadDecimal(reader, "UnitCost");
                            decimal discount = ReadDecimal(reader, "LineDiscRate");
                            decimal totalLine = quantity * cost * (1 - (discount / 100));
                            var line = new DocumentoDetalleLineaViewModel
                            {
                                Codigo = ReadString(reader, "ItemCode"),
                                Descripcion = ReadString(reader, "ItemDescription"),
                                Unidad = ReadString(reader, "UnitOfMeasure"),
                                Cantidad = quantity,
                                CantidadSolicitada = quantity,
                                CantidadRecibida = quantity,
                                CantidadPendiente = 0,
                                CostoUnitario = cost,
                                DescuentoPorcentaje = discount,
                                DescuentoMonto = quantity * cost * (discount / 100),
                                ImpuestoPorcentaje = 0,
                                TotalLinea = totalLine,
                                Regalia = cost <= 0,
                                Observacion = ReadString(reader, "LineComment")
                            };

                            documento.DetalleLineas.Add(line);
                        }
                    }
                }
            }

            foreach (DocumentoInventarioViewModel documento in documentos.Values)
            {
                documento.CantidadLineasDetalle = documento.DetalleLineas.Count;
                documento.LineasEspeciales = documento.DetalleLineas
                    .Where(x => x.Regalia)
                    .Select(x => new DocumentoLineaEspecialViewModel
                    {
                        CodigoProducto = x.Codigo,
                        Descripcion = x.Descripcion,
                        Cantidad = x.CantidadRecibida,
                        Costo = x.CostoUnitario,
                        TipoLinea = "Regalia",
                        RequiereAuditoria = true
                    })
                    .ToList();
                documento.CantidadLineasEspeciales = documento.LineasEspeciales.Count;
                documento.TotalLineasEspeciales = documento.LineasEspeciales.Sum(x => x.Costo * x.Cantidad);
                documento.ResumenAuditoria = documento.CantidadLineasEspeciales > 0
                    ? documento.CantidadLineasEspeciales + " lineas especiales detectadas desde SQL."
                    : "Sin lineas especiales.";
                documento.Producto = documento.DetalleLineas.Count == 1
                    ? documento.DetalleLineas[0].Descripcion
                    : documento.DetalleLineas.Count + " lineas";
            }

            return documentos.Values.ToList();
        }

        [HttpPost]
        public JsonResult Duplicar(int id, string password)
        {
            try
            {
                if (!IsProcessAuthorized(password))
                    return Json(new JsonResponse("Clave invalida.", "La clave de autorizacion no es correcta.", null, false), JsonRequestBehavior.AllowGet);

                DocumentoInventarioViewModel source = IsCustomDocumentClientId(id)
                    ? GetCustomDocumentoByClientId(id)
                    : GetSqlDocumentoForOperation(id);

                if (source == null)
                    return Json(new JsonResponse("Documento no encontrado.", "No se encontro el documento seleccionado.", null, false), JsonRequestBehavior.AllowGet);

                var copy = Clone(source);
                copy.ID = 0;
                copy.Numero = GetNextNumero(copy.Tipo);
                copy.Estado = "Borrador";
                copy.MotivoAnulacion = "";
                copy.UsuarioAnula = "";
                copy.FechaHoraAnula = null;
                copy.FacturaRef = "";
                copy.FechaSolicitud = DateTime.Today;
                copy.FechaEntrega = null;
                copy.FechaAplicacion = null;
                ResetRecepcion(copy.DetalleLineas);
                copy.LineasEspeciales = BuildLineasEspeciales(copy.DetalleLineas);
                copy.CantidadLineasDetalle = copy.DetalleLineas == null ? 0 : copy.DetalleLineas.Count;
                copy.CantidadLineasEspeciales = copy.LineasEspeciales.Count;
                copy.TotalLineasEspeciales = copy.LineasEspeciales.Sum(x => x.Cantidad * x.Costo);
                copy.ResumenAuditoria = BuildResumenAuditoria(copy.LineasEspeciales);
                copy.Producto = BuildResumenProducto(copy.DetalleLineas, copy.LineasEspeciales);
                copy.EventosAuditoria = new List<DocumentoAuditoriaEventoViewModel>();
                AddAuditoria(copy, "Creado", GetCurrentUserName(), "Documento creado por duplicado.");
                AddAuditoria(copy, "Duplicado", GetCurrentUserName(), "Origen: " + source.Numero + ".");
                copy = SaveCustomDocumento(copy, null);

                return Json(new JsonResponse("", "Documento duplicado como borrador en SQL. Revise factura y fechas antes de enviarlo.", ToGrid(copy), true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(e.Message, "No se pudo duplicar el documento.", null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ValidarClaveProceso(string password)
        {
            if (!IsProcessAuthorized(password))
                return Json(new JsonResponse("Clave invalida.", "La clave de autorizacion no es correcta.", null, false), JsonRequestBehavior.AllowGet);

            return Json(new JsonResponse("", "Clave autorizada.", null, true), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EjecutarAccion(int id, string accion, string motivo, string password)
        {
            try
            {
                if (!IsProcessAuthorized(password))
                    return Json(new JsonResponse("Clave invalida.", "La clave de autorizacion no es correcta.", null, false), JsonRequestBehavior.AllowGet);

                if (!IsCustomDocumentClientId(id))
                    return Json(new JsonResponse("Documento historico.", "Este documento viene del historico SQL y no se modifica desde la bandeja. Use Duplicar para crear un borrador editable.", null, false), JsonRequestBehavior.AllowGet);

                int databaseID = DecodeCustomDocumentClientId(id);
                var documento = GetCustomDocumentoByDatabaseId(databaseID);
                if (documento == null)
                    return Json(new JsonResponse("Documento no encontrado.", "No se encontro el documento seleccionado.", null, false), JsonRequestBehavior.AllowGet);

                var transitionMessage = ApplyEstadoFromAccion(documento, accion, GetCurrentUserName(), motivo, IsCurrentProcessAdmin());
                if (!String.IsNullOrWhiteSpace(transitionMessage))
                    return Json(new JsonResponse("Cambio de estado no permitido.", transitionMessage, null, false), JsonRequestBehavior.AllowGet);

                documento = SaveCustomDocumento(documento, databaseID);
                return Json(new JsonResponse("", "Documento actualizado correctamente en SQL.", ToGrid(documento), true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonResponse(e.Message, "No se pudo actualizar el documento.", null, false), JsonRequestBehavior.AllowGet);
            }
        }

        private DocumentoInventarioRegistroViewModel CreateRegistroModel(string tipoDocumento)
        {
            var tipo = NormalizeTipoDocumento(tipoDocumento);
            var today = DateTime.Today;

            return new DocumentoInventarioRegistroViewModel
            {
                DocumentoId = null,
                TipoDocumento = tipo,
                NumeroDocumento = GetNextNumero(tipo),
                Estado = "Borrador",
                FechaSolicitud = today,
                FechaEntrega = today.AddDays(7),
                FechaAplicacion = today,
                PersonaSolicita = GetCurrentUserName(),
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
        }

        private DocumentoInventarioRegistroViewModel ToRegistroModel(DocumentoInventarioViewModel documento)
        {
            var proveedor = FindProveedor(documento.Proveedor);
            var detalleLineas = (documento.DetalleLineas ?? new List<DocumentoDetalleLineaViewModel>())
                .Select(CloneDetalleLinea)
                .ToList();
            var tipo = NormalizeTipoDocumento(documento.Tipo);
            var tipoSalida = String.Equals(tipo, "Salida de Inventario", StringComparison.OrdinalIgnoreCase)
                ? (String.IsNullOrWhiteSpace(documento.TipoSalida) ? "Est\u00e1ndar" : documento.TipoSalida)
                : "";

            return new DocumentoInventarioRegistroViewModel
            {
                DocumentoId = documento.ID,
                TipoDocumento = tipo,
                NumeroDocumento = documento.Numero,
                Estado = documento.Estado,
                FechaSolicitud = documento.FechaSolicitud,
                FechaEntrega = documento.FechaEntrega,
                FechaAplicacion = documento.FechaAplicacion,
                ProveedorBusqueda = proveedor == null ? documento.Proveedor : proveedor.Key,
                CodigoProveedor = proveedor == null ? "" : proveedor.Codigo,
                NombreProveedor = proveedor == null ? documento.Proveedor : proveedor.Nombre,
                PlazoCredito = proveedor == null ? 0 : proveedor.PlazoCredito,
                EstadoFacturaMeCR = "Pendiente",
                FacturaRef = documento.FacturaRef,
                PersonaSolicita = documento.PersonaSolicita,
                TipoSalida = tipoSalida,
                JustificacionSalida = documento.JustificacionSalida,
                JustificacionEntrada = documento.JustificacionEntrada,
                LineasEspecialesJson = JsonConvert.SerializeObject(BuildLineasEspeciales(detalleLineas)),
                DetalleLineasJson = JsonConvert.SerializeObject(detalleLineas),
                TiposDocumento = GetTiposDocumento(tipo),
                TiposSalida = GetTiposSalida(tipoSalida),
                JustificacionesSalida = GetJustificacionesSalida(documento.JustificacionSalida),
                JustificacionesEntrada = GetJustificacionesEntrada(documento.JustificacionEntrada)
            };
        }

        private static void UpdateDocumentoFromRegistro(DocumentoInventarioViewModel documento, DocumentoInventarioRegistroViewModel model, List<DocumentoDetalleLineaViewModel> detalleLineas)
        {
            foreach (var linea in detalleLineas)
                EnsureRecepcionLinea(linea, documento.Estado);

            var lineasEspeciales = BuildLineasEspeciales(detalleLineas);

            documento.Tipo = ToListTipo(model.TipoDocumento);
            documento.Proveedor = !String.IsNullOrWhiteSpace(model.NombreProveedor) ? model.NombreProveedor : model.ProveedorBusqueda;
            documento.FacturaRef = (model.FacturaRef ?? "").Trim();
            documento.FechaSolicitud = model.FechaSolicitud == DateTime.MinValue ? DateTime.Today : model.FechaSolicitud;
            documento.FechaEntrega = model.FechaEntrega;
            documento.FechaAplicacion = model.FechaAplicacion;
            documento.Total = detalleLineas.Any() ? detalleLineas.Sum(x => x.TotalLinea) : (model.TotalFactura ?? 0m);
            documento.PersonaSolicita = (model.PersonaSolicita ?? "").Trim();
            documento.TipoSalida = String.Equals(documento.Tipo, "Salida de Inventario", StringComparison.OrdinalIgnoreCase) ? model.TipoSalida : "";
            documento.JustificacionSalida = String.Equals(documento.Tipo, "Salida de Inventario", StringComparison.OrdinalIgnoreCase) ? model.JustificacionSalida : "";
            documento.JustificacionEntrada = String.Equals(documento.Tipo, "Entrada de Inventario", StringComparison.OrdinalIgnoreCase) ? model.JustificacionEntrada : "";
            documento.DetalleLineas = detalleLineas.Select(CloneDetalleLinea).ToList();
            documento.CantidadLineasDetalle = detalleLineas.Count;
            documento.LineasEspeciales = lineasEspeciales;
            documento.CantidadLineasEspeciales = lineasEspeciales.Count;
            documento.TotalLineasEspeciales = lineasEspeciales.Sum(x => x.Cantidad * x.Costo);
            documento.ResumenAuditoria = BuildResumenAuditoria(lineasEspeciales);
            documento.Producto = BuildResumenProducto(detalleLineas, lineasEspeciales);
        }

        private static void UpdateRecepcionFromRegistro(DocumentoInventarioViewModel documento, List<DocumentoDetalleLineaViewModel> postedLineas)
        {
            var actuales = documento.DetalleLineas ?? new List<DocumentoDetalleLineaViewModel>();
            for (var i = 0; i < actuales.Count; i++)
            {
                var actual = actuales[i];
                EnsureRecepcionLinea(actual, documento.Estado);

                var posted = postedLineas.ElementAtOrDefault(i);
                if (posted == null)
                    continue;

                actual.CantidadRecibida = Clamp(posted.CantidadRecibida, 0m, actual.CantidadSolicitada);
                actual.CantidadPendiente = Math.Max(0m, actual.CantidadSolicitada - actual.CantidadRecibida);
            }

            documento.DetalleLineas = actuales;
            documento.Producto = BuildResumenProducto(actuales, documento.LineasEspeciales);
        }

        private static void ResetRecepcion(List<DocumentoDetalleLineaViewModel> lineas)
        {
            foreach (var linea in lineas ?? new List<DocumentoDetalleLineaViewModel>())
            {
                EnsureRecepcionLinea(linea, "Borrador");
                linea.CantidadRecibida = 0m;
                linea.CantidadPendiente = linea.CantidadSolicitada;
            }
        }

        private static void EnsureRecepcionLinea(DocumentoDetalleLineaViewModel linea, string estado)
        {
            if (linea == null)
                return;

            if (linea.Cantidad <= 0)
                linea.Cantidad = linea.CantidadSolicitada > 0 ? linea.CantidadSolicitada : 1m;

            if (linea.CantidadSolicitada <= 0)
                linea.CantidadSolicitada = linea.Cantidad;

            if (String.Equals(estado, "Recibida", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(estado, "Cerrada", StringComparison.OrdinalIgnoreCase))
            {
                linea.CantidadRecibida = linea.CantidadSolicitada;
            }
            else
            {
                linea.CantidadRecibida = Clamp(linea.CantidadRecibida, 0m, linea.CantidadSolicitada);
            }

            linea.CantidadPendiente = Math.Max(0m, linea.CantidadSolicitada - linea.CantidadRecibida);
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

        private static void AddLikeParameter(SqlCommand command, string name, string value)
        {
            string clean = (value ?? "").Trim();
            command.Parameters.AddWithValue(name, clean);
            command.Parameters.AddWithValue(name + "Like", "%" + clean + "%");
        }

        private static string ReadString(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? "" : Convert.ToString(reader.GetValue(ordinal));
        }

        private static int ReadInt(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static decimal ReadDecimal(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToDecimal(reader.GetValue(ordinal));
        }

        private static bool ReadBool(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return !reader.IsDBNull(ordinal) && Convert.ToBoolean(reader.GetValue(ordinal));
        }

        private static DateTime ReadDateTime(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? DateTime.Today : Convert.ToDateTime(reader.GetValue(ordinal));
        }

        private static DateTime? ReadNullableDateTime(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (DateTime?)null : Convert.ToDateTime(reader.GetValue(ordinal));
        }

        private static string FirstNonEmpty(params string[] values)
        {
            return values.FirstOrDefault(x => !String.IsNullOrWhiteSpace(x)) ?? "";
        }

        private static bool IsCustomDocumentClientId(int id)
        {
            return id >= CustomDocumentClientIdOffset;
        }

        private static int ToCustomDocumentClientId(int databaseID)
        {
            return CustomDocumentClientIdOffset + databaseID;
        }

        private static int DecodeCustomDocumentClientId(int clientID)
        {
            return clientID - CustomDocumentClientIdOffset;
        }

        private static object DbString(string value)
        {
            return String.IsNullOrWhiteSpace(value) ? (object)"" : value.Trim();
        }

        private SqlConnection CreateMasterConnection()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];
            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("No se encontro la conexion de la base de datos.");

            return new SqlConnection(settings.ConnectionString);
        }

        private void EnsureDocumentSqlSchema()
        {
            if (DocumentSqlSchemaEnsured)
                return;

            lock (SyncRoot)
            {
                if (DocumentSqlSchemaEnsured)
                    return;

                using (SqlConnection connection = CreateMasterConnection())
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
IF OBJECT_ID('dbo.ExtCentral_DocumentoInventario', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExtCentral_DocumentoInventario
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExtCentral_DocumentoInventario PRIMARY KEY,
        Numero NVARCHAR(50) NOT NULL,
        Tipo NVARCHAR(40) NOT NULL,
        ProveedorCodigo NVARCHAR(50) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_ProveedorCodigo DEFAULT(''),
        ProveedorNombre NVARCHAR(200) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_ProveedorNombre DEFAULT(''),
        FacturaRef NVARCHAR(80) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_FacturaRef DEFAULT(''),
        FechaSolicitud DATETIME NOT NULL,
        FechaEntrega DATETIME NULL,
        FechaAplicacion DATETIME NULL,
        Estado NVARCHAR(30) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_Estado DEFAULT('Borrador'),
        MotivoAnulacion NVARCHAR(500) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_Motivo DEFAULT(''),
        UsuarioAnula NVARCHAR(100) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_UsuarioAnula DEFAULT(''),
        FechaHoraAnula DATETIME NULL,
        Total DECIMAL(19,4) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_Total DEFAULT(0),
        PersonaSolicita NVARCHAR(100) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_Persona DEFAULT(''),
        TipoSalida NVARCHAR(80) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_TipoSalida DEFAULT(''),
        JustificacionSalida NVARCHAR(200) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_JustSalida DEFAULT(''),
        JustificacionEntrada NVARCHAR(200) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_JustEntrada DEFAULT(''),
        UsuarioCrea NVARCHAR(100) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_UsuarioCrea DEFAULT(''),
        FechaCrea DATETIME NOT NULL CONSTRAINT DF_ExtCentral_DocInv_FechaCrea DEFAULT(GETDATE()),
        UsuarioModifica NVARCHAR(100) NOT NULL CONSTRAINT DF_ExtCentral_DocInv_UsuarioMod DEFAULT(''),
        FechaModifica DATETIME NULL,
        SyncGuid UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_ExtCentral_DocInv_SyncGuid DEFAULT(NEWID())
    );
END;

IF OBJECT_ID('dbo.ExtCentral_DocumentoInventarioDetalle', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExtCentral_DocumentoInventarioDetalle
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExtCentral_DocInvDetalle PRIMARY KEY,
        DocumentoID INT NOT NULL,
        LineNumber INT NOT NULL,
        Codigo NVARCHAR(50) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_Codigo DEFAULT(''),
        Descripcion NVARCHAR(200) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_Descripcion DEFAULT(''),
        Unidad NVARCHAR(20) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_Unidad DEFAULT(''),
        Cantidad DECIMAL(19,4) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_Cantidad DEFAULT(0),
        CantidadSolicitada DECIMAL(19,4) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_CantSol DEFAULT(0),
        CantidadRecibida DECIMAL(19,4) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_CantRec DEFAULT(0),
        CantidadPendiente DECIMAL(19,4) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_CantPen DEFAULT(0),
        CostoUnitario DECIMAL(19,4) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_Costo DEFAULT(0),
        DescuentoPorcentaje DECIMAL(9,4) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_DescPct DEFAULT(0),
        DescuentoMonto DECIMAL(19,4) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_DescMonto DEFAULT(0),
        ImpuestoPorcentaje DECIMAL(9,4) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_ImpPct DEFAULT(0),
        TotalLinea DECIMAL(19,4) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_Total DEFAULT(0),
        Regalia BIT NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_Regalia DEFAULT(0),
        Observacion NVARCHAR(500) NOT NULL CONSTRAINT DF_ExtCentral_DocInvDet_Obs DEFAULT(''),
        CONSTRAINT FK_ExtCentral_DocInvDetalle_Documento FOREIGN KEY (DocumentoID) REFERENCES dbo.ExtCentral_DocumentoInventario(ID) ON DELETE CASCADE
    );
END;

IF OBJECT_ID('dbo.ExtCentral_DocumentoInventarioAuditoria', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExtCentral_DocumentoInventarioAuditoria
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExtCentral_DocInvAuditoria PRIMARY KEY,
        DocumentoID INT NOT NULL,
        Evento NVARCHAR(50) NOT NULL CONSTRAINT DF_ExtCentral_DocInvAud_Evento DEFAULT(''),
        Usuario NVARCHAR(100) NOT NULL CONSTRAINT DF_ExtCentral_DocInvAud_Usuario DEFAULT(''),
        FechaHora DATETIME NOT NULL CONSTRAINT DF_ExtCentral_DocInvAud_Fecha DEFAULT(GETDATE()),
        Comentario NVARCHAR(500) NOT NULL CONSTRAINT DF_ExtCentral_DocInvAud_Comentario DEFAULT(''),
        CONSTRAINT FK_ExtCentral_DocInvAuditoria_Documento FOREIGN KEY (DocumentoID) REFERENCES dbo.ExtCentral_DocumentoInventario(ID) ON DELETE CASCADE
    );
END;";

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                DocumentSqlSchemaEnsured = true;
            }
        }

        private DocumentoInventarioViewModel GetCustomDocumentoByClientId(int clientID)
        {
            return IsCustomDocumentClientId(clientID)
                ? GetCustomDocumentoByDatabaseId(DecodeCustomDocumentClientId(clientID))
                : null;
        }

        private DocumentoInventarioViewModel GetCustomDocumentoByDatabaseId(int databaseID)
        {
            return GetCustomDocumentosFromDatabase("", "Todos", "", "", "", "", "", "", "", false, databaseID).FirstOrDefault();
        }

        private List<DocumentoInventarioViewModel> GetCustomDocumentosFromDatabase(
            string tipoDocumento,
            string estado,
            string fechaDesde,
            string fechaHasta,
            string fechaAplicacion,
            string proveedor,
            string usuario,
            string facturaRef,
            string producto,
            bool cerradasOnly,
            int? databaseID = null)
        {
            EnsureDocumentSqlSchema();

            var documentos = new Dictionary<int, DocumentoInventarioViewModel>();
            DateTime? desde = ParseDate(fechaDesde);
            DateTime? hasta = ParseDate(fechaHasta);
            DateTime? aplicacion = ParseDate(fechaAplicacion);

            using (SqlConnection connection = CreateMasterConnection())
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
DECLARE @Selected TABLE (ID INT PRIMARY KEY);

INSERT INTO @Selected (ID)
SELECT TOP 500 D.ID
FROM dbo.ExtCentral_DocumentoInventario D
WHERE (@DocumentoID IS NULL OR D.ID = @DocumentoID)
  AND (@TipoDocumento = '' OR @TipoDocumento = 'Todos' OR D.Tipo = @TipoDocumento
       OR (@TipoDocumento = 'Entrada' AND D.Tipo = 'Entrada de Inventario')
       OR (@TipoDocumento = 'Salida' AND D.Tipo = 'Salida de Inventario'))
  AND (@Estado = '' OR @Estado = 'Todos' OR D.Estado = @Estado)
  AND (@CerradasOnly = 0 OR D.Estado = 'Cerrada')
  AND (@FechaDesde IS NULL OR CONVERT(date, D.FechaSolicitud) >= @FechaDesde)
  AND (@FechaHasta IS NULL OR CONVERT(date, D.FechaSolicitud) <= @FechaHasta)
  AND (@FechaAplicacion IS NULL OR CONVERT(date, D.FechaAplicacion) = @FechaAplicacion)
  AND (@Proveedor = '' OR D.ProveedorCodigo LIKE @ProveedorLike OR D.ProveedorNombre LIKE @ProveedorLike)
  AND (@Usuario = '' OR D.PersonaSolicita LIKE @UsuarioLike)
  AND (@FacturaRef = '' OR D.FacturaRef LIKE @FacturaRefLike OR D.Numero LIKE @FacturaRefLike)
  AND (@Producto = '' OR EXISTS (
        SELECT 1
        FROM dbo.ExtCentral_DocumentoInventarioDetalle L
        WHERE L.DocumentoID = D.ID
          AND (L.Codigo LIKE @ProductoLike OR L.Descripcion LIKE @ProductoLike)))
ORDER BY D.FechaSolicitud DESC, ISNULL(D.FechaAplicacion, D.FechaSolicitud) DESC, D.ID DESC;

SELECT D.*
FROM dbo.ExtCentral_DocumentoInventario D
JOIN @Selected S ON S.ID = D.ID
ORDER BY D.FechaSolicitud DESC, D.ID DESC;

SELECT L.*
FROM dbo.ExtCentral_DocumentoInventarioDetalle L
JOIN @Selected S ON S.ID = L.DocumentoID
ORDER BY L.DocumentoID, L.LineNumber;

SELECT A.*
FROM dbo.ExtCentral_DocumentoInventarioAuditoria A
JOIN @Selected S ON S.ID = A.DocumentoID
ORDER BY A.DocumentoID, A.FechaHora, A.ID;";

                command.Parameters.Add("@DocumentoID", SqlDbType.Int).Value = databaseID.HasValue ? (object)databaseID.Value : DBNull.Value;
                command.Parameters.AddWithValue("@TipoDocumento", (tipoDocumento ?? "").Trim());
                command.Parameters.AddWithValue("@Estado", (estado ?? "").Trim());
                command.Parameters.Add("@CerradasOnly", SqlDbType.Bit).Value = cerradasOnly;
                command.Parameters.Add("@FechaDesde", SqlDbType.Date).Value = desde.HasValue ? (object)desde.Value.Date : DBNull.Value;
                command.Parameters.Add("@FechaHasta", SqlDbType.Date).Value = hasta.HasValue ? (object)hasta.Value.Date : DBNull.Value;
                command.Parameters.Add("@FechaAplicacion", SqlDbType.Date).Value = aplicacion.HasValue ? (object)aplicacion.Value.Date : DBNull.Value;
                AddLikeParameter(command, "@Proveedor", proveedor);
                AddLikeParameter(command, "@Usuario", usuario);
                AddLikeParameter(command, "@FacturaRef", facturaRef);
                AddLikeParameter(command, "@Producto", producto);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = ReadInt(reader, "ID");
                        documentos[id] = new DocumentoInventarioViewModel
                        {
                            ID = ToCustomDocumentClientId(id),
                            Numero = ReadString(reader, "Numero"),
                            Tipo = ReadString(reader, "Tipo"),
                            Proveedor = JoinCodeName(ReadString(reader, "ProveedorCodigo"), ReadString(reader, "ProveedorNombre")),
                            FacturaRef = ReadString(reader, "FacturaRef"),
                            FechaSolicitud = ReadDateTime(reader, "FechaSolicitud"),
                            FechaEntrega = ReadNullableDateTime(reader, "FechaEntrega"),
                            FechaAplicacion = ReadNullableDateTime(reader, "FechaAplicacion"),
                            Estado = ReadString(reader, "Estado"),
                            MotivoAnulacion = ReadString(reader, "MotivoAnulacion"),
                            UsuarioAnula = ReadString(reader, "UsuarioAnula"),
                            FechaHoraAnula = ReadNullableDateTime(reader, "FechaHoraAnula"),
                            Total = ReadDecimal(reader, "Total"),
                            PersonaSolicita = ReadString(reader, "PersonaSolicita"),
                            TipoSalida = ReadString(reader, "TipoSalida"),
                            JustificacionSalida = ReadString(reader, "JustificacionSalida"),
                            JustificacionEntrada = ReadString(reader, "JustificacionEntrada"),
                            DetalleLineas = new List<DocumentoDetalleLineaViewModel>(),
                            LineasEspeciales = new List<DocumentoLineaEspecialViewModel>(),
                            EventosAuditoria = new List<DocumentoAuditoriaEventoViewModel>()
                        };
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            int documentoID = ReadInt(reader, "DocumentoID");
                            DocumentoInventarioViewModel documento;
                            if (!documentos.TryGetValue(documentoID, out documento))
                                continue;

                            documento.DetalleLineas.Add(new DocumentoDetalleLineaViewModel
                            {
                                Codigo = ReadString(reader, "Codigo"),
                                Descripcion = ReadString(reader, "Descripcion"),
                                Unidad = ReadString(reader, "Unidad"),
                                Cantidad = ReadDecimal(reader, "Cantidad"),
                                CantidadSolicitada = ReadDecimal(reader, "CantidadSolicitada"),
                                CantidadRecibida = ReadDecimal(reader, "CantidadRecibida"),
                                CantidadPendiente = ReadDecimal(reader, "CantidadPendiente"),
                                CostoUnitario = ReadDecimal(reader, "CostoUnitario"),
                                DescuentoPorcentaje = ReadDecimal(reader, "DescuentoPorcentaje"),
                                DescuentoMonto = ReadDecimal(reader, "DescuentoMonto"),
                                ImpuestoPorcentaje = ReadDecimal(reader, "ImpuestoPorcentaje"),
                                TotalLinea = ReadDecimal(reader, "TotalLinea"),
                                Regalia = ReadBool(reader, "Regalia"),
                                Observacion = ReadString(reader, "Observacion")
                            });
                        }
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            int documentoID = ReadInt(reader, "DocumentoID");
                            DocumentoInventarioViewModel documento;
                            if (!documentos.TryGetValue(documentoID, out documento))
                                continue;

                            documento.EventosAuditoria.Add(CreateAuditoriaEvento(
                                ReadString(reader, "Evento"),
                                ReadString(reader, "Usuario"),
                                ReadDateTime(reader, "FechaHora"),
                                ReadString(reader, "Comentario")));
                        }
                    }
                }
            }

            foreach (DocumentoInventarioViewModel documento in documentos.Values)
                FinalizeDocumento(documento);

            return documentos.Values.ToList();
        }

        private DocumentoInventarioViewModel SaveCustomDocumento(DocumentoInventarioViewModel documento, int? databaseID)
        {
            if (documento == null)
                throw new ArgumentNullException("documento");

            EnsureDocumentSqlSchema();
            FinalizeDocumento(documento);

            if (!databaseID.HasValue)
                documento.Numero = GetNextNumero(documento.Tipo);

            using (SqlConnection connection = CreateMasterConnection())
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int savedID = databaseID.HasValue
                            ? UpdateCustomDocumento(connection, transaction, documento, databaseID.Value)
                            : InsertCustomDocumento(connection, transaction, documento);

                        ReplaceCustomDocumentoDetails(connection, transaction, savedID, documento.DetalleLineas);
                        ReplaceCustomDocumentoAudit(connection, transaction, savedID, documento.EventosAuditoria);

                        transaction.Commit();
                        documento.ID = ToCustomDocumentClientId(savedID);
                        return documento;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private int InsertCustomDocumento(SqlConnection connection, SqlTransaction transaction, DocumentoInventarioViewModel documento)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                command.CommandText = @"
INSERT INTO dbo.ExtCentral_DocumentoInventario
(
    Numero, Tipo, ProveedorCodigo, ProveedorNombre, FacturaRef, FechaSolicitud, FechaEntrega,
    FechaAplicacion, Estado, MotivoAnulacion, UsuarioAnula, FechaHoraAnula, Total,
    PersonaSolicita, TipoSalida, JustificacionSalida, JustificacionEntrada, UsuarioCrea
)
OUTPUT inserted.ID
VALUES
(
    @Numero, @Tipo, @ProveedorCodigo, @ProveedorNombre, @FacturaRef, @FechaSolicitud, @FechaEntrega,
    @FechaAplicacion, @Estado, @MotivoAnulacion, @UsuarioAnula, @FechaHoraAnula, @Total,
    @PersonaSolicita, @TipoSalida, @JustificacionSalida, @JustificacionEntrada, @Usuario
);";

                AddCustomDocumentoParameters(command, documento);
                command.Parameters.AddWithValue("@Usuario", GetCurrentUserName());
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private int UpdateCustomDocumento(SqlConnection connection, SqlTransaction transaction, DocumentoInventarioViewModel documento, int databaseID)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                command.CommandText = @"
UPDATE dbo.ExtCentral_DocumentoInventario
SET Tipo = @Tipo,
    ProveedorCodigo = @ProveedorCodigo,
    ProveedorNombre = @ProveedorNombre,
    FacturaRef = @FacturaRef,
    FechaSolicitud = @FechaSolicitud,
    FechaEntrega = @FechaEntrega,
    FechaAplicacion = @FechaAplicacion,
    Estado = @Estado,
    MotivoAnulacion = @MotivoAnulacion,
    UsuarioAnula = @UsuarioAnula,
    FechaHoraAnula = @FechaHoraAnula,
    Total = @Total,
    PersonaSolicita = @PersonaSolicita,
    TipoSalida = @TipoSalida,
    JustificacionSalida = @JustificacionSalida,
    JustificacionEntrada = @JustificacionEntrada,
    UsuarioModifica = @Usuario,
    FechaModifica = GETDATE()
WHERE ID = @ID;";

                AddCustomDocumentoParameters(command, documento);
                command.Parameters.AddWithValue("@Usuario", GetCurrentUserName());
                command.Parameters.AddWithValue("@ID", databaseID);
                command.ExecuteNonQuery();
                return databaseID;
            }
        }

        private static void AddCustomDocumentoParameters(SqlCommand command, DocumentoInventarioViewModel documento)
        {
            string proveedorCodigo;
            string proveedorNombre;
            SplitCodeName(documento.Proveedor, out proveedorCodigo, out proveedorNombre);

            command.Parameters.AddWithValue("@Numero", DbString(documento.Numero));
            command.Parameters.AddWithValue("@Tipo", DbString(ToListTipo(documento.Tipo)));
            command.Parameters.AddWithValue("@ProveedorCodigo", DbString(proveedorCodigo));
            command.Parameters.AddWithValue("@ProveedorNombre", DbString(proveedorNombre));
            command.Parameters.AddWithValue("@FacturaRef", DbString(documento.FacturaRef));
            command.Parameters.Add("@FechaSolicitud", SqlDbType.DateTime).Value = documento.FechaSolicitud == DateTime.MinValue ? DateTime.Today : documento.FechaSolicitud;
            command.Parameters.Add("@FechaEntrega", SqlDbType.DateTime).Value = documento.FechaEntrega.HasValue ? (object)documento.FechaEntrega.Value : DBNull.Value;
            command.Parameters.Add("@FechaAplicacion", SqlDbType.DateTime).Value = documento.FechaAplicacion.HasValue ? (object)documento.FechaAplicacion.Value : DBNull.Value;
            command.Parameters.AddWithValue("@Estado", DbString(documento.Estado));
            command.Parameters.AddWithValue("@MotivoAnulacion", DbString(documento.MotivoAnulacion));
            command.Parameters.AddWithValue("@UsuarioAnula", DbString(documento.UsuarioAnula));
            command.Parameters.Add("@FechaHoraAnula", SqlDbType.DateTime).Value = documento.FechaHoraAnula.HasValue ? (object)documento.FechaHoraAnula.Value : DBNull.Value;
            command.Parameters.Add("@Total", SqlDbType.Decimal).Value = documento.Total;
            command.Parameters["@Total"].Precision = 19;
            command.Parameters["@Total"].Scale = 4;
            command.Parameters.AddWithValue("@PersonaSolicita", DbString(documento.PersonaSolicita));
            command.Parameters.AddWithValue("@TipoSalida", DbString(documento.TipoSalida));
            command.Parameters.AddWithValue("@JustificacionSalida", DbString(documento.JustificacionSalida));
            command.Parameters.AddWithValue("@JustificacionEntrada", DbString(documento.JustificacionEntrada));
        }

        private static void ReplaceCustomDocumentoDetails(SqlConnection connection, SqlTransaction transaction, int documentoID, IList<DocumentoDetalleLineaViewModel> lineas)
        {
            using (SqlCommand delete = connection.CreateCommand())
            {
                delete.Transaction = transaction;
                delete.CommandText = "DELETE FROM dbo.ExtCentral_DocumentoInventarioDetalle WHERE DocumentoID = @DocumentoID";
                delete.Parameters.AddWithValue("@DocumentoID", documentoID);
                delete.ExecuteNonQuery();
            }

            int lineNumber = 1;
            foreach (DocumentoDetalleLineaViewModel linea in lineas ?? new List<DocumentoDetalleLineaViewModel>())
            {
                using (SqlCommand insert = connection.CreateCommand())
                {
                    insert.Transaction = transaction;
                    insert.CommandType = CommandType.Text;
                    insert.CommandText = @"
INSERT INTO dbo.ExtCentral_DocumentoInventarioDetalle
(DocumentoID, LineNumber, Codigo, Descripcion, Unidad, Cantidad, CantidadSolicitada, CantidadRecibida,
 CantidadPendiente, CostoUnitario, DescuentoPorcentaje, DescuentoMonto, ImpuestoPorcentaje,
 TotalLinea, Regalia, Observacion)
VALUES
(@DocumentoID, @LineNumber, @Codigo, @Descripcion, @Unidad, @Cantidad, @CantidadSolicitada, @CantidadRecibida,
 @CantidadPendiente, @CostoUnitario, @DescuentoPorcentaje, @DescuentoMonto, @ImpuestoPorcentaje,
 @TotalLinea, @Regalia, @Observacion);";

                    insert.Parameters.AddWithValue("@DocumentoID", documentoID);
                    insert.Parameters.AddWithValue("@LineNumber", lineNumber++);
                    insert.Parameters.AddWithValue("@Codigo", DbString(linea.Codigo));
                    insert.Parameters.AddWithValue("@Descripcion", DbString(linea.Descripcion));
                    insert.Parameters.AddWithValue("@Unidad", DbString(linea.Unidad));
                    AddDecimalParameter(insert, "@Cantidad", linea.Cantidad);
                    AddDecimalParameter(insert, "@CantidadSolicitada", linea.CantidadSolicitada);
                    AddDecimalParameter(insert, "@CantidadRecibida", linea.CantidadRecibida);
                    AddDecimalParameter(insert, "@CantidadPendiente", linea.CantidadPendiente);
                    AddDecimalParameter(insert, "@CostoUnitario", linea.CostoUnitario);
                    AddDecimalParameter(insert, "@DescuentoPorcentaje", linea.DescuentoPorcentaje, 9);
                    AddDecimalParameter(insert, "@DescuentoMonto", linea.DescuentoMonto);
                    AddDecimalParameter(insert, "@ImpuestoPorcentaje", linea.ImpuestoPorcentaje, 9);
                    AddDecimalParameter(insert, "@TotalLinea", linea.TotalLinea);
                    insert.Parameters.Add("@Regalia", SqlDbType.Bit).Value = linea.Regalia;
                    insert.Parameters.AddWithValue("@Observacion", DbString(linea.Observacion));
                    insert.ExecuteNonQuery();
                }
            }
        }

        private static void ReplaceCustomDocumentoAudit(SqlConnection connection, SqlTransaction transaction, int documentoID, IList<DocumentoAuditoriaEventoViewModel> eventos)
        {
            using (SqlCommand delete = connection.CreateCommand())
            {
                delete.Transaction = transaction;
                delete.CommandText = "DELETE FROM dbo.ExtCentral_DocumentoInventarioAuditoria WHERE DocumentoID = @DocumentoID";
                delete.Parameters.AddWithValue("@DocumentoID", documentoID);
                delete.ExecuteNonQuery();
            }

            foreach (DocumentoAuditoriaEventoViewModel evento in eventos ?? new List<DocumentoAuditoriaEventoViewModel>())
            {
                using (SqlCommand insert = connection.CreateCommand())
                {
                    insert.Transaction = transaction;
                    insert.CommandType = CommandType.Text;
                    insert.CommandText = @"
INSERT INTO dbo.ExtCentral_DocumentoInventarioAuditoria
(DocumentoID, Evento, Usuario, FechaHora, Comentario)
VALUES (@DocumentoID, @Evento, @Usuario, @FechaHora, @Comentario);";

                    insert.Parameters.AddWithValue("@DocumentoID", documentoID);
                    insert.Parameters.AddWithValue("@Evento", DbString(evento.Evento));
                    insert.Parameters.AddWithValue("@Usuario", DbString(evento.Usuario));
                    insert.Parameters.Add("@FechaHora", SqlDbType.DateTime).Value = evento.FechaHora == DateTime.MinValue ? DateTime.Now : evento.FechaHora;
                    insert.Parameters.AddWithValue("@Comentario", DbString(evento.Comentario));
                    insert.ExecuteNonQuery();
                }
            }
        }

        private static void AddDecimalParameter(SqlCommand command, string name, decimal value, byte precision = 19)
        {
            SqlParameter parameter = command.Parameters.Add(name, SqlDbType.Decimal);
            parameter.Precision = precision;
            parameter.Scale = 4;
            parameter.Value = value;
        }

        private static void FinalizeDocumento(DocumentoInventarioViewModel documento)
        {
            if (documento == null)
                return;

            documento.DetalleLineas = documento.DetalleLineas ?? new List<DocumentoDetalleLineaViewModel>();
            foreach (DocumentoDetalleLineaViewModel linea in documento.DetalleLineas)
            {
                EnsureRecepcionLinea(linea, documento.Estado);
                linea.TotalLinea = CalculateTotalLinea(linea.Cantidad, linea.CostoUnitario, linea.DescuentoPorcentaje, linea.DescuentoMonto, linea.ImpuestoPorcentaje);
            }

            documento.LineasEspeciales = BuildLineasEspeciales(documento.DetalleLineas);
            documento.CantidadLineasDetalle = documento.DetalleLineas.Count;
            documento.CantidadLineasEspeciales = documento.LineasEspeciales.Count;
            documento.TotalLineasEspeciales = documento.LineasEspeciales.Sum(x => x.Cantidad * x.Costo);
            documento.ResumenAuditoria = BuildResumenAuditoria(documento.LineasEspeciales);
            documento.Producto = BuildResumenProducto(documento.DetalleLineas, documento.LineasEspeciales);
            documento.Total = documento.DetalleLineas.Sum(x => x.TotalLinea);
            documento.EventosAuditoria = documento.EventosAuditoria ?? new List<DocumentoAuditoriaEventoViewModel>();
        }

        private static void SplitCodeName(string value, out string code, out string name)
        {
            code = "";
            name = value ?? "";

            var parts = (value ?? "").Split(new[] { " - " }, 2, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                code = parts[0].Trim();
                name = parts[1].Trim();
            }
        }

        private static string JoinCodeName(string code, string name)
        {
            if (String.IsNullOrWhiteSpace(code))
                return name ?? "";

            if (String.IsNullOrWhiteSpace(name))
                return code;

            return code + " - " + name;
        }

        private void PrepareRegistroCatalogs()
        {
            ViewBag.ProveedoresJson = JsonConvert.SerializeObject(
                GetProveedoresSafe("", 100).Select(ToProveedorJson).ToList());
            ViewBag.ProductosJson = JsonConvert.SerializeObject(
                GetProductosSafe("", 120).Select(ToProductoJson).ToList());
        }

        private List<ProveedorCatalogo> GetProveedoresSafe(string searchValue = "", int take = 100)
        {
            try
            {
                int limit = take <= 0 ? 100 : take;
                return new CT_Supplier()
                    .GetAll("%", searchValue ?? "", 0, limit)
                    .Where(x => x != null)
                    .Select(x => new ProveedorCatalogo(x.Code, x.Name, 0))
                    .Where(x => !String.IsNullOrWhiteSpace(x.Codigo) || !String.IsNullOrWhiteSpace(x.Nombre))
                    .OrderBy(x => x.Nombre)
                    .ThenBy(x => x.Codigo)
                    .ToList();
            }
            catch
            {
                return new List<ProveedorCatalogo>();
            }
        }

        private List<ProductoCatalogo> GetProductosSafe(string searchValue = "", int take = 120)
        {
            try
            {
                int limit = take <= 0 ? 120 : take;
                return new CT_Item()
                    .GetDynamicList(searchValue ?? "", 1, "asc", 0, limit, null, null, null, null, null, "%")
                    .Where(x => x != null)
                    .Select(MapProducto)
                    .Where(x => !String.IsNullOrWhiteSpace(x.Codigo) || !String.IsNullOrWhiteSpace(x.Descripcion))
                    .OrderBy(x => x.Codigo)
                    .ToList();
            }
            catch
            {
                return new List<ProductoCatalogo>();
            }
        }

        private static ProductoCatalogo MapProducto(EN_Item item)
        {
            decimal cost = item.ReplacementCost > 0 ? item.ReplacementCost : item.Cost;

            return new ProductoCatalogo
            {
                Codigo = item.ItemLookupCode,
                Descripcion = item.Description,
                Unidad = String.IsNullOrWhiteSpace(item.UnitOfMeasure) ? "UND" : item.UnitOfMeasure,
                Costo = cost,
                Impuesto = Convert.ToDecimal(item.TaxPercentage)
            };
        }

        private static object ToProveedorJson(ProveedorCatalogo proveedor)
        {
            return new
            {
                key = proveedor.Key,
                codigo = proveedor.Codigo,
                nombre = proveedor.Nombre,
                plazo = proveedor.PlazoCredito
            };
        }

        private static object ToProductoJson(ProductoCatalogo producto)
        {
            return new
            {
                codigo = producto.Codigo,
                descripcion = producto.Descripcion,
                unidad = producto.Unidad,
                costo = producto.Costo,
                impuesto = producto.Impuesto
            };
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
                MotivoAnulacion = source.MotivoAnulacion,
                UsuarioAnula = source.UsuarioAnula,
                FechaHoraAnula = source.FechaHoraAnula,
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
                ResumenAuditoria = source.ResumenAuditoria,
                EventosAuditoria = (source.EventosAuditoria ?? new List<DocumentoAuditoriaEventoViewModel>())
                    .Select(CloneAuditoriaEvento)
                    .ToList()
            };
        }

        private static object ToGrid(DocumentoInventarioViewModel documento, bool soloLecturaSql = false)
        {
            return new
            {
                documento.ID,
                SoloLecturaSql = soloLecturaSql,
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
                documento.MotivoAnulacion,
                documento.UsuarioAnula,
                FechaHoraAnula = FormatDateTime(documento.FechaHoraAnula),
                DetalleLineas = (documento.DetalleLineas ?? new List<DocumentoDetalleLineaViewModel>())
                    .Select(x => new
                    {
                        x.Codigo,
                        x.Descripcion,
                        x.Unidad,
                        x.Cantidad,
                        x.CantidadSolicitada,
                        x.CantidadRecibida,
                        x.CantidadPendiente,
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
                    }),
                EventosAuditoria = (documento.EventosAuditoria ?? new List<DocumentoAuditoriaEventoViewModel>())
                    .OrderBy(x => x.FechaHora)
                    .Select(x => new
                    {
                        x.Evento,
                        x.Usuario,
                        FechaHora = FormatDateTime(x.FechaHora),
                        x.Comentario
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
                CantidadSolicitada = source.CantidadSolicitada,
                CantidadRecibida = source.CantidadRecibida,
                CantidadPendiente = source.CantidadPendiente,
                CostoUnitario = source.CostoUnitario,
                DescuentoPorcentaje = source.DescuentoPorcentaje,
                DescuentoMonto = source.DescuentoMonto,
                ImpuestoPorcentaje = source.ImpuestoPorcentaje,
                TotalLinea = source.TotalLinea,
                Regalia = source.Regalia,
                Observacion = source.Observacion
            };
        }

        private static DocumentoAuditoriaEventoViewModel CloneAuditoriaEvento(DocumentoAuditoriaEventoViewModel source)
        {
            return new DocumentoAuditoriaEventoViewModel
            {
                Evento = source.Evento,
                Usuario = source.Usuario,
                FechaHora = source.FechaHora,
                Comentario = source.Comentario
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
                        var cantidadSolicitada = x.CantidadSolicitada <= 0 ? cantidad : x.CantidadSolicitada;
                        var cantidadRecibida = Clamp(x.CantidadRecibida, 0m, cantidadSolicitada);

                        return new DocumentoDetalleLineaViewModel
                        {
                            Codigo = (x.Codigo ?? "").Trim(),
                            Descripcion = (x.Descripcion ?? "").Trim(),
                            Unidad = String.IsNullOrWhiteSpace(x.Unidad) ? "UND" : x.Unidad.Trim(),
                            Cantidad = cantidad,
                            CantidadSolicitada = cantidadSolicitada,
                            CantidadRecibida = cantidadRecibida,
                            CantidadPendiente = Math.Max(0m, cantidadSolicitada - cantidadRecibida),
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

                if (linea.CantidadSolicitada <= 0)
                    return rowLabel + "la cantidad solicitada debe ser mayor a cero.";

                if (linea.CantidadRecibida < 0 || linea.CantidadRecibida > linea.CantidadSolicitada)
                    return rowLabel + "la cantidad recibida debe estar entre 0 y la cantidad solicitada.";

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
                    return rowLabel + "las regal\u00edas requieren observaci\u00f3n para auditor\u00eda.";
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
                ? "1 l\u00ednea especial auditada"
                : lineas.Count.ToString(CultureInfo.InvariantCulture) + " l\u00edneas especiales auditadas";
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

        private string ValidateEncabezado(DocumentoInventarioRegistroViewModel model, string accion)
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
            if ((normalizedAction == "Enviar" || normalizedAction == "Parcial" || normalizedAction == "Recibir" || normalizedAction == "Cerrar") &&
                String.IsNullOrWhiteSpace(model.FacturaRef))
                return "Ingrese el numero de factura o referencia antes de avanzar el documento.";

            return "";
        }

        private ProveedorCatalogo FindProveedor(DocumentoInventarioRegistroViewModel model)
        {
            if (model == null)
                return null;

            var codigoOriginal = (model.CodigoProveedor ?? "").Trim();
            var nombreOriginal = (model.NombreProveedor ?? "").Trim();
            var busquedaOriginal = (model.ProveedorBusqueda ?? "").Trim();
            var codigo = NormalizeLookup(codigoOriginal);
            var nombre = NormalizeLookup(nombreOriginal);
            var busqueda = NormalizeLookup(busquedaOriginal);
            var candidates = new List<ProveedorCatalogo>();

            AddProveedorCandidates(candidates, GetProveedoresSafe(codigoOriginal, 50));
            AddProveedorCandidates(candidates, GetProveedoresSafe(nombreOriginal, 50));
            AddProveedorCandidates(candidates, GetProveedoresSafe(busquedaOriginal, 50));

            var proveedor = candidates.FirstOrDefault(x =>
                NormalizeLookup(x.Codigo) == codigo ||
                NormalizeLookup(x.Nombre) == nombre ||
                NormalizeLookup(x.Key) == busqueda ||
                NormalizeLookup(x.Codigo) == busqueda ||
                NormalizeLookup(x.Nombre) == busqueda);

            if (proveedor != null)
                return proveedor;

            if (!String.IsNullOrWhiteSpace(codigoOriginal) && !String.IsNullOrWhiteSpace(nombreOriginal))
                return new ProveedorCatalogo(codigoOriginal, nombreOriginal, model.PlazoCredito);

            return null;
        }

        private ProveedorCatalogo FindProveedor(string value)
        {
            var lookup = NormalizeLookup(value);
            if (String.IsNullOrWhiteSpace(lookup))
                return null;

            var proveedor = GetProveedoresSafe(value, 50).FirstOrDefault(x =>
                NormalizeLookup(x.Codigo) == lookup ||
                NormalizeLookup(x.Nombre) == lookup ||
                NormalizeLookup(x.Key) == lookup);

            if (proveedor != null)
                return proveedor;

            var parts = (value ?? "").Split(new[] { " - " }, 2, StringSplitOptions.None);
            if (parts.Length == 2 && !String.IsNullOrWhiteSpace(parts[0]) && !String.IsNullOrWhiteSpace(parts[1]))
                return new ProveedorCatalogo(parts[0].Trim(), parts[1].Trim(), 0);

            return null;
        }

        private static void AddProveedorCandidates(List<ProveedorCatalogo> target, IEnumerable<ProveedorCatalogo> source)
        {
            if (target == null || source == null)
                return;

            foreach (var item in source)
            {
                if (item == null)
                    continue;

                bool exists = target.Any(x =>
                    String.Equals(x.Codigo, item.Codigo, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(x.Nombre, item.Nombre, StringComparison.OrdinalIgnoreCase));

                if (!exists)
                    target.Add(item);
            }
        }

        private static string NormalizeLookup(string value)
        {
            return (value ?? "").Trim().ToUpperInvariant();
        }

        private static string ApplyEstadoFromAccion(DocumentoInventarioViewModel documento, string accion, string usuario, string motivoAnulacion, bool allowAdminOverride = false)
        {
            var normalizedAction = NormalizeAccion(accion);
            var estadoActual = documento.Estado ?? "Borrador";

            if (normalizedAction == "Borrador")
            {
                if (!allowAdminOverride && !String.Equals(estadoActual, "Borrador", StringComparison.OrdinalIgnoreCase))
                    return "Solo los documentos en Borrador pueden guardarse como borrador.";

                documento.Estado = "Borrador";
                return "";
            }

            if (normalizedAction == "Enviar")
            {
                if (!allowAdminOverride && !String.Equals(estadoActual, "Borrador", StringComparison.OrdinalIgnoreCase))
                    return "Solo los documentos en Borrador pueden enviarse.";

                documento.Estado = "Enviada";
                ResetRecepcion(documento.DetalleLineas);
                AddAuditoria(documento, "Enviado", usuario, "Documento enviado para recepcion.");
                return "";
            }

            if (normalizedAction == "Recibir")
            {
                if (!allowAdminOverride &&
                    !String.Equals(estadoActual, "Enviada", StringComparison.OrdinalIgnoreCase) &&
                    !String.Equals(estadoActual, "Parcial", StringComparison.OrdinalIgnoreCase))
                    return "Solo los documentos Enviados o Parciales pueden recibirse.";

                CompletarRecepcion(documento.DetalleLineas);
                documento.Estado = "Recibida";
                documento.FechaEntrega = documento.FechaEntrega ?? DateTime.Today;
                AddAuditoria(documento, "Recibido", usuario, "Recepcion completa aplicada.");
                return "";
            }

            if (normalizedAction == "Parcial")
            {
                if (!allowAdminOverride &&
                    !String.Equals(estadoActual, "Enviada", StringComparison.OrdinalIgnoreCase) &&
                    !String.Equals(estadoActual, "Parcial", StringComparison.OrdinalIgnoreCase))
                    return "Solo los documentos Enviados o Parciales pueden guardarse como recepcion parcial.";

                documento.Estado = "Parcial";
                documento.FechaEntrega = documento.FechaEntrega ?? DateTime.Today;
                AddAuditoria(documento, "Parcial", usuario, "Recepcion parcial guardada.");
                return "";
            }

            if (normalizedAction == "Cerrar")
            {
                if (!allowAdminOverride && !String.Equals(estadoActual, "Recibida", StringComparison.OrdinalIgnoreCase))
                    return "Solo los documentos Recibidos pueden cerrarse.";

                documento.Estado = "Cerrada";
                documento.FechaAplicacion = documento.FechaAplicacion ?? DateTime.Today;
                AddAuditoria(documento, "Cerrado", usuario, "Documento cerrado y enviado al historico.");
                return "";
            }

            if (normalizedAction == "Anular")
            {
                if (!allowAdminOverride && String.Equals(estadoActual, "Cerrada", StringComparison.OrdinalIgnoreCase))
                    return "Los documentos Cerrados no pueden anularse desde esta pantalla.";

                if (!allowAdminOverride && String.Equals(estadoActual, "Recibida", StringComparison.OrdinalIgnoreCase))
                    return "Los documentos Recibidos no pueden anularse sin generar una reversa de inventario.";

                if (String.Equals(estadoActual, "Anulada", StringComparison.OrdinalIgnoreCase))
                    return "El documento ya se encuentra Anulado.";

                if (String.IsNullOrWhiteSpace(motivoAnulacion))
                    return "Ingrese el motivo de anulacion.";

                documento.Estado = "Anulada";
                documento.MotivoAnulacion = motivoAnulacion.Trim();
                documento.UsuarioAnula = usuario;
                documento.FechaHoraAnula = DateTime.Now;
                AddAuditoria(documento, "Anulado", usuario, documento.MotivoAnulacion);
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
                case "parcial":
                    return "Parcial";
                case "recibir":
                case "aplicar":
                    return "Recibir";
                case "cerrar":
                    return "Cerrar";
                case "anular":
                    return "Anular";
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

        private static string FormatDateTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd/MM/yyyy HH:mm") : "";
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        private static void CompletarRecepcion(List<DocumentoDetalleLineaViewModel> lineas)
        {
            foreach (var linea in lineas ?? new List<DocumentoDetalleLineaViewModel>())
            {
                EnsureRecepcionLinea(linea, "Parcial");
                linea.CantidadRecibida = linea.CantidadSolicitada;
                linea.CantidadPendiente = 0m;
            }
        }

        private static DocumentoAuditoriaEventoViewModel CreateAuditoriaEvento(string evento, string usuario, DateTime fechaHora, string comentario)
        {
            return new DocumentoAuditoriaEventoViewModel
            {
                Evento = evento,
                Usuario = String.IsNullOrWhiteSpace(usuario) ? "Sistema" : usuario,
                FechaHora = fechaHora,
                Comentario = comentario ?? ""
            };
        }

        private static void AddAuditoria(DocumentoInventarioViewModel documento, string evento, string usuario, string comentario)
        {
            if (documento == null)
                return;

            documento.EventosAuditoria = documento.EventosAuditoria ?? new List<DocumentoAuditoriaEventoViewModel>();
            documento.EventosAuditoria.Add(CreateAuditoriaEvento(evento, usuario, DateTime.Now, comentario));
        }

        private string GetCurrentUserName()
        {
            return User != null && User.Identity != null && !String.IsNullOrWhiteSpace(User.Identity.Name)
                ? User.Identity.Name.Trim()
                : "Usuario actual";
        }

        private DocumentoInventarioViewModel GetSqlDocumentoForOperation(int id)
        {
            return GetHistoricoAjustesFromDatabase("", "", "", "", "", "", id)
                .FirstOrDefault(x => x.ID == id);
        }

        private bool IsProcessAuthorized(string password)
        {
            return IsCurrentProcessAdmin() || IsProcessPasswordValid(password);
        }

        private bool IsCurrentProcessAdmin()
        {
            string account = ConfigurationManager.AppSettings["ProcessAuthorizationUserAccount"] ?? "100";
            string identityName = User != null && User.Identity != null ? User.Identity.Name : "";
            string sessionAccount = Session["USER_ACCOUNT"] == null ? "" : Convert.ToString(Session["USER_ACCOUNT"]);

            return String.Equals((identityName ?? "").Trim(), account, StringComparison.OrdinalIgnoreCase) ||
                   String.Equals((sessionAccount ?? "").Trim(), account, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsProcessPasswordValid(string password)
        {
            if (String.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                string account = ConfigurationManager.AppSettings["ProcessAuthorizationUserAccount"] ?? "100";
                var result = new CT_User().ValidateUser(account, password.Trim());
                return result.Item1 != null && result.Item2 == 0;
            }
            catch
            {
                return false;
            }
        }

        private static string GetDisplayUserName(string databaseUserName)
        {
            if (String.IsNullOrWhiteSpace(databaseUserName) || String.Equals(databaseUserName.Trim(), "SQL", StringComparison.OrdinalIgnoreCase))
                return "Usuario no encontrado";

            return databaseUserName.Trim();
        }

        private static string NormalizeTipoDocumento(string tipoDocumento)
        {
            if (String.Equals(tipoDocumento, "Entrada de Inventario", StringComparison.OrdinalIgnoreCase))
                return "Entrada de Inventario";

            if (String.Equals(tipoDocumento, "Salida de Inventario", StringComparison.OrdinalIgnoreCase))
                return "Salida de Inventario";

            return "Compra";
        }

        private string GetNextNumero(string tipoDocumento)
        {
            var prefix = GetNumeroPrefix(tipoDocumento);
            var seed = GetNumeroSeed(tipoDocumento);
            var max = seed - 1;

            try
            {
                EnsureDocumentSqlSchema();
                using (SqlConnection connection = CreateMasterConnection())
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
SELECT MAX(Value) AS MaxValue
FROM
(
    SELECT TRY_CONVERT(INT, SUBSTRING(Numero, LEN(@Prefix) + 1, 20)) AS Value
    FROM dbo.ExtCentral_DocumentoInventario
    WHERE Numero LIKE @PrefixLike
    UNION ALL
    SELECT TRY_CONVERT(INT, SUBSTRING(Number, LEN(@Prefix) + 1, 20)) AS Value
    FROM dbo.POD_Receipt
    WHERE Number LIKE @PrefixLike
) X
WHERE Value IS NOT NULL;";
                    command.Parameters.AddWithValue("@Prefix", prefix);
                    command.Parameters.AddWithValue("@PrefixLike", prefix + "%");

                    connection.Open();
                    object value = command.ExecuteScalar();
                    if (value != null && value != DBNull.Value)
                        max = Math.Max(max, Convert.ToInt32(value));
                }
            }
            catch
            {
                max = seed - 1;
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
                Codigo = codigo ?? "";
                Nombre = nombre ?? "";
                PlazoCredito = plazoCredito;
            }

            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public int PlazoCredito { get; private set; }
            public string Key { get { return Codigo + " - " + Nombre; } }
        }

        private class ProductoCatalogo
        {
            public string Codigo { get; set; }
            public string Descripcion { get; set; }
            public string Unidad { get; set; }
            public decimal Costo { get; set; }
            public decimal Impuesto { get; set; }
        }
    }
}
