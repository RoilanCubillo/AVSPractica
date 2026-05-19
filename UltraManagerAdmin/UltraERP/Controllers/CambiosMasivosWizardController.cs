using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Security.EntitiesAVS;
using UltraERP.BusinessEntities;
using UltraERP.BusinessLogic;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class CambiosMasivosWizardController : Controller
    {
        public ActionResult Inicio()
        {
            var catalog = GetStoreCatalogSafe();
            var model = new CambiosMasivosWizardViewModel
            {
                FechaEfectivaSugerida = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"),
                Tareas = GetTareasSafe(),
                GruposTienda = catalog.Grupos,
                Tiendas = catalog.Tiendas
            };

            ViewBag.CambiosMasivosDataSource = catalog.FromSql ? "SQL" : "Sin datos";
            return View(model);
        }

        [HttpPost]
        public JsonResult Aplicar(CambioMasivoAplicacionRequest request)
        {
            if (request == null)
                return Json(new JsonResponse("Solicitud vacia.", "No se recibio informacion del wizard.", null, false));

            var tareas = GetTareasSafe();
            var catalog = GetStoreCatalogSafe();
            var tarea = tareas.FirstOrDefault(x => String.Equals(x.Codigo, request.TaskCode, StringComparison.OrdinalIgnoreCase));
            if (tarea == null)
                return Json(new JsonResponse("Tarea no encontrada.", "Seleccione una tarea valida para continuar.", null, false));

            var tiendasSeleccionadas = catalog.Tiendas
                .Where(x => request.StoreIDs != null && request.StoreIDs.Contains(x.ID))
                .ToList();

            if (tiendasSeleccionadas.Count == 0)
                return Json(new JsonResponse("Sin tiendas.", "Seleccione al menos una tienda para crear la hoja de trabajo.", null, false));

            var filas = NormalizeRows(request.Rows);
            if (filas.Count == 0)
                return Json(new JsonResponse("Sin contenido.", "Cargue al menos una fila antes de aplicar cambios.", null, false));

            var validationMessage = ValidateRows(tarea, filas);
            if (!String.IsNullOrWhiteSpace(validationMessage))
                return Json(new JsonResponse("Validacion de archivo.", validationMessage, null, false));

            DateTime fechaEfectiva;
            if (!DateTime.TryParseExact(request.EffectiveDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaEfectiva))
                fechaEfectiva = DateTime.Today.AddDays(1);

            var notas = BuildNotes(request.Notes, request.FileName, request.Separator, filas.Count);

            WorksheetCreationResult creation = CreateWorksheetInSql(tarea, tiendasSeleccionadas, filas, fechaEfectiva, notas);
            if (!creation.Status)
                return Json(new JsonResponse(creation.InternalMessage, creation.Message, null, false));

            return Json(new JsonResponse("", creation.Message, new
            {
                WorksheetID = creation.WorksheetID,
                RedirectUrl = Url.Action("Inicio", "HojasTrabajo"),
                RegisteredRows = creation.RegisteredRows,
                Stores = creation.Stores
            }, true));
        }

        private WorksheetCreationResult CreateWorksheetInSql(CambioMasivoTaskViewModel tarea, IList<CambioMasivoTiendaViewModel> tiendas, IList<List<string>> filas, DateTime fechaEfectiva, string notas)
        {
            var storeIDs = tiendas.Select(x => x.ID).ToArray();
            var items = ResolveItems(filas);
            if (items.MissingCodes.Count > 0)
            {
                return new WorksheetCreationResult
                {
                    Status = false,
                    Message = "No se pudo crear la hoja: hay productos que no existen o no estan disponibles.",
                    InternalMessage = "Productos no encontrados: " + String.Join(", ", items.MissingCodes.Take(8))
                };
            }

            string title = BuildWizardTitle(tarea);
            string userID = GetCurrentUserID();
            var wizard = new CT_Wizard();
            var responses = new List<Respuesta>();

            switch ((tarea.Codigo ?? "").Trim())
            {
                case "104":
                    var taxRows = filas.Select(row =>
                    {
                        EN_Item item = items.ByCode[NormalizeLookup(GetValue(row, 0))];
                        return new EN_WizardStructs.TablaItemTax
                        {
                            ID = item.ID,
                            ItemLookupCode = item.ItemLookupCode,
                            Description = item.Description,
                            TaxPerAnterior = FirstNonEmpty(GetValue(row, 1), Convert.ToString(item.TaxPercentage, CultureInfo.InvariantCulture)),
                            TaxPercentage = GetValue(row, 2)
                        };
                    }).ToList();
                    foreach (int storeID in storeIDs)
                        responses.Add(wizard.Task104(storeID, title, notas, taxRows, userID));
                    break;

                case "107":
                    var descriptionRows = filas.Select(row =>
                    {
                        EN_Item item = items.ByCode[NormalizeLookup(GetValue(row, 0))];
                        return new EN_WizardStructs.TablaItemDes
                        {
                            ID = item.ID,
                            ItemLookupCode = item.ItemLookupCode,
                            Description = item.Description,
                            DesAnterior = item.Description,
                            NuevaDes = GetValue(row, 1)
                        };
                    }).ToList();
                    foreach (int storeID in storeIDs)
                        responses.Add(wizard.Task107(storeID, title, notas, descriptionRows, userID));
                    break;

                case "110":
                    var cabysRows = filas.Select(row =>
                    {
                        EN_Item item = items.ByCode[NormalizeLookup(GetValue(row, 0))];
                        return new EN_WizardStructs.TablaItemSub3
                        {
                            ID = item.ID,
                            ItemLookupCode = item.ItemLookupCode,
                            Description = item.Description,
                            OldCabys = item.SubDescription3,
                            SubDescription3 = GetValue(row, 1)
                        };
                    }).ToList();
                    foreach (int storeID in storeIDs)
                        responses.Add(wizard.Task110(storeID, title, notas, cabysRows, userID));
                    break;

                case "121":
                    var dynamicRows = filas.Select(row =>
                    {
                        EN_Item item = items.ByCode[NormalizeLookup(GetValue(row, 0))];
                        return new EN_WizardStructs.TablaItemPriceDynamic
                        {
                            ID = item.ID,
                            SupplierCode = item.SupplierCode,
                            ItemLookupCode = item.ItemLookupCode,
                            InvoiceDiscount = FirstNonEmpty(GetValue(row, 3), "0"),
                            CustomerDiscount = FirstNonEmpty(GetValue(row, 4), "0"),
                            StartDate = FormatDateForWizard(ParseRowDate(GetValue(row, 5), fechaEfectiva)),
                            EndDate = FormatDateForWizard(ParseRowDate(GetValue(row, 6), fechaEfectiva.AddDays(14))),
                            SalePrice = GetValue(row, 1),
                            Quantity = FirstNonEmpty(GetValue(row, 2), "0")
                        };
                    }).ToList();
                    foreach (int storeID in storeIDs)
                        responses.Add(wizard.Task121(storeID, title, notas, fechaEfectiva, dynamicRows, userID));
                    break;

                case "123":
                    var priceRows = filas.Select(row =>
                    {
                        EN_Item item = items.ByCode[NormalizeLookup(GetValue(row, 0))];
                        decimal newPrice = ParseDecimal(GetValue(row, 1));
                        decimal cost = item.ReplacementCost > 0 ? item.ReplacementCost : item.Cost;
                        decimal utility = cost > 0 ? Math.Round(((newPrice - cost) / cost) * 100m, 2) : item.Utility;
                        return new EN_WizardStructs.TablaItemPriceRegular
                        {
                            ID = item.ID,
                            ItemLookupCode = item.ItemLookupCode,
                            InvoiceDiscount = "0",
                            CustomerDiscount = "0",
                            Cost = FormatDecimalForWizard(cost),
                            Utility = FormatDecimalForWizard(utility)
                        };
                    }).ToList();
                    responses.Add(wizard.Task123(storeIDs, title, notas, fechaEfectiva, priceRows, userID));
                    break;

                default:
                    return new WorksheetCreationResult
                    {
                        Status = false,
                        Message = "La tarea " + tarea.Codigo + " todavia no tiene guardado SQL conectado desde esta pantalla.",
                        InternalMessage = "TaskCode no soportado por CambiosMasivosWizardController."
                    };
            }

            bool ok = responses.Count > 0 && responses.All(x => x != null && x.Status);
            return new WorksheetCreationResult
            {
                Status = ok,
                Message = ok
                    ? "Hoja de trabajo creada correctamente en SQL."
                    : FirstNonEmpty(responses.Where(x => x != null).Select(x => x.Message).FirstOrDefault(), "No se pudo crear la hoja de trabajo en SQL."),
                InternalMessage = String.Join(" | ", responses.Where(x => x != null && !String.IsNullOrWhiteSpace(x.InternalMessage)).Select(x => x.InternalMessage)),
                WorksheetID = GetLatestWorksheetID(title, userID),
                RegisteredRows = filas.Count,
                Stores = storeIDs.Length
            };
        }

        private ResolvedWizardItems ResolveItems(IList<List<string>> filas)
        {
            var result = new ResolvedWizardItems();
            var codes = filas
                .Select(x => GetValue(x, 0))
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (codes.Length == 0)
                return result;

            List<EN_Item> items = new CT_Item().GetByItemLookupCodebyLista(String.Join(",", codes), GetStoresAvailable());
            foreach (EN_Item item in items ?? new List<EN_Item>())
            {
                string key = NormalizeLookup(item.ItemLookupCode);
                if (!String.IsNullOrWhiteSpace(key) && !result.ByCode.ContainsKey(key))
                    result.ByCode.Add(key, item);
            }

            foreach (string code in codes)
            {
                if (!result.ByCode.ContainsKey(NormalizeLookup(code)))
                    result.MissingCodes.Add(code);
            }

            return result;
        }

        private int GetLatestWorksheetID(string title, string userID)
        {
            try
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];
                if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                    return 0;

                using (SqlConnection connection = new SqlConnection(settings.ConnectionString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
SELECT TOP 1 W.ID
FROM dbo.Worksheet W
LEFT JOIN dbo.ExtCentral_Worksheet EW ON EW.WorksheetID = W.ID
WHERE (@Title = '' OR W.Title = @Title OR EW.WorsheetTitle = @Title)
  AND (@UserID = '' OR EW.HQUserID = TRY_CONVERT(INT, @UserID))
ORDER BY W.ID DESC;";
                    command.Parameters.AddWithValue("@Title", title ?? "");
                    command.Parameters.AddWithValue("@UserID", userID ?? "");

                    connection.Open();
                    object value = command.ExecuteScalar();
                    return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
                }
            }
            catch
            {
                return 0;
            }
        }

        private string GetCurrentUserID()
        {
            return Session["USER_AUTOID"] == null ? "0" : Convert.ToString(Session["USER_AUTOID"]);
        }

        private string GetCurrentUserName(int maxLength = 0)
        {
            string name = User != null && User.Identity != null && !String.IsNullOrWhiteSpace(User.Identity.Name)
                ? User.Identity.Name.Trim()
                : "UltraERP";

            return maxLength > 0 && name.Length > maxLength ? name.Substring(0, maxLength) : name;
        }

        private string BuildWizardTitle(CambioMasivoTaskViewModel tarea)
        {
            string title = (tarea.Codigo + "-" + tarea.Nombre + "-" + GetCurrentUserName(18)).Trim();
            return title.Length > 50 ? title.Substring(0, 50) : title;
        }

        private static DateTime ParseRowDate(string value, DateTime fallback)
        {
            if (String.IsNullOrWhiteSpace(value))
                return fallback;

            DateTime date;
            string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy", "yyyy/MM/dd" };
            if (DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return date;

            return DateTime.TryParse(value, out date) ? date : fallback;
        }

        private static decimal ParseDecimal(string value)
        {
            decimal number;
            if (Decimal.TryParse((value ?? "").Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                return number;

            return 0m;
        }

        private static string FormatDecimalForWizard(decimal value)
        {
            return value.ToString("0.####", CultureInfo.InvariantCulture);
        }

        private static string FormatDateForWizard(DateTime value)
        {
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private List<CambioMasivoTaskViewModel> GetTareasSafe()
        {
            try
            {
                var dbTasks = new CT_ExtCentral_WizardList()
                    .GetAll()
                    .Where(x => x != null && x.Estado != 'I')
                    .ToList();

                if (dbTasks.Count == 0)
                    return new List<CambioMasivoTaskViewModel>();

                return dbTasks
                    .Select(MapTaskFromDatabase)
                    .Where(x => x != null)
                    .OrderBy(x => x.Codigo)
                    .ToList();
            }
            catch
            {
                return new List<CambioMasivoTaskViewModel>();
            }
        }

        private static CambioMasivoTaskViewModel MapTaskFromDatabase(EN_ExtCentral_WizardList source)
        {
            if (source == null || String.IsNullOrWhiteSpace(source.Codigo))
                return null;

            var template = GetSupportedTaskTemplate(source.Codigo);
            if (template != null)
            {
                var task = CloneTask(template);
                if (!String.IsNullOrWhiteSpace(source.Descripcion))
                {
                    task.Nombre = source.Descripcion.Trim();
                    task.Descripcion = source.Descripcion.Trim();
                }

                return task;
            }

            return CreateTask(
                source.Codigo.Trim(),
                String.IsNullOrWhiteSpace(source.Descripcion) ? "Tarea " + source.Codigo.Trim() : source.Descripcion.Trim(),
                "Tarea disponible en el catalogo de Wizard.",
                "Wizard",
                410,
                "Use las columnas base para preparar la hoja y revise el resultado antes de crearla.",
                "Codigo de producto",
                "Valor nuevo");
        }

        private StoreCatalogData GetStoreCatalogSafe()
        {
            try
            {
                string storesAvailable = GetStoresAvailable();
                var groups = new CT_StoreGroup().GetAll(storesAvailable);
                var stores = new List<CambioMasivoTiendaViewModel>();
                var viewGroups = new List<CambioMasivoGrupoTiendaViewModel>();

                if (groups != null && groups.Count > 0)
                {
                    foreach (var group in groups)
                    {
                        var groupStores = new CT_Store()
                            .GetAll_By_StoreGroupID(group.ID, storesAvailable)
                            .Select(x => MapStore(x, group.ID))
                            .Where(x => x != null)
                            .ToList();

                        viewGroups.Add(new CambioMasivoGrupoTiendaViewModel
                        {
                            ID = group.ID,
                            Nombre = FirstNonEmpty(group.Description, group.Code, "Grupo " + group.ID),
                            CantidadTiendas = groupStores.Count
                        });

                        stores.AddRange(groupStores);
                    }
                }

                if (stores.Count == 0)
                {
                    stores = new CT_Store()
                        .GetAll("", 0, 0)
                        .Select(x => MapStore(x, 1))
                        .Where(x => x != null)
                        .ToList();

                    viewGroups = new List<CambioMasivoGrupoTiendaViewModel>
                    {
                        new CambioMasivoGrupoTiendaViewModel
                        {
                            ID = 1,
                            Nombre = "Todas las tiendas",
                            CantidadTiendas = stores.Count
                        }
                    };
                }

                stores = stores
                    .GroupBy(x => x.ID)
                    .Select(x => x.First())
                    .OrderBy(x => x.Codigo)
                    .ThenBy(x => x.Nombre)
                    .ToList();

                viewGroups = viewGroups
                    .Where(x => stores.Any(store => store.GrupoID == x.ID))
                    .OrderBy(x => x.Nombre)
                    .ToList();

                return new StoreCatalogData
                {
                    FromSql = stores.Count > 0,
                    Grupos = viewGroups,
                    Tiendas = stores
                };
            }
            catch
            {
                return new StoreCatalogData
                {
                    FromSql = false,
                    Grupos = new List<CambioMasivoGrupoTiendaViewModel>(),
                    Tiendas = new List<CambioMasivoTiendaViewModel>()
                };
            }
        }

        private string GetStoresAvailable()
        {
            string dataStoreCode = ConfigurationManager.AppSettings["DataStoreCode"] ?? "uerp-store";
            EN_SC_DataAccess[] dataAccess = Session["USER_DATAACCESS"] as EN_SC_DataAccess[];

            if (dataAccess == null || dataAccess.Length == 0)
                return "%";

            EN_SC_DataAccess[] storesAccess = dataAccess.Where(x => x.Code == dataStoreCode).ToArray();
            if (storesAccess.Length == 0 || storesAccess.Any(x => x.EnableAll))
                return "%";

            return String.Join(",", storesAccess.Where(x => !String.IsNullOrWhiteSpace(x.DataIDs)).Select(x => x.DataIDs));
        }

        private static string BuildNotes(string notes, string fileName, string separator, int rows)
        {
            var parts = new List<string>();
            if (!String.IsNullOrWhiteSpace(notes))
                parts.Add(notes.Trim());

            parts.Add("Generado desde Cambios Masivos Wizard.");
            parts.Add("Filas cargadas: " + rows + ".");

            if (!String.IsNullOrWhiteSpace(fileName))
                parts.Add("Archivo fuente: " + fileName.Trim() + ".");

            if (!String.IsNullOrWhiteSpace(separator))
                parts.Add("Separador usado: " + separator.Trim() + ".");

            return String.Join(Environment.NewLine, parts);
        }

        private static List<List<string>> NormalizeRows(IEnumerable<CambioMasivoFilaRequest> rows)
        {
            if (rows == null)
                return new List<List<string>>();

            return rows
                .Where(x => x != null && x.TieneContenido)
                .Select(x => x.Values == null
                    ? new List<string>()
                    : x.Values.Select(value => (value ?? String.Empty).Trim()).ToList())
                .ToList();
        }

        private static string ValidateRows(CambioMasivoTaskViewModel tarea, IList<List<string>> rows)
        {
            if (tarea == null)
                return "La tarea seleccionada no existe.";

            if (rows == null || rows.Count == 0)
                return "No hay filas para procesar.";

            var expectedColumns = tarea.ColumnasArchivo == null ? 0 : tarea.ColumnasArchivo.Count;
            var invalidCount = rows.Any(x => x.Count != expectedColumns);
            if (invalidCount)
                return "La plantilla cargada no coincide con las columnas esperadas para " + tarea.Nombre + ".";

            if (rows.Any(x => String.IsNullOrWhiteSpace(GetValue(x, 0))))
                return "Cada fila debe incluir un codigo de producto en la primera columna.";

            var duplicates = rows
                .GroupBy(x => (GetValue(x, 0) ?? String.Empty).ToUpperInvariant())
                .Where(x => !String.IsNullOrWhiteSpace(x.Key) && x.Count() > 1)
                .Select(x => x.Key)
                .Take(3)
                .ToList();

            if (duplicates.Count > 0)
                return "Hay codigos repetidos en el archivo: " + String.Join(", ", duplicates) + ".";

            return String.Empty;
        }

        private static List<HojaTrabajoContenidoViewModel> BuildWorksheetContent(CambioMasivoTaskViewModel tarea, IList<CambioMasivoTiendaViewModel> stores, IList<List<string>> rows, DateTime fechaEfectiva)
        {
            var tiendaTexto = stores == null || stores.Count == 0
                ? "Sin tienda"
                : (stores.Count == 1 ? stores[0].Nombre : "Varias tiendas");

            return rows.Take(120).Select(row => new HojaTrabajoContenidoViewModel
            {
                Tienda = tiendaTexto,
                Disponible = true,
                CodigoArticulo = GetValue(row, 0),
                Descripcion = BuildDescription(tarea, row),
                DescripcionExtendida = BuildExtendedDescription(tarea, row),
                PrecioLiquidacion = ExtractDecimalValue(tarea, row, "precio"),
                InicioLiquidacion = tarea.Codigo == "121" ? fechaEfectiva.Date : (DateTime?)null,
                FinLiquidacion = tarea.Codigo == "121" ? fechaEfectiva.Date.AddDays(14) : (DateTime?)null,
                LimiteInferior = ExtractDecimalValue(tarea, row, "cantidad"),
                LimiteSuperior = null,
                PrecioCantidad = ExtractDecimalValue(tarea, row, "cliente"),
                CantidadLiquidacion = ExtractDecimalValue(tarea, row, "oferta"),
                Impuesto = BuildTaxHint(tarea, row)
            }).ToList();
        }

        private static string BuildDescription(CambioMasivoTaskViewModel tarea, IList<string> row)
        {
            var secondValue = GetValue(row, 1);
            return String.IsNullOrWhiteSpace(secondValue) ? tarea.Nombre : secondValue;
        }

        private static string BuildExtendedDescription(CambioMasivoTaskViewModel tarea, IList<string> row)
        {
            if (tarea == null || tarea.ColumnasArchivo == null || row == null)
                return String.Empty;

            var pairs = new List<string>();
            for (var i = 1; i < tarea.ColumnasArchivo.Count && i < row.Count; i++)
            {
                var value = GetValue(row, i);
                if (String.IsNullOrWhiteSpace(value))
                    continue;

                pairs.Add(tarea.ColumnasArchivo[i] + ": " + value);
                if (pairs.Count == 3)
                    break;
            }

            return pairs.Count == 0 ? tarea.Resumen : String.Join(" | ", pairs);
        }

        private static string BuildTaxHint(CambioMasivoTaskViewModel tarea, IList<string> row)
        {
            if (tarea == null)
                return String.Empty;

            if (tarea.Codigo == "104")
                return GetValue(row, 2);

            if (tarea.Codigo == "110")
                return "CABYS";

            if (tarea.Codigo == "201")
                return GetValue(row, 1);

            return tarea.Categoria;
        }

        private static decimal? ExtractDecimalValue(CambioMasivoTaskViewModel tarea, IList<string> row, string keyword)
        {
            if (tarea == null || tarea.ColumnasArchivo == null || row == null)
                return null;

            for (var i = 0; i < tarea.ColumnasArchivo.Count && i < row.Count; i++)
            {
                if (tarea.ColumnasArchivo[i].IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                decimal number;
                if (Decimal.TryParse((row[i] ?? String.Empty).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                    return number;
            }

            return null;
        }

        private static string GetValue(IList<string> row, int index)
        {
            if (row == null || index < 0 || index >= row.Count)
                return String.Empty;

            return row[index] ?? String.Empty;
        }

        private static CambioMasivoTaskViewModel CloneTask(CambioMasivoTaskViewModel task)
        {
            return new CambioMasivoTaskViewModel
            {
                Codigo = task.Codigo,
                Nombre = task.Nombre,
                Descripcion = task.Descripcion,
                Categoria = task.Categoria,
                Resumen = task.Resumen,
                EstiloHojaID = task.EstiloHojaID,
                ColumnasArchivo = task.ColumnasArchivo == null ? new List<string>() : task.ColumnasArchivo.ToList()
            };
        }

        private static CambioMasivoTiendaViewModel MapStore(EN_Store store, int groupID)
        {
            if (store == null || store.IDS <= 0)
                return null;

            return new CambioMasivoTiendaViewModel
            {
                ID = store.IDS,
                GrupoID = groupID,
                Codigo = store.CodeS,
                Nombre = FirstNonEmpty(store.NameS, store.CodeS, "Tienda " + store.IDS),
                Ciudad = ""
            };
        }

        private static string FirstNonEmpty(params string[] values)
        {
            if (values == null)
                return "";

            return values.FirstOrDefault(x => !String.IsNullOrWhiteSpace(x)) ?? "";
        }

        private static string NormalizeLookup(string value)
        {
            return (value ?? "").Trim().ToUpperInvariant();
        }

        private static CambioMasivoTaskViewModel CreateTask(string codigo, string nombre, string descripcion, string categoria, int estiloHojaID, string resumen, params string[] columnas)
        {
            return new CambioMasivoTaskViewModel
            {
                Codigo = codigo,
                Nombre = nombre,
                Descripcion = descripcion,
                Categoria = categoria,
                Resumen = resumen,
                EstiloHojaID = estiloHojaID,
                ColumnasArchivo = columnas == null ? new List<string>() : columnas.ToList()
            };
        }

        private static CambioMasivoTaskViewModel GetSupportedTaskTemplate(string codigo)
        {
            switch ((codigo ?? "").Trim())
            {
                case "104":
                    return CreateTask("104", "Cambio de impuestos", "Actualice el impuesto de venta para multiples articulos en una sola solicitud.", "Fiscal", 320, "Recomendado para ajustes tributarios por categoria o reforma.", "Codigo de producto", "Impuesto actual", "Impuesto nuevo");
                case "107":
                    return CreateTask("107", "Cambio de descripcion corta", "Renueve la descripcion corta visible en listado y punto de venta.", "Catalogo", 410, "Util para estandarizar nombres comerciales y marcas visibles.", "Codigo de producto", "Descripcion corta nueva");
                case "110":
                    return CreateTask("110", "Cambio de CABYS", "Corrija o sustituya la clasificacion CABYS de varios articulos.", "Catalogo", 410, "Pensado para depuracion regulatoria y homologacion contable.", "Codigo de producto", "CABYS nuevo");
                case "121":
                    return CreateTask("121", "Precio dinamico", "Prepare cambios de precio promocional con fechas y descuentos.", "Precios", 251, "Genera una hoja lista para aprobacion comercial.", "Codigo de producto", "Precio oferta", "Cantidad oferta", "Desc. factura (%)", "Desc. cliente (%)", "Fecha inicio", "Fecha fin");
                case "123":
                    return CreateTask("123", "Precio regular", "Actualice el precio regular base de un grupo de articulos.", "Precios", 251, "Ideal para cambios por proveedor, costo o estrategia comercial.", "Codigo de producto", "Precio regular nuevo");
                case "201":
                    return CreateTask("201", "Cambio de propiedades", "Cambie propiedades extendidas como origen, registro o atributos logisticos.", "Propiedades", 410, "Complementa la pantalla de Propiedades de Articulos cuando el cambio viene por lote.", "Codigo de producto", "Propiedad", "Valor nuevo");
                default:
                    return null;
            }
        }

        private class StoreCatalogData
        {
            public StoreCatalogData()
            {
                Grupos = new List<CambioMasivoGrupoTiendaViewModel>();
                Tiendas = new List<CambioMasivoTiendaViewModel>();
            }

            public bool FromSql { get; set; }
            public List<CambioMasivoGrupoTiendaViewModel> Grupos { get; set; }
            public List<CambioMasivoTiendaViewModel> Tiendas { get; set; }
        }

        private class ResolvedWizardItems
        {
            public ResolvedWizardItems()
            {
                ByCode = new Dictionary<string, EN_Item>(StringComparer.OrdinalIgnoreCase);
                MissingCodes = new List<string>();
            }

            public Dictionary<string, EN_Item> ByCode { get; private set; }
            public List<string> MissingCodes { get; private set; }
        }

        private class WorksheetCreationResult
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public string InternalMessage { get; set; }
            public int WorksheetID { get; set; }
            public int RegisteredRows { get; set; }
            public int Stores { get; set; }
        }
    }
}
