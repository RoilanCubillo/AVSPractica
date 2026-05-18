using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static readonly List<CambioMasivoTaskViewModel> TareasBase = new List<CambioMasivoTaskViewModel>
        {
            CreateTask("104", "Cambio de impuestos", "Actualice el impuesto de venta para multiples articulos en una sola solicitud.", "Fiscal", 320, "Recomendado para ajustes tributarios por categoria o reforma.", "Codigo de producto", "Impuesto actual", "Impuesto nuevo"),
            CreateTask("107", "Cambio de descripcion corta", "Renueve la descripcion corta visible en listado y punto de venta.", "Catalogo", 410, "Util para estandarizar nombres comerciales y marcas visibles.", "Codigo de producto", "Descripcion corta nueva"),
            CreateTask("110", "Cambio de CABYS", "Corrija o sustituya la clasificacion CABYS de varios articulos.", "Catalogo", 410, "Pensado para depuracion regulatoria y homologacion contable.", "Codigo de producto", "CABYS nuevo"),
            CreateTask("121", "Precio dinamico", "Prepare cambios de precio promocional con fechas y descuentos.", "Precios", 251, "Genera una hoja lista para aprobacion comercial.", "Codigo de producto", "Precio oferta", "Cantidad oferta", "Desc. factura (%)", "Desc. cliente (%)", "Fecha inicio", "Fecha fin"),
            CreateTask("123", "Precio regular", "Actualice el precio regular base de un grupo de articulos.", "Precios", 251, "Ideal para cambios por proveedor, costo o estrategia comercial.", "Codigo de producto", "Precio regular nuevo"),
            CreateTask("201", "Cambio de propiedades", "Cambie propiedades extendidas como origen, registro o atributos logisticos.", "Propiedades", 410, "Complementa la pantalla de Propiedades de Articulos cuando el cambio viene por lote.", "Codigo de producto", "Propiedad", "Valor nuevo")
        };

        private static readonly List<CambioMasivoTiendaViewModel> TiendasDemo = new List<CambioMasivoTiendaViewModel>
        {
            CreateStore(1, 1, "SJ-CTR", "San Jose Centro", "San Jose"),
            CreateStore(2, 1, "HER-PLZ", "Heredia Plaza", "Heredia"),
            CreateStore(3, 1, "CRT-EST", "Cartago Este", "Cartago"),
            CreateStore(4, 2, "ESC-GAM", "Escazu", "San Jose"),
            CreateStore(5, 2, "SBN-COR", "Sabana", "San Jose"),
            CreateStore(6, 2, "SPD-UNI", "San Pedro", "San Jose"),
            CreateStore(7, 3, "LBR-OCC", "Liberia", "Guanacaste"),
            CreateStore(8, 3, "LIM-CAR", "Limon Centro", "Limon"),
            CreateStore(9, 3, "PZM-SUR", "Perez Zeledon", "San Jose")
        };

        private static readonly List<CambioMasivoGrupoTiendaViewModel> GruposDemo = new List<CambioMasivoGrupoTiendaViewModel>
        {
            new CambioMasivoGrupoTiendaViewModel { ID = 1, Nombre = "Valle Central", CantidadTiendas = 3 },
            new CambioMasivoGrupoTiendaViewModel { ID = 2, Nombre = "GAM Premium", CantidadTiendas = 3 },
            new CambioMasivoGrupoTiendaViewModel { ID = 3, Nombre = "Regional", CantidadTiendas = 3 }
        };

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

            ViewBag.CambiosMasivosDataSource = catalog.FromSql ? "SQL" : "Demo";
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

            var now = DateTime.Now;
            var usuario = User != null && User.Identity != null && User.Identity.IsAuthenticated && !String.IsNullOrWhiteSpace(User.Identity.Name)
                ? User.Identity.Name
                : "Usuario UltraERP";
            var tiendasHoja = tiendasSeleccionadas
                .Select(x => new HojaTrabajoTiendaViewModel
                {
                    TiendaID = x.ID,
                    TiendaNombre = x.Nombre,
                    EstadoID = 1,
                    FechaProcesado = null
                })
                .ToList();

            var contenido = BuildWorksheetContent(tarea, tiendasSeleccionadas, filas, fechaEfectiva);
            var notas = BuildNotes(request.Notes, request.FileName, request.Separator, filas.Count);

            var hoja = new HojaTrabajoViewModel
            {
                EstiloID = tarea.EstiloHojaID,
                EstadoID = 1,
                Titulo = tarea.Nombre + " - " + now.ToString("dd/MM HH:mm"),
                Notas = notas,
                TareaCodigo = tarea.Codigo,
                TareaDescripcion = tarea.Nombre,
                Usuario = usuario,
                FechaCreacion = now,
                FechaEfectiva = fechaEfectiva.Date.AddHours(22),
                ArchivoID = 1,
                Tiendas = tiendasHoja,
                Historial = new List<HojaTrabajoHistorialViewModel>
                {
                    new HojaTrabajoHistorialViewModel
                    {
                        EstadoID = 1,
                        Tienda = "Oficina Central",
                        FechaHora = now,
                        Comentario = "Hoja creada desde Cambios Masivos Wizard."
                    },
                    new HojaTrabajoHistorialViewModel
                    {
                        EstadoID = 1,
                        Tienda = "Oficina Central",
                        FechaHora = now.AddMinutes(1),
                        Comentario = "Pendiente de aprobacion comercial."
                    }
                },
                Contenido = contenido
            };

            var registrada = HojasTrabajoController.RegistrarHoja(hoja);
            return Json(new JsonResponse("", "Hoja de trabajo creada correctamente.", new
            {
                WorksheetID = registrada.ID,
                RedirectUrl = Url.Action("Inicio", "HojasTrabajo"),
                RegisteredRows = registrada.CantidadContenido,
                Stores = registrada.CantidadTiendas
            }, true));
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
                    return TareasBase.Select(CloneTask).ToList();

                return dbTasks
                    .Select(MapTaskFromDatabase)
                    .Where(x => x != null)
                    .OrderBy(x => x.Codigo)
                    .ToList();
            }
            catch
            {
                return TareasBase.Select(CloneTask).ToList();
            }
        }

        private static CambioMasivoTaskViewModel MapTaskFromDatabase(EN_ExtCentral_WizardList source)
        {
            if (source == null || String.IsNullOrWhiteSpace(source.Codigo))
                return null;

            var template = TareasBase.FirstOrDefault(x => String.Equals(x.Codigo, source.Codigo, StringComparison.OrdinalIgnoreCase));
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
                    Grupos = GruposDemo.Select(CloneGroup).ToList(),
                    Tiendas = TiendasDemo.Select(CloneStore).ToList()
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

        private static CambioMasivoGrupoTiendaViewModel CloneGroup(CambioMasivoGrupoTiendaViewModel group)
        {
            return new CambioMasivoGrupoTiendaViewModel
            {
                ID = group.ID,
                Nombre = group.Nombre,
                CantidadTiendas = group.CantidadTiendas
            };
        }

        private static CambioMasivoTiendaViewModel CloneStore(CambioMasivoTiendaViewModel store)
        {
            return new CambioMasivoTiendaViewModel
            {
                ID = store.ID,
                GrupoID = store.GrupoID,
                Codigo = store.Codigo,
                Nombre = store.Nombre,
                Ciudad = store.Ciudad
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

        private static CambioMasivoTiendaViewModel CreateStore(int id, int grupoID, string codigo, string nombre, string ciudad)
        {
            return new CambioMasivoTiendaViewModel
            {
                ID = id,
                GrupoID = grupoID,
                Codigo = codigo,
                Nombre = nombre,
                Ciudad = ciudad
            };
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
    }
}
