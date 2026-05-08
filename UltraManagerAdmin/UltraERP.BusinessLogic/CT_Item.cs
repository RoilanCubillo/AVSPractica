using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_Item
    {
        #region Variables
        EN_Item oEN_Item = new EN_Item();
        DT_Item oDT_Item = new DT_Item();
        #endregion

        #region Constructors
        public CT_Item() { }
		#endregion

		#region Methods
		public Dictionary<string, object> Save(EN_Item item, string title, string userID)
        {
			return oDT_Item.Save(item, title, userID);
        }


		public DataSet GetAll(string valorBusqueda = "", int estado = 0, int resultCount = 0)
        {
            return oDT_Item.GetAll(valorBusqueda, estado, resultCount);
        }

		public List<EN_Item> GetAllList(string valorBusqueda = "", int estado = 0, int resultCount = 0)
		{
			return oDT_Item.GetAllList(valorBusqueda, estado, resultCount);
		}

		public List<EN_Item> GetDynamicList(string SearchValue, int OrderColumn, string OrderDirection, int Skip, int Take, int[] famsIDs, int[] depsIDs, int[] catsIDs, int[] subcatsIDs, int[] segsIDs, string storesID)
		{
			return oDT_Item.GetDynamicList(SearchValue, OrderColumn, OrderDirection, Skip, Take, famsIDs, depsIDs, catsIDs, subcatsIDs, segsIDs, storesID);
		}

		public Dictionary<string, int> GetCountRecords(string SearchValue, int[] famsIDs, int[] depsIDs, int[] catsIDs, int[] subcatsIDs, int[] segsIDs, string storesID)
		{
			return oDT_Item.GetCountRecords(SearchValue, famsIDs, depsIDs, catsIDs, subcatsIDs, segsIDs, storesID);
        }

		public List<EN_Item> GetAll_Simple(int SupplierID)
		{
			return oDT_Item.GetAll_Simple(SupplierID);
        }

		public List<EN_ItemStore> GetAll_ItemStore(int itemID, string storesID)
        {
			return oDT_Item.GetAll_ItemStore(itemID, storesID);
		}

		public EN_Item GetByItemLookupCode(string itemLookupCode, string storesID)
        {
			return oDT_Item.GetByItemLookupCode(itemLookupCode, storesID);
        }

		public List<EN_Item> GetByItemLookupCodebyLista(string itemLookupCode, string storesID)
		{
			return oDT_Item.GetByItemLookupCodeList(itemLookupCode, storesID);
		}

		public Respuesta ValidateExists(string itemLookupCode)
        {
			return oDT_Item.ValidateExists(itemLookupCode);
        }

		public List<Respuesta> ValidateItemsExistsList(string[] itemsLookupCodes)
        {
			return oDT_Item.ValidateItemsExistsList(itemsLookupCodes);
        }
		#endregion
	}
}
