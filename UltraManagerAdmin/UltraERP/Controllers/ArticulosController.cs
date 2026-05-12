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
        private static readonly List<string> Departamentos = new List<string> { "Abarrotes", "Bebidas", "Limpieza", "Materia Prima", "Empaque" };
        private static readonly List<string> Categorias = new List<string> { "General", "Consumo", "Produccion", "Reventa", "Regalia" };
        private static readonly List<string> Proveedores = new List<string> { "Distribuidora Central", "CONEJO DORADO S.R.L.", "Comercial La Union", "Proveedor Demo" };
        private static readonly List<string> Bodegas = new List<string> { "Bodega Principal", "Sucursal Escazu", "Sucursal Sabana" };

        private static readonly List<ArticuloViewModel> Articulos = new List<ArticuloViewModel>
        {
            CreateArticulo(1, "ARR-220", "Arroz precocido", "KG", "Abarrotes", "Consumo", "Distribuidora Central", "Bodega Principal", 2450m, 3125m, 1m, 35m, 10m, 80m, true, true, false),
            CreateArticulo(2, "SAL-441", "Salsa base", "GAL", "Materia Prima", "Produccion", "Comercial La Union", "Bodega Principal", 74850m, 84580.50m, 13m, 11m, 4m, 25m, true, true, false),
            CreateArticulo(3, "PROMO-1", "Producto de cortesia", "UND", "Abarrotes", "Regalia", "Proveedor Demo", "Sucursal Sabana", 0m, 0.01m, 0m, 18m, 0m, 200m, false, true, true)
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
            model.Departamento = (model.Departamento ?? "").Trim();
            model.Categoria = (model.Categoria ?? "").Trim();
            model.Proveedor = (model.Proveedor ?? "").Trim();
            model.Bodega = (model.Bodega ?? "").Trim();
        }

        private void PrepareCatalogs()
        {
            ViewBag.Unidades = ToSelectList(Unidades);
            ViewBag.Departamentos = ToSelectList(Departamentos);
            ViewBag.Categorias = ToSelectList(Categorias);
            ViewBag.Proveedores = ToSelectList(Proveedores);
            ViewBag.Bodegas = ToSelectList(Bodegas);
        }

        private static IEnumerable<SelectListItem> ToSelectList(IEnumerable<string> values)
        {
            return values.Select(x => new SelectListItem { Text = x, Value = x });
        }

        private ArticuloViewModel CreateNewArticulo()
        {
            return new ArticuloViewModel
            {
                Activo = true,
                Inventariable = true,
                UnidadMedida = "UND",
                Departamento = "Abarrotes",
                Categoria = "General",
                Proveedor = "Proveedor Demo",
                Bodega = "Bodega Principal",
                ImpuestoPorcentaje = 13m,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };
        }

        private static ArticuloViewModel CreateArticulo(int id, string codigo, string descripcion, string unidad, string departamento, string categoria, string proveedor, string bodega, decimal costo, decimal precio, decimal impuesto, decimal existencia, decimal minimo, decimal maximo, bool activo, bool inventariable, bool exento)
        {
            return new ArticuloViewModel
            {
                ID = id,
                Codigo = codigo,
                Descripcion = descripcion,
                DescripcionExtendida = descripcion + " para gestion de inventario.",
                UnidadMedida = unidad,
                Departamento = departamento,
                Categoria = categoria,
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
                Departamento = source.Departamento,
                Categoria = source.Categoria,
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
            target.Departamento = source.Departamento;
            target.Categoria = source.Categoria;
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
    }
}
