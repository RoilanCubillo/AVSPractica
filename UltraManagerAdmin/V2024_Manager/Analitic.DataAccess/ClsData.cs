using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analitic.DataAccess
{
    public class ClsData
    {
        protected DT_Conexion con = new DT_Conexion();
        public string _SQLConnection;

        public ClsData() { _SQLConnection = con.CadenaConexion; }

        public DataTable SQLCargaDataTable(string SQLQuery, DataTable dt)
        {
            SqlConnection cmd = new SqlConnection(_SQLConnection);
            dt = new DataTable();
            try
            {
                //string Querry = "";

                SqlDataAdapter da = new SqlDataAdapter(SQLQuery, cmd);
                da.SelectCommand.CommandTimeout = 0;
                dt = new DataTable();
                da.Fill(dt);

                cmd.Dispose();
                return dt;
            }

            catch (Exception)
            {
                return dt;
            }

            finally
            {
                cmd.Dispose();
            }

        }

        public SqlDataReader SQLCargaDataReader(string SQLQuery)
        {
            SqlConnection cn = new SqlConnection();
            SqlCommand sqlCommand = new SqlCommand(SQLQuery, cn);
            SqlDataReader dr;

            try
            {
                cn = new SqlConnection(_SQLConnection);
                if ((cn.State == ConnectionState.Closed))
                {
                    cn.Open();
                }
                sqlCommand = new SqlCommand(SQLQuery, cn);
                sqlCommand.CommandTimeout = 0;

                dr = sqlCommand.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void bulkItToTable(DataTable dt, string ToTable)
        {
            SqlConnection cn = new SqlConnection(this._SQLConnection);
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(cn))
            {
                cn.Open();
                bulkCopy.DestinationTableName = ToTable;
                try
                {
                    bulkCopy.WriteToServer(dt);
                }
                catch (Exception e) { throw e; }
            }
        }

        public void SQLExecute(string SQLQuery)
        {
            using (SqlConnection conn = new SqlConnection(_SQLConnection))
            using (SqlCommand cmd = new SqlCommand(SQLQuery, conn))
            {
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception) { }
            }
        }
    }
}
