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
    public class DT_ExtCentral_Family : DT
    {

        #region Variables
        protected EN_ExtCentral_Family oEN_ExtCentral_Family = new EN_ExtCentral_Family();
        #endregion

        #region Constructores
        public DT_ExtCentral_Family() : base() { }
        #endregion

        #region Metodos
        public virtual Respuesta Save(EN_ExtCentral_Family family)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", family.ID),
                new SqlParameter("@Name", (family.Name==String.Empty)?Convert.DBNull:family.Name),
                new SqlParameter("@Code", (family.Code==String.Empty)?Convert.DBNull:family.Code)
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_EXTCENTRAL_FAMILY_INSERT_UPDATE", parameters);
                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                    int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));

                    if (String.IsNullOrEmpty(result)) return new Respuesta("RESULTADO: VACIO", "error_get_proc", scope, false);
                    else if (result.Contains("error")) return new Respuesta("ERROR AL ACCIONAR EL PROCEDIMIENTO", result, scope, false);
                    return new Respuesta("", result, scope, true);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }
        }


        public virtual Respuesta Get(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ID", ID) };

            try
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_EXTCENTRAL_FAMILY_GET", parameters))
                {
                    if (dataReader.Read())
                    {
                        return new Respuesta("", "", MakeEN_ExtCentral_Family(dataReader), true);
                    }
                    return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
                }
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }
        }

        public virtual List<EN_ExtCentral_Family> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ValorBusqueda", busqueda),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@ResultCount", cantidad),
            };


            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_EXTCENTRAL_FAMILY_GETALL", parameters))
            {
                List<EN_ExtCentral_Family> list = new List<EN_ExtCentral_Family>();
                while (dataReader.Read())
                {
                    EN_ExtCentral_Family category = MakeEN_ExtCentral_Family(dataReader);
                    list.Add(category);
                }

                return list;
            }
        }

        protected virtual EN_ExtCentral_Family MakeEN_ExtCentral_Family(SqlDataReader dataReader)
        {
            return new EN_ExtCentral_Family
            {
                ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID")),
                Name = dataReader.IsDBNull(dataReader.GetOrdinal("Name")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Name")),
                Code = dataReader.IsDBNull(dataReader.GetOrdinal("Code")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Code"))
            };
        }
        #endregion
    }
}
