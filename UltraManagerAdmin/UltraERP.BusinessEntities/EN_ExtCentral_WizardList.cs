using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ExtCentral_WizardList
    {
        private string codigo;
        private string descripcion;
        private char estado;

        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        public char Estado
        {
            get { return estado; }
            set { estado = value; }
        }
    }
}
