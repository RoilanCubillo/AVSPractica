using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class FamiliasController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<FamiliaViewModel> Familias = new List<FamiliaViewModel>
        {
            CreateFamilia(1, "ABAR", "Abarrotes", "Productos de consumo regular y rotacion alta.", true, 18),
            CreateFamilia(2, "MP", "Materia Prima", "Insumos usados en procesos internos o preparaciones.", true, 9),
            CreateFamilia(3, "EMP", "Empaque", "Materiales de empaque y presentacion.", true, 6),
            CreateFamilia(4, "PROMO", "Promociones", "Productos de cortesia, combos o lineas especiales.", false, 2)
        };

        public ActionResult Inicio()
        {
            List<FamiliaViewModel> model;
            lock (SyncRoot)
            {
                model = Familias.Select(Clone).OrderBy(x => x.Codigo).ToList();
            }

            return View(model);
        }

        public ActionResult Registro(int? id)
        {
            FamiliaViewModel model = null;
            if (id.HasValue)
            {
                lock (SyncRoot)
                {
                    model = Familias.Where(x => x.ID == id.Value).Select(Clone).FirstOrDefault();
                }
            }

            return View(model ?? CreateNewFamilia());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(FamiliaViewModel model)
        {
            Normalize(model);
            ValidateFamilia(model);

            if (!ModelState.IsValid)
                return View(model);

            lock (SyncRoot)
            {
                var existing = Familias.FirstOrDefault(x => x.ID == model.ID);
                if (existing == null)
                {
                    model.ID = Familias.Count == 0 ? 1 : Familias.Max(x => x.ID) + 1;
                    model.UsuarioCrea = GetCurrentUser();
                    model.FechaCrea = DateTime.Now;
                    Familias.Add(Clone(model));
                    TempData["FamiliaMessage"] = "Familia creada correctamente.";
                }
                else
                {
                    model.UsuarioCrea = existing.UsuarioCrea;
                    model.FechaCrea = existing.FechaCrea;
                    model.UsuarioModifica = GetCurrentUser();
                    model.FechaModifica = DateTime.Now;
                    CopyValues(model, existing);
                    TempData["FamiliaMessage"] = "Familia actualizada correctamente.";
                }
            }

            return RedirectToAction("Inicio");
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            lock (SyncRoot)
            {
                var familia = Familias.FirstOrDefault(x => x.ID == id);
                if (familia == null)
                    return Json(new JsonResponse("Familia no encontrada.", "No se encontro la familia.", null, false));

                familia.Activa = !familia.Activa;
                familia.UsuarioModifica = GetCurrentUser();
                familia.FechaModifica = DateTime.Now;

                return Json(new JsonResponse("", familia.Activa ? "Familia activada." : "Familia inactivada.", Clone(familia), true));
            }
        }

        private void ValidateFamilia(FamiliaViewModel model)
        {
            if (model == null)
                return;

            lock (SyncRoot)
            {
                var codeExists = Familias.Any(x => x.ID != model.ID && String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));
                if (codeExists)
                    ModelState.AddModelError("Codigo", "Ya existe una familia con este codigo.");

                var nameExists = Familias.Any(x => x.ID != model.ID && String.Equals(x.Nombre, model.Nombre, StringComparison.OrdinalIgnoreCase));
                if (nameExists)
                    ModelState.AddModelError("Nombre", "Ya existe una familia con este nombre.");
            }
        }

        private static void Normalize(FamiliaViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.Nombre = (model.Nombre ?? "").Trim();
            model.Descripcion = (model.Descripcion ?? "").Trim();
        }

        private FamiliaViewModel CreateNewFamilia()
        {
            return new FamiliaViewModel
            {
                Activa = true,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };
        }

        private static FamiliaViewModel CreateFamilia(int id, string codigo, string nombre, string descripcion, bool activa, int cantidadArticulos)
        {
            return new FamiliaViewModel
            {
                ID = id,
                Codigo = codigo,
                Nombre = nombre,
                Descripcion = descripcion,
                Activa = activa,
                CantidadArticulos = cantidadArticulos,
                UsuarioCrea = "Soporte",
                FechaCrea = DateTime.Now.AddDays(-4)
            };
        }

        private static FamiliaViewModel Clone(FamiliaViewModel source)
        {
            if (source == null)
                return null;

            return new FamiliaViewModel
            {
                ID = source.ID,
                Codigo = source.Codigo,
                Nombre = source.Nombre,
                Descripcion = source.Descripcion,
                Activa = source.Activa,
                CantidadArticulos = source.CantidadArticulos,
                UsuarioCrea = source.UsuarioCrea,
                FechaCrea = source.FechaCrea,
                UsuarioModifica = source.UsuarioModifica,
                FechaModifica = source.FechaModifica
            };
        }

        private static void CopyValues(FamiliaViewModel source, FamiliaViewModel target)
        {
            target.Codigo = source.Codigo;
            target.Nombre = source.Nombre;
            target.Descripcion = source.Descripcion;
            target.Activa = source.Activa;
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
    }
}
