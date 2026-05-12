using System;
using System.ComponentModel.DataAnnotations;

namespace UltraERP.Models
{
    public class DepartamentoViewModel
    {
        public int ID { get; set; }
        public int HQID { get; set; }

        [Required(ErrorMessage = "La familia es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una familia.")]
        public int FamiliaID { get; set; }

        public string FamiliaCodigo { get; set; }
        public string FamiliaNombre { get; set; }

        [Required(ErrorMessage = "El codigo es obligatorio.")]
        [StringLength(17, ErrorMessage = "El codigo no puede superar 17 caracteres.")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(30, ErrorMessage = "El nombre no puede superar 30 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(180, ErrorMessage = "La descripcion no puede superar 180 caracteres.")]
        public string Descripcion { get; set; }

        public bool Activo { get; set; }
        public int CantidadCategorias { get; set; }
        public int CantidadArticulos { get; set; }
        public string UsuarioCrea { get; set; }
        public DateTime FechaCrea { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }

        public string EstadoTexto
        {
            get { return Activo ? "Activo" : "Inactivo"; }
        }

        public string FamiliaTexto
        {
            get
            {
                if (String.IsNullOrWhiteSpace(FamiliaCodigo) && String.IsNullOrWhiteSpace(FamiliaNombre))
                    return "-";

                return String.Format("{0} - {1}", FamiliaCodigo, FamiliaNombre).Trim(' ', '-');
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
