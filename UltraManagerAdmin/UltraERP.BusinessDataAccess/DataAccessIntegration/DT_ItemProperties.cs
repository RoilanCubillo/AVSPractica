using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_ItemProperties : DT
    {
        public DT_ItemProperties() : base() { }

        public List<EN_ItemCustomProperty> GetCustomProperty(string propertiesAvailable)
        {
            try
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

                return list.Count > 0 ? list : GetCustomPropertyFromTable(propertiesAvailable);
            }
            catch (SqlException ex)
            {
                if (!IsMissingObjectError(ex))
                    throw;

                List<EN_ItemCustomProperty> tableProperties = GetCustomPropertyFromTable(propertiesAvailable);
                return tableProperties.Count > 0 ? tableProperties : GetCustomPropertyFromXml(propertiesAvailable);
            }
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

            return list.Count > 0 ? list : GetItemPropertiesByItemFromXml(itemID, propertiesAvailable);
        }

        public List<EN_ItemProperty> GetAllItemsProperties(string storesAvailable, string propertiesAvailable, string searchValue, int orderColumn, string orderDirection, int skip, int take)
        {
            return GetAllItemsPropertiesFromXml(searchValue, orderColumn, orderDirection, skip, take, propertiesAvailable);
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

            try
            {
                foreach (var i in db.UEP_ITEMEXT_UPDATE(properties, itemID, propsAvailable, stores, hqUserID))
                    respuesta = new Respuesta(i.STATUS.Value ? "" : i.ERROR, i.RESPUESTA, null, i.STATUS.Value);
            }
            catch (Exception ex)
            {
                respuesta = new Respuesta(ex.Message, "error_reg_actualizado", null, false);
            }

            Respuesta directResponse = Save_ItemExtProperty_Direct(properties, itemID);
            if (!directResponse.Status)
                return respuesta.Status ? respuesta : directResponse;

            if (!respuesta.Status)
                return new Respuesta(respuesta.InternalMessage, "succ_actualizado", null, true);

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

        private Respuesta Save_ItemExtProperty_Direct(string properties, int itemID)
        {
            string xml = "<ExtendedProperties>" + (properties ?? "") + "</ExtendedProperties>";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ItemID", itemID),
                new SqlParameter("@XmlData", xml)
            };

            string sql = @"
                IF EXISTS (SELECT 1 FROM dbo.ItemExt WHERE ItemId = @ItemID)
                BEGIN
                    UPDATE dbo.ItemExt
                    SET ItemCustomPropertyXmlData = @XmlData
                    WHERE ItemId = @ItemID
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.ItemExt (ItemId, XmlData, ItemCustomPropertyXmlData)
                    VALUES (
                        @ItemID,
                        '<ExtendedProperties><Property Name=""WholeNumber"" Type=""System.Boolean"">false</Property><Property Name=""DoNotAllowEditUnitOfMeasure"" Type=""System.Boolean"">false</Property></ExtendedProperties>',
                        @XmlData
                    )
                END";

            try
            {
                SqlHelper.ExecuteNonQuery(cn, CommandType.Text, sql, parameters);
                return new Respuesta("", "succ_actualizado", null, true);
            }
            catch (Exception ex)
            {
                return new Respuesta(ex.Message, "error_reg_actualizado", null, false);
            }
        }

        private List<EN_ItemCustomProperty> GetCustomPropertyFromTable(string propertiesAvailable)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string where = "";

            if (!String.IsNullOrWhiteSpace(propertiesAvailable) && propertiesAvailable != "%")
            {
                string[] values = propertiesAvailable
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !String.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToArray();

                if (values.Length > 0)
                {
                    string[] parameterNames = values.Select((x, index) => "@Property" + index).ToArray();
                    for (int i = 0; i < values.Length; i++)
                        parameters.Add(new SqlParameter(parameterNames[i], values[i]));

                    where = " WHERE CONVERT(VARCHAR(20), ID) IN (" + String.Join(",", parameterNames) + ") OR Name IN (" + String.Join(",", parameterNames) + ")";
                }
            }

            string sql = @"
                SELECT ID, Name, Type, Inactive, CONVERT(NVARCHAR(MAX), ListValue) AS ListValue
                FROM dbo.ItemCustomProperty" + where + @"
                ORDER BY Name";

            List<EN_ItemCustomProperty> list = new List<EN_ItemCustomProperty>();

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.Text, sql, parameters.ToArray()))
            {
                while (dataReader.Read())
                {
                    list.Add(new EN_ItemCustomProperty
                    {
                        ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID")),
                        Name = dataReader.IsDBNull(dataReader.GetOrdinal("Name")) ? "" : dataReader.GetString(dataReader.GetOrdinal("Name")),
                        Type = dataReader.IsDBNull(dataReader.GetOrdinal("Type")) ? 0 : Convert.ToInt32(dataReader["Type"]),
                        Inactive = !dataReader.IsDBNull(dataReader.GetOrdinal("Inactive")) && dataReader.GetBoolean(dataReader.GetOrdinal("Inactive")),
                        ListValue = dataReader.IsDBNull(dataReader.GetOrdinal("ListValue")) ? "" : Convert.ToString(dataReader["ListValue"])
                    });
                }
            }

            return list.Count > 0 ? list : GetCustomPropertyFromXml(propertiesAvailable);
        }

        private List<EN_ItemCustomProperty> GetCustomPropertyFromXml(string propertiesAvailable)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string filter = BuildPropertiesFilter(propertiesAvailable, "PropertyName", parameters);

            string sql = @"
                SELECT PropertyName, PropertyType
                FROM (
                    SELECT
                        P.N.value('@Name', 'NVARCHAR(255)') AS PropertyName,
                        P.N.value('@Type', 'NVARCHAR(255)') AS PropertyType
                    FROM dbo.ItemExt IE
                    CROSS APPLY (SELECT TRY_CONVERT(XML, CONVERT(NVARCHAR(MAX), IE.ItemCustomPropertyXmlData)) AS XmlValue) D
                    CROSS APPLY D.XmlValue.nodes('/ExtendedProperties/Property') P(N)
                    WHERE D.XmlValue IS NOT NULL
                    UNION ALL
                    SELECT
                        P.N.value('@Name', 'NVARCHAR(255)') AS PropertyName,
                        P.N.value('@Type', 'NVARCHAR(255)') AS PropertyType
                    FROM dbo.ItemExt IE
                    CROSS APPLY (SELECT TRY_CONVERT(XML, CONVERT(NVARCHAR(MAX), IE.XmlData)) AS XmlValue) D
                    CROSS APPLY D.XmlValue.nodes('/ExtendedProperties/Property') P(N)
                    WHERE D.XmlValue IS NOT NULL
                ) Properties
                WHERE PropertyName IS NOT NULL AND PropertyName <> ''" + filter + @"
                GROUP BY PropertyName, PropertyType
                ORDER BY PropertyName";

            List<EN_ItemCustomProperty> list = new List<EN_ItemCustomProperty>();
            int id = 1;

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.Text, sql, parameters.ToArray()))
            {
                while (dataReader.Read())
                {
                    list.Add(new EN_ItemCustomProperty
                    {
                        ID = id++,
                        Name = Convert.ToString(dataReader["PropertyName"]),
                        Type = GetPropertyTypeID(Convert.ToString(dataReader["PropertyType"])),
                        Inactive = false,
                        ListValue = ""
                    });
                }
            }

            return list;
        }

        private List<EN_ItemExt> GetItemPropertiesByItemFromXml(int itemID, string propertiesAvailable)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@ItemID", itemID)
            };

            string filter = BuildPropertiesFilter(propertiesAvailable, "PropertyName", parameters);
            string sql = @"
                SELECT PropertyName, PropertyValue, PropertyType
                FROM (
                    SELECT
                        P.N.value('@Name', 'NVARCHAR(255)') AS PropertyName,
                        P.N.value('.', 'NVARCHAR(MAX)') AS PropertyValue,
                        P.N.value('@Type', 'NVARCHAR(255)') AS PropertyType,
                        1 AS Priority
                    FROM dbo.ItemExt IE
                    CROSS APPLY (SELECT TRY_CONVERT(XML, CONVERT(NVARCHAR(MAX), IE.XmlData)) AS XmlValue) D
                    CROSS APPLY D.XmlValue.nodes('/ExtendedProperties/Property') P(N)
                    WHERE IE.ItemId = @ItemID AND D.XmlValue IS NOT NULL
                    UNION ALL
                    SELECT
                        P.N.value('@Name', 'NVARCHAR(255)') AS PropertyName,
                        P.N.value('.', 'NVARCHAR(MAX)') AS PropertyValue,
                        P.N.value('@Type', 'NVARCHAR(255)') AS PropertyType,
                        2 AS Priority
                    FROM dbo.ItemExt IE
                    CROSS APPLY (SELECT TRY_CONVERT(XML, CONVERT(NVARCHAR(MAX), IE.ItemCustomPropertyXmlData)) AS XmlValue) D
                    CROSS APPLY D.XmlValue.nodes('/ExtendedProperties/Property') P(N)
                    WHERE IE.ItemId = @ItemID AND D.XmlValue IS NOT NULL
                ) Properties
                WHERE PropertyName IS NOT NULL AND PropertyName <> ''" + filter + @"
                ORDER BY PropertyName, Priority";

            List<EN_ItemExt> list = new List<EN_ItemExt>();

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.Text, sql, parameters.ToArray()))
            {
                while (dataReader.Read())
                {
                    string propertyName = Convert.ToString(dataReader["PropertyName"]);
                    EN_ItemExt current = list.FirstOrDefault(x => String.Equals(x.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase));

                    if (current == null)
                    {
                        list.Add(new EN_ItemExt
                        {
                            ItemID = itemID,
                            PropertyName = propertyName,
                            PropertyType = GetPropertyTypeID(Convert.ToString(dataReader["PropertyType"])),
                            Value = Convert.ToString(dataReader["PropertyValue"])
                        });
                    }
                    else
                    {
                        current.Value = Convert.ToString(dataReader["PropertyValue"]);
                    }
                }
            }

            return list;
        }

        private List<EN_ItemProperty> GetAllItemsPropertiesFromXml(string searchValue, int orderColumn, string orderDirection, int skip, int take, string propertiesAvailable)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@SearchValue", searchValue ?? ""),
                new SqlParameter("@SearchPrefix", (searchValue ?? "").Trim() + "%"),
                new SqlParameter("@Skip", skip),
                new SqlParameter("@Take", take <= 0 ? 10000 : take)
            };

            string where = "";
            if (!String.IsNullOrWhiteSpace(searchValue))
            {
                where = @"
                    WHERE I.ItemLookupCode LIKE @SearchPrefix
                       OR I.[Description] LIKE @SearchPrefix
                       OR CONVERT(NVARCHAR(MAX), I.ExtendedDescription) LIKE '%' + @SearchValue + '%'";
            }

            string orderColumnName;
            switch (orderColumn)
            {
                case 1: orderColumnName = "I.[Description]"; break;
                case 2: orderColumnName = "CONVERT(NVARCHAR(MAX), I.ExtendedDescription)"; break;
                default: orderColumnName = "I.ItemLookupCode"; break;
            }

            string sortDirection = String.Equals(orderDirection, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
            string sql = @"
                SELECT I.ID, I.ItemLookupCode, I.[Description], CONVERT(NVARCHAR(MAX), I.ExtendedDescription) AS ExtendedDescription,
                       CONVERT(NVARCHAR(MAX), IE.XmlData) AS XmlData,
                       CONVERT(NVARCHAR(MAX), IE.ItemCustomPropertyXmlData) AS ItemCustomPropertyXmlData
                FROM dbo.Item I
                LEFT JOIN dbo.ItemExt IE ON IE.ItemId = I.ID
                " + where + @"
                ORDER BY " + orderColumnName + " " + sortDirection + @"
                OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

            List<EN_ItemProperty> list = new List<EN_ItemProperty>();

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.Text, sql, parameters.ToArray()))
            {
                while (dataReader.Read())
                {
                    string xmlData = dataReader.IsDBNull(dataReader.GetOrdinal("XmlData")) ? "" : Convert.ToString(dataReader["XmlData"]);
                    string customXmlData = dataReader.IsDBNull(dataReader.GetOrdinal("ItemCustomPropertyXmlData")) ? "" : Convert.ToString(dataReader["ItemCustomPropertyXmlData"]);

                    list.Add(new EN_ItemProperty
                    {
                        ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID")),
                        ItemLookupCode = dataReader.IsDBNull(dataReader.GetOrdinal("ItemLookupCode")) ? "" : Convert.ToString(dataReader["ItemLookupCode"]),
                        Description = dataReader.IsDBNull(dataReader.GetOrdinal("Description")) ? "" : Convert.ToString(dataReader["Description"]),
                        ExtDescription = dataReader.IsDBNull(dataReader.GetOrdinal("ExtendedDescription")) ? "" : Convert.ToString(dataReader["ExtendedDescription"]),
                        Properties = FormatProperties(ParseXmlProperties(xmlData, customXmlData), propertiesAvailable)
                    });
                }
            }

            return list;
        }

        private static string FormatProperties(IDictionary<string, XmlPropertyValue> values, string propertiesAvailable)
        {
            IEnumerable<XmlPropertyValue> properties = values.Values;
            if (!String.IsNullOrWhiteSpace(propertiesAvailable) && propertiesAvailable != "%")
            {
                string[] allowed = propertiesAvailable.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                properties = properties.Where(x => allowed.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
            }

            return String.Join(", ", properties.OrderBy(x => x.Name).Select(x => x.Name + ": " + x.Value));
        }

        private static IDictionary<string, XmlPropertyValue> ParseXmlProperties(string xmlData, string customXmlData)
        {
            Dictionary<string, XmlPropertyValue> values = new Dictionary<string, XmlPropertyValue>(StringComparer.OrdinalIgnoreCase);
            AddXmlProperties(values, xmlData);
            AddXmlProperties(values, customXmlData);
            return values;
        }

        private static void AddXmlProperties(IDictionary<string, XmlPropertyValue> values, string xml)
        {
            if (String.IsNullOrWhiteSpace(xml))
                return;

            try
            {
                XDocument document = XDocument.Parse(xml);
                foreach (XElement node in document.Descendants("Property"))
                {
                    string name = node.Attribute("Name") == null ? "" : node.Attribute("Name").Value;
                    if (String.IsNullOrWhiteSpace(name))
                        continue;

                    values[name] = new XmlPropertyValue
                    {
                        Name = name,
                        TypeName = node.Attribute("Type") == null ? "" : node.Attribute("Type").Value,
                        Value = Convert.ToString(node.Value)
                    };
                }
            }
            catch
            {
            }
        }

        private static string BuildPropertiesFilter(string propertiesAvailable, string columnName, IList<SqlParameter> parameters)
        {
            if (String.IsNullOrWhiteSpace(propertiesAvailable) || propertiesAvailable == "%")
                return "";

            string[] values = propertiesAvailable
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();

            if (values.Length == 0)
                return "";

            string[] parameterNames = values.Select((x, index) => "@AllowedProperty" + parameters.Count + "_" + index).ToArray();
            for (int i = 0; i < values.Length; i++)
                parameters.Add(new SqlParameter(parameterNames[i], values[i]));

            return " AND " + columnName + " IN (" + String.Join(",", parameterNames) + ")";
        }

        private static int GetPropertyTypeID(string typeName)
        {
            switch ((typeName ?? "").Trim())
            {
                case "System.DateTime": return 1;
                case "System.Decimal": return 2;
                case "System.Boolean": return 4;
                case "System.Collections.ArrayList": return 5;
                default: return 0;
            }
        }

        private static bool IsMissingObjectError(SqlException ex)
        {
            return ex.Errors.Cast<SqlError>().Any(x => x.Number == 208 || x.Number == 2812);
        }

        private class XmlPropertyValue
        {
            public string Name { get; set; }
            public string TypeName { get; set; }
            public string Value { get; set; }
        }
    }
}
