using System;
using System.ComponentModel.DataAnnotations;

namespace UltraERP.Models
{
    public class ArticuloViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El codigo es obligatorio.")]
        [StringLength(25, ErrorMessage = "El codigo no puede superar 25 caracteres.")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria.")]
        [StringLength(80, ErrorMessage = "La descripcion no puede superar 80 caracteres.")]
        public string Descripcion { get; set; }

        [StringLength(180, ErrorMessage = "La descripcion extendida no puede superar 180 caracteres.")]
        public string DescripcionExtendida { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        public string UnidadMedida { get; set; }

        [Required(ErrorMessage = "La familia es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una familia.")]
        public int FamiliaID { get; set; }

        [Required(ErrorMessage = "El departamento es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un departamento.")]
        public int DepartamentoID { get; set; }

        [Required(ErrorMessage = "La categoria es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoria.")]
        public int CategoriaID { get; set; }

        [Required(ErrorMessage = "La subcategoria es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una subcategoria.")]
        public int SubCategoriaID { get; set; }

        public string Familia { get; set; }
        public string Departamento { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public string Proveedor { get; set; }
        public string Bodega { get; set; }

        [Range(0, 999999999, ErrorMessage = "El costo debe ser mayor o igual a cero.")]
        public decimal Costo { get; set; }

        [Range(0.01, 999999999, ErrorMessage = "El precio de venta debe ser mayor a cero.")]
        public decimal PrecioVenta { get; set; }

        [Range(0, 100, ErrorMessage = "El impuesto debe estar entre 0 y 100.")]
        public decimal ImpuestoPorcentaje { get; set; }

        [Range(0, 999999999, ErrorMessage = "La existencia no puede ser negativa.")]
        public decimal Existencia { get; set; }

        [Range(0, 999999999, ErrorMessage = "La existencia minima no puede ser negativa.")]
        public decimal ExistenciaMinima { get; set; }

        [Range(0, 999999999, ErrorMessage = "La existencia maxima no puede ser negativa.")]
        public decimal ExistenciaMaxima { get; set; }

        public bool Activo { get; set; }
        public bool Inventariable { get; set; }
        public bool Exento { get; set; }

        public string UsuarioCrea { get; set; }
        public DateTime FechaCrea { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }

        public decimal MargenPorcentaje
        {
            get
            {
                if (PrecioVenta <= 0)
                    return 0;

                return Math.Round(((PrecioVenta - Costo) / PrecioVenta) * 100, 2);
            }
        }

        public string EstadoTexto
        {
            get { return Activo ? "Activo" : "Inactivo"; }
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
