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
    public class MarcasController : Controller
    {
        public ActionResult Inicio()
        {
            try
            {
                List<MarcaViewModel> model = new CT_ExtCentral_Brand()
                    .GetAll()
                    .Select(MapMarca)
                    .OrderBy(x => x.Descripcion)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["MarcaError"] = "No se pudo cargar marcas desde SQL: " + ex.Message;
                return View(Enumerable.Empty<MarcaViewModel>());
            }
        }

        public ActionResult Registro(int? id)
        {
            MarcaViewModel model = null;

            if (id.HasValue)
            {
                try
                {
                    EN_ExtCentral_Brand brand = GetMarcaById(id.Value);
                    if (brand == null)
                        TempData["MarcaError"] = "No se encontro la marca en SQL.";
                    else
                        model = MapMarca(brand);
                }
                catch (Exception ex)
                {
                    TempData["MarcaError"] = "No se pudo leer la marca desde SQL: " + ex.Message;
                }
            }

            return View(model ?? CreateNewMarca());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(MarcaViewModel model)
        {
            Normalize(model);
            ValidateMarca(model);

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                Respuesta response = new CT_ExtCentral_Brand().Save(model.ID, model.Descripcion);
                if (!response.Status)
                {
                    ModelState.AddModelError("", "SQL rechazo el guardado de la marca: " + response.Message);
                    return View(model);
                }

                TempData["MarcaMessage"] = model.ID > 0 ? "Marca actualizada correctamente en SQL." : "Marca creada correctamente en SQL.";
                return RedirectToAction("Inicio");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar la marca en SQL. " + ex.Message);
                return View(model);
            }
        }

        private void ValidateMarca(MarcaViewModel model)
        {
            if (model == null)
                return;

            try
            {
                List<MarcaViewModel> marcas = new CT_ExtCentral_Brand()
                    .GetAll()
                    .Select(MapMarca)
                    .ToList();

                bool exists = marcas.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Descripcion, model.Descripcion, StringComparison.OrdinalIgnoreCase));

                if (exists)
                    ModelState.AddModelError("Descripcion", "Ya existe una marca con esta descripcion.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo validar la marca contra SQL. " + ex.Message);
            }
        }

        private static void Normalize(MarcaViewModel model)
        {
            if (model == null)
                return;

            model.Descripcion = (model.Descripcion ?? "").Trim();
            model.Nota = (model.Nota ?? "").Trim();
        }

        private EN_ExtCentral_Brand GetMarcaById(int id)
        {
            return new CT_ExtCentral_Brand()
                .GetAll()
                .FirstOrDefault(x => x.ID == id);
        }

        private static MarcaViewModel MapMarca(EN_ExtCentral_Brand brand)
        {
            if (brand == null)
                return null;

            return new MarcaViewModel
            {
                ID = brand.ID,
                Descripcion = brand.Description,
                Nota = "",
                CantidadArticulos = 0
            };
        }

        private MarcaViewModel CreateNewMarca()
        {
            return new MarcaViewModel
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
