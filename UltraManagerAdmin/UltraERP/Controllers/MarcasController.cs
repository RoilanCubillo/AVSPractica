using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class MarcasController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<MarcaViewModel> Marcas = new List<MarcaViewModel>
        {
            CreateMarca(1, "Dos Pinos", "Lacteos y bebidas de alta presencia en Costa Rica.", 18),
            CreateMarca(2, "Salsa Lizano", "Salsas y condimentos reconocidos en la mesa costarricense.", 9),
            CreateMarca(3, "Cafe Britt", "Cafe costarricense para retail y turismo.", 7),
            CreateMarca(4, "Cafe 1820", "Cafe molido de consumo masivo nacional.", 8),
            CreateMarca(5, "Irex", "Productos de limpieza y cuidado del hogar.", 12),
            CreateMarca(6, "Tio Pelon", "Arroces y granos disponibles en supermercados nacionales.", 10),
            CreateMarca(7, "Pozuelo", "Galletas y snacks populares en Costa Rica.", 11),
            CreateMarca(8, "Tropical", "Bebidas listas y refrescos distribuidos localmente.", 6)
        };

        public ActionResult Inicio()
        {
            List<MarcaViewModel> model;
            lock (SyncRoot)
            {
                model = Marcas.Select(Clone).OrderBy(x => x.Descripcion).ToList();
            }

            return View(model);
        }

        public ActionResult Registro(int? id)
        {
            MarcaViewModel model = null;
            if (id.HasValue)
            {
                lock (SyncRoot)
                {
                    model = Marcas.Where(x => x.ID == id.Value).Select(Clone).FirstOrDefault();
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

            lock (SyncRoot)
            {
                var existing = Marcas.FirstOrDefault(x => x.ID == model.ID);
                if (existing == null)
                {
                    model.ID = Marcas.Count == 0 ? 1 : Marcas.Max(x => x.ID) + 1;
                    model.UsuarioCrea = GetCurrentUser();
                    model.FechaCrea = DateTime.Now;
                    Marcas.Add(Clone(model));
                    TempData["MarcaMessage"] = "Marca creada correctamente.";
                }
                else
                {
                    model.UsuarioCrea = existing.UsuarioCrea;
                    model.FechaCrea = existing.FechaCrea;
                    model.UsuarioModifica = GetCurrentUser();
                    model.FechaModifica = DateTime.Now;
                    CopyValues(model, existing);
                    TempData["MarcaMessage"] = "Marca actualizada correctamente.";
                }
            }

            return RedirectToAction("Inicio");
        }

        private void ValidateMarca(MarcaViewModel model)
        {
            if (model == null)
                return;

            lock (SyncRoot)
            {
                var exists = Marcas.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Descripcion, model.Descripcion, StringComparison.OrdinalIgnoreCase));

                if (exists)
                    ModelState.AddModelError("Descripcion", "Ya existe una marca con esta descripcion.");
            }
        }

        private static void Normalize(MarcaViewModel model)
        {
            if (model == null)
                return;

            model.Descripcion = (model.Descripcion ?? "").Trim();
            model.Nota = (model.Nota ?? "").Trim();
        }

        private MarcaViewModel CreateNewMarca()
        {
            return new MarcaViewModel
            {
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };
        }

        private static MarcaViewModel CreateMarca(int id, string descripcion, string nota, int cantidadArticulos)
        {
            return new MarcaViewModel
            {
                ID = id,
                Descripcion = descripcion,
                Nota = nota,
                CantidadArticulos = cantidadArticulos,
                UsuarioCrea = "Soporte",
                FechaCrea = DateTime.Now.AddDays(-1)
            };
        }

        private static MarcaViewModel Clone(MarcaViewModel source)
        {
            if (source == null)
                return null;

            return new MarcaViewModel
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

        private static void CopyValues(MarcaViewModel source, MarcaViewModel target)
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
