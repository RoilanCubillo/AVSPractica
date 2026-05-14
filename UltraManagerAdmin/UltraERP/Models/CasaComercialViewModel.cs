using System;
using System.ComponentModel.DataAnnotations;

namespace UltraERP.Models
{
    public class CasaComercialViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El codigo es obligatorio.")]
        [StringLength(20, ErrorMessage = "El codigo no puede superar 20 caracteres.")]
        public string Codigo { get; set; }

        [StringLength(20, ErrorMessage = "El codigo extra no puede superar 20 caracteres.")]
        public string CodigoExtra { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo electronico es obligatorio.")]
        [StringLength(225, ErrorMessage = "El correo electronico no puede superar 225 caracteres.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electronico valido.")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "El telefono es obligatorio.")]
        [StringLength(30, ErrorMessage = "El telefono no puede superar 30 caracteres.")]
        public string Telefono { get; set; }

        public bool Inactivo { get; set; }
        public int ArticulosAsociados { get; set; }
        public string UsuarioCrea { get; set; }
        public DateTime FechaCrea { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }

        public string EstadoTexto
        {
            get { return Inactivo ? "Inactivo" : "Activo"; }
        }

        public string FechaActualizacionTexto
        {
            get
            {
                var date = FechaModifica ?? FechaCrea;
                return date == DateTime.MinValue ? "-" : date.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string FechaCreaTexto
        {
            get { return FechaCrea == DateTime.MinValue ? "-" : FechaCrea.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string FechaModificaTexto
        {
            get { return FechaModifica.HasValue ? FechaModifica.Value.ToString("dd/MM/yyyy HH:mm") : "-"; }
        }
    }
}
