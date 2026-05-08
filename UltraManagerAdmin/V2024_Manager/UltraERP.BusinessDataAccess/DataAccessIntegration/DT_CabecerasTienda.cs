using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_CabecerasTienda : DT
    {

        #region Constructors

        public DT_CabecerasTienda() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Actualizar filas en Category table.
        /// </summary>
        public virtual Respuesta Save(EN_CabecerasTienda cabecera)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id",cabecera.ID),
                new SqlParameter("@SupplierId", (cabecera.IDSupplier)),
                new SqlParameter("@CabeceraId", (cabecera.IDCabecera)),
                new SqlParameter("@StoreId", (cabecera.StoreId)),
                new SqlParameter("@Dinamica", (cabecera.Dinamica)),
                new SqlParameter("@DetalleDinamica", (cabecera.DetalleDinamica)),
                new SqlParameter("@ListaAsociado", (cabecera.ListaProd)),
                new SqlParameter("@ListaCategorias", (cabecera.ListaCategorias)),
                new SqlParameter("@FechaInicia", (cabecera.FechaIni)),
                new SqlParameter("@FechaFin", (cabecera.FechaFin)),
                new SqlParameter("@Total", (cabecera.Monto)),
                new SqlParameter("@TaxId", cabecera.IDTax),
                new SqlParameter("@Usuario", "admin")
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_CABECERATIENDA_INSERT_UPDATE", parameters);
                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                    int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));

                    if (String.IsNullOrEmpty(result)) return new Respuesta("RESULTADO: VACIO", "error_get_proc", scope, false);
                    else if (result.Contains("ERROR")) return new Respuesta("ERROR AL ACCIONAR EL PROCEDIMIENTO", result, scope, false);
                    return new Respuesta("", result, scope, true);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }
        }


        /// <summary>
        /// Selects all records from the Category table.
        /// </summary>
        public virtual List<EN_CabecerasTienda> GetAll()
        {


            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_CABECERATIENDA_GETALL"))
            {
                List<EN_CabecerasTienda> cabeceraList = new List<EN_CabecerasTienda>();
                while (dataReader.Read())
                {
                    EN_CabecerasTienda cabecera = MakeEN_Cabecera(dataReader);
                    cabeceraList.Add(cabecera);
                }

                return cabeceraList;
            }
        }

        /// <summary>
        /// Creates a new instance of the Category class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_CabecerasTienda MakeEN_Cabecera(SqlDataReader dataReader)
        {
            EN_CabecerasTienda oeN_Cabecera = new EN_CabecerasTienda();
            oeN_Cabecera.ID = dataReader.IsDBNull(dataReader.GetOrdinal("Id")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Id"));
            oeN_Cabecera.IDSupplier = dataReader.IsDBNull(dataReader.GetOrdinal("SupplierId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SupplierId"));
            oeN_Cabecera.IDCabecera = dataReader.IsDBNull(dataReader.GetOrdinal("CabeceraId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("CabeceraId"));
            oeN_Cabecera.StoreId = dataReader.IsDBNull(dataReader.GetOrdinal("StoreId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("StoreId"));
            oeN_Cabecera.Dinamica = dataReader.IsDBNull(dataReader.GetOrdinal("Dinamica")) ? "" : dataReader.GetString(dataReader.GetOrdinal("Dinamica"));
            oeN_Cabecera.DetalleDinamica = dataReader.IsDBNull(dataReader.GetOrdinal("DetalleDinamica")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("DetalleDinamica"));
            oeN_Cabecera.ListaProd = dataReader.IsDBNull(dataReader.GetOrdinal("ListaAsociado")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("ListaAsociado"));
            oeN_Cabecera.ListaCategorias = dataReader.IsDBNull(dataReader.GetOrdinal("ListaCategorias")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("ListaCategorias"));
            oeN_Cabecera.FechaIni = dataReader.IsDBNull(dataReader.GetOrdinal("FechaInicia")) ? DateTime.MinValue : dataReader.GetDateTime(dataReader.GetOrdinal("FechaInicia"));
            oeN_Cabecera.FechaFin = dataReader.IsDBNull(dataReader.GetOrdinal("FechaFin")) ? DateTime.MaxValue : dataReader.GetDateTime(dataReader.GetOrdinal("FechaFin"));
            oeN_Cabecera.IDTax = dataReader.IsDBNull(dataReader.GetOrdinal("TaxId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("TaxId"));
            oeN_Cabecera.Monto = dataReader.IsDBNull(dataReader.GetOrdinal("Total")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Total"));
            //iRow["Fecha_Fin"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(iRow["Fecha_Fin"]);
            return oeN_Cabecera;
        }

        #endregion
    }
}
