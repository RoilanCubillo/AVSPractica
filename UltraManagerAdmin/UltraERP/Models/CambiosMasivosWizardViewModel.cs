using System;
using System.Collections.Generic;
using System.Linq;

namespace UltraERP.Models
{
    public class CambiosMasivosWizardViewModel
    {
        public IList<CambioMasivoTaskViewModel> Tareas { get; set; }
        public IList<CambioMasivoGrupoTiendaViewModel> GruposTienda { get; set; }
        public IList<CambioMasivoTiendaViewModel> Tiendas { get; set; }
        public string FechaEfectivaSugerida { get; set; }

        public CambiosMasivosWizardViewModel()
        {
            Tareas = new List<CambioMasivoTaskViewModel>();
            GruposTienda = new List<CambioMasivoGrupoTiendaViewModel>();
            Tiendas = new List<CambioMasivoTiendaViewModel>();
        }
    }

    public class CambioMasivoTaskViewModel
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string Resumen { get; set; }
        public int EstiloHojaID { get; set; }
        public IList<string> ColumnasArchivo { get; set; }

        public CambioMasivoTaskViewModel()
        {
            ColumnasArchivo = new List<string>();
        }

        public string PlantillaEjemplo
        {
            get
            {
                return ColumnasArchivo == null || ColumnasArchivo.Count == 0
                    ? String.Empty
                    : String.Join(";", ColumnasArchivo);
            }
        }
    }

    public class CambioMasivoGrupoTiendaViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public int CantidadTiendas { get; set; }
    }

    public class CambioMasivoTiendaViewModel
    {
        public int ID { get; set; }
        public int GrupoID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Ciudad { get; set; }

        public string Texto
        {
            get { return Codigo + " - " + Nombre; }
        }
    }

    public class CambioMasivoAplicacionRequest
    {
        public string TaskCode { get; set; }
        public string Notes { get; set; }
        public string EffectiveDate { get; set; }
        public string FileName { get; set; }
        public string Separator { get; set; }
        public IList<int> StoreIDs { get; set; }
        public IList<CambioMasivoFilaRequest> Rows { get; set; }

        public CambioMasivoAplicacionRequest()
        {
            StoreIDs = new List<int>();
            Rows = new List<CambioMasivoFilaRequest>();
        }
    }

    public class CambioMasivoFilaRequest
    {
        public IList<string> Values { get; set; }

        public CambioMasivoFilaRequest()
        {
            Values = new List<string>();
        }

        public bool TieneContenido
        {
            get { return Values != null && Values.Any(x => !String.IsNullOrWhiteSpace(x)); }
        }
    }
}
