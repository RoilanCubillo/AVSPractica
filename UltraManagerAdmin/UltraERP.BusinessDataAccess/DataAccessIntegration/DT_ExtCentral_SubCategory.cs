using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ExtCentral_SubCategory : DT
    {

        #region Variables
        protected EN_ExtCentral_SubCategory oEN_ExtCentral_SubCategory = new EN_ExtCentral_SubCategory();
        #endregion

        #region Constructores
        public DT_ExtCentral_SubCategory() : base() { }
        #endregion

        #region Métodos
        public virtual Respuesta Save(EN_ExtCentral_SubCategory subCategory)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", subCategory.ID),
                new SqlParameter("@Description", (subCategory.Description==String.Empty)?Convert.DBNull:subCategory.Description),
                new SqlParameter("@Code", (subCategory.Code==String.Empty)?Convert.DBNull:subCategory.Code),
                new SqlParameter("@CategoryID", subCategory.CategoryID)
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_EXTCENTRAL_SUBCATEGORY_INSERT_UPDATE", parameters);
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

        public virtual Respuesta Get(int ID)
        {
            try
            {
                EN_ExtCentral_SubCategory subCategory = GetAll("", 0, 0).FirstOrDefault(x => x.ID == ID);
                if (subCategory == null)
                    return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);

                return new Respuesta("", "", subCategory, true);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }
        }

        public virtual List<EN_ExtCentral_SubCategory> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ValorBusqueda", busqueda),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@ResultCount", cantidad),
            };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_EXTCENTRAL_SUBCATEGORY_GETALL", parameters))
            {
                List<EN_ExtCentral_SubCategory> list = new List<EN_ExtCentral_SubCategory>();
                while (dataReader.Read())
                {
                    EN_ExtCentral_SubCategory elem = MakeEN_ExtCentral_SubCategory(dataReader);
                    list.Add(elem);
                }

                return list;
            }
        }

        public List<EN_ExtCentral_SubCategory> GetAll_By_CategoryID(int categoryID)
        {
            List<EN_ExtCentral_SubCategory> list = (
                from i in db.UEP_EXTCENTRAL_SUBCATEGORY_GETALL_BY_CATEGORYID(categoryID)
                select new EN_ExtCentral_SubCategory() { ID = i.ID, CategoryID = i.CategoryID, Code = i.Code, Description = i.Description }
            ).ToList();

            return list;
        }

        protected virtual EN_ExtCentral_SubCategory MakeEN_ExtCentral_SubCategory(SqlDataReader dataReader)
        {
            return new EN_ExtCentral_SubCategory
            {
                ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID")),
                Description = dataReader.IsDBNull(dataReader.GetOrdinal("Description")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Description")),
                Code = dataReader.IsDBNull(dataReader.GetOrdinal("Code")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Code")),
                CategoryID = dataReader.IsDBNull(dataReader.GetOrdinal("CategoryID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("CategoryID"))
            };
        }
        #endregion
    }
}
