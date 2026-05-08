using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;
using UltraERP.BusinessLogic;

namespace WebApplication1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UltraERPService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select UltraERPService.svc or UltraERPService.svc.cs at the Solution Explorer and start debugging.
    public class UltraERPService : IUltraERPService
    {
        #region Mantenimiento CXC_Proveedores
        public List<EN_CXC_Proveedor> GetAll_CXC_Proveedor()
        {
            return new CT_CXC_Proveedor().GetAll();
        }
        public List<EN_CXC_FormaRebajo> GetAll_CXC_FormaRebajos()
        {
            return new CT_CXC_Proveedor().GetAll_FormaRebajos();
        }
        public List<EN_Tipo> GetAll_CXC_IDCTipo()
        {
            return new CT_CXC_Proveedor().GetAll_IDCTipo();
        }
        public List<EN_Tipo> GetAll_CXC_IICTipo()
        {
            return new CT_CXC_Proveedor().GetAll_IICTipo();
        }
        public List<EN_Tipo> GetAll_CXC_FrecuenciasPago()
        {
            return new CT_CXC_Proveedor().GetAll_FrecuenciaPagos();
        }
        public List<EN_Tipo> GetAll_CXC_Negociaciones()
        {
            return new CT_CXC_Proveedor().GetAll_Negociaciones();
        }
        public List<EN_CXC_IIC> GetAll_CXC_IIC(int fichaID)
        {
            return new CT_CXC_Proveedor().GetAll_IIC(fichaID);
        }
        public List<EN_CXC_IDC> GetAll_CXC_IDC(int fichaID)
        {
            return new CT_CXC_Proveedor().GetAll_IDC(fichaID);
        }
        public Dictionary<string, object> Save_CXC_Proveedor(EN_CXC_Proveedor ficha)
        {
            return new CT_CXC_Proveedor().Save_CXC_Proveedor(ficha);
        }
        public Respuesta Save_CXC_IDC(EN_CXC_IDC idc, int CXC_ProveedorID)
        {
            return new DT_CXC_Proveedor().Save_IDC(idc, CXC_ProveedorID);
        }
        public Respuesta Save_CXC_IIC(EN_CXC_IIC iic, int CXC_ProveedorID)
        {
            return new DT_CXC_Proveedor().Save_IIC(iic, CXC_ProveedorID);
        }
        public Respuesta Delete_CXC_IDC(int ID)
        {
            return new DT_CXC_Proveedor().DeleteIDC(ID);
        }
        public Respuesta Delete_CXC_IIC(int ID)
        {
            return new DT_CXC_Proveedor().DeleteIIC(ID);
        }
        public Respuesta ChangeStatus_CXC_Proveedor(int proveedorID, string status)
        {
            return new DT_CXC_Proveedor().ChangeStatus_CXC_Proveedor(proveedorID, status);
        }
        #endregion

        #region Mantenimiento ExtCentral_Segment
        public List<EN_ExtCentral_Segment> GetAll_ExtCentral_Segment()
        {
            return new CT_ExtCentral_Segment().GetAll();
        }
        public Respuesta Save_ExtCentral_Segment(EN_ExtCentral_Segment segment)
        {
            return new CT_ExtCentral_Segment().Save(segment);
        }
        public List<EN_ExtCentral_Segment> GetAllSegment_By_SubCategoryID(int subCategoryID)
        {
            return new CT_ExtCentral_Segment().GetAll_By_SubCategoryID(subCategoryID);
        }
        #endregion

        #region Mantenimiento de Casas Comerciales
        public List<EN_Purchaser> GetAllPurchasers()
        {
            return new CT_Purchaser().GetAll();
        }
        public List<EN_Purchaser> GetAllPurchasersByInactive(bool inactive)
        {
            return new CT_Purchaser().GetAllByInactive(inactive);
        }
        public Respuesta SavePurchaser(EN_Purchaser purchaser)
        {
            return new CT_Purchaser().Save(purchaser);
        }
        #endregion

        #region Departamentos

        public EN_Department GetDepartment(int id)
        {
            CT_Department evento = new CT_Department();
            return evento.Get(id);
        }
        public List<EN_Department> GetAllDepartments(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            CT_Department evento = new CT_Department();
            return evento.GetAll(busqueda, estado, cantidad);
        }
        public Respuesta SaveDepartment(EN_Department department)
        {
            CT_Department evento = new CT_Department();
            return evento.Save(department);
        }

        #endregion

        #region Categorias
        public List<EN_Category> GetAllCategories(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return new CT_Category().GetAll(busqueda, estado, cantidad);
        }
        public List<EN_Category> GetAllCategory_Simple(int supplierID)
        {
            return new CT_Category().GetAll_Simple(supplierID);
        }
        public Respuesta SaveCategory(EN_Category category)
        {
            return new CT_Category().Save(category);
        }
        public Respuesta GetCategory(int ID)
        {
            return new CT_Category().Get(ID);
        }
        public List<EN_Category> GetAllCategories_ByDepartmentID(int departmentID)
        {
            return new CT_Category().GetAll_ByDepartmentID(departmentID);
        }
        #endregion

        #region Descuentos
        public List<EN_QuantityDiscount> GetAllQuantityDiscounts(int estado = 0, int cantidad = 0)
        {
            return new CT_QuantityDiscount().GetAll(estado, cantidad);
        }
        public Respuesta SaveQuantityDiscount(EN_QuantityDiscount quantityDiscount)
        {
            return new CT_QuantityDiscount().Save(quantityDiscount);
        }
        public EN_QuantityDiscount GetQuantityDiscount(int ID)
        {
            return new CT_QuantityDiscount().Get(ID);
        }
        public List<EN_QuantityDiscount> GetAllSimpleQuantityDiscountsByType(int type)
        {
            return new CT_QuantityDiscount().GetAllSimpleByType(type);
        }
        #endregion

        #region ItemMessages
        public List<EN_ItemMessage> GetAllItemMessage(string busqueda, int estado, int cantidad)
        {
            return (List<EN_ItemMessage>)new CT_ItemMessage().GetAll(busqueda, estado, cantidad).Result;
        }

        public Respuesta SaveItemMessage(EN_ItemMessage itemMessage)
        {
            return new CT_ItemMessage().Save(itemMessage);
        }

        public Respuesta GetItemMessage(int ID)
        {
            return new CT_ItemMessage().Get(ID);
        }
        #endregion

        #region Horarios
        public List<EN_Schedule> GetAllSchedules(string busqueda, int estado, int cantidad)
        {
            return new CT_Schedule().GetAll(busqueda, estado, cantidad);
        }
        public Respuesta SaveSchedule(EN_Schedule schedule, List<EN_ScheduleSegment> segments)
        {
            return new CT_Schedule().Save(schedule, segments);
        }
        public Respuesta GetSchedule(int ID)
        {
            return new CT_Schedule().Get(ID);
        }
        #region Segmentos de Horarios
        public List<EN_ScheduleSegment> GetAllScheduleSegment()
        {
            return new CT_ScheduleSegment().GetAll();
        }
        public List<EN_ScheduleSegment> GetAllScheduleSegmentByScheduleID(int scheduleID)
        {
            return new CT_ScheduleSegment().GetByScheduleID(scheduleID);
        }
        #endregion
        #endregion

        #region Familias
        public List<EN_ExtCentral_Family> GetAllFamilies(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return new CT_ExtCentral_Family().GetAll(busqueda, estado, cantidad);
        }

        public Respuesta SaveFamily(EN_ExtCentral_Family family)
        {
            return new CT_ExtCentral_Family().Save(family);
        }

        public Respuesta GetFamily(int ID)
        {
            return new CT_ExtCentral_Family().Get(ID);
        }
        #endregion

        #region SubCategorías
        public List<EN_ExtCentral_SubCategory> GetAllSubCategories(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return new CT_ExtCentral_SubCategory().GetAll(busqueda, estado, cantidad);
        }
        public Respuesta SaveSubCategories(EN_ExtCentral_SubCategory subCategory)
        {
            return new CT_ExtCentral_SubCategory().Save(subCategory);
        }
        public Respuesta GetSubCategories(int ID)
        {
            return new CT_ExtCentral_SubCategory().Get(ID);
        }
        public List<EN_ExtCentral_SubCategory> GetAllSubCategory_By_CategoryID(int categoryID)
        {
            return new CT_ExtCentral_SubCategory().GetAll_By_CategoryID(categoryID);
        }
        #endregion

        #region Mantenimiento Marcas
        public List<EN_ExtCentral_Brand> GetAllBrands()
        {
            return new CT_ExtCentral_Brand().GetAll();
        }
        public Respuesta SaveBrand(int id, string description)
        {
            return new CT_ExtCentral_Brand().Save(id, description);
        }
        #endregion

        #region Mantenimiento Fabricantes
        public List<EN_ExtCentral_Manufacturer> GetAllManufacturers()
        {
            return new CT_ExtCentral_Manufacturer().GetAll();
        }
        public Respuesta SaveManufacturer(int id, string description)
        {
            return new CT_ExtCentral_Manufacturer().Save(id, description);
        }
        #endregion

        #region Artículos
        public DataSet GetAllItems(string valorBusqueda = "", int estado = 0, int resultCount = 0)
        {
            return new CT_Item().GetAll(valorBusqueda, estado, resultCount);
        }
        public List<EN_Item> GetAllItemsList(string valorBusqueda = "", int estado = 0, int resultCount = 0)
        {
            return new CT_Item().GetAllList(valorBusqueda, estado, resultCount);
        }
        public List<EN_Item> GetAllItemsDynamicList(string SearchValue, int OrderColumn, string OrderDirection, int Skip, int Take, int[] famsIDs, int[] depsIDs, int[] catsIDs, int[] subcatsIDs, int[] segsIDs, string storesID)
        {
            return new CT_Item().GetDynamicList(SearchValue, OrderColumn, OrderDirection, Skip, Take, famsIDs, depsIDs, catsIDs, subcatsIDs, segsIDs, storesID);
        }
        public Dictionary<string, int> GetItemsCountRecords(string SearchValue, int[] famsIDs, int[] depsIDs, int[] catsIDs, int[] subcatsIDs, int[] segsIDs, string storesID)
        {
            return new CT_Item().GetCountRecords(SearchValue, famsIDs, depsIDs, catsIDs, subcatsIDs, segsIDs, storesID);
        }
        public List<EN_Item> GetAllItem_Simple(int supplierID)
        {
            return new CT_Item().GetAll_Simple(supplierID);
        }
        public List<EN_ItemStore> GetAllItem_ItemStore(int itemID, string storesID)
        {
            return new CT_Item().GetAll_ItemStore(itemID, storesID);
        }
        public Dictionary<string, object> SaveItem(EN_Item item, string title, string userID)
        {
            return new CT_Item().Save(item, title, userID);
        }
        public EN_Item GetItemByItemLookupCode(string itemLookupCode, string storesID)
        {
            return new CT_Item().GetByItemLookupCode(itemLookupCode, storesID);
        }

        public Respuesta ValidateItemExists(string itemLookupCode)
        {
            return new CT_Item().ValidateExists(itemLookupCode);
        }

        public List<Respuesta> ValidateItemsExistsList(string[] itemLookupCode)
        {
            return new CT_Item().ValidateItemsExistsList(itemLookupCode);
        }
        #endregion

        #region SupplierList
        public List<EN_SupplierList> GetAllSupplierListByItemID(int itemID, string storesID)
        {
            return new CT_SupplierList().GetAllByItemID(itemID, storesID);
        }
        #endregion

        #region POC_ItemPrice
        public List<EN_POC_ItemPrice> GetAllPOC_ItemPriceByItemID(int itemID, string storesID)
        {
            return new CT_POC_ItemPrice().GetAllByItemID(itemID, storesID);
        }
        #endregion

        #region Stores
        public List<EN_Store> GetAllStore(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return new CT_Store().GetAll(busqueda, estado, cantidad);
        }
        public Respuesta GetStore(int ID)
        {
            return new CT_Store().Get(ID);
        }
        public List<EN_Store> GetAllStore_By_StoreGroupID(int storeGroupID, string stores_Id)
        {
            return new CT_Store().GetAll_By_StoreGroupID(storeGroupID, stores_Id);
        }
        public List<EN_Store> GetStores_ItemStatus(int itemID, string storesAvailable)
        {
            return new CT_Store().GetStores_ItemStatus(itemID, storesAvailable);
        }
        #endregion

        #region StoreGroup
        public List<EN_StoreGroup> GetAll_StoreGroup(string stores_Id)
        {
            return new CT_StoreGroup().GetAll(stores_Id);
        }
        #endregion

        #region Proveedores
        public List<EN_Supplier> GetAllSupplier(string stores_ID, string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return new CT_Supplier().GetAll(stores_ID, busqueda, estado, cantidad);
        }

        public EN_Supplier GetSupplier(int ID, string stores_ID)
        {
            return new CT_Supplier().Get(ID, stores_ID);
        }
        #endregion

        #region Cabeceras
        public List<EN_Cabeceras> GetAllCabeceras()
        {
            return new CT_Cabeceras().GetAll();
        }

        public Respuesta SaveCabecera(EN_Cabeceras cabecera)
        {
            return new CT_Cabeceras().Save(cabecera);
        }

        public Respuesta GetCabecera(int ID)
        {
            return new CT_Cabeceras().Get(ID);
        }
        public List<EN_Cabeceras> GetAllCabecerasXTienda(int idStore)
        {
            return new CT_Cabeceras().GetAllCabeceraxTienda(idStore);
        }

        public List<EN_CabecerasTienda> GetAllCabecerasTienda()
        {
            return new CT_CabecerasTienda().GetAll();
        }

        public Respuesta SaveCabeceraTienda(EN_CabecerasTienda cabecera)
        {
            return new CT_CabecerasTienda().Save(cabecera);
        }
        #endregion

        #region Impuestos de Ventas
        public List<EN_Tax> GetAllTax(string busqueda = "", int estado = 0, int cantidad = 0)
        {
            return new CT_Tax().GetAll(busqueda, estado, cantidad);
        }
        public EN_Tax GetTax(int ID)
        {
            return new CT_Tax().Get(ID);
        }
        #endregion

        #region Impuestos de Ventas
        public List<EN_ItemTax> GetAll_ItemTax()
        {
            return new CT_ItemTax().GetAll();
        }
        #endregion

        #region IslasSideKick
        public List<EN_IslasSideKick> GetAllIslasSideKick()
        {
            return new CT_IslasSideKick().GetAll();
        }

        public Respuesta SaveIslasSideKick(EN_IslasSideKick islasSideKick)
        {
            return new CT_IslasSideKick().Save(islasSideKick);
        }

        public Respuesta GetIslasSideKick(int ID)
        {
            return new CT_IslasSideKick().Get(ID);
        }

        #endregion

        #region Espacios Frios
        public List<EN_EspaciosFrios> GetAllEspaciosFrios()
        {
            return new CT_EspaciosFrios().GetAll();
        }

        public Respuesta SaveEspaciosFrios(EN_EspaciosFrios espFrio)
        {
            return new CT_EspaciosFrios().Save(espFrio);
        }




        public List<EN_EspaciosFriosTienda> GetAllEspaciosFriosTienda()
        {
            return new CT_EspaciosFriosTienda().GetAll();
        }

        public Respuesta SaveEspaciosFriosTienda(EN_EspaciosFriosTienda espFrio)
        {
            return new CT_EspaciosFriosTienda().Save(espFrio);
        }

        public List<EN_EspaciosFrios> GetAllEspaciosFriosXTienda(int idStore)
        {
            return new CT_EspaciosFrios().GetAllEspaciosFriosxTienda(idStore);
        }
        #endregion

        #region Unidades de Medida
        public List<EN_UOM> GetAllUOMByInactive(bool inactive)
        {
            return new CT_UOM().GetAllByInactive(inactive);
        }
        #endregion

        #region Wizard
        public List<EN_ExtCentral_WizardList> GetAll_WizardList()
        {
            return new CT_ExtCentral_WizardList().GetAll();
        }
        /** Tarea cambio costos */
        public Respuesta Wizard_Task102(int storeID, string title, string notes, List<EN_WizardStructs.TablaCost> items, string userID)
        {
            return new CT_Wizard().Task102(storeID, title, notes, items, userID);
        }
        /** Tarea cambio impuestos */
        public Respuesta Wizard_Task104(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemTax> items, string userID)
        {
            return new CT_Wizard().Task104(storeID, title, notes, items, userID);
        }
        /** Tarea cambio estados */
        public Respuesta Wizard_Task105(int storeID, string title, string notes, List<EN_WizardStructs.TablaEstado> items, string userID)
        {
            return new CT_Wizard().Task105(storeID, title, notes, items, userID);
        }
        /** Tarea cambio Descripción (Corta y Extendida) */
        public Respuesta Wizard_Task107(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemDes> items, string userID)
        {
            return new CT_Wizard().Task107(storeID, title, notes, items, userID);
        }
        /** Tarea cambio Descripción Larga */
        public List<Respuesta> Wizard_Task108(int[] stores, string title, string notes, List<EN_WizardStructs.TablaItemSub1> items, string userID)
        {
            return new CT_Wizard().Task108(stores, title, notes, items, userID);
        }
        /** Tarea cambio subdescription2 */
        public Respuesta Wizard_Task109(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemSub2> items, string userID)
        {
            return new CT_Wizard().Task109(storeID, title, notes, items, userID);
        }
        /** Tarea cambio subdescription3 (Cabys) */
        public Respuesta Wizard_Task110(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemSub3> items, string userID)
        {
            return new CT_Wizard().Task110(storeID, title, notes, items, userID);
        }
        /** Cambio de Proveedor - Costos */
        public Respuesta Wizard_Task120(int[] storesID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemProveedor> items, string userID, int workSheetContentID)
        {
            return new CT_Wizard().Task120(storesID, tittle, notes, effectiveDate, items, userID, workSheetContentID);
        }
        /** Cambio precios: Dinámicos */
        public Respuesta Wizard_Task121(int storeID, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemPriceDynamic> items, string userID)
        {
            return new CT_Wizard().Task121(storeID, title, notes, effectiveDate, items, userID);
        }
        /** Cambio precios: Descuentos Compre X lleve Y por Z porcentaje de descuento */
        public Respuesta Wizard_Task122(int storeID, string title, string notes, int quantityDiscountID, DateTime startDate, DateTime endDate, List<EN_WizardStructs.TablaItemQuantityDiscount> items, string userID)
        {
            return new CT_Wizard().Task122(storeID, title, notes, quantityDiscountID, startDate, endDate, items, userID);
        }
        /** Cambio precios: Margen de Utilidad */
        public Respuesta Wizard_Task124(int[] stores, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaMargenUtility> items, string userID)
        {
            return new CT_Wizard().Task124(stores, title, notes, effectiveDate, items, userID);
        }
        /** Cambio precios: Regular */
        public Respuesta Wizard_Task123(int[] storesID, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemPriceRegular> items, string userID)
        {
            return new CT_Wizard().Task123(storesID, title, notes, effectiveDate, items, userID);
        }
        /** Imprtar productos */
        public Respuesta Wizard_Task130(int[] storesID, string title, string notes, DateTime effectiveDate, DateTime fromDate, List<EN_WizardStructs.TablaItem> items, string userID)
        {
            return new CT_Wizard().Task130(storesID, title, notes, effectiveDate, fromDate, items, userID);
        }
        /** Imprtar productos */
        public Respuesta Wizard_Task131(int[] storesID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemActivate> items, string userID, int workSheetContentID)
        {
            return new CT_Wizard().Task131(storesID, tittle, notes, effectiveDate, items, userID, workSheetContentID);
        }
        List<EN_Item> IUltraERPService.GetItemByItemLookupCodebyLista(string itemLookupCode, string storesID)
        {
            return new CT_Item().GetByItemLookupCodebyLista(itemLookupCode, storesID);
        }
        public Respuesta Wizard_Download_Items_Stores(int[] storesID, string title, string notes, DateTime effectiveDate, DateTime fromDate, string[] items, string userID, int workSheetContentID)
        {
            return new CT_Wizard().Wizard_Download_Items_Stores(storesID, title, notes, effectiveDate, fromDate, items, userID, workSheetContentID);
        }
        /** Cambio de propiedades */
        public Respuesta EN_Worksheet_Change_Item_Properties(string stores, string properties, int dataContentID, string userID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemProperties> items)
        {
            return new CT_Wizard().Task201(stores, properties, dataContentID, userID, tittle, notes, effectiveDate, items);
        }
        #endregion

        #region Users
        //public (EN_User, int) EN_User_ValidateUser(string userName, string password)
        //{
        //    return new CT_User().ValidateUser(userName, password);
        //}

        public EN_User_ValidateUserResponse EN_User_ValidateUser(string userName, string password)
        {
            return new CT_User().ValidateUser(userName, password);
        }


        #endregion

        #region Worksheet
        public List<EN_Worksheet> EN_Worksheet_GetAll(string storesID, string tasksCode, string hqUsersID, string searchValue, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            return new CT_Worksheet().GetAll(storesID, tasksCode, hqUsersID, searchValue, orderColumn, orderDirection, skip, take, fromDate, toDate);
        }

        public EN_Worksheet EN_Worksheet_Get(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new CT_Worksheet().Get(storesID, tasksCode, users, worksheetID);
        }

        public Dictionary<string, int> EN_Worksheet_GetCountRecord(string storesID, string tasksCode, string hqUsersID, string searchValue, string fromDate, string toDate)
        {
            return new CT_Worksheet().GetCountRecord(storesID, tasksCode, hqUsersID, searchValue, fromDate, toDate);
        }

        public List<EN_Worksheet.WorksheetStore> EN_Worksheet_GetStores(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new CT_Worksheet().GetStores(storesID, tasksCode, users, worksheetID);
        }

        public List<EN_Worksheet.WorksheetHistory> EN_Worksheet_GetHistories(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new CT_Worksheet().GetHistories(storesID, tasksCode, users, worksheetID);
        }

        public List<EN_Worksheet.WorksheetUpdateItemPrice> EN_Worksheet_GetAll_WorksheetUpdateItemPrice(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new CT_Worksheet().GetAll_WorksheetUpdateItemPrice(storesID, tasksCode, users, worksheetID);
        }

        public List<EN_Worksheet.Worksheet_ItemUpdate> EN_Worksheet_GetAll_Worksheet_ItemUpdate(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new CT_Worksheet().GetAll_Worksheet_ItemUpdate(storesID, tasksCode, users, worksheetID);
        }

        public List<EN_Worksheet.Worksheet_ItemTax> EN_Worksheet_GetAll_Worksheet_ItemTax(string storesID, string tasksCode, string users, int worksheetID)
        {
            return new CT_Worksheet().GetAll_Worksheet_ItemTax(storesID, tasksCode, users, worksheetID);
        }

        public Respuesta EN_Worksheet_Change_Status(string stores, string tasks, string users, int hqUserID, int worksheetID, int status)
        {
            return new CT_Worksheet().Change_Status(stores, tasks, users, hqUserID, worksheetID, status);
        }

        public EN_Worksheet.WorksheetContent EN_Worksheet_Get_WorksheetContent(int worksheetContentID, int worksheetID, string stores, string tasks, string users)
        {
            return new CT_Worksheet().Get_WorksheetContent(worksheetContentID, worksheetID, stores, tasks, users);
        }

        public Respuesta EN_Worksheet_Save_WorksheetContent(EN_Worksheet.WorksheetContent cont)
        {
            return new CT_Worksheet().Save_WorksheetContent(cont);
        }
        #endregion

        #region Propiedades de Artículos
        public List<EN_ItemCustomProperty> GetCustomProperty(string propertiesAvailable)
        {
            return new CT_ItemProperties().GetCustomProperty(propertiesAvailable);
        }
        public List<EN_ItemExt> GetItemPropertiesByItem(int itemID, string storesAvailable, string propertiesAvailable)
        {
            return new CT_ItemProperties().GetItemPropertiesByItem(itemID, storesAvailable, propertiesAvailable);
        }
        public List<EN_ItemProperty> GetAllItemsProperties(string storesAvailable, string propertiesAvailable, string searchValue, int orderColumn, string orderDirection, int skip, int take)
        {
            return new CT_ItemProperties().GetAllItemsProperties(storesAvailable, propertiesAvailable, searchValue, orderColumn, orderDirection, skip, take);
        }
        public Dictionary<string, int> Get_ItemProperties_CountRecord(string storesAvailable, string searchValue)
        {
            return new CT_ItemProperties().Get_ItemProperties_CountRecord(storesAvailable, searchValue);
        }
        public Respuesta Save_ItemExtProperty(string properties, int itemID, string propsAvailable, string stores, int hqUserID)
        {
            return new CT_ItemProperties().Save_ItemExtProperty(properties, itemID, propsAvailable, stores, hqUserID);
        }
        public List<EN_ItemProperty> GetAllItemsProperties_By_List(string items, string storesAvailable, string propertiesAvailable)
        {
            return new CT_ItemProperties().GetAllItemsProperties_By_List(items, storesAvailable, propertiesAvailable);
        }
        #endregion

        #region DocumentosMH
        public List<EN_DocumentosMH> GetAllDocsMH(string storesID, string hqUsersID, string searchValue, string estadoHacienda, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            return new CT_DocumentosMH().GetAllDocsMH(storesID, hqUsersID, searchValue, estadoHacienda, orderColumn, orderDirection, skip, take, fromDate, toDate);
        }


        public Dictionary<string, int> GetCountRecordDocsMH(string storesID, string hqUsersID, string searchValue, string estadoHacienda, string fromDate, string toDate)
        {
            return new CT_DocumentosMH().GetCountRecordDocsMH(storesID, hqUsersID, searchValue, estadoHacienda, fromDate, toDate);
        }

        #endregion

        #region Graphics
        public List<EN_Graphics> GetAllGraphics(string storeID = "", string codSucursal = "", string usersID = "", string busqueda = "", string fromDate = "", string toDate = "", string tipo = "")
        {
            return new CT_Graphics().GetAll(storeID, codSucursal, usersID, busqueda, fromDate, toDate, tipo);
        }

        #endregion

        #region Mantenimiento PaymentTerms
        public List<EN_AR_PaymentTerms> AR_PaymentTerms_GetAll()
        {
            return new CT_AR_PaymentTerms().GetAll();
        }
        public Respuesta EN_AR_PAYMENTTERMS_SAVE(EN_AR_PaymentTerms paymentTerms)
        {
            return new CT_AR_PaymentTerms().Save(paymentTerms);
        }
        #endregion
        #region Mantenimiento AccountGroup
        public List<EN_AR_AccountGroup> AR_AccountGroup_GetAll()
        {
            return new CT_AR_AccountGroup().GetAll();
        }

        public List<EN_ExtCentral_AR_AccountGroup> ExtCentral_AR_AccountGroup_GetAll()
        {
            return new CT_ExtCentral_AR_AccountGroup().GetAll();
        }

        public List<EN_AR_FinanceCharge> AR_FinanceCharge_GetAll()
        {
            return new CT_AR_FinanceCharge().GetAll();
        }

        public List<EN_AR_AccountManager> AR_AccountManager_GetAll()
        {
            return new CT_AR_AccountManager().GetAll();
        }

        public List<EN_AR_StatementType> AR_StatementType_GetAll()
        {
            return new CT_AR_StatementType().GetAll();
        }

        public List<EN_RR_NumberSeries> RR_NumberSeries_GetAll()
        {
            return new CT_RR_NumberSeries().GetAll();
        }

        public Respuesta EN_AR_ACCOUNTGROUP_SAVE(EN_AR_AccountGroup accountGroup, EN_ExtCentral_AR_AccountGroup extCentral_AR_AccountGroup)
        {
            return new CT_AR_AccountGroup().Save(accountGroup, extCentral_AR_AccountGroup);
        }
        #endregion

        #region Mantenimiento Customers
        public List<EN_Customer> Customers_GetAll()
        {
            return new CT_Customer().GetAll();
        }

        public Respuesta EN_CUSTOMER_SAVE(string prefijo, int accountGroupID, EN_Customer customer)
        {
            return new CT_Customer().Save(prefijo, accountGroupID, customer);
        }

        public List<EN_ExtCentral_Customer> ExtCentral_Customer_GetAll()
        {
            return new CT_ExtCentral_Customer().GetAll();
        }

        public List<EN_CustomCaption> CustomCaption_GetAll()
        {
            return new CT_CustomCaption().GetAll();
        }

        public List<EN_AR_Account> AR_Account_GetAll()
        {
            return new CT_AR_Account().GetAll();
        }

        public List<EN_AR_AccountBalance> AR_AccountBalance_GetAll()
        {
            return new CT_AR_AccountBalance().GetAll();
        }

        public List<EN_AR_CustomerBalance> AR_CustomerBalance_GetAll()
        {
            return new CT_AR_CustomerBalance().GetAll();
        }

        public List<EN_ExtCentral_AR_AccountDynamic> ExtCentral_AR_AccountDynamic_GetAll()
        {
            return new CT_ExtCentral_AR_AccountDynamic().GetAll();
        }

        public Respuesta EN_AR_ACCOUNT_SAVE(EN_AR_Account account, EN_ExtCentral_AR_AccountDynamic accountDynamic, EN_AR_AccountLink accountLink)
        {
            return new CT_AR_Account().Save(account, accountDynamic, accountLink);
        }

        public List<EN_ExtCentral_AR_AccountDynamic> ExtCentral_AR_AccountDynamic_GetStoreBalance(int accountID, int groupID)
        {
            return new CT_ExtCentral_AR_AccountDynamic().GetStoreBalance(accountID, groupID);
        }
        #endregion
    }
}
