using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class SubCategoriasController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<CategoriaCatalogo> Categorias = new List<CategoriaCatalogo>
        {
            new CategoriaCatalogo(1, "ARROZ", "Arroces", "GRANOS", "Granos basicos", "ABAR", "Abarrotes ticos"),
            new CategoriaCatalogo(2, "FRIJ", "Frijoles", "GRANOS", "Granos basicos", "ABAR", "Abarrotes ticos"),
            new CategoriaCatalogo(3, "SALT", "Salsas ticas", "SALSAS", "Salsas y condimentos", "ABAR", "Abarrotes ticos"),
            new CategoriaCatalogo(4, "LECHE", "Leches y lacteos", "REFRI", "Refrigerados", "LACT", "Lacteos y frescos"),
            new CategoriaCatalogo(5, "CAFCR", "Cafe costarricense", "CAFE", "Cafe y bebidas", "BEB", "Bebidas y cafe"),
            new CategoriaCatalogo(6, "DETER", "Detergentes", "HOGAR", "Cuidado del hogar", "LIMP", "Limpieza y hogar")
        };

        private static readonly List<SubCategoriaViewModel> SubCategorias = new List<SubCategoriaViewModel>
        {
            CreateSubCategoria(1, 1, "GRANO", "Arroz grano entero", "Arroces enteros nacionales en bolsa.", true, 2, 12),
            CreateSubCategoria(2, 1, "PRECOC", "Arroz precocido", "Arroces precocidos de coccion rapida.", true, 1, 6),
            CreateSubCategoria(3, 2, "ROJOS", "Frijoles rojos", "Frijoles rojos en grano y empacados.", true, 1, 8),
            CreateSubCategoria(4, 3, "MESA", "Salsas de mesa", "Salsas listas para acompanamiento tipico.", true, 2, 9),
            CreateSubCategoria(5, 4, "FLUIDA", "Leche fluida", "Leches frescas y larga duracion.", true, 1, 11),
            CreateSubCategoria(6, 5, "MOLIDO", "Cafe molido", "Cafe molido de origen costarricense.", true, 2, 10),
            CreateSubCategoria(7, 6, "POLVO", "Detergente en polvo", "Detergentes en polvo de venta masiva.", true, 1, 7)
        };

        public ActionResult Inicio()
        {
            List<SubCategoriaViewModel> model;
            lock (SyncRoot)
            {
                model = SubCategorias.Select(Clone).OrderBy(x => x.CategoriaNombre).ThenBy(x => x.Codigo).ToList();
            }

            return View(model);
        }

        public ActionResult Registro(int? id)
        {
            SubCategoriaViewModel model = null;
            if (id.HasValue)
            {
                lock (SyncRoot)
                {
                    model = SubCategorias.Where(x => x.ID == id.Value).Select(Clone).FirstOrDefault();
                }
            }

            PrepareCatalogs();
            return View(model ?? CreateNewSubCategoria());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(SubCategoriaViewModel model)
        {
            Normalize(model);
            ApplyCategoria(model);
            ValidateSubCategoria(model);

            if (!ModelState.IsValid)
            {
                PrepareCatalogs();
                return View(model);
            }

            lock (SyncRoot)
            {
                var existing = SubCategorias.FirstOrDefault(x => x.ID == model.ID);
                if (existing == null)
                {
                    model.ID = SubCategorias.Count == 0 ? 1 : SubCategorias.Max(x => x.ID) + 1;
                    model.UsuarioCrea = GetCurrentUser();
                    model.FechaCrea = DateTime.Now;
                    SubCategorias.Add(Clone(model));
                    TempData["SubCategoriaMessage"] = "Subcategoria creada correctamente.";
                }
                else
                {
                    model.UsuarioCrea = existing.UsuarioCrea;
                    model.FechaCrea = existing.FechaCrea;
                    model.UsuarioModifica = GetCurrentUser();
                    model.FechaModifica = DateTime.Now;
                    CopyValues(model, existing);
                    TempData["SubCategoriaMessage"] = "Subcategoria actualizada correctamente.";
                }
            }

            return RedirectToAction("Inicio");
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            lock (SyncRoot)
            {
                var subCategoria = SubCategorias.FirstOrDefault(x => x.ID == id);
                if (subCategoria == null)
                    return Json(new JsonResponse("Subcategoria no encontrada.", "No se encontro la subcategoria.", null, false));

                subCategoria.Activa = !subCategoria.Activa;
                subCategoria.UsuarioModifica = GetCurrentUser();
                subCategoria.FechaModifica = DateTime.Now;

                return Json(new JsonResponse("", subCategoria.Activa ? "Subcategoria activada." : "Subcategoria inactivada.", Clone(subCategoria), true));
            }
        }

        private void ValidateSubCategoria(SubCategoriaViewModel model)
        {
            if (model == null)
                return;

            if (model.CategoriaID <= 0 || !Categorias.Any(x => x.ID == model.CategoriaID))
                ModelState.AddModelError("CategoriaID", "Seleccione una categoria valida.");

            lock (SyncRoot)
            {
                var codeExists = SubCategorias.Any(x =>
                    x.ID != model.ID &&
                    x.CategoriaID == model.CategoriaID &&
                    String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));

                if (codeExists)
                    ModelState.AddModelError("Codigo", "Ya existe una subcategoria con este codigo en la categoria seleccionada.");

                var nameExists = SubCategorias.Any(x =>
                    x.ID != model.ID &&
                    x.CategoriaID == model.CategoriaID &&
                    String.Equals(x.Descripcion, model.Descripcion, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    ModelState.AddModelError("Descripcion", "Ya existe una subcategoria con esta descripcion en la categoria seleccionada.");
            }
        }

        private static void Normalize(SubCategoriaViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.Descripcion = (model.Descripcion ?? "").Trim();
            model.Nota = (model.Nota ?? "").Trim();
        }

        private static void ApplyCategoria(SubCategoriaViewModel model)
        {
            if (model == null)
                return;

            var categoria = Categorias.FirstOrDefault(x => x.ID == model.CategoriaID);
            if (categoria == null)
            {
                model.CategoriaCodigo = "";
                model.CategoriaNombre = "";
                model.DepartamentoCodigo = "";
                model.DepartamentoNombre = "";
                model.FamiliaCodigo = "";
                model.FamiliaNombre = "";
                return;
            }

            model.CategoriaCodigo = categoria.Codigo;
            model.CategoriaNombre = categoria.Nombre;
            model.DepartamentoCodigo = categoria.DepartamentoCodigo;
            model.DepartamentoNombre = categoria.DepartamentoNombre;
            model.FamiliaCodigo = categoria.FamiliaCodigo;
            model.FamiliaNombre = categoria.FamiliaNombre;
        }

        private void PrepareCatalogs()
        {
            ViewBag.Categorias = Categorias
                .Select(x => new SelectListItem { Text = x.FamiliaCodigo + " / " + x.DepartamentoCodigo + " / " + x.Codigo + " - " + x.Nombre, Value = x.ID.ToString() })
                .ToList();
        }

        private SubCategoriaViewModel CreateNewSubCategoria()
        {
            var model = new SubCategoriaViewModel
            {
                Activa = true,
                CategoriaID = Categorias.First().ID,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };

            ApplyCategoria(model);
            return model;
        }

        private static SubCategoriaViewModel CreateSubCategoria(int id, int categoriaID, string codigo, string descripcion, string nota, bool activa, int cantidadSegmentos, int cantidadArticulos)
        {
            var model = new SubCategoriaViewModel
            {
                ID = id,
                CategoriaID = categoriaID,
                Codigo = codigo,
                Descripcion = descripcion,
                Nota = nota,
                Activa = activa,
                CantidadSegmentos = cantidadSegmentos,
                CantidadArticulos = cantidadArticulos,
                UsuarioCrea = "Soporte",
                FechaCrea = DateTime.Now.AddDays(-1)
            };

            ApplyCategoria(model);
            return model;
        }

        private static SubCategoriaViewModel Clone(SubCategoriaViewModel source)
        {
            if (source == null)
                return null;

            return new SubCategoriaViewModel
            {
                ID = source.ID,
                CategoriaID = source.CategoriaID,
                CategoriaCodigo = source.CategoriaCodigo,
                CategoriaNombre = source.CategoriaNombre,
                DepartamentoCodigo = source.DepartamentoCodigo,
                DepartamentoNombre = source.DepartamentoNombre,
                FamiliaCodigo = source.FamiliaCodigo,
                FamiliaNombre = source.FamiliaNombre,
                Codigo = source.Codigo,
                Descripcion = source.Descripcion,
                Nota = source.Nota,
                Activa = source.Activa,
                CantidadSegmentos = source.CantidadSegmentos,
                CantidadArticulos = source.CantidadArticulos,
                UsuarioCrea = source.UsuarioCrea,
                FechaCrea = source.FechaCrea,
                UsuarioModifica = source.UsuarioModifica,
                FechaModifica = source.FechaModifica
            };
        }

        private static void CopyValues(SubCategoriaViewModel source, SubCategoriaViewModel target)
        {
            target.CategoriaID = source.CategoriaID;
            target.CategoriaCodigo = source.CategoriaCodigo;
            target.CategoriaNombre = source.CategoriaNombre;
            target.DepartamentoCodigo = source.DepartamentoCodigo;
            target.DepartamentoNombre = source.DepartamentoNombre;
            target.FamiliaCodigo = source.FamiliaCodigo;
            target.FamiliaNombre = source.FamiliaNombre;
            target.Codigo = source.Codigo;
            target.Descripcion = source.Descripcion;
            target.Nota = source.Nota;
            target.Activa = source.Activa;
            target.CantidadSegmentos = source.CantidadSegmentos;
            target.CantidadArticulos = source.CantidadArticulos;
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

        private class CategoriaCatalogo
        {
            public CategoriaCatalogo(int id, string codigo, string nombre, string departamentoCodigo, string departamentoNombre, string familiaCodigo, string familiaNombre)
            {
                ID = id;
                Codigo = codigo;
                Nombre = nombre;
                DepartamentoCodigo = departamentoCodigo;
                DepartamentoNombre = departamentoNombre;
                FamiliaCodigo = familiaCodigo;
                FamiliaNombre = familiaNombre;
            }

            public int ID { get; private set; }
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public string DepartamentoCodigo { get; private set; }
            public string DepartamentoNombre { get; private set; }
            public string FamiliaCodigo { get; private set; }
            public string FamiliaNombre { get; private set; }
        }
    }
}
