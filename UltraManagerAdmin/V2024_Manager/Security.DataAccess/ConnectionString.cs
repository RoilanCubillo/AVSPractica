using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.DataAccess
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
            //return Properties.Settings.Default.AVS_SECURITYConnectionString1.ToString().Trim();

            //llamar la conexión del Web.config (El que esta en Security.WebServices)
            return ConfigurationManager.ConnectionStrings["Security.DataAccess.Properties.Settings.AVS_SECURITYConnectionString1"].ConnectionString;

        }
    }
}
