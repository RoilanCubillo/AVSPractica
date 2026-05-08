using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Tax : DT
    {
        #region Variables
        protected EN_Tax oEN_Tax = new EN_Tax();
        #endregion

        #region Constructors

        public DT_Tax() : base() { }

        #endregion

        #region Methods



        /// <summary>
        /// Seleccionar una fila desde store table.
        /// </summary>
        public virtual EN_Tax Get(int iD)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", iD)
            };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_TAX_GET", parameters))
            {
                if (dataReader.Read())
                {
                    return MakeEN_Tax(dataReader);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Selects all records from the supplier table.
        /// </summary>
        public virtual List<EN_Tax> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ValorBusqueda", busqueda),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@ResultCount", cantidad),
            };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_TAX_GETALL", parameters))
            {
                List<EN_Tax> taxList = new List<EN_Tax>();
                while (dataReader.Read())
                {
                    EN_Tax tax = MakeEN_Tax(dataReader);
                    taxList.Add(tax);
                }

                return taxList;
            }
        }

        /// <summary>
        /// Creates a new instance of the store class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_Tax MakeEN_Tax(SqlDataReader dataReader)
        {
            EN_Tax oe_EN_Tax = new EN_Tax();

            oe_EN_Tax.ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
            oe_EN_Tax.Name = dataReader.IsDBNull(dataReader.GetOrdinal("Description")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Description"));
            oe_EN_Tax.Percentage = dataReader.IsDBNull(dataReader.GetOrdinal("Percentage")) ? 0 : dataReader.GetFloat(dataReader.GetOrdinal("Percentage"));

            return oe_EN_Tax;
        }

        #endregion

    }
}
