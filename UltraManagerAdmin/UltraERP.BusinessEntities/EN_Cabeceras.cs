using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_Cabeceras
    {
        #region Fields

        private int Id;
        private int IdTienda;
        private int IdCategoria;
        private string numCabecera;
        private string tipo;
        private string ubicacion;
        private decimal monto;
        private int electricidad;
        #endregion


        #region Constructors

        public EN_Cabeceras()
        {


        }


        public EN_Cabeceras(int ID, int IDTienda, string NumCab, string Tipo, string Ubicacion, decimal Monto, int Electricidad)
        {

            this.Id = ID;
            this.IdTienda = IDTienda;
            this.numCabecera = NumCab;
            this.tipo = Tipo;
            this.ubicacion = Ubicacion;
            this.monto = Monto;
            this.electricidad = Electricidad;
        }

        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the ID value.
        /// </summary>
        /// 
        public virtual int ID
        {
            get { return Id; }
            set { Id = value; }
        }
        public virtual int IDTienda
        {
            get { return IdTienda; }
            set { IdTienda = value; }
        }

        public virtual int IDCategoria
        {
            get { return IdCategoria; }
            set { IdCategoria = value; }
        }

        public virtual int Electricidad
        {
            get { return electricidad; }
            set { electricidad = value; }
        }

        public virtual string NumCab
        {
            get { return numCabecera; }
            set { numCabecera = value; }
        }

        public virtual string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }
        public virtual string Ubicacion
        {
            get { return ubicacion; }
            set { ubicacion = value; }
        }

        public virtual decimal Monto
        {
            get { return monto; }
            set { monto = value; }
        }


        #endregion
    }
}
