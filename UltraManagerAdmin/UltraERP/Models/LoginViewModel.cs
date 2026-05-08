using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace UltraERP.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingrese el usuario.")]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Ingrese la contrasena.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contrasena")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Empresa")]
        public int? CompanyId { get; set; }

        public bool RequireCompanySelection { get; set; }

        public string LoginMessage { get; set; }

        public SelectListItem[] Companies { get; set; }
    }
}
