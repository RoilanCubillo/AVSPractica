using Analitic.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;

namespace Analitic.DataAccess.DataAccessIntegration
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
                    if (tipo == "Docs_MH")
                    {
                        EN_Graphics graphic = MakeEN_Graphics_DocsMH(dataReader);
                        graphicList.Add(graphic);
                    }
                    else if(tipo == "Docs_ERP")
                    {
                        EN_Graphics graphic = MakeEN_Graphics_DocsERP(dataReader);
                        graphicList.Add(graphic);
                    }
                    else if (tipo == "Asientos_ERP")
                    {
                        EN_Graphics graphic = MakeEN_Graphics_AsientosERP(dataReader);
                        graphicList.Add(graphic);
                    }

                }

                return graphicList;
            }
        }

        /// <summary>
        /// Creates a new instance of the Category class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_Graphics MakeEN_Graphics_DocsMH(SqlDataReader dataReader)
        {
            EN_Graphics oeN_Graphics = new EN_Graphics();
            oeN_Graphics.Meses = dataReader.IsDBNull(dataReader.GetOrdinal("Meses")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Meses"));
            oeN_Graphics.Doc_Aceptados = dataReader.IsDBNull(dataReader.GetOrdinal("Doc_Aceptados")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Doc_Aceptados"));
            oeN_Graphics.Doc_Rechazados = dataReader.IsDBNull(dataReader.GetOrdinal("Doc_Rechazados")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Doc_Rechazados"));
            oeN_Graphics.Tot_Doc_Aceptados = dataReader.IsDBNull(dataReader.GetOrdinal("Tot_Doc_Aceptados")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Tot_Doc_Aceptados"));
            oeN_Graphics.Tot_Doc_Rechazados = dataReader.IsDBNull(dataReader.GetOrdinal("Tot_Doc_Rechazados")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Tot_Doc_Rechazados"));

            return oeN_Graphics;
        }

        protected virtual EN_Graphics MakeEN_Graphics_DocsERP(SqlDataReader dataReader)
        {
            EN_Graphics oeN_Graphics = new EN_Graphics();
            oeN_Graphics.Total_Ventas = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Ventas")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Ventas"));
            oeN_Graphics.Total_Ventas_Sync = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Ventas_Sync")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Ventas_Sync"));
            oeN_Graphics.Total_NcVentas = dataReader.IsDBNull(dataReader.GetOrdinal("Total_NcVentas")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_NcVentas"));
            oeN_Graphics.Total_NcVentas_Sync = dataReader.IsDBNull(dataReader.GetOrdinal("Total_NcVentas_Sync")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_NcVentas_Sync"));
            oeN_Graphics.Total_Compras = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Compras")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Compras"));
            oeN_Graphics.Total_Compras_Sync = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Compras_Sync")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Compras_Sync"));
            oeN_Graphics.Total_NcCompras = dataReader.IsDBNull(dataReader.GetOrdinal("Total_NcCompras")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_NcCompras"));
            oeN_Graphics.Total_NcCompras_Sync = dataReader.IsDBNull(dataReader.GetOrdinal("Total_NcCompras_Sync")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_NcCompras_Sync"));
            oeN_Graphics.Total_Depositos = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Depositos")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Depositos"));
            oeN_Graphics.Total_Depositos_Sync = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Depositos_Sync")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Depositos_Sync"));
            oeN_Graphics.Total_Docs_NoSync = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Docs_NoSync")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Docs_NoSync"));

            return oeN_Graphics;
        }

        protected virtual EN_Graphics MakeEN_Graphics_AsientosERP(SqlDataReader dataReader)
        {
            EN_Graphics oeN_Graphics = new EN_Graphics();
            oeN_Graphics.Total_Asientos = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Asientos")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Asientos"));
            oeN_Graphics.Total_Asientos_Sync = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Asientos_Sync")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Asientos_Sync"));
            oeN_Graphics.Total_Asientos_No_Sync = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Asientos_No_Sync")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Asientos_No_Sync"));
            oeN_Graphics.Total_Asientos_Error = dataReader.IsDBNull(dataReader.GetOrdinal("Total_Asientos_Error")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Total_Asientos_Error"));

            return oeN_Graphics;
        }

        #endregion
    }
}
