using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ExtCentral_Manufacturer
    {
        private int iD;
        private string description;

        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
