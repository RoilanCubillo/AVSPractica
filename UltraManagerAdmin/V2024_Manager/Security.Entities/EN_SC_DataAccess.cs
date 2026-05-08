using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Entities
{
    public class EN_SC_DataAccess : EN_SC_Data
    {
        public int DataID { get; set; }
        public bool EnableAll { get; set; }
        public string DataIDs { get; set; }
    }
}
