using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace UltraERP.Models
{
    public class PropiedadesArticuloInicioViewModel
    {
        public IList<ArticuloPropiedadViewModel> Articulos { get; set; }
        public IList<PropiedadPersonalizadaViewModel> PropiedadesDisponibles { get; set; }
        public IList<TiendaAplicacionViewModel> Tiendas { get; set; }

        public PropiedadesArticuloInicioViewModel()
        {
            Articulos = new List<ArticuloPropiedadViewModel>();
            PropiedadesDisponibles = new List<PropiedadPersonalizadaViewModel>();
            Tiendas = new List<TiendaAplicacionViewModel>();
        }
    }

    public class ArticuloPropiedadViewModel
    {
        public int ID { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionExtendida { get; set; }
        public IList<ArticuloPropiedadValorViewModel> Propiedades { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }

        public ArticuloPropiedadViewModel()
        {
            Propiedades = new List<ArticuloPropiedadValorViewModel>();
        }

        public int PropiedadesConfiguradas
        {
            get { return Propiedades == null ? 0 : Propiedades.Count(x => !String.IsNullOrWhiteSpace(x.Valor)); }
        }

        public int PropiedadesPendientes
        {
            get { return Propiedades == null ? 0 : Propiedades.Count(x => String.IsNullOrWhiteSpace(x.Valor)); }
        }

        public string PropiedadesTexto
        {
            get
            {
                if (Propiedades == null || Propiedades.Count == 0)
                    return "-";

                return String.Join(", ", Propiedades.Where(x => !String.IsNullOrWhiteSpace(x.Valor)).Select(x => x.Nombre + ": " + x.ValorTexto));
            }
        }

        public string FechaModificaTexto
        {
            get { return FechaModifica.HasValue ? FechaModifica.Value.ToString("dd/MM/yyyy HH:mm") : "-"; }
        }
    }

    public class ArticuloPropiedadValorViewModel
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string Nombre { get; set; }
        public int Tipo { get; set; }
        public bool Inactivo { get; set; }
        public string ListaValores { get; set; }
        public string Valor { get; set; }

        public string TipoTexto
        {
            get
            {
                switch (Tipo)
                {
                    case 1: return "Fecha";
                    case 2: return "Decimal";
                    case 3: return "Moneda";
                    case 4: return "Si/No";
                    case 5: return "Lista";
                    default: return "Texto";
                }
            }
        }

        public IList<string> Opciones
        {
            get
            {
                return String.IsNullOrWhiteSpace(ListaValores)
                    ? new List<string>()
                    : ListaValores.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            }
        }

        public string ValorTexto
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Valor))
                    return "-";

                if (Tipo == 4)
                    return String.Equals(Valor, "true", StringComparison.OrdinalIgnoreCase) ? "Si" : "No";

                decimal number;
                if ((Tipo == 2 || Tipo == 3) && Decimal.TryParse(Valor, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                    return number.ToString(Tipo == 3 ? "N2" : "0.##");

                return Valor;
            }
        }
    }

    public class PropiedadPersonalizadaViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El nombre de la propiedad es obligatorio.")]
        public string Nombre { get; set; }

        public int Tipo { get; set; }
        public bool Inactivo { get; set; }
        public string ListaValores { get; set; }
    }

    public class TiendaAplicacionViewModel
    {
        public string ID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }

        public string Texto
        {
            get { return Codigo + " - " + Nombre; }
        }
    }

    public class GuardarPropiedadesArticuloViewModel
    {
        public int ItemID { get; set; }
        public string Tiendas { get; set; }
        public IList<ArticuloPropiedadValorViewModel> Propiedades { get; set; }

        public GuardarPropiedadesArticuloViewModel()
        {
            Propiedades = new List<ArticuloPropiedadValorViewModel>();
        }
    }
}
