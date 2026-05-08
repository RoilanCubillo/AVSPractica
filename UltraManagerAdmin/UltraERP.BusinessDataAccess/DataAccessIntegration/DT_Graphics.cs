using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Graphics : DT
    {
        #region Variables
        protected EN_Graphics oEN_Graphics = new EN_Graphics();

        #endregion

        #region Constructors

        public DT_Graphics() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Obtiene toda la informacion de los graficos 
        /// </summary>
        public virtual List<EN_Graphics> GetAll(string storeID = "", string codSucursal = "", string usersID = "", string busqueda = "", string fromDate = "", string toDate = "", string tipo = "")
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@STORE_ID", storeID),
                new SqlParameter("@COD_SUCURSAL", codSucursal),
                new SqlParameter("@USERS_ID", usersID),
                new SqlParameter("@SearchValue", busqueda),
                new SqlParameter("@FROMDATE", fromDate),
                new SqlParameter("@TODATE", toDate),
                new SqlParameter("@TIPO", tipo),
            };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_ANALITICA_GRAPHICS_GETALL", parameters))
            {
                List<EN_Graphics> graphicList = new List<EN_Graphics>();
                while (dataReader.Read())
                {
                    EN_Graphics graphic = MakeEN_Graphics(dataReader);
                    graphicList.Add(graphic);
                }

                return graphicList;
            }
        }

        /// <summary>
        /// Creates a new instance of the Category class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_Graphics MakeEN_Graphics(SqlDataReader dataReader)
        {
            EN_Graphics oeN_Graphics = new EN_Graphics();
            oeN_Graphics.Meses = dataReader.IsDBNull(dataReader.GetOrdinal("Meses")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Meses"));
            oeN_Graphics.Doc_Aceptados = dataReader.IsDBNull(dataReader.GetOrdinal("Doc_Aceptados")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Doc_Aceptados"));
            oeN_Graphics.Doc_Rechazados = dataReader.IsDBNull(dataReader.GetOrdinal("Doc_Rechazados")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Doc_Rechazados"));
            oeN_Graphics.Tot_Doc_Aceptados = dataReader.IsDBNull(dataReader.GetOrdinal("Tot_Doc_Aceptados")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Tot_Doc_Aceptados"));
            oeN_Graphics.Tot_Doc_Rechazados = dataReader.IsDBNull(dataReader.GetOrdinal("Tot_Doc_Rechazados")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Tot_Doc_Rechazados"));

            return oeN_Graphics;
        }

        #endregion
    }
}
