using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Supplier : DT
    {

        #region Variables
        protected EN_Supplier oEN_Supplier = new EN_Supplier();
        #endregion

        #region Constructors

        public DT_Supplier() : base() { }

        #endregion

        #region Methods



        /// <summary>
        /// Seleccionar una fila desde store table.
        /// </summary>
        public virtual EN_Supplier Get(int iD, string stores_ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", iD),
                new SqlParameter("@Stores_ID", stores_ID)
            };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_SUPPLIER_GET", parameters))
            {
                if (dataReader.Read())
                {
                    return MakeEN_SUPPLIER(dataReader);
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
        public virtual List<EN_Supplier> GetAll(string stores_ID, string busqueda = "", int estado = 0, int cantidad = 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ValorBusqueda", busqueda),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@ResultCount", cantidad),
                new SqlParameter("@Stores_ID", stores_ID),
            };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_SUPPLIER_GETALL", parameters))
            {
                List<EN_Supplier> supplierList = new List<EN_Supplier>();
                while (dataReader.Read())
                {
                    EN_Supplier supplier = MakeEN_SUPPLIER(dataReader);
                    supplierList.Add(supplier);
                }

                return supplierList;
            }
        }

        /// <summary>
        /// Creates a new instance of the store class and populates it with data from the specified SqlDataReader.
        /// </summary>
        protected virtual EN_Supplier MakeEN_SUPPLIER(SqlDataReader dataReader)
        {
           EN_Supplier oeN_Supplier = new EN_Supplier();

            oeN_Supplier.ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
            oeN_Supplier.Name = dataReader.IsDBNull(dataReader.GetOrdinal("SupplierName")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SupplierName"));
            oeN_Supplier.Code = dataReader.IsDBNull(dataReader.GetOrdinal("Code")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Code"));

            return oeN_Supplier;
        }

        #endregion
    }
}
