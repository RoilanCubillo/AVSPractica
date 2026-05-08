using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ItemProperties : DT
    {
        public DT_ItemProperties() : base() { }

        public List<EN_ItemCustomProperty> GetCustomProperty(string propertiesAvailable)
        {
            List<EN_ItemCustomProperty> list = (
                from i in db.UEP_ITEM_CUSTOMPROPERTY_GETALL(propertiesAvailable)
                select new EN_ItemCustomProperty()
                {
                    ID = i.ID,
                    Inactive = i.Inactive,
                    ListValue = i.ListValue,
                    Name = i.Name,
                    Type = i.Type
                }
            ).ToList();

            return list;
        }

        public List<EN_ItemExt> GetItemPropertiesByItem(int itemID, string storesAvailable, string propertiesAvailable)
        {
            List<EN_ItemExt> list = (
                from i in db.UEP_ITEMEXT_PROPERTY_GET_BY_ITEMID(itemID, storesAvailable, propertiesAvailable, false)
                select new EN_ItemExt()
                {
                    ID = i.ItemPropertyID ?? 0,
                    ItemID = i.ItemID ?? 0,
                    PropertyName = i.PropertyName,
                    PropertyType = Convert.ToInt32(i.PropertyType),
                    Value = i.Value
                }
            ).ToList();

            return list;
        }

        public List<EN_ItemProperty> GetAllItemsProperties(string storesAvailable, string propertiesAvailable, string searchValue, int orderColumn, string orderDirection, int skip, int take)
        {
            List<EN_ItemProperty> list = (
                from i in db.UEP_ITEMEXT_PROPERTY_GETALL(storesAvailable, propertiesAvailable, searchValue, orderColumn, orderDirection, skip, take)
                select new EN_ItemProperty()
                {
                    ID = i.ItemID,
                    ItemLookupCode = i.ItemLookupCode,
                    Description = i.Description,
                    ExtDescription = i.ExtendedDescription,
                    Properties = i.PropertyValue
                }
            ).ToList();

            return list;
        }

        public Dictionary<string, int> Get_ItemProperties_CountRecord(string storesAvailable, string searchValue)
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();

            foreach (var i in db.UEP_ITEMEXT_PROPERTY_COUNT_RECORDS(storesAvailable, searchValue))
            {
                counts.Add("TOTAL_FILTERED", i.TOTAL_FILTERED ?? 0);
                counts.Add("TOTAL_RECORDS", i.TOTAL_RECORDS ?? 0);
            }

            return counts;
        }

        public Respuesta Save_ItemExtProperty(string properties, int itemID, string propsAvailable, string stores, int hqUserID)
        {
            Respuesta respuesta = new Respuesta("", "error_reg_actualizado", null, false);

            foreach (var i in db.UEP_ITEMEXT_UPDATE(properties, itemID, propsAvailable, stores, hqUserID))
                respuesta = new Respuesta(i.STATUS.Value ? "" : i.ERROR, i.RESPUESTA, null, i.STATUS.Value);

            return respuesta;
        }

        public List<EN_ItemProperty> GetAllItemsProperties_By_List(string items, string storesAvailable, string propertiesAvailable)
        {
            List<EN_ItemProperty> list = (
                from i in db.UEP_ITEMEXT_PROPERTY_GET_BY_ARRAY(items, storesAvailable, propertiesAvailable)
                select new EN_ItemProperty()
                {
                    ID = i.ItemID ?? 0,
                    ItemLookupCode = i.ItemLookupCode,
                    Description = i.Description,
                    ExtDescription = i.ExtendedDescription,
                    Properties = i.Properties
                }
            ).ToList();

            return list;
        }
    }
}
