using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ScheduleSegment : DT
    {
        #region Variables
        protected EN_ScheduleSegment oEN_ScheduleSegment = new EN_ScheduleSegment();
        #endregion

        #region Constructors
        public DT_ScheduleSegment() : base() { }
        #endregion

        #region Methods
        /// <summary>
        /// Insertar filas dentro de la tablaScheduleSegment
        /// </summary>
        public virtual Respuesta Insert(EN_ScheduleSegment scheduleSegment)
        {
            SqlCommand comComando = new SqlCommand();
            comComando.Connection = cn;

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HQID", (scheduleSegment.HQID==0)?Convert.DBNull:scheduleSegment.HQID),
                new SqlParameter("@ScheduleID", (scheduleSegment.ScheduleID==0)?Convert.DBNull:scheduleSegment.ScheduleID),
                new SqlParameter("@StartDay", (scheduleSegment.StartDay==0)?Convert.DBNull:scheduleSegment.StartDay),
                new SqlParameter("@EndDay", (scheduleSegment.EndDay==0)?Convert.DBNull:scheduleSegment.EndDay),
                new SqlParameter("@StartTime", (scheduleSegment.StartTime==new DateTime(1900,01,01))?Convert.DBNull:scheduleSegment.StartTime),
                new SqlParameter("@EndTime", (scheduleSegment.EndTime==new DateTime(1900,01,01))?Convert.DBNull:scheduleSegment.EndTime),
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_SCHEDULESEGMENT_INSERT", parameters);
                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                    int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));

                    if (String.IsNullOrEmpty(result)) return new Respuesta("RESULTADO: VACIO", "ERROR AL OBTENER LA RESPUESTA DEL PROCEDIMIENTO", scope, false);
                    else if (result.Contains("ERROR")) return new Respuesta("ERROR AL ACCIONAR EL PROCEDIMIENTO", result, scope, false);
                    return new Respuesta("", result, scope, true);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "ERROR. NO SE PUDO LEER LA RESPUESTA DEL PROCEDIMIENTO", null, false);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "ERROR INTERNO. OCURRIÓ UN ERROR AL GUARDAR", null, false);
            }
        }

        /// <summary>
        /// Elimina filas desde ScheduleSegment table by its primary key.
        /// </summary>
        public virtual Respuesta DeleteByScheduleID(int scheduleID)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ScheduleID", scheduleID)
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_SCHEDULESEGMENT_DELETE_BY_SCHEDULEID", parameters);
                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));

                    if (String.IsNullOrEmpty(result)) return new Respuesta("RESULTADO: VACIO", "ERROR AL OBTENER LA RESPUESTA DEL PROCEDIMIENTO", null, false);
                    else if (result.Contains("ERROR")) return new Respuesta("ERROR AL ACCIONAR EL PROCEDIMIENTO", result, null, false);
                    return new Respuesta("", result, null, true);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "ERROR. NO SE PUDO LEER LA RESPUESTA DEL PROCEDIMIENTO", null, false);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "ERROR INTERNO. OCURRIÓ UN ERROR AL ELIMINAR", null, false);
            }
        }

        /// <summary>
        /// Selects all records from the ScheduleSegment table.
        /// </summary>
        public virtual List<EN_ScheduleSegment> GetAll()
        {
            try
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_SCHEDULESEGMENT_GETALL"))
                {
                    List<EN_ScheduleSegment> scheduleSegmentList = new List<EN_ScheduleSegment>();
                    while (dataReader.Read())
                    {
                        EN_ScheduleSegment scheduleSegment = MakeEN_ScheduleSegment(dataReader);
                        scheduleSegmentList.Add(scheduleSegment);
                    }

                    return scheduleSegmentList;
                }
            }
            catch (Exception) { return null; }
        }
        /// <summary>
        /// Selects all records from the ScheduleSegment table.
        /// </summary>
        public virtual List<EN_ScheduleSegment> GetByScheduleID(int scheduleID)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ScheduleID", scheduleID) };
            try
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_SCHEDULESEGMENT_GET_BY_SCHEDULEID", parameters))
                {
                    List<EN_ScheduleSegment> scheduleSegmentList = new List<EN_ScheduleSegment>();
                    while (dataReader.Read())
                    {
                        EN_ScheduleSegment scheduleSegment = MakeEN_ScheduleSegment(dataReader);
                        scheduleSegmentList.Add(scheduleSegment);
                    }

                    return scheduleSegmentList;
                }
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Creates a new instance of the ScheduleSegment class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_ScheduleSegment MakeEN_ScheduleSegment(SqlDataReader dataReader)
        {
            EN_ScheduleSegment oeN_ScheduleSegment = new EN_ScheduleSegment();
            oeN_ScheduleSegment.ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
            oeN_ScheduleSegment.HQID = dataReader.IsDBNull(dataReader.GetOrdinal("HQID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("HQID"));
            oeN_ScheduleSegment.ScheduleID = dataReader.IsDBNull(dataReader.GetOrdinal("ScheduleID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ScheduleID"));
            oeN_ScheduleSegment.StartDay = dataReader.IsDBNull(dataReader.GetOrdinal("StartDay")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("StartDay"));
            oeN_ScheduleSegment.EndDay = dataReader.IsDBNull(dataReader.GetOrdinal("EndDay")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("EndDay"));
            oeN_ScheduleSegment.StartTime = dataReader.IsDBNull(dataReader.GetOrdinal("StartTime")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("StartTime"));
            oeN_ScheduleSegment.EndTime = dataReader.IsDBNull(dataReader.GetOrdinal("EndTime")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("EndTime"));

            return oeN_ScheduleSegment;
        }
        #endregion
    }
}
