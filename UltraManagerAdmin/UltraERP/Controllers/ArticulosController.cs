using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class ArticulosController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<string> Unidades = new List<string> { "UND", "KG", "CJ", "GAL", "L", "QQ", "BQ" };
        private static readonly List<string> Proveedores = new List<string> { "Distribuidora Central", "CONEJO DORADO S.R.L.", "Comercial La Union", "Proveedor Demo" };
        private static readonly List<string> Bodegas = new List<string> { "Bodega Principal", "Sucursal Escazu", "Sucursal Sabana" };

        private static readonly List<FamiliaCatalogo> Familias = new List<FamiliaCatalogo>
        {
            new FamiliaCatalogo(1, "ABAR", "Abarrotes"),
            new FamiliaCatalogo(2, "MP", "Materia Prima"),
            new FamiliaCatalogo(3, "EMP", "Empaque"),
            new FamiliaCatalogo(4, "PROMO", "Promociones")
        };

        private static readonly List<DepartamentoCatalogo> Departamentos = new List<DepartamentoCatalogo>
        {
            new DepartamentoCatalogo(1, 1, "SECOS", "Productos secos"),
            new DepartamentoCatalogo(2, 1, "BEB", "Bebidas"),
            new DepartamentoCatalogo(3, 2, "PROD", "Produccion"),
            new DepartamentoCatalogo(4, 3, "CAJAS", "Cajas y bolsas")
        };

        private static readonly List<CategoriaCatalogo> Categorias = new List<CategoriaCatalogo>
        {
            new CategoriaCatalogo(1, 1, "ARROZ", "Arroces"),
            new CategoriaCatalogo(2, 1, "COND", "Condimentos"),
            new CategoriaCatalogo(3, 2, "GASE", "Gaseosas"),
            new CategoriaCatalogo(4, 3, "HAR", "Harinas"),
            new CategoriaCatalogo(5, 4, "BOLS", "Bolsas")
        };

        private static readonly List<SubCategoriaCatalogo> SubCategorias = new List<SubCategoriaCatalogo>
        {
            new SubCategoriaCatalogo(1, 1, "BLANCO", "Arroz blanco"),
            new SubCategoriaCatalogo(2, 1, "INTEG", "Arroz integral"),
            new SubCategoriaCatalogo(3, 2, "SALSA", "Salsas y bases"),
            new SubCategoriaCatalogo(4, 3, "RET", "Retornables"),
            new SubCategoriaCatalogo(5, 4, "PREM", "Premezclas"),
            new SubCategoriaCatalogo(6, 5, "BOLSA", "Bolsa flexible")
        };

        private static readonly List<ArticuloViewModel> Articulos = new List<ArticuloViewModel>
        {
            CreateArticulo(1, "ARR-220", "Arroz precocido", "KG", 1, "Distribuidora Central", "Bodega Principal", 2450m, 3125m, 1m, 35m, 10m, 80m, true, true, false),
            CreateArticulo(2, "SAL-441", "Salsa base", "GAL", 3, "Comercial La Union", "Bodega Principal", 74850m, 84580.50m, 13m, 11m, 4m, 25m, true, true, false),
            CreateArticulo(3, "HAR-112", "Harina preparada", "QQ", 5, "Proveedor Demo", "Bodega Principal", 10900m, 12750m, 1m, 15m, 5m, 40m, true, true, false)
        };

        public ActionResult Inicio()
        {
            List<ArticuloViewModel> model;
            lock (SyncRoot)
            {
                model = Articulos.Select(Clone).OrderBy(x => x.Codigo).ToList();
            }

            return View(model);
        }

        public ActionResult Registro(int? id)
        {
            ArticuloViewModel model = null;
            if (id.HasValue)
            {
                lock (SyncRoot)
                {
                    model = Articulos.Where(x => x.ID == id.Value).Select(Clone).FirstOrDefault();
                }
            }

            PrepareCatalogs();
            return View(model ?? CreateNewArticulo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(ArticuloViewModel model)
        {
            Normalize(model);
            ApplyClasificacion(model);
            ValidateArticulo(model);

            if (!ModelState.IsValid)
            {
                PrepareCatalogs();
                return View(model);
            }

            lock (SyncRoot)
            {
                var existing = Articulos.FirstOrDefault(x => x.ID == model.ID);
                if (existing == null)
                {
                    model.ID = Articulos.Count == 0 ? 1 : Articulos.Max(x => x.ID) + 1;
                    model.UsuarioCrea = GetCurrentUser();
                    model.FechaCrea = DateTime.Now;
                    Articulos.Add(Clone(model));
                    TempData["ArticuloMessage"] = "Articulo creado correctamente.";
                }
                else
                {
                    model.UsuarioCrea = existing.UsuarioCrea;
                    model.FechaCrea = existing.FechaCrea;
                    model.UsuarioModifica = GetCurrentUser();
                    model.FechaModifica = DateTime.Now;
                    CopyValues(model, existing);
                    TempData["ArticuloMessage"] = "Articulo actualizado correctamente.";
                }
            }

            return RedirectToAction("Inicio");
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            lock (SyncRoot)
            {
                var articulo = Articulos.FirstOrDefault(x => x.ID == id);
                if (articulo == null)
                    return Json(new JsonResponse("Articulo no encontrado.", "No se encontro el articulo.", null, false));

                articulo.Activo = !articulo.Activo;
                articulo.UsuarioModifica = GetCurrentUser();
                articulo.FechaModifica = DateTime.Now;

                return Json(new JsonResponse("", articulo.Activo ? "Articulo activado." : "Articulo inactivado.", Clone(articulo), true));
            }
        }

        private void ValidateArticulo(ArticuloViewModel model)
        {
            if (model == null)
                return;

            if (!Familias.Any(x => x.ID == model.FamiliaID))
                ModelState.AddModelError("FamiliaID", "Seleccione una familia valida.");

            var departamento = Departamentos.FirstOrDefault(x => x.ID == model.DepartamentoID);
            if (departamento == null || departamento.FamiliaID != model.FamiliaID)
                ModelState.AddModelError("DepartamentoID", "Seleccione un departamento valido para la familia.");

            var categoria = Categorias.FirstOrDefault(x => x.ID == model.CategoriaID);
            if (categoria == null || categoria.DepartamentoID != model.DepartamentoID)
                ModelState.AddModelError("CategoriaID", "Seleccione una categoria valida para el departamento.");

            var subCategoria = SubCategorias.FirstOrDefault(x => x.ID == model.SubCategoriaID);
            if (subCategoria == null || subCategoria.CategoriaID != model.CategoriaID)
                ModelState.AddModelError("SubCategoriaID", "Seleccione una subcategoria valida para la categoria.");

            lock (SyncRoot)
            {
                var exists = Articulos.Any(x => x.ID != model.ID && String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));
                if (exists)
                    ModelState.AddModelError("Codigo", "Ya existe un articulo con este codigo.");
            }

            if (model.ExistenciaMaxima > 0 && model.ExistenciaMinima > model.ExistenciaMaxima)
                ModelState.AddModelError("ExistenciaMinima", "La existencia minima no puede ser mayor que la maxima.");

            if (model.Exento)
                model.ImpuestoPorcentaje = 0;
        }

        private void Normalize(ArticuloViewModel model)
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

        private static void ApplyClasificacion(ArticuloViewModel model)
        {
            if (model == null)
                return;

            var familia = Familias.FirstOrDefault(x => x.ID == model.FamiliaID);
            var departamento = Departamentos.FirstOrDefault(x => x.ID == model.DepartamentoID);
            var categoria = Categorias.FirstOrDefault(x => x.ID == model.CategoriaID);
            var subCategoria = SubCategorias.FirstOrDefault(x => x.ID == model.SubCategoriaID);

            model.Familia = familia == null ? "" : familia.Texto;
            model.Departamento = departamento == null ? "" : departamento.Texto;
            model.Categoria = categoria == null ? "" : categoria.Texto;
            model.SubCategoria = subCategoria == null ? "" : subCategoria.Texto;
        }

        private void PrepareCatalogs()
        {
            ViewBag.Unidades = ToSelectList(Unidades);
            ViewBag.Proveedores = ToSelectList(Proveedores);
            ViewBag.Bodegas = ToSelectList(Bodegas);
            ViewBag.Familias = Familias.Select(x => new SelectListItem { Text = x.Texto, Value = x.ID.ToString() }).ToList();
            ViewBag.Departamentos = Departamentos.Select(x => new SelectListItem { Text = x.Texto, Value = x.ID.ToString() }).ToList();
            ViewBag.Categorias = Categorias.Select(x => new SelectListItem { Text = x.Texto, Value = x.ID.ToString() }).ToList();
            ViewBag.SubCategorias = SubCategorias.Select(x => new SelectListItem { Text = x.Texto, Value = x.ID.ToString() }).ToList();
            ViewBag.ClasificacionJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                departamentos = Departamentos,
                categorias = Categorias,
                subCategorias = SubCategorias
            });
        }

        private static IEnumerable<SelectListItem> ToSelectList(IEnumerable<string> values)
        {
            return values.Select(x => new SelectListItem { Text = x, Value = x });
        }

        private ArticuloViewModel CreateNewArticulo()
        {
            var model = new ArticuloViewModel
            {
                Activo = true,
                Inventariable = true,
                UnidadMedida = "UND",
                FamiliaID = 1,
                DepartamentoID = 1,
                CategoriaID = 1,
                SubCategoriaID = 1,
                Proveedor = "Proveedor Demo",
                Bodega = "Bodega Principal",
                ImpuestoPorcentaje = 13m,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };

            ApplyClasificacion(model);
            return model;
        }

        private static ArticuloViewModel CreateArticulo(int id, string codigo, string descripcion, string unidad, int subCategoriaID, string proveedor, string bodega, decimal costo, decimal precio, decimal impuesto, decimal existencia, decimal minimo, decimal maximo, bool activo, bool inventariable, bool exento)
        {
            var subCategoria = SubCategorias.First(x => x.ID == subCategoriaID);
            var categoria = Categorias.First(x => x.ID == subCategoria.CategoriaID);
            var departamento = Departamentos.First(x => x.ID == categoria.DepartamentoID);
            var familia = Familias.First(x => x.ID == departamento.FamiliaID);

            var model = new ArticuloViewModel
            {
                ID = id,
                Codigo = codigo,
                Descripcion = descripcion,
                DescripcionExtendida = descripcion + " para gestion de inventario.",
                UnidadMedida = unidad,
                FamiliaID = familia.ID,
                DepartamentoID = departamento.ID,
                CategoriaID = categoria.ID,
                SubCategoriaID = subCategoria.ID,
                Proveedor = proveedor,
                Bodega = bodega,
                Costo = costo,
                PrecioVenta = precio,
                ImpuestoPorcentaje = impuesto,
                Existencia = existencia,
                ExistenciaMinima = minimo,
                ExistenciaMaxima = maximo,
                Activo = activo,
                Inventariable = inventariable,
                Exento = exento,
                UsuarioCrea = "Soporte",
                FechaCrea = DateTime.Now.AddDays(-2)
            };

            ApplyClasificacion(model);
            return model;
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

        private static void CopyValues(ArticuloViewModel source, ArticuloViewModel target)
        {
            target.Codigo = source.Codigo;
            target.Descripcion = source.Descripcion;
            target.DescripcionExtendida = source.DescripcionExtendida;
            target.UnidadMedida = source.UnidadMedida;
            target.FamiliaID = source.FamiliaID;
            target.DepartamentoID = source.DepartamentoID;
            target.CategoriaID = source.CategoriaID;
            target.SubCategoriaID = source.SubCategoriaID;
            target.Familia = source.Familia;
            target.Departamento = source.Departamento;
            target.Categoria = source.Categoria;
            target.SubCategoria = source.SubCategoria;
            target.Proveedor = source.Proveedor;
            target.Bodega = source.Bodega;
            target.Costo = source.Costo;
            target.PrecioVenta = source.PrecioVenta;
            target.ImpuestoPorcentaje = source.ImpuestoPorcentaje;
            target.Existencia = source.Existencia;
            target.ExistenciaMinima = source.ExistenciaMinima;
            target.ExistenciaMaxima = source.ExistenciaMaxima;
            target.Activo = source.Activo;
            target.Inventariable = source.Inventariable;
            target.Exento = source.Exento;
            target.UsuarioCrea = source.UsuarioCrea;
            target.FechaCrea = source.FechaCrea;
            target.UsuarioModifica = source.UsuarioModifica;
            target.FechaModifica = source.FechaModifica;
        }

        private string GetCurrentUser()
        {
            return User != null && User.Identity != null && !String.IsNullOrWhiteSpace(User.Identity.Name)
                ? User.Identity.Name
                : "Soporte";
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
