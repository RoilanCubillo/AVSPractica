using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using UltraERP.BusinessEntities;
using UltraERP.BusinessLogic;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class CasasComercialesController : Controller
    {
        public ActionResult Inicio()
        {
            try
            {
                IDictionary<int, int> articulosAsociados = GetArticulosAsociadosPorCasa();
                List<CasaComercialViewModel> model = new CT_Purchaser()
                    .GetAll()
                    .Select(x => MapCasa(x, articulosAsociados))
                    .OrderBy(x => x.Nombre)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["CasaComercialError"] = "No se pudieron cargar las casas comerciales desde SQL: " + ex.Message;
                return View(Enumerable.Empty<CasaComercialViewModel>());
            }
        }

        public ActionResult Registro(int? id)
        {
            CasaComercialViewModel model = null;

            if (id.HasValue)
            {
                try
                {
                    EN_Purchaser purchaser = GetCasaById(id.Value);
                    if (purchaser == null)
                        TempData["CasaComercialError"] = "No se encontro la casa comercial en SQL.";
                    else
                        model = MapCasa(purchaser, GetArticulosAsociadosPorCasa());
                }
                catch (Exception ex)
                {
                    TempData["CasaComercialError"] = "No se pudo leer la casa comercial desde SQL: " + ex.Message;
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

            try
            {
                EN_Purchaser purchaser = new EN_Purchaser
                {
                    ID = model.ID,
                    Code = model.Codigo,
                    ExtCode = model.CodigoExtra ?? "",
                    Name = model.Nombre,
                    EmailAddress = model.CorreoElectronico,
                    Telephone = model.Telefono,
                    Inactive = model.Inactivo
                };

                Respuesta response = new CT_Purchaser().Save(purchaser);
                if (!response.Status)
                {
                    ModelState.AddModelError("", "SQL rechazo el guardado de la casa comercial: " + response.Message);
                    return View(model);
                }

                TempData["CasaComercialMessage"] = model.ID > 0
                    ? "Casa comercial actualizada correctamente en SQL."
                    : "Casa comercial creada correctamente en SQL.";

                return RedirectToAction("Inicio");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar la casa comercial en SQL. " + ex.Message);
                return View(model);
            }
        }

        private EN_Purchaser GetCasaById(int id)
        {
            return new CT_Purchaser()
                .GetAll()
                .FirstOrDefault(x => x.ID == id);
        }

        private void ValidateCasa(CasaComercialViewModel model)
        {
            if (model == null)
                return;

            try
            {
                List<EN_Purchaser> purchasers = new CT_Purchaser().GetAll();

                bool duplicatedCode = purchasers.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Code, model.Codigo, StringComparison.OrdinalIgnoreCase));

                if (duplicatedCode)
                    ModelState.AddModelError("Codigo", "Ya existe una casa comercial con este c\u00f3digo.");

                bool duplicatedName = purchasers.Any(x =>
                    x.ID != model.ID &&
                    String.Equals(x.Name, model.Nombre, StringComparison.OrdinalIgnoreCase));

                if (duplicatedName)
                    ModelState.AddModelError("Nombre", "Ya existe una casa comercial con este nombre.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo validar la casa comercial contra SQL. " + ex.Message);
            }
        }

        private IDictionary<int, int> GetArticulosAsociadosPorCasa()
        {
            var result = new Dictionary<int, int>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];

            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return result;

            try
            {
                using (var connection = new SqlConnection(settings.ConnectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
                        SELECT PurchaserID, COUNT(1) AS ItemCount
                        FROM dbo.ExtCentral_Item
                        WHERE PurchaserID IS NOT NULL AND PurchaserID > 0
                        GROUP BY PurchaserID";

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int purchaserID = reader.IsDBNull(reader.GetOrdinal("PurchaserID"))
                                ? 0
                                : Convert.ToInt32(reader["PurchaserID"]);

                            if (purchaserID <= 0)
                                continue;

                            result[purchaserID] = reader.IsDBNull(reader.GetOrdinal("ItemCount"))
                                ? 0
                                : Convert.ToInt32(reader["ItemCount"]);
                        }
                    }
                }
            }
            catch
            {
                return result;
            }

            return result;
        }

        private static CasaComercialViewModel MapCasa(EN_Purchaser purchaser, IDictionary<int, int> articulosAsociados)
        {
            if (purchaser == null)
                return null;

            DateTime fechaBase = purchaser.LastUpdated == DateTime.MinValue ? DateTime.Now : purchaser.LastUpdated;

            return new CasaComercialViewModel
            {
                ID = purchaser.ID,
                Codigo = purchaser.Code,
                CodigoExtra = purchaser.ExtCode,
                Nombre = purchaser.Name,
                CorreoElectronico = purchaser.EmailAddress,
                Telefono = purchaser.Telephone,
                Inactivo = purchaser.Inactive,
                ArticulosAsociados = articulosAsociados.ContainsKey(purchaser.ID) ? articulosAsociados[purchaser.ID] : 0,
                UsuarioCrea = "SQL",
                FechaCrea = fechaBase,
                UsuarioModifica = "SQL",
                FechaModifica = purchaser.LastUpdated == DateTime.MinValue ? (DateTime?)null : purchaser.LastUpdated
            };
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
                UsuarioCrea = GetCurrentUser(),
                FechaCrea = DateTime.Now
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
