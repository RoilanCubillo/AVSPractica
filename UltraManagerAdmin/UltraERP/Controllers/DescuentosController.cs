using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
                return View(Enumerable.Empty<DescuentoViewModel>());
            }
            catch (Exception ex)
            {
                TempData["DescuentoError"] = "No se pudo cargar descuentos desde SQL: " + ex.Message;
                return View(Enumerable.Empty<DescuentoViewModel>());
            }
        }

        [HttpGet]
        public JsonResult Buscar(int page = 1, int pageSize = 10, string search = "", string tipo = "Todos")
        {
            try
            {
                page = Math.Max(page, 1);
                pageSize = NormalizePageSize(pageSize);

                int total;
                List<DescuentoViewModel> rows = SearchDescuentos(page, pageSize, search, tipo, out total);
                int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));

                return Json(new JsonResponse("", "", new
                {
                    Rows = rows.Select(ToGridSummary).ToList(),
                    Total = total,
                    TotalPages = totalPages,
                    Page = Math.Min(page, totalPages),
                    PageSize = pageSize,
                    SummaryTypes = rows.Select(x => x.Tipo).Distinct().Count(),
                    SummaryOdd = rows.Count(x => x.DescontarImpares)
                }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse(ex.Message, "No se pudieron cargar descuentos desde SQL.", null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Detalle(int id)
        {
            try
            {
                EN_QuantityDiscount discount = GetDescuentoById(id);
                if (discount == null)
                    return Json(new JsonResponse("Descuento no encontrado.", "No se pudo cargar el descuento.", null, false), JsonRequestBehavior.AllowGet);

                return Json(new JsonResponse("", "", MapDescuento(discount), true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse(ex.Message, "No se pudo cargar el descuento.", null, false), JsonRequestBehavior.AllowGet);
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
                List<DescuentoViewModel> descuentos = (new CT_QuantityDiscount()
                    .GetAll(0, 0) ?? new List<EN_QuantityDiscount>())
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

        private List<DescuentoViewModel> SearchDescuentos(int page, int pageSize, string search, string tipo, out int total)
        {
            try
            {
                return SearchDescuentosFromSql(page, pageSize, search, tipo, out total);
            }
            catch
            {
                return SearchDescuentosFromBusinessLogic(page, pageSize, search, tipo, out total);
            }
        }

        private List<DescuentoViewModel> SearchDescuentosFromSql(int page, int pageSize, string search, string tipo, out int total)
        {
            var rows = new List<DescuentoViewModel>();
            total = 0;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];
            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("No se encontro la cadena de conexion MasterDB.");

            int typeValue = GetTipoValue(tipo);
            string cleanSearch = (search ?? "").Trim();
            int startRow = ((page - 1) * pageSize) + 1;
            int endRow = page * pageSize;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
WITH Filtered AS
(
    SELECT
        Q.[Description],
        Q.HQID,
        Q.ID,
        Q.DiscountOddItems,
        Q.Quantity1,
        Q.Price1,
        Q.Price1A,
        Q.Price1B,
        Q.Price1C,
        Q.Quantity2,
        Q.Price2,
        Q.Price2A,
        Q.Price2B,
        Q.Price2C,
        Q.Quantity3,
        Q.Price3,
        Q.Price3A,
        Q.Price3B,
        Q.Price3C,
        Q.Quantity4,
        Q.Price4,
        Q.Price4A,
        Q.Price4B,
        Q.Price4C,
        Q.[Type],
        Q.PercentOffPrice1,
        Q.PercentOffPrice1A,
        Q.PercentOffPrice1B,
        Q.PercentOffPrice1C,
        Q.PercentOffPrice2,
        Q.PercentOffPrice2A,
        Q.PercentOffPrice2B,
        Q.PercentOffPrice2C,
        Q.PercentOffPrice3,
        Q.PercentOffPrice3A,
        Q.PercentOffPrice3B,
        Q.PercentOffPrice3C,
        Q.PercentOffPrice4,
        Q.PercentOffPrice4A,
        Q.PercentOffPrice4B,
        Q.PercentOffPrice4C,
        ROW_NUMBER() OVER (ORDER BY Q.[Description], Q.ID) AS RowNumber,
        COUNT(1) OVER () AS TotalRows
    FROM dbo.QuantityDiscount Q
    WHERE (@Tipo = 0 OR Q.[Type] = @Tipo)
      AND (@Search = '' OR Q.[Description] LIKE @SearchPrefix OR CONVERT(NVARCHAR(30), Q.ID) LIKE @SearchPrefix)
)
SELECT *
FROM Filtered
WHERE RowNumber BETWEEN @StartRow AND @EndRow
ORDER BY RowNumber;";

                command.Parameters.AddWithValue("@Tipo", typeValue);
                command.Parameters.AddWithValue("@Search", cleanSearch);
                command.Parameters.AddWithValue("@SearchPrefix", cleanSearch + "%");
                command.Parameters.AddWithValue("@StartRow", startRow);
                command.Parameters.AddWithValue("@EndRow", endRow);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (total == 0 && !reader.IsDBNull(reader.GetOrdinal("TotalRows")))
                            total = reader.GetInt32(reader.GetOrdinal("TotalRows"));

                        rows.Add(MapDescuento(reader));
                    }
                }
            }

            return rows;
        }

        private List<DescuentoViewModel> SearchDescuentosFromBusinessLogic(int page, int pageSize, string search, string tipo, out int total)
        {
            int typeValue = GetTipoValue(tipo);
            string cleanSearch = (search ?? "").Trim();

            IEnumerable<DescuentoViewModel> query = (new CT_QuantityDiscount().GetAll(0, 0) ?? new List<EN_QuantityDiscount>())
                .Select(MapDescuento)
                .Where(x => x != null);

            if (typeValue > 0)
                query = query.Where(x => x.Tipo == typeValue);

            if (!String.IsNullOrWhiteSpace(cleanSearch))
            {
                query = query.Where(x =>
                    StartsWith(x.CodigoTexto, cleanSearch) ||
                    StartsWith(x.Descripcion, cleanSearch) ||
                    StartsWith(x.TipoTexto, cleanSearch));
            }

            List<DescuentoViewModel> filtered = query
                .OrderBy(x => x.Descripcion)
                .ThenBy(x => x.ID)
                .ToList();

            total = filtered.Count;
            return filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        private static DescuentoViewModel MapDescuento(SqlDataReader reader)
        {
            return MapDescuento(new EN_QuantityDiscount
            {
                Description = GetString(reader, "Description"),
                HQID = GetInt(reader, "HQID"),
                ID = GetInt(reader, "ID"),
                DiscountOddItems = GetBool(reader, "DiscountOddItems"),
                Quantity1 = GetFloat(reader, "Quantity1"),
                Price1 = GetDecimal(reader, "Price1"),
                Price1A = GetDecimal(reader, "Price1A"),
                Price1B = GetDecimal(reader, "Price1B"),
                Price1C = GetDecimal(reader, "Price1C"),
                Quantity2 = GetFloat(reader, "Quantity2"),
                Price2 = GetDecimal(reader, "Price2"),
                Price2A = GetDecimal(reader, "Price2A"),
                Price2B = GetDecimal(reader, "Price2B"),
                Price2C = GetDecimal(reader, "Price2C"),
                Quantity3 = GetFloat(reader, "Quantity3"),
                Price3 = GetDecimal(reader, "Price3"),
                Price3A = GetDecimal(reader, "Price3A"),
                Price3B = GetDecimal(reader, "Price3B"),
                Price3C = GetDecimal(reader, "Price3C"),
                Quantity4 = GetFloat(reader, "Quantity4"),
                Price4 = GetDecimal(reader, "Price4"),
                Price4A = GetDecimal(reader, "Price4A"),
                Price4B = GetDecimal(reader, "Price4B"),
                Price4C = GetDecimal(reader, "Price4C"),
                Type = GetInt(reader, "Type"),
                PercentOffPrice1 = GetFloat(reader, "PercentOffPrice1"),
                PercentOffPrice1A = GetFloat(reader, "PercentOffPrice1A"),
                PercentOffPrice1B = GetFloat(reader, "PercentOffPrice1B"),
                PercentOffPrice1C = GetFloat(reader, "PercentOffPrice1C"),
                PercentOffPrice2 = GetFloat(reader, "PercentOffPrice2"),
                PercentOffPrice2A = GetFloat(reader, "PercentOffPrice2A"),
                PercentOffPrice2B = GetFloat(reader, "PercentOffPrice2B"),
                PercentOffPrice2C = GetFloat(reader, "PercentOffPrice2C"),
                PercentOffPrice3 = GetFloat(reader, "PercentOffPrice3"),
                PercentOffPrice3A = GetFloat(reader, "PercentOffPrice3A"),
                PercentOffPrice3B = GetFloat(reader, "PercentOffPrice3B"),
                PercentOffPrice3C = GetFloat(reader, "PercentOffPrice3C"),
                PercentOffPrice4 = GetFloat(reader, "PercentOffPrice4"),
                PercentOffPrice4A = GetFloat(reader, "PercentOffPrice4A"),
                PercentOffPrice4B = GetFloat(reader, "PercentOffPrice4B"),
                PercentOffPrice4C = GetFloat(reader, "PercentOffPrice4C")
            });
        }

        private static object ToGridSummary(DescuentoViewModel discount)
        {
            return new
            {
                discount.ID,
                discount.CodigoTexto,
                discount.Descripcion,
                discount.Tipo,
                discount.TipoTexto,
                discount.ResumenTexto,
                discount.DescontarImpares
            };
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

        private static int NormalizePageSize(int pageSize)
        {
            return new[] { 5, 10, 20, 50, 100 }.Contains(pageSize) ? pageSize : 10;
        }

        private static int GetTipoValue(string tipo)
        {
            if (String.IsNullOrWhiteSpace(tipo) || String.Equals(tipo, "Todos", StringComparison.OrdinalIgnoreCase))
                return 0;

            int numericType;
            if (Int32.TryParse(tipo, out numericType))
                return numericType >= 1 && numericType <= 4 ? numericType : 0;

            if (String.Equals(tipo, "Mezcle y Combine: Precio Unitario", StringComparison.OrdinalIgnoreCase))
                return 1;

            if (String.Equals(tipo, "Compre X y lleve Y por Z: Precio Unitario", StringComparison.OrdinalIgnoreCase))
                return 2;

            if (String.Equals(tipo, "Mezcle y Combine: Porcentaje de Descuento", StringComparison.OrdinalIgnoreCase))
                return 3;

            if (String.Equals(tipo, "Compre X y lleve Y por Z: Porcentaje de Descuento", StringComparison.OrdinalIgnoreCase))
                return 4;

            return 0;
        }

        private static bool StartsWith(string value, string filter)
        {
            return String.IsNullOrWhiteSpace(filter) ||
                   (value ?? "").StartsWith(filter, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetString(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? "" : Convert.ToString(reader.GetValue(ordinal));
        }

        private static int GetInt(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        private static bool GetBool(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return !reader.IsDBNull(ordinal) && Convert.ToBoolean(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        private static float GetFloat(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0F : Convert.ToSingle(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        private static decimal GetDecimal(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0m : Convert.ToDecimal(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        private string GetCurrentUser()
        {
            return User != null && User.Identity != null && !String.IsNullOrWhiteSpace(User.Identity.Name)
                ? User.Identity.Name
                : "Soporte";
        }
    }
}
