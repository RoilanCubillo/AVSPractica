using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Entities
{
    public class EN_SC_User
    {
        public int ID { get; set; }
        public int AutoID { get; set; }
        public int SystemID { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public bool EnableCloseSession { get; set; }
        public bool Enable { get; set; }
    }
}
