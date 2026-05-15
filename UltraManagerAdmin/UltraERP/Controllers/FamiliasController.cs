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
    public class FamiliasController : Controller
    {
        public ActionResult Inicio()
        {
            try
            {
                List<FamiliaViewModel> model = new CT_ExtCentral_Family()
                    .GetAll("", 0, 0)
                    .Select(MapFamilia)
                    .OrderBy(x => x.Codigo)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["FamiliaError"] = "No se pudo cargar familias desde SQL: " + ex.Message;
                return View(Enumerable.Empty<FamiliaViewModel>());
            }
        }

        public ActionResult Registro(int? id)
        {
            FamiliaViewModel model = null;

            if (id.HasValue)
            {
                try
                {
                    EN_ExtCentral_Family family = GetFamiliaById(id.Value);
                    if (family == null)
                        TempData["FamiliaError"] = "No se encontro la familia en SQL.";
                    else
                        model = MapFamilia(family);
                }
                catch (Exception ex)
                {
                    TempData["FamiliaError"] = "No se pudo leer la familia desde SQL: " + ex.Message;
                }
            }

            return View(model ?? CreateNewFamilia());
        }

        private EN_ExtCentral_Family GetFamiliaById(int id)
        {
            return new CT_ExtCentral_Family()
                .GetAll("", 0, 0)
                .FirstOrDefault(x => x.ID == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(FamiliaViewModel model)
        {
            Normalize(model);
            ValidateFamilia(model);

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                EN_ExtCentral_Family family = new EN_ExtCentral_Family
                {
                    ID = model.ID,
                    Code = model.Codigo,
                    Name = model.Nombre
                };

                Respuesta response = new CT_ExtCentral_Family().Save(family);
                if (!response.Status)
                {
                    ModelState.AddModelError("", "SQL rechazo el guardado de la familia: " + response.Message);
                    return View(model);
                }

                TempData["FamiliaMessage"] = model.ID > 0 ? "Familia actualizada correctamente en SQL." : "Familia creada correctamente en SQL.";
                return RedirectToAction("Inicio");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar la familia en SQL. " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            return Json(new JsonResponse(
                "Pendiente de migracion",
                "El catalogo de familias ya consulta SQL. La activacion e inactivacion se conectara cuando el procedimiento exponga estado.",
                null,
                false));
        }

        private void ValidateFamilia(FamiliaViewModel model)
        {
            if (model == null)
                return;

            try
            {
                List<EN_ExtCentral_Family> families = new CT_ExtCentral_Family().GetAll("", 0, 0);

                bool codeExists = families.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Code, model.Codigo, StringComparison.OrdinalIgnoreCase));

                if (codeExists)
                    ModelState.AddModelError("Codigo", "Ya existe una familia con este codigo.");

                bool nameExists = families.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Name, model.Nombre, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    ModelState.AddModelError("Nombre", "Ya existe una familia con este nombre.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo validar la familia contra SQL. " + ex.Message);
            }
        }

        private static FamiliaViewModel MapFamilia(EN_ExtCentral_Family family)
        {
            return new FamiliaViewModel
            {
                ID = family.ID,
                Codigo = family.Code,
                Nombre = family.Name,
                Activa = true
            };
        }

        private static void Normalize(FamiliaViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.Nombre = (model.Nombre ?? "").Trim();
        }

        private FamiliaViewModel CreateNewFamilia()
        {
            return new FamiliaViewModel
            {
                Activa = true
            };
        }
    }
}
