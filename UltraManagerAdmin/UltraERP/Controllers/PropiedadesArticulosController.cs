using Security.EntitiesAVS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using UltraERP.BusinessEntities;
using UltraERP.BusinessLogic;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class PropiedadesArticulosController : Controller
    {
        private const int DefaultResultCount = 500;

        public ActionResult Inicio()
        {
            try
            {
                IList<PropiedadPersonalizadaViewModel> propiedades = GetPropiedadesDisponibles();

                var model = new PropiedadesArticuloInicioViewModel
                {
                    Articulos = GetArticulosFromDatabase(propiedades, "", DefaultResultCount),
                    PropiedadesDisponibles = propiedades,
                    Tiendas = GetTiendasDisponibles()
                };

                ViewBag.PropiedadesArticulosDataSource = "SQL";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["PropiedadArticuloError"] = "No se pudieron cargar las propiedades de art\u00edculos desde SQL: " + ex.Message;
                ViewBag.PropiedadesArticulosDataSource = "SQL";

                return View(new PropiedadesArticuloInicioViewModel());
            }
        }

        [HttpPost]
        public JsonResult Guardar(GuardarPropiedadesArticuloViewModel model)
        {
            if (model == null)
                return Json(new JsonResponse("Solicitud vac\u00eda.", "No se recibieron propiedades para guardar.", null, false));

            if (model.ItemID <= 0)
                return Json(new JsonResponse("Art\u00edculo inv\u00e1lido.", "Seleccione un art\u00edculo v\u00e1lido.", null, false));

            if (String.IsNullOrWhiteSpace(model.Tiendas))
                return Json(new JsonResponse("Sin tiendas.", "Seleccione al menos una tienda para aplicar los cambios.", null, false));

            HydratePostedProperties(model);

            if (model.Propiedades == null || model.Propiedades.Count == 0)
                return Json(new JsonResponse("Sin propiedades.", "No se recibieron propiedades para actualizar.", null, false));

            try
            {
                IList<PropiedadPersonalizadaViewModel> propiedades = GetPropiedadesDisponibles();
                string propertiesAvailable = GetPropertiesAvailable();
                string xml = BuildPropertiesXml(model.Propiedades, propiedades, propertiesAvailable);

                Respuesta response = new CT_ItemProperties().Save_ItemExtProperty(
                    xml,
                    model.ItemID,
                    propertiesAvailable,
                    model.Tiendas,
                    GetCurrentUserID());

                if (!response.Status)
                    return Json(new JsonResponse(response.InternalMessage, response.Message, null, false));

                ArticuloPropiedadViewModel articulo = GetArticuloById(model.ItemID, propiedades);
                ApplyPostedValues(articulo, model.Propiedades);
                TempData["PropiedadArticuloMessage"] = "Propiedades actualizadas correctamente en SQL.";

                return Json(new JsonResponse(response.InternalMessage, "Propiedades actualizadas correctamente.", articulo, true));
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse(ex.Message, "No se pudieron guardar las propiedades en SQL.", null, false));
            }
        }

        private IList<PropiedadPersonalizadaViewModel> GetPropiedadesDisponibles()
        {
            return new CT_ItemProperties()
                .GetCustomProperty(GetPropertiesAvailable())
                .Where(x => x != null && !String.IsNullOrWhiteSpace(x.Name))
                .OrderBy(x => x.Name)
                .Select(MapPropiedad)
                .ToList();
        }

        private static void HydratePostedProperties(GuardarPropiedadesArticuloViewModel model)
        {
            if (model == null || String.IsNullOrWhiteSpace(model.PropiedadesJson))
                return;

            try
            {
                var properties = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ArticuloPropiedadValorViewModel>>(model.PropiedadesJson);
                if (properties != null && properties.Count > 0)
                    model.Propiedades = properties;
            }
            catch
            {
            }
        }

        private IList<ArticuloPropiedadViewModel> GetArticulosFromDatabase(IList<PropiedadPersonalizadaViewModel> propiedades, string searchValue, int take)
        {
            int recordLimit = take <= 0 ? 10000 : take;

            return new CT_ItemProperties()
                .GetAllItemsProperties(GetStoresAvailable(), GetPropertiesAvailable(), searchValue ?? "", 0, "asc", 0, recordLimit)
                .Select(x => MapArticulo(x, propiedades))
                .OrderBy(x => x.Codigo)
                .ToList();
        }

        private ArticuloPropiedadViewModel GetArticuloById(int itemID, IList<PropiedadPersonalizadaViewModel> propiedades)
        {
            EN_ItemProperty item = new CT_ItemProperties()
                .GetAllItemsProperties(GetStoresAvailable(), GetPropertiesAvailable(), "", 0, "asc", 0, 10000)
                .FirstOrDefault(x => x.ID == itemID);

            return item == null ? null : MapArticulo(item, propiedades);
        }

        private IList<TiendaAplicacionViewModel> GetTiendasDisponibles()
        {
            string storesAvailable = GetStoresAvailable();
            var stores = new CT_Store().GetAll("", 0, 0);
            if (stores == null || stores.Count == 0)
                stores = GetStoresFromTable();

            var filteredStores = stores;

            if (storesAvailable != "%")
            {
                var allowedIds = storesAvailable
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                filteredStores = stores.Where(x => allowedIds.Contains(x.IDS.ToString(CultureInfo.InvariantCulture))).ToList();
            }

            if (filteredStores.Count == 0)
                filteredStores = stores;

            return filteredStores
                .OrderBy(x => x.NameS)
                .Select(x => new TiendaAplicacionViewModel
                {
                    ID = x.IDS.ToString(CultureInfo.InvariantCulture),
                    Codigo = x.CodeS,
                    Nombre = x.NameS
                })
                .ToList();
        }

        private static List<EN_Store> GetStoresFromTable()
        {
            var stores = new List<EN_Store>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];

            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return stores;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT ID, StoreCode, Name
                    FROM dbo.Store
                    ORDER BY Name";

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stores.Add(new EN_Store
                        {
                            IDS = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                            CodeS = reader.IsDBNull(reader.GetOrdinal("StoreCode")) ? "" : Convert.ToString(reader["StoreCode"]),
                            NameS = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : Convert.ToString(reader["Name"])
                        });
                    }
                }
            }

            return stores;
        }

        private static PropiedadPersonalizadaViewModel MapPropiedad(EN_ItemCustomProperty source)
        {
            return new PropiedadPersonalizadaViewModel
            {
                ID = source.ID,
                Nombre = source.Name,
                Tipo = source.Type,
                Inactivo = source.Inactive,
                ListaValores = source.ListValue
            };
        }

        private static ArticuloPropiedadViewModel MapArticulo(EN_ItemProperty item, IList<PropiedadPersonalizadaViewModel> propiedades)
        {
            IDictionary<string, string> values = ParseProperties(item.Properties);

            return new ArticuloPropiedadViewModel
            {
                ID = item.ID,
                Codigo = item.ItemLookupCode,
                Descripcion = item.Description,
                DescripcionExtendida = item.ExtDescription,
                UsuarioModifica = "SQL",
                FechaModifica = null,
                Propiedades = propiedades.Select(property => new ArticuloPropiedadValorViewModel
                {
                    ID = property.ID,
                    ItemID = item.ID,
                    Nombre = property.Nombre,
                    Tipo = property.Tipo,
                    Inactivo = property.Inactivo,
                    ListaValores = property.ListaValores,
                    Valor = values.ContainsKey(property.Nombre) ? values[property.Nombre] : GetDefaultValue(property)
                }).ToList()
            };
        }

        private static string GetDefaultValue(PropiedadPersonalizadaViewModel property)
        {
            return property != null && property.Tipo == 4 ? "false" : "";
        }

        private static void ApplyPostedValues(ArticuloPropiedadViewModel articulo, IEnumerable<ArticuloPropiedadValorViewModel> postedProperties)
        {
            if (articulo == null || articulo.Propiedades == null || postedProperties == null)
                return;

            foreach (ArticuloPropiedadValorViewModel postedProperty in postedProperties)
            {
                ArticuloPropiedadValorViewModel current = articulo.Propiedades.FirstOrDefault(x =>
                    x.ID == postedProperty.ID ||
                    String.Equals(x.Nombre, postedProperty.Nombre, StringComparison.OrdinalIgnoreCase));

                if (current != null)
                    current.Valor = NormalizeValue(postedProperty.Valor, new PropiedadPersonalizadaViewModel
                    {
                        ID = current.ID,
                        Nombre = current.Nombre,
                        Tipo = current.Tipo,
                        Inactivo = current.Inactivo,
                        ListaValores = current.ListaValores
                    });
            }
        }

        private static IDictionary<string, string> ParseProperties(string properties)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (String.IsNullOrWhiteSpace(properties))
                return values;

            foreach (string part in properties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int separator = part.IndexOf(':');
                if (separator <= 0)
                    continue;

                string name = part.Substring(0, separator).Trim();
                string value = part.Substring(separator + 1).Trim();

                if (!values.ContainsKey(name))
                    values.Add(name, value);
            }

            return values;
        }

        private static string BuildPropertiesXml(IEnumerable<ArticuloPropiedadValorViewModel> postedProperties, IList<PropiedadPersonalizadaViewModel> catalog, string propertiesAvailable)
        {
            var allowedNames = (propertiesAvailable ?? "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            var xml = new System.Text.StringBuilder();

            foreach (ArticuloPropiedadValorViewModel property in postedProperties ?? new List<ArticuloPropiedadValorViewModel>())
            {
                PropiedadPersonalizadaViewModel catalogProperty = catalog.FirstOrDefault(x =>
                    x.ID == property.ID ||
                    String.Equals(x.Nombre, property.Nombre, StringComparison.OrdinalIgnoreCase));

                if (catalogProperty == null)
                    continue;

                if (propertiesAvailable != "%" && !allowedNames.Contains(catalogProperty.Nombre))
                    continue;

                string normalized = NormalizeValue(property.Valor, catalogProperty);

                xml.Append("<Property Name=\"")
                    .Append(EscapeXml(catalogProperty.Nombre))
                    .Append("\" Type=\"")
                    .Append(GetDataTypeProperty(catalogProperty.Tipo))
                    .Append("\">")
                    .Append(EscapeXml(normalized))
                    .Append("</Property>");
            }

            return xml.ToString();
        }

        private static string NormalizeValue(string value, PropiedadPersonalizadaViewModel property)
        {
            var text = (value ?? "").Trim();
            if (String.IsNullOrWhiteSpace(text))
                return "";

            if (property.Tipo == 1)
            {
                DateTime date;
                if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ||
                    DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                    return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                return text;
            }

            if (property.Tipo == 2 || property.Tipo == 3)
            {
                decimal number;
                if (Decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out number) ||
                    Decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                    return number.ToString("0.##", CultureInfo.InvariantCulture);

                return text;
            }

            if (property.Tipo == 4)
                return String.Equals(text, "true", StringComparison.OrdinalIgnoreCase) ? "true" : "false";

            if (property.Tipo == 5)
            {
                var options = String.IsNullOrWhiteSpace(property.ListaValores)
                    ? new List<string>()
                    : property.ListaValores.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                return options.FirstOrDefault(x => String.Equals(x, text, StringComparison.OrdinalIgnoreCase)) ?? text;
            }

            return text;
        }

        private static string GetDataTypeProperty(int type)
        {
            switch (type)
            {
                case 1: return "System.DateTime";
                case 2: return "System.Decimal";
                case 3: return "System.Decimal";
                case 4: return "System.Boolean";
                case 5: return "System.Collections.ArrayList";
                default: return "System.String";
            }
        }

        private static string EscapeXml(string value)
        {
            return SecurityElement.Escape(value ?? "") ?? "";
        }

        private string GetStoresAvailable()
        {
            return GetDataAccessAvailable(ConfigurationManager.AppSettings["DataStoreCode"] ?? "uerp-store");
        }

        private string GetPropertiesAvailable()
        {
            return GetDataAccessAvailable(ConfigurationManager.AppSettings["DataProperty"] ?? "uerp-item-property");
        }

        private string GetDataAccessAvailable(string dataCode)
        {
            EN_SC_DataAccess[] dataAccess = Session["USER_DATAACCESS"] as EN_SC_DataAccess[];

            if (dataAccess == null || dataAccess.Length == 0)
                return "%";

            EN_SC_DataAccess[] access = dataAccess.Where(x => x.Code == dataCode).ToArray();
            if (access.Length == 0 || access.Any(x => x.EnableAll))
                return "%";

            return String.Join(",", access.Where(x => !String.IsNullOrWhiteSpace(x.DataIDs)).Select(x => x.DataIDs));
        }

        private int GetCurrentUserID()
        {
            int userID;
            return Session["USER_AUTOID"] != null && Int32.TryParse(Convert.ToString(Session["USER_AUTOID"]), out userID)
                ? userID
                : 0;
        }
    }
}
