using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_ItemProperties
    {
        public List<EN_ItemCustomProperty> GetCustomProperty(string propertiesAvailable)
        {
            return new DT_ItemProperties().GetCustomProperty(propertiesAvailable);
        }

        public List<EN_ItemExt> GetItemPropertiesByItem(int itemID, string storesAvailable, string propertiesAvailable)
        {
            return new DT_ItemProperties().GetItemPropertiesByItem(itemID, storesAvailable, propertiesAvailable);
        }

        public List<EN_ItemProperty> GetAllItemsProperties(string storesAvailable, string propertiesAvailable, string searchValue, int orderColumn, string orderDirection, int skip, int take)
        {
            return new DT_ItemProperties().GetAllItemsProperties(storesAvailable, propertiesAvailable, searchValue, orderColumn, orderDirection, skip, take);
        }

        public Dictionary<string, int> Get_ItemProperties_CountRecord(string storesAvailable, string searchValue)
        {
            return new DT_ItemProperties().Get_ItemProperties_CountRecord(storesAvailable, searchValue);
        }

        public Respuesta Save_ItemExtProperty(string properties, int itemID, string propsAvailable, string stores, int hqUserID)
        {
            return new DT_ItemProperties().Save_ItemExtProperty(properties, itemID, propsAvailable, stores, hqUserID);
        }

        public List<EN_ItemProperty> GetAllItemsProperties_By_List(string items, string storesAvailable, string propertiesAvailable)
        {
            return new DT_ItemProperties().GetAllItemsProperties_By_List(items, storesAvailable, propertiesAvailable);
        }
    }
}
