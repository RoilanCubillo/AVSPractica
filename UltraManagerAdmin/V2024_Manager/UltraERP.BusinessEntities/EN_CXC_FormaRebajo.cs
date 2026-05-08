using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_CXC_FormaRebajo
    {
        private int iD;
        private string nombre;

        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
    }
}
