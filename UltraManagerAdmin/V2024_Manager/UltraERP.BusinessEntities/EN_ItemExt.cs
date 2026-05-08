using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_ItemExt
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string PropertyName { get; set; }
        public int PropertyType { get; set; }
        public string Value { get; set; }
    }
}
