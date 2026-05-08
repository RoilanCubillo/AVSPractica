using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_Store
    {

        #region Variables

        EN_Store oEN_Store = new EN_Store();
        DT_Store oDT_Store = new DT_Store();

        #endregion

        #region Constructors

        public CT_Store()
        {
        }

        #endregion

        #region Methods
        public Respuesta Get(int iD)
        {
            return oDT_Store.Get(iD);
        }
        
        public List<EN_Store> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return oDT_Store.GetAll(busqueda, estado, cantidad);
        }

        public List<EN_Store> GetAll_By_StoreGroupID(int storeGroupID, string stores_ID)
        {
            return oDT_Store.GetAll_By_StoreGroupID(storeGroupID, stores_ID);
        }

        public List<EN_Store> GetStores_ItemStatus(int itemID, string storesAvailable)
        {
            return oDT_Store.GetStores_ItemStatus(itemID, storesAvailable);
        }
        #endregion
    }
}

