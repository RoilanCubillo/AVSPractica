using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class DescuentosController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<DescuentoViewModel> Descuentos = new List<DescuentoViewModel>
        {
            CreateDescuento(1, "Arroz tico por volumen", 1, true, 6m, 12m, 24m, 48m, 2095m, 1995m, 1895m, 1795m, 0m, 0m, 0m, 0m, 4, 8),
            CreateDescuento(2, "Lizano 3x2 pulperia", 2, false, 2m, 1m, 0m, 0m, 0m, 1200m, 0m, 0m, 0m, 0m, 0m, 0m, 6, 5),
            CreateDescuento(3, "Cafe CR mayoreo", 3, false, 6m, 12m, 24m, 48m, 0m, 0m, 0m, 0m, 5m, 8m, 10m, 12m, 5, 7),
            CreateDescuento(4, "Dos Pinos promo", 4, false, 3m, 1m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 15m, 0m, 0m, 3, 6),
            CreateDescuento(5, "Irex hogar", 3, true, 4m, 8m, 16m, 32m, 0m, 0m, 0m, 0m, 4m, 7m, 9m, 11m, 4, 4)
        };

        public ActionResult Inicio()
        {
            List<DescuentoViewModel> model;
            lock (SyncRoot)
            {
                model = Descuentos.Select(Clone).OrderBy(x => x.Descripcion).ToList();
            }

            return View(model);
        }

        public ActionResult Registro(int? id)
        {
            DescuentoViewModel model = null;
            if (id.HasValue)
            {
                lock (SyncRoot)
                {
                    model = Descuentos.Where(x => x.ID == id.Value).Select(Clone).FirstOrDefault();
                }
            }

            PrepareCatalogs();
            return View(model ?? CreateNewDescuento());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(DescuentoViewModel model)
        {
            Normalize(model);
            ValidateDescuento(model);

            if (!ModelState.IsValid)
            {
                PrepareCatalogs();
                return View(model);
            }

            lock (SyncRoot)
            {
                var existing = Descuentos.FirstOrDefault(x => x.ID == model.ID);
                if (existing == null)
                {
                    model.ID = Descuentos.Count == 0 ? 1 : Descuentos.Max(x => x.ID) + 1;
                    model.HQID = 0;
                    model.UsuarioCrea = GetCurrentUser();
                    model.FechaCrea = DateTime.Now;
                    Descuentos.Add(Clone(model));
                    TempData["DescuentoMessage"] = "Descuento creado correctamente.";
                }
                else
                {
                    model.HQID = existing.HQID;
                    model.UsuarioCrea = existing.UsuarioCrea;
                    model.FechaCrea = existing.FechaCrea;
                    model.UsuarioModifica = GetCurrentUser();
                    model.FechaModifica = DateTime.Now;
                    CopyValues(model, existing);
                    TempData["DescuentoMessage"] = "Descuento actualizado correctamente.";
                }
            }

            return RedirectToAction("Inicio");
        }

        private void ValidateDescuento(DescuentoViewModel model)
        {
            if (model == null)
                return;

            lock (SyncRoot)
            {
                var exists = Descuentos.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Descripcion, model.Descripcion, StringComparison.OrdinalIgnoreCase));

                if (exists)
                    ModelState.AddModelError("Descripcion", "Ya existe un descuento con esta descripcion.");
            }

            if (model.Tipo == 1)
            {
                if (model.Cantidad1 <= 0)
                    ModelState.AddModelError("Cantidad1", "Ingrese al menos una cantidad para el precio por volumen.");

                if (model.Precio1 <= 0)
                    ModelState.AddModelError("Precio1", "Ingrese el precio principal del descuento.");
            }
            else if (model.Tipo == 2)
            {
                if (model.Cantidad1 <= 0 || model.Cantidad2 <= 0)
                    ModelState.AddModelError("Cantidad2", "Ingrese la cantidad a comprar y la cantidad con precio especial.");

                if (model.Precio2 <= 0)
                    ModelState.AddModelError("Precio2", "Ingrese el precio especial.");
            }
            else if (model.Tipo == 3)
            {
                if (model.Cantidad1 <= 0)
                    ModelState.AddModelError("Cantidad1", "Ingrese al menos una cantidad para el porcentaje por volumen.");

                if (model.Porcentaje1 <= 0 || model.Porcentaje1 > 100)
                    ModelState.AddModelError("Porcentaje1", "Ingrese un porcentaje entre 0 y 100.");
            }
            else if (model.Tipo == 4)
            {
                if (model.Cantidad1 <= 0 || model.Cantidad2 <= 0)
                    ModelState.AddModelError("Cantidad2", "Ingrese la cantidad a comprar y la cantidad con descuento.");

                if (model.Porcentaje2 <= 0 || model.Porcentaje2 > 100)
                    ModelState.AddModelError("Porcentaje2", "Ingrese un porcentaje entre 0 y 100.");
            }
        }

        private static void Normalize(DescuentoViewModel model)
        {
            if (model == null)
                return;

            model.Descripcion = (model.Descripcion ?? "").Trim();
            model.Cantidad1 = Math.Max(0m, model.Cantidad1);
            model.Cantidad2 = Math.Max(0m, model.Cantidad2);
            model.Cantidad3 = Math.Max(0m, model.Cantidad3);
            model.Cantidad4 = Math.Max(0m, model.Cantidad4);
            model.Precio1 = Math.Max(0m, model.Precio1);
            model.Precio2 = Math.Max(0m, model.Precio2);
            model.Precio3 = Math.Max(0m, model.Precio3);
            model.Precio4 = Math.Max(0m, model.Precio4);
            model.Porcentaje1 = ClampPercent(model.Porcentaje1);
            model.Porcentaje2 = ClampPercent(model.Porcentaje2);
            model.Porcentaje3 = ClampPercent(model.Porcentaje3);
            model.Porcentaje4 = ClampPercent(model.Porcentaje4);
        }

        private static decimal ClampPercent(decimal value)
        {
            if (value < 0m)
                return 0m;

            if (value > 100m)
                return 100m;

            return value;
        }

        private void PrepareCatalogs()
        {
            ViewBag.Tipos = new List<SelectListItem>
            {
                new SelectListItem { Text = "Precio por cantidad", Value = "1" },
                new SelectListItem { Text = "Compre X y lleve Y a precio", Value = "2" },
                new SelectListItem { Text = "Porcentaje por cantidad", Value = "3" },
                new SelectListItem { Text = "Compre X y lleve Y con porcentaje", Value = "4" }
            };
        }

        private DescuentoViewModel CreateNewDescuento()
        {
            return new DescuentoViewModel
            {
                Tipo = 1,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };
        }

        private static DescuentoViewModel CreateDescuento(int id, string descripcion, int tipo, bool descontarImpares, decimal cantidad1, decimal cantidad2, decimal cantidad3, decimal cantidad4, decimal precio1, decimal precio2, decimal precio3, decimal precio4, decimal porcentaje1, decimal porcentaje2, decimal porcentaje3, decimal porcentaje4, int tiendasAsociadas, int articulosAsociados)
        {
            return new DescuentoViewModel
            {
                ID = id,
                HQID = 0,
                Descripcion = descripcion,
                Tipo = tipo,
                DescontarImpares = descontarImpares,
                Cantidad1 = cantidad1,
                Cantidad2 = cantidad2,
                Cantidad3 = cantidad3,
                Cantidad4 = cantidad4,
                Precio1 = precio1,
                Precio2 = precio2,
                Precio3 = precio3,
                Precio4 = precio4,
                Porcentaje1 = porcentaje1,
                Porcentaje2 = porcentaje2,
                Porcentaje3 = porcentaje3,
                Porcentaje4 = porcentaje4,
                TiendasAsociadas = tiendasAsociadas,
                ArticulosAsociados = articulosAsociados,
                UsuarioCrea = "Soporte",
                FechaCrea = DateTime.Now.AddDays(-1)
            };
        }

        private static DescuentoViewModel Clone(DescuentoViewModel source)
        {
            if (source == null)
                return null;

            return new DescuentoViewModel
            {
                ID = source.ID,
                HQID = source.HQID,
                Descripcion = source.Descripcion,
                Tipo = source.Tipo,
                DescontarImpares = source.DescontarImpares,
                Cantidad1 = source.Cantidad1,
                Cantidad2 = source.Cantidad2,
                Cantidad3 = source.Cantidad3,
                Cantidad4 = source.Cantidad4,
                Precio1 = source.Precio1,
                Precio2 = source.Precio2,
                Precio3 = source.Precio3,
                Precio4 = source.Precio4,
                Porcentaje1 = source.Porcentaje1,
                Porcentaje2 = source.Porcentaje2,
                Porcentaje3 = source.Porcentaje3,
                Porcentaje4 = source.Porcentaje4,
                TiendasAsociadas = source.TiendasAsociadas,
                ArticulosAsociados = source.ArticulosAsociados,
                UsuarioCrea = source.UsuarioCrea,
                FechaCrea = source.FechaCrea,
                UsuarioModifica = source.UsuarioModifica,
                FechaModifica = source.FechaModifica
            };
        }

        private static void CopyValues(DescuentoViewModel source, DescuentoViewModel target)
        {
            target.HQID = source.HQID;
            target.Descripcion = source.Descripcion;
            target.Tipo = source.Tipo;
            target.DescontarImpares = source.DescontarImpares;
            target.Cantidad1 = source.Cantidad1;
            target.Cantidad2 = source.Cantidad2;
            target.Cantidad3 = source.Cantidad3;
            target.Cantidad4 = source.Cantidad4;
            target.Precio1 = source.Precio1;
            target.Precio2 = source.Precio2;
            target.Precio3 = source.Precio3;
            target.Precio4 = source.Precio4;
            target.Porcentaje1 = source.Porcentaje1;
            target.Porcentaje2 = source.Porcentaje2;
            target.Porcentaje3 = source.Porcentaje3;
            target.Porcentaje4 = source.Porcentaje4;
            target.TiendasAsociadas = source.TiendasAsociadas;
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
