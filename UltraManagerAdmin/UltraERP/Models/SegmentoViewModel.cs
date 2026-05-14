using System;
using System.ComponentModel.DataAnnotations;

namespace UltraERP.Models
{
    public class SegmentoViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "La subcategoria es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una subcategoria.")]
        public int SubCategoriaID { get; set; }

        public string SubCategoriaCodigo { get; set; }
        public string SubCategoriaNombre { get; set; }
        public string CategoriaCodigo { get; set; }
        public string CategoriaNombre { get; set; }
        public string DepartamentoCodigo { get; set; }
        public string DepartamentoNombre { get; set; }
        public string FamiliaCodigo { get; set; }
        public string FamiliaNombre { get; set; }

        [Required(ErrorMessage = "El codigo es obligatorio.")]
        [StringLength(17, ErrorMessage = "El codigo no puede superar 17 caracteres.")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria.")]
        [StringLength(80, ErrorMessage = "La descripcion no puede superar 80 caracteres.")]
        public string Descripcion { get; set; }

        [StringLength(180, ErrorMessage = "La nota no puede superar 180 caracteres.")]
        public string Nota { get; set; }

        public int CantidadArticulos { get; set; }
        public string UsuarioCrea { get; set; }
        public DateTime FechaCrea { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }

        public string SubCategoriaTexto
        {
            get
            {
                if (String.IsNullOrWhiteSpace(SubCategoriaCodigo) && String.IsNullOrWhiteSpace(SubCategoriaNombre))
                    return "-";

                return String.Format("{0} - {1}", SubCategoriaCodigo, SubCategoriaNombre).Trim(' ', '-');
            }
        }

        public string CategoriaTexto
        {
            get
            {
                if (String.IsNullOrWhiteSpace(CategoriaCodigo) && String.IsNullOrWhiteSpace(CategoriaNombre))
                    return "-";

                return String.Format("{0} - {1}", CategoriaCodigo, CategoriaNombre).Trim(' ', '-');
            }
        }

        public string DepartamentoTexto
        {
            get
            {
                if (String.IsNullOrWhiteSpace(DepartamentoCodigo) && String.IsNullOrWhiteSpace(DepartamentoNombre))
                    return "-";

                return String.Format("{0} - {1}", DepartamentoCodigo, DepartamentoNombre).Trim(' ', '-');
            }
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
