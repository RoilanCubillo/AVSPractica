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
    public class FabricantesController : Controller
    {
        public ActionResult Inicio()
        {
            try
            {
                List<FabricanteViewModel> model = new CT_ExtCentral_Manufacturer()
                    .GetAll()
                    .Select(MapFabricante)
                    .OrderBy(x => x.Descripcion)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["FabricanteError"] = "No se pudo cargar fabricantes desde SQL: " + ex.Message;
                return View(Enumerable.Empty<FabricanteViewModel>());
            }
        }

        public ActionResult Registro(int? id)
        {
            FabricanteViewModel model = null;

            if (id.HasValue)
            {
                try
                {
                    EN_ExtCentral_Manufacturer manufacturer = GetFabricanteById(id.Value);
                    if (manufacturer == null)
                        TempData["FabricanteError"] = "No se encontro el fabricante en SQL.";
                    else
                        model = MapFabricante(manufacturer);
                }
                catch (Exception ex)
                {
                    TempData["FabricanteError"] = "No se pudo leer el fabricante desde SQL: " + ex.Message;
                }
            }

            return View(model ?? CreateNewFabricante());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(FabricanteViewModel model)
        {
            Normalize(model);
            ValidateFabricante(model);

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                Respuesta response = new CT_ExtCentral_Manufacturer().Save(model.ID, model.Descripcion);
                if (!response.Status)
                {
                    ModelState.AddModelError("", "SQL rechazo el guardado del fabricante: " + response.Message);
                    return View(model);
                }

                TempData["FabricanteMessage"] = model.ID > 0 ? "Fabricante actualizado correctamente en SQL." : "Fabricante creado correctamente en SQL.";
                return RedirectToAction("Inicio");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar el fabricante en SQL. " + ex.Message);
                return View(model);
            }
        }

        private void ValidateFabricante(FabricanteViewModel model)
        {
            if (model == null)
                return;

            try
            {
                List<FabricanteViewModel> fabricantes = new CT_ExtCentral_Manufacturer()
                    .GetAll()
                    .Select(MapFabricante)
                    .ToList();

                bool exists = fabricantes.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Descripcion, model.Descripcion, StringComparison.OrdinalIgnoreCase));

                if (exists)
                    ModelState.AddModelError("Descripcion", "Ya existe un fabricante con esta descripcion.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo validar el fabricante contra SQL. " + ex.Message);
            }
        }

        private static void Normalize(FabricanteViewModel model)
        {
            if (model == null)
                return;

            model.Descripcion = (model.Descripcion ?? "").Trim();
            model.Nota = (model.Nota ?? "").Trim();
        }

        private EN_ExtCentral_Manufacturer GetFabricanteById(int id)
        {
            return new CT_ExtCentral_Manufacturer()
                .GetAll()
                .FirstOrDefault(x => x.ID == id);
        }

        private static FabricanteViewModel MapFabricante(EN_ExtCentral_Manufacturer manufacturer)
        {
            if (manufacturer == null)
                return null;

            return new FabricanteViewModel
            {
                ID = manufacturer.ID,
                Descripcion = manufacturer.Description,
                Nota = "",
                CantidadArticulos = 0
            };
        }

        private FabricanteViewModel CreateNewFabricante()
        {
            return new FabricanteViewModel
            {
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };
        }

        private string GetCurrentUser()
        {
            return User != null && User.Identity != null && !String.IsNullOrWhiteSpace(User.Identity.Name)
                ? User.Identity.Name
                : "Soporte";
        }
    }
}
