using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
   public  class DT_EspaciosFrios : DT
    {
        #region Variables
        #endregion

        #region Constructors

        public DT_EspaciosFrios() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Actualizar filas en espacios frios table.
        /// </summary>
        public virtual Respuesta Save(EN_EspaciosFrios espFrio)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {

                new SqlParameter("@ID",espFrio.ID),
                new SqlParameter("@Storeid", (espFrio.StoreId)),                
                new SqlParameter("@camara", (espFrio.Camara)),              
                new SqlParameter("@puerta", espFrio.Puerta),
                new SqlParameter("@numparrilas", espFrio.NumParrillas),
                 new SqlParameter("@dimension", (espFrio.Dimension)),
                new SqlParameter("@ubicacion", (espFrio.Ubicacion)),
                new SqlParameter("@monto",espFrio.Monto)



            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_FRIOS_INSERT_UPDATE", parameters);
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
        public virtual List<EN_EspaciosFrios> GetAll()
        {


            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_FRIOS_GETALL"))
            {
                List<EN_EspaciosFrios> espFrioList = new List<EN_EspaciosFrios>();
                while (dataReader.Read())
                {
                    EN_EspaciosFrios espFrio = MakeEN_ESPACIOSFRIOS(dataReader);
                    espFrioList.Add(espFrio);
                }

                return espFrioList;
            }
        }

        /// <summary>
        /// Creates a new instance of the espacios frios class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_EspaciosFrios MakeEN_ESPACIOSFRIOS(SqlDataReader dataReader)
        {
            EN_EspaciosFrios oeN_EspaciosFrios = new EN_EspaciosFrios();
            oeN_EspaciosFrios.ID = dataReader.IsDBNull(dataReader.GetOrdinal("Id")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Id"));
            oeN_EspaciosFrios.StoreId = dataReader.IsDBNull(dataReader.GetOrdinal("StoreId")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("StoreId"));
            oeN_EspaciosFrios.Camara = dataReader.IsDBNull(dataReader.GetOrdinal("Camara")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Camara"));
            oeN_EspaciosFrios.Puerta = dataReader.IsDBNull(dataReader.GetOrdinal("Puerta")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Puerta"));
            oeN_EspaciosFrios.NumParrillas = dataReader.IsDBNull(dataReader.GetOrdinal("NumParrillas")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("NumParrillas"));
            oeN_EspaciosFrios.Dimension = dataReader.IsDBNull(dataReader.GetOrdinal("Dimension")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Dimension"));
            oeN_EspaciosFrios.Ubicacion = dataReader.IsDBNull(dataReader.GetOrdinal("Ubicacion")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Ubicacion"));
            oeN_EspaciosFrios.Monto = dataReader.IsDBNull(dataReader.GetOrdinal("Monto")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Monto"));
           
           
           
            return oeN_EspaciosFrios;
        }


        public List<EN_EspaciosFrios> GetAll_EspaciosXtienda(int idStore)
        {
            return (from i in db.UEP_CXC_FRIOS_GETALL_TIENDA(idStore)
                    select new EN_EspaciosFrios() { ID = i.ID, Camara = i.Camara, Puerta = i.Puerta, NumParrillas = i.NumParrillas, Monto = i.Monto }
                    ).ToList();
        }
        #endregion
    }
}
