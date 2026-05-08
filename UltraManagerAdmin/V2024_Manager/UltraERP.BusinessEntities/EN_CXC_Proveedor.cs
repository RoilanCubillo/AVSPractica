using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_CXC_Proveedor
    {
        #region Fields
        private int iD;
        private int formaRebajoID;
        private int proveedorID;
        private int casaComercialID;
        private string numeroContrato;
        private DateTime vigenciaDesde;
        private DateTime vigenciaHasta;
        private string estado;
        #endregion

        #region Properties
        public int ID {
            get { return iD; }
            set { iD = value; } 
        }
        public int FormaRebajoID {
            get { return formaRebajoID; }
            set { formaRebajoID = value; } 
        }
        public int ProveedorID {
            get { return proveedorID; }
            set { proveedorID = value; } 
        }
        public int CasaComercialID {
            get { return casaComercialID; }
            set { casaComercialID = value; } 
        }
        public string NumeroContrato {
            get { return numeroContrato; }
            set { numeroContrato = value; } 
        }
        public DateTime VigenciaDesde {
            get { return vigenciaDesde; }
            set { vigenciaDesde = value; } 
        }
        public DateTime VigenciaHasta {
            get { return vigenciaHasta; }
            set { vigenciaHasta = value; } 
        }
        public string VigenciaDesdeAux
        {
            get { return vigenciaDesde != null ? vigenciaDesde.ToString("dd/MM/yyyy") : "01/01/1900"; }
        }
        public string VigenciaHastaAux
        {
            get { return vigenciaHasta != null ? vigenciaHasta.ToString("dd/MM/yyyy") : "01/01/1900"; }
        }
        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }
        #endregion
    }
}
