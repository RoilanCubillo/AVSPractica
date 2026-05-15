using Security.EntitiesAVS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using UltraERP.BusinessEntities;
using UltraERP.BusinessLogic;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class ArticulosController : Controller
    {
        private const int DefaultResultCount = 500;

        private static readonly List<string> Unidades = new List<string> { "UND", "KG", "CJ", "GAL", "L", "QQ", "BQ" };
        private static readonly List<string> Proveedores = new List<string> { "Cooperativa Dos Pinos", "Cafe Britt Costa Rica", "Irex de Costa Rica", "Distribuidora San Jose", "Central de Abarrotes Cartago" };
        private static readonly List<string> Bodegas = new List<string> { "Bodega Central Heredia", "Sucursal Escazu", "Sucursal Sabana", "Sucursal Cartago" };

        private static readonly List<FamiliaCatalogo> Familias = new List<FamiliaCatalogo>
        {
            new FamiliaCatalogo(1, "ABAR", "Abarrotes ticos"),
            new FamiliaCatalogo(2, "LACT", "Lacteos y frescos"),
            new FamiliaCatalogo(3, "BEB", "Bebidas y cafe"),
            new FamiliaCatalogo(4, "LIMP", "Limpieza y hogar")
        };

        private static readonly List<DepartamentoCatalogo> Departamentos = new List<DepartamentoCatalogo>
        {
            new DepartamentoCatalogo(1, 1, "GRANOS", "Granos basicos"),
            new DepartamentoCatalogo(2, 1, "SALSAS", "Salsas y condimentos"),
            new DepartamentoCatalogo(3, 2, "REFRI", "Refrigerados"),
            new DepartamentoCatalogo(4, 3, "CAFE", "Cafe y bebidas"),
            new DepartamentoCatalogo(5, 4, "HOGAR", "Cuidado del hogar")
        };

        private static readonly List<CategoriaCatalogo> Categorias = new List<CategoriaCatalogo>
        {
            new CategoriaCatalogo(1, 1, "ARROZ", "Arroces"),
            new CategoriaCatalogo(2, 1, "FRIJ", "Frijoles"),
            new CategoriaCatalogo(3, 2, "SALT", "Salsas ticas"),
            new CategoriaCatalogo(4, 3, "LECHE", "Leches y lacteos"),
            new CategoriaCatalogo(5, 4, "CAFCR", "Cafe costarricense"),
            new CategoriaCatalogo(6, 5, "DETER", "Detergentes")
        };

        private static readonly List<SubCategoriaCatalogo> SubCategorias = new List<SubCategoriaCatalogo>
        {
            new SubCategoriaCatalogo(1, 1, "GRANO", "Arroz grano entero"),
            new SubCategoriaCatalogo(2, 1, "PRECOC", "Arroz precocido"),
            new SubCategoriaCatalogo(3, 2, "ROJOS", "Frijoles rojos"),
            new SubCategoriaCatalogo(4, 3, "MESA", "Salsas de mesa"),
            new SubCategoriaCatalogo(5, 4, "FLUIDA", "Leche fluida"),
            new SubCategoriaCatalogo(6, 5, "MOLIDO", "Cafe molido"),
            new SubCategoriaCatalogo(7, 6, "POLVO", "Detergente en polvo")
        };

        public ActionResult Inicio()
        {
            try
            {
                var model = GetArticulosFromDatabase("", DefaultResultCount)
                    .OrderBy(x => x.Codigo)
                    .ToList();

                ViewBag.ArticulosDataSource = "SQL";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ArticuloError"] = "No se pudo cargar articulos desde SQL: " + ex.Message;
                ViewBag.ArticulosDataSource = "SQL";
                return View(Enumerable.Empty<ArticuloViewModel>());
            }
        }

        public ActionResult Registro(int? id)
        {
            ArticuloViewModel model = null;

            if (id.HasValue)
            {
                try
                {
                    model = GetArticuloById(id.Value);
                    if (model == null)
                        TempData["ArticuloError"] = "No se encontro el articulo en SQL.";
                }
                catch (Exception ex)
                {
                    TempData["ArticuloError"] = "No se pudo leer el articulo desde SQL: " + ex.Message;
                }
            }

            CatalogData catalog = GetCatalogDataSafe();
            PrepareCatalogs(catalog);
            return View(model ?? CreateNewArticulo(catalog));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(ArticuloViewModel model)
        {
            CatalogData catalog = GetCatalogDataSafe();

            Normalize(model);
            ApplyClasificacion(model, catalog);
            ValidateArticulo(model, catalog);

            if (!ModelState.IsValid)
            {
                PrepareCatalogs(catalog);
                return View(model);
            }

            try
            {
                EN_Item item = BuildItem(model, catalog);
                string title = "AVS WEB:" + model.Codigo + "-" + GetCurrentUser(42 - model.Codigo.Length);
                Dictionary<string, object> result = new CT_Item().Save(item, title, GetCurrentUserID());
                string response = result.ContainsKey("RESPUESTA") ? Convert.ToString(result["RESPUESTA"]) : "";

                if (response.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ModelState.AddModelError("", "SQL rechazo el guardado del articulo: " + response);
                    PrepareCatalogs(catalog);
                    return View(model);
                }

                TempData["ArticuloMessage"] = model.ID > 0 ? "Articulo actualizado correctamente en SQL." : "Articulo creado correctamente en SQL.";
                return RedirectToAction("Inicio");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar el articulo en SQL. " + ex.Message);
                PrepareCatalogs(catalog);
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            return Json(new JsonResponse(
                "Pendiente de migracion",
                "El listado de articulos ya consulta SQL. La activacion e inactivacion se conectara cuando migremos el procedimiento de estado.",
                null,
                false));
        }

        private List<ArticuloViewModel> GetArticulosFromDatabase(string searchValue, int take)
        {
            int recordLimit = take <= 0 ? 10000 : take;

            return new CT_Item()
                .GetDynamicList(searchValue ?? "", 1, "asc", 0, recordLimit, null, null, null, null, null, GetStoresAvailable())
                .Select(MapArticulo)
                .ToList();
        }

        private ArticuloViewModel GetArticuloById(int id)
        {
            return GetArticulosFromDatabase("", 0).FirstOrDefault(x => x.ID == id);
        }

        private static ArticuloViewModel MapArticulo(EN_Item item)
        {
            decimal costo = item.ReplacementCost > 0 ? item.ReplacementCost : item.Cost;
            decimal precio = item.Price > 0 ? item.Price : CalculatePrice(costo, item.Utility);

            return new ArticuloViewModel
            {
                ID = item.ID,
                Codigo = item.ItemLookupCode,
                Descripcion = item.Description,
                DescripcionExtendida = item.ExtendedDescription,
                UnidadMedida = item.UnitOfMeasure,
                FamiliaID = item.FamilyID,
                DepartamentoID = item.DepartmentID,
                CategoriaID = item.CategoryID,
                SubCategoriaID = item.SubCategoryID,
                Familia = JoinCodeName(item.FamilyCode, item.FamilyName),
                Departamento = JoinCodeName(item.DepartmentCode, item.DepartmentName),
                Categoria = JoinCodeName(item.CategoryCode, item.CategoryName),
                SubCategoria = JoinCodeName(item.SubCategoryCode, item.SubCategoryName),
                Proveedor = !String.IsNullOrWhiteSpace(item.SupplierName) ? item.SupplierName : item.SupplierCode,
                Bodega = item.StoresNameSelected,
                Costo = costo,
                PrecioVenta = precio,
                ImpuestoPorcentaje = Convert.ToDecimal(item.TaxPercentage),
                Existencia = Convert.ToDecimal(item.Quantity),
                ExistenciaMinima = 0,
                ExistenciaMaxima = 0,
                Activo = !item.Inactive,
                Inventariable = true,
                Exento = item.TaxPercentage <= 0,
                UsuarioCrea = "SQL",
                FechaCrea = item.DateCreated == DateTime.MinValue || item.DateCreated.Year <= 1900 ? DateTime.Now : item.DateCreated,
                FechaModifica = item.LastUpdated
            };
        }

        private static decimal CalculatePrice(decimal cost, decimal utility)
        {
            if (cost <= 0)
                return 0;

            return Math.Round(cost + (cost * utility / 100), 2);
        }

        private EN_Item BuildItem(ArticuloViewModel model, CatalogData catalog)
        {
            EN_Item item = new CT_Item().GetByItemLookupCode(model.Codigo, GetStoresAvailable()) ?? new EN_Item();

            item.ID = model.ID;
            item.ItemLookupCode = model.Codigo;
            item.Description = model.Descripcion;
            item.ExtendedDescription = model.DescripcionExtendida;
            item.UnitOfMeasure = model.UnidadMedida;
            item.FamilyID = model.FamiliaID;
            item.DepartmentID = model.DepartamentoID;
            item.CategoryID = model.CategoriaID;
            item.SubCategoryID = model.SubCategoriaID;
            item.ReplacementCost = model.Costo;
            item.Cost = model.Costo;
            item.Price = model.PrecioVenta;
            item.Utility = model.Costo > 0 ? Math.Round(((model.PrecioVenta - model.Costo) / model.Costo) * 100, 2) : item.Utility;
            item.SupplierID = ResolveSupplierID(model.Proveedor, catalog, item.SupplierID);
            item.TaxID = ResolveTaxID(model.ImpuestoPorcentaje, catalog, item.TaxID);
            item.StoresSelected = String.IsNullOrWhiteSpace(item.StoresSelected) ? GetStoresAvailable() : item.StoresSelected;

            return item;
        }

        private void ValidateArticulo(ArticuloViewModel model, CatalogData catalog)
        {
            if (model == null)
                return;

            if (!catalog.Familias.Any(x => x.ID == model.FamiliaID))
                ModelState.AddModelError("FamiliaID", "Seleccione una familia valida.");

            var departamento = catalog.Departamentos.FirstOrDefault(x => x.ID == model.DepartamentoID);
            if (departamento == null || departamento.FamiliaID != model.FamiliaID)
                ModelState.AddModelError("DepartamentoID", "Seleccione un departamento valido para la familia.");

            var categoria = catalog.Categorias.FirstOrDefault(x => x.ID == model.CategoriaID);
            if (categoria == null || categoria.DepartamentoID != model.DepartamentoID)
                ModelState.AddModelError("CategoriaID", "Seleccione una categoria valida para el departamento.");

            var subCategoria = catalog.SubCategorias.FirstOrDefault(x => x.ID == model.SubCategoriaID);
            if (subCategoria == null || subCategoria.CategoriaID != model.CategoriaID)
                ModelState.AddModelError("SubCategoriaID", "Seleccione una subcategoria valida para la categoria.");

            try
            {
                EN_Item existing = new CT_Item().GetByItemLookupCode(model.Codigo, GetStoresAvailable());
                if (existing != null && existing.ID != model.ID)
                    ModelState.AddModelError("Codigo", "Ya existe un articulo con este codigo.");
            }
            catch
            {
                ModelState.AddModelError("", "No se pudo validar el codigo del articulo contra SQL.");
            }

            if (model.ExistenciaMaxima > 0 && model.ExistenciaMinima > model.ExistenciaMaxima)
                ModelState.AddModelError("ExistenciaMinima", "La existencia minima no puede ser mayor que la maxima.");

            if (model.Exento)
                model.ImpuestoPorcentaje = 0;
        }

        private static void Normalize(ArticuloViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.Descripcion = (model.Descripcion ?? "").Trim();
            model.DescripcionExtendida = (model.DescripcionExtendida ?? "").Trim();
            model.UnidadMedida = (model.UnidadMedida ?? "").Trim().ToUpperInvariant();
            model.Proveedor = (model.Proveedor ?? "").Trim();
            model.Bodega = (model.Bodega ?? "").Trim();
        }

        private static void ApplyClasificacion(ArticuloViewModel model, CatalogData catalog)
        {
            if (model == null)
                return;

            var familia = catalog.Familias.FirstOrDefault(x => x.ID == model.FamiliaID);
            var departamento = catalog.Departamentos.FirstOrDefault(x => x.ID == model.DepartamentoID);
            var categoria = catalog.Categorias.FirstOrDefault(x => x.ID == model.CategoriaID);
            var subCategoria = catalog.SubCategorias.FirstOrDefault(x => x.ID == model.SubCategoriaID);

            model.Familia = familia == null ? "" : familia.Texto;
            model.Departamento = departamento == null ? "" : departamento.Texto;
            model.Categoria = categoria == null ? "" : categoria.Texto;
            model.SubCategoria = subCategoria == null ? "" : subCategoria.Texto;
        }

        private void PrepareCatalogs(CatalogData catalog)
        {
            ViewBag.Unidades = ToSelectList(catalog.Unidades);
            ViewBag.Proveedores = ToSelectList(catalog.Proveedores.Select(x => x.Name).Where(x => !String.IsNullOrWhiteSpace(x)));
            ViewBag.Bodegas = ToSelectList(catalog.Bodegas.Select(x => x.NameS).Where(x => !String.IsNullOrWhiteSpace(x)));
            ViewBag.Familias = catalog.Familias.Select(x => new SelectListItem { Text = x.Texto, Value = x.ID.ToString() }).ToList();
            ViewBag.Departamentos = catalog.Departamentos.Select(x => new SelectListItem { Text = x.Texto, Value = x.ID.ToString() }).ToList();
            ViewBag.Categorias = catalog.Categorias.Select(x => new SelectListItem { Text = x.Texto, Value = x.ID.ToString() }).ToList();
            ViewBag.SubCategorias = catalog.SubCategorias.Select(x => new SelectListItem { Text = x.Texto, Value = x.ID.ToString() }).ToList();
            ViewBag.ClasificacionJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                departamentos = catalog.Departamentos,
                categorias = catalog.Categorias,
                subCategorias = catalog.SubCategorias
            });
        }

        private CatalogData GetCatalogDataSafe()
        {
            try
            {
                string storesAvailable = GetStoresAvailable();
                return new CatalogData
                {
                    Unidades = new CT_UOM().GetAllByInactive(false).Select(x => x.Code).Where(x => !String.IsNullOrWhiteSpace(x)).ToList(),
                    Proveedores = new CT_Supplier().GetAll(storesAvailable, "", 0, 0),
                    Bodegas = new CT_Store().GetAll("", 0, 0),
                    Familias = new CT_ExtCentral_Family().GetAll("", 0, 0).Select(x => new FamiliaCatalogo(x.ID, x.Code, x.Name)).ToList(),
                    Departamentos = new CT_Department().GetAll("", 0, 0).Select(x => new DepartamentoCatalogo(x.ID, x.FamilyID, x.Code, x.Name)).ToList(),
                    Categorias = new CT_Category().GetAll("", 0, 0).Select(x => new CategoriaCatalogo(x.ID, x.DepartmentID, x.Code, x.Name)).ToList(),
                    SubCategorias = new CT_ExtCentral_SubCategory().GetAll("", 0, 0).Select(x => new SubCategoriaCatalogo(x.ID, x.CategoryID, x.Code, x.Description)).ToList(),
                    Impuestos = new CT_Tax().GetAll("", 0, 0)
                };
            }
            catch
            {
                return new CatalogData
                {
                    Unidades = Unidades,
                    Proveedores = Proveedores.Select((x, i) => new EN_Supplier(i + 1, x, x)).ToList(),
                    Bodegas = Bodegas.Select((x, i) => new EN_Store(i + 1, x, x)).ToList(),
                    Familias = Familias,
                    Departamentos = Departamentos,
                    Categorias = Categorias,
                    SubCategorias = SubCategorias,
                    Impuestos = new List<EN_Tax>()
                };
            }
        }

        private static IEnumerable<SelectListItem> ToSelectList(IEnumerable<string> values)
        {
            return values.Select(x => new SelectListItem { Text = x, Value = x });
        }

        private ArticuloViewModel CreateNewArticulo(CatalogData catalog)
        {
            var familia = catalog.Familias.FirstOrDefault();
            var departamento = catalog.Departamentos.FirstOrDefault(x => familia != null && x.FamiliaID == familia.ID) ?? catalog.Departamentos.FirstOrDefault();
            var categoria = catalog.Categorias.FirstOrDefault(x => departamento != null && x.DepartamentoID == departamento.ID) ?? catalog.Categorias.FirstOrDefault();
            var subCategoria = catalog.SubCategorias.FirstOrDefault(x => categoria != null && x.CategoriaID == categoria.ID) ?? catalog.SubCategorias.FirstOrDefault();

            var model = new ArticuloViewModel
            {
                Activo = true,
                Inventariable = true,
                UnidadMedida = catalog.Unidades.FirstOrDefault() ?? "UND",
                FamiliaID = familia == null ? 0 : familia.ID,
                DepartamentoID = departamento == null ? 0 : departamento.ID,
                CategoriaID = categoria == null ? 0 : categoria.ID,
                SubCategoriaID = subCategoria == null ? 0 : subCategoria.ID,
                Proveedor = catalog.Proveedores.Select(x => x.Name).FirstOrDefault() ?? "",
                Bodega = catalog.Bodegas.Select(x => x.NameS).FirstOrDefault() ?? "",
                ImpuestoPorcentaje = 13m,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };

            ApplyClasificacion(model, catalog);
            return model;
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

        private string GetCurrentUser(int maxLength = 0)
        {
            string name = User != null && User.Identity != null && !String.IsNullOrWhiteSpace(User.Identity.Name)
                ? User.Identity.Name
                : "Soporte";

            return maxLength > 0 && name.Length > maxLength ? name.Substring(0, maxLength) : name;
        }

        private string GetCurrentUserID()
        {
            return Session["USER_AUTOID"] == null ? "" : Convert.ToString(Session["USER_AUTOID"]);
        }

        private static int ResolveSupplierID(string supplierName, CatalogData catalog, int currentSupplierID)
        {
            EN_Supplier supplier = catalog.Proveedores.FirstOrDefault(x =>
                String.Equals(x.Name, supplierName, StringComparison.OrdinalIgnoreCase) ||
                String.Equals(x.Code, supplierName, StringComparison.OrdinalIgnoreCase));

            return supplier == null ? currentSupplierID : supplier.ID;
        }

        private static int ResolveTaxID(decimal percentage, CatalogData catalog, int currentTaxID)
        {
            EN_Tax tax = catalog.Impuestos.FirstOrDefault(x => Math.Abs(Convert.ToDecimal(x.Percentage) - percentage) < 0.01m);
            return tax == null ? currentTaxID : tax.ID;
        }

        private static string JoinCodeName(string code, string name)
        {
            if (String.IsNullOrWhiteSpace(code))
                return name ?? "";

            if (String.IsNullOrWhiteSpace(name))
                return code;

            return code + " - " + name;
        }

        private static ArticuloViewModel Clone(ArticuloViewModel source)
        {
            if (source == null)
                return null;

            return new ArticuloViewModel
            {
                ID = source.ID,
                Codigo = source.Codigo,
                Descripcion = source.Descripcion,
                DescripcionExtendida = source.DescripcionExtendida,
                UnidadMedida = source.UnidadMedida,
                FamiliaID = source.FamiliaID,
                DepartamentoID = source.DepartamentoID,
                CategoriaID = source.CategoriaID,
                SubCategoriaID = source.SubCategoriaID,
                Familia = source.Familia,
                Departamento = source.Departamento,
                Categoria = source.Categoria,
                SubCategoria = source.SubCategoria,
                Proveedor = source.Proveedor,
                Bodega = source.Bodega,
                Costo = source.Costo,
                PrecioVenta = source.PrecioVenta,
                ImpuestoPorcentaje = source.ImpuestoPorcentaje,
                Existencia = source.Existencia,
                ExistenciaMinima = source.ExistenciaMinima,
                ExistenciaMaxima = source.ExistenciaMaxima,
                Activo = source.Activo,
                Inventariable = source.Inventariable,
                Exento = source.Exento,
                UsuarioCrea = source.UsuarioCrea,
                FechaCrea = source.FechaCrea,
                UsuarioModifica = source.UsuarioModifica,
                FechaModifica = source.FechaModifica
            };
        }

        private class CatalogData
        {
            public CatalogData()
            {
                Unidades = new List<string>();
                Proveedores = new List<EN_Supplier>();
                Bodegas = new List<EN_Store>();
                Familias = new List<FamiliaCatalogo>();
                Departamentos = new List<DepartamentoCatalogo>();
                Categorias = new List<CategoriaCatalogo>();
                SubCategorias = new List<SubCategoriaCatalogo>();
                Impuestos = new List<EN_Tax>();
            }

            public IList<string> Unidades { get; set; }
            public IList<EN_Supplier> Proveedores { get; set; }
            public IList<EN_Store> Bodegas { get; set; }
            public IList<FamiliaCatalogo> Familias { get; set; }
            public IList<DepartamentoCatalogo> Departamentos { get; set; }
            public IList<CategoriaCatalogo> Categorias { get; set; }
            public IList<SubCategoriaCatalogo> SubCategorias { get; set; }
            public IList<EN_Tax> Impuestos { get; set; }
        }

        private class FamiliaCatalogo
        {
            public FamiliaCatalogo(int id, string codigo, string nombre)
            {
                ID = id;
                Codigo = codigo;
                Nombre = nombre;
            }

            public int ID { get; private set; }
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public string Texto { get { return Codigo + " - " + Nombre; } }
        }

        private class DepartamentoCatalogo
        {
            public DepartamentoCatalogo(int id, int familiaID, string codigo, string nombre)
            {
                ID = id;
                FamiliaID = familiaID;
                Codigo = codigo;
                Nombre = nombre;
            }

            public int ID { get; private set; }
            public int FamiliaID { get; private set; }
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public string Texto { get { return Codigo + " - " + Nombre; } }
        }

        private class CategoriaCatalogo
        {
            public CategoriaCatalogo(int id, int departamentoID, string codigo, string nombre)
            {
                ID = id;
                DepartamentoID = departamentoID;
                Codigo = codigo;
                Nombre = nombre;
            }

            public int ID { get; private set; }
            public int DepartamentoID { get; private set; }
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public string Texto { get { return Codigo + " - " + Nombre; } }
        }

        private class SubCategoriaCatalogo
        {
            public SubCategoriaCatalogo(int id, int categoriaID, string codigo, string nombre)
            {
                ID = id;
                CategoriaID = categoriaID;
                Codigo = codigo;
                Nombre = nombre;
            }

            public int ID { get; private set; }
            public int CategoriaID { get; private set; }
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public string Texto { get { return Codigo + " - " + Nombre; } }
        }
    }
}
