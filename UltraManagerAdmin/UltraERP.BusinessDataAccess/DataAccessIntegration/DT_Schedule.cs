using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Schedule : DT
    {
        #region Variables
        protected EN_Schedule oEN_Schedule = new EN_Schedule();
        #endregion

        #region Constructors
        public DT_Schedule() : base() { }
        #endregion

        #region Methods
        /// <summary>
        /// Actualizar filas en Schedule table.
        /// </summary>
        public virtual Respuesta Save(EN_Schedule schedule, List<EN_ScheduleSegment> segments)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", schedule.ID),
                new SqlParameter("@HQID", schedule.HQID),
                new SqlParameter("@Description", (schedule.Description==String.Empty)?Convert.DBNull:schedule.Description),
                new SqlParameter("@Increment", (schedule.Increment==0)?Convert.DBNull:schedule.Increment)
            };
            SqlTransaction tr = null;
            Respuesta respuesta = new Respuesta("", "", null, false);
            try
            {

                using (SqlConnection cnn = new SqlConnection(con.CadenaConexion))
                {
                    cnn.Open();
                    respuesta.Message += " CONNECT_OPEN ";

                    tr = cnn.BeginTransaction("SampleTransaction");
                    respuesta.Message += " TRANSAC_CREADA ";
                    
                    SqlCommand cm = cnn.CreateCommand();
                    respuesta.Message += " COMAND_CREADO ";



                    cm.Transaction = tr;
                    respuesta.Message += " TRANSAC_INSERT ";

                    cm.CommandType = CommandType.StoredProcedure;
                    respuesta.Message += " TYPE_MET ";

                    cm.Parameters.AddRange(parameters);
                    respuesta.Message += " PARAMS_MET ";

                    using (SqlDataReader dataReader = cm.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                            int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));

                            if (String.IsNullOrEmpty(result))
                            {
                                if (tr != null) tr.Rollback();
                                return new Respuesta("RESULTADO: VACIO", "ERROR AL OBTENER LA RESPUESTA DEL PROCEDIMIENTO", scope, false);
                            }
                            else if (result.Contains("ERROR"))
                            {
                                if (tr != null) tr.Rollback();
                                return new Respuesta("ERROR AL ACCIONAR EL PROCEDIMIENTO", result, scope, false);
                            }
                            cn.Close();
                            if (tr != null) tr.Commit();
                            return new Respuesta("", result + ((segments != null) ? segments.Count.ToString() : "0"), scope, true);
                        }
                        return new Respuesta("RESULTADO: NO LEIDO", "ERROR. NO SE PUDO LEER LA RESPUESTA DEL PROCEDIMIENTO", null, false);
                    }
                }
                /*
                cn.Open();


                SqlCommand cm = cn.CreateCommand();


                tr = cn.BeginTransaction(IsolationLevel.ReadCommitted);
                respuesta.Message += " TRANSAC_CREADA ";

                cm.Connection = cn;
                respuesta.Message += " CONNC_INSERT ";

                cm.Transaction = tr;
                respuesta.Message += " TRANSAC_INSERT ";

                cm.CommandText = "UEP_SCHEDULE_INSERT_UPDATE";
                respuesta.Message += " TEXT_INSERT ";





                SqlDataReader dataReader = cm.ExecuteReader();
                respuesta.Message += " EXEC_DR ";

                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                    int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));

                    if (String.IsNullOrEmpty(result))
                    {
                        if (tr != null) tr.Rollback();
                        return new Respuesta("RESULTADO: VACIO", "ERROR AL OBTENER LA RESPUESTA DEL PROCEDIMIENTO", scope, false);
                    }
                    else if (result.Contains("ERROR"))
                    {
                        if (tr != null) tr.Rollback();
                        return new Respuesta("ERROR AL ACCIONAR EL PROCEDIMIENTO", result, scope, false);
                    }
                    //cn.Close();
                    if (tr != null) tr.Commit();
                    return new Respuesta("", result + ((segments != null) ? segments.Count.ToString() : "0"), scope, true);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "ERROR. NO SE PUDO LEER LA RESPUESTA DEL PROCEDIMIENTO", null, false);
                */
                /*SqlCommand com = new SqlCommand("dbo.UEP_SCHEDULE_INSERT_UPDATE", cn);
                tr = cn.BeginTransaction();
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddRange(parameters);
                com.Transaction = tr;


                SqlCommand command = new SqlCommand("UEP_SCHEDULE_INSERT_UPDATE", cn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters);*/

                /*cn.Open();

                tr = cn.BeginTransaction();
                

                Console.WriteLine("LOL");

                SqlDataReader dataReader = SqlHelper.ExecuteReader(tr, CommandType.StoredProcedure, "UEP_SCHEDULE_INSERT_UPDATE", parameters);
                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                    int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));

                    if (String.IsNullOrEmpty(result))
                    {
                        //if (tr != null) tr.Rollback();
                        return new Respuesta("RESULTADO: VACIO", "ERROR AL OBTENER LA RESPUESTA DEL PROCEDIMIENTO", scope, false);
                    }
                    else if (result.Contains("ERROR"))
                    {
                        //if (tr != null) tr.Rollback();
                        return new Respuesta("ERROR AL ACCIONAR EL PROCEDIMIENTO", result, scope, false);
                    }
                    if (tr != null) tr.Commit();
                    return new Respuesta("", result + ((segments != null) ? segments.Count.ToString() : "0"), scope, true);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "ERROR. NO SE PUDO LEER LA RESPUESTA DEL PROCEDIMIENTO", null, false);*/
            }
            catch (Exception e)
            {
                //cn.Close();
                if (tr != null) tr.Rollback();
                respuesta.Message += " : " + e.Message;
                //return new Respuesta(e.Message, "ERROR INTERNO. OCURRIÓ UN ERROR AL GUARDAR", null, false);
            }
            return respuesta;
        }

        /// <summary>
        /// Seleccionar una fila desde Schedule table.
        /// </summary>
        public virtual Respuesta Get(int iD)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", iD)
            };
            try
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_SCHEDULE_GET", parameters))
                {
                    if (dataReader.Read())
                    {
                        return new Respuesta("", "", MakeEN_Schedule(dataReader), true);
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
        /// Selects all records from the Schedule table.
        /// </summary>
        public virtual List<EN_Schedule> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ValorBusqueda", busqueda),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@ResultCount", cantidad)
            };

            try
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_SCHEDULE_GETALL", parameters))
                {
                    List<EN_Schedule> scheduleList = new List<EN_Schedule>();
                    while (dataReader.Read())
                    {
                        EN_Schedule schedule = MakeEN_Schedule(dataReader);
                        scheduleList.Add(schedule);
                    }
                    return scheduleList;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a new instance of the Schedule class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_Schedule MakeEN_Schedule(SqlDataReader dataReader)
        {
            EN_Schedule oeN_Schedule = new EN_Schedule();
            oeN_Schedule.ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
            oeN_Schedule.HQID = dataReader.IsDBNull(dataReader.GetOrdinal("HQID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("HQID"));
            oeN_Schedule.Description = dataReader.IsDBNull(dataReader.GetOrdinal("Description")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Description"));
            oeN_Schedule.Increment = dataReader.IsDBNull(dataReader.GetOrdinal("Increment")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Increment"));

            return oeN_Schedule;
        }
        #endregion
    }
}
