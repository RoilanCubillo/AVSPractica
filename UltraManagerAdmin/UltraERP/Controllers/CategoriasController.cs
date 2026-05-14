using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<DepartamentoCatalogo> Departamentos = new List<DepartamentoCatalogo>
        {
            new DepartamentoCatalogo(1, "GRANOS", "Granos basicos", "ABAR", "Abarrotes ticos"),
            new DepartamentoCatalogo(2, "SALSAS", "Salsas y condimentos", "ABAR", "Abarrotes ticos"),
            new DepartamentoCatalogo(3, "REFRI", "Refrigerados", "LACT", "Lacteos y frescos"),
            new DepartamentoCatalogo(4, "CAFE", "Cafe y bebidas", "BEB", "Bebidas y cafe"),
            new DepartamentoCatalogo(5, "HOGAR", "Cuidado del hogar", "LIMP", "Limpieza y hogar")
        };

        private static readonly List<CategoriaViewModel> Categorias = new List<CategoriaViewModel>
        {
            CreateCategoria(1, 1, "ARROZ", "Arroces", "Arroces pilados y precocidos comunes en Costa Rica.", true, 2, 18),
            CreateCategoria(2, 1, "FRIJ", "Frijoles", "Frijoles rojos y negros para consumo diario.", true, 2, 10),
            CreateCategoria(3, 2, "SALT", "Salsas ticas", "Salsas y condimentos usados en mesa y cocina nacional.", true, 2, 14),
            CreateCategoria(4, 3, "LECHE", "Leches y lacteos", "Leche fluida, yogurt y natilla refrigerada.", true, 2, 21),
            CreateCategoria(5, 4, "CAFCR", "Cafe costarricense", "Cafe molido y tostado de marcas presentes en el pais.", true, 2, 15),
            CreateCategoria(6, 5, "DETER", "Detergentes", "Detergentes y limpiadores para hogares costarricenses.", true, 2, 16)
        };

        public ActionResult Inicio()
        {
            List<CategoriaViewModel> model;
            lock (SyncRoot)
            {
                model = Categorias.Select(Clone).OrderBy(x => x.DepartamentoNombre).ThenBy(x => x.Codigo).ToList();
            }

            return View(model);
        }

        public ActionResult Registro(int? id)
        {
            CategoriaViewModel model = null;
            if (id.HasValue)
            {
                lock (SyncRoot)
                {
                    model = Categorias.Where(x => x.ID == id.Value).Select(Clone).FirstOrDefault();
                }
            }

            PrepareCatalogs();
            return View(model ?? CreateNewCategoria());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(CategoriaViewModel model)
        {
            Normalize(model);
            ApplyDepartamento(model);
            ValidateCategoria(model);

            if (!ModelState.IsValid)
            {
                PrepareCatalogs();
                return View(model);
            }

            lock (SyncRoot)
            {
                var existing = Categorias.FirstOrDefault(x => x.ID == model.ID);
                if (existing == null)
                {
                    model.ID = Categorias.Count == 0 ? 1 : Categorias.Max(x => x.ID) + 1;
                    model.HQID = 0;
                    model.UsuarioCrea = GetCurrentUser();
                    model.FechaCrea = DateTime.Now;
                    Categorias.Add(Clone(model));
                    TempData["CategoriaMessage"] = "Categoria creada correctamente.";
                }
                else
                {
                    model.HQID = existing.HQID;
                    model.UsuarioCrea = existing.UsuarioCrea;
                    model.FechaCrea = existing.FechaCrea;
                    model.UsuarioModifica = GetCurrentUser();
                    model.FechaModifica = DateTime.Now;
                    CopyValues(model, existing);
                    TempData["CategoriaMessage"] = "Categoria actualizada correctamente.";
                }
            }

            return RedirectToAction("Inicio");
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id)
        {
            lock (SyncRoot)
            {
                var categoria = Categorias.FirstOrDefault(x => x.ID == id);
                if (categoria == null)
                    return Json(new JsonResponse("Categoria no encontrada.", "No se encontro la categoria.", null, false));

                categoria.Activa = !categoria.Activa;
                categoria.UsuarioModifica = GetCurrentUser();
                categoria.FechaModifica = DateTime.Now;

                return Json(new JsonResponse("", categoria.Activa ? "Categoria activada." : "Categoria inactivada.", Clone(categoria), true));
            }
        }

        private void ValidateCategoria(CategoriaViewModel model)
        {
            if (model == null)
                return;

            if (model.DepartamentoID <= 0 || !Departamentos.Any(x => x.ID == model.DepartamentoID))
                ModelState.AddModelError("DepartamentoID", "Seleccione un departamento valido.");

            lock (SyncRoot)
            {
                var codeExists = Categorias.Any(x =>
                    x.ID != model.ID &&
                    x.DepartamentoID == model.DepartamentoID &&
                    String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));

                if (codeExists)
                    ModelState.AddModelError("Codigo", "Ya existe una categoria con este codigo en el departamento seleccionado.");

                var nameExists = Categorias.Any(x =>
                    x.ID != model.ID &&
                    x.DepartamentoID == model.DepartamentoID &&
                    String.Equals(x.Nombre, model.Nombre, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    ModelState.AddModelError("Nombre", "Ya existe una categoria con este nombre en el departamento seleccionado.");
            }
        }

        private static void Normalize(CategoriaViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.Nombre = (model.Nombre ?? "").Trim();
            model.Descripcion = (model.Descripcion ?? "").Trim();
        }

        private static void ApplyDepartamento(CategoriaViewModel model)
        {
            if (model == null)
                return;

            var departamento = Departamentos.FirstOrDefault(x => x.ID == model.DepartamentoID);
            if (departamento == null)
            {
                model.DepartamentoCodigo = "";
                model.DepartamentoNombre = "";
                model.FamiliaCodigo = "";
                model.FamiliaNombre = "";
                return;
            }

            model.DepartamentoCodigo = departamento.Codigo;
            model.DepartamentoNombre = departamento.Nombre;
            model.FamiliaCodigo = departamento.FamiliaCodigo;
            model.FamiliaNombre = departamento.FamiliaNombre;
        }

        private void PrepareCatalogs()
        {
            ViewBag.Departamentos = Departamentos
                .Select(x => new SelectListItem { Text = x.FamiliaCodigo + " / " + x.Codigo + " - " + x.Nombre, Value = x.ID.ToString() })
                .ToList();
        }

        private CategoriaViewModel CreateNewCategoria()
        {
            var model = new CategoriaViewModel
            {
                Activa = true,
                DepartamentoID = Departamentos.First().ID,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };

            ApplyDepartamento(model);
            return model;
        }

        private static CategoriaViewModel CreateCategoria(int id, int departamentoID, string codigo, string nombre, string descripcion, bool activa, int cantidadSubcategorias, int cantidadArticulos)
        {
            var model = new CategoriaViewModel
            {
                ID = id,
                HQID = 0,
                DepartamentoID = departamentoID,
                Codigo = codigo,
                Nombre = nombre,
                Descripcion = descripcion,
                Activa = activa,
                CantidadSubcategorias = cantidadSubcategorias,
                CantidadArticulos = cantidadArticulos,
                UsuarioCrea = "Soporte",
                FechaCrea = DateTime.Now.AddDays(-2)
            };

            ApplyDepartamento(model);
            return model;
        }

        private static CategoriaViewModel Clone(CategoriaViewModel source)
        {
            if (source == null)
                return null;

            return new CategoriaViewModel
            {
                ID = source.ID,
                HQID = source.HQID,
                DepartamentoID = source.DepartamentoID,
                DepartamentoCodigo = source.DepartamentoCodigo,
                DepartamentoNombre = source.DepartamentoNombre,
                FamiliaCodigo = source.FamiliaCodigo,
                FamiliaNombre = source.FamiliaNombre,
                Codigo = source.Codigo,
                Nombre = source.Nombre,
                Descripcion = source.Descripcion,
                Activa = source.Activa,
                CantidadSubcategorias = source.CantidadSubcategorias,
                CantidadArticulos = source.CantidadArticulos,
                UsuarioCrea = source.UsuarioCrea,
                FechaCrea = source.FechaCrea,
                UsuarioModifica = source.UsuarioModifica,
                FechaModifica = source.FechaModifica
            };
        }

        private static void CopyValues(CategoriaViewModel source, CategoriaViewModel target)
        {
            target.HQID = source.HQID;
            target.DepartamentoID = source.DepartamentoID;
            target.DepartamentoCodigo = source.DepartamentoCodigo;
            target.DepartamentoNombre = source.DepartamentoNombre;
            target.FamiliaCodigo = source.FamiliaCodigo;
            target.FamiliaNombre = source.FamiliaNombre;
            target.Codigo = source.Codigo;
            target.Nombre = source.Nombre;
            target.Descripcion = source.Descripcion;
            target.Activa = source.Activa;
            target.CantidadSubcategorias = source.CantidadSubcategorias;
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

        private class DepartamentoCatalogo
        {
            public DepartamentoCatalogo(int id, string codigo, string nombre, string familiaCodigo, string familiaNombre)
            {
                ID = id;
                Codigo = codigo;
                Nombre = nombre;
                FamiliaCodigo = familiaCodigo;
                FamiliaNombre = familiaNombre;
            }

            public int ID { get; private set; }
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public string FamiliaCodigo { get; private set; }
            public string FamiliaNombre { get; private set; }
        }
    }
}
