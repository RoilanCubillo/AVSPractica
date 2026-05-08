using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
   public class EN_EspaciosFrios
    {

        #region Atributos
        private int id;
        private int storeId;
        private string camara;
        private int puerta;
        private int numParrillas;
        private string dimension;
        private string ubicacion;
        private decimal monto;
        #endregion

        #region Contructor
        public EN_EspaciosFrios() { }
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
        /// Gets or sets the StoreId value.
        /// </summary>
        public virtual int StoreId
        {
            get { return storeId; }
            set { storeId = value; }
        }

        /// <summary>
        /// Gets or sets the IDStore value.
        /// </summary>
        public virtual string Camara
        {
            get { return camara; }
            set { camara = value; }
        }
        /// <summary>
        /// Gets or sets the Puerta value.
        /// </summary>
        public virtual int Puerta
        {
            get { return puerta; }
            set { puerta = value; }
        }
        /// <summary>
        /// Gets or sets the NumParrillas value.
        /// </summary>
        public virtual int NumParrillas
        {
            get { return numParrillas; }
            set { numParrillas = value; }
        }
        /// <summary>
        /// Gets or sets the Dimension value.
        /// </summary>
        public virtual string Dimension
        {
            get { return dimension; }
            set { dimension = value; }
        }

        /// <summary>
        /// Gets or sets the Ubicacion value.
        /// </summary>
        public virtual string Ubicacion
        {
            get { return ubicacion; }
            set { ubicacion = value; }
        }

        /// <summary>
        /// Gets or sets the MOnto value.
        /// </summary>
        public virtual decimal Monto
        {
            get { return monto; }
            set { monto = value; }
        }
        #endregion
    }
}
