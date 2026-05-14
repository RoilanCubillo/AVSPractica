using System;
using System.ComponentModel.DataAnnotations;

namespace UltraERP.Models
{
    public class MarcaViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria.")]
        [StringLength(50, ErrorMessage = "La descripcion no puede superar 50 caracteres.")]
        public string Descripcion { get; set; }

        [StringLength(180, ErrorMessage = "La nota no puede superar 180 caracteres.")]
        public string Nota { get; set; }

        public int CantidadArticulos { get; set; }
        public string UsuarioCrea { get; set; }
        public DateTime FechaCrea { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }

        public string CodigoTexto
        {
            get { return ID > 0 ? ID.ToString() : "Automatico"; }
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
