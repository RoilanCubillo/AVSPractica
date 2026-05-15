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
    public class DepartamentosController : Controller
    {
        public ActionResult Inicio()
        {
            try
            {
                List<DepartamentoViewModel> model = new CT_Department()
                    .GetAll("", 0, 0)
                    .Select(MapDepartamento)
                    .OrderBy(x => x.FamiliaNombre)
                    .ThenBy(x => x.Codigo)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["DepartamentoError"] = "No se pudo cargar departamentos desde SQL: " + ex.Message;
                return View(Enumerable.Empty<DepartamentoViewModel>());
            }
        }

        public ActionResult Registro(int? id)
        {
            DepartamentoViewModel model = null;

            if (id.HasValue)
            {
                try
                {
                    EN_Department department = GetDepartamentoById(id.Value);
                    if (department == null)
                        TempData["DepartamentoError"] = "No se encontro el departamento en SQL.";
                    else
                        model = MapDepartamento(department);
                }
                catch (Exception ex)
                {
                    TempData["DepartamentoError"] = "No se pudo leer el departamento desde SQL: " + ex.Message;
                }
            }

            PrepareCatalogs();
            return View(model ?? CreateNewDepartamento());
        }

        private EN_Department GetDepartamentoById(int id)
        {
            return new CT_Department()
                .GetAll("", 0, 0)
                .FirstOrDefault(x => x.ID == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(DepartamentoViewModel model)
        {
            Normalize(model);
            ApplyFamilia(model);
            ValidateDepartamento(model);

            if (!ModelState.IsValid)
            {
                PrepareCatalogs();
                return View(model);
            }

            try
            {
                EN_Department department = new EN_Department
                {
                    HQID = model.HQID,
                    ID = model.ID,
                    FamilyID = model.FamiliaID,
                    Code = model.Codigo,
                    Name = model.Nombre
                };

                Respuesta response = new CT_Department().Save(department);
                if (!response.Status)
                {
                    ModelState.AddModelError("", "SQL rechazo el guardado del departamento: " + response.Message);
                    PrepareCatalogs();
                    return View(model);
                }

                TempData["DepartamentoMessage"] = model.ID > 0 ? "Departamento actualizado correctamente en SQL." : "Departamento creado correctamente en SQL.";
                return RedirectToAction("Inicio");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar el departamento en SQL. " + ex.Message);
                PrepareCatalogs();
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            return Json(new JsonResponse(
                "Pendiente de migracion",
                "El catalogo de departamentos ya consulta SQL. La activacion e inactivacion se conectara cuando el procedimiento exponga estado.",
                null,
                false));
        }

        private void ValidateDepartamento(DepartamentoViewModel model)
        {
            if (model == null)
                return;

            try
            {
                List<DepartamentoViewModel> departments = new CT_Department()
                    .GetAll("", 0, 0)
                    .Select(MapDepartamento)
                    .ToList();

                bool codeExists = departments.Any(x =>
                    x.ID != model.ID &&
                    x.FamiliaID == model.FamiliaID &&
                    String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));

                if (codeExists)
                    ModelState.AddModelError("Codigo", "Ya existe un departamento con este codigo en la familia seleccionada.");

                bool nameExists = departments.Any(x =>
                    x.ID != model.ID &&
                    x.FamiliaID == model.FamiliaID &&
                    String.Equals(x.Nombre, model.Nombre, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    ModelState.AddModelError("Nombre", "Ya existe un departamento con este nombre en la familia seleccionada.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo validar el departamento contra SQL. " + ex.Message);
            }
        }

        private static DepartamentoViewModel MapDepartamento(EN_Department department)
        {
            return new DepartamentoViewModel
            {
                ID = department.ID,
                HQID = department.HQID,
                FamiliaID = department.FamilyID,
                FamiliaCodigo = department.FamilyCode,
                FamiliaNombre = department.FamilyName,
                Codigo = department.Code,
                Nombre = department.Name,
                Activo = true
            };
        }

        private static void Normalize(DepartamentoViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.Nombre = (model.Nombre ?? "").Trim();
        }

        private void ApplyFamilia(DepartamentoViewModel model)
        {
            if (model == null)
                return;

            EN_ExtCentral_Family family = GetFamilias().FirstOrDefault(x => x.ID == model.FamiliaID);
            if (family == null)
            {
                model.FamiliaCodigo = model.FamiliaID == 0 ? "-NO DEFINIDO-" : "";
                model.FamiliaNombre = model.FamiliaID == 0 ? "-NO DEFINIDO-" : "";
                return;
            }

            model.FamiliaCodigo = family.Code;
            model.FamiliaNombre = family.Name;
        }

        private void PrepareCatalogs()
        {
            List<SelectListItem> families = GetFamilias()
                .Select(x => new SelectListItem { Text = x.Code + " - " + x.Name, Value = x.ID.ToString() })
                .ToList();

            families.Insert(0, new SelectListItem { Text = "-NO DEFINIDO-", Value = "0" });
            ViewBag.Familias = families;
        }

        private List<EN_ExtCentral_Family> GetFamilias()
        {
            return new CT_ExtCentral_Family().GetAll("", 0, 0);
        }

        private DepartamentoViewModel CreateNewDepartamento()
        {
            var model = new DepartamentoViewModel
            {
                Activo = true
            };

            ApplyFamilia(model);
            return model;
        }
    }
}
