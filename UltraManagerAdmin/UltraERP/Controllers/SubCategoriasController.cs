using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.BusinessEntities;
using UltraERP.BusinessLogic;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class SubCategoriasController : Controller
    {
        public ActionResult Inicio()
        {
            try
            {
                List<CategoriaCatalogo> categorias = GetCategorias();
                List<SubCategoriaViewModel> model = new CT_ExtCentral_SubCategory()
                    .GetAll("", 0, 0)
                    .Select(x => MapSubCategoria(x, categorias))
                    .OrderBy(x => x.CategoriaNombre)
                    .ThenBy(x => x.Codigo)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["SubCategoriaError"] = "No se pudo cargar subcategorias desde SQL: " + ex.Message;
                return View(Enumerable.Empty<SubCategoriaViewModel>());
            }
        }

        public ActionResult Registro(int? id)
        {
            List<CategoriaCatalogo> categorias = GetCategorias();
            SubCategoriaViewModel model = null;

            if (id.HasValue)
            {
                try
                {
                    EN_ExtCentral_SubCategory subCategory = GetSubCategoriaById(id.Value);
                    if (subCategory == null)
                        TempData["SubCategoriaError"] = "No se encontro la subcategoria en SQL.";
                    else
                        model = MapSubCategoria(subCategory, categorias);
                }
                catch (Exception ex)
                {
                    TempData["SubCategoriaError"] = "No se pudo leer la subcategoria desde SQL: " + ex.Message;
                }
            }

            PrepareCatalogs(categorias);
            return View(model ?? CreateNewSubCategoria(categorias));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(SubCategoriaViewModel model)
        {
            List<CategoriaCatalogo> categorias = GetCategorias();
            Normalize(model);
            ApplyCategoria(model, categorias);
            ValidateSubCategoria(model, categorias);

            if (!ModelState.IsValid)
            {
                PrepareCatalogs(categorias);
                return View(model);
            }

            try
            {
                EN_ExtCentral_SubCategory subCategory = new EN_ExtCentral_SubCategory
                {
                    ID = model.ID,
                    CategoryID = model.CategoriaID,
                    Code = model.Codigo,
                    Description = model.Descripcion
                };

                Respuesta response = new CT_ExtCentral_SubCategory().Save(subCategory);
                if (!response.Status)
                {
                    ModelState.AddModelError("", "SQL rechazo el guardado de la subcategoria: " + response.Message);
                    PrepareCatalogs(categorias);
                    return View(model);
                }

                TempData["SubCategoriaMessage"] = model.ID > 0 ? "Subcategoria actualizada correctamente en SQL." : "Subcategoria creada correctamente en SQL.";
                return RedirectToAction("Inicio");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar la subcategoria en SQL. " + ex.Message);
                PrepareCatalogs(categorias);
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            return Json(new JsonResponse(
                "Pendiente de migracion",
                "El catalogo de subcategorias ya consulta SQL. La activacion e inactivacion se conectara cuando el procedimiento exponga estado.",
                null,
                false));
        }

        private void ValidateSubCategoria(SubCategoriaViewModel model, List<CategoriaCatalogo> categorias)
        {
            if (model == null)
                return;

            if (model.CategoriaID <= 0 || !categorias.Any(x => x.ID == model.CategoriaID))
                ModelState.AddModelError("CategoriaID", "Seleccione una categoria valida.");

            try
            {
                List<SubCategoriaViewModel> subCategorias = new CT_ExtCentral_SubCategory()
                    .GetAll("", 0, 0)
                    .Select(MapSubCategoria)
                    .ToList();

                bool codeExists = subCategorias.Any(x =>
                    x.ID != model.ID &&
                    x.CategoriaID == model.CategoriaID &&
                    String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));

                if (codeExists)
                    ModelState.AddModelError("Codigo", "Ya existe una subcategoria con este codigo en la categoria seleccionada.");

                bool nameExists = subCategorias.Any(x =>
                    x.ID != model.ID &&
                    x.CategoriaID == model.CategoriaID &&
                    String.Equals(x.Descripcion, model.Descripcion, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    ModelState.AddModelError("Descripcion", "Ya existe una subcategoria con esta descripcion en la categoria seleccionada.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo validar la subcategoria contra SQL. " + ex.Message);
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

        private static void ApplyCategoria(SubCategoriaViewModel model, List<CategoriaCatalogo> categorias)
        {
            if (model == null)
                return;

            CategoriaCatalogo categoria = categorias.FirstOrDefault(x => x.ID == model.CategoriaID);
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

        private void PrepareCatalogs(List<CategoriaCatalogo> categorias)
        {
            ViewBag.Categorias = categorias
                .Select(x => new SelectListItem
                {
                    Text = x.FamiliaCodigo + " / " + x.DepartamentoCodigo + " / " + x.Codigo + " - " + x.Nombre,
                    Value = x.ID.ToString()
                })
                .ToList();
        }

        private SubCategoriaViewModel CreateNewSubCategoria(List<CategoriaCatalogo> categorias)
        {
            CategoriaCatalogo firstCategoria = categorias.FirstOrDefault();
            var model = new SubCategoriaViewModel
            {
                Activa = true,
                CategoriaID = firstCategoria == null ? 0 : firstCategoria.ID,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };

            ApplyCategoria(model, categorias);
            return model;
        }

        private EN_ExtCentral_SubCategory GetSubCategoriaById(int id)
        {
            return new CT_ExtCentral_SubCategory()
                .GetAll("", 0, 0)
                .FirstOrDefault(x => x.ID == id);
        }

        private static SubCategoriaViewModel MapSubCategoria(EN_ExtCentral_SubCategory subCategory)
        {
            return MapSubCategoria(subCategory, GetCategorias());
        }

        private static SubCategoriaViewModel MapSubCategoria(EN_ExtCentral_SubCategory subCategory, List<CategoriaCatalogo> categorias)
        {
            if (subCategory == null)
                return null;

            CategoriaCatalogo categoria = categorias.FirstOrDefault(x => x.ID == subCategory.CategoryID);

            return new SubCategoriaViewModel
            {
                ID = subCategory.ID,
                CategoriaID = subCategory.CategoryID,
                CategoriaCodigo = categoria == null ? "" : categoria.Codigo,
                CategoriaNombre = categoria == null ? "" : categoria.Nombre,
                DepartamentoCodigo = categoria == null ? "" : categoria.DepartamentoCodigo,
                DepartamentoNombre = categoria == null ? "" : categoria.DepartamentoNombre,
                FamiliaCodigo = categoria == null ? "" : categoria.FamiliaCodigo,
                FamiliaNombre = categoria == null ? "" : categoria.FamiliaNombre,
                Codigo = subCategory.Code,
                Descripcion = subCategory.Description,
                Nota = "",
                Activa = true
            };
        }

        private static List<CategoriaCatalogo> GetCategorias()
        {
            List<EN_Department> departamentos = new CT_Department().GetAll("", 0, 0);

            return new CT_Category()
                .GetAll("", 0, 0)
                .Select(x =>
                {
                    EN_Department departamento = departamentos.FirstOrDefault(d => d.ID == x.DepartmentID);
                    return new CategoriaCatalogo(
                        x.ID,
                        x.Code,
                        x.Name,
                        departamento == null ? "" : departamento.Code,
                        departamento == null ? "" : departamento.Name,
                        departamento == null ? "" : departamento.FamilyCode,
                        departamento == null ? "" : departamento.FamilyName);
                })
                .ToList();
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
