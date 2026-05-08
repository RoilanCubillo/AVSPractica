using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ItemProperty
    {
        public int ID { get; set; }
        public string ItemLookupCode { get; set; }
        public string Description { get; set; }
        public string ExtDescription { get; set; }
        public string Properties { get; set; }
    }
}
