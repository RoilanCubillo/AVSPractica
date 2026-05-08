using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Entities
{
    public class EN_SC_Data : EN_SC
    {
        public int SystemID { get; set; }
        public string TableName { get; set; }
        public string TablePKName { get; set; }
    }
}
