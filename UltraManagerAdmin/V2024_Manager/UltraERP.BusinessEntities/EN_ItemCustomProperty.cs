    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ItemCustomProperty
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public bool Inactive { get; set; }
        public string ListValue { get; set; }
    }
}
