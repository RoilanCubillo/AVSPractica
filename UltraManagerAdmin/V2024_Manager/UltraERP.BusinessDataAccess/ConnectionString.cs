using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessDataAccess
{
    static class ConnectionString
    {
        public static string Get()
        {

            // Mostrar las cadenas disponibles (solo para depuración)
            //foreach (ConnectionStringSettings conn in ConfigurationManager.ConnectionStrings)
            //{
            //    Console.WriteLine($"[DEBUG] Found connection string: {conn.Name}");
            //}

            //Conexion anterior
            //return Properties.Settings.Default.ADGROUP_CENTRALConnectionString.ToString().Trim();

            //llamar la conexión del Web.config (El que esta en UltraERP.WebServices)
            return ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.BM_RMHCConnectionString"].ConnectionString;

        }
    }
}
