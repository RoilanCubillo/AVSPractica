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
    public class CategoriasController : Controller
    {
        public ActionResult Inicio()
        {
            try
            {
                List<CategoriaViewModel> model = new CT_Category()
                    .GetAll("", 0, 0)
                    .Select(MapCategoria)
                    .OrderBy(x => x.DepartamentoNombre)
                    .ThenBy(x => x.Codigo)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["CategoriaError"] = "No se pudo cargar categorias desde SQL: " + ex.Message;
                return View(Enumerable.Empty<CategoriaViewModel>());
            }
        }

        public ActionResult Registro(int? id)
        {
            List<DepartamentoCatalogo> departamentos = GetDepartamentos();
            CategoriaViewModel model = null;
            if (id.HasValue)
            {
                try
                {
                    EN_Category category = GetCategoriaById(id.Value);
                    if (category == null)
                        TempData["CategoriaError"] = "No se encontro la categoria en SQL.";
                    else
                        model = MapCategoria(category, departamentos);
                }
                catch (Exception ex)
                {
                    TempData["CategoriaError"] = "No se pudo leer la categoria desde SQL: " + ex.Message;
                }
            }

            PrepareCatalogs(departamentos);
            return View(model ?? CreateNewCategoria(departamentos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(CategoriaViewModel model)
        {
            List<DepartamentoCatalogo> departamentos = GetDepartamentos();
            Normalize(model);
            ApplyDepartamento(model, departamentos);
            ValidateCategoria(model, departamentos);

            if (!ModelState.IsValid)
            {
                PrepareCatalogs(departamentos);
                return View(model);
            }

            try
            {
                EN_Category category = new EN_Category
                {
                    HQID = model.HQID,
                    ID = model.ID,
                    DepartmentID = model.DepartamentoID,
                    Code = model.Codigo,
                    Name = model.Nombre
                };

                Respuesta response = new CT_Category().Save(category);
                if (!response.Status)
                {
                    ModelState.AddModelError("", "SQL rechazo el guardado de la categoria: " + response.Message);
                    PrepareCatalogs(departamentos);
                    return View(model);
                }

                TempData["CategoriaMessage"] = model.ID > 0 ? "Categoria actualizada correctamente en SQL." : "Categoria creada correctamente en SQL.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar la categoria en SQL. " + ex.Message);
                PrepareCatalogs(departamentos);
                return View(model);
            }

            return RedirectToAction("Inicio");
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            return Json(new JsonResponse(
                "Pendiente de migracion",
                "El catalogo de categorias ya consulta SQL. La activacion e inactivacion se conectara cuando el procedimiento exponga estado.",
                null,
                false));
        }

        private void ValidateCategoria(CategoriaViewModel model, List<DepartamentoCatalogo> departamentos)
        {
            if (model == null)
                return;

            if (model.DepartamentoID <= 0 || !departamentos.Any(x => x.ID == model.DepartamentoID))
                ModelState.AddModelError("DepartamentoID", "Seleccione un departamento valido.");

            try
            {
                List<CategoriaViewModel> categorias = new CT_Category()
                    .GetAll("", 0, 0)
                    .Select(MapCategoria)
                    .ToList();

                bool codeExists = categorias.Any(x =>
                    x.ID != model.ID &&
                    x.DepartamentoID == model.DepartamentoID &&
                    String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));

                if (codeExists)
                    ModelState.AddModelError("Codigo", "Ya existe una categoria con este codigo en el departamento seleccionado.");

                bool nameExists = categorias.Any(x =>
                    x.ID != model.ID &&
                    x.DepartamentoID == model.DepartamentoID &&
                    String.Equals(x.Nombre, model.Nombre, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    ModelState.AddModelError("Nombre", "Ya existe una categoria con este nombre en el departamento seleccionado.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo validar la categoria contra SQL. " + ex.Message);
            }
        }

        private static void Normalize(CategoriaViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.Nombre = (model.Nombre ?? "").Trim();
            model.Descripcion = (model.Descripcion ?? "").Trim();
        }

        private static void ApplyDepartamento(CategoriaViewModel model)
        {
            ApplyDepartamento(model, GetDepartamentos());
        }

        private static void ApplyDepartamento(CategoriaViewModel model, List<DepartamentoCatalogo> departamentos)
        {
            if (model == null)
                return;

            DepartamentoCatalogo departamento = departamentos.FirstOrDefault(x => x.ID == model.DepartamentoID);
            if (departamento == null)
            {
                model.DepartamentoCodigo = "";
                model.DepartamentoNombre = "";
                model.FamiliaCodigo = "";
                model.FamiliaNombre = "";
                return;
            }

            model.DepartamentoCodigo = departamento.Codigo;
            model.DepartamentoNombre = departamento.Nombre;
            model.FamiliaCodigo = departamento.FamiliaCodigo;
            model.FamiliaNombre = departamento.FamiliaNombre;
        }

        private void PrepareCatalogs()
        {
            PrepareCatalogs(GetDepartamentos());
        }

        private void PrepareCatalogs(List<DepartamentoCatalogo> departamentos)
        {
            ViewBag.Departamentos = departamentos
                .Select(x => new SelectListItem { Text = x.FamiliaCodigo + " / " + x.Codigo + " - " + x.Nombre, Value = x.ID.ToString() })
                .ToList();
        }

        private CategoriaViewModel CreateNewCategoria()
        {
            return CreateNewCategoria(GetDepartamentos());
        }

        private CategoriaViewModel CreateNewCategoria(List<DepartamentoCatalogo> departamentos)
        {
            DepartamentoCatalogo firstDepartamento = departamentos.FirstOrDefault();
            var model = new CategoriaViewModel
            {
                Activa = true,
                DepartamentoID = firstDepartamento == null ? 0 : firstDepartamento.ID,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };

            ApplyDepartamento(model, departamentos);
            return model;
        }

        private static CategoriaViewModel MapCategoria(EN_Category category)
        {
            return MapCategoria(category, GetDepartamentos());
        }

        private static CategoriaViewModel MapCategoria(EN_Category category, List<DepartamentoCatalogo> departamentos)
        {
            if (category == null)
                return null;

            DepartamentoCatalogo departamento = departamentos.FirstOrDefault(x => x.ID == category.DepartmentID);

            return new CategoriaViewModel
            {
                ID = category.ID,
                HQID = category.HQID,
                DepartamentoID = category.DepartmentID,
                DepartamentoCodigo = departamento == null ? "" : departamento.Codigo,
                DepartamentoNombre = departamento == null ? "" : departamento.Nombre,
                FamiliaCodigo = departamento == null ? "" : departamento.FamiliaCodigo,
                FamiliaNombre = departamento == null ? "" : departamento.FamiliaNombre,
                Codigo = category.Code,
                Nombre = category.Name,
                Descripcion = "",
                Activa = true
            };
        }

        private EN_Category GetCategoriaById(int id)
        {
            return new CT_Category()
                .GetAll("", 0, 0)
                .FirstOrDefault(x => x.ID == id);
        }

        private static List<DepartamentoCatalogo> GetDepartamentos()
        {
            return new CT_Department()
                .GetAll("", 0, 0)
                .Select(x => new DepartamentoCatalogo(x.ID, x.Code, x.Name, x.FamilyCode, x.FamilyName))
                .ToList();
        }

        private string GetCurrentUser()
        {
            return User != null && User.Identity != null && !String.IsNullOrWhiteSpace(User.Identity.Name)
                ? User.Identity.Name
                : "Soporte";
        }

        private class DepartamentoCatalogo
        {
            public DepartamentoCatalogo(int id, string codigo, string nombre, string familiaCodigo, string familiaNombre)
            {
                ID = id;
                Codigo = codigo;
                Nombre = nombre;
                FamiliaCodigo = familiaCodigo;
                FamiliaNombre = familiaNombre;
            }

            public int ID { get; private set; }
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public string FamiliaCodigo { get; private set; }
            public string FamiliaNombre { get; private set; }
        }
    }
}
