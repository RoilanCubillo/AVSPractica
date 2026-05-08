-- ============================================================
-- SCRIPT 03: Tenantizar tablas de seguridad con ID_Company
-- Afecta a SC_User, SC_Role, SC_Module y SC_UserRole
-- Base de datos destino: ejecutar conectado a la base de seguridad requerida
-- ============================================================

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 1: Agregar ID_Company a SC_User
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_User', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.SC_User')
      AND name = 'ID_Company'
)
BEGIN
    ALTER TABLE [dbo].[SC_User]
        ADD [ID_Company] INT NULL;

    PRINT 'Columna ID_Company agregada a SC_User.';
END
ELSE
BEGIN
    PRINT 'La columna ID_Company ya existe en SC_User o la tabla no existe. Paso omitido.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 2: Agregar FK entre SC_User y SC_Company
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_User', 'U') IS NOT NULL
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SC_Company')
AND NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_SC_User_SC_Company'
)
BEGIN
    ALTER TABLE [dbo].[SC_User]
        ADD CONSTRAINT [FK_SC_User_SC_Company]
        FOREIGN KEY ([ID_Company])
        REFERENCES [dbo].[SC_Company] ([ID_Company]);

    PRINT 'FK FK_SC_User_SC_Company creada correctamente.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 3: Agregar ID_Company a SC_Role
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_Role', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.SC_Role')
      AND name = 'ID_Company'
)
BEGIN
    ALTER TABLE [dbo].[SC_Role]
        ADD [ID_Company] INT NULL;

    PRINT 'Columna ID_Company agregada a SC_Role.';
END
ELSE
BEGIN
    PRINT 'La columna ID_Company ya existe en SC_Role o la tabla no existe. Paso omitido.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 4: Agregar FK entre SC_Role y SC_Company
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_Role', 'U') IS NOT NULL
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SC_Company')
AND NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_SC_Role_SC_Company'
)
BEGIN
    ALTER TABLE [dbo].[SC_Role]
        ADD CONSTRAINT [FK_SC_Role_SC_Company]
        FOREIGN KEY ([ID_Company])
        REFERENCES [dbo].[SC_Company] ([ID_Company]);

    PRINT 'FK FK_SC_Role_SC_Company creada correctamente.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 5: Agregar ID_Company a SC_Module
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_Module', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.SC_Module')
      AND name = 'ID_Company'
)
BEGIN
    ALTER TABLE [dbo].[SC_Module]
        ADD [ID_Company] INT NULL;

    PRINT 'Columna ID_Company agregada a SC_Module.';
END
ELSE
BEGIN
    PRINT 'La columna ID_Company ya existe en SC_Module o la tabla no existe. Paso omitido.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 6: Agregar FK entre SC_Module y SC_Company
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_Module', 'U') IS NOT NULL
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SC_Company')
AND NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_SC_Module_SC_Company'
)
BEGIN
    ALTER TABLE [dbo].[SC_Module]
        ADD CONSTRAINT [FK_SC_Module_SC_Company]
        FOREIGN KEY ([ID_Company])
        REFERENCES [dbo].[SC_Company] ([ID_Company]);

    PRINT 'FK FK_SC_Module_SC_Company creada correctamente.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 7: Agregar ID_Company a SC_UserRole
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_UserRole', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.SC_UserRole')
      AND name = 'ID_Company'
)
BEGIN
    ALTER TABLE [dbo].[SC_UserRole]
        ADD [ID_Company] INT NULL;

    PRINT 'Columna ID_Company agregada a SC_UserRole.';
END
ELSE
BEGIN
    PRINT 'La columna ID_Company ya existe en SC_UserRole o la tabla no existe. Paso omitido.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 8: Agregar FK entre SC_UserRole y SC_Company
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_UserRole', 'U') IS NOT NULL
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SC_Company')
AND NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_SC_UserRole_SC_Company'
)
BEGIN
    ALTER TABLE [dbo].[SC_UserRole]
        ADD CONSTRAINT [FK_SC_UserRole_SC_Company]
        FOREIGN KEY ([ID_Company])
        REFERENCES [dbo].[SC_Company] ([ID_Company]);

    PRINT 'FK FK_SC_UserRole_SC_Company creada correctamente.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 9: Migración asistida cuando existe una sola empresa
--         Si hay varias empresas, la asignación debe hacerse
--         por negocio antes de endurecer integridad adicional.
-- ─────────────────────────────────────────────────────────────
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SC_Company')
BEGIN
    DECLARE @CompanyCount INT;
    DECLARE @DefaultCompanyID INT;

    SELECT
        @CompanyCount = COUNT(*),
        @DefaultCompanyID = MIN([ID_Company])
    FROM [dbo].[SC_Company];

    IF @CompanyCount = 1
    BEGIN
        UPDATE [dbo].[SC_User]
        SET [ID_Company] = @DefaultCompanyID
        WHERE [ID_Company] IS NULL;
        PRINT CAST(@@ROWCOUNT AS VARCHAR(20)) + ' usuarios actualizados con la empresa por defecto.';

        UPDATE [dbo].[SC_Role]
        SET [ID_Company] = @DefaultCompanyID
        WHERE [ID_Company] IS NULL;
        PRINT CAST(@@ROWCOUNT AS VARCHAR(20)) + ' roles actualizados con la empresa por defecto.';

        UPDATE [dbo].[SC_Module]
        SET [ID_Company] = @DefaultCompanyID
        WHERE [ID_Company] IS NULL;
        PRINT CAST(@@ROWCOUNT AS VARCHAR(20)) + ' módulos actualizados con la empresa por defecto.';

        UPDATE [dbo].[SC_UserRole]
        SET [ID_Company] = @DefaultCompanyID
        WHERE [ID_Company] IS NULL;
        PRINT CAST(@@ROWCOUNT AS VARCHAR(20)) + ' relaciones usuario-rol actualizadas con la empresa por defecto.';
    END
    ELSE
    BEGIN
        UPDATE ur
        SET ur.[ID_Company] = u.[ID_Company]
        FROM [dbo].[SC_UserRole] ur
        INNER JOIN [dbo].[SC_User] u ON u.[ID] = ur.[UserID]
        WHERE ur.[ID_Company] IS NULL
          AND u.[ID_Company] IS NOT NULL;

        PRINT CAST(@@ROWCOUNT AS VARCHAR(20)) + ' relaciones SC_UserRole heredaron ID_Company desde SC_User.';
        PRINT 'Se detectaron múltiples empresas. Revise y complete manualmente ID_Company en SC_Role y SC_Module antes de cerrar la migración.';
    END
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 10: Reemplazar unicidad legacy de SC_User por versiones
--          filtradas (legacy y tenant)
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_User', 'U') IS NOT NULL
AND EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.SC_User')
      AND name = 'SC_User_Unq01'
)
BEGIN
    DROP INDEX [SC_User_Unq01] ON [dbo].[SC_User];
    PRINT 'Índice SC_User_Unq01 reemplazado por índices filtrados.';
END
GO

IF OBJECT_ID('dbo.SC_User', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.SC_User')
      AND name = 'SC_User_Unq01_Legacy'
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [SC_User_Unq01_Legacy]
        ON [dbo].[SC_User] ([Account] ASC, [AutoID] ASC, [SystemID] ASC)
        WHERE [ID_Company] IS NULL;

    PRINT 'Índice SC_User_Unq01_Legacy creado correctamente.';
END
GO

IF OBJECT_ID('dbo.SC_User', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.SC_User')
      AND name = 'SC_User_Unq01_Tenant'
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [SC_User_Unq01_Tenant]
        ON [dbo].[SC_User] ([Account] ASC, [SystemID] ASC, [ID_Company] ASC)
        WHERE [ID_Company] IS NOT NULL;

    PRINT 'Índice SC_User_Unq01_Tenant creado correctamente.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 11: Reemplazar unicidad legacy de SC_Role por versiones
--          filtradas (legacy y tenant)
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_Role', 'U') IS NOT NULL
AND EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.SC_Role')
      AND name = 'SC_Role_Unq01'
)
BEGIN
    DROP INDEX [SC_Role_Unq01] ON [dbo].[SC_Role];
    PRINT 'Índice SC_Role_Unq01 reemplazado por índices filtrados.';
END
GO

IF OBJECT_ID('dbo.SC_Role', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.SC_Role')
      AND name = 'SC_Role_Unq01_Legacy'
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [SC_Role_Unq01_Legacy]
        ON [dbo].[SC_Role] ([Code] ASC)
        WHERE [ID_Company] IS NULL;

    PRINT 'Índice SC_Role_Unq01_Legacy creado correctamente.';
END
GO

IF OBJECT_ID('dbo.SC_Role', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.SC_Role')
      AND name = 'SC_Role_Unq01_Tenant'
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [SC_Role_Unq01_Tenant]
        ON [dbo].[SC_Role] ([Code] ASC, [ID_Company] ASC)
        WHERE [ID_Company] IS NOT NULL;

    PRINT 'Índice SC_Role_Unq01_Tenant creado correctamente.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 12: Reemplazar unicidad legacy de SC_Module por versiones
--          filtradas (legacy y tenant)
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_Module', 'U') IS NOT NULL
AND EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.SC_Module')
      AND name = 'SC_Module_Unq_01'
)
BEGIN
    DROP INDEX [SC_Module_Unq_01] ON [dbo].[SC_Module];
    PRINT 'Índice SC_Module_Unq_01 reemplazado por índices filtrados.';
END
GO

IF OBJECT_ID('dbo.SC_Module', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.SC_Module')
      AND name = 'SC_Module_Unq_01_Legacy'
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [SC_Module_Unq_01_Legacy]
        ON [dbo].[SC_Module] ([SystemID] ASC, [Code] ASC)
        WHERE [ID_Company] IS NULL;

    PRINT 'Índice SC_Module_Unq_01_Legacy creado correctamente.';
END
GO

IF OBJECT_ID('dbo.SC_Module', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.SC_Module')
      AND name = 'SC_Module_Unq_01_Tenant'
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [SC_Module_Unq_01_Tenant]
        ON [dbo].[SC_Module] ([SystemID] ASC, [Code] ASC, [ID_Company] ASC)
        WHERE [ID_Company] IS NOT NULL;

    PRINT 'Índice SC_Module_Unq_01_Tenant creado correctamente.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- RESULTADO:
--   SC_User, SC_Role, SC_Module y SC_UserRole quedan preparados
--   para manejar seguridad por empresa.
--   SC_User valida unicidad tenant por (Account + SystemID + ID_Company).
--   SC_Role y SC_Module permiten repetir códigos entre empresas.
-- ─────────────────────────────────────────────────────────────
