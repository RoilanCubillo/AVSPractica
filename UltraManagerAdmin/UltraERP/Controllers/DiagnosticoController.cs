using System;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using UltraERP.Models;
using UltraERP.Services;

namespace UltraERP.Controllers
{
    [Authorize]
    public class DiagnosticoController : Controller
    {
        public ActionResult Inicio()
        {
            ViewBag.Title = "Diagnostico";
            ViewBag.ActiveMenu = "Diagnostico";

            var model = new UserDiagnosticsViewModel
            {
                Login = User != null && User.Identity != null ? User.Identity.Name : "",
                Nombre = ReadSessionString("USER_NAME"),
                Cuenta = ReadSessionString("USER_ACCOUNT"),
                Correo = ReadSessionString("USER_EMAIL"),
                UserID = ReadSessionInt("USER_ID"),
                AutoID = ReadSessionInt("USER_AUTOID"),
                EmpresaID = ReadSessionInt("USER_COMPANY_ID"),
                EmpresaCodigo = ReadSessionString("USER_COMPANY_CODE"),
                EmpresaNombre = ReadSessionString("USER_COMPANY_NAME"),
                Rol = UserPermissionService.GetCurrentRoleName(),
                RolOrigen = UserPermissionService.GetCurrentRoleSource(),
                RolesSeguridad = ReadSessionString("USER_SECURITY_ROLES"),
                EsAdministrador = UserPermissionService.IsAdmin(),
                EsSoporte = UserPermissionService.IsSupport()
            };

            FillDiagnosticRows(Session["USER_MODULES"], model.Modulos);
            FillDiagnosticRows(Session["USER_VIEWS"], model.Vistas);
            FillDiagnosticRows(Session["USER_DATAACCESS"], model.AccesosDatos);

            return View(model);
        }

        private static void FillDiagnosticRows(object source, System.Collections.Generic.IList<DiagnosticListItemViewModel> target)
        {
            IEnumerable rows = source as IEnumerable;
            if (rows == null)
                return;

            foreach (object row in rows)
            {
                if (row == null)
                    continue;

                target.Add(new DiagnosticListItemViewModel
                {
                    Codigo = FirstValue(row, "Code", "Codigo", "Name", "ID"),
                    Nombre = FirstValue(row, "Name", "Nombre", "Description", "Descripcion"),
                    Descripcion = FirstValue(row, "Description", "Descripcion", "Label", "Text"),
                    Extra = BuildExtra(row),
                    Activo = ReadBoolean(row, "Enable", "Enabled", "Active", "Activo")
                });
            }
        }

        private static string BuildExtra(object value)
        {
            string enableAll = FirstValue(value, "EnableAll");
            string dataIDs = FirstValue(value, "DataIDs", "DataIds", "DataID", "DataId");

            if (!String.IsNullOrWhiteSpace(enableAll) || !String.IsNullOrWhiteSpace(dataIDs))
                return "Todos: " + (String.IsNullOrWhiteSpace(enableAll) ? "-" : enableAll) + " | Datos: " + (String.IsNullOrWhiteSpace(dataIDs) ? "-" : dataIDs);

            string controller = FirstValue(value, "Controller", "ControllerName");
            string action = FirstValue(value, "Action", "ActionName");
            if (!String.IsNullOrWhiteSpace(controller) || !String.IsNullOrWhiteSpace(action))
                return "Ruta: " + controller + "/" + action;

            return "";
        }

        private static string FirstValue(object source, params string[] names)
        {
            foreach (string name in names)
            {
                string value = ReadProperty(source, name);
                if (!String.IsNullOrWhiteSpace(value))
                    return value;
            }

            return "";
        }

        private static bool ReadBoolean(object source, params string[] names)
        {
            foreach (string name in names)
            {
                string value = ReadProperty(source, name);
                bool parsed;
                if (Boolean.TryParse(value, out parsed))
                    return parsed;
            }

            return true;
        }

        private static string ReadProperty(object source, string name)
        {
            if (source == null || String.IsNullOrWhiteSpace(name))
                return "";

            PropertyInfo property = source.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (property == null)
                return "";

            object value = property.GetValue(source, null);
            return value == null ? "" : Convert.ToString(value);
        }

        private int ReadSessionInt(string key)
        {
            int value;
            return Int32.TryParse(ReadSessionString(key), out value) ? value : 0;
        }

        private string ReadSessionString(string key)
        {
            HttpSessionStateBase session = Session;
            if (session == null || session[key] == null)
                return "";

            return Convert.ToString(session[key]);
        }
    }
}
