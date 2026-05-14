using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class FabricantesController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<FabricanteViewModel> Fabricantes = new List<FabricanteViewModel>
        {
            CreateFabricante(1, "Cooperativa de Productores de Leche Dos Pinos R.L.", "Fabricante nacional de lacteos y bebidas refrigeradas.", 18),
            CreateFabricante(2, "Productos Lizano S.A.", "Fabricante de salsas y condimentos tradicionales de Costa Rica.", 9),
            CreateFabricante(3, "Cafe Britt Costa Rica S.A.", "Tostador y productor de cafe costarricense para retail y turismo.", 7),
            CreateFabricante(4, "Compania Numar S.A.", "Fabricante y distribuidor de productos alimenticios de consumo masivo.", 12),
            CreateFabricante(5, "Irex de Costa Rica S.A.", "Fabricante costarricense de productos de limpieza y cuidado del hogar.", 14),
            CreateFabricante(6, "Pozuelo Pro S.A.", "Fabricante local de galletas y snacks de alta rotacion.", 11),
            CreateFabricante(7, "Florida Bebidas S.A.", "Fabricante y distribuidor nacional de bebidas.", 10)
        };

        public ActionResult Inicio()
        {
            List<FabricanteViewModel> model;
            lock (SyncRoot)
            {
                model = Fabricantes.Select(Clone).OrderBy(x => x.Descripcion).ToList();
            }

            return View(model);
        }

        public ActionResult Registro(int? id)
        {
            FabricanteViewModel model = null;
            if (id.HasValue)
            {
                lock (SyncRoot)
                {
                    model = Fabricantes.Where(x => x.ID == id.Value).Select(Clone).FirstOrDefault();
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

            lock (SyncRoot)
            {
                var existing = Fabricantes.FirstOrDefault(x => x.ID == model.ID);
                if (existing == null)
                {
                    model.ID = Fabricantes.Count == 0 ? 1 : Fabricantes.Max(x => x.ID) + 1;
                    model.UsuarioCrea = GetCurrentUser();
                    model.FechaCrea = DateTime.Now;
                    Fabricantes.Add(Clone(model));
                    TempData["FabricanteMessage"] = "Fabricante creado correctamente.";
                }
                else
                {
                    model.UsuarioCrea = existing.UsuarioCrea;
                    model.FechaCrea = existing.FechaCrea;
                    model.UsuarioModifica = GetCurrentUser();
                    model.FechaModifica = DateTime.Now;
                    CopyValues(model, existing);
                    TempData["FabricanteMessage"] = "Fabricante actualizado correctamente.";
                }
            }

            return RedirectToAction("Inicio");
        }

        private void ValidateFabricante(FabricanteViewModel model)
        {
            if (model == null)
                return;

            lock (SyncRoot)
            {
                var exists = Fabricantes.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Descripcion, model.Descripcion, StringComparison.OrdinalIgnoreCase));

                if (exists)
                    ModelState.AddModelError("Descripcion", "Ya existe un fabricante con esta descripcion.");
            }
        }

        private static void Normalize(FabricanteViewModel model)
        {
            if (model == null)
                return;

            model.Descripcion = (model.Descripcion ?? "").Trim();
            model.Nota = (model.Nota ?? "").Trim();
        }

        private FabricanteViewModel CreateNewFabricante()
        {
            return new FabricanteViewModel
            {
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };
        }

        private static FabricanteViewModel CreateFabricante(int id, string descripcion, string nota, int cantidadArticulos)
        {
            return new FabricanteViewModel
            {
                ID = id,
                Descripcion = descripcion,
                Nota = nota,
                CantidadArticulos = cantidadArticulos,
                UsuarioCrea = "Soporte",
                FechaCrea = DateTime.Now.AddDays(-1)
            };
        }

        private static FabricanteViewModel Clone(FabricanteViewModel source)
        {
            if (source == null)
                return null;

            return new FabricanteViewModel
            {
                ID = source.ID,
                Descripcion = source.Descripcion,
                Nota = source.Nota,
                CantidadArticulos = source.CantidadArticulos,
                UsuarioCrea = source.UsuarioCrea,
                FechaCrea = source.FechaCrea,
                UsuarioModifica = source.UsuarioModifica,
                FechaModifica = source.FechaModifica
            };
        }

        private static void CopyValues(FabricanteViewModel source, FabricanteViewModel target)
        {
            target.Descripcion = source.Descripcion;
            target.Nota = source.Nota;
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
