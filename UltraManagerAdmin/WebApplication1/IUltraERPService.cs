using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace WebApplication1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUltraERPService" in both code and config file together.
    [ServiceContract]
    public interface IUltraERPService
    {
        #region Mantenimiento CXC_Proveedores
        [OperationContract]
        List<EN_CXC_Proveedor> GetAll_CXC_Proveedor();
        [OperationContract]
        List<EN_CXC_FormaRebajo> GetAll_CXC_FormaRebajos();
        [OperationContract]
        List<EN_Tipo> GetAll_CXC_IDCTipo();
        [OperationContract]
        List<EN_Tipo> GetAll_CXC_IICTipo();
        [OperationContract]
        List<EN_Tipo> GetAll_CXC_FrecuenciasPago();
        [OperationContract]
        List<EN_Tipo> GetAll_CXC_Negociaciones();
        [OperationContract]
        List<EN_CXC_IIC> GetAll_CXC_IIC(int fichaID);
        [OperationContract]
        List<EN_CXC_IDC> GetAll_CXC_IDC(int fichaID);
        [OperationContract]
        Dictionary<string, object> Save_CXC_Proveedor(EN_CXC_Proveedor ficha);
        [OperationContract]
        Respuesta Save_CXC_IDC(EN_CXC_IDC idc, int CXC_ProveedorID);
        [OperationContract]
        Respuesta Save_CXC_IIC(EN_CXC_IIC iic, int CXC_ProveedorID);
        [OperationContract]
        Respuesta Delete_CXC_IDC(int ID);
        [OperationContract]
        Respuesta Delete_CXC_IIC(int ID);
        [OperationContract]
        Respuesta ChangeStatus_CXC_Proveedor(int proveedorID, string status);
        #endregion

        #region Mantenimiento ExtCentral_Segment
        [OperationContract]
        List<EN_ExtCentral_Segment> GetAll_ExtCentral_Segment();
        [OperationContract]
        Respuesta Save_ExtCentral_Segment(EN_ExtCentral_Segment segment);
        [OperationContract]
        List<EN_ExtCentral_Segment> GetAllSegment_By_SubCategoryID(int subCategoryID);
        #endregion

        #region Mantenimiento Casas Comerciales
        [OperationContract]
        List<EN_Purchaser> GetAllPurchasers();
        [OperationContract]
        List<EN_Purchaser> GetAllPurchasersByInactive(bool inactive);
        [OperationContract]
        Respuesta SavePurchaser(EN_Purchaser purchaser);
        #endregion

        #region Mantenimiento departamentos
        [OperationContract]
        List<EN_Department> GetAllDepartments(string busqueda = "", int estado = 0, int cantidad = 0);
        [OperationContract]
        EN_Department GetDepartment(int id);
        [OperationContract]
        Respuesta SaveDepartment(EN_Department department);
        #endregion

        #region Mantenimiento Categorías
        [OperationContract]
        List<EN_Category> GetAllCategories(string busqueda = "", int estado = 0, int cantidad = 0);
        [OperationContract]
        List<EN_Category> GetAllCategory_Simple(int supplierID);
        [OperationContract]
        Respuesta SaveCategory(EN_Category category);
        [OperationContract]
        Respuesta GetCategory(int ID);
        [OperationContract]
        List<EN_Category> GetAllCategories_ByDepartmentID(int departmentID);
        #endregion

        #region Mantenimiento Descuentos
        [OperationContract]
        List<EN_QuantityDiscount> GetAllQuantityDiscounts(int estado = 0, int cantidad = 0);
        [OperationContract]
        Respuesta SaveQuantityDiscount(EN_QuantityDiscount quantityDiscount);
        [OperationContract]
        EN_QuantityDiscount GetQuantityDiscount(int ID);
        [OperationContract]
        List<EN_QuantityDiscount> GetAllSimpleQuantityDiscountsByType(int type);
        #endregion

        #region Mantenimiento de Mensajes de Artículos
        [OperationContract]
        List<EN_ItemMessage> GetAllItemMessage(string busqueda, int estado, int cantidad);
        [OperationContract]
        Respuesta SaveItemMessage(EN_ItemMessage itemMessage);
        [OperationContract]
        Respuesta GetItemMessage(int ID);
        #endregion

        #region Mantenimiento de Horarios
        [OperationContract]
        List<EN_Schedule> GetAllSchedules(string busqueda, int estado, int cantidad);
        [OperationContract]
        Respuesta SaveSchedule(EN_Schedule schedule, List<EN_ScheduleSegment> segments);
        [OperationContract]
        Respuesta GetSchedule(int ID);
        #region Segmentos de horarios
        [OperationContract]
        List<EN_ScheduleSegment> GetAllScheduleSegment();
        [OperationContract]
        List<EN_ScheduleSegment> GetAllScheduleSegmentByScheduleID(int scheduleID);
        #endregion
        #endregion

        #region Mantenimiento Familias
        [OperationContract]
        List<EN_ExtCentral_Family> GetAllFamilies(string busqueda = "", int estado = 0, int cantidad = 0);
        [OperationContract]
        Respuesta SaveFamily(EN_ExtCentral_Family family);
        [OperationContract]
        Respuesta GetFamily(int ID);
        #endregion

        #region Mantenimiento SubCategorías
        [OperationContract]
        List<EN_ExtCentral_SubCategory> GetAllSubCategories(string busqueda = "", int estado = 0, int cantidad = 0);
        [OperationContract]
        Respuesta SaveSubCategories(EN_ExtCentral_SubCategory subCategory);
        [OperationContract]
        Respuesta GetSubCategories(int ID);
        [OperationContract]
        List<EN_ExtCentral_SubCategory> GetAllSubCategory_By_CategoryID(int categoryID);
        #endregion

        #region Mantenimiento Marcas
        [OperationContract]
        List<EN_ExtCentral_Brand> GetAllBrands();
        [OperationContract]
        Respuesta SaveBrand(int id, string description);
        #endregion

        #region Mantenimiento Fabricante
        [OperationContract]
        List<EN_ExtCentral_Manufacturer> GetAllManufacturers();
        [OperationContract]
        Respuesta SaveManufacturer(int id, string description);
        #endregion

        #region Mantenimiento Artículos
        [OperationContract]
        DataSet GetAllItems(string valorBusqueda = "", int estado = 0, int resultCount = 0);
        [OperationContract]
        List<EN_Item> GetAllItemsList(string valorBusqueda = "", int estado = 0, int resultCount = 0);
        [OperationContract]
        List<EN_Item> GetAllItemsDynamicList(string SearchValue, int OrderColumn, string OrderDirection, int Skip, int Take, int[] famsIDs, int[] depsIDs, int[] catsIDs, int[] subcatsIDs, int[] segsIDs, string storesID);
        [OperationContract]
        Dictionary<string, int> GetItemsCountRecords(string SearchValue, int[] famsIDs, int[] depsIDs, int[] catsIDs, int[] subcatsIDs, int[] segsIDs, string storesID);
        [OperationContract]
        List<EN_Item> GetAllItem_Simple(int supplierID);
        [OperationContract]
        List<EN_ItemStore> GetAllItem_ItemStore(int itemID, string storesID);
        [OperationContract]
        Dictionary<string, object> SaveItem(EN_Item item, string title, string userID);
        [OperationContract]
        EN_Item GetItemByItemLookupCode(string itemLookupCode, string storesID);
        [OperationContract]
        List<EN_Item> GetItemByItemLookupCodebyLista(string itemLookupCode, string storesID);
        [OperationContract]
        Respuesta ValidateItemExists(string itemLookupCode);
        [OperationContract]
        List<Respuesta> ValidateItemsExistsList(string[] itemLookupCode);
        #endregion

        #region SupplierList
        [OperationContract]
        List<EN_SupplierList> GetAllSupplierListByItemID(int itemID, string storesID);
        #endregion

        #region POC_ItemPrice
        [OperationContract]
        List<EN_POC_ItemPrice> GetAllPOC_ItemPriceByItemID(int itemID, string storesID);
        #endregion

        #region Stores
        [OperationContract]
        List<EN_Store> GetAllStore(string busqueda = "", int estado = 0, int cantidad = 0);
        [OperationContract]
        Respuesta GetStore(int ID);
        [OperationContract]
        List<EN_Store> GetAllStore_By_StoreGroupID(int storeGroupID, string stores_Id);
        [OperationContract]
        List<EN_Store> GetStores_ItemStatus(int itemID, string storesAvailable);
        #endregion

        #region StoreGroup
        [OperationContract]
        List<EN_StoreGroup> GetAll_StoreGroup(string stores_Id);
        #endregion

        #region obtiene Proveedores
        [OperationContract]
        List<EN_Supplier> GetAllSupplier(string stores_ID, string busqueda = "", int estado = 0, int cantidad = 0);
        [OperationContract]
        EN_Supplier GetSupplier(int ID, string stores_ID);
        #endregion

        #region Impuesto de Ventas
        [OperationContract]
        List<EN_Tax> GetAllTax(string busqueda = "", int estado = 0, int cantidad = 0);
        [OperationContract]
        EN_Tax GetTax(int ID);
        #endregion

        #region Impuesto de Artículos
        [OperationContract]
        List<EN_ItemTax> GetAll_ItemTax();
        #endregion

        #region Mantenimiento cabeceras
        [OperationContract]
        List<EN_Cabeceras> GetAllCabeceras();
        [OperationContract]
        Respuesta SaveCabecera(EN_Cabeceras cabecera);
        [OperationContract]
        Respuesta GetCabecera(int ID);
        [OperationContract]
        List<EN_CabecerasTienda> GetAllCabecerasTienda();
        [OperationContract]
        Respuesta SaveCabeceraTienda(EN_CabecerasTienda cabecera);
        [OperationContract]
        List<EN_Cabeceras> GetAllCabecerasXTienda(int idStore);
        #endregion

        #region Mantenimiento Islas y SideKick
        [OperationContract]
        List<EN_IslasSideKick> GetAllIslasSideKick();
        [OperationContract]
        Respuesta SaveIslasSideKick(EN_IslasSideKick islasSideKick);
        [OperationContract]
        Respuesta GetIslasSideKick(int ID);

        #endregion

        #region Mantenimiento espacios frios
        [OperationContract]
        List<EN_EspaciosFrios> GetAllEspaciosFrios();
        [OperationContract]
        Respuesta SaveEspaciosFrios(EN_EspaciosFrios espFrio);

        [OperationContract]
        List<EN_EspaciosFriosTienda> GetAllEspaciosFriosTienda();
        [OperationContract]
        Respuesta SaveEspaciosFriosTienda(EN_EspaciosFriosTienda espFrio);
        [OperationContract]
        List<EN_EspaciosFrios> GetAllEspaciosFriosXTienda(int idStore);

        #endregion

        #region Unidades de medida
        [OperationContract]
        List<EN_UOM> GetAllUOMByInactive(bool inactive);
        #endregion

        #region wizard
        /** Obtener lista de tareas Wizard */
        [OperationContract]
        List<EN_ExtCentral_WizardList> GetAll_WizardList();
        /** Tarea cambio costos */
        [OperationContract]
        Respuesta Wizard_Task102(int storeID, string title, string notes, List<EN_WizardStructs.TablaCost> items, string userID);
        /** Tarea cambio impuestos */
        [OperationContract]
        Respuesta Wizard_Task104(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemTax> items, string userID);
        /** Tarea cambio estados */
        [OperationContract]
        Respuesta Wizard_Task105(int storeID, string title, string notes, List<EN_WizardStructs.TablaEstado> items, string userID);
        /** Tarea cambio Descripción (Corta y Extendida) */
        [OperationContract]
        Respuesta Wizard_Task107(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemDes> items, string userID);
        /** Tarea cambio Descripción Larga */
        [OperationContract]
        List<Respuesta> Wizard_Task108(int[] stores, string title, string notes, List<EN_WizardStructs.TablaItemSub1> items, string userID);
        /** Tarea cambio subdescription2 */
        [OperationContract]
        Respuesta Wizard_Task109(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemSub2> items, string userID);
        /** Tarea cambio subdescription3 (CabyS) */
        [OperationContract]
        Respuesta Wizard_Task110(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemSub3> items, string userID);
        /** Cambio de Proveedor - Costos */
        [OperationContract]
        Respuesta Wizard_Task120(int[] storesID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemProveedor> items, string userID, int workSheetContentID);
        /** Cambio precios: Dinámicos */
        [OperationContract]
        Respuesta Wizard_Task121(int storeID, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemPriceDynamic> items, string userID);
        /** Cambio precios: Descuentos */
        [OperationContract]
        Respuesta Wizard_Task122(int storeID, string title, string notes, int quantityDiscountID, DateTime startDate, DateTime endDate, List<EN_WizardStructs.TablaItemQuantityDiscount> items, string userID);
        /** Cambio precios: Regular */
        [OperationContract]
        Respuesta Wizard_Task123(int[] storesID, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemPriceRegular> items, string userID);
        /** Cambio precios: Margen de Utilidad */
        [OperationContract]
        Respuesta Wizard_Task124(int[] stores, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaMargenUtility> items, string userID);
        /** Imprtar productos */
        [OperationContract]
        Respuesta Wizard_Task130(int[] storesID, string title, string notes, DateTime effectiveDate, DateTime fromDate, List<EN_WizardStructs.TablaItem> items, string userID);
        [OperationContract]
        Respuesta Wizard_Task131(int[] storesID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemActivate> items, string userID, int workSheetContentID);
        [OperationContract]
        Respuesta Wizard_Download_Items_Stores(int[] storesID, string title, string notes, DateTime effectiveDate, DateTime fromDate, string[] items, string userID, int workSheetContentID);
        #endregion

        #region Users
        [OperationContract]
        (EN_User, int) EN_User_ValidateUser(string userName, string password);
        #endregion

        #region Worksheet
        [OperationContract]
        List<EN_Worksheet> EN_Worksheet_GetAll(string storesID, string tasksCode, string hqUsersID, string searchValue, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate);
        [OperationContract]
        EN_Worksheet EN_Worksheet_Get(string storesID, string tasksCode, string users, int worksheetID);
        [OperationContract]
        Dictionary<string, int> EN_Worksheet_GetCountRecord(string storesID, string tasksCode, string hqUsersID, string searchValue, string fromDate, string toDate);
        [OperationContract]
        List<EN_Worksheet.WorksheetStore> EN_Worksheet_GetStores(string storesID, string tasksCode, string users, int worksheetID);
        [OperationContract]
        List<EN_Worksheet.WorksheetHistory> EN_Worksheet_GetHistories(string storesID, string tasksCode, string users, int worksheetID);
        [OperationContract]
        List<EN_Worksheet.WorksheetUpdateItemPrice> EN_Worksheet_GetAll_WorksheetUpdateItemPrice(string storesID, string tasksCode, string users, int worksheetID);
        [OperationContract]
        List<EN_Worksheet.Worksheet_ItemUpdate> EN_Worksheet_GetAll_Worksheet_ItemUpdate(string storesID, string tasksCode, string users, int worksheetID);
        [OperationContract]
        List<EN_Worksheet.Worksheet_ItemTax> EN_Worksheet_GetAll_Worksheet_ItemTax(string storesID, string tasksCode, string users, int worksheetID);
        [OperationContract]
        Respuesta EN_Worksheet_Change_Status(string stores, string tasks, string users, int hqUserID, int worksheetID, int status);
        [OperationContract]
        EN_Worksheet.WorksheetContent EN_Worksheet_Get_WorksheetContent(int worksheetContentID, int worksheetID, string stores, string tasks, string users);
        [OperationContract]
        Respuesta EN_Worksheet_Save_WorksheetContent(EN_Worksheet.WorksheetContent cont);
        [OperationContract]
        Respuesta EN_Worksheet_Change_Item_Properties(string stores, string properties, int dataContentID, string userID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemProperties> items);
        #endregion

        #region Propiedades de Artículos
        [OperationContract]
        List<EN_ItemCustomProperty> GetCustomProperty(string propertiesAvailable);
        [OperationContract]
        List<EN_ItemExt> GetItemPropertiesByItem(int itemID, string storesAvailable, string propertiesAvailable);
        [OperationContract]
        List<EN_ItemProperty> GetAllItemsProperties(string storesAvailable, string propertiesAvailable, string searchValue, int orderColumn, string orderDirection, int skip, int take);
        [OperationContract]
        Dictionary<string, int> Get_ItemProperties_CountRecord(string storesAvailable, string searchValue);
        [OperationContract]
        Respuesta Save_ItemExtProperty(string properties, int itemID, string propsAvailable, string stores, int hqUserID);
        [OperationContract]
        List<EN_ItemProperty> GetAllItemsProperties_By_List(string items, string storesAvailable, string propertiesAvailable);
        #endregion

        #region DocumentosMH
        [OperationContract]
        List<EN_DocumentosMH> GetAllDocsMH(string storesID, string hqUsersID, string searchValue, string estadoHacienda, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate);
        [OperationContract]
        Dictionary<string, int> GetCountRecordDocsMH(string storesID, string hqUsersID, string searchValue, string estadoHacienda, string fromDate, string toDate);


        #endregion

        #region Graphics

        [OperationContract]
        List<EN_Graphics> GetAllGraphics(string storeID = "", string codSucursal = "", string usersID = "", string busqueda = "", string fromDate = "", string toDate = "", string tipo = "");

        #endregion

        #region Mantenimiento PaymentTerms
        [OperationContract]
        List<EN_AR_PaymentTerms> AR_PaymentTerms_GetAll();

        [OperationContract]
        Respuesta EN_AR_PAYMENTTERMS_SAVE(EN_AR_PaymentTerms paymentTerms);
        #endregion

        #region Mantenimiento AccountGroup
        [OperationContract]
        List<EN_AR_AccountGroup> AR_AccountGroup_GetAll();
        [OperationContract]
        List<EN_ExtCentral_AR_AccountGroup> ExtCentral_AR_AccountGroup_GetAll();
        [OperationContract]
        List<EN_AR_FinanceCharge> AR_FinanceCharge_GetAll();
        [OperationContract]
        List<EN_AR_AccountManager> AR_AccountManager_GetAll();
        [OperationContract]
        List<EN_AR_StatementType> AR_StatementType_GetAll();
        [OperationContract]
        List<EN_RR_NumberSeries> RR_NumberSeries_GetAll();
        [OperationContract]
        Respuesta EN_AR_ACCOUNTGROUP_SAVE(EN_AR_AccountGroup accountGroup, EN_ExtCentral_AR_AccountGroup extCentral_AR_AccountGroup);
        #endregion
        #region Customers
        [OperationContract]
        List<EN_Customer> Customers_GetAll();
        [OperationContract]
        List<EN_ExtCentral_Customer> ExtCentral_Customer_GetAll();
        [OperationContract]
        List<EN_CustomCaption> CustomCaption_GetAll();
        [OperationContract]
        Respuesta EN_CUSTOMER_SAVE(string prefijo, int accountGroupID, EN_Customer customer);
        [OperationContract]
        List<EN_AR_Account> AR_Account_GetAll();
        [OperationContract]
        List<EN_AR_AccountBalance> AR_AccountBalance_GetAll();
        [OperationContract]
        List<EN_AR_CustomerBalance> AR_CustomerBalance_GetAll();
        [OperationContract]
        List<EN_ExtCentral_AR_AccountDynamic> ExtCentral_AR_AccountDynamic_GetAll();
        [OperationContract]
        Respuesta EN_AR_ACCOUNT_SAVE(EN_AR_Account account, EN_ExtCentral_AR_AccountDynamic accountDynamic, EN_AR_AccountLink accountLink);
        [OperationContract]
        List<EN_ExtCentral_AR_AccountDynamic> ExtCentral_AR_AccountDynamic_GetStoreBalance(int accountID, int groupID);
        #endregion

    }
}
