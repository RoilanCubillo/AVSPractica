using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_IslasSideKick
    {

        #region Atributos
        private int Id;
        private int IdStore;
        private int IdSupplier;
        private string tipoEspacio;
        private string numEspacio;
        private string ubicacion;
        private string dinamica;
        private string detalle;
        private DateTime fechaInicio;
        private DateTime fechaFin;
        private decimal monto;
        private int IdTax;
        private decimal total;
        #endregion

        #region Constructor
        public EN_IslasSideKick() { }
        #endregion

        #region Propiedades
        /// <summary>
		/// Gets or sets the ID value.
		/// </summary>
        public virtual int ID
        {
            get { return Id; }
            set { Id = value; }
        }

        /// <summary>
        /// Gets or sets the IDStore value.
        /// </summary>
        public virtual int IDStore
        {
            get { return IdStore; }
            set { IdStore = value; }
        }
        /// <summary>
        /// Gets or sets the IDSupplier value.
        /// </summary>
        public virtual int IDSupplier
        {
            get { return IdSupplier; }
            set { IdSupplier = value; }
        }

        /// <summary>
        /// Gets or sets the TipoEspacio value.
        /// </summary>
        public virtual string TipoEspacio
        {
            get { return tipoEspacio; }
            set { tipoEspacio = value; }
        }
        /// <summary>
        /// Gets or sets the NumEspacio value.
        /// </summary>
        public virtual string NumEspacio
        {
            get { return numEspacio; }
            set { numEspacio = value; }
        }
        /// <summary>
        /// Gets or sets the Ubicacion  value.
        /// </summary>
        public virtual string Ubicacion
        {
            get { return ubicacion; }
            set { ubicacion = value; }
        }

        /// <summary>
        /// Gets or sets the Dinamica value.
        /// </summary>
        public virtual string Dinamica
        {
            get { return dinamica; }
            set { dinamica = value; }
        }
        /// <summary>
        /// Gets or sets the Detalle value.
        /// </summary>
        public virtual string Detalle
        {
            get { return detalle; }
            set { detalle = value; }
        }
        /// <summary>
        /// Gets or sets the FechaInicio value.
        /// </summary>
        
        public virtual DateTime FechaInicio
        {
            get { return fechaInicio; }
            set { fechaInicio = value; }
        }

        public virtual string FechaInicioAux
        {
            get { return fechaInicio != null ? fechaInicio.ToString("dd/MM/yyyy") : "01/01/1900"; }
            
        }

        public virtual string FechaFinAux
        {
            get { return fechaFin != null ? fechaFin.ToString("dd/MM/yyyy") : "01/01/1900"; }

        }


        /// <summary>
        /// Gets or sets the FechaFin value.
        /// </summary>
        public virtual DateTime FechaFin
        {
            get { return fechaFin; }
            set { fechaFin = value; }
        }

        /// <summary>
        /// Gets or sets the Monto value.
        /// </summary>
        public virtual decimal Monto
        {
            get { return monto; }
            set { monto = value; }
        }

        /// <summary>
        /// Gets or sets the Impuesto value.
        /// </summary>
        public virtual int IDTax
        {
            get { return IdTax; }
            set { IdTax = value; }
        }

        /// <summary>
        /// Gets or sets the Total value.
        /// </summary>
        public virtual decimal Total
        {
            get { return total; }
            set { total = value; }
        }
        #endregion
    }
}
