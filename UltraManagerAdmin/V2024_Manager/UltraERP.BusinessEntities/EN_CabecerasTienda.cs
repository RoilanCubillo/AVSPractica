using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_CabecerasTienda
    {
        private int Id;
        private int IdSupplier;
        private int IdCabecera;
        private int storeId;
        private string dinamica;
        private string listaProd;
        private DateTime fechaIni;
        private DateTime fechaFin;
        private int IdTax;
        private decimal montoTotal;
        private string listaCategorias;
        private string detalleDinamica;

        public EN_CabecerasTienda()

        {



        }
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
        public virtual int IDSupplier
        {
            get { return IdSupplier; }
            set { IdSupplier = value; }
        }

        public virtual int IDCabecera
        {
            get { return IdCabecera; }
            set { IdCabecera = value; }
        }
        
        public virtual int StoreId
        {
            get { return storeId; }
            set { storeId = value; }
        }

        public virtual int IDTax
        {
            get { return IdTax; }
            set { IdTax = value; }
        }

        public virtual string Dinamica
        {
            get { return dinamica; }
            set { dinamica = value; }
        }
        public virtual string ListaProd
        {
            get { return listaProd; }
            set { listaProd = value; }
        }
        public virtual DateTime FechaIni
        {
            get { return fechaIni; }
            set { fechaIni = value; }
        }
        public virtual DateTime FechaFin
        {
            get { return fechaFin; }
            set { fechaFin = value; }
        }
        
        public virtual string FechaIniAux
        {
            get { return fechaIni != null ? fechaIni.ToString("dd/MM/yyyy") : "01/01/1900"; }
        }
        public virtual string FechaFinAux
        {
            get { return fechaFin != null ? fechaFin.ToString("dd/MM/yyyy") : "01/01/1900"; }
        }


        public virtual decimal Monto
        {
            get { return montoTotal; }
            set { montoTotal = value; }
        }

        public virtual string DetalleDinamica
        {
            get { return detalleDinamica; }
            set { detalleDinamica = value; }
        }

        public virtual string ListaCategorias
        {
            get { return listaCategorias; }
            set { listaCategorias = value; }
        }
        #endregion
    }
}

