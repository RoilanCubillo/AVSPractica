using CentralWizard;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Item : DT
    {

        #region Constructors
        public DT_Item() : base() { }
        #endregion

        #region Methods
        public Dictionary<string, object> Save(EN_Item item, string title, string userID)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            string error = "";

            foreach (var i in db.UEP_ITEM_INSERT_UPDATE(item.ID, item.ItemLookupCode, item.Description, item.ExtendedDescription, item.SubDescription3,
                item.SubDescription4, item.SubDescription5, item.SubDescription6, item.SubDescription7, item.SubDescription8, item.SubDescription9, item.SubDescription10,
                item.FamilyID, item.DepartmentID, item.CategoryID, item.SubCategoryID, item.SegmentID, item.TaxID, item.ManufacturerID, item.BrandID, item.PurchaserID, item.SupplierID,
                item.UnitOfMeasure, item.ReplacementCost, Convert.ToDouble(item.Utility), Convert.ToDouble(item.InvoiceDiscount), Convert.ToDouble(item.CustomerDiscount), item.MSRP, item.StoresSelected, title, userID))
            {
                result.Add("RESPUESTA", i.RESPUESTA);
                result.Add("SCOPE", i.SCOPE);
                error = i.ERROR;
            }

            return result;
        }

        public DataSet GetAll(string valorBusqueda = "", int estado = 0, int resultCount = 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ValorBusqueda", valorBusqueda),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@ResultCount", resultCount)
            };

            //try { } catch (Exception e)
            SqlDataAdapter da = new SqlDataAdapter();
            da.TableMappings.Add("Table", "Item");

            SqlCommand cmd = new SqlCommand("UEP_ITEM_GETALL", cn);
            cmd.Parameters.AddRange(parameters);
            cmd.CommandType = CommandType.StoredProcedure;

            da.SelectCommand = cmd;

            DataSet ds = new DataSet("Item");

            da.Fill(ds);

            return ds;
        }

        public virtual List<EN_Item> GetAllList(string valorBusqueda = "", int estado = 0, int resultCount = 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ValorBusqueda", valorBusqueda),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@ResultCount", resultCount)
            };
            List<EN_Item> list = new List<EN_Item>();
            try
            {
                list.Add(new EN_Item { Description = "Intento get" });
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_ITEM_GETALL", parameters))
                {
                    List<EN_Item> itemList = new List<EN_Item>();
                    list.Add(new EN_Item { Description = "Antes del while" });
                    while (dataReader.Read())
                    {
                        EN_Item item = MakeEN_Item(dataReader);
                        itemList.Add(item);
                        //itemList.Add(new EN_Item { Description = dataReader.FieldCount.ToString() });
                    }
                    list.Add(new EN_Item { Description = itemList.Count.ToString() });
                    return itemList;
                }
            }
            catch (Exception e)
            {
                list.Add(new EN_Item { Description = e.ToString() });
                return list;
            }
        }

        public List<EN_Item> GetDynamicList(string SearchValue, int OrderColumn, string OrderDirection, int Skip, int Take, int[] famsIDs, int[] depsIDs, int[] catsIDs, int[] subcatsIDs, int[] segsIDs, string storesID)
        {
            string fams = "", deps = "", cats = "", subcats = "", segs = "";

            if (famsIDs != null) foreach (int i in famsIDs) fams += String.IsNullOrEmpty(fams) ? i.ToString() : "," + i.ToString();
            if (depsIDs != null) foreach (int i in depsIDs) deps += String.IsNullOrEmpty(deps) ? i.ToString() : "," + i.ToString();
            if (catsIDs != null) foreach (int i in catsIDs) cats += String.IsNullOrEmpty(cats) ? i.ToString() : "," + i.ToString();
            if (subcatsIDs != null) foreach (int i in subcatsIDs) subcats += String.IsNullOrEmpty(subcats) ? i.ToString() : "," + i.ToString();
            if (segsIDs != null) foreach (int i in segsIDs) segs += String.IsNullOrEmpty(segs) ? i.ToString() : "," + i.ToString();

            List<EN_Item> list = (
                from i in db.UEP_ITEM_GETALL_DYNAMIC_3_0(SearchValue, OrderColumn, OrderDirection, Skip, Take, fams, deps, cats, subcats, segs, storesID)
                select new EN_Item()
                {
                    BuydownPrice = i.BuydownPrice,
                    BuydownQuantity = (float)i.BuydownQuantity,
                    HQID = i.HQID,
                    LastReceived = (DateTime)(i.LastReceived == null ? new DateTime(1900, 01, 01) : i.LastReceived),
                    LastUpdated = i.LastUpdated,
                    ID = i.ID,
                    ItemLookupCode = i.ItemLookupCode,
                    Price = i.Price,
                    PriceA = i.PriceA,
                    PriceB = i.PriceB,
                    PriceC = i.PriceC,
                    SalePrice = i.SalePrice,
                    SaleStartDate = (DateTime)(i.SaleStartDate == null ? new DateTime(1900, 01, 01) : i.SaleStartDate),
                    SaleEndDate = (DateTime)(i.SaleEndDate == null ? new DateTime(1900, 01, 01) : i.SaleEndDate),
                    QuantityDiscountID = i.QuantityDiscountID,
                    ItemType = i.ItemType,
                    Cost = i.Cost,
                    Quantity = (float)i.Quantity,
                    PriceLowerBound = i.PriceLowerBound,
                    PriceUpperBound = i.PriceUpperBound,
                    LastSold = (DateTime)(i.LastSold == null ? new DateTime(1900, 01, 01) : i.LastSold),

                    Description = i.Description,
                    ExtendedDescription = i.ExtendedDescription,
                    SubDescription1 = i.SubDescription1,
                    SubDescription2 = i.SubDescription2,
                    SubDescription3 = i.SubDescription3,
                    SubDescription4 = i.SubDescription4,
                    SubDescription5 = i.SubDescription5,
                    SubDescription6 = i.SubDescription6,
                    SubDescription7 = i.SubDescription7,
                    SubDescription8 = i.SubDescription8,
                    SubDescription9 = i.SubDescription9,
                    SubDescription10 = i.SubDescription10,

                    UnitOfMeasure = i.UnitOfMeasure,
                    LastCost = i.LastCost,
                    ReplacementCost = i.ReplacementCost,
                    SaleType = i.SaleType,
                    SaleScheduleID = i.SaleScheduleID,
                    Inactive = i.Inactive,
                    LastCounted = (DateTime)(i.LastCounted == null ? new DateTime(1900, 01, 01) : i.LastCounted),
                    MSRP = i.MSRP,
                    DateCreated = i.DateCreated,
                    Utility = i.Utility,

                    FamilyID = i.FamilyID,
                    DepartmentID = i.DepartmentID,
                    CategoryID = i.CategoryID,
                    SubCategoryID = i.SubCategoryID,
                    SegmentID = i.SegmentID,
                    BrandID = i.BrandID,
                    ManufacturerID = i.ManufacturerID,
                    PurchaserID = i.PurchaserID,
                    SupplierID = i.SupplierID,

                    FamilyCode = i.FamilyCode,
                    DepartmentCode = i.DepartmentCode,
                    CategoryCode = i.CategoryCode,
                    SubCategoryCode = i.SubCategoryCode,
                    SegmentCode = i.SegmentCode,
                    PurchaserCode = i.PurchaserCode,

                    FamilyName = i.FamilyName,
                    DepartmentName = i.DepartmentName,
                    CategoryName = i.CategoryName,
                    SubCategoryName = i.SubCategoryName,
                    SegmentName = i.SegmentName,
                    BrandDescription = i.BrandDescription,
                    ManufacturerDescription = i.ManufacturerDescription,
                    PurchaserName = i.PurchaserName,

                    InvoiceDiscount = i.InvoiceDiscount,
                    CustomerDiscount = i.CustomerDiscount,

                    TaxID = i.TaxID ?? 0,
                    StoresSelected = i.StoresSelected
                }
            ).ToList();
            return list;
        }

        public List<EN_Item> GetAll_Simple(int SupplierID)
        {
            return (from i in db.UEP_ITEM_GETALL_SIMPLE(SupplierID)
                    select new EN_Item() { ID = i.ID, Description = i.Description }
                    ).ToList();
        }

        public Dictionary<string, int> GetCountRecords(string SearchValue, int[] famsIDs, int[] depsIDs, int[] catsIDs, int[] subcatsIDs, int[] segsIDs, string storesID)
        {

            Dictionary<string, int> dict = new Dictionary<string, int>();

            string fams = "", deps = "", cats = "", subcats = "", segs = "";

            if (famsIDs != null) foreach (int i in famsIDs) fams += String.IsNullOrEmpty(fams) ? i.ToString() : "," + i.ToString();
            if (depsIDs != null) foreach (int i in depsIDs) deps += String.IsNullOrEmpty(deps) ? i.ToString() : "," + i.ToString();
            if (catsIDs != null) foreach (int i in catsIDs) cats += String.IsNullOrEmpty(cats) ? i.ToString() : "," + i.ToString();
            if (subcatsIDs != null) foreach (int i in subcatsIDs) subcats += String.IsNullOrEmpty(subcats) ? i.ToString() : "," + i.ToString();
            if (segsIDs != null) foreach (int i in segsIDs) segs += String.IsNullOrEmpty(segs) ? i.ToString() : "," + i.ToString();

            foreach (var i in db.UEP_ITEM_GET_ALLRECORDS(storesID))
                dict.Add("TOTAL", (int)(i.TOTAL == null ? 0 : i.TOTAL));

            foreach (var i in db.UEP_ITEM_GET_COUNTFILTERED(SearchValue, fams, deps, cats, subcats, segs, storesID))
                dict.Add("FILTERED", (int)(i.RECORDSFILTERED == null ? 0 : i.RECORDSFILTERED));

            return dict;
        }

        public List<EN_ItemStore> GetAll_ItemStore(int itemID, string storesID)
        {
            List<EN_ItemStore> list = (
                from i in db.UEP_ITEMSTORE_GET_BY_ITEMID(itemID, storesID)
                select new EN_ItemStore()
                {
                    ItemID = i.ItemID,
                    StoreID = i.StoreID ?? 0,
                    SupplierID = i.SupplierID ?? 0,
                    StoreName = i.StoreName,
                    SupplierName = i.SupplierName,
                    Quantity = (decimal)(i.Quantity),
                    Cost = i.Cost,
                    GrossCost = Convert.ToDecimal(i.GrossCost),
                    Utility = Convert.ToDecimal(i.Utility),
                    CustomerDiscount = Convert.ToDecimal(i.CustomerDiscount),
                    InvoiceDiscount = Convert.ToDecimal(i.InvoiceDiscount),
                    LastReceived = Convert.ToDateTime(i.LastReceived.ToString()),
                }
            ).ToList();

            return list;
        }

        public EN_Item GetByItemLookupCode(string itemLookupCode, string storesID)
        {
            EN_Item item = null;

            foreach (var i in db.UEP_ITEM_GET_BY_ITEMLOOKUPCODE(itemLookupCode, storesID))
            {
                item = new EN_Item()
                {
                    ID = i.ID,
                    Description = i.Description,
                    ExtendedDescription = i.ExtendedDescription,
                    ItemLookupCode = itemLookupCode,
                    SubDescription1 = i.SubDescription1,
                    SubDescription2 = i.SubDescription2,
                    SubDescription3 = i.SubDescription3,
                    Cost = i.Cost,
                    Inactive = i.Inactive,
                    TaxID = i.TaxID,
                    SupplierID = i.SupplierID,
                    SupplierCode = i.SupplierCode,
                    SupplierName = i.SupplierName,
                    SupplierCost = i.SupplierCost,
                    SupplierCostStartDate = i.SupplierCostStartDate,
                    Price = i.Price,
                    QuantityDiscountName = i.QuantityDiscountName,
                    ReplacementCost = i.ReplacementCost,
                    MSRP = i.MSRP,
                    TaxPercentage = i.TaxPercentage,
                    Utility = i.Utility,
                    CustomerDiscount = i.CustomerDiscount
                };
            }

            return item;
        }

        public List<EN_Item> GetByItemLookupCodeList(string ListaProductos, string storesID)
        {
            EN_Item item = new EN_Item();
            List<EN_Item> Lista = new List<EN_Item>();
            try
            {
                ClsData dt = new ClsData();

                string sql = $@"if OBJECT_ID ('tempdb..#TMPListaCodigos') is not null drop table #TMPListaCodigos
                            CREATE TABLE #TMPListaCodigos (ItemCode varchar(max))
                            insert into #TMPListaCodigos (ItemCode) select value from string_split('{ListaProductos}', ',')
                            
                            select * from View_UEP_ITEM_GET_BY_ITEMLOOKUPCODE_LIST
                            where ItemLookupCode in (select ItemCode from #TMPListaCodigos)
                                {(storesID != "%" ? $" AND (SELECT COUNT(*) FROM [GlobalCatalog] GC WITH (NOLOCK) WHERE GC.SyncGuid = ItemSyncGuid AND GC.TableName = 'Item' AND GC.StoreId IN (SELECT VALUE FROM STRING_SPLIT('{storesID}', ',') WHERE VALUE <> '') AND GC.IsActive = 1) > 0" : "")}";
                //dt.SQLExecute(sql);

                var datos = dt.SQLCargaDataTable(sql, null);

                for (int i = 0; i <= datos.Rows.Count - 1; i++)
                {
                    item = new EN_Item();
                    item.ID = Convert.ToInt32(datos.Rows[i]["ID"].ToString());
                    item.Description = datos.Rows[i]["Description"].ToString();
                    item.ExtendedDescription = datos.Rows[i]["ExtendedDescription"].ToString();
                    item.ItemLookupCode = datos.Rows[i]["ItemLookupCode"].ToString();
                    item.SubDescription1 = datos.Rows[i]["SubDescription1"].ToString();
                    item.SubDescription2 = datos.Rows[i]["SubDescription2"].ToString();
                    item.SubDescription3 = datos.Rows[i]["SubDescription3"].ToString();
                    item.Cost = Convert.ToDecimal(datos.Rows[i]["Cost"].ToString());
                    item.Inactive = Convert.ToBoolean(datos.Rows[i]["Inactive"].ToString());
                    item.TaxID = Convert.ToInt32(datos.Rows[i]["TaxID"].ToString());
                    item.SupplierID = Convert.ToInt32(datos.Rows[i]["SupplierID"].ToString());
                    item.SupplierCode = datos.Rows[i]["SupplierCode"].ToString();
                    item.SupplierName = datos.Rows[i]["SupplierName"].ToString();
                    item.SupplierCost = Convert.ToDecimal(datos.Rows[i]["SupplierCost"].ToString());
                    item.SupplierCostStartDate = datos.Rows[i]["SupplierCostStartDate"].ToString();
                    item.Price = Convert.ToDecimal(datos.Rows[i]["Price"].ToString());
                    item.QuantityDiscountName = datos.Rows[i]["QuantityDiscountName"].ToString();
                    item.ReplacementCost = Convert.ToDecimal(datos.Rows[i]["ReplacementCost"].ToString());
                    item.MSRP = Convert.ToDecimal(datos.Rows[i]["MSRP"].ToString());
                    item.TaxPercentage = Convert.ToInt64(datos.Rows[i]["TaxPercentage"].ToString());
                    item.Utility = Convert.ToDecimal(datos.Rows[i]["Utility"].ToString());
                    item.CustomerDiscount = Convert.ToDecimal(datos.Rows[i]["CustomerDiscount"].ToString());
                    item.StoresSelected = datos.Rows[i]["StoresIDActive"].ToString();
                    item.StoresNameSelected = datos.Rows[i]["StoresNameActive"].ToString();
                    Lista.Add(item);
                }

                //   Lista = (
                //   from i in db.View_UEP_ITEM_GET_BY_ITEMLOOKUPCODE_LISTs.ToLookup(p=> p.ItemLookupCode. .Where(p=> p.ItemLookupCode.Contains(ListaProductos))
                //select new EN_Item()
                //  {
                //     ID = i.ID,
                //     Description = i.Description,
                //     ExtendedDescription = i.ExtendedDescription,
                //     ItemLookupCode = i.ItemLookupCode,
                //     SubDescription1 = i.SubDescription1,
                //     SubDescription2 = i.SubDescription2,
                //     SubDescription3 = i.SubDescription3,
                //     Cost = i.Cost,
                //     Inactive = i.Inactive,
                //     TaxID = i.TaxID,
                //     SupplierID = i.SupplierID,
                //     SupplierCode = i.SupplierCode,
                //     SupplierName = i.SupplierName,
                //     SupplierCost = i.SupplierCost,
                //     SupplierCostStartDate = i.SupplierCostStartDate,
                //     Price = i.Price,
                //     QuantityDiscountName = i.QuantityDiscountName,
                //     ReplacementCost = i.ReplacementCost,
                //     MSRP = i.MSRP,
                //     TaxPercentage = i.TaxPercentage,
                //     Utility = i.Utility,
                //     CustomerDiscount = i.CustomerDiscount
                //  }) .ToList();
            }
            catch (Exception)
            {
                Lista = null;
            }
            return Lista;

        }

        public Respuesta ValidateExists(string itemLookupCode)
        {
            try
            {
                Respuesta respuesta = null;

                foreach (var i in db.UEP_ITEM_VALIDATE_EXISTS_BY_ITEMLOOKUPCODE(itemLookupCode))
                {
                    respuesta = new Respuesta("",
                        i.EXISTS == true ? $"El Producto código '{itemLookupCode}' YA EXISTE EN EL SISTEMA." :
                            i.EXISTS == false ? $"No existe el producto {itemLookupCode}." :
                                $"Error en respuesta del producto {itemLookupCode}.",
                        i.EXISTS, i.EXISTS == true
                    );
                }

                return respuesta == null ? new Respuesta("ERROR CONSULTAR RESULTADO.", $"Error interno. Error al consultar el producto {itemLookupCode}", null, false) : respuesta;

            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, $"Error interno. Error al consultar el producto {itemLookupCode}", null, false);
            }
        }

        public List<Respuesta> ValidateItemsExistsList(string[] itemsLookupCodes)
        {
            try
            {
                string codes = String.Join(",", itemsLookupCodes);

                ClsData dt = new ClsData();

                string sql = $@"IF OBJECT_ID ('tempdb..#TMPListaNuevaCodigos') IS NOT NULL DROP TABLE #TMPListaNuevaCodigos
                            CREATE TABLE #TMPListaNuevaCodigos
                            (
                                ItemCode VARCHAR(50)
                            )
                            INSERT INTO #TMPListaNuevaCodigos (ItemCode) SELECT VALUE FROM STRING_SPLIT('{codes}', ',') 
                            SELECT ItemCode, CONVERT(BIT, 1) AS [EXISTS] FROM #TMPListaNuevaCodigos WHERE ItemCode IN (SELECT ItemLookupCode FROM Item)";

                DataTable datos = dt.SQLCargaDataTable(sql, null);

                List<Respuesta> respuestas = new List<Respuesta>();

                for (int i = 0; i <= datos.Rows.Count - 1; i++)
                {
                    bool result = Convert.ToBoolean(datos.Rows[i]["EXISTS"]);
                    respuestas.Add(new Respuesta("", datos.Rows[i]["ItemCode"].ToString(), result, result));
                }

                return respuestas;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual EN_Item MakeEN_Item(SqlDataReader dataReader)
        {

            EN_Item eN_Item = new EN_Item();

            eN_Item.BuydownPrice = dataReader.IsDBNull(dataReader.GetOrdinal("BuydownPrice")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("BuydownPrice"));
            eN_Item.BuydownQuantity = dataReader.IsDBNull(dataReader.GetOrdinal("BuydownQuantity")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("BuydownQuantity")));
            eN_Item.Description = dataReader.IsDBNull(dataReader.GetOrdinal("Description")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Description"));
            eN_Item.HQID = dataReader.IsDBNull(dataReader.GetOrdinal("HQID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("HQID"));
            eN_Item.LastReceived = dataReader.IsDBNull(dataReader.GetOrdinal("LastReceived")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("LastReceived"));
            eN_Item.LastUpdated = dataReader.IsDBNull(dataReader.GetOrdinal("LastUpdated")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("LastUpdated"));
            eN_Item.ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
            eN_Item.ItemLookupCode = dataReader.IsDBNull(dataReader.GetOrdinal("ItemLookupCode")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("ItemLookupCode"));
            eN_Item.DepartmentID = dataReader.IsDBNull(dataReader.GetOrdinal("DepartmentID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("DepartmentID"));
            eN_Item.CategoryID = dataReader.IsDBNull(dataReader.GetOrdinal("CategoryID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("CategoryID"));
            eN_Item.Price = dataReader.IsDBNull(dataReader.GetOrdinal("Price")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price"));
            eN_Item.PriceA = dataReader.IsDBNull(dataReader.GetOrdinal("PriceA")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("PriceA"));
            eN_Item.PriceB = dataReader.IsDBNull(dataReader.GetOrdinal("PriceB")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("PriceB"));
            eN_Item.PriceC = dataReader.IsDBNull(dataReader.GetOrdinal("PriceC")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("PriceC"));
            eN_Item.SalePrice = dataReader.IsDBNull(dataReader.GetOrdinal("SalePrice")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("SalePrice"));
            eN_Item.SaleStartDate = dataReader.IsDBNull(dataReader.GetOrdinal("SaleStartDate")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("SaleStartDate"));
            eN_Item.SaleEndDate = dataReader.IsDBNull(dataReader.GetOrdinal("SaleEndDate")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("SaleEndDate"));
            eN_Item.QuantityDiscountID = dataReader.IsDBNull(dataReader.GetOrdinal("QuantityDiscountID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("QuantityDiscountID"));
            eN_Item.ItemType = dataReader.IsDBNull(dataReader.GetOrdinal("ItemType")) ? (short)0 : dataReader.GetInt16(dataReader.GetOrdinal("ItemType"));
            eN_Item.Cost = dataReader.IsDBNull(dataReader.GetOrdinal("Cost")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Cost"));
            eN_Item.Quantity = dataReader.IsDBNull(dataReader.GetOrdinal("Quantity")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("Quantity")));
            eN_Item.PriceLowerBound = dataReader.IsDBNull(dataReader.GetOrdinal("PriceLowerBound")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("PriceLowerBound"));
            eN_Item.PriceUpperBound = dataReader.IsDBNull(dataReader.GetOrdinal("PriceUpperBound")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("PriceUpperBound"));
            eN_Item.LastSold = dataReader.IsDBNull(dataReader.GetOrdinal("LastSold")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("LastSold"));
            eN_Item.ExtendedDescription = dataReader.IsDBNull(dataReader.GetOrdinal("ExtendedDescription")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("ExtendedDescription"));
            eN_Item.SubDescription1 = dataReader.IsDBNull(dataReader.GetOrdinal("SubDescription1")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SubDescription1"));
            eN_Item.SubDescription2 = dataReader.IsDBNull(dataReader.GetOrdinal("SubDescription2")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SubDescription2"));
            eN_Item.SubDescription3 = dataReader.IsDBNull(dataReader.GetOrdinal("SubDescription3")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SubDescription3"));
            eN_Item.UnitOfMeasure = dataReader.IsDBNull(dataReader.GetOrdinal("UnitOfMeasure")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("UnitOfMeasure"));
            eN_Item.LastCost = dataReader.IsDBNull(dataReader.GetOrdinal("LastCost")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("LastCost"));
            eN_Item.ReplacementCost = dataReader.IsDBNull(dataReader.GetOrdinal("ReplacementCost")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("ReplacementCost"));
            eN_Item.SaleType = dataReader.IsDBNull(dataReader.GetOrdinal("SaleType")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SaleType"));
            eN_Item.SaleScheduleID = dataReader.IsDBNull(dataReader.GetOrdinal("SaleScheduleID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SaleScheduleID"));
            eN_Item.Inactive = dataReader.IsDBNull(dataReader.GetOrdinal("Inactive")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("Inactive"));
            eN_Item.LastCounted = dataReader.IsDBNull(dataReader.GetOrdinal("LastCounted")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("LastCounted"));
            eN_Item.MSRP = dataReader.IsDBNull(dataReader.GetOrdinal("MSRP")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("MSRP"));
            eN_Item.DateCreated = dataReader.IsDBNull(dataReader.GetOrdinal("DateCreated")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("DateCreated"));
            eN_Item.FamilyID = dataReader.IsDBNull(dataReader.GetOrdinal("FamilyID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("FamilyID"));
            eN_Item.SubCategoryID = dataReader.IsDBNull(dataReader.GetOrdinal("SubCategoryID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SubCategoryID"));
            eN_Item.SubDescription4 = dataReader.IsDBNull(dataReader.GetOrdinal("SubDescription4")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SubDescription4"));
            eN_Item.SubDescription5 = dataReader.IsDBNull(dataReader.GetOrdinal("SubDescription5")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SubDescription5"));
            eN_Item.SubDescription6 = dataReader.IsDBNull(dataReader.GetOrdinal("SubDescription6")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SubDescription6"));
            eN_Item.SubDescription7 = dataReader.IsDBNull(dataReader.GetOrdinal("SubDescription7")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SubDescription7"));
            eN_Item.SubDescription8 = dataReader.IsDBNull(dataReader.GetOrdinal("SubDescription8")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SubDescription8"));
            eN_Item.SubDescription9 = dataReader.IsDBNull(dataReader.GetOrdinal("SubDescription9")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SubDescription9"));
            eN_Item.SubDescription10 = dataReader.IsDBNull(dataReader.GetOrdinal("SubDescription10")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("SubDescription10"));
            /*
            BinLocation = dataReader.IsDBNull(dataReader.GetOrdinal("BinLocation")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("BinLocation")),
            TaxID = dataReader.IsDBNull(dataReader.GetOrdinal("TaxID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("TaxID")),
            MessageID = dataReader.IsDBNull(dataReader.GetOrdinal("MessageID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("MessageID")),
            ItemNotDiscountable = dataReader.IsDBNull(dataReader.GetOrdinal("ItemNotDiscountable")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("ItemNotDiscountable")),
            FoodStampable = dataReader.IsDBNull(dataReader.GetOrdinal("FoodStampable")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("FoodStampable")),
            CommissionAmount = dataReader.IsDBNull(dataReader.GetOrdinal("CommissionAmount")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("CommissionAmount")),
            CommissionMaximum = dataReader.IsDBNull(dataReader.GetOrdinal("CommissionMaximum")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("CommissionMaximum")),
            CommissionMode = dataReader.IsDBNull(dataReader.GetOrdinal("CommissionMode")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("CommissionMode")),
            CommissionPercentProfit = dataReader.IsDBNull(dataReader.GetOrdinal("CommissionPercentProfit")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("CommissionPercentProfit")),
            CommissionPercentSale = dataReader.IsDBNull(dataReader.GetOrdinal("CommissionPercentSale")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("CommissionPercentSale")),
            Notes = dataReader.IsDBNull(dataReader.GetOrdinal("Notes")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Notes")),
            QuantityCommitted = dataReader.IsDBNull(dataReader.GetOrdinal("QuantityCommitted")) ? 0.0F : dataReader.GetDouble(dataReader.GetOrdinal("QuantityCommitted")),
            SerialNumberCount = dataReader.IsDBNull(dataReader.GetOrdinal("SerialNumberCount")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SerialNumberCount")),
            TareWeightPercent = dataReader.IsDBNull(dataReader.GetOrdinal("TareWeightPercent")) ? 0.0F : dataReader.GetDouble(dataReader.GetOrdinal("TareWeightPercent")),
            ReorderPoint = dataReader.IsDBNull(dataReader.GetOrdinal("ReorderPoint")) ? 0.0F : dataReader.GetDouble(dataReader.GetOrdinal("ReorderPoint")),
            RestockLevel = dataReader.IsDBNull(dataReader.GetOrdinal("RestockLevel")) ? 0.0F : dataReader.GetDouble(dataReader.GetOrdinal("RestockLevel")),
            TareWeight = dataReader.IsDBNull(dataReader.GetOrdinal("TareWeight")) ? 0.0F : dataReader.GetDouble(dataReader.GetOrdinal("TareWeight")),
            SupplierID = dataReader.IsDBNull(dataReader.GetOrdinal("SupplierID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SupplierID")),
            TagAlongItem = dataReader.IsDBNull(dataReader.GetOrdinal("TagAlongItem")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("TagAlongItem")),
            TagAlongQuantity = dataReader.IsDBNull(dataReader.GetOrdinal("TagAlongQuantity")) ? 0.0F : dataReader.GetDouble(dataReader.GetOrdinal("TagAlongQuantity")),
            ParentItem = dataReader.IsDBNull(dataReader.GetOrdinal("ParentItem")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ParentItem")),
            ParentQuantity = dataReader.IsDBNull(dataReader.GetOrdinal("ParentQuantity")) ? 0.0F : dataReader.GetDouble(dataReader.GetOrdinal("ParentQuantity")),
            BarcodeFormat = dataReader.IsDBNull(dataReader.GetOrdinal("BarcodeFormat")) ? (short)0 : dataReader.GetInt16(dataReader.GetOrdinal("BarcodeFormat")),
            PictureName = dataReader.IsDBNull(dataReader.GetOrdinal("PictureName")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("PictureName")),
            QuantityEntryNotAllowed = dataReader.IsDBNull(dataReader.GetOrdinal("QuantityEntryNotAllowed")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("QuantityEntryNotAllowed")),
            PriceMustBeEntered = dataReader.IsDBNull(dataReader.GetOrdinal("PriceMustBeEntered")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("PriceMustBeEntered")),
            BlockSalesReason = dataReader.IsDBNull(dataReader.GetOrdinal("BlockSalesReason")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("BlockSalesReason")),
            BlockSalesAfterDate = dataReader.IsDBNull(dataReader.GetOrdinal("BlockSalesAfterDate")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("BlockSalesAfterDate")),
            Weight = dataReader.IsDBNull(dataReader.GetOrdinal("Weight")) ? 0.0F : dataReader.GetDouble(dataReader.GetOrdinal("Weight")),
            Taxable = dataReader.IsDBNull(dataReader.GetOrdinal("Taxable")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("Taxable")),
            BlockSalesBeforeDate = dataReader.IsDBNull(dataReader.GetOrdinal("BlockSalesBeforeDate")) ? new DateTime(1900, 01, 01) : dataReader.GetDateTime(dataReader.GetOrdinal("BlockSalesBeforeDate")),
            WebItem = dataReader.IsDBNull(dataReader.GetOrdinal("WebItem")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("WebItem")),
            BlockSalesType = dataReader.IsDBNull(dataReader.GetOrdinal("BlockSalesType")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("BlockSalesType")),
            BlockSalesScheduleID = dataReader.IsDBNull(dataReader.GetOrdinal("BlockSalesScheduleID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("BlockSalesScheduleID")),
            Consignment = dataReader.IsDBNull(dataReader.GetOrdinal("Consignment")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("Consignment")),
            DoNotOrder = dataReader.IsDBNull(dataReader.GetOrdinal("DoNotOrder")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("DoNotOrder")),
            Content = dataReader.IsDBNull(dataReader.GetOrdinal("Content")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Content")),
            UsuallyShip = dataReader.IsDBNull(dataReader.GetOrdinal("UsuallyShip")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("UsuallyShip")),
            NumberFormat = dataReader.IsDBNull(dataReader.GetOrdinal("NumberFormat")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("NumberFormat")),
            ItemCannotBeRet = dataReader.IsDBNull(dataReader.GetOrdinal("ItemCannotBeRet")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("ItemCannotBeRet")),
            ItemCannotBeSold = dataReader.IsDBNull(dataReader.GetOrdinal("ItemCannotBeSold")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("ItemCannotBeSold")),
            IsAutogenerated = dataReader.IsDBNull(dataReader.GetOrdinal("IsAutogenerated")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("IsAutogenerated")),
            IsGlobalvoucher = dataReader.IsDBNull(dataReader.GetOrdinal("IsGlobalvoucher")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("IsGlobalvoucher")),
            DeleteZeroBalanceEntry = dataReader.IsDBNull(dataReader.GetOrdinal("DeleteZeroBalanceEntry")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("DeleteZeroBalanceEntry")),
            TenderID = dataReader.IsDBNull(dataReader.GetOrdinal("TenderID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("TenderID"))
            */

            return eN_Item;

        }
        #endregion
    }
}
