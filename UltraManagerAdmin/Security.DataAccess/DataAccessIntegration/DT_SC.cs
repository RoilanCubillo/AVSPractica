using Security.DataAccess.Data;

namespace Security.DataAccess.DataAccessIntegration
{
    public class DT_SC
    {
        protected DT_Conexion con;
        protected SecurityDBDataContext db;

        protected DT_SC()
        {
            con = new DT_Conexion();
            db = new SecurityDBDataContext();
        }
    }
}
