using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class CasasComercialesController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<CasaComercialViewModel> CasasComerciales = new List<CasaComercialViewModel>
        {
            CreateCasa(1, "CC-DP", "DOSPINOS", "Casa Comercial Dos Pinos", "compras@dospinos.co.cr", "2247-8000", false, 18),
            CreateCasa(2, "CC-LIZ", "LIZANO", "Casa Comercial Lizano", "abasto@lizano.cr", "2220-4100", false, 9),
            CreateCasa(3, "CC-BRT", "BRITT", "Casa Comercial Cafe Britt", "retail@brittcostarica.com", "2277-1600", false, 7),
            CreateCasa(4, "CC-IRX", "IREX", "Casa Comercial Irex", "ventas@irex.cr", "2296-2200", false, 12),
            CreateCasa(5, "CC-FLB", "FLORIDA", "Casa Comercial Florida Bebidas", "abastecimiento@florida.co.cr", "2437-7000", false, 11),
            CreateCasa(6, "CC-NMR", "NUMAR", "Casa Comercial Numar", "servicioalcliente@numar.net", "2209-9800", true, 4)
        };

        public ActionResult Inicio()
        {
            List<CasaComercialViewModel> model;
            lock (SyncRoot)
            {
                model = CasasComerciales.Select(Clone).OrderBy(x => x.Nombre).ToList();
            }

            return View(model);
        }

        public ActionResult Registro(int? id)
        {
            CasaComercialViewModel model = null;
            if (id.HasValue)
            {
                lock (SyncRoot)
                {
                    model = CasasComerciales.Where(x => x.ID == id.Value).Select(Clone).FirstOrDefault();
                }
            }

            return View(model ?? CreateNewCasa());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(CasaComercialViewModel model)
        {
            Normalize(model);
            ValidateCasa(model);

            if (!ModelState.IsValid)
                return View(model);

            lock (SyncRoot)
            {
                var existing = CasasComerciales.FirstOrDefault(x => x.ID == model.ID);
                if (existing == null)
                {
                    model.ID = CasasComerciales.Count == 0 ? 1 : CasasComerciales.Max(x => x.ID) + 1;
                    model.UsuarioCrea = GetCurrentUser();
                    model.FechaCrea = DateTime.Now;
                    CasasComerciales.Add(Clone(model));
                    TempData["CasaComercialMessage"] = "Casa comercial creada correctamente.";
                }
                else
                {
                    model.UsuarioCrea = existing.UsuarioCrea;
                    model.FechaCrea = existing.FechaCrea;
                    model.UsuarioModifica = GetCurrentUser();
                    model.FechaModifica = DateTime.Now;
                    CopyValues(model, existing);
                    TempData["CasaComercialMessage"] = "Casa comercial actualizada correctamente.";
                }
            }

            return RedirectToAction("Inicio");
        }

        private void ValidateCasa(CasaComercialViewModel model)
        {
            if (model == null)
                return;

            lock (SyncRoot)
            {
                var duplicatedCode = CasasComerciales.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));
                if (duplicatedCode)
                    ModelState.AddModelError("Codigo", "Ya existe una casa comercial con este codigo.");

                var duplicatedName = CasasComerciales.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Nombre, model.Nombre, StringComparison.OrdinalIgnoreCase));
                if (duplicatedName)
                    ModelState.AddModelError("Nombre", "Ya existe una casa comercial con este nombre.");
            }
        }

        private static void Normalize(CasaComercialViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.CodigoExtra = (model.CodigoExtra ?? "").Trim().ToUpperInvariant();
            model.Nombre = (model.Nombre ?? "").Trim();
            model.CorreoElectronico = (model.CorreoElectronico ?? "").Trim();
            model.Telefono = (model.Telefono ?? "").Trim();
            model.ArticulosAsociados = Math.Max(0, model.ArticulosAsociados);
        }

        private CasaComercialViewModel CreateNewCasa()
        {
            return new CasaComercialViewModel
            {
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };
        }

        private static CasaComercialViewModel CreateCasa(int id, string codigo, string codigoExtra, string nombre, string correo, string telefono, bool inactivo, int articulosAsociados)
        {
            return new CasaComercialViewModel
            {
                ID = id,
                Codigo = codigo,
                CodigoExtra = codigoExtra,
                Nombre = nombre,
                CorreoElectronico = correo,
                Telefono = telefono,
                Inactivo = inactivo,
                ArticulosAsociados = articulosAsociados,
                UsuarioCrea = "Soporte",
                FechaCrea = DateTime.Now.AddDays(-5),
                UsuarioModifica = inactivo ? "Soporte" : "Compras",
                FechaModifica = DateTime.Now.AddDays(-1)
            };
        }

        private static CasaComercialViewModel Clone(CasaComercialViewModel source)
        {
            if (source == null)
                return null;

            return new CasaComercialViewModel
            {
                ID = source.ID,
                Codigo = source.Codigo,
                CodigoExtra = source.CodigoExtra,
                Nombre = source.Nombre,
                CorreoElectronico = source.CorreoElectronico,
                Telefono = source.Telefono,
                Inactivo = source.Inactivo,
                ArticulosAsociados = source.ArticulosAsociados,
                UsuarioCrea = source.UsuarioCrea,
                FechaCrea = source.FechaCrea,
                UsuarioModifica = source.UsuarioModifica,
                FechaModifica = source.FechaModifica
            };
        }

        private static void CopyValues(CasaComercialViewModel source, CasaComercialViewModel target)
        {
            target.Codigo = source.Codigo;
            target.CodigoExtra = source.CodigoExtra;
            target.Nombre = source.Nombre;
            target.CorreoElectronico = source.CorreoElectronico;
            target.Telefono = source.Telefono;
            target.Inactivo = source.Inactivo;
            target.ArticulosAsociados = source.ArticulosAsociados;
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
