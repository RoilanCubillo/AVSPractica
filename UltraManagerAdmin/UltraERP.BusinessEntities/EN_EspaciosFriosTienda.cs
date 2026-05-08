using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_EspaciosFriosTienda
    {

        #region Atributos
        private int id;
        private int storeId;
        private int espacioId;
        private int numPuerta;
        private int numParrilla;
        private float porcentaje;
        private int supplierId;
        private string listaProductos;
        private int categoria;
        private DateTime fechaIni;
        private DateTime fechaFin;
        private int taxId;
        private decimal total;
        #endregion

        #region Contructor
        public EN_EspaciosFriosTienda() { }
        #endregion

        #region Propiedades
        /// <summary>
        /// Gets or sets the ID value.
        /// </summary>
        public virtual int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the tienda  value.
        /// </summary>
        public virtual int StoreId
        {
            get { return storeId; }
            set { storeId = value; }
        }
        /// <summary>
        /// Gets or sets the SupplierId value.
        /// </summary>
        public virtual int SupplierId
        {
            get { return supplierId; }
            set { supplierId = value; }
        }
        /// <summary>
        /// Gets or sets the SupplierId value.
        /// </summary>
        public virtual int EspacioId
        {
            get { return espacioId; }
            set { espacioId = value; }
        }
        /// <summary>
        /// Gets or sets the ListaProductos value.
        /// </summary>
        public virtual string ListaProductos
        {
            get { return listaProductos; }
            set { listaProductos = value; }
        }
        /// <summary>
        /// Gets or sets the Categoria value.
        /// </summary>
        public virtual int Categoria
        {
            get { return categoria; }
            set { categoria = value; }
        }
        /// <summary>
        /// Gets or sets the FechaInicio value.
        /// </summary>
        public virtual DateTime FechaInicio
        {
            get { return fechaIni; }
            set { fechaIni = value; }
        }
        /// <summary>
        /// Gets or sets the FechaFin value.
        /// </summary>
        public virtual DateTime FechaFin
        {
            get { return fechaFin; }
            set { fechaFin = value; }
        }

        public virtual string FechaInicioAux
        {
            get { return fechaIni != null ? fechaIni.ToString("dd/MM/yyyy") : "01/01/1900"; }

        }

        public virtual string FechaFinAux
        {
            get { return fechaFin != null ? fechaFin.ToString("dd/MM/yyyy") : "01/01/1900"; }

        }

        public virtual int NumPuerta
        {
            get { return numPuerta; }
            set { numPuerta = value; }
        }
        public virtual int NumParrilla
        {
            get { return numParrilla; }
            set { numParrilla = value; }
        }

        public virtual float Porcentaje
        {
            get { return porcentaje; }
            set { porcentaje = value; }
        }
        public virtual int TaxId
        {
            get { return taxId; }
            set { taxId = value; }
        }

        public virtual decimal Total
        {
            get { return total; }
            set { total = value; }
        }




        #endregion
    }
}
