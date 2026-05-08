using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Store : DT
    {

        #region Variables
        protected EN_Store oEN_Store = new EN_Store();
        #endregion

        #region Constructors

        public DT_Store() : base() { }

        #endregion

        #region Methods
        public virtual Respuesta Get(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", ID)
            };

            try
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_STORE_GET", parameters))
                {
                    if (dataReader.Read())
                    {
                        return new Respuesta("", "", MakeEN_STORE(dataReader), true);
                    }
                    return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
                }
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }

        }

        public virtual List<EN_Store> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ValorBusqueda", busqueda),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@ResultCount", cantidad),
            };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_STORE_GETALL", parameters))
            {
                List<EN_Store> storeList = new List<EN_Store>();
                while (dataReader.Read())
                {
                    EN_Store store = MakeEN_STORE(dataReader);
                    storeList.Add(store);
                }

                return storeList;
            }
        }

        public List<EN_Store> GetAll_By_StoreGroupID(int storeGroupID, string stores_ID)
        {
            List<EN_Store> list = (
                from i in db.UEP_STORE_GETALL_BY_STOREGROUP(storeGroupID, stores_ID)
                select new EN_Store() { IDS=i.ID, CodeS=i.StoreCode, NameS=i.Name }
            ).ToList();

            return list;
        }

        public List<EN_Store> GetStores_ItemStatus(int itemID, string storesAvailable)
        {
            List<EN_Store> list = (
                from i in db.UEP_STORES_ITEM_STATUS(itemID, storesAvailable)
                select new EN_Store() { IDS = i.ID, NameS = i.Name, ItemStatus = Convert.ToBoolean(i.IsActive) }
            ).ToList();

            /*
             * List<EN_Store> list = new List<EN_Store>();
             * 
             * list.Add(new EN_Store() { IDS = 15, NameS = "CEDI", ItemStatus = false });
             * list.Add(new EN_Store() { IDS = 16, NameS = "MSV", ItemStatus = true });
             */

            return list;
        }

        protected virtual EN_Store MakeEN_STORE(SqlDataReader dataReader)
        {
            EN_Store oeN_Store = new EN_Store();

            oeN_Store.IDS = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
            oeN_Store.NameS = dataReader.IsDBNull(dataReader.GetOrdinal("Name")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Name"));
            oeN_Store.CodeS = dataReader.IsDBNull(dataReader.GetOrdinal("StoreCode")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("StoreCode"));

            return oeN_Store;
        }

        #endregion
    }
}
