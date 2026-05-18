using Security.EntitiesAVS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using UltraERP.BusinessEntities;
using UltraERP.BusinessLogic;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class HojasTrabajoController : Controller
    {
        private const int DefaultResultCount = 500;
        private const int StyleUpdateItemPrice = 251;
        private const int StyleDownloadItem = 261;
        private const int StyleUpdateItemTax = 320;

        public ActionResult Inicio()
        {
            try
            {
                List<HojaTrabajoViewModel> model = GetHojasTrabajoFromSql(DefaultResultCount);
                ViewBag.HojasTrabajoDataSource = "SQL";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["HojaTrabajoError"] = "No se pudieron cargar las hojas de trabajo desde SQL: " + ex.Message;
                ViewBag.HojasTrabajoDataSource = "SQL";
                return View(Enumerable.Empty<HojaTrabajoViewModel>());
            }
        }

        public static HojaTrabajoViewModel RegistrarHoja(HojaTrabajoViewModel hoja)
        {
            if (hoja == null)
                return null;

            hoja.ID = hoja.ID > 0 ? hoja.ID : 0;
            hoja.FechaCreacion = hoja.FechaCreacion == default(DateTime) ? DateTime.Now : hoja.FechaCreacion;
            hoja.FechaEfectiva = hoja.FechaEfectiva == default(DateTime) ? hoja.FechaCreacion : hoja.FechaEfectiva;
            hoja.TiendasTexto = hoja.Tiendas == null ? "" : String.Join(", ", hoja.Tiendas.Select(x => x.TiendaNombre));
            return hoja;
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id, int estado)
        {
            try
            {
                Respuesta response = new CT_Worksheet().Change_Status(
                    GetStoresAvailable(),
                    GetTasksAvailable(),
                    GetWorksheetUsersAvailable(),
                    GetCurrentUserID(),
                    id,
                    estado);

                response = EnsureWorksheetStatusChange(id, estado, response);

                if (!response.Status)
                    return Json(new JsonResponse(response.InternalMessage, GetWorksheetStatusMessage(response), null, false));

                HojaTrabajoViewModel hoja = GetHojaTrabajoById(id);
                return Json(new JsonResponse(response.InternalMessage, "Estado actualizado correctamente.", hoja, true));
            }
            catch (Exception ex)
            {
                Respuesta response = EnsureWorksheetStatusChange(id, estado, new Respuesta(ex.Message, "error_reg_actualizado", null, false));
                if (response.Status)
                {
                    HojaTrabajoViewModel hoja = GetHojaTrabajoById(id);
                    return Json(new JsonResponse(response.InternalMessage, "Estado actualizado correctamente.", hoja, true));
                }

                return Json(new JsonResponse(response.InternalMessage, GetWorksheetStatusMessage(response), null, false));
            }
        }

        public ActionResult DescargarArchivo(int id)
        {
            try
            {
                HojaTrabajoViewModel hoja = GetHojaTrabajoById(id);
                if (hoja == null)
                    return HttpNotFound();

                if (hoja.ArchivoID > 0)
                {
                    EN_Worksheet.WorksheetContent content = new CT_Worksheet().Get_WorksheetContent(
                        hoja.ArchivoID,
                        hoja.ID,
                        GetStoresAvailable(),
                        GetTasksAvailable(),
                        GetWorksheetUsersAvailable());

                    if (content != null && content.ContentData != null)
                    {
                        string fileName = "T" + content.TaskCode + "-" + hoja.ID + "-" + content.ID + "-" + content.DateAppliedStringFile + "-" + content.FileName;
                        byte[] contentData = NormalizeDownloadContent(content);
                        return File(contentData, content.ContentType, fileName);
                    }
                }

                byte[] bytes = Encoding.UTF8.GetBytes(BuildWorksheetText(hoja));
                return File(bytes, "text/plain", "worksheet-" + hoja.ID + ".txt");
            }
            catch
            {
                return HttpNotFound();
            }
        }

        private List<HojaTrabajoViewModel> GetHojasTrabajoFromSql(int take)
        {
            var worksheets = new List<HojaTrabajoViewModel>();
            ConnectionStringSettings settings = GetMasterConnection();

            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return worksheets;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT TOP (@Take)
                        W.ID,
                        W.Style,
                        W.Status,
                        W.Title,
                        W.Notes,
                        W.EffectiveDate,
                        W.FromDate,
                        XW.Date AS ExtDate,
                        XW.WizardTaskCode,
                        XW.WorksheetContentID,
                        XW.HQUserID,
                        ISNULL(WL.Descripcion, XW.WizardTaskCode) AS TaskDescription,
                        CASE
                            WHEN XW.HQUserID IS NULL THEN ''
                            ELSE 'Usuario ' + CONVERT(VARCHAR(20), XW.HQUserID)
                        END AS UserName,
                        Stores.StoresID,
                        Stores.StoresName,
                        Stores.LastProcessed
                    FROM dbo.Worksheet W
                    LEFT JOIN dbo.ExtCentral_Worksheet XW ON XW.WorksheetID = W.ID
                    LEFT JOIN dbo.ExtCentral_WizardLista WL ON WL.Codigo = XW.WizardTaskCode
                    OUTER APPLY
                    (
                        SELECT
                            STRING_AGG(CONVERT(VARCHAR(20), WS.StoreID), ', ') AS StoresID,
                            STRING_AGG(S.Name, ', ') AS StoresName,
                            MAX(WS.DateProcessed) AS LastProcessed
                        FROM dbo.WorksheetStore WS
                        LEFT JOIN dbo.Store S ON S.ID = WS.StoreID
                        WHERE WS.WorksheetID = W.ID
                    ) Stores
                    WHERE
                        (@Tasks = '%' OR XW.WizardTaskCode IN (SELECT value FROM STRING_SPLIT(@Tasks, ',') WHERE value <> ''))
                        AND (@Users = '%' OR XW.HQUserID IN (SELECT TRY_CONVERT(INT, value) FROM STRING_SPLIT(@Users, ',') WHERE value <> ''))
                        AND
                        (
                            @Stores = '%'
                            OR EXISTS
                            (
                                SELECT 1
                                FROM dbo.WorksheetStore WSFilter
                                WHERE WSFilter.WorksheetID = W.ID
                                  AND WSFilter.StoreID IN (SELECT TRY_CONVERT(INT, value) FROM STRING_SPLIT(@Stores, ',') WHERE value <> '')
                            )
                        )
                    ORDER BY W.ID DESC";

                command.Parameters.AddWithValue("@Take", take <= 0 ? DefaultResultCount : take);
                command.Parameters.AddWithValue("@Stores", GetStoresAvailable());
                command.Parameters.AddWithValue("@Tasks", GetTasksAvailable());
                command.Parameters.AddWithValue("@Users", GetWorksheetUsersAvailable());

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        worksheets.Add(MapWorksheet(reader));
                }
            }

            foreach (HojaTrabajoViewModel worksheet in worksheets)
                HydrateWorksheetDetail(worksheet);

            return worksheets;
        }

        private HojaTrabajoViewModel GetHojaTrabajoById(int id)
        {
            return GetHojasTrabajoFromSql(DefaultResultCount).FirstOrDefault(x => x.ID == id);
        }

        private void HydrateWorksheetDetail(HojaTrabajoViewModel worksheet)
        {
            if (worksheet == null)
                return;

            worksheet.Tiendas = GetWorksheetStores(worksheet.ID);
            worksheet.Historial = GetWorksheetHistory(worksheet.ID);
            worksheet.Contenido = GetWorksheetContent(worksheet);

            if (worksheet.Tiendas.Count > 0)
                worksheet.TiendasTexto = String.Join(", ", worksheet.Tiendas.Select(x => x.TiendaNombre).Where(x => !String.IsNullOrWhiteSpace(x)));
        }

        private List<HojaTrabajoTiendaViewModel> GetWorksheetStores(int worksheetID)
        {
            try
            {
                return new CT_Worksheet()
                    .GetStores(GetStoresAvailable(), GetTasksAvailable(), GetWorksheetUsersAvailable(), worksheetID)
                    .Select(MapStore)
                    .ToList();
            }
            catch
            {
                return GetWorksheetStoresDirect(worksheetID);
            }
        }

        private List<HojaTrabajoHistorialViewModel> GetWorksheetHistory(int worksheetID)
        {
            try
            {
                return new CT_Worksheet()
                    .GetHistories(GetStoresAvailable(), GetTasksAvailable(), GetWorksheetUsersAvailable(), worksheetID)
                    .Select(MapHistory)
                    .OrderBy(x => x.FechaHora)
                    .ToList();
            }
            catch
            {
                return GetWorksheetHistoryDirect(worksheetID);
            }
        }

        private List<HojaTrabajoContenidoViewModel> GetWorksheetContent(HojaTrabajoViewModel worksheet)
        {
            try
            {
                if (worksheet.EstiloID == StyleUpdateItemPrice)
                {
                    return new CT_Worksheet()
                        .GetAll_WorksheetUpdateItemPrice(GetStoresAvailable(), GetTasksAvailable(), GetWorksheetUsersAvailable(), worksheet.ID)
                        .Select(MapPriceContent)
                        .ToList();
                }

                if (worksheet.EstiloID == StyleUpdateItemTax)
                {
                    return new CT_Worksheet()
                        .GetAll_Worksheet_ItemTax(GetStoresAvailable(), GetTasksAvailable(), GetWorksheetUsersAvailable(), worksheet.ID)
                        .Select(MapTaxContent)
                        .ToList();
                }

                return new CT_Worksheet()
                    .GetAll_Worksheet_ItemUpdate(GetStoresAvailable(), GetTasksAvailable(), GetWorksheetUsersAvailable(), worksheet.ID)
                    .Select(MapItemContent)
                    .ToList();
            }
            catch
            {
                return GetWorksheetContentDirect(worksheet);
            }
        }

        private List<HojaTrabajoTiendaViewModel> GetWorksheetStoresDirect(int worksheetID)
        {
            var stores = new List<HojaTrabajoTiendaViewModel>();
            ConnectionStringSettings settings = GetMasterConnection();

            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return stores;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT WS.StoreID, ISNULL(S.Name, CONVERT(VARCHAR(20), WS.StoreID)) AS StoreName, WS.Status, WS.DateProcessed
                    FROM dbo.WorksheetStore WS
                    LEFT JOIN dbo.Store S ON S.ID = WS.StoreID
                    WHERE WS.WorksheetID = @WorksheetID
                    ORDER BY S.Name";
                command.Parameters.AddWithValue("@WorksheetID", worksheetID);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stores.Add(new HojaTrabajoTiendaViewModel
                        {
                            TiendaID = ReadInt(reader, "StoreID"),
                            TiendaNombre = ReadString(reader, "StoreName"),
                            EstadoID = ReadInt(reader, "Status"),
                            FechaProcesado = ReadNullableDate(reader, "DateProcessed")
                        });
                    }
                }
            }

            return stores;
        }

        private List<HojaTrabajoHistorialViewModel> GetWorksheetHistoryDirect(int worksheetID)
        {
            var history = new List<HojaTrabajoHistorialViewModel>();
            ConnectionStringSettings settings = GetMasterConnection();

            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return history;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT WH.Status, WH.HistoryDate, ISNULL(S.Name, 'Oficina Central') AS StoreName, WH.Comment
                    FROM dbo.WorksheetHistory WH
                    LEFT JOIN dbo.Store S ON S.ID = WH.StoreID
                    WHERE WH.WorksheetID = @WorksheetID
                    ORDER BY WH.HistoryDate";
                command.Parameters.AddWithValue("@WorksheetID", worksheetID);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        history.Add(new HojaTrabajoHistorialViewModel
                        {
                            EstadoID = ReadInt(reader, "Status"),
                            Tienda = ReadString(reader, "StoreName"),
                            FechaHora = ReadNullableDate(reader, "HistoryDate") ?? DateTime.MinValue,
                            Comentario = ReadString(reader, "Comment")
                        });
                    }
                }
            }

            return history;
        }

        private List<HojaTrabajoContenidoViewModel> GetWorksheetContentDirect(HojaTrabajoViewModel worksheet)
        {
            var content = new List<HojaTrabajoContenidoViewModel>();
            ConnectionStringSettings settings = GetMasterConnection();

            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString) || worksheet == null)
                return content;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = worksheet.EstiloID == StyleUpdateItemPrice
                    ? GetPriceContentSql()
                    : worksheet.EstiloID == StyleUpdateItemTax
                        ? GetTaxContentSql()
                        : GetItemUpdateContentSql();

                command.Parameters.AddWithValue("@WorksheetID", worksheet.ID);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        content.Add(new HojaTrabajoContenidoViewModel
                        {
                            Tienda = worksheet.TiendasTexto,
                            Disponible = true,
                            CodigoArticulo = ReadString(reader, "ItemLookupCode"),
                            Descripcion = ReadString(reader, "Description"),
                            DescripcionExtendida = ReadString(reader, "ExtendedDescription"),
                            PrecioLiquidacion = ReadNullableDecimal(reader, "SalePrice"),
                            InicioLiquidacion = ReadNullableDate(reader, "SaleStartDate"),
                            FinLiquidacion = ReadNullableDate(reader, "SaleEndDate"),
                            LimiteInferior = ReadNullableDecimal(reader, "LowerBound"),
                            LimiteSuperior = ReadNullableDecimal(reader, "UpperBound"),
                            PrecioCantidad = ReadNullableDecimal(reader, "BuydownPrice"),
                            CantidadLiquidacion = ReadNullableDecimal(reader, "BuydownQuantity"),
                            Impuesto = ReadString(reader, "ItemTaxDescription")
                        });
                    }
                }
            }

            return content;
        }

        private static string GetItemUpdateContentSql()
        {
            return @"
                SELECT I.ItemLookupCode, I.Description, I.ExtendedDescription,
                       NULL AS SalePrice, NULL AS SaleStartDate, NULL AS SaleEndDate,
                       NULL AS LowerBound, NULL AS UpperBound, NULL AS BuydownPrice,
                       NULL AS BuydownQuantity, '' AS ItemTaxDescription
                FROM dbo.Worksheet_ItemUpdate WI
                LEFT JOIN dbo.Item I ON I.ID = WI.ItemID
                WHERE WI.WorksheetID = @WorksheetID
                ORDER BY I.ItemLookupCode";
        }

        private static string GetPriceContentSql()
        {
            return @"
                SELECT I.ItemLookupCode, I.Description, I.ExtendedDescription,
                       WP.SalePrice, WP.SaleStartDate, WP.SaleEndDate,
                       WP.LowerBound, WP.UpperBound, WP.BuydownPrice,
                       WP.BuydownQuantity, '' AS ItemTaxDescription
                FROM dbo.WorksheetUpdateItemPrices WP
                LEFT JOIN dbo.Item I ON I.ID = WP.ItemID
                WHERE WP.WorksheetID = @WorksheetID
                ORDER BY I.ItemLookupCode";
        }

        private static string GetTaxContentSql()
        {
            return @"
                SELECT I.ItemLookupCode, I.Description, I.ExtendedDescription,
                       NULL AS SalePrice, NULL AS SaleStartDate, NULL AS SaleEndDate,
                       NULL AS LowerBound, NULL AS UpperBound, NULL AS BuydownPrice,
                       NULL AS BuydownQuantity, ISNULL(T.Description, '') AS ItemTaxDescription
                FROM dbo.Worksheet_ItemTax WT
                LEFT JOIN dbo.Item I ON I.ID = WT.ItemID
                LEFT JOIN dbo.Tax T ON T.ID = WT.TaxID
                WHERE WT.WorksheetID = @WorksheetID
                ORDER BY I.ItemLookupCode";
        }

        private static HojaTrabajoViewModel MapWorksheet(SqlDataReader reader)
        {
            DateTime fromDate = ReadNullableDate(reader, "FromDate")
                ?? ReadNullableDate(reader, "ExtDate")
                ?? ReadNullableDate(reader, "EffectiveDate")
                ?? DateTime.Now;

            DateTime effectiveDate = ReadNullableDate(reader, "EffectiveDate") ?? fromDate;

            return new HojaTrabajoViewModel
            {
                ID = ReadInt(reader, "ID"),
                EstiloID = ReadInt(reader, "Style"),
                EstadoID = ReadInt(reader, "Status"),
                Titulo = ReadString(reader, "Title"),
                Notas = ReadString(reader, "Notes"),
                TareaCodigo = ReadString(reader, "WizardTaskCode"),
                TareaDescripcion = ReadString(reader, "TaskDescription"),
                Usuario = ReadString(reader, "UserName"),
                FechaCreacion = fromDate,
                FechaEfectiva = effectiveDate,
                FechaAplicacion = ReadNullableDate(reader, "LastProcessed"),
                TiendasTexto = ReadString(reader, "StoresName"),
                ArchivoID = ReadInt(reader, "WorksheetContentID")
            };
        }

        private static HojaTrabajoTiendaViewModel MapStore(EN_Worksheet.WorksheetStore store)
        {
            return new HojaTrabajoTiendaViewModel
            {
                TiendaID = store.StoreID,
                TiendaNombre = store.StoreName,
                EstadoID = store.Status,
                FechaProcesado = store.DateProcessed
            };
        }

        private static HojaTrabajoHistorialViewModel MapHistory(EN_Worksheet.WorksheetHistory history)
        {
            return new HojaTrabajoHistorialViewModel
            {
                EstadoID = history.Status,
                Tienda = history.StoreName,
                FechaHora = history.HistoryDate ?? DateTime.MinValue,
                Comentario = history.Comment
            };
        }

        private static HojaTrabajoContenidoViewModel MapPriceContent(EN_Worksheet.WorksheetUpdateItemPrice item)
        {
            HojaTrabajoContenidoViewModel model = MapItemDetail(item);
            model.PrecioLiquidacion = item.SalePrice;
            model.InicioLiquidacion = item.SaleStartDate;
            model.FinLiquidacion = item.SaleEndDate;
            model.LimiteInferior = item.LowerBound;
            model.LimiteSuperior = item.UpperBound;
            model.PrecioCantidad = item.BuydownPrice;
            model.CantidadLiquidacion = Convert.ToDecimal(item.BuydownQuantity);
            model.Impuesto = item.ItemTaxDescription;
            return model;
        }

        private static HojaTrabajoContenidoViewModel MapTaxContent(EN_Worksheet.Worksheet_ItemTax item)
        {
            HojaTrabajoContenidoViewModel model = MapItemDetail(item);
            model.Impuesto = item.ItemTaxDescription;
            return model;
        }

        private static HojaTrabajoContenidoViewModel MapItemContent(EN_Worksheet.Worksheet_ItemUpdate item)
        {
            return MapItemDetail(item);
        }

        private static HojaTrabajoContenidoViewModel MapItemDetail(EN_Worksheet.WorksheetDetail item)
        {
            return new HojaTrabajoContenidoViewModel
            {
                Tienda = item.StoreName,
                Disponible = item.StoreAvailibity,
                CodigoArticulo = item.ItemLookupcode,
                Descripcion = item.Description,
                DescripcionExtendida = item.ExtendedDescription
            };
        }

        private string BuildWorksheetText(HojaTrabajoViewModel hoja)
        {
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

            foreach (HojaTrabajoContenidoViewModel item in hoja.Contenido)
                builder.AppendLine(item.Tienda + " | " + item.CodigoArticulo + " | " + item.Descripcion + " | " + (String.IsNullOrWhiteSpace(item.Impuesto) ? "-" : item.Impuesto));

            return builder.ToString();
        }

        private static byte[] NormalizeDownloadContent(EN_Worksheet.WorksheetContent content)
        {
            if (content == null || content.ContentData == null)
                return new byte[0];

            string contentType = content.ContentType ?? "";
            string fileName = content.FileName ?? "";
            bool isXml = contentType.IndexOf("xml", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase);

            if (!isXml)
                return content.ContentData;

            string xml = Encoding.UTF8.GetString(content.ContentData).Trim();
            if (String.IsNullOrWhiteSpace(xml))
                return content.ContentData;

            if (xml.StartsWith("<Property", StringComparison.OrdinalIgnoreCase))
                xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExtendedProperties>" + xml + "</ExtendedProperties>";

            return Encoding.UTF8.GetBytes(xml);
        }

        private Respuesta EnsureWorksheetStatusChange(int worksheetID, int status, Respuesta response)
        {
            if (response != null && response.Status)
                return response;

            Respuesta fallback = ChangeWorksheetStatusDirect(worksheetID, status);
            if (fallback.Status)
                return fallback;

            if (response != null)
            {
                response.Message = GetWorksheetStatusMessage(response);
                return response;
            }

            return fallback;
        }

        private Respuesta ChangeWorksheetStatusDirect(int worksheetID, int status)
        {
            if (!IsWorksheetStatusAllowed(status))
                return new Respuesta("", "No se puede cambiar la hoja al estado solicitado.", null, false);

            ConnectionStringSettings settings = GetMasterConnection();
            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return new Respuesta("", "No se encontro la conexion de la base de datos.", null, false);

            try
            {
                using (var connection = new SqlConnection(settings.ConnectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        WorksheetStatusContext context = GetWorksheetStatusContext(connection, transaction, worksheetID);
                        if (context == null || !context.Exists)
                        {
                            transaction.Rollback();
                            return new Respuesta("", "No se encontro la hoja de trabajo o no tiene acceso a ella.", null, false);
                        }

                        int expectedCurrentStatus = status == 1 ? 2 : 1;
                        bool worksheetAlreadyUpdated = context.CurrentStatus == status;
                        bool storesAlreadyUpdated = context.StoreStatuses.Count > 0 && context.StoreStatuses.All(x => x == status);

                        if (worksheetAlreadyUpdated && storesAlreadyUpdated)
                        {
                            transaction.Rollback();
                            return new Respuesta("", "Estado actualizado correctamente.", null, true);
                        }

                        if (!worksheetAlreadyUpdated && context.CurrentStatus != expectedCurrentStatus)
                        {
                            transaction.Rollback();
                            return new Respuesta("", "La hoja ya no esta en un estado valido para esta accion.", null, false);
                        }

                        if (!worksheetAlreadyUpdated)
                            UpdateWorksheetStatus(connection, transaction, worksheetID, status);

                        UpdateWorksheetStoresStatus(connection, transaction, worksheetID, status);
                        InsertWorksheetHistory(connection, transaction, worksheetID, status);

                        transaction.Commit();
                        return new Respuesta("", "Estado actualizado correctamente.", null, true);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Respuesta(ex.Message, "No se pudo cambiar el estado de la hoja de trabajo.", null, false);
            }
        }

        private WorksheetStatusContext GetWorksheetStatusContext(SqlConnection connection, SqlTransaction transaction, int worksheetID)
        {
            WorksheetStatusContext context = null;

            using (SqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT W.ID, W.Status
                    FROM dbo.Worksheet W
                    LEFT JOIN dbo.ExtCentral_Worksheet EW ON EW.WorksheetID = W.ID
                    WHERE W.ID = @WorksheetID
                      AND (@Users = '%' OR EW.HQUserID IN (SELECT TRY_CONVERT(INT, value) FROM STRING_SPLIT(@Users, ',') WHERE value <> ''))
                      AND (@Tasks = '%' OR EW.WizardTaskCode IN (SELECT value FROM STRING_SPLIT(@Tasks, ',') WHERE value <> ''))
                      AND (
                            @Stores = '%'
                            OR (SELECT COUNT(*) FROM dbo.WorksheetStore WHERE WorksheetID = @WorksheetID) =
                               (SELECT COUNT(*) FROM dbo.WorksheetStore WHERE WorksheetID = @WorksheetID AND StoreID IN (SELECT TRY_CONVERT(INT, value) FROM STRING_SPLIT(@Stores, ',') WHERE value <> ''))
                      );

                    SELECT WS.StoreID, WS.Status
                    FROM dbo.WorksheetStore WS
                    WHERE WS.WorksheetID = @WorksheetID
                    ORDER BY WS.StoreID;";

                command.Parameters.AddWithValue("@WorksheetID", worksheetID);
                command.Parameters.AddWithValue("@Stores", GetStoresAvailable());
                command.Parameters.AddWithValue("@Tasks", GetTasksAvailable());
                command.Parameters.AddWithValue("@Users", GetWorksheetUsersAvailable());

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        context = new WorksheetStatusContext
                        {
                            Exists = true,
                            CurrentStatus = ReadInt(reader, "Status")
                        };
                    }

                    if (context != null && reader.NextResult())
                    {
                        while (reader.Read())
                            context.StoreStatuses.Add(ReadInt(reader, "Status"));
                    }
                }
            }

            return context;
        }

        private static void UpdateWorksheetStatus(SqlConnection connection, SqlTransaction transaction, int worksheetID, int status)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                command.CommandText = "UPDATE dbo.Worksheet SET [Status] = @Status WHERE ID = @WorksheetID";
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@WorksheetID", worksheetID);
                command.ExecuteNonQuery();
            }
        }

        private static void UpdateWorksheetStoresStatus(SqlConnection connection, SqlTransaction transaction, int worksheetID, int status)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                command.CommandText = "UPDATE dbo.WorksheetStore SET [Status] = @Status WHERE WorksheetID = @WorksheetID";
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@WorksheetID", worksheetID);
                command.ExecuteNonQuery();
            }
        }

        private void InsertWorksheetHistory(SqlConnection connection, SqlTransaction transaction, int worksheetID, int status)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    DECLARE @Inserted TABLE (WorksheetID INT, WorksheetHistoryID INT);

                    INSERT INTO dbo.WorksheetHistory (Comment, HistoryDate, [Status], StoreID, WorksheetID)
                    OUTPUT inserted.WorksheetID, inserted.ID INTO @Inserted(WorksheetID, WorksheetHistoryID)
                    SELECT @Comment, GETDATE(), @Status, WS.StoreID, @WorksheetID
                    FROM dbo.WorksheetStore WS
                    WHERE WS.WorksheetID = @WorksheetID;

                    INSERT INTO dbo.ExtCentral_WorksheetHistory (WorksheetID, WorksheetHistoryID, HQUserID)
                    SELECT WorksheetID, WorksheetHistoryID, @HQUserID
                    FROM @Inserted;";

                command.Parameters.AddWithValue("@Comment", BuildWorksheetStatusComment(status));
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@WorksheetID", worksheetID);
                command.Parameters.AddWithValue("@HQUserID", GetCurrentUserID());
                command.ExecuteNonQuery();
            }
        }

        private string BuildWorksheetStatusComment(int status)
        {
            string action;
            switch (status)
            {
                case 1:
                    action = "Desaprobado";
                    break;
                case 0:
                    action = "Suspendido";
                    break;
                case 8:
                    action = "Aprobado sin cambios";
                    break;
                default:
                    action = "Aprobado";
                    break;
            }

            return action + " por " + GetCurrentUserName() + " desde AVS Central Manager";
        }

        private string GetCurrentUserName()
        {
            if (User != null && User.Identity != null && !String.IsNullOrWhiteSpace(User.Identity.Name))
                return User.Identity.Name.Trim();

            return "soporte";
        }

        private static bool IsWorksheetStatusAllowed(int status)
        {
            return status == 0 || status == 1 || status == 2 || status == 8;
        }

        private static string GetWorksheetStatusMessage(Respuesta response)
        {
            if (response == null)
                return "No se pudo cambiar el estado de la hoja de trabajo.";

            string message = response.Message ?? "";
            string internalMessage = response.InternalMessage ?? "";

            if (String.Equals(message, "error_reg_actualizado", StringComparison.OrdinalIgnoreCase))
            {
                if (internalMessage.IndexOf("HQUser", StringComparison.OrdinalIgnoreCase) >= 0)
                    return "No se pudo completar el historial automatico de la hoja. Intente nuevamente.";

                return "No se pudo actualizar completamente la hoja de trabajo.";
            }

            if (String.Equals(message, "error_w_status", StringComparison.OrdinalIgnoreCase))
                return "No se puede cambiar la hoja al estado solicitado.";

            return String.IsNullOrWhiteSpace(message) ? "No se pudo cambiar el estado de la hoja de trabajo." : message;
        }

        private static ConnectionStringSettings GetMasterConnection()
        {
            return ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];
        }

        private string GetStoresAvailable()
        {
            return GetDataAccessAvailable(ConfigurationManager.AppSettings["DataStoreCode"] ?? "uerp-store");
        }

        private string GetTasksAvailable()
        {
            return GetDataAccessAvailable(ConfigurationManager.AppSettings["DataWizardCode"] ?? "uerp-wizard");
        }

        private string GetWorksheetUsersAvailable()
        {
            return GetDataAccessAvailable(ConfigurationManager.AppSettings["DataWorksheetUser"] ?? "uerp-worksheet-user");
        }

        private string GetDataAccessAvailable(string dataCode)
        {
            EN_SC_DataAccess[] dataAccess = Session["USER_DATAACCESS"] as EN_SC_DataAccess[];

            if (dataAccess == null || dataAccess.Length == 0)
                return "%";

            EN_SC_DataAccess[] access = dataAccess.Where(x => x.Code == dataCode).ToArray();
            if (access.Length == 0 || access.Any(x => x.EnableAll))
                return "%";

            return String.Join(",", access.Where(x => !String.IsNullOrWhiteSpace(x.DataIDs)).Select(x => x.DataIDs));
        }

        private int GetCurrentUserID()
        {
            int userID;
            return Session["USER_AUTOID"] != null && Int32.TryParse(Convert.ToString(Session["USER_AUTOID"]), out userID)
                ? userID
                : 0;
        }

        private static string ReadString(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? "" : Convert.ToString(reader[column]);
        }

        private static int ReadInt(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader[column]);
        }

        private static DateTime? ReadNullableDate(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (DateTime?)null : Convert.ToDateTime(reader[column]);
        }

        private static decimal? ReadNullableDecimal(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (decimal?)null : Convert.ToDecimal(reader[column]);
        }

        private class WorksheetStatusContext
        {
            public WorksheetStatusContext()
            {
                StoreStatuses = new List<int>();
            }

            public bool Exists { get; set; }
            public int CurrentStatus { get; set; }
            public List<int> StoreStatuses { get; private set; }
        }
    }
}
