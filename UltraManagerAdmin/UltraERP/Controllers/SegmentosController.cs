using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class SegmentosController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<SubCategoriaCatalogo> SubCategorias = new List<SubCategoriaCatalogo>
        {
            new SubCategoriaCatalogo(1, "GRANO", "Arroz grano entero", "ARROZ", "Arroces", "GRANOS", "Granos basicos", "ABAR", "Abarrotes ticos"),
            new SubCategoriaCatalogo(2, "PRECOC", "Arroz precocido", "ARROZ", "Arroces", "GRANOS", "Granos basicos", "ABAR", "Abarrotes ticos"),
            new SubCategoriaCatalogo(3, "ROJOS", "Frijoles rojos", "FRIJ", "Frijoles", "GRANOS", "Granos basicos", "ABAR", "Abarrotes ticos"),
            new SubCategoriaCatalogo(4, "MESA", "Salsas de mesa", "SALT", "Salsas ticas", "SALSAS", "Salsas y condimentos", "ABAR", "Abarrotes ticos"),
            new SubCategoriaCatalogo(5, "FLUIDA", "Leche fluida", "LECHE", "Leches y lacteos", "REFRI", "Refrigerados", "LACT", "Lacteos y frescos"),
            new SubCategoriaCatalogo(6, "MOLIDO", "Cafe molido", "CAFCR", "Cafe costarricense", "CAFE", "Cafe y bebidas", "BEB", "Bebidas y cafe"),
            new SubCategoriaCatalogo(7, "POLVO", "Detergente en polvo", "DETER", "Detergentes", "HOGAR", "Cuidado del hogar", "LIMP", "Limpieza y hogar")
        };

        private static readonly List<SegmentoViewModel> Segmentos = new List<SegmentoViewModel>
        {
            CreateSegmento(1, 1, "2KG", "Bolsa 2 kg", "Arroz grano entero en presentacion familiar.", 8),
            CreateSegmento(2, 1, "5KG", "Bolsa 5 kg", "Arroz grano entero para mayor consumo.", 4),
            CreateSegmento(3, 2, "RAPIDO", "Coccion rapida", "Arroz precocido de preparacion rapida.", 6),
            CreateSegmento(4, 3, "900G", "Bolsa 900 g", "Frijol rojo empacado para supermercado y pulperia.", 7),
            CreateSegmento(5, 4, "LIZANO", "Salsa tipo Lizano", "Salsas de mesa inspiradas en el gusto costarricense.", 9),
            CreateSegmento(6, 5, "ENTERA", "Leche entera", "Leche fluida entera de alta rotacion.", 11),
            CreateSegmento(7, 6, "TARRAZU", "Origen Tarrazu", "Cafe molido de zonas cafetaleras costarricenses.", 5),
            CreateSegmento(8, 7, "BIO", "Biodegradable", "Detergentes biodegradables de uso domestico.", 4)
        };

        public ActionResult Inicio()
        {
            List<SegmentoViewModel> model;
            lock (SyncRoot)
            {
                model = Segmentos.Select(Clone).OrderBy(x => x.SubCategoriaNombre).ThenBy(x => x.Codigo).ToList();
            }

            return View(model);
        }

        public ActionResult Registro(int? id)
        {
            SegmentoViewModel model = null;
            if (id.HasValue)
            {
                lock (SyncRoot)
                {
                    model = Segmentos.Where(x => x.ID == id.Value).Select(Clone).FirstOrDefault();
                }
            }

            PrepareCatalogs();
            return View(model ?? CreateNewSegmento());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(SegmentoViewModel model)
        {
            Normalize(model);
            ApplySubCategoria(model);
            ValidateSegmento(model);

            if (!ModelState.IsValid)
            {
                PrepareCatalogs();
                return View(model);
            }

            lock (SyncRoot)
            {
                var existing = Segmentos.FirstOrDefault(x => x.ID == model.ID);
                if (existing == null)
                {
                    model.ID = Segmentos.Count == 0 ? 1 : Segmentos.Max(x => x.ID) + 1;
                    model.UsuarioCrea = GetCurrentUser();
                    model.FechaCrea = DateTime.Now;
                    Segmentos.Add(Clone(model));
                    TempData["SegmentoMessage"] = "Segmento creado correctamente.";
                }
                else
                {
                    model.UsuarioCrea = existing.UsuarioCrea;
                    model.FechaCrea = existing.FechaCrea;
                    model.UsuarioModifica = GetCurrentUser();
                    model.FechaModifica = DateTime.Now;
                    CopyValues(model, existing);
                    TempData["SegmentoMessage"] = "Segmento actualizado correctamente.";
                }
            }

            return RedirectToAction("Inicio");
        }

        private void ValidateSegmento(SegmentoViewModel model)
        {
            if (model == null)
                return;

            if (model.SubCategoriaID <= 0 || !SubCategorias.Any(x => x.ID == model.SubCategoriaID))
                ModelState.AddModelError("SubCategoriaID", "Seleccione una subcategoria valida.");

            lock (SyncRoot)
            {
                var codeExists = Segmentos.Any(x =>
                    x.ID != model.ID &&
                    x.SubCategoriaID == model.SubCategoriaID &&
                    String.Equals(x.Codigo, model.Codigo, StringComparison.OrdinalIgnoreCase));

                if (codeExists)
                    ModelState.AddModelError("Codigo", "Ya existe un segmento con este codigo en la subcategoria seleccionada.");

                var nameExists = Segmentos.Any(x =>
                    x.ID != model.ID &&
                    x.SubCategoriaID == model.SubCategoriaID &&
                    String.Equals(x.Descripcion, model.Descripcion, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    ModelState.AddModelError("Descripcion", "Ya existe un segmento con esta descripcion en la subcategoria seleccionada.");
            }
        }

        private static void Normalize(SegmentoViewModel model)
        {
            if (model == null)
                return;

            model.Codigo = (model.Codigo ?? "").Trim().ToUpperInvariant();
            model.Descripcion = (model.Descripcion ?? "").Trim();
            model.Nota = (model.Nota ?? "").Trim();
        }

        private static void ApplySubCategoria(SegmentoViewModel model)
        {
            if (model == null)
                return;

            var subCategoria = SubCategorias.FirstOrDefault(x => x.ID == model.SubCategoriaID);
            if (subCategoria == null)
            {
                model.SubCategoriaCodigo = "";
                model.SubCategoriaNombre = "";
                model.CategoriaCodigo = "";
                model.CategoriaNombre = "";
                model.DepartamentoCodigo = "";
                model.DepartamentoNombre = "";
                model.FamiliaCodigo = "";
                model.FamiliaNombre = "";
                return;
            }

            model.SubCategoriaCodigo = subCategoria.Codigo;
            model.SubCategoriaNombre = subCategoria.Nombre;
            model.CategoriaCodigo = subCategoria.CategoriaCodigo;
            model.CategoriaNombre = subCategoria.CategoriaNombre;
            model.DepartamentoCodigo = subCategoria.DepartamentoCodigo;
            model.DepartamentoNombre = subCategoria.DepartamentoNombre;
            model.FamiliaCodigo = subCategoria.FamiliaCodigo;
            model.FamiliaNombre = subCategoria.FamiliaNombre;
        }

        private void PrepareCatalogs()
        {
            ViewBag.SubCategorias = SubCategorias
                .Select(x => new SelectListItem { Text = x.FamiliaCodigo + " / " + x.DepartamentoCodigo + " / " + x.CategoriaCodigo + " / " + x.Codigo + " - " + x.Nombre, Value = x.ID.ToString() })
                .ToList();
        }

        private SegmentoViewModel CreateNewSegmento()
        {
            var model = new SegmentoViewModel
            {
                SubCategoriaID = SubCategorias.First().ID,
                FechaCrea = DateTime.Now,
                UsuarioCrea = GetCurrentUser()
            };

            ApplySubCategoria(model);
            return model;
        }

        private static SegmentoViewModel CreateSegmento(int id, int subCategoriaID, string codigo, string descripcion, string nota, int cantidadArticulos)
        {
            var model = new SegmentoViewModel
            {
                ID = id,
                SubCategoriaID = subCategoriaID,
                Codigo = codigo,
                Descripcion = descripcion,
                Nota = nota,
                CantidadArticulos = cantidadArticulos,
                UsuarioCrea = "Soporte",
                FechaCrea = DateTime.Now.AddDays(-1)
            };

            ApplySubCategoria(model);
            return model;
        }

        private static SegmentoViewModel Clone(SegmentoViewModel source)
        {
            if (source == null)
                return null;

            return new SegmentoViewModel
            {
                ID = source.ID,
                SubCategoriaID = source.SubCategoriaID,
                SubCategoriaCodigo = source.SubCategoriaCodigo,
                SubCategoriaNombre = source.SubCategoriaNombre,
                CategoriaCodigo = source.CategoriaCodigo,
                CategoriaNombre = source.CategoriaNombre,
                DepartamentoCodigo = source.DepartamentoCodigo,
                DepartamentoNombre = source.DepartamentoNombre,
                FamiliaCodigo = source.FamiliaCodigo,
                FamiliaNombre = source.FamiliaNombre,
                Codigo = source.Codigo,
                Descripcion = source.Descripcion,
                Nota = source.Nota,
                CantidadArticulos = source.CantidadArticulos,
                UsuarioCrea = source.UsuarioCrea,
                FechaCrea = source.FechaCrea,
                UsuarioModifica = source.UsuarioModifica,
                FechaModifica = source.FechaModifica
            };
        }

        private static void CopyValues(SegmentoViewModel source, SegmentoViewModel target)
        {
            target.SubCategoriaID = source.SubCategoriaID;
            target.SubCategoriaCodigo = source.SubCategoriaCodigo;
            target.SubCategoriaNombre = source.SubCategoriaNombre;
            target.CategoriaCodigo = source.CategoriaCodigo;
            target.CategoriaNombre = source.CategoriaNombre;
            target.DepartamentoCodigo = source.DepartamentoCodigo;
            target.DepartamentoNombre = source.DepartamentoNombre;
            target.FamiliaCodigo = source.FamiliaCodigo;
            target.FamiliaNombre = source.FamiliaNombre;
            target.Codigo = source.Codigo;
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

        private class SubCategoriaCatalogo
        {
            public SubCategoriaCatalogo(int id, string codigo, string nombre, string categoriaCodigo, string categoriaNombre, string departamentoCodigo, string departamentoNombre, string familiaCodigo, string familiaNombre)
            {
                ID = id;
                Codigo = codigo;
                Nombre = nombre;
                CategoriaCodigo = categoriaCodigo;
                CategoriaNombre = categoriaNombre;
                DepartamentoCodigo = departamentoCodigo;
                DepartamentoNombre = departamentoNombre;
                FamiliaCodigo = familiaCodigo;
                FamiliaNombre = familiaNombre;
            }

            public int ID { get; private set; }
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public string CategoriaCodigo { get; private set; }
            public string CategoriaNombre { get; private set; }
            public string DepartamentoCodigo { get; private set; }
            public string DepartamentoNombre { get; private set; }
            public string FamiliaCodigo { get; private set; }
            public string FamiliaNombre { get; private set; }
        }
    }
}
