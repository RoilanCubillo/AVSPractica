using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analitic.DataAccess.Data;

namespace Analitic.DataAccess.DataAccessIntegration
{
    public class DT
    {
        protected DT_Conexion con;
        protected SqlConnection cn;
        protected AnaliticDBDataContext db;

        protected DT()
        {
            con = new DT_Conexion();
            cn = new SqlConnection(con.CadenaConexion);
            db = new AnaliticDBDataContext();
        }

    }
}

