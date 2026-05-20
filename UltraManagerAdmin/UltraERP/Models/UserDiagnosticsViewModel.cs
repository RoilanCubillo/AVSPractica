using System.Collections.Generic;

namespace UltraERP.Models
{
    public class UserDiagnosticsViewModel
    {
        public UserDiagnosticsViewModel()
        {
            Modulos = new List<DiagnosticListItemViewModel>();
            Vistas = new List<DiagnosticListItemViewModel>();
            AccesosDatos = new List<DiagnosticListItemViewModel>();
        }

        public string Login { get; set; }
        public string Nombre { get; set; }
        public string Cuenta { get; set; }
        public string Correo { get; set; }
        public int UserID { get; set; }
        public int AutoID { get; set; }
        public int EmpresaID { get; set; }
        public string EmpresaCodigo { get; set; }
        public string EmpresaNombre { get; set; }
        public string Rol { get; set; }
        public string RolOrigen { get; set; }
        public string RolesSeguridad { get; set; }
        public bool EsAdministrador { get; set; }
        public bool EsSoporte { get; set; }
        public IList<DiagnosticListItemViewModel> Modulos { get; set; }
        public IList<DiagnosticListItemViewModel> Vistas { get; set; }
        public IList<DiagnosticListItemViewModel> AccesosDatos { get; set; }
    }

    public class DiagnosticListItemViewModel
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Extra { get; set; }
        public bool Activo { get; set; }
    }

    public class DiagnosticTableViewModel
    {
        public DiagnosticTableViewModel(string title, IEnumerable<DiagnosticListItemViewModel> rows)
        {
            Title = title;
            Rows = rows == null ? new List<DiagnosticListItemViewModel>() : new List<DiagnosticListItemViewModel>(rows);
        }

        public string Title { get; private set; }
        public IList<DiagnosticListItemViewModel> Rows { get; private set; }
    }
}
