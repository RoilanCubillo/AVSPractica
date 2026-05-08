using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT
    {
        protected DT_Conexion con;
        protected SqlConnection cn;
        protected MasterDBDataContext db;

        protected DT()
        {
            con = new DT_Conexion();
            cn = new SqlConnection(con.CadenaConexion);
            db = new MasterDBDataContext();
        }
    }
}
