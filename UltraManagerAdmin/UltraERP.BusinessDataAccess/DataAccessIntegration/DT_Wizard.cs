using CentralWizard;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Wizard : DT
    {
        private ClsData data = new ClsData();
        public DT_Wizard() : base() { }

        #region Generar DataTables
        private DataTable getDataTableItemsCost(List<EN_WizardStructs.TablaCost> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("Description");
            dt.Columns.Add("MontoAnterior");
            dt.Columns.Add("NuevoMonto");

            foreach (EN_WizardStructs.TablaCost i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["Description"] = i.Description;
                row["MontoAnterior"] = i.MontoAnterior;
                row["NuevoMonto"] = i.NuevoMonto;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable getDataTableItemTax(List<EN_WizardStructs.TablaItemTax> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("Description");
            dt.Columns.Add("TaxPerAnterior");
            dt.Columns.Add("TaxPercentage");

            foreach (EN_WizardStructs.TablaItemTax i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["Description"] = i.Description;
                row["TaxPerAnterior"] = i.TaxPerAnterior;
                row["TaxPercentage"] = i.TaxPercentage;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable getDataTableItemEstado(List<EN_WizardStructs.TablaEstado> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("Description");
            dt.Columns.Add("AnteriorEstado");
            dt.Columns.Add("NuevoEstado");

            foreach (EN_WizardStructs.TablaEstado i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["Description"] = i.Description;
                row["AnteriorEstado"] = i.AnteriorEstado;
                row["NuevoEstado"] = i.NuevoEstado;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable getDataTableItemDes(List<EN_WizardStructs.TablaItemDes> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("Description");
            dt.Columns.Add("DesAnterior");
            dt.Columns.Add("NuevaDes");

            foreach (EN_WizardStructs.TablaItemDes i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["Description"] = i.Description;
                row["DesAnterior"] = i.DesAnterior;
                row["NuevaDes"] = i.NuevaDes;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable getDataTableItemsSubDesc1(List<EN_WizardStructs.TablaItemSub1> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("ExtendedDescription");

            foreach (EN_WizardStructs.TablaItemSub1 i in items)
            {
                DataRow row = dt.NewRow();

                row["ItemLookupCode"] = i.ItemLookupCode;
                row["ExtendedDescription"] = i.SubDescription1;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable getDataTableItemsSubDesc2(List<EN_WizardStructs.TablaItemSub2> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("Description");
            dt.Columns.Add("OldSubDescription2");
            dt.Columns.Add("SubDescription2");

            foreach (EN_WizardStructs.TablaItemSub2 i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["Description"] = i.Description;
                row["OldSubDescription2"] = i.OldSubDescription2;
                row["SubDescription2"] = i.SubDescription2;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable getDataTableItemsSubDesc3(List<EN_WizardStructs.TablaItemSub3> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("Description");
            dt.Columns.Add("OldCabys");
            dt.Columns.Add("SubDescription3");

            foreach (EN_WizardStructs.TablaItemSub3 i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["Description"] = i.Description;
                row["OldCabys"] = i.OldCabys;
                row["SubDescription3"] = i.SubDescription3;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable getDataTableItemProveedor(List<EN_WizardStructs.TablaItemProveedor> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("SupplierCode");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("Cost");
            dt.Columns.Add("Utility");
            dt.Columns.Add("InvoiceDiscount");
            dt.Columns.Add("CustomerDiscount");
            dt.Columns.Add("StartDate");

            foreach (EN_WizardStructs.TablaItemProveedor i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["SupplierCode"] = i.SupplierCode;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["Cost"] = i.Cost;
                row["Utility"] = i.Utility;
                row["InvoiceDiscount"] = i.InvoiceDiscount;
                row["CustomerDiscount"] = i.CustomerDiscount;
                row["StartDate"] = i.StartDate;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable GetDataTableItemPriceDynamic(List<EN_WizardStructs.TablaItemPriceDynamic> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("SupplierCode");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("InvoiceDiscount");
            dt.Columns.Add("CustomerDiscount");
            dt.Columns.Add("StartDate");
            dt.Columns.Add("EndDate");
            dt.Columns.Add("SalePrice");
            dt.Columns.Add("Quantity");

            foreach (EN_WizardStructs.TablaItemPriceDynamic i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["SupplierCode"] = i.SupplierCode;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["InvoiceDiscount"] = i.InvoiceDiscount;
                row["CustomerDiscount"] = i.CustomerDiscount;
                row["StartDate"] = i.StartDate;
                row["EndDate"] = i.EndDate;
                row["SalePrice"] = i.SalePrice;
                row["Quantity"] = i.Quantity;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable GetDataTableItemQuantityDiscount(List<EN_WizardStructs.TablaItemQuantityDiscount> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("SupplierCode");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("InvoiceDiscount");
            dt.Columns.Add("CustomerDiscount");

            foreach (EN_WizardStructs.TablaItemQuantityDiscount i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["SupplierCode"] = i.SupplierCode;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["InvoiceDiscount"] = i.InvoiceDiscount;
                row["CustomerDiscount"] = i.CustomerDiscount;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable GetDataTableItemPriceRegular(List<EN_WizardStructs.TablaItemPriceRegular> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("InvoiceDiscount");
            dt.Columns.Add("CustomerDiscount");
            dt.Columns.Add("Cost");
            dt.Columns.Add("Utility");

            foreach (EN_WizardStructs.TablaItemPriceRegular i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["InvoiceDiscount"] = i.InvoiceDiscount;
                row["CustomerDiscount"] = i.CustomerDiscount;
                row["Cost"] = i.Cost;
                row["Utility"] = i.Utility;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable GetDataTablaMargenUtility(List<EN_WizardStructs.TablaMargenUtility> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("Utility");

            foreach (EN_WizardStructs.TablaMargenUtility i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["Utility"] = i.Utility;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable GetDataTableItem(List<EN_WizardStructs.TablaItem> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("Description");
            dt.Columns.Add("DescriptionExtended");
            dt.Columns.Add("Subdescription3");
            dt.Columns.Add("Subdescription4");
            dt.Columns.Add("Subdescription5");
            dt.Columns.Add("Subdescription6");
            dt.Columns.Add("Subdescription7");
            dt.Columns.Add("Subdescription8");
            dt.Columns.Add("Subdescription9");
            dt.Columns.Add("Subdescription10");
            dt.Columns.Add("UOM");
            dt.Columns.Add("FamilyCode");
            dt.Columns.Add("DepartmentCode");
            dt.Columns.Add("CategoryCode");
            dt.Columns.Add("SubCategoryCode");
            dt.Columns.Add("SegmentCode");
            dt.Columns.Add("ManufacturerCode");
            dt.Columns.Add("BrandCode");
            dt.Columns.Add("PurchaserCode");
            dt.Columns.Add("SupplierCode");
            dt.Columns.Add("Cost");
            dt.Columns.Add("MSRP");
            dt.Columns.Add("Utility");
            dt.Columns.Add("InvoiceDiscount");
            dt.Columns.Add("CustomerDiscount");
            dt.Columns.Add("TaxID");

            foreach (EN_WizardStructs.TablaItem i in items)
            {
                DataRow row = dt.NewRow();

                row["ItemLookupCode"] = i.ItemLookupCode;
                row["Description"] = i.Description;
                row["DescriptionExtended"] = i.DescriptionExtended;
                row["Subdescription3"] = i.Subdescription3;
                row["Subdescription4"] = i.Subdescription4;
                row["Subdescription5"] = i.Subdescription5;
                row["Subdescription6"] = i.Subdescription6;
                row["Subdescription7"] = i.Subdescription7;
                row["Subdescription8"] = i.Subdescription8;
                row["Subdescription9"] = i.Subdescription9;
                row["Subdescription10"] = i.Subdescription10;
                row["UOM"] = i.UOM;
                row["FamilyCode"] = i.FamilyCode;
                row["DepartmentCode"] = i.DepartmentCode;
                row["CategoryCode"] = i.CategoryCode;
                row["SubCategoryCode"] = i.SubCategoryCode;
                row["SegmentCode"] = i.SegmentCode;
                row["ManufacturerCode"] = i.ManufacturerCode;
                row["BrandCode"] = i.BrandCode;
                row["PurchaserCode"] = i.PurchaserCode;
                row["SupplierCode"] = i.SupplierCode;
                row["Cost"] = i.Cost;
                row["MSRP"] = i.MSRP;
                row["Utility"] = i.Utility;
                row["InvoiceDiscount"] = i.InvoiceDiscount;
                row["CustomerDiscount"] = i.CustomerDiscount;
                row["TaxID"] = i.TaxID;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable GetDataTableItemActivate(List<EN_WizardStructs.TablaItemActivate> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("SupplierID");
            dt.Columns.Add("SupplierCode");
            dt.Columns.Add("GrossCost");
            dt.Columns.Add("Utility");
            dt.Columns.Add("InvoiceDiscount");
            dt.Columns.Add("CustomerDiscount");

            foreach (EN_WizardStructs.TablaItemActivate i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["SupplierID"] = i.SupplierID;
                row["SupplierCode"] = i.SupplierCode;
                row["GrossCost"] = i.GrossCost;
                row["Utility"] = i.Utility;
                row["InvoiceDiscount"] = i.InvoiceDiscount;
                row["CustomerDiscount"] = i.CustomerDiscount;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable GetDataTableItemProperties(List<EN_WizardStructs.TablaItemProperties> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("ItemLookupCode");
            dt.Columns.Add("Properties");

            foreach (EN_WizardStructs.TablaItemProperties i in items)
            {
                DataRow row = dt.NewRow();

                row["ID"] = i.ID;
                row["ItemLookupCode"] = i.ItemLookupCode;
                row["Properties"] = i.Properties;

                dt.Rows.Add(row);
            }

            return dt;
        }
        #endregion

        #region BulkCopies
        private void BulkCopyToTablaCost(List<EN_WizardStructs.TablaCost> items)
        {
            try
            {
                string SQL01 = @"if OBJECT_ID('tempdb..##TablaCost') is not null 
                        drop table ##TablaCost;
                        CREATE TABLE ##TablaCost (ID int , ItemLookupCode nvarchar(25),[Description] nvarchar(500), MontoAnterior nvarchar(20), NuevoMonto nvarchar(20))";
                data.SQLCargaDataReader(SQL01);
                data.bulkItToTable(getDataTableItemsCost(items), "##TablaCost");
            }
            catch (Exception e) { throw e; }
        }
        private void BulkCopyToTableItemTax(List<EN_WizardStructs.TablaItemTax> items)
        {
            try
            {
                string SQL01 = @"if OBJECT_ID('tempdb..##TablaItemTax') is not null 
                        drop table ##TablaItemTax;
                        CREATE TABLE ##TablaItemTax (ID int , ItemLookupCode nvarchar(25),[Description] nvarchar(500), TaxPerAnterior nvarchar(20),  TaxPercentage nvarchar(20))";
                data.SQLCargaDataReader(SQL01);
                data.bulkItToTable(getDataTableItemTax(items), "##TablaItemTax");
            }
            catch (Exception e) { throw e; }
        }
        private void BulkCopyToTableEstado(List<EN_WizardStructs.TablaEstado> items)
        {
            try
            {
                string SQL01 = @"if OBJECT_ID('tempdb..##TablaEstado') is not null 
                        drop table ##TablaEstado;
                        CREATE TABLE ##TablaEstado (ID int , ItemLookupCode nvarchar(25),[Description] nvarchar(500), AnteriorEstado nvarchar(1), NuevoEstado nvarchar(1))";
                data.SQLCargaDataReader(SQL01);
                data.bulkItToTable(getDataTableItemEstado(items), "##TablaEstado");
            }
            catch (Exception e) { throw e; }
        }
        private void BulkCopyToTableItemDes(List<EN_WizardStructs.TablaItemDes> items)
        {
            try
            {
                string SQL01 = @"if OBJECT_ID('tempdb..##TablaItemDes') is not null 
                        drop table ##TablaItemDes;
                        CREATE TABLE ##TablaItemDes (ID int , ItemLookupCode nvarchar(25),[Description] nvarchar(500), DesAnterior nvarchar(500), NuevaDes nvarchar(500))";
                data.SQLCargaDataReader(SQL01);
                data.bulkItToTable(getDataTableItemDes(items), "##TablaItemDes");
            }
            catch (Exception e) { throw e; }
        }
        private void BulkCopyToTablaItemSub1(List<EN_WizardStructs.TablaItemSub1> items)
        {
            try
            {
                string SQL01 = @"if OBJECT_ID('tempdb..##TablaItemSub1') is not null 
                        drop table ##TablaItemSub1;
                        CREATE TABLE ##TablaItemSub1 (ItemLookupCode nvarchar(25), ExtendedDescription ntext)";
                data.SQLCargaDataReader(SQL01);
                data.bulkItToTable(getDataTableItemsSubDesc1(items), "##TablaItemSub1");
            }
            catch (Exception e) { throw e; }
        }
        private void BulkCopyToTablaItemSub2(List<EN_WizardStructs.TablaItemSub2> items)
        {
            try
            {
                string SQL01 = @"if OBJECT_ID('tempdb..##TablaItemSub2') is not null 
                        drop table ##TablaItemSub2;
                        CREATE TABLE ##TablaItemSub2 (ID int , ItemLookupCode nvarchar(25),[Description] nvarchar(500), OldSubDescription2 nvarchar(500),SubDescription2 nvarchar(500))";
                data.SQLCargaDataReader(SQL01);
                data.bulkItToTable(getDataTableItemsSubDesc2(items), "##TablaItemSub2");
            }
            catch (Exception e) { throw e; }
        }
        private void BulkCopyToTablaItemSub3(List<EN_WizardStructs.TablaItemSub3> items)
        {
            try
            {
                string SQL01 = @"if OBJECT_ID('tempdb..##TablaItemSub3') is not null 
                        drop table ##TablaItemSub3;
                        CREATE TABLE ##TablaItemSub3 (ID int , ItemLookupCode nvarchar(25),[Description] nvarchar(500), OldCabys nvarchar(500),SubDescription3 nvarchar(500))";
                data.SQLCargaDataReader(SQL01);
                data.bulkItToTable(getDataTableItemsSubDesc3(items), "##TablaItemSub3");
            }
            catch (Exception e) { throw e; }
        }
        private void BulkCopyToTableItemProveedor(List<EN_WizardStructs.TablaItemProveedor> items)
        {
            try
            {
                string SQL = @"if OBJECT_ID('tempdb..##TablaItemProveedor') is not null
                                    drop table ##TablaItemProveedor;
                                CREATE TABLE ##TablaItemProveedor (ID int, SupplierCode VARCHAR(30),
                                    ItemLookupCode nvarchar(25), Cost nvarchar(30), Utility nvarchar(30),
                                    InvoiceDiscount nvarchar(30), CustomerDiscount nvarchar(30), StartDate nvarchar(30))";
                data.SQLCargaDataReader(SQL);
                data.bulkItToTable(getDataTableItemProveedor(items), "##TablaItemProveedor");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private void BulkCopyToTableItemPriceDynamic(List<EN_WizardStructs.TablaItemPriceDynamic> items)
        {
            string SQL = @"if OBJECT_ID('tempdb..##TablaItemPriceDynamic') is not null
                                drop table ##TablaItemPriceDynamic;
                            CREATE TABLE ##TablaItemPriceDynamic (ID INT, SupplierCode VARCHAR(30),
                                ItemLookupCode VARCHAR(25), InvoiceDiscount VARCHAR(30), CustomerDiscount VARCHAR(30),
                                StartDate VARCHAR(30), EndDate VARCHAR(30), SalePrice VARCHAR(30), Quantity VARCHAR(30))";
            data.SQLCargaDataReader(SQL);
            data.bulkItToTable(GetDataTableItemPriceDynamic(items), "##TablaItemPriceDynamic");
        }
        private void BulkCopyToTableItemQuantityDiscount(List<EN_WizardStructs.TablaItemQuantityDiscount> items)
        {
            string SQL = @"if OBJECT_ID('tempdb..##TablaItemQuantityDiscount') is not null
                                drop table ##TablaItemQuantityDiscount;
                            CREATE TABLE ##TablaItemQuantityDiscount (ID INT, SupplierCode VARCHAR(30),
                                ItemLookupCode VARCHAR(25), InvoiceDiscount VARCHAR(30), CustomerDiscount VARCHAR(30))";
            data.SQLCargaDataReader(SQL);
            data.bulkItToTable(GetDataTableItemQuantityDiscount(items), "##TablaItemQuantityDiscount");
        }
        private void BulkCopyToTableItemPriceRegular(List<EN_WizardStructs.TablaItemPriceRegular> items)
        {
            string SQL = @"if OBJECT_ID('tempdb..##TablaItemPriceRegular') is not null
                                drop table ##TablaItemPriceRegular;
                            CREATE TABLE ##TablaItemPriceRegular (ID INT, ItemLookupCode VARCHAR(25), InvoiceDiscount VARCHAR(30), CustomerDiscount VARCHAR(30), Cost VARCHAR(30), Utility VARCHAR(30))";
            data.SQLCargaDataReader(SQL);
            data.bulkItToTable(GetDataTableItemPriceRegular(items), "##TablaItemPriceRegular");
        }
        private void BulkCopyToTablaMargenUtility(List<EN_WizardStructs.TablaMargenUtility> items)
        {
            string SQL = @"if OBJECT_ID('tempdb..##TablaMargenUtility') is not null
                                drop table ##TablaMargenUtility;
                            CREATE TABLE ##TablaMargenUtility (ID INT, ItemLookupCode VARCHAR(25), Utility VARCHAR(30))";
            data.SQLCargaDataReader(SQL);
            data.bulkItToTable(GetDataTablaMargenUtility(items), "##TablaMargenUtility");
        }
        private void BulkCopyToTableItem(List<EN_WizardStructs.TablaItem> items)
        {
            string SQL = @"if OBJECT_ID('tempdb..##TablaItem') is not null
                                drop table ##TablaItem;
                            CREATE TABLE ##TablaItem (ItemLookupCode VARCHAR(30), Description VARCHAR(80), DescriptionExtended NTEXT, Subdescription3 VARCHAR(30),
		                        Subdescription4 VARCHAR(30),Subdescription5 VARCHAR(38),Subdescription6 VARCHAR(30),Subdescription7 VARCHAR(30),Subdescription8 VARCHAR(30),Subdescription9 VARCHAR(30),Subdescription10 VARCHAR(30),
		                        UOM VARCHAR(30), FamilyCode VARCHAR(30), DepartmentCode VARCHAR(30), CategoryCode VARCHAR(30), SubCategoryCode VARCHAR(30), SegmentCode VARCHAR(30), ManufacturerCode INT, BrandCode INT, PurchaserCode VARCHAR(30), SupplierCode VARCHAR(30),
		                        Cost VARCHAR(30), MSRP VARCHAR(30), Utility VARCHAR(30), InvoiceDiscount VARCHAR(30), CustomerDiscount VARCHAR(30), TaxID INT)";
            data.SQLCargaDataReader(SQL);
            data.bulkItToTable(GetDataTableItem(items), "##TablaItem");
        }
        private void BulkCopyToTablaItemActivate(List<EN_WizardStructs.TablaItemActivate> items)
        {
            try
            {
                string SQL01 = @"if OBJECT_ID('tempdb..##TablaItemActivate') is not null 
                        drop table ##TablaItemActivate;
                        CREATE TABLE ##TablaItemActivate (ID int, ItemLookupCode varchar(25), SupplierID int, SupplierCode varchar(30),
                            GrossCost varchar(30), Utility varchar(30), InvoiceDiscount varchar(30), CustomerDiscount varchar(30))";
                data.SQLCargaDataReader(SQL01);
                data.bulkItToTable(GetDataTableItemActivate(items), "##TablaItemActivate");
            }
            catch (Exception e) { throw e; }
        }
        private void BulkCopyToTablaItemProperties(List<EN_WizardStructs.TablaItemProperties> items)
        {
            try
            {
                string SQL01 = @"IF OBJECT_ID('tempdb..##TablaItemProperties') IS NOT NULL DROP TABLE ##TablaItemProperties;
                                CREATE TABLE ##TablaItemProperties (ID int, ItemLookupCode varchar(25), Properties VARCHAR(MAX))";
                
                data.SQLCargaDataReader(SQL01);
                data.bulkItToTable(GetDataTableItemProperties(items), "##TablaItemProperties");
            }
            catch (Exception e) { throw e; }
        }
        #endregion

        #region Ejecución de Tareas WIZARD
        /** Cambio de costos */
        public Respuesta Task102(string title, string notes, int storeID, List<EN_WizardStructs.TablaCost> items, string userID)
        {
            try
            {
                BulkCopyToTablaCost(items);

                string SQL01 = $"EXEC AVS_WS_HEADER 261, '102', N'{notes}', N'{title}', N'{DateTime.Now.ToString("yyyy/MM/dd")}', {storeID}, N'{userID}'";
                SqlDataReader registros = data.SQLCargaDataReader(SQL01);

                string WSID = "";
                while (registros.Read()) WSID = registros["WorksheetID"].ToString();
                registros.Close();

                SQL01 = "EXEC AVS_CHANGE_ITEM_COST " + WSID + ",'" + storeID + "'";
                data.SQLCargaDataReader(SQL01);

                return new Respuesta("", "Cambios aplicados en tienda " + storeID, null, true);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error al intentar aplicar cambios para la tienda " + storeID, null, false);
            }
        }
        /** Cambio de Impuestos */
        public Respuesta Task104(string title, string notes, int storeID, List<EN_WizardStructs.TablaItemTax> items, string userID)
        {
            try
            {
                BulkCopyToTableItemTax(items);

                string SQL01 = $"EXEC AVS_WS_HEADER 320, '104', N'{notes}', N'{title}', N'{DateTime.Now.ToString("yyyy/MM/dd")}', {storeID}, N'{userID}'";
                SqlDataReader registros = data.SQLCargaDataReader(SQL01);

                string WSID = "";
                while (registros.Read()) WSID = registros["WorksheetID"].ToString();
                registros.Close();

                SQL01 = "EXEC AVS_CHANGE_ITEM_TAX " + WSID;
                data.SQLCargaDataReader(SQL01);

                return new Respuesta("", "Cambios aplicados en tienda " + storeID, null, true);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error al intentar aplicar cambios para la tienda " + storeID, null, false);
            }
        }
        /** Cambio de Estados */
        public Respuesta Task105(string title, string notes, int storeID, List<EN_WizardStructs.TablaEstado> items, string userID)
        {
            try
            {
                BulkCopyToTableEstado(items);

                string SQL01 = $"EXEC AVS_WS_HEADER 261, '105', N'{notes}', N'{title}', N'{DateTime.Now.ToString("yyyy/MM/dd")}', {storeID}, N'{userID}'";
                SqlDataReader registros = data.SQLCargaDataReader(SQL01);

                string WSID = "";
                while (registros.Read()) WSID = registros["WorksheetID"].ToString();
                registros.Close();

                SQL01 = "EXEC AVS_CHANGE_ITEM_STATUS " + WSID;
                data.SQLCargaDataReader(SQL01);

                return new Respuesta("", "Cambios aplicados en tienda " + storeID, null, true);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error al intentar aplicar cambios para la tienda " + storeID, null, false);
            }
        }
        /** Cambio de Descripción */
        public Respuesta Task107(string title, string notes, int storeID, List<EN_WizardStructs.TablaItemDes> items, string userID)
        {
            try
            {
                BulkCopyToTableItemDes(items);

                string SQL01 = $"EXEC AVS_WS_HEADER 261, '107', N'{notes}', N'{title}', N'{DateTime.Now.ToString("yyyy/MM/dd")}', {storeID}, N'{userID}'";
                SqlDataReader registros = data.SQLCargaDataReader(SQL01);

                string WSID = "";
                while (registros.Read()) WSID = registros["WorksheetID"].ToString();
                registros.Close();

                SQL01 = "EXEC AVS_CHANGE_ITEM_DESCRIPTION " + WSID;
                data.SQLCargaDataReader(SQL01);

                return new Respuesta("", "Cambios aplicados en tienda " + storeID, null, true);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error al intentar aplicar cambios para la tienda " + storeID, null, false);
            }
        }
        /** Cambio de Descripcoin Larga */
        public List<Respuesta> Task108(int[] storesID, string title, string notes, List<EN_WizardStructs.TablaItemSub1> items, string userID)
        {
            List<Respuesta> list = new List<Respuesta>();

            BulkCopyToTablaItemSub1(items);

            foreach (int i in storesID)
            {
                try
                {
                    string SQL01 = $"EXEC AVS_WS_HEADER 261, '108', N'{notes}', N'{title}', N'{DateTime.Now.ToString("yyyy/MM/dd HH:mm")}', N'{i}', N'{userID}'";
                    SqlDataReader registros = data.SQLCargaDataReader(SQL01);

                    string WSID = "";
                    while (registros.Read()) WSID = registros["WorksheetID"].ToString();
                    registros.Close();

                    SQL01 = "EXEC AVS_CHANGE_ITEM_EXTENDED_DESCRIPTION '" + WSID + "'";
                    data.SQLExecute(SQL01);

                    list.Add(new Respuesta("", $"Hoja de trabajo creada para la tienda ({i})", null, true));
                }
                catch (Exception e)
                {
                    list.Add(new Respuesta(e.Message, $"Error al intentar aplicar cambios para la tienda ({i})", null, false));
                }
            }

            return list;
        }
        /** Cambio de Subdescripción2 */
        public Respuesta Task109(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemSub2> items, string userID)
        {
            try
            {
                BulkCopyToTablaItemSub2(items);

                string SQL01 = $"EXEC AVS_WS_HEADER 261, '109', N'{notes}', N'{title}', N'{DateTime.Now.ToString("yyyy/MM/dd")}', {storeID}, N'{userID}'";
                SqlDataReader registros = data.SQLCargaDataReader(SQL01);

                string WSID = "";
                while (registros.Read()) WSID = registros["WorksheetID"].ToString();
                registros.Close();

                SQL01 = "EXEC AVS_CHANGE_ITEM_SUB_DESCRIPTION2 '" + WSID + "'";
                data.SQLExecute(SQL01);

                return new Respuesta("", "Cambios aplicados en tienda " + storeID, null, true);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error al intentar aplicar cambios para la tienda " + storeID, null, false);
            }
        }
        /** Cambio de Subdescripción3 (CabyS) */
        public Respuesta Task110(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemSub3> items, string userID)
        {
            try
            {
                BulkCopyToTablaItemSub3(items);

                string SQL01 = $"EXEC AVS_WS_HEADER 261, '110', N'{notes}', N'{title}', N'{DateTime.Now.ToString("yyyy/MM/dd")}', {storeID}, N'{userID}'";
                SqlDataReader registros = data.SQLCargaDataReader(SQL01);

                string WSID = "";
                while (registros.Read()) WSID = registros["WorksheetID"].ToString();
                registros.Close();

                SQL01 = "EXEC AVS_CHANGE_ITEM_SUB_DESCRIPTION3 '" + WSID + "'";
                data.SQLExecute(SQL01);

                return new Respuesta("", "Cambios aplicados en tienda " + storeID, null, true);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error al intentar aplicar cambios para la tienda " + storeID, null, false);
            }
        }
        /** Cambio de Proveedor - Costos */
        public Respuesta Task120(int[] storesID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemProveedor> items, string userID, int workSheetContentID)
        {
            try
            {
                string stores = String.Join(",", storesID);

                BulkCopyToTableItemProveedor(items);

                string SQL = $"EXEC UEP_WS_CHANGE_ITEM_SUPPLIER N'{stores}', N'{tittle}', N'{notes}', N'{effectiveDate.ToString("yyyy-MM-dd")}', N'{userID}', {workSheetContentID}";
                SqlDataReader sheet = data.SQLCargaDataReader(SQL);

                Respuesta respuesta = new Respuesta("Hoja de trabajo no ejecutada.", $"Cambios no aplicados para las tiendas ({stores})", null, false);
                while (sheet.Read())
                {
                    string LAST = sheet["LASTACTION"].ToString(); // Para uso de debug
                    string ERROR = sheet["ERROR"].ToString(); // Para uso de debug
                    respuesta = new Respuesta("", sheet["RESPUESTA"].ToString(), null, sheet.GetBoolean(sheet.GetOrdinal("STATUS")));
                }
                sheet.Close();

                return respuesta;
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, $"Error al intentar aplicar cambios", null, false);
            }
        }
        /** Cambio precios: Dinámicos */
        public Respuesta Task121(int storeID, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemPriceDynamic> items, string userID)
        {
            try
            {
                BulkCopyToTableItemPriceDynamic(items);

                string SQL = $"EXEC UEP_WS_CHANGE_ITEM_PRICE_DYNACIC 251, N'{notes}', N'{title}', N'{DateTime.Now.ToString("yyyy/MM/dd")}', {storeID}, N'{effectiveDate.ToString("yyyy/MM/dd")}', N'{userID}'";
                SqlDataReader sheet = data.SQLCargaDataReader(SQL);

                Respuesta respuesta = new Respuesta("Hoja de trabajo no ejecutada.", $"Cambios no aplicados para tienda {storeID}", null, false);
                while (sheet.Read())
                {
                    string LAST = sheet["LASTACTION"].ToString(); // Para uso de debug
                    string ERROR = sheet["ERROR"].ToString(); // Para uso de debug
                    respuesta = new Respuesta("", sheet["RESPUESTA"].ToString(), null, sheet.GetBoolean(sheet.GetOrdinal("STATUS")));
                }
                sheet.Close();

                return respuesta;
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error al intentar aplicar cambios para la tienda " + storeID, null, false);
            }
        }
        /** Cambio precios: Descuentos compre X lleve Y por Z porcentaje de descuento */
        public Respuesta Task122(int storeID, string title, string notes, int quantityDiscountID, DateTime startDate, DateTime endDate, List<EN_WizardStructs.TablaItemQuantityDiscount> items, string userID)
        {
            try
            {
                BulkCopyToTableItemQuantityDiscount(items);

                string SQL = $@"EXEC UEP_WS_CHANGE_ITEM_PRICE_PROMO 261, N'{title}', N'{notes}', 
                            {quantityDiscountID}, {storeID}, N'{startDate.ToString("yyyy/MM/dd")}',
                            N'{endDate.ToString("yyyy/MM/dd")}', N'{DateTime.Now.ToString("yyyy/MM/dd")}',
                            N'{userID}'";
                SqlDataReader sheet = data.SQLCargaDataReader(SQL);

                Respuesta respuesta = new Respuesta("Hoja de trabajo no ejecutada.", $"Cambios no aplicados para tienda {storeID}", null, false);
                while (sheet.Read())
                {
                    string LAST = sheet["LASTACTION"].ToString(); // Para uso de debug
                    string ERROR = sheet["ERROR"].ToString(); // Para uso de debug
                    respuesta = new Respuesta("", sheet["RESPUESTA"].ToString(), null, sheet.GetBoolean(sheet.GetOrdinal("STATUS")));
                }
                sheet.Close();

                return respuesta;
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error al intentar aplicar cambios para la tienda " + storeID, null, false);
            }
        }
        /** Cambio precios: Regular */
        public Respuesta Task123(int[] storesID, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemPriceRegular> items, string userID)
        {
            try
            {
                string stores = String.Join(",", storesID);

                BulkCopyToTableItemPriceRegular(items);

                string SQL = $@"EXEC UEP_WS_CHANGE_ITEM_PRICE_REGULAR N'{title}', N'{notes}', N'{stores}',
                            N'{effectiveDate.ToString("yyyy/MM/dd")}', N'{DateTime.Now.ToString("yyyy/MM/dd")}',
                            N'{userID}'";
                SqlDataReader sheet = data.SQLCargaDataReader(SQL);

                Respuesta respuesta = new Respuesta("Hoja de trabajo no ejecutada.", $"Cambios no aplicados para las tienda ({stores}).", null, false);
                while (sheet.Read())
                {
                    string LAST = sheet["LASTACTION"].ToString(); // Para uso de debug
                    string ERROR = sheet["ERROR"].ToString(); // Para uso de debug
                    respuesta = new Respuesta("", sheet["RESPUESTA"].ToString(), null, sheet.GetBoolean(sheet.GetOrdinal("STATUS")));
                }
                sheet.Close();

                return respuesta;
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error al intentar aplicar cambios", null, false);
            }
        }
        /** Cambio precios: Margen de Utilidad */
        public Respuesta Task124(int[] stores, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaMargenUtility> items, string userID)
        {
            string storesID = String.Join(",", stores);

            try
            {
                BulkCopyToTablaMargenUtility(items);

                string SQL = $"EXEC UEP_WS_CHANGE_ITEM_PRICE_UTILITY N'{storesID}', N'{effectiveDate.ToString("yyyy/MM/dd")}', N'{title}', N'{notes}', N'{userID}'";
                SqlDataReader sheet = data.SQLCargaDataReader(SQL);

                Respuesta respuesta = new Respuesta("Hoja de trabajo no ejecutada.", $"Cambios no aplicados para las tiendas ({storesID}).", null, false);
                while (sheet.Read())
                {
                    string LAST = sheet["LASTACTION"].ToString(); // Para uso de debug
                    string ERROR = sheet["ERROR"].ToString(); // Para uso de debug
                    string COUNT = sheet["COUNT"].ToString(); // Para uso de debug
                    respuesta = new Respuesta("", sheet["RESPUESTA"].ToString(), null, sheet.GetBoolean(sheet.GetOrdinal("STATUS")));
                }
                sheet.Close();

                return respuesta;
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, $"Error al intentar aplicar cambios para las tiendas ({storesID})", null, false);
            }
        }
        /** Importar artículos */
        public Respuesta Task130(int[] storesID, string title, string notes, DateTime effectiveDate, DateTime fromDate, List<EN_WizardStructs.TablaItem> items, string userID)
        {
            try
            {
                BulkCopyToTableItem(items);

                string SQL = $"EXEC UEP_WS_IMPORT_ITEMS '{String.Join(",", storesID)}', '{title}', '{notes}', '{userID}'";
                SqlDataReader sheet = data.SQLCargaDataReader(SQL);

                Respuesta respuesta = new Respuesta("Importar no procesado.", $"No se ha podido obtener resultado de los cambios.", null, false);
                while (sheet.Read())
                {
                    string LAST = sheet["LASTACTION"].ToString(); // Para uso de debug
                    string ERROR = sheet["ERROR"].ToString(); // Para uso de debug
                    respuesta = new Respuesta("", sheet["RESPUESTA"].ToString(), null, sheet.GetBoolean(sheet.GetOrdinal("STATUS")));
                }
                sheet.Close();

                return respuesta;
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error al intentar importar los productos", null, false);
            }
        }
        /** Crea hojas de trabajo para la descarga de productos */
        public Respuesta Wizard_Download_Items_Stores(int[] storesID, string title, string notes, DateTime effectiveDate, DateTime fromDate, string[] items, string userID, int workSheetContentID)
        {
            try
            {
                ClsData dt = new ClsData();
                Respuesta respuesta = new Respuesta("", "", null, false);

                // carga todos los códigos de productos a la tabla temporal
                string itemCodes = String.Join(",", items);
                string sql = $@"IF OBJECT_ID ('tempdb..##TMPListItemsDownload') IS NOT NULL DROP TABLE ##TMPListItemsDownload
                            CREATE TABLE ##TMPListItemsDownload ( ItemLookupCode VARCHAR(50) )
                            INSERT INTO ##TMPListItemsDownload (ItemLookupCode) SELECT VALUE FROM STRING_SPLIT('{itemCodes}', ',')";
                dt.SQLCargaDataReader(sql);

                // crea las hojas de trabajo respectivas y a cada una le asigna los productos
                string stores = String.Join(",", storesID);
                string SQLWORKSHEET = $@"EXEC UEP_WS_DOWNLOAD_ITEMS N'{stores}', N'{title}', N'{notes}', 
                                                    N'{effectiveDate.AddHours(-2).ToString("yyyy-MM-dd")}', 
                                                    N'{fromDate.AddHours(-2).ToString("yyyy-MM-dd")}',
                                                    N'{userID}',
                                                    {workSheetContentID}";
                SqlDataReader sheetWorkSheet = data.SQLCargaDataReader(SQLWORKSHEET);

                // obtener respuesta del proceso de sincronización
                while (sheetWorkSheet.Read())
                {
                    respuesta.Message = sheetWorkSheet["RESPUESTA"].ToString();
                    respuesta.Status = sheetWorkSheet.GetBoolean(sheetWorkSheet.GetOrdinal("STATUS"));
                    string LAST = sheetWorkSheet["LASTACTION"].ToString(); // Para uso de debug
                    string ERROR = sheetWorkSheet["ERROR"].ToString(); // Para uso de debug
                    string ERROR_NUMBER = sheetWorkSheet["ERROR_NUMBER"].ToString(); // Para uso de debug

                    if (!String.IsNullOrEmpty(ERROR))
                    {
                        ERROR = ERROR + "";
                    }
                }
                sheetWorkSheet.Close();

                return respuesta;
            }
            catch (Exception)
            {
                return new Respuesta("Error intentando sincronizar los productos con las tiendas selecciondas.", "Error intentando sincronizar los productos con las tiendas selecciondas.", null, false);
            }
        }
        /** Activación de productos */
        public Respuesta Task131(int[] storesID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemActivate> items, string userID, int workSheetContentID)
        {
            try
            {
                string stores = String.Join(",", storesID);

                BulkCopyToTablaItemActivate(items);

                string SQL = $"EXEC [UEP_WS_CHANGE_ITEM_ACTIVATE_STORE] N'{stores}', N'{tittle}', N'{notes}', N'{effectiveDate.ToString("yyyy-MM-dd")}', N'{userID}', {workSheetContentID}";
                SqlDataReader sheet = data.SQLCargaDataReader(SQL);

                Respuesta respuesta = new Respuesta("Hoja de trabajo no ejecutada.", $"Cambios no aplicados para las tiendas ({stores})", null, false);
                while (sheet.Read())
                {
                    string LAST = sheet["LASTACTION"].ToString(); // Para uso de debug
                    string ERROR = sheet["ERROR"].ToString(); // Para uso de debug
                    respuesta = new Respuesta("", sheet["RESPUESTA"].ToString(), null, sheet.GetBoolean(sheet.GetOrdinal("STATUS")));
                }
                sheet.Close();

                return respuesta;
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, $"Error al intentar aplicar cambios", null, false);
            }
        }
        /** Cambio de propiedades */
        public Respuesta Task201(string stores, string properties, int dataContentID, string userID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemProperties> items)
        {
            try
            {
                BulkCopyToTablaItemProperties(items);

                string SQL = $"EXEC [UEP_WS_CHANGE_ITEM_PROPERTIES] N'{stores}', N'{properties}', {dataContentID}, {userID}, N'{tittle}', N'{notes}', N'{effectiveDate.ToString("yyyy-MM-dd")}'";
                SqlDataReader sheet = data.SQLCargaDataReader(SQL);

                Respuesta respuesta = new Respuesta("Hoja de trabajo no ejecutada.", $"Cambios no aplicados para las tiendas ({stores})", null, false);
                while (sheet.Read())
                {
                    string LAST = sheet["LASTACTION"].ToString(); // Para uso de debug
                    string ERROR = sheet["ERROR"].ToString(); // Para uso de debug
                    respuesta = new Respuesta("", sheet["RESPUESTA"].ToString(), null, sheet.GetBoolean(sheet.GetOrdinal("STATUS")));
                }
                sheet.Close();

                return respuesta;
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, $"Error al intentar aplicar cambios", null, false);
            }
        }
        #endregion
    }
}
