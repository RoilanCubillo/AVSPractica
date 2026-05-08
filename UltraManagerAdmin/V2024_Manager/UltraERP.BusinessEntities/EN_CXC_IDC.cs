using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_CXC_IDC
    {
        #region Fields
        private int iD;
        private int cXC_ProveedorID;
        private int iDCTipoID;
        private int frecuenciaPagoID;
        private int negociacionID;
        private float valorNegPorcentaje;
        private decimal valorNegMonetizado;
        private string listaCategorias;
        #endregion

        #region Properties
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        public int IDAux { get; set; }
        public int Estado { get; set; }
        public int CXC_ProveedorID
        {
            get { return cXC_ProveedorID; }
            set { cXC_ProveedorID = value; }
        }
        public int IDCTipoID
        {
            get { return iDCTipoID; }
            set { iDCTipoID = value; }
        }
        public int FrecuenciaPagoID
        {
            get { return frecuenciaPagoID; }
            set { frecuenciaPagoID = value; }
        }
        public int NegociacionID
        {
            get { return negociacionID; }
            set { negociacionID = value; }
        }
        public float ValorNegPorcentaje
        {
            get { return valorNegPorcentaje; }
            set { valorNegPorcentaje = value; }
        }
        public decimal ValorNegMonetizado
        {
            get { return valorNegMonetizado; }
            set { valorNegMonetizado = value; }
        }
        public string ListaCategorias
        {
            get { return listaCategorias; }
            set { listaCategorias = value; }
        }
        #endregion
    }
}
