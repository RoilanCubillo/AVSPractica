/*
    UltraERP - Indices de apoyo para listados grandes
    Objetivo:
    - Articulos
    - Documentos de inventario
    - Hojas de trabajo
    - Catalogos relacionados

    Nota:
    Antes de ejecutar en produccion, revise tamano de tablas, ventana de mantenimiento
    y planes actuales. Estos indices estan pensados para filtros, joins y ordenamientos
    usados por la migracion visual actual.
*/

SET NOCOUNT ON;

IF OBJECT_ID('dbo.Item', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Item_ItemLookupCode_List' AND object_id = OBJECT_ID('dbo.Item'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_Item_ItemLookupCode_List
            ON dbo.Item (ItemLookupCode)
            INCLUDE (Description, UnitOfMeasure, DepartmentID, CategoryID, SupplierID, TaxID, ReplacementCost, Cost, Price, Inactive, DateCreated, LastUpdated);
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Item_Inactive_LastUpdated_List' AND object_id = OBJECT_ID('dbo.Item'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_Item_Inactive_LastUpdated_List
            ON dbo.Item (Inactive, LastUpdated DESC, DateCreated DESC, ID DESC)
            INCLUDE (ItemLookupCode, Description, UnitOfMeasure, DepartmentID, CategoryID, SupplierID, TaxID, ReplacementCost, Cost, Price);
    END;
END;

IF OBJECT_ID('dbo.ExtCentral_Item', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ExtCentral_Item_ItemID' AND object_id = OBJECT_ID('dbo.ExtCentral_Item'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_ExtCentral_Item_ItemID
            ON dbo.ExtCentral_Item (ItemID)
            INCLUDE (FamilyID, SubCategoryID);
    END;
END;

IF OBJECT_ID('dbo.ItemExt', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ItemExt_ItemId' AND object_id = OBJECT_ID('dbo.ItemExt'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_ItemExt_ItemId
            ON dbo.ItemExt (ItemId);
    END;
END;

IF OBJECT_ID('dbo.ItemCustomProperty', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ItemCustomProperty_Name' AND object_id = OBJECT_ID('dbo.ItemCustomProperty'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_ItemCustomProperty_Name
            ON dbo.ItemCustomProperty (Name)
            INCLUDE (ID, Type, Inactive);
    END;
END;

IF OBJECT_ID('dbo.Supplier', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Supplier_Code_List' AND object_id = OBJECT_ID('dbo.Supplier'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_Supplier_Code_List
            ON dbo.Supplier (Code)
            INCLUDE (SupplierName, ID);
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Supplier_Name_List' AND object_id = OBJECT_ID('dbo.Supplier'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_Supplier_Name_List
            ON dbo.Supplier (SupplierName)
            INCLUDE (Code, ID);
    END;o
END;

IF OBJECT_ID('dbo.ExtCentral_Family', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ExtCentral_Family_Code' AND object_id = OBJECT_ID('dbo.ExtCentral_Family'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_ExtCentral_Family_Code
            ON dbo.ExtCentral_Family (Code)
            INCLUDE (Name, ID);
    END;
END;

IF OBJECT_ID('dbo.Department', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Department_Code' AND object_id = OBJECT_ID('dbo.Department'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_Department_Code
            ON dbo.Department (Code)
            INCLUDE (Name, ID);
    END;
END;

IF OBJECT_ID('dbo.Category', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Category_Department_Code' AND object_id = OBJECT_ID('dbo.Category'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_Category_Department_Code
            ON dbo.Category (DepartmentID, Code)
            INCLUDE (Name, ID);
    END;
END;

IF OBJECT_ID('dbo.ExtCentral_SubCategory', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SubCategory_Category_Code' AND object_id = OBJECT_ID('dbo.ExtCentral_SubCategory'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_SubCategory_Category_Code
            ON dbo.ExtCentral_SubCategory (CategoryID, Code)
            INCLUDE (Description, ID);
    END;
END;

IF OBJECT_ID('dbo.QuantityDiscount', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuantityDiscount_Description' AND object_id = OBJECT_ID('dbo.QuantityDiscount'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_QuantityDiscount_Description
            ON dbo.QuantityDiscount ([Description], ID)
            INCLUDE ([Type], DiscountOddItems);
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuantityDiscount_Type_Description' AND object_id = OBJECT_ID('dbo.QuantityDiscount'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_QuantityDiscount_Type_Description
            ON dbo.QuantityDiscount ([Type], [Description], ID)
            INCLUDE (DiscountOddItems);
    END;
END;

IF OBJECT_ID('dbo.POD_Receipt', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_POD_Receipt_DatePosted_List' AND object_id = OBJECT_ID('dbo.POD_Receipt'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_POD_Receipt_DatePosted_List
            ON dbo.POD_Receipt (DatePosted DESC, ID DESC)
            INCLUDE (Number, Reference, SupplierDocNo, SupplierID, UserID, TotalAmount, OrderDate, RequiredDate, PostingComment);
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_POD_Receipt_Number' AND object_id = OBJECT_ID('dbo.POD_Receipt'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_POD_Receipt_Number
            ON dbo.POD_Receipt (Number)
            INCLUDE (DatePosted, SupplierID, UserID, Reference, SupplierDocNo, TotalAmount);
    END;
END;

IF OBJECT_ID('dbo.AVS_INTEGRAFAST_01', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AVSIntegrafast_Store_Date_Number' AND object_id = OBJECT_ID('dbo.AVS_INTEGRAFAST_01'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_AVSIntegrafast_Store_Date_Number
            ON dbo.AVS_INTEGRAFAST_01 (COD_SUCURSAL, FECHA_TRANSAC DESC, TRANSACTIONNUMBER DESC)
            INCLUDE (CLAVE50, COMPROBANTE_TIPO, NOMBRE_CLIENTE, FECHA_HACIENDA, ESTADO_HACIENDA);
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AVSIntegrafast_Number_Store' AND object_id = OBJECT_ID('dbo.AVS_INTEGRAFAST_01'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_AVSIntegrafast_Number_Store
            ON dbo.AVS_INTEGRAFAST_01 (TRANSACTIONNUMBER, COD_SUCURSAL)
            INCLUDE (FECHA_TRANSAC, CLAVE50, COMPROBANTE_TIPO, NOMBRE_CLIENTE, FECHA_HACIENDA, ESTADO_HACIENDA);
    END;
END;

IF OBJECT_ID('dbo.POD_ReceiptEntry', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_POD_ReceiptEntry_Receipt_Line' AND object_id = OBJECT_ID('dbo.POD_ReceiptEntry'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_POD_ReceiptEntry_Receipt_Line
            ON dbo.POD_ReceiptEntry (ReceiptID, LineNumber)
            INCLUDE (EntryID, Description, Quantity, QtyInvoiced, UnitCost, LineDiscRate, Comment);
    END;
END;

IF OBJECT_ID('dbo.ExtCentral_DocumentoInventario', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ExtCentral_DocInv_Status_Date' AND object_id = OBJECT_ID('dbo.ExtCentral_DocumentoInventario'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_ExtCentral_DocInv_Status_Date
            ON dbo.ExtCentral_DocumentoInventario (Estado, FechaSolicitud DESC, ID DESC)
            INCLUDE (Numero, Tipo, ProveedorCodigo, ProveedorNombre, FacturaRef, FechaEntrega, FechaAplicacion, Total, PersonaSolicita);
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ExtCentral_DocInv_Numero' AND object_id = OBJECT_ID('dbo.ExtCentral_DocumentoInventario'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_ExtCentral_DocInv_Numero
            ON dbo.ExtCentral_DocumentoInventario (Numero)
            INCLUDE (Tipo, Estado, FechaSolicitud, FechaAplicacion, Total, PersonaSolicita, ProveedorCodigo, ProveedorNombre, FacturaRef);
    END;
END;

IF OBJECT_ID('dbo.ExtCentral_DocumentoInventarioDetalle', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ExtCentral_DocInvDetalle_Documento_Line' AND object_id = OBJECT_ID('dbo.ExtCentral_DocumentoInventarioDetalle'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_ExtCentral_DocInvDetalle_Documento_Line
            ON dbo.ExtCentral_DocumentoInventarioDetalle (DocumentoID, LineNumber)
            INCLUDE (Codigo, Descripcion, Unidad, Cantidad, CantidadSolicitada, CantidadRecibida, CantidadPendiente, CostoUnitario, DescuentoPorcentaje, DescuentoMonto, ImpuestoPorcentaje, TotalLinea, Regalia, Observacion);
    END;
END;

IF OBJECT_ID('dbo.Worksheet', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Worksheet_ID_EffectiveDate' AND object_id = OBJECT_ID('dbo.Worksheet'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_Worksheet_ID_EffectiveDate
            ON dbo.Worksheet (ID DESC, EffectiveDate)
            INCLUDE (Style, Status, Title, Notes, FromDate);
    END;
END;

IF OBJECT_ID('dbo.ExtCentral_Worksheet', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ExtCentral_Worksheet_WorksheetID' AND object_id = OBJECT_ID('dbo.ExtCentral_Worksheet'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_ExtCentral_Worksheet_WorksheetID
            ON dbo.ExtCentral_Worksheet (WorksheetID)
            INCLUDE (WizardTaskCode, WorksheetContentID, HQUserID, [Date], WorsheetTitle);
    END;
END;

IF OBJECT_ID('dbo.WorksheetStore', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WorksheetStore_Worksheet_Store' AND object_id = OBJECT_ID('dbo.WorksheetStore'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_WorksheetStore_Worksheet_Store
            ON dbo.WorksheetStore (WorksheetID, StoreID)
            INCLUDE (DateProcessed, Status);
    END;
END;
