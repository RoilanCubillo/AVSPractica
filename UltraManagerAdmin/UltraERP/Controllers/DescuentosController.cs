using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using UltraERP.BusinessEntities;
using UltraERP.BusinessLogic;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class DescuentosController : Controller
    {
        public ActionResult Inicio()
        {
            try
            {
                List<DescuentoViewModel> model = new CT_QuantityDiscount()
                    .GetAll(0, 0)
                    .Select(MapDescuento)
                    .OrderBy(x => x.Descripcion)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["DescuentoError"] = "No se pudo cargar descuentos desde SQL: " + ex.Message;
                return View(Enumerable.Empty<DescuentoViewModel>());
            }
        }

        public ActionResult Registro(int? id)
        {
            DescuentoViewModel model = null;

            if (id.HasValue)
            {
                try
                {
                    EN_QuantityDiscount discount = GetDescuentoById(id.Value);
                    if (discount == null)
                        TempData["DescuentoError"] = "No se encontro el descuento en SQL.";
                    else
                        model = MapDescuento(discount);
                }
                catch (Exception ex)
                {
                    TempData["DescuentoError"] = "No se pudo leer el descuento desde SQL: " + ex.Message;
                }
            }

            PrepareCatalogs();
            return View(model ?? CreateNewDescuento());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(DescuentoViewModel model)
        {
            PrepareCatalogs();
            NormalizeNumericModelState(model);
            Normalize(model);
            ValidateDescuento(model);

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                Respuesta response = new CT_QuantityDiscount().Save(BuildQuantityDiscount(model));
                if (!response.Status)
                {
                    ModelState.AddModelError("", "SQL rechazo el guardado del descuento: " + response.Message);
                    return View(model);
                }

                TempData["DescuentoMessage"] = model.ID > 0 ? "Descuento actualizado correctamente en SQL." : "Descuento creado correctamente en SQL.";
                return RedirectToAction("Inicio");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar el descuento en SQL. " + ex.Message);
                return View(model);
            }
        }

        private void ValidateDescuento(DescuentoViewModel model)
        {
            if (model == null)
                return;

            try
            {
                List<DescuentoViewModel> descuentos = new CT_QuantityDiscount()
                    .GetAll(0, 0)
                    .Select(MapDescuento)
                    .ToList();

                bool exists = descuentos.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Descripcion, model.Descripcion, StringComparison.OrdinalIgnoreCase));

                if (exists)
                    ModelState.AddModelError("Descripcion", "Ya existe un descuento con esta descripcion.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo validar el descuento contra SQL. " + ex.Message);
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
            model.Precio1A = Math.Max(0m, model.Precio1A);
            model.Precio1B = Math.Max(0m, model.Precio1B);
            model.Precio1C = Math.Max(0m, model.Precio1C);
            model.Precio2 = Math.Max(0m, model.Precio2);
            model.Precio2A = Math.Max(0m, model.Precio2A);
            model.Precio2B = Math.Max(0m, model.Precio2B);
            model.Precio2C = Math.Max(0m, model.Precio2C);
            model.Precio3 = Math.Max(0m, model.Precio3);
            model.Precio3A = Math.Max(0m, model.Precio3A);
            model.Precio3B = Math.Max(0m, model.Precio3B);
            model.Precio3C = Math.Max(0m, model.Precio3C);
            model.Precio4 = Math.Max(0m, model.Precio4);
            model.Precio4A = Math.Max(0m, model.Precio4A);
            model.Precio4B = Math.Max(0m, model.Precio4B);
            model.Precio4C = Math.Max(0m, model.Precio4C);
            model.Porcentaje1 = ClampPercent(model.Porcentaje1);
            model.Porcentaje1A = ClampPercent(model.Porcentaje1A);
            model.Porcentaje1B = ClampPercent(model.Porcentaje1B);
            model.Porcentaje1C = ClampPercent(model.Porcentaje1C);
            model.Porcentaje2 = ClampPercent(model.Porcentaje2);
            model.Porcentaje2A = ClampPercent(model.Porcentaje2A);
            model.Porcentaje2B = ClampPercent(model.Porcentaje2B);
            model.Porcentaje2C = ClampPercent(model.Porcentaje2C);
            model.Porcentaje3 = ClampPercent(model.Porcentaje3);
            model.Porcentaje3A = ClampPercent(model.Porcentaje3A);
            model.Porcentaje3B = ClampPercent(model.Porcentaje3B);
            model.Porcentaje3C = ClampPercent(model.Porcentaje3C);
            model.Porcentaje4 = ClampPercent(model.Porcentaje4);
            model.Porcentaje4A = ClampPercent(model.Porcentaje4A);
            model.Porcentaje4B = ClampPercent(model.Porcentaje4B);
            model.Porcentaje4C = ClampPercent(model.Porcentaje4C);
        }

        private static decimal ClampPercent(decimal value)
        {
            if (value < 0m)
                return 0m;

            if (value > 100m)
                return 100m;

            return value;
        }

        private void NormalizeNumericModelState(DescuentoViewModel model)
        {
            if (model == null)
                return;

            SetPostedDecimal("Cantidad1", value => model.Cantidad1 = value);
            SetPostedDecimal("Cantidad2", value => model.Cantidad2 = value);
            SetPostedDecimal("Cantidad3", value => model.Cantidad3 = value);
            SetPostedDecimal("Cantidad4", value => model.Cantidad4 = value);

            SetPostedDecimal("Precio1", value => model.Precio1 = value);
            SetPostedDecimal("Precio1A", value => model.Precio1A = value);
            SetPostedDecimal("Precio1B", value => model.Precio1B = value);
            SetPostedDecimal("Precio1C", value => model.Precio1C = value);
            SetPostedDecimal("Precio2", value => model.Precio2 = value);
            SetPostedDecimal("Precio2A", value => model.Precio2A = value);
            SetPostedDecimal("Precio2B", value => model.Precio2B = value);
            SetPostedDecimal("Precio2C", value => model.Precio2C = value);
            SetPostedDecimal("Precio3", value => model.Precio3 = value);
            SetPostedDecimal("Precio3A", value => model.Precio3A = value);
            SetPostedDecimal("Precio3B", value => model.Precio3B = value);
            SetPostedDecimal("Precio3C", value => model.Precio3C = value);
            SetPostedDecimal("Precio4", value => model.Precio4 = value);
            SetPostedDecimal("Precio4A", value => model.Precio4A = value);
            SetPostedDecimal("Precio4B", value => model.Precio4B = value);
            SetPostedDecimal("Precio4C", value => model.Precio4C = value);

            SetPostedDecimal("Porcentaje1", value => model.Porcentaje1 = value);
            SetPostedDecimal("Porcentaje1A", value => model.Porcentaje1A = value);
            SetPostedDecimal("Porcentaje1B", value => model.Porcentaje1B = value);
            SetPostedDecimal("Porcentaje1C", value => model.Porcentaje1C = value);
            SetPostedDecimal("Porcentaje2", value => model.Porcentaje2 = value);
            SetPostedDecimal("Porcentaje2A", value => model.Porcentaje2A = value);
            SetPostedDecimal("Porcentaje2B", value => model.Porcentaje2B = value);
            SetPostedDecimal("Porcentaje2C", value => model.Porcentaje2C = value);
            SetPostedDecimal("Porcentaje3", value => model.Porcentaje3 = value);
            SetPostedDecimal("Porcentaje3A", value => model.Porcentaje3A = value);
            SetPostedDecimal("Porcentaje3B", value => model.Porcentaje3B = value);
            SetPostedDecimal("Porcentaje3C", value => model.Porcentaje3C = value);
            SetPostedDecimal("Porcentaje4", value => model.Porcentaje4 = value);
            SetPostedDecimal("Porcentaje4A", value => model.Porcentaje4A = value);
            SetPostedDecimal("Porcentaje4B", value => model.Porcentaje4B = value);
            SetPostedDecimal("Porcentaje4C", value => model.Porcentaje4C = value);
        }

        private void SetPostedDecimal(string key, Action<decimal> setValue)
        {
            string postedValue = Request.Form[key];
            decimal parsedValue;

            if (postedValue == null || !TryParseDecimal(postedValue, out parsedValue))
                return;

            setValue(parsedValue);
            ModelState.Remove(key);
        }

        private static bool TryParseDecimal(string value, out decimal result)
        {
            value = (value ?? "").Trim();
            if (String.IsNullOrWhiteSpace(value))
            {
                result = 0m;
                return true;
            }

            if (Decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out result))
                return true;

            if (Decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                return true;

            string normalized = value.Replace(" ", "");
            bool hasComma = normalized.IndexOf(",") >= 0;
            bool hasDot = normalized.IndexOf(".") >= 0;

            if (hasComma && hasDot && normalized.LastIndexOf(",") > normalized.LastIndexOf("."))
                normalized = normalized.Replace(".", "").Replace(",", ".");
            else if (hasComma && hasDot)
                normalized = normalized.Replace(",", "");
            else if (hasComma)
                normalized = normalized.Replace(",", ".");

            return Decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
        }

        private void PrepareCatalogs()
        {
            ViewBag.Tipos = new List<SelectListItem>
            {
                new SelectListItem { Text = "Mezcle y Combine: Precio Unitario", Value = "1" },
                new SelectListItem { Text = "Compre X y lleve Y por Z: Precio Unitario", Value = "2" },
                new SelectListItem { Text = "Mezcle y Combine: Porcentaje de Descuento", Value = "3" },
                new SelectListItem { Text = "Compre X y lleve Y por Z: Porcentaje de Descuento", Value = "4" }
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

        private EN_QuantityDiscount GetDescuentoById(int id)
        {
            return new CT_QuantityDiscount().Get(id);
        }

        private static DescuentoViewModel MapDescuento(EN_QuantityDiscount discount)
        {
            if (discount == null)
                return null;

            return new DescuentoViewModel
            {
                ID = discount.ID,
                HQID = discount.HQID,
                Descripcion = discount.Description,
                Tipo = discount.Type,
                DescontarImpares = discount.DiscountOddItems,
                Cantidad1 = Convert.ToDecimal(discount.Quantity1),
                Cantidad2 = Convert.ToDecimal(discount.Quantity2),
                Cantidad3 = Convert.ToDecimal(discount.Quantity3),
                Cantidad4 = Convert.ToDecimal(discount.Quantity4),
                Precio1 = discount.Price1,
                Precio1A = discount.Price1A,
                Precio1B = discount.Price1B,
                Precio1C = discount.Price1C,
                Precio2 = discount.Price2,
                Precio2A = discount.Price2A,
                Precio2B = discount.Price2B,
                Precio2C = discount.Price2C,
                Precio3 = discount.Price3,
                Precio3A = discount.Price3A,
                Precio3B = discount.Price3B,
                Precio3C = discount.Price3C,
                Precio4 = discount.Price4,
                Precio4A = discount.Price4A,
                Precio4B = discount.Price4B,
                Precio4C = discount.Price4C,
                Porcentaje1 = Convert.ToDecimal(discount.PercentOffPrice1),
                Porcentaje1A = Convert.ToDecimal(discount.PercentOffPrice1A),
                Porcentaje1B = Convert.ToDecimal(discount.PercentOffPrice1B),
                Porcentaje1C = Convert.ToDecimal(discount.PercentOffPrice1C),
                Porcentaje2 = Convert.ToDecimal(discount.PercentOffPrice2),
                Porcentaje2A = Convert.ToDecimal(discount.PercentOffPrice2A),
                Porcentaje2B = Convert.ToDecimal(discount.PercentOffPrice2B),
                Porcentaje2C = Convert.ToDecimal(discount.PercentOffPrice2C),
                Porcentaje3 = Convert.ToDecimal(discount.PercentOffPrice3),
                Porcentaje3A = Convert.ToDecimal(discount.PercentOffPrice3A),
                Porcentaje3B = Convert.ToDecimal(discount.PercentOffPrice3B),
                Porcentaje3C = Convert.ToDecimal(discount.PercentOffPrice3C),
                Porcentaje4 = Convert.ToDecimal(discount.PercentOffPrice4),
                Porcentaje4A = Convert.ToDecimal(discount.PercentOffPrice4A),
                Porcentaje4B = Convert.ToDecimal(discount.PercentOffPrice4B),
                Porcentaje4C = Convert.ToDecimal(discount.PercentOffPrice4C),
                TiendasAsociadas = 0,
                ArticulosAsociados = 0
            };
        }

        private static EN_QuantityDiscount BuildQuantityDiscount(DescuentoViewModel model)
        {
            return new EN_QuantityDiscount
            {
                ID = model.ID,
                HQID = model.HQID,
                Description = model.Descripcion,
                Type = model.Tipo,
                DiscountOddItems = model.DescontarImpares,
                Quantity1 = Convert.ToSingle(model.Cantidad1),
                Quantity2 = Convert.ToSingle(model.Cantidad2),
                Quantity3 = Convert.ToSingle(model.Cantidad3),
                Quantity4 = Convert.ToSingle(model.Cantidad4),
                Price1 = model.Precio1,
                Price1A = model.Precio1A,
                Price1B = model.Precio1B,
                Price1C = model.Precio1C,
                Price2 = model.Precio2,
                Price2A = model.Precio2A,
                Price2B = model.Precio2B,
                Price2C = model.Precio2C,
                Price3 = model.Precio3,
                Price3A = model.Precio3A,
                Price3B = model.Precio3B,
                Price3C = model.Precio3C,
                Price4 = model.Precio4,
                Price4A = model.Precio4A,
                Price4B = model.Precio4B,
                Price4C = model.Precio4C,
                PercentOffPrice1 = Convert.ToSingle(model.Porcentaje1),
                PercentOffPrice1A = Convert.ToSingle(model.Porcentaje1A),
                PercentOffPrice1B = Convert.ToSingle(model.Porcentaje1B),
                PercentOffPrice1C = Convert.ToSingle(model.Porcentaje1C),
                PercentOffPrice2 = Convert.ToSingle(model.Porcentaje2),
                PercentOffPrice2A = Convert.ToSingle(model.Porcentaje2A),
                PercentOffPrice2B = Convert.ToSingle(model.Porcentaje2B),
                PercentOffPrice2C = Convert.ToSingle(model.Porcentaje2C),
                PercentOffPrice3 = Convert.ToSingle(model.Porcentaje3),
                PercentOffPrice3A = Convert.ToSingle(model.Porcentaje3A),
                PercentOffPrice3B = Convert.ToSingle(model.Porcentaje3B),
                PercentOffPrice3C = Convert.ToSingle(model.Porcentaje3C),
                PercentOffPrice4 = Convert.ToSingle(model.Porcentaje4),
                PercentOffPrice4A = Convert.ToSingle(model.Porcentaje4A),
                PercentOffPrice4B = Convert.ToSingle(model.Porcentaje4B),
                PercentOffPrice4C = Convert.ToSingle(model.Porcentaje4C)
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
