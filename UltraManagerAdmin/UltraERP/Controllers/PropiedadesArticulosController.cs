using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class PropiedadesArticulosController : Controller
    {
        private static readonly object SyncRoot = new object();

        private static readonly List<PropiedadPersonalizadaViewModel> PropiedadesDisponibles = new List<PropiedadPersonalizadaViewModel>
        {
            CreatePropiedad(1, "CABYS", 0, ""),
            CreatePropiedad(2, "Registro sanitario", 0, ""),
            CreatePropiedad(3, "Fecha vencimiento registro", 1, ""),
            CreatePropiedad(4, "Peso neto", 2, ""),
            CreatePropiedad(5, "Requiere refrigeracion", 4, ""),
            CreatePropiedad(6, "Origen", 5, "Costa Rica,Importado"),
            CreatePropiedad(7, "Presentacion", 0, "")
        };

        private static readonly List<TiendaAplicacionViewModel> Tiendas = new List<TiendaAplicacionViewModel>
        {
            new TiendaAplicacionViewModel { ID = "1", Codigo = "SJ01", Nombre = "San Jose Centro" },
            new TiendaAplicacionViewModel { ID = "2", Codigo = "HD01", Nombre = "Heredia" },
            new TiendaAplicacionViewModel { ID = "3", Codigo = "CT01", Nombre = "Cartago" },
            new TiendaAplicacionViewModel { ID = "4", Codigo = "AL01", Nombre = "Alajuela" },
            new TiendaAplicacionViewModel { ID = "5", Codigo = "GTE01", Nombre = "Guanacaste" }
        };

        private static readonly List<ArticuloPropiedadViewModel> Articulos = new List<ArticuloPropiedadViewModel>
        {
            CreateArticulo(1, "ARR-TP-2K", "Arroz Tio Pelon 2 kg", "Arroz nacional grano entero para gondola y mayoreo.",
                new Dictionary<string, string>
                {
                    { "CABYS", "1006100000000" },
                    { "Registro sanitario", "A-CR-11245" },
                    { "Fecha vencimiento registro", "2027-08-31" },
                    { "Peso neto", "2" },
                    { "Requiere refrigeracion", "false" },
                    { "Origen", "Costa Rica" },
                    { "Presentacion", "Bolsa 2 kg" }
                }),
            CreateArticulo(2, "FRJ-DP-900", "Frijoles rojos Don Pedro 900 g", "Frijol rojo empacado para consumo masivo.",
                new Dictionary<string, string>
                {
                    { "CABYS", "0713330000000" },
                    { "Registro sanitario", "A-CR-88412" },
                    { "Fecha vencimiento registro", "2027-04-15" },
                    { "Peso neto", "0.9" },
                    { "Requiere refrigeracion", "false" },
                    { "Origen", "Costa Rica" },
                    { "Presentacion", "Bolsa 900 g" }
                }),
            CreateArticulo(3, "SAL-LIZ-700", "Salsa Lizano 700 ml", "Salsa de mesa tradicional para abarrotes.",
                new Dictionary<string, string>
                {
                    { "CABYS", "2103900000000" },
                    { "Registro sanitario", "A-CR-55102" },
                    { "Fecha vencimiento registro", "2028-02-20" },
                    { "Peso neto", "0.7" },
                    { "Requiere refrigeracion", "false" },
                    { "Origen", "Costa Rica" },
                    { "Presentacion", "Botella 700 ml" }
                }),
            CreateArticulo(4, "LEC-DP-1L", "Leche Dos Pinos 1 L", "Leche fluida refrigerada de alta rotacion.",
                new Dictionary<string, string>
                {
                    { "CABYS", "0401200000000" },
                    { "Registro sanitario", "A-CR-22401" },
                    { "Fecha vencimiento registro", "2026-11-30" },
                    { "Peso neto", "1" },
                    { "Requiere refrigeracion", "true" },
                    { "Origen", "Costa Rica" },
                    { "Presentacion", "Caja 1 L" }
                }),
            CreateArticulo(5, "CAF-1820-500", "Cafe 1820 molido 500 g", "Cafe molido tostado nacional.",
                new Dictionary<string, string>
                {
                    { "CABYS", "0901210000000" },
                    { "Registro sanitario", "A-CR-33075" },
                    { "Fecha vencimiento registro", "2027-12-10" },
                    { "Peso neto", "0.5" },
                    { "Requiere refrigeracion", "false" },
                    { "Origen", "Costa Rica" },
                    { "Presentacion", "Bolsa 500 g" }
                }),
            CreateArticulo(6, "DET-IRX-1K", "Detergente Irex 1 kg", "Detergente en polvo para cuidado del hogar.",
                new Dictionary<string, string>
                {
                    { "CABYS", "3402200000000" },
                    { "Registro sanitario", "" },
                    { "Fecha vencimiento registro", "" },
                    { "Peso neto", "1" },
                    { "Requiere refrigeracion", "false" },
                    { "Origen", "Costa Rica" },
                    { "Presentacion", "Bolsa 1 kg" }
                })
        };

        public ActionResult Inicio()
        {
            PropiedadesArticuloInicioViewModel model;
            lock (SyncRoot)
            {
                model = new PropiedadesArticuloInicioViewModel
                {
                    Articulos = Articulos.Select(CloneArticulo).OrderBy(x => x.Codigo).ToList(),
                    PropiedadesDisponibles = PropiedadesDisponibles.Select(ClonePropiedad).ToList(),
                    Tiendas = Tiendas.Select(CloneTienda).ToList()
                };
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Guardar(GuardarPropiedadesArticuloViewModel model)
        {
            if (model == null)
                return Json(new JsonResponse("Solicitud vacia.", "No se recibieron propiedades para guardar.", null, false));

            if (String.IsNullOrWhiteSpace(model.Tiendas))
                return Json(new JsonResponse("Sin tiendas.", "Seleccione al menos una tienda para aplicar los cambios.", null, false));

            lock (SyncRoot)
            {
                var articulo = Articulos.FirstOrDefault(x => x.ID == model.ItemID);
                if (articulo == null)
                    return Json(new JsonResponse("Articulo no encontrado.", "No se encontro el articulo seleccionado.", null, false));

                foreach (var property in model.Propiedades ?? new List<ArticuloPropiedadValorViewModel>())
                {
                    var catalogProperty = PropiedadesDisponibles.FirstOrDefault(x => String.Equals(x.Nombre, property.Nombre, StringComparison.OrdinalIgnoreCase));
                    if (catalogProperty == null)
                        continue;

                    var current = articulo.Propiedades.FirstOrDefault(x => String.Equals(x.Nombre, catalogProperty.Nombre, StringComparison.OrdinalIgnoreCase));
                    if (current == null)
                        continue;

                    var normalized = NormalizeValue(property.Valor, catalogProperty);
                    if (!String.IsNullOrEmpty(normalized.ErrorMessage))
                        return Json(new JsonResponse(normalized.InternalMessage, normalized.ErrorMessage, null, false));

                    current.Valor = normalized.Value;
                }

                articulo.UsuarioModifica = GetCurrentUser();
                articulo.FechaModifica = DateTime.Now;

                return Json(new JsonResponse("", "Propiedades actualizadas correctamente.", CloneArticulo(articulo), true));
            }
        }

        private static PropiedadPersonalizadaViewModel CreatePropiedad(int id, string nombre, int tipo, string listaValores)
        {
            return new PropiedadPersonalizadaViewModel
            {
                ID = id,
                Nombre = nombre,
                Tipo = tipo,
                ListaValores = listaValores,
                Inactivo = false
            };
        }

        private static ArticuloPropiedadViewModel CreateArticulo(int id, string codigo, string descripcion, string descripcionExtendida, IDictionary<string, string> values)
        {
            return new ArticuloPropiedadViewModel
            {
                ID = id,
                Codigo = codigo,
                Descripcion = descripcion,
                DescripcionExtendida = descripcionExtendida,
                UsuarioModifica = "Soporte",
                FechaModifica = DateTime.Now.AddDays(-1),
                Propiedades = PropiedadesDisponibles.Select(property => new ArticuloPropiedadValorViewModel
                {
                    ID = property.ID,
                    ItemID = id,
                    Nombre = property.Nombre,
                    Tipo = property.Tipo,
                    Inactivo = property.Inactivo,
                    ListaValores = property.ListaValores,
                    Valor = values.ContainsKey(property.Nombre) ? values[property.Nombre] : ""
                }).ToList()
            };
        }

        private static NormalizedPropertyValue NormalizeValue(string value, PropiedadPersonalizadaViewModel property)
        {
            var text = (value ?? "").Trim();
            if (String.IsNullOrWhiteSpace(text))
                return new NormalizedPropertyValue("");

            if (property.Tipo == 1)
            {
                DateTime date;
                if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date) &&
                    !DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                    return new NormalizedPropertyValue("Fecha invalida", "La propiedad " + property.Nombre + " debe tener una fecha valida.");

                return new NormalizedPropertyValue(date.ToString("yyyy-MM-dd"));
            }

            if (property.Tipo == 2 || property.Tipo == 3)
            {
                decimal number;
                if (!Decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out number) &&
                    !Decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                    return new NormalizedPropertyValue("Numero invalido", "La propiedad " + property.Nombre + " debe tener un numero valido.");

                if (number < 0)
                    return new NormalizedPropertyValue("Numero negativo", "La propiedad " + property.Nombre + " no puede ser negativa.");

                return new NormalizedPropertyValue(number.ToString("0.##", CultureInfo.InvariantCulture));
            }

            if (property.Tipo == 4)
                return new NormalizedPropertyValue(String.Equals(text, "true", StringComparison.OrdinalIgnoreCase) ? "true" : "false");

            if (property.Tipo == 5)
            {
                var options = String.IsNullOrWhiteSpace(property.ListaValores)
                    ? new List<string>()
                    : property.ListaValores.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                if (options.Count > 0 && !options.Any(x => String.Equals(x, text, StringComparison.OrdinalIgnoreCase)))
                    return new NormalizedPropertyValue("Opcion invalida", "Seleccione una opcion valida para " + property.Nombre + ".");

                return new NormalizedPropertyValue(options.FirstOrDefault(x => String.Equals(x, text, StringComparison.OrdinalIgnoreCase)) ?? text);
            }

            return new NormalizedPropertyValue(text);
        }

        private static ArticuloPropiedadViewModel CloneArticulo(ArticuloPropiedadViewModel source)
        {
            if (source == null)
                return null;

            return new ArticuloPropiedadViewModel
            {
                ID = source.ID,
                Codigo = source.Codigo,
                Descripcion = source.Descripcion,
                DescripcionExtendida = source.DescripcionExtendida,
                UsuarioModifica = source.UsuarioModifica,
                FechaModifica = source.FechaModifica,
                Propiedades = source.Propiedades.Select(CloneValor).ToList()
            };
        }

        private static ArticuloPropiedadValorViewModel CloneValor(ArticuloPropiedadValorViewModel source)
        {
            return new ArticuloPropiedadValorViewModel
            {
                ID = source.ID,
                ItemID = source.ItemID,
                Nombre = source.Nombre,
                Tipo = source.Tipo,
                Inactivo = source.Inactivo,
                ListaValores = source.ListaValores,
                Valor = source.Valor
            };
        }

        private static PropiedadPersonalizadaViewModel ClonePropiedad(PropiedadPersonalizadaViewModel source)
        {
            return new PropiedadPersonalizadaViewModel
            {
                ID = source.ID,
                Nombre = source.Nombre,
                Tipo = source.Tipo,
                Inactivo = source.Inactivo,
                ListaValores = source.ListaValores
            };
        }

        private static TiendaAplicacionViewModel CloneTienda(TiendaAplicacionViewModel source)
        {
            return new TiendaAplicacionViewModel
            {
                ID = source.ID,
                Codigo = source.Codigo,
                Nombre = source.Nombre
            };
        }

        private string GetCurrentUser()
        {
            return User != null && User.Identity != null && !String.IsNullOrWhiteSpace(User.Identity.Name)
                ? User.Identity.Name
                : "Soporte";
        }

        private class NormalizedPropertyValue
        {
            public NormalizedPropertyValue(string value)
            {
                Value = value;
            }

            public NormalizedPropertyValue(string internalMessage, string errorMessage)
            {
                InternalMessage = internalMessage;
                ErrorMessage = errorMessage;
            }

            public string Value { get; private set; }
            public string InternalMessage { get; private set; }
            public string ErrorMessage { get; private set; }
        }
    }
}
