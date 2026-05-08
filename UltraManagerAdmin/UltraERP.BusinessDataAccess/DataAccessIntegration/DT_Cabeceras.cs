using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Cabeceras : DT
    {

        #region Constructors

        public DT_Cabeceras() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Actualizar filas en Category table.
        /// </summary>
        public virtual Respuesta Save(EN_Cabeceras cabecera)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID",cabecera.ID),
                new SqlParameter("@IDTienda", cabecera.IDTienda),
                new SqlParameter("@IDCategoria", cabecera.IDCategoria),
                new SqlParameter("@NumCab", cabecera.NumCab),
                new SqlParameter("@Tipo", cabecera.Tipo),
                new SqlParameter("@Ubicacion", cabecera.Ubicacion),
                new SqlParameter("@Monto", cabecera.Monto),
                new SqlParameter("@Electricidad", cabecera.Electricidad)
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_CABECERA_INSERT_UPDATE", parameters);
                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                    int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));

                    if (String.IsNullOrEmpty(result)) return new Respuesta("RESULTADO: VACIO", "error_get_proc", scope, false);
                    else if (result.ToUpper().Contains("ERROR")) return new Respuesta("ERROR AL ACCIONAR EL PROCEDIMIENTO", result, scope, false);
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
                new SqlParameter("@Id", ID)
            };

            try
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_CABECERA_GET", parameters))
                {
                    if (dataReader.Read())
                    {
                        return new Respuesta("", "", MakeEN_Cabecera(dataReader), true);
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
        public virtual List<EN_Cabeceras> GetAll()
        {
         

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_CABECERA_GETALL"))
            {
                List<EN_Cabeceras> cabeceraList = new List<EN_Cabeceras>();
                while (dataReader.Read())
                {
                    EN_Cabeceras cabecera = MakeEN_Cabecera(dataReader);
                    cabeceraList.Add(cabecera);
                }

                return cabeceraList;
            }
        }

        /// <summary>
        /// Creates a new instance of the Category class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_Cabeceras MakeEN_Cabecera(SqlDataReader dataReader)
        {
            EN_Cabeceras oeN_Cabecera = new EN_Cabeceras();
            oeN_Cabecera.ID = dataReader.IsDBNull(dataReader.GetOrdinal("Id")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Id"));
            oeN_Cabecera.IDTienda = dataReader.IsDBNull(dataReader.GetOrdinal("StoreId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("StoreId"));           
            oeN_Cabecera.IDCategoria = dataReader.IsDBNull(dataReader.GetOrdinal("CategoryId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("CategoryId"));           
            oeN_Cabecera.NumCab = dataReader.IsDBNull(dataReader.GetOrdinal("NumCabecera")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("NumCabecera"));
            oeN_Cabecera.Tipo = dataReader.IsDBNull(dataReader.GetOrdinal("Tipo")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Tipo"));
            oeN_Cabecera.Ubicacion = dataReader.IsDBNull(dataReader.GetOrdinal("Ubicacion")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Ubicacion"));
            oeN_Cabecera.Monto = dataReader.IsDBNull(dataReader.GetOrdinal("Monto")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Monto"));
            oeN_Cabecera.Electricidad = dataReader.IsDBNull(dataReader.GetOrdinal("TieneElectricidad")) ? 0 : dataReader.GetInt16(dataReader.GetOrdinal("TieneElectricidad"));

            return oeN_Cabecera;
        }

        /// <summary>
        /// Selects all records from the Category table.
        /// </summary>
      

        public virtual List<EN_Cabeceras> GetAllCabeceraXTienda(int IdStore)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", IdStore)
            };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_CABECERA_GETALL_TIENDA", parameters))
            {
                List<EN_Cabeceras> cabeceraList = new List<EN_Cabeceras>();
                while (dataReader.Read())
                {
                    EN_Cabeceras cabecera = MakeEN_Cabecera(dataReader);
                    cabeceraList.Add(cabecera);
                }

                return cabeceraList;
            }
        }

        public List<EN_Cabeceras> GetAll_TiendaxCabeceras(int idStore)
        {
            return (from i in db.UEP_CXC_CABECERA_GETALL_TIENDA(idStore)
                    select new EN_Cabeceras() { ID = i.ID, NumCab = i.NumCabecera , Monto = i.Monto }
                    ).ToList();
        }
        #endregion
    }
}
