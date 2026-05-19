using Security.EntitiesAVS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
        private const int DefaultResultCount = 10000;

        public ActionResult Inicio()
        {
            try
            {
                ViewBag.ArticulosDataSource = "SQL";
                return View(Enumerable.Empty<ArticuloViewModel>());
            }
            catch (Exception ex)
            {
                TempData["ArticuloError"] = "No se pudo cargar art\u00edculos desde SQL: " + ex.Message;
                ViewBag.ArticulosDataSource = "SQL";
                return View(Enumerable.Empty<ArticuloViewModel>());
            }
        }

        public JsonResult Buscar(int page = 1, int pageSize = 10, string search = "", string estado = "Todos", string familia = "", string departamento = "", string categoria = "", string subcategoria = "", string proveedor = "")
        {
            try
            {
                page = Math.Max(page, 1);
                pageSize = new[] { 5, 10, 20, 50, 100 }.Contains(pageSize) ? pageSize : 10;

                int total;
                List<ArticuloViewModel> rows = SearchArticulos(page, pageSize, search, estado, familia, departamento, categoria, subcategoria, proveedor, out total);
                int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));

                return Json(new JsonResponse("", "", new
                {
                    Rows = rows,
                    Total = total,
                    TotalPages = totalPages,
                    Page = Math.Min(page, totalPages),
                    PageSize = pageSize
                }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse(ex.Message, "No se pudieron cargar los art\u00edculos desde SQL.", null, false), JsonRequestBehavior.AllowGet);
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
                        TempData["ArticuloError"] = "No se encontr\u00f3 el art\u00edculo en SQL.";
                }
                catch (Exception ex)
                {
                    TempData["ArticuloError"] = "No se pudo leer el art\u00edculo desde SQL: " + ex.Message;
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

            TranslateModelBindingErrors();
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
                // TASK_CODE de EXTCENTRAL_WIZARD_LOG es varchar(20), por eso se envía un código corto fijo.
                string title = "ACT. INDIVIDUAL PROD";
                Dictionary<string, object> result = new CT_Item().Save(item, title, GetCurrentUserID());
                string response = result.ContainsKey("RESPUESTA") ? Convert.ToString(result["RESPUESTA"]) : "";

                if (response.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ModelState.AddModelError("", "SQL rechaz\u00f3 el guardado del art\u00edculo: " + response);
                    PrepareCatalogs(catalog);
                    return View(model);
                }

                TempData["ArticuloMessage"] = model.ID > 0 ? "Art\u00edculo actualizado correctamente en SQL." : "Art\u00edculo creado correctamente en SQL.";
                return RedirectToAction("Inicio");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar el art\u00edculo en SQL. " + ex.Message);
                PrepareCatalogs(catalog);
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            return Json(new JsonResponse(
                "Pendiente de migraci\u00f3n",
                "El listado de art\u00edculos ya consulta SQL. La activaci\u00f3n e inactivaci\u00f3n se conectar\u00e1 cuando migremos el procedimiento de estado.",
                null,
                false));
        }

        private ArticuloViewModel GetArticuloById(int id)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];
            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return null;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
SELECT TOP 1
    I.ID,
    I.ItemLookupCode,
    I.Description,
    I.ExtendedDescription,
    I.UnitOfMeasure,
    ISNULL(EI.FamilyID, 0) AS FamilyID,
    ISNULL(I.DepartmentID, 0) AS DepartmentID,
    ISNULL(I.CategoryID, 0) AS CategoryID,
    ISNULL(EI.SubCategoryID, 0) AS SubCategoryID,
    ISNULL(F.Code, '') AS FamilyCode,
    ISNULL(F.Name, '') AS FamilyName,
    ISNULL(D.Code, '') AS DepartmentCode,
    ISNULL(D.Name, '') AS DepartmentName,
    ISNULL(C.Code, '') AS CategoryCode,
    ISNULL(C.Name, '') AS CategoryName,
    ISNULL(SC.Code, '') AS SubCategoryCode,
    ISNULL(SC.Description, '') AS SubCategoryName,
    ISNULL(S.SupplierName, '') AS SupplierName,
    ISNULL(S.Code, '') AS SupplierCode,
    ISNULL(I.ReplacementCost, 0) AS ReplacementCost,
    ISNULL(I.Cost, 0) AS Cost,
    ISNULL(I.Price, 0) AS Price,
    CONVERT(DECIMAL(18, 4), ISNULL(TD.Percentage, ISNULL(TI.Percentage, 0))) AS TaxPercentage,
    CONVERT(DECIMAL(18, 4), 0) AS Quantity,
    ISNULL(I.Inactive, 0) AS Inactive,
    I.DateCreated,
    I.LastUpdated
FROM dbo.Item I
LEFT JOIN dbo.ExtCentral_Item EI ON EI.ItemID = I.ID
LEFT JOIN dbo.ExtCentral_Family F ON F.ID = EI.FamilyID
LEFT JOIN dbo.Department D ON D.ID = I.DepartmentID
LEFT JOIN dbo.Category C ON C.ID = I.CategoryID
LEFT JOIN dbo.ExtCentral_SubCategory SC ON SC.ID = EI.SubCategoryID
LEFT JOIN dbo.Supplier S ON S.ID = I.SupplierID
LEFT JOIN dbo.ItemTax IT ON IT.ID = I.TaxID
LEFT JOIN dbo.Tax TD ON TD.ID = I.TaxID
LEFT JOIN dbo.Tax TI ON TI.ID = IT.TaxID01
WHERE I.ID = @ID;";
                command.Parameters.AddWithValue("@ID", id);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapArticuloSearch(reader) : null;
                }
            }
        }

        private static decimal CalculatePrice(decimal cost, decimal utility)
        {
            if (cost <= 0)
                return 0;

            return Math.Round(cost + (cost * utility / 100), 2);
        }

        private static DateTime GetArticuloSortDate(ArticuloViewModel item)
        {
            if (item == null)
                return DateTime.MinValue;

            if (item.FechaModifica.HasValue && item.FechaModifica.Value.Year > 1900)
                return item.FechaModifica.Value;

            return item.FechaCrea.Year > 1900 ? item.FechaCrea : DateTime.MinValue;
        }

        private List<ArticuloViewModel> SearchArticulos(int page, int pageSize, string search, string estado, string familia, string departamento, string categoria, string subcategoria, string proveedor, out int total)
        {
            var rows = new List<ArticuloViewModel>();
            total = 0;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];
            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return rows;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
;WITH Filtered AS
(
    SELECT
        I.ID,
        I.ItemLookupCode,
        I.Description,
        I.ExtendedDescription,
        I.UnitOfMeasure,
        ISNULL(EI.FamilyID, 0) AS FamilyID,
        ISNULL(I.DepartmentID, 0) AS DepartmentID,
        ISNULL(I.CategoryID, 0) AS CategoryID,
        ISNULL(EI.SubCategoryID, 0) AS SubCategoryID,
        ISNULL(F.Code, '') AS FamilyCode,
        ISNULL(F.Name, '') AS FamilyName,
        ISNULL(D.Code, '') AS DepartmentCode,
        ISNULL(D.Name, '') AS DepartmentName,
        ISNULL(C.Code, '') AS CategoryCode,
        ISNULL(C.Name, '') AS CategoryName,
        ISNULL(SC.Code, '') AS SubCategoryCode,
        ISNULL(SC.Description, '') AS SubCategoryName,
        ISNULL(S.SupplierName, '') AS SupplierName,
        ISNULL(S.Code, '') AS SupplierCode,
        ISNULL(I.ReplacementCost, 0) AS ReplacementCost,
        ISNULL(I.Cost, 0) AS Cost,
        ISNULL(I.Price, 0) AS Price,
        CONVERT(DECIMAL(18, 4), ISNULL(TD.Percentage, ISNULL(TI.Percentage, 0))) AS TaxPercentage,
        CONVERT(DECIMAL(18, 4), 0) AS Quantity,
        ISNULL(I.Inactive, 0) AS Inactive,
        I.DateCreated,
        I.LastUpdated,
        COUNT(1) OVER() AS TotalRows,
        ROW_NUMBER() OVER (ORDER BY ISNULL(I.LastUpdated, I.DateCreated) DESC, I.ID DESC) AS RowNum
    FROM dbo.Item I
    LEFT JOIN dbo.ExtCentral_Item EI ON EI.ItemID = I.ID
    LEFT JOIN dbo.ExtCentral_Family F ON F.ID = EI.FamilyID
    LEFT JOIN dbo.Department D ON D.ID = I.DepartmentID
    LEFT JOIN dbo.Category C ON C.ID = I.CategoryID
    LEFT JOIN dbo.ExtCentral_SubCategory SC ON SC.ID = EI.SubCategoryID
    LEFT JOIN dbo.Supplier S ON S.ID = I.SupplierID
    LEFT JOIN dbo.ItemTax IT ON IT.ID = I.TaxID
    LEFT JOIN dbo.Tax TD ON TD.ID = I.TaxID
    LEFT JOIN dbo.Tax TI ON TI.ID = IT.TaxID01
    WHERE (@Estado = 'Todos' OR (@Estado = 'Activo' AND ISNULL(I.Inactive, 0) = 0) OR (@Estado = 'Inactivo' AND ISNULL(I.Inactive, 0) = 1))
      AND (@Search = '' OR I.ItemLookupCode LIKE @SearchLike OR I.Description LIKE @SearchLike OR ISNULL(I.ExtendedDescription, '') LIKE @SearchLike OR ISNULL(F.Code, '') LIKE @SearchLike OR ISNULL(F.Name, '') LIKE @SearchLike OR ISNULL(D.Code, '') LIKE @SearchLike OR ISNULL(D.Name, '') LIKE @SearchLike OR ISNULL(C.Code, '') LIKE @SearchLike OR ISNULL(C.Name, '') LIKE @SearchLike OR ISNULL(SC.Code, '') LIKE @SearchLike OR ISNULL(SC.Description, '') LIKE @SearchLike OR ISNULL(S.SupplierName, '') LIKE @SearchLike)
      AND (@Familia = '' OR ISNULL(F.Code, '') LIKE @FamiliaLike OR ISNULL(F.Name, '') LIKE @FamiliaLike)
      AND (@Departamento = '' OR ISNULL(D.Code, '') LIKE @DepartamentoLike OR ISNULL(D.Name, '') LIKE @DepartamentoLike)
      AND (@Categoria = '' OR ISNULL(C.Code, '') LIKE @CategoriaLike OR ISNULL(C.Name, '') LIKE @CategoriaLike)
      AND (@Subcategoria = '' OR ISNULL(SC.Code, '') LIKE @SubcategoriaLike OR ISNULL(SC.Description, '') LIKE @SubcategoriaLike)
      AND (@Proveedor = '' OR ISNULL(S.SupplierName, '') LIKE @ProveedorLike OR ISNULL(S.Code, '') LIKE @ProveedorLike)
)
SELECT *
FROM Filtered
WHERE RowNum BETWEEN @StartRow AND @EndRow
ORDER BY RowNum;";

                AddSearchParameters(command, search, estado, familia, departamento, categoria, subcategoria, proveedor);
                command.Parameters.AddWithValue("@StartRow", ((page - 1) * pageSize) + 1);
                command.Parameters.AddWithValue("@EndRow", page * pageSize);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(MapArticuloSearch(reader));
                        if (total == 0)
                            total = ReadInt(reader, "TotalRows");
                    }
                }
            }

            return rows;
        }

        private static void AddSearchParameters(SqlCommand command, string search, string estado, string familia, string departamento, string categoria, string subcategoria, string proveedor)
        {
            AddTextParameter(command, "@Search", search);
            AddTextParameter(command, "@Estado", String.IsNullOrWhiteSpace(estado) ? "Todos" : estado);
            AddTextParameter(command, "@Familia", familia);
            AddTextParameter(command, "@Departamento", departamento);
            AddTextParameter(command, "@Categoria", categoria);
            AddTextParameter(command, "@Subcategoria", subcategoria);
            AddTextParameter(command, "@Proveedor", proveedor);
        }

        private static void AddTextParameter(SqlCommand command, string name, string value)
        {
            string clean = (value ?? "").Trim();
            command.Parameters.AddWithValue(name, clean);
            command.Parameters.AddWithValue(name + "Like", "%" + clean + "%");
        }

        private static ArticuloViewModel MapArticuloSearch(SqlDataReader reader)
        {
            decimal costo = ReadDecimal(reader, "ReplacementCost") > 0 ? ReadDecimal(reader, "ReplacementCost") : ReadDecimal(reader, "Cost");
            decimal precio = ReadDecimal(reader, "Price");

            return new ArticuloViewModel
            {
                ID = ReadInt(reader, "ID"),
                Codigo = ReadString(reader, "ItemLookupCode"),
                Descripcion = ReadString(reader, "Description"),
                DescripcionExtendida = ReadString(reader, "ExtendedDescription"),
                UnidadMedida = ReadString(reader, "UnitOfMeasure"),
                FamiliaID = ReadInt(reader, "FamilyID"),
                DepartamentoID = ReadInt(reader, "DepartmentID"),
                CategoriaID = ReadInt(reader, "CategoryID"),
                SubCategoriaID = ReadInt(reader, "SubCategoryID"),
                Familia = JoinCodeName(ReadString(reader, "FamilyCode"), ReadString(reader, "FamilyName")),
                Departamento = JoinCodeName(ReadString(reader, "DepartmentCode"), ReadString(reader, "DepartmentName")),
                Categoria = JoinCodeName(ReadString(reader, "CategoryCode"), ReadString(reader, "CategoryName")),
                SubCategoria = JoinCodeName(ReadString(reader, "SubCategoryCode"), ReadString(reader, "SubCategoryName")),
                Proveedor = !String.IsNullOrWhiteSpace(ReadString(reader, "SupplierName")) ? ReadString(reader, "SupplierName") : ReadString(reader, "SupplierCode"),
                Costo = costo,
                PrecioVenta = precio,
                ImpuestoPorcentaje = ReadDecimal(reader, "TaxPercentage"),
                Existencia = ReadDecimal(reader, "Quantity"),
                ExistenciaMinima = 0,
                ExistenciaMaxima = 0,
                Activo = !ReadBool(reader, "Inactive"),
                Inventariable = true,
                Exento = ReadDecimal(reader, "TaxPercentage") <= 0,
                UsuarioCrea = "SQL",
                FechaCrea = ReadDateTime(reader, "DateCreated"),
                FechaModifica = ReadNullableDateTime(reader, "LastUpdated")
            };
        }

        private EN_Item BuildItem(ArticuloViewModel model, CatalogData catalog)
        {
            string storesAvailable = GetStoresAvailable();
            EN_Item item = new CT_Item().GetByItemLookupCode(model.Codigo, storesAvailable) ?? new EN_Item();

            item.ID = model.ID;
            item.ItemLookupCode = model.Codigo;
            item.Description = model.Descripcion;
            item.ExtendedDescription = model.DescripcionExtendida;
            item.SubDescription3 = item.SubDescription3 ?? "";
            item.SubDescription4 = item.SubDescription4 ?? "";
            item.SubDescription5 = item.SubDescription5 ?? "";
            item.SubDescription6 = item.SubDescription6 ?? "";
            item.SubDescription7 = item.SubDescription7 ?? "";
            item.SubDescription8 = item.SubDescription8 ?? "";
            item.SubDescription9 = item.SubDescription9 ?? "";
            item.SubDescription10 = item.SubDescription10 ?? "";
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
            item.StoresSelected = ResolveStoresSelectedForSave(item.StoresSelected, storesAvailable, catalog);

            return item;
        }

        private void TranslateModelBindingErrors()
        {
            ReplaceModelBindingError("Costo", "Ingrese el costo del art\u00edculo.", "Ingrese un costo v\u00e1lido.");
            ReplaceModelBindingError("PrecioVenta", "Ingrese el precio de venta del art\u00edculo.", "Ingrese un precio de venta v\u00e1lido.");
        }

        private void ReplaceModelBindingError(string fieldName, string requiredMessage, string invalidMessage)
        {
            ModelState state = ModelState[fieldName];
            if (state == null || state.Errors.Count == 0)
                return;

            string value = Request.Form[fieldName];
            ModelState.Remove(fieldName);
            ModelState.AddModelError(fieldName, String.IsNullOrWhiteSpace(value) ? requiredMessage : invalidMessage);
        }

        private void ValidateArticulo(ArticuloViewModel model, CatalogData catalog)
        {
            if (model == null)
                return;

            bool categoryHasSubCategories = model.CategoriaID > 0 && catalog.SubCategorias.Any(x => x.CategoriaID == model.CategoriaID);

            if (!catalog.Familias.Any(x => x.ID == model.FamiliaID))
                ModelState.AddModelError("FamiliaID", "Seleccione una familia v\u00e1lida.");

            var departamento = catalog.Departamentos.FirstOrDefault(x => x.ID == model.DepartamentoID);
            if (model.DepartamentoID > 0 && departamento == null)
                ModelState.AddModelError("DepartamentoID", "Seleccione un departamento v\u00e1lido.");

            var categoria = catalog.Categorias.FirstOrDefault(x => x.ID == model.CategoriaID);
            if (model.CategoriaID > 0 && categoria == null)
                ModelState.AddModelError("CategoriaID", "Seleccione una categor\u00eda v\u00e1lida.");

            if (categoryHasSubCategories)
            {
                var subCategoria = catalog.SubCategorias.FirstOrDefault(x => x.ID == model.SubCategoriaID);
                if (model.SubCategoriaID > 0 && (subCategoria == null || subCategoria.CategoriaID != model.CategoriaID))
                    ModelState.AddModelError("SubCategoriaID", "Seleccione una subcategor\u00eda v\u00e1lida para la categor\u00eda.");
            }
            else
            {
                model.SubCategoriaID = 0;
                if (ModelState.ContainsKey("SubCategoriaID"))
                    ModelState["SubCategoriaID"].Errors.Clear();
            }

            try
            {
                EN_Item existing = new CT_Item().GetByItemLookupCode(model.Codigo, GetStoresAvailable());
                if (existing != null && existing.ID != model.ID)
                    ModelState.AddModelError("Codigo", "Ya existe un art\u00edculo con este c\u00f3digo.");
            }
            catch
            {
                ModelState.AddModelError("", "No se pudo validar el c\u00f3digo del art\u00edculo contra SQL.");
            }

            if (model.ExistenciaMaxima > 0 && model.ExistenciaMinima > model.ExistenciaMaxima)
                ModelState.AddModelError("ExistenciaMinima", "La existencia m\u00ednima no puede ser mayor que la m\u00e1xima.");

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
                return NormalizeCatalogData(new CatalogData
                {
                    Unidades = new CT_UOM().GetAllByInactive(false).Select(x => x.Code).Where(x => !String.IsNullOrWhiteSpace(x)).ToList(),
                    Proveedores = new CT_Supplier().GetAll(storesAvailable, "", 0, 0),
                    Bodegas = new CT_Store().GetAll("", 0, 0),
                    Familias = new CT_ExtCentral_Family().GetAll("", 0, 0).Select(x => new FamiliaCatalogo(x.ID, x.Code, x.Name)).ToList(),
                    Departamentos = new CT_Department().GetAll("", 0, 0).Select(x => new DepartamentoCatalogo(x.ID, x.FamilyID, x.Code, x.Name)).ToList(),
                    Categorias = new CT_Category().GetAll("", 0, 0).Select(x => new CategoriaCatalogo(x.ID, x.DepartmentID, x.Code, x.Name)).ToList(),
                    SubCategorias = new CT_ExtCentral_SubCategory().GetAll("", 0, 0).Select(x => new SubCategoriaCatalogo(x.ID, x.CategoryID, x.Code, x.Description)).ToList(),
                    Impuestos = new CT_Tax().GetAll("", 0, 0)
                });
            }
            catch
            {
                return NormalizeCatalogData(new CatalogData
                {
                    Unidades = new List<string>(),
                    Proveedores = new List<EN_Supplier>(),
                    Bodegas = new List<EN_Store>(),
                    Familias = new List<FamiliaCatalogo>(),
                    Departamentos = new List<DepartamentoCatalogo>(),
                    Categorias = new List<CategoriaCatalogo>(),
                    SubCategorias = new List<SubCategoriaCatalogo>(),
                    Impuestos = new List<EN_Tax>()
                });
            }
        }

        private CatalogData NormalizeCatalogData(CatalogData catalog)
        {
            catalog = catalog ?? new CatalogData();

            if (catalog.Unidades == null || !catalog.Unidades.Any())
                catalog.Unidades = GetUnitsDirect();
            if (catalog.Bodegas == null || !catalog.Bodegas.Any())
                catalog.Bodegas = GetStoresDirect();
            if (catalog.Familias == null || !catalog.Familias.Any())
                catalog.Familias = GetFamiliesDirect();
            if (catalog.Departamentos == null || !catalog.Departamentos.Any())
                catalog.Departamentos = GetDepartmentsDirect();
            if (catalog.Categorias == null || !catalog.Categorias.Any())
                catalog.Categorias = GetCategoriesDirect();
            if (catalog.SubCategorias == null || !catalog.SubCategorias.Any())
                catalog.SubCategorias = GetSubCategoriesDirect();

            return catalog;
        }

        private IList<string> GetUnitsDirect()
        {
            return ReadCatalogRows(@"
                SELECT Code
                FROM dbo.POA_UOM
                WHERE ISNULL(Inactive, 0) = 0
                  AND Code IS NOT NULL
                  AND LTRIM(RTRIM(Code)) <> ''
                ORDER BY Code", reader => ReadString(reader, "Code"));
        }

        private IList<EN_Store> GetStoresDirect()
        {
            return ReadCatalogRows(@"
                SELECT ID, Name, StoreCode
                FROM dbo.Store
                WHERE ISNULL(Inactive, 0) = 0
                ORDER BY ID", reader => new EN_Store(
                    ReadInt(reader, "ID"),
                    ReadString(reader, "Name"),
                    ReadString(reader, "StoreCode")));
        }

        private IList<FamiliaCatalogo> GetFamiliesDirect()
        {
            return ReadCatalogRows(@"
                SELECT ID, Code, Name
                FROM dbo.ExtCentral_Family
                ORDER BY ID", reader => new FamiliaCatalogo(
                    ReadInt(reader, "ID"),
                    ReadString(reader, "Code"),
                    ReadString(reader, "Name")));
        }

        private IList<DepartamentoCatalogo> GetDepartmentsDirect()
        {
            return ReadCatalogRows(@"
                SELECT D.ID, FD.FamilyID, D.code, D.Name
                FROM dbo.Department D
                INNER JOIN dbo.ExtCentral_FamilyDepartment FD ON FD.DepartmentID = D.ID
                ORDER BY D.ID", reader => new DepartamentoCatalogo(
                    ReadInt(reader, "ID"),
                    ReadInt(reader, "FamilyID"),
                    ReadString(reader, "code"),
                    ReadString(reader, "Name")));
        }

        private IList<CategoriaCatalogo> GetCategoriesDirect()
        {
            return ReadCatalogRows(@"
                SELECT ID, DepartmentID, Code, Name
                FROM dbo.Category
                ORDER BY ID", reader => new CategoriaCatalogo(
                    ReadInt(reader, "ID"),
                    ReadInt(reader, "DepartmentID"),
                    ReadString(reader, "Code"),
                    ReadString(reader, "Name")));
        }

        private IList<SubCategoriaCatalogo> GetSubCategoriesDirect()
        {
            return ReadCatalogRows(@"
                SELECT ID, CategoryID, Code, Description
                FROM dbo.ExtCentral_SubCategory
                ORDER BY ID", reader => new SubCategoriaCatalogo(
                    ReadInt(reader, "ID"),
                    ReadInt(reader, "CategoryID"),
                    ReadString(reader, "Code"),
                    ReadString(reader, "Description")));
        }

        private IList<T> ReadCatalogRows<T>(string sql, Func<SqlDataReader, T> mapper)
        {
            var items = new List<T>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];

            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return items;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        items.Add(mapper(reader));
                }
            }

            return items;
        }

        private static IEnumerable<SelectListItem> ToSelectList(IEnumerable<string> values)
        {
            return values.Select(x => new SelectListItem { Text = x, Value = x });
        }

        private ArticuloViewModel CreateNewArticulo(CatalogData catalog)
        {
            var subCategoria = catalog.SubCategorias.FirstOrDefault();
            var categoria = subCategoria == null
                ? catalog.Categorias.FirstOrDefault()
                : catalog.Categorias.FirstOrDefault(x => x.ID == subCategoria.CategoriaID) ?? catalog.Categorias.FirstOrDefault();
            var departamento = categoria == null
                ? catalog.Departamentos.FirstOrDefault()
                : catalog.Departamentos.FirstOrDefault(x => x.ID == categoria.DepartamentoID) ?? catalog.Departamentos.FirstOrDefault();
            var familia = departamento == null
                ? catalog.Familias.FirstOrDefault()
                : catalog.Familias.FirstOrDefault(x => x.ID == departamento.FamiliaID) ?? catalog.Familias.FirstOrDefault();

            var model = new ArticuloViewModel
            {
                Activo = true,
                Inventariable = true,
                UnidadMedida = catalog.Unidades.FirstOrDefault() ?? "",
                FamiliaID = familia == null ? 0 : familia.ID,
                DepartamentoID = departamento == null ? 0 : departamento.ID,
                CategoriaID = categoria == null ? 0 : categoria.ID,
                SubCategoriaID = subCategoria == null ? 0 : subCategoria.ID,
                Proveedor = catalog.Proveedores.Select(x => x.Name).FirstOrDefault() ?? "",
                Bodega = catalog.Bodegas.Select(x => x.NameS).FirstOrDefault() ?? "",
                ImpuestoPorcentaje = ResolveDefaultTaxPercentage(catalog),
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };

            ApplyClasificacion(model, catalog);
            return model;
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
            return reader.IsDBNull(ordinal) ? DateTime.Now : Convert.ToDateTime(reader.GetValue(ordinal));
        }

        private static DateTime? ReadNullableDateTime(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (DateTime?)null : Convert.ToDateTime(reader.GetValue(ordinal));
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

        private static decimal ResolveDefaultTaxPercentage(CatalogData catalog)
        {
            if (catalog == null || catalog.Impuestos == null || catalog.Impuestos.Count == 0)
                return 0m;

            EN_Tax preferred = catalog.Impuestos.FirstOrDefault(x => Math.Abs(Convert.ToDecimal(x.Percentage) - 13m) < 0.01m);
            EN_Tax fallback = preferred ?? catalog.Impuestos.FirstOrDefault();
            return fallback == null ? 0m : Convert.ToDecimal(fallback.Percentage);
        }

        private static string ResolveStoresSelectedForSave(string currentStores, string storesAvailable, CatalogData catalog)
        {
            if (!String.IsNullOrWhiteSpace(currentStores) && currentStores != "%")
                return currentStores;

            if (!String.IsNullOrWhiteSpace(storesAvailable) && storesAvailable != "%")
                return storesAvailable;

            var storeIds = (catalog.Bodegas ?? new List<EN_Store>())
                .Where(x => x != null && x.IDS > 0)
                .Select(x => x.IDS.ToString())
                .Distinct()
                .ToList();

            return storeIds.Any() ? String.Join(",", storeIds) : "";
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
