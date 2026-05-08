using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ItemTax
    {
        private int id;
        private int taxID1;
        private int taxID2;
        private int taxID3;
        private string code;
        private string description;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public int TaxID1
        {
            get { return taxID1; }
            set { taxID1 = value; }
        }
        public int TaxID2
        {
            get { return taxID2; }
            set { taxID2 = value; }
        }
        public int TaxID3
        {
            get { return taxID3; }
            set { taxID3 = value; }
        }
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
