using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analitic.DataAccess
{
    static class ConnectionString
    {
        public static string Get()
        {
            return Properties.Settings.Default.AVS_ANALITICAConnectionString.ToString().Trim();
        }
    }
}
