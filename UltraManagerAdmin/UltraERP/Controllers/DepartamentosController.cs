using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class DepartamentosController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<FamiliaCatalogo> Familias = new List<FamiliaCatalogo>
        {
            new FamiliaCatalogo(1, "ABAR", "Abarrotes"),
            new FamiliaCatalogo(2, "MP", "Materia Prima"),
            new FamiliaCatalogo(3, "EMP", "Empaque"),
            new FamiliaCatalogo(4, "PROMO", "Promociones")
        };

        private static readonly List<DepartamentoViewModel> Departamentos = new List<DepartamentoViewModel>
        {
            CreateDepartamento(1, 1, "SECOS", "Productos secos", "Abarrotes secos y productos no refrigerados.", true, 6, 24),
            CreateDepartamento(2, 1, "BEB", "Bebidas", "Bebidas listas para venta y consumo.", true, 4, 18),
            CreateDepartamento(3, 2, "PROD", "Produccion", "Insumos base usados en produccion.", true, 5, 13),
            CreateDepartamento(4, 3, "CAJAS", "Cajas y bolsas", "Material de empaque por presentacion.", false, 2, 7)
        };

        public ActionResult Inicio()
        {
            List<DepartamentoViewModel> model;
            lock (SyncRoot)
            {
                model = Departamentos.Select(Clone).OrderBy(x => x.FamiliaNombre).ThenBy(x => x.Codigo).ToList();
            }

            return View(model);
        }

        public ActionResult Registro(int? id)
        {
            DepartamentoViewModel model = null;
            if (id.HasValue)
            {
                lock (SyncRoot)
                {
                    model = Departamentos.Where(x => x.ID == id.Value).Select(Clone).FirstOrDefault();
                }
            }

            PrepareCatalogs();
            return View(model ?? CreateNewDepartamento());
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

            lock (SyncRoot)
            {
                var existing = Departamentos.FirstOrDefault(x => x.ID == model.ID);
                if (existing == null)
                {
                    model.ID = Departamentos.Count == 0 ? 1 : Departamentos.Max(x => x.ID) + 1;
                    model.HQID = 0;
                    model.UsuarioCrea = GetCurrentUser();
                    model.FechaCrea = DateTime.Now;
                    Departamentos.Add(Clone(model));
                    TempData["DepartamentoMessage"] = "Departamento creado correctamente.";
                }
                else
                {
                    model.HQID = existing.HQID;
                    model.UsuarioCrea = existing.UsuarioCrea;
                    model.FechaCrea = existing.FechaCrea;
                    model.UsuarioModifica = GetCurrentUser();
                    model.FechaModifica = DateTime.Now;
                    CopyValues(model, existing);
                    TempData["DepartamentoMessage"] = "Departamento actualizado correctamente.";
                }
            }

            return RedirectToAction("Inicio");
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            lock (SyncRoot)
            {
                var departamento = Departamentos.FirstOrDefault(x => x.ID == id);
                if (departamento == null)
                    return Json(new JsonResponse("Departamento no encontrado.", "No se encontro el departamento.", null, false));

                departamento.Activo = !departamento.Activo;
                departamento.UsuarioModifica = GetCurrentUser();
                departamento.FechaModifica = DateTime.Now;

                return Json(new JsonResponse("", departamento.Activo ? "Departamento activado." : "Departamento inactivado.", Clone(departamento), true));
            }
        }

        private void ValidateDepartamento(DepartamentoViewModel model)
        {
            if (model == null)
                return;

            if (model.FamiliaID <= 0 || !Familias.Any(x => x.ID == model.FamiliaID))
                ModelState.AddModelError("FamiliaID", "Seleccione una familia valida.");

            lock (SyncRoot)
            {
                var codeExists = Departamentos.Any(x =>
                    x.ID != model.ID &&
                    x.FamiliaID == model.FamiliaID &&
                    String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));

                if (codeExists)
                    ModelState.AddModelError("Codigo", "Ya existe un departamento con este codigo en la familia seleccionada.");

                var nameExists = Departamentos.Any(x =>
                    x.ID != model.ID &&
                    x.FamiliaID == model.FamiliaID &&
                    String.Equals(x.Nombre, model.Nombre, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    ModelState.AddModelError("Nombre", "Ya existe un departamento con este nombre en la familia seleccionada.");
            }
        }

        private static void Normalize(DepartamentoViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.Nombre = (model.Nombre ?? "").Trim();
            model.Descripcion = (model.Descripcion ?? "").Trim();
        }

        private static void ApplyFamilia(DepartamentoViewModel model)
        {
            if (model == null)
                return;

            var familia = Familias.FirstOrDefault(x => x.ID == model.FamiliaID);
            if (familia == null)
            {
                model.FamiliaCodigo = "";
                model.FamiliaNombre = "";
                return;
            }

            model.FamiliaCodigo = familia.Codigo;
            model.FamiliaNombre = familia.Nombre;
        }

        private void PrepareCatalogs()
        {
            ViewBag.Familias = Familias
                .Select(x => new SelectListItem { Text = x.Codigo + " - " + x.Nombre, Value = x.ID.ToString() })
                .ToList();
        }

        private DepartamentoViewModel CreateNewDepartamento()
        {
            var model = new DepartamentoViewModel
            {
                Activo = true,
                FamiliaID = Familias.First().ID,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };

            ApplyFamilia(model);
            return model;
        }

        private static DepartamentoViewModel CreateDepartamento(int id, int familiaID, string codigo, string nombre, string descripcion, bool activo, int cantidadCategorias, int cantidadArticulos)
        {
            var model = new DepartamentoViewModel
            {
                ID = id,
                HQID = 0,
                FamiliaID = familiaID,
                Codigo = codigo,
                Nombre = nombre,
                Descripcion = descripcion,
                Activo = activo,
                CantidadCategorias = cantidadCategorias,
                CantidadArticulos = cantidadArticulos,
                UsuarioCrea = "Soporte",
                FechaCrea = DateTime.Now.AddDays(-3)
            };

            ApplyFamilia(model);
            return model;
        }

        private static DepartamentoViewModel Clone(DepartamentoViewModel source)
        {
            if (source == null)
                return null;

            return new DepartamentoViewModel
            {
                ID = source.ID,
                HQID = source.HQID,
                FamiliaID = source.FamiliaID,
                FamiliaCodigo = source.FamiliaCodigo,
                FamiliaNombre = source.FamiliaNombre,
                Codigo = source.Codigo,
                Nombre = source.Nombre,
                Descripcion = source.Descripcion,
                Activo = source.Activo,
                CantidadCategorias = source.CantidadCategorias,
                CantidadArticulos = source.CantidadArticulos,
                UsuarioCrea = source.UsuarioCrea,
                FechaCrea = source.FechaCrea,
                UsuarioModifica = source.UsuarioModifica,
                FechaModifica = source.FechaModifica
            };
        }

        private static void CopyValues(DepartamentoViewModel source, DepartamentoViewModel target)
        {
            target.HQID = source.HQID;
            target.FamiliaID = source.FamiliaID;
            target.FamiliaCodigo = source.FamiliaCodigo;
            target.FamiliaNombre = source.FamiliaNombre;
            target.Codigo = source.Codigo;
            target.Nombre = source.Nombre;
            target.Descripcion = source.Descripcion;
            target.Activo = source.Activo;
            target.CantidadCategorias = source.CantidadCategorias;
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

        private class FamiliaCatalogo
        {
            public FamiliaCatalogo(int id, string codigo, string nombre)
            {
                ID = id;
                Codigo = codigo;
                Nombre = nombre;
            }

            public int ID { get; private set; }
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
        }
    }
}
