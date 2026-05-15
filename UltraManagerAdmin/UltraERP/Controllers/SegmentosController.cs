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
    public class SegmentosController : Controller
    {
        public ActionResult Inicio()
        {
            try
            {
                List<SubCategoriaCatalogo> subCategorias = GetSubCategorias();
                List<SegmentoViewModel> model = new CT_ExtCentral_Segment()
                    .GetAll()
                    .Select(x => MapSegmento(x, subCategorias))
                    .OrderBy(x => x.SubCategoriaNombre)
                    .ThenBy(x => x.Codigo)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["SegmentoError"] = "No se pudo cargar segmentos desde SQL: " + ex.Message;
                return View(Enumerable.Empty<SegmentoViewModel>());
            }
        }

        public ActionResult Registro(int? id)
        {
            List<SubCategoriaCatalogo> subCategorias = GetSubCategorias();
            SegmentoViewModel model = null;

            if (id.HasValue)
            {
                try
                {
                    EN_ExtCentral_Segment segment = GetSegmentoById(id.Value);
                    if (segment == null)
                        TempData["SegmentoError"] = "No se encontro el segmento en SQL.";
                    else
                        model = MapSegmento(segment, subCategorias);
                }
                catch (Exception ex)
                {
                    TempData["SegmentoError"] = "No se pudo leer el segmento desde SQL: " + ex.Message;
                }
            }

            PrepareCatalogs(subCategorias);
            return View(model ?? CreateNewSegmento(subCategorias));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(SegmentoViewModel model)
        {
            List<SubCategoriaCatalogo> subCategorias = GetSubCategorias();
            Normalize(model);
            ApplySubCategoria(model, subCategorias);
            ValidateSegmento(model, subCategorias);

            if (!ModelState.IsValid)
            {
                PrepareCatalogs(subCategorias);
                return View(model);
            }

            try
            {
                EN_ExtCentral_Segment segment = new EN_ExtCentral_Segment
                {
                    ID = model.ID,
                    SubCategoryID = model.SubCategoriaID,
                    Code = model.Codigo,
                    Description = model.Descripcion
                };

                Respuesta response = new CT_ExtCentral_Segment().Save(segment);
                if (!response.Status)
                {
                    ModelState.AddModelError("", "SQL rechazo el guardado del segmento: " + response.Message);
                    PrepareCatalogs(subCategorias);
                    return View(model);
                }

                TempData["SegmentoMessage"] = model.ID > 0 ? "Segmento actualizado correctamente en SQL." : "Segmento creado correctamente en SQL.";
                return RedirectToAction("Inicio");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar el segmento en SQL. " + ex.Message);
                PrepareCatalogs(subCategorias);
                return View(model);
            }
        }

        private void ValidateSegmento(SegmentoViewModel model, List<SubCategoriaCatalogo> subCategorias)
        {
            if (model == null)
                return;

            if (model.SubCategoriaID <= 0 || !subCategorias.Any(x => x.ID == model.SubCategoriaID))
                ModelState.AddModelError("SubCategoriaID", "Seleccione una subcategoria valida.");

            try
            {
                List<SegmentoViewModel> segmentos = new CT_ExtCentral_Segment()
                    .GetAll()
                    .Select(x => MapSegmento(x, subCategorias))
                    .ToList();

                bool codeExists = segmentos.Any(x =>
                    x.ID != model.ID &&
                    x.SubCategoriaID == model.SubCategoriaID &&
                    String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));

                if (codeExists)
                    ModelState.AddModelError("Codigo", "Ya existe un segmento con este codigo en la subcategoria seleccionada.");

                bool nameExists = segmentos.Any(x =>
                    x.ID != model.ID &&
                    x.SubCategoriaID == model.SubCategoriaID &&
                    String.Equals(x.Descripcion, model.Descripcion, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    ModelState.AddModelError("Descripcion", "Ya existe un segmento con esta descripcion en la subcategoria seleccionada.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo validar el segmento contra SQL. " + ex.Message);
            }
        }

        private static void Normalize(SegmentoViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.Descripcion = (model.Descripcion ?? "").Trim();
            model.Nota = (model.Nota ?? "").Trim();
        }

        private static void ApplySubCategoria(SegmentoViewModel model, List<SubCategoriaCatalogo> subCategorias)
        {
            if (model == null)
                return;

            SubCategoriaCatalogo subCategoria = subCategorias.FirstOrDefault(x => x.ID == model.SubCategoriaID);
            if (subCategoria == null)
            {
                model.SubCategoriaCodigo = "";
                model.SubCategoriaNombre = "";
                model.CategoriaCodigo = "";
                model.CategoriaNombre = "";
                model.DepartamentoCodigo = "";
                model.DepartamentoNombre = "";
                model.FamiliaCodigo = "";
                model.FamiliaNombre = "";
                return;
            }

            model.SubCategoriaCodigo = subCategoria.Codigo;
            model.SubCategoriaNombre = subCategoria.Nombre;
            model.CategoriaCodigo = subCategoria.CategoriaCodigo;
            model.CategoriaNombre = subCategoria.CategoriaNombre;
            model.DepartamentoCodigo = subCategoria.DepartamentoCodigo;
            model.DepartamentoNombre = subCategoria.DepartamentoNombre;
            model.FamiliaCodigo = subCategoria.FamiliaCodigo;
            model.FamiliaNombre = subCategoria.FamiliaNombre;
        }

        private void PrepareCatalogs(List<SubCategoriaCatalogo> subCategorias)
        {
            ViewBag.SubCategorias = subCategorias
                .Select(x => new SelectListItem
                {
                    Text = x.FamiliaCodigo + " / " + x.DepartamentoCodigo + " / " + x.CategoriaCodigo + " / " + x.Codigo + " - " + x.Nombre,
                    Value = x.ID.ToString()
                })
                .ToList();
        }

        private SegmentoViewModel CreateNewSegmento(List<SubCategoriaCatalogo> subCategorias)
        {
            SubCategoriaCatalogo firstSubCategoria = subCategorias.FirstOrDefault();
            var model = new SegmentoViewModel
            {
                SubCategoriaID = firstSubCategoria == null ? 0 : firstSubCategoria.ID,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };

            ApplySubCategoria(model, subCategorias);
            return model;
        }

        private EN_ExtCentral_Segment GetSegmentoById(int id)
        {
            return new CT_ExtCentral_Segment()
                .GetAll()
                .FirstOrDefault(x => x.ID == id);
        }

        private static SegmentoViewModel MapSegmento(EN_ExtCentral_Segment segment)
        {
            return MapSegmento(segment, GetSubCategorias());
        }

        private static SegmentoViewModel MapSegmento(EN_ExtCentral_Segment segment, List<SubCategoriaCatalogo> subCategorias)
        {
            if (segment == null)
                return null;

            SubCategoriaCatalogo subCategoria = subCategorias.FirstOrDefault(x => x.ID == segment.SubCategoryID);

            return new SegmentoViewModel
            {
                ID = segment.ID,
                SubCategoriaID = segment.SubCategoryID,
                SubCategoriaCodigo = subCategoria == null ? "" : subCategoria.Codigo,
                SubCategoriaNombre = subCategoria == null ? "" : subCategoria.Nombre,
                CategoriaCodigo = subCategoria == null ? "" : subCategoria.CategoriaCodigo,
                CategoriaNombre = subCategoria == null ? "" : subCategoria.CategoriaNombre,
                DepartamentoCodigo = subCategoria == null ? "" : subCategoria.DepartamentoCodigo,
                DepartamentoNombre = subCategoria == null ? "" : subCategoria.DepartamentoNombre,
                FamiliaCodigo = subCategoria == null ? "" : subCategoria.FamiliaCodigo,
                FamiliaNombre = subCategoria == null ? "" : subCategoria.FamiliaNombre,
                Codigo = segment.Code,
                Descripcion = segment.Description,
                Nota = "",
                CantidadArticulos = 0
            };
        }

        private static List<SubCategoriaCatalogo> GetSubCategorias()
        {
            List<EN_Department> departamentos = new CT_Department().GetAll("", 0, 0);
            List<EN_Category> categorias = new CT_Category().GetAll("", 0, 0);

            return new CT_ExtCentral_SubCategory()
                .GetAll("", 0, 0)
                .Select(x =>
                {
                    EN_Category categoria = categorias.FirstOrDefault(c => c.ID == x.CategoryID);
                    EN_Department departamento = categoria == null ? null : departamentos.FirstOrDefault(d => d.ID == categoria.DepartmentID);

                    return new SubCategoriaCatalogo(
                        x.ID,
                        x.Code,
                        x.Description,
                        categoria == null ? "" : categoria.Code,
                        categoria == null ? "" : categoria.Name,
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

        private class SubCategoriaCatalogo
        {
            public SubCategoriaCatalogo(int id, string codigo, string nombre, string categoriaCodigo, string categoriaNombre, string departamentoCodigo, string departamentoNombre, string familiaCodigo, string familiaNombre)
            {
                ID = id;
                Codigo = codigo;
                Nombre = nombre;
                CategoriaCodigo = categoriaCodigo;
                CategoriaNombre = categoriaNombre;
                DepartamentoCodigo = departamentoCodigo;
                DepartamentoNombre = departamentoNombre;
                FamiliaCodigo = familiaCodigo;
                FamiliaNombre = familiaNombre;
            }

            public int ID { get; private set; }
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public string CategoriaCodigo { get; private set; }
            public string CategoriaNombre { get; private set; }
            public string DepartamentoCodigo { get; private set; }
            public string DepartamentoNombre { get; private set; }
            public string FamiliaCodigo { get; private set; }
            public string FamiliaNombre { get; private set; }
        }
    }
}
