using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessDataAccess
{
    static class ConnectionString
    {
        public static string Get()
        {
            return Properties.Settings.Default.MasterDB.ToString().Trim();
        }
    }
}
