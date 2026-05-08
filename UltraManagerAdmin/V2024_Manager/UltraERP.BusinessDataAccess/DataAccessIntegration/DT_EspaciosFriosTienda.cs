using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
 public   class DT_EspaciosFriosTienda : DT
    {

        #region Variables
        #endregion

        #region Constructors

        public DT_EspaciosFriosTienda() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Actualizar filas en espacios frios  table.
        /// </summary>
        public virtual Respuesta Save(EN_EspaciosFriosTienda espFrio)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {

                new SqlParameter("@ID",espFrio.ID),
                new SqlParameter("@storeId", (espFrio.StoreId)),
                new SqlParameter("@EspacioId", (espFrio.EspacioId)),
                new SqlParameter("@numPuerta",espFrio.NumPuerta),
                new SqlParameter("@numParrilla",espFrio.NumParrilla),
                new SqlParameter("@porcentaje", espFrio.Porcentaje),
                new SqlParameter("@supplierId", (espFrio.SupplierId)),
                new SqlParameter("@productos", (espFrio.ListaProductos)),
                new SqlParameter("@categoria", (espFrio.Categoria)),
                new SqlParameter("@fechaIni", (espFrio.FechaInicio)),
                new SqlParameter("@FechaFin", (espFrio.FechaFin)),
                new SqlParameter("@taxId", (espFrio.TaxId)),
                new SqlParameter("@total", (espFrio.Total))
                


            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_FRIOSTIENDA_INSERT_UPDATE", parameters);
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
        /// Selects all records from the espacios frios table.
        /// </summary>
        public virtual List<EN_EspaciosFriosTienda> GetAll()
        {


            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_FRIOSTIENDA_GETALL"))
            {
                List<EN_EspaciosFriosTienda> espFrioList = new List<EN_EspaciosFriosTienda>();
                while (dataReader.Read())
                {
                    EN_EspaciosFriosTienda espFrio = MakeEN_ESPACIOFRIOTIENDA(dataReader);
                    espFrioList.Add(espFrio);
                }

                return espFrioList;
            }
        }

        /// <summary>
        /// Creates a new instance of the espacios frios class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_EspaciosFriosTienda MakeEN_ESPACIOFRIOTIENDA(SqlDataReader dataReader)
        {
            EN_EspaciosFriosTienda oeN_friosTienda = new EN_EspaciosFriosTienda();
            oeN_friosTienda.ID = dataReader.IsDBNull(dataReader.GetOrdinal("id")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("id"));
            oeN_friosTienda.StoreId = dataReader.IsDBNull(dataReader.GetOrdinal("StoreId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("StoreId"));
            oeN_friosTienda.EspacioId = dataReader.IsDBNull(dataReader.GetOrdinal("EspacioId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("EspacioId"));
            oeN_friosTienda.NumPuerta = dataReader.IsDBNull(dataReader.GetOrdinal("NumPuerta")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("NumPuerta"));
            oeN_friosTienda.NumParrilla = dataReader.IsDBNull(dataReader.GetOrdinal("NumParrilla")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("NumParrilla"));
            oeN_friosTienda.Porcentaje = dataReader.IsDBNull(dataReader.GetOrdinal("Porcentaje")) ? 0 : dataReader.GetFloat(dataReader.GetOrdinal("Porcentaje"));
            oeN_friosTienda.SupplierId = dataReader.IsDBNull(dataReader.GetOrdinal("ProveedorId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ProveedorId"));
            oeN_friosTienda.ListaProductos = dataReader.IsDBNull(dataReader.GetOrdinal("ListaProductos")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("ListaProductos"));
            oeN_friosTienda.Categoria = dataReader.IsDBNull(dataReader.GetOrdinal("Categoria")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Categoria"));
            oeN_friosTienda.FechaInicio = dataReader.IsDBNull(dataReader.GetOrdinal("FechaInicio")) ? DateTime.MinValue : dataReader.GetDateTime(dataReader.GetOrdinal("FechaInicio"));
            oeN_friosTienda.FechaFin = dataReader.IsDBNull(dataReader.GetOrdinal("FechaFin")) ? DateTime.MaxValue : dataReader.GetDateTime(dataReader.GetOrdinal("FechaFin"));
            oeN_friosTienda.TaxId = dataReader.IsDBNull(dataReader.GetOrdinal("TaxId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("TaxId"));
            oeN_friosTienda.Total = dataReader.IsDBNull(dataReader.GetOrdinal("Total")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Total"));
           
            return oeN_friosTienda;
        }

        #endregion
    }
}
