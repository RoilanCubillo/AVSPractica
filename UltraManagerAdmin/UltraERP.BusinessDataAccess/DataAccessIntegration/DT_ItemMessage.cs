using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ItemMessage : DT
    {
        #region Variables
        protected EN_ItemMessage oEN_ItemMessage = new EN_ItemMessage();
        #endregion

        #region Constructors
        public DT_ItemMessage() : base() { }
        #endregion

        #region Methods
        /// <summary>
        /// Actualizar filas en ItemMessage table.
        /// </summary>
        public virtual Respuesta Save(EN_ItemMessage itemMessage)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HQID", itemMessage.HQID),
                new SqlParameter("@ID", itemMessage.ID),
                new SqlParameter("@Title", (itemMessage.Title==String.Empty)?Convert.DBNull:itemMessage.Title),
                new SqlParameter("@AgeLimit", (itemMessage.AgeLimit==(short)0)?Convert.DBNull:itemMessage.AgeLimit),
                new SqlParameter("@Message", (itemMessage.Message==String.Empty)?Convert.DBNull:itemMessage.Message)
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_ITEMMESSAGE_INSERT_UPDATE", parameters);
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
        /// Seleccionar una fila desde ItemMessage table.
        /// </summary>
        public virtual Respuesta Get(int iD)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", iD)
            };
            try
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_ITEMMESSAGE_GET", parameters))
                {
                    if (dataReader.Read())
                    {
                        return new Respuesta("", "", MakeEN_ItemMessage(dataReader), true);
                    }
                    else
                    {
                        return new Respuesta("RESULTADO: NO LEIDO", "ERROR. NO SE PUDO OBTENER EL REGISTRO", null, false);
                    }
                }
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "ERROR INTERNO. NO SE PUDO OBTENER EL REGISTRO", null, false);
            }

        }

        /// <summary>
        /// Selects all records from the ItemMessage table.
        /// </summary>
        public virtual Respuesta GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ValorBusqueda", busqueda),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@ResultCount", cantidad)
            };

            try
            {
                List<EN_ItemMessage> itemMessageList = new List<EN_ItemMessage>();
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_ITEMMESSAGE_GETALL", parameters))
                {
                    while (dataReader.Read())
                    {
                        EN_ItemMessage itemMessage = MakeEN_ItemMessage(dataReader);
                        itemMessageList.Add(itemMessage);
                    }
                }
                return new Respuesta("", "", itemMessageList, true);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "ERROR INTERNO. NO SE PUDO OBTENER LA LISTA DE REGISTROS", null, false);
            }
        }

        /// <summary>
        /// Creates a new instance of the ItemMessage class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_ItemMessage MakeEN_ItemMessage(SqlDataReader dataReader)
        {
            EN_ItemMessage oeN_ItemMessage = new EN_ItemMessage();
            oeN_ItemMessage.HQID = dataReader.IsDBNull(dataReader.GetOrdinal("HQID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("HQID"));
            oeN_ItemMessage.ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
            oeN_ItemMessage.Title = dataReader.IsDBNull(dataReader.GetOrdinal("Title")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Title"));
            oeN_ItemMessage.AgeLimit = dataReader.IsDBNull(dataReader.GetOrdinal("AgeLimit")) ? (short)0 : dataReader.GetInt16(dataReader.GetOrdinal("AgeLimit"));
            oeN_ItemMessage.Message = dataReader.IsDBNull(dataReader.GetOrdinal("Message")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Message"));

            return oeN_ItemMessage;
        }

        #endregion
    }
}
