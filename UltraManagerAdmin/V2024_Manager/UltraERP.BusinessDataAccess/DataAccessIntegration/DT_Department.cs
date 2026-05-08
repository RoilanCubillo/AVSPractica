using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{public
     class DT_Department : DT
    {
        #region Variables
        protected EN_Department oEN_Department = new EN_Department();
        #endregion

        #region Constructors

        public DT_Department() : base() { }

        #endregion

        #region Methods

        /// <summary>
        /// Actualizar filas en Department table.
        /// </summary>
        public virtual Respuesta Save(EN_Department department)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HQID", department.HQID),
                new SqlParameter("@ID", department.ID),
                new SqlParameter("@Name", department.Name),
                new SqlParameter("@code", department.Code),
                new SqlParameter("@FamilyID", department.FamilyID)
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_DEPARTMENT_INSERT_UPDATE", parameters);
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

        /// <summary>
        /// Seleccionar una fila desde Department table.
        /// </summary>
        public virtual EN_Department Get(int iD)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", iD)
            };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_DEPARTMENT_GET", parameters))
            {
                if (dataReader.Read())
                {
                    return MakeEN_Department(dataReader);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Selects all records from the Department table.
        /// </summary>
        public virtual List<EN_Department> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            List<EN_Department> list = (
                from i in db.UEP_DEPARTMENT_GETALL(busqueda, estado==1, cantidad)
                select new EN_Department()
                {
                    ID = i.ID,
                    Code = i.code,
                    Name = i.Name,
                    FamilyID = i.FamilyID ?? 0,
                    FamilyCode = i.FamilyCode,
                    FamilyName = i.FamilyName
                }
            ).ToList();

            return list;
        }

        public List<EN_Department> GetAllByFamily(int familyID)
        {
            List<EN_Department> list = (
                from i in db.UEP_DEPARTMENT_GETALL_BY_FAMILY(familyID)
                select new EN_Department()
                {
                    ID = i.ID,
                    Code = i.Code,
                    Name = i.Name,
                    FamilyID = i.FamilyID,
                    FamilyCode = i.FamilyCode,
                    FamilyName = i.FamilyName
                }
            ).ToList();

            return list;
        }

        /// <summary>
        /// Creates a new instance of the Department class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_Department MakeEN_Department(SqlDataReader dataReader)
        {
            EN_Department oeN_Department = new EN_Department();
            oeN_Department.HQID = dataReader.IsDBNull(dataReader.GetOrdinal("HQID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("HQID"));
            oeN_Department.ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
            oeN_Department.Name = dataReader.IsDBNull(dataReader.GetOrdinal("Name")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Name"));
            oeN_Department.Code = dataReader.IsDBNull(dataReader.GetOrdinal("code")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("code"));
            oeN_Department.FamilyID = dataReader.IsDBNull(dataReader.GetOrdinal("FamilyID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("FamilyID"));
            oeN_Department.FamilyCode = dataReader.IsDBNull(dataReader.GetOrdinal("FamilyCode")) ? "" : dataReader.GetString(dataReader.GetOrdinal("FamilyCode"));
            oeN_Department.FamilyName = dataReader.IsDBNull(dataReader.GetOrdinal("FamilyName")) ? "" : dataReader.GetString(dataReader.GetOrdinal("FamilyName"));

            return oeN_Department;
        }

        #endregion
    }
}
