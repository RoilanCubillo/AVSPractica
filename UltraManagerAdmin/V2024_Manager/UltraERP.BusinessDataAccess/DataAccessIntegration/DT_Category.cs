using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Category : DT
    {
        #region Variables
        protected EN_Category oEN_Category = new EN_Category();

        #endregion

        #region Constructors

        public DT_Category() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Actualizar filas en Category table.
        /// </summary>
        public virtual Respuesta Save(EN_Category category)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HQID", category.HQID),
                new SqlParameter("@ID", category.ID),
                new SqlParameter("@DepartmentID", (category.DepartmentID==0)?Convert.DBNull:category.DepartmentID),
                new SqlParameter("@Name", (category.Name==String.Empty)?Convert.DBNull:category.Name),
                new SqlParameter("@Code", (category.Code==String.Empty)?Convert.DBNull:category.Code)
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CATEGORY_INSERT_UPDATE", parameters);
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
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CATEGORY_GET", parameters))
                {
                    if (dataReader.Read())
                    {
                        return new Respuesta("", "", MakeEN_Category(dataReader), true);
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
        public virtual List<EN_Category> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ValorBusqueda", busqueda),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@ResultCount", cantidad),
            };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CATEGORY_GETALL", parameters))
            {
                List<EN_Category> categoryList = new List<EN_Category>();
                while (dataReader.Read())
                {
                    EN_Category category = MakeEN_Category(dataReader);
                    categoryList.Add(category);
                }

                return categoryList;
            }
        }

        public List<EN_Category> GetAll_Simple(int SupplierID)
        {
            return (from c in db.UEP_CATEGORY_GETALL_BY_SUPPLIER(SupplierID)
                    select new EN_Category() { ID = c.ID, Name = c.Name }
                    ).ToList();
        }

        public List<EN_Category> GetAll_ByDepartmentID(int departmentID)
        {
            List<EN_Category> list = (
                from i in db.UEP_CATEGORY_GETALL_BY_DEPARTMENTID(departmentID)
                select new EN_Category() { ID = i.ID, Code = i.Code, Name = i.Name, DepartmentID = i.DepartmentID }
            ).ToList();

            return list;
        }

        /// <summary>
        /// Creates a new instance of the Category class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_Category MakeEN_Category(SqlDataReader dataReader)
        {
            EN_Category oeN_Category = new EN_Category();
            oeN_Category.HQID = dataReader.IsDBNull(dataReader.GetOrdinal("HQID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("HQID"));
            oeN_Category.ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
            oeN_Category.DepartmentID = dataReader.IsDBNull(dataReader.GetOrdinal("DepartmentID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("DepartmentID"));
            oeN_Category.Name = dataReader.IsDBNull(dataReader.GetOrdinal("Name")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Name"));
            oeN_Category.Code = dataReader.IsDBNull(dataReader.GetOrdinal("Code")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Code"));

            return oeN_Category;
        }

        #endregion
    }
}
