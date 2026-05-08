using System.Configuration;
using System.Data.SqlClient;

namespace Analitic.DataAccess
{
    public class DT_Conexion
    {
        public string strCadena;
        private SqlConnection con;

        //*Conexion
        public DT_Conexion()
        {
            strCadena = ConnectionString.Get();

            this.con = new SqlConnection(strCadena);
        }


        //*Propiedad para obtener la conexion
        public SqlConnection conexion
        {
            get
            {
                return con;
            }
        }


        public string CadenaConexion
        {
            get
            {
                return strCadena;
            }
        }
    }
}
