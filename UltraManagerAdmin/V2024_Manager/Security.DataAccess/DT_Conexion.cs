using System.Configuration;
using System.Data.SqlClient;

namespace Security.DataAccess
{
    public class DT_Conexion
    {
        private string strConnection;
        private SqlConnection con;

        //*Conexion
        public DT_Conexion()
        {
            this.strConnection = ConnectionString.Get();

            this.con = new SqlConnection(this.strConnection);
        }

        public DT_Conexion(string strConnection)
        {
            con = new SqlConnection(strConnection);

            this.strConnection = this.con.ConnectionString;
        }


        //*Propiedad para obtener la conexion
        public SqlConnection Connection
        {
            get
            {
                return con;
            }
        }


        public string StrConnection
        {
            get
            {
                return strConnection;
            }
        }
    }
}
