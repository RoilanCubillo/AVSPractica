using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_IslasSideKick : DT
    {
        #region Variables
        protected EN_IslasSideKick oEN_IslasSideKick = new EN_IslasSideKick();
        #endregion

        #region Constructors

        public DT_IslasSideKick() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Actualizar filas en IslasSide table.
        /// </summary>
        public virtual Respuesta Save(EN_IslasSideKick islasSidekick)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
              
                new SqlParameter("@id", islasSidekick.ID),
                new SqlParameter("@IDTienda", (islasSidekick.IDStore)),
                new SqlParameter("@IDProv", (islasSidekick.IDSupplier)),
                new SqlParameter("@Tipo", (islasSidekick.TipoEspacio)),
                new SqlParameter("@Num", (islasSidekick.NumEspacio)),
                new SqlParameter("@Ubicacion", (islasSidekick.Ubicacion)),
                new SqlParameter("@Dinamica", (islasSidekick.Dinamica)),
                new SqlParameter("@Detalle", (islasSidekick.Detalle)),
                new SqlParameter("@fechaIni", (islasSidekick.FechaInicio)),
                new SqlParameter("@fechaFin", (islasSidekick.FechaFin)),
                new SqlParameter("@Monto", (islasSidekick.Monto)),
                new SqlParameter("@TaxId", (islasSidekick.IDTax)),
                new SqlParameter("@Total", (islasSidekick.Total))
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_ASIGNAESPACIOS_INSERT_UPDATE", parameters);
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
        /// Seleccionar una fila desde Category table.
        /// </summary>
        public virtual Respuesta Get(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", ID)
            };

            try
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_ASIGNAESPACIOS_GET", parameters))
                {
                    if (dataReader.Read())
                    {
                        return new Respuesta("", "", MakeEN_IslasSideKick(dataReader), true);
                    }
                    return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
                }
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }

        }
        /// <summary>
        /// Selects all records from the Category table.
        /// </summary>
        public virtual List<EN_IslasSideKick> GetAll()
        {
            

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_ASIGNAESPACIOS_GETALL"))
            {
                List<EN_IslasSideKick> islasSideKickList = new List<EN_IslasSideKick>();
                while (dataReader.Read())
                {
                    EN_IslasSideKick islasSideKick = MakeEN_IslasSideKick(dataReader);
                    islasSideKickList.Add(islasSideKick);
                }

                return islasSideKickList;
            }
        }

     

        /// <summary>
        /// Creates a new instance of the Category class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_IslasSideKick MakeEN_IslasSideKick(SqlDataReader dataReader)
        {
            
            EN_IslasSideKick oeN_IslasSideKick = new EN_IslasSideKick();
            oeN_IslasSideKick.ID = dataReader.IsDBNull(dataReader.GetOrdinal("Id")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Id"));
            oeN_IslasSideKick.IDStore = dataReader.IsDBNull(dataReader.GetOrdinal("StoreId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("StoreId"));
            oeN_IslasSideKick.IDSupplier = dataReader.IsDBNull(dataReader.GetOrdinal("SupplierId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SupplierId"));
            oeN_IslasSideKick.TipoEspacio = dataReader.IsDBNull(dataReader.GetOrdinal("TipoEspacio")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("TipoEspacio"));
            oeN_IslasSideKick.NumEspacio = dataReader.IsDBNull(dataReader.GetOrdinal("NumEspacio")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("NumEspacio"));
            oeN_IslasSideKick.Ubicacion = dataReader.IsDBNull(dataReader.GetOrdinal("Ubicacion")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Ubicacion"));
            oeN_IslasSideKick.Dinamica= dataReader.IsDBNull(dataReader.GetOrdinal("Dinamica")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Dinamica"));
            oeN_IslasSideKick.Detalle= dataReader.IsDBNull(dataReader.GetOrdinal("DetalleDinamica")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("DetalleDinamica"));
            oeN_IslasSideKick.FechaInicio = dataReader.IsDBNull(dataReader.GetOrdinal("FechaInicio")) ? DateTime.MinValue : dataReader.GetDateTime(dataReader.GetOrdinal("FechaInicio"));
            oeN_IslasSideKick.FechaFin = dataReader.IsDBNull(dataReader.GetOrdinal("FechaFin")) ? DateTime.MaxValue : dataReader.GetDateTime(dataReader.GetOrdinal("FechaFin"));
            oeN_IslasSideKick.Monto = dataReader.IsDBNull(dataReader.GetOrdinal("Monto")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Monto"));
            oeN_IslasSideKick.IDTax = dataReader.IsDBNull(dataReader.GetOrdinal("TaxId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("TaxId"));
            oeN_IslasSideKick.Total = dataReader.IsDBNull(dataReader.GetOrdinal("Total")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Total"));
            return oeN_IslasSideKick;
        }

        #endregion
    }
}
