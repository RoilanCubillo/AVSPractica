using System;
using System.ComponentModel.DataAnnotations;

namespace UltraERP.Models
{
    public class DescuentoViewModel
    {
        public int ID { get; set; }
        public int HQID { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria.")]
        [StringLength(30, ErrorMessage = "La descripcion no puede superar 30 caracteres.")]
        public string Descripcion { get; set; }

        [Range(1, 4, ErrorMessage = "Seleccione un tipo de descuento.")]
        public int Tipo { get; set; }

        public bool DescontarImpares { get; set; }
        public decimal Cantidad1 { get; set; }
        public decimal Cantidad2 { get; set; }
        public decimal Cantidad3 { get; set; }
        public decimal Cantidad4 { get; set; }
        public decimal Precio1 { get; set; }
        public decimal Precio1A { get; set; }
        public decimal Precio1B { get; set; }
        public decimal Precio1C { get; set; }
        public decimal Precio2 { get; set; }
        public decimal Precio2A { get; set; }
        public decimal Precio2B { get; set; }
        public decimal Precio2C { get; set; }
        public decimal Precio3 { get; set; }
        public decimal Precio3A { get; set; }
        public decimal Precio3B { get; set; }
        public decimal Precio3C { get; set; }
        public decimal Precio4 { get; set; }
        public decimal Precio4A { get; set; }
        public decimal Precio4B { get; set; }
        public decimal Precio4C { get; set; }
        public decimal Porcentaje1 { get; set; }
        public decimal Porcentaje1A { get; set; }
        public decimal Porcentaje1B { get; set; }
        public decimal Porcentaje1C { get; set; }
        public decimal Porcentaje2 { get; set; }
        public decimal Porcentaje2A { get; set; }
        public decimal Porcentaje2B { get; set; }
        public decimal Porcentaje2C { get; set; }
        public decimal Porcentaje3 { get; set; }
        public decimal Porcentaje3A { get; set; }
        public decimal Porcentaje3B { get; set; }
        public decimal Porcentaje3C { get; set; }
        public decimal Porcentaje4 { get; set; }
        public decimal Porcentaje4A { get; set; }
        public decimal Porcentaje4B { get; set; }
        public decimal Porcentaje4C { get; set; }
        public int TiendasAsociadas { get; set; }
        public int ArticulosAsociados { get; set; }
        public string UsuarioCrea { get; set; }
        public DateTime FechaCrea { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }

        public string CodigoTexto
        {
            get { return ID > 0 ? ID.ToString() : "Automatico"; }
        }

        public string TipoTexto
        {
            get
            {
                switch (Tipo)
                {
                    case 1: return "Mezcle y Combine: Precio Unitario";
                    case 2: return "Compre X y lleve Y por Z: Precio Unitario";
                    case 3: return "Mezcle y Combine: Porcentaje de Descuento";
                    case 4: return "Compre X y lleve Y por Z: Porcentaje de Descuento";
                    default: return "-";
                }
            }
        }

        public string ResumenTexto
        {
            get
            {
                if (Tipo == 1)
                    return Cantidad1.ToString("0.##") + "+ desde " + Precio1.ToString("N2") + " / A " + Precio1A.ToString("N2") + " / B " + Precio1B.ToString("N2") + " / C " + Precio1C.ToString("N2");

                if (Tipo == 2)
                    return "Compre " + Cantidad1.ToString("0.##") + " y lleve " + Cantidad2.ToString("0.##") + " a " + Precio2.ToString("N2");

                if (Tipo == 3)
                    return Cantidad1.ToString("0.##") + "+ con " + Porcentaje1.ToString("0.##") + "% / A " + Porcentaje1A.ToString("0.##") + "% / B " + Porcentaje1B.ToString("0.##") + "% / C " + Porcentaje1C.ToString("0.##") + "%";

                if (Tipo == 4)
                    return "Compre " + Cantidad1.ToString("0.##") + " y lleve " + Cantidad2.ToString("0.##") + " con " + Porcentaje2.ToString("0.##") + "%";

                return "-";
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
