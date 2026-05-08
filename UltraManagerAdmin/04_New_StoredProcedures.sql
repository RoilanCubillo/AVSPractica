-- ============================================================
-- SCRIPT 04: Stored Procedures para arquitectura Multitenant
-- Base de datos destino: ejecutar conectado a la base de seguridad requerida
-- ============================================================

-- ─────────────────────────────────────────────────────────────
-- SP: SC_COMPANY_GET — Lista todas las empresas activas
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_COMPANY_GET', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SC_COMPANY_GET];
GO

CREATE PROCEDURE [dbo].[SC_COMPANY_GET]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [ID_Company],
        [Code],
        [Name],
        [Connection_String],
        [Enable]
    FROM [dbo].[SC_Company]
    WHERE [Enable] = 1
    ORDER BY [Name] ASC;
END
GO

-- ─────────────────────────────────────────────────────────────
-- SP: SC_COMPANY_GET_BY_ID — Obtiene datos de una empresa por ID
--     Utilizado por el CompanyConnectionProvider en C#
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_COMPANY_GET_BY_ID', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SC_COMPANY_GET_BY_ID];
GO

CREATE PROCEDURE [dbo].[SC_COMPANY_GET_BY_ID]
    @ID_Company INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [ID_Company],
        [Code],
        [Name],
        [Connection_String],
        [Enable]
    FROM [dbo].[SC_Company]
    WHERE [ID_Company] = @ID_Company
      AND [Enable] = 1;
END
GO

-- ─────────────────────────────────────────────────────────────
-- SP: SC_COMPANY_GET_BY_USER — lista las empresas activas
--     vinculadas a un usuario dentro de un sistema.
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_COMPANY_GET_BY_USER', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SC_COMPANY_GET_BY_USER];
GO

CREATE PROCEDURE [dbo].[SC_COMPANY_GET_BY_USER]
    @Account  NVARCHAR(100),
    @AutoID   INT,
    @SystemID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- @AutoID se conserva por compatibilidad con clientes actuales,
    -- pero la resolución de empresas vinculadas se hace por Account + SystemID.

    SELECT DISTINCT
        c.[ID_Company],
        c.[Code],
        c.[Name],
        c.[Connection_String],
        c.[Enable]
    FROM [dbo].[SC_User] u
    INNER JOIN [dbo].[SC_Company] c
        ON c.[ID_Company] = u.[ID_Company]
    WHERE u.[Account] = @Account
      AND u.[SystemID] = @SystemID
      AND u.[Enable] = 1
      AND c.[Enable] = 1
    ORDER BY c.[Name] ASC;
END
GO

-- ─────────────────────────────────────────────────────────────
-- SP: SC_USER_VALIDATE_COMPANY — Valida usuario filtrado por empresa
--     Reemplaza/complementa a SC_USER_VALIDATE en contexto multitenant
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_USER_VALIDATE_COMPANY', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SC_USER_VALIDATE_COMPANY];
GO

CREATE PROCEDURE [dbo].[SC_USER_VALIDATE_COMPANY]
    @Account    NVARCHAR(100),
    @AutoID     INT,
    @SystemID   INT,
    @ID_Company INT
AS
BEGIN
    SET NOCOUNT ON;

    -- @AutoID se conserva por compatibilidad con clientes actuales,
    -- pero la validación multitenant ya no filtra por este campo.

    SELECT
        u.[ID],
        u.[AutoID],
        u.[SystemID],
        u.[Account],
        u.[Name],
        u.[EnCloseSession]  AS [EnCloseSession],
        u.[Enable],
        u.[ID_Company]
    FROM [dbo].[SC_User] u
    WHERE u.[Account]    = @Account
      AND u.[SystemID]   = @SystemID
      AND u.[ID_Company] = @ID_Company
      AND u.[Enable]     = 1;
END
GO

-- ─────────────────────────────────────────────────────────────
-- SP: SC_USER_VALIDATE_SESSION_COMPANY — Valida sesión activa
--     con contexto de empresa
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_USER_VALIDATE_SESSION_COMPANY', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SC_USER_VALIDATE_SESSION_COMPANY];
GO

CREATE PROCEDURE [dbo].[SC_USER_VALIDATE_SESSION_COMPANY]
    @ID         INT,
    @AutoID     INT,
    @SystemID   INT,
    @ID_Company INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT u.[ID]
    FROM [dbo].[SC_User] u
    WHERE u.[ID]         = @ID
      AND u.[AutoID]     = @AutoID
      AND u.[SystemID]   = @SystemID
      AND u.[ID_Company] = @ID_Company
      AND u.[Enable]     = 1;
END
GO

-- ─────────────────────────────────────────────────────────────
-- SP: SC_MODULE_VALIDATE_GET — valida módulos por usuario
--     respetando el contexto de empresa del registro SC_User.
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_MODULE_VALIDATE_GET', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SC_MODULE_VALIDATE_GET];
GO

CREATE PROCEDURE [dbo].[SC_MODULE_VALIDATE_GET]
    @SYSTEM_CODE VARCHAR(30),
    @USER_ID     INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT M.*
    FROM [dbo].[SC_User] U
    INNER JOIN [dbo].[SC_System] S
        ON S.[Code] = @SYSTEM_CODE
       AND S.[ID] = U.[SystemID]
    INNER JOIN [dbo].[SC_UserRole] UR
        ON UR.[UserID] = U.[ID]
       AND ((UR.[ID_Company] = U.[ID_Company]) OR (UR.[ID_Company] IS NULL AND U.[ID_Company] IS NULL))
    INNER JOIN [dbo].[SC_Role] R
        ON R.[ID] = UR.[RoleID]
       AND R.[SystemID] = U.[SystemID]
       AND ((R.[ID_Company] = U.[ID_Company]) OR (R.[ID_Company] IS NULL AND U.[ID_Company] IS NULL))
    INNER JOIN [dbo].[SC_Module] M
        ON M.[ID] IN (
            SELECT TRY_CAST(LTRIM(RTRIM([value])) AS INT)
            FROM STRING_SPLIT(R.[EnModulesID], ',')
            WHERE LTRIM(RTRIM([value])) <> ''
        )
       AND M.[SystemID] = U.[SystemID]
       AND ((M.[ID_Company] = U.[ID_Company]) OR (M.[ID_Company] IS NULL AND U.[ID_Company] IS NULL))
    WHERE U.[ID] = @USER_ID
      AND U.[Enable] = 1
      AND U.[EnCloseSession] = 0
      AND R.[Enable] = 1
      AND M.[Enable] = 1;
END
GO

-- ─────────────────────────────────────────────────────────────
-- SP: SC_VIEW_VALIDATE_GET — valida vistas por usuario
--     respetando el contexto de empresa del registro SC_User.
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_VIEW_VALIDATE_GET', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SC_VIEW_VALIDATE_GET];
GO

CREATE PROCEDURE [dbo].[SC_VIEW_VALIDATE_GET]
    @USER_ID     INT,
    @SYSTEM_CODE VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT V.*
    FROM [dbo].[SC_View] V
    INNER JOIN [dbo].[SC_Module] M
        ON M.[ID] = V.[MuduleID]
    INNER JOIN [dbo].[SC_System] S
        ON S.[ID] = M.[SystemID]
    INNER JOIN [dbo].[SC_Role] R
        ON R.[SystemID] = S.[ID]
       AND ((R.[ID_Company] = M.[ID_Company]) OR (R.[ID_Company] IS NULL AND M.[ID_Company] IS NULL))
    INNER JOIN [dbo].[SC_UserRole] UR
        ON UR.[RoleID] = R.[ID]
       AND ((UR.[ID_Company] = R.[ID_Company]) OR (UR.[ID_Company] IS NULL AND R.[ID_Company] IS NULL))
    INNER JOIN [dbo].[SC_User] U
        ON U.[ID] = UR.[UserID]
       AND U.[SystemID] = S.[ID]
       AND ((U.[ID_Company] = UR.[ID_Company]) OR (U.[ID_Company] IS NULL AND UR.[ID_Company] IS NULL))
    WHERE U.[ID] = @USER_ID
      AND S.[Code] = @SYSTEM_CODE
      AND V.[ID] IN (
            SELECT TRY_CAST(LTRIM(RTRIM([value])) AS INT)
            FROM STRING_SPLIT(R.[EnViewsID], ',')
            WHERE LTRIM(RTRIM([value])) <> ''
      )
      AND M.[ID] IN (
            SELECT TRY_CAST(LTRIM(RTRIM([value])) AS INT)
            FROM STRING_SPLIT(R.[EnModulesID], ',')
            WHERE LTRIM(RTRIM([value])) <> ''
      )
      AND U.[Enable] = 1
      AND U.[EnCloseSession] = 0
      AND R.[Enable] = 1
      AND M.[Enable] = 1
      AND V.[Enable] = 1;
END
GO

-- ─────────────────────────────────────────────────────────────
-- SP: SC_DATAACCESS_VALIDATE — valida accesos a datos por
--     usuario respetando el contexto de empresa del SC_User.
-- ─────────────────────────────────────────────────────────────
IF OBJECT_ID('dbo.SC_DATAACCESS_VALIDATE', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SC_DATAACCESS_VALIDATE];
GO

CREATE PROCEDURE [dbo].[SC_DATAACCESS_VALIDATE]
    @DATA_CODE   VARCHAR(30),
    @USER_ID     INT,
    @SYSTEM_CODE VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        D.[ID]            AS [DataID],
        D.[Code]          AS [DataCode],
        D.[Description]   AS [DataDescription],
        D.[TableName]     AS [DataTableName],
        D.[TablePKName]   AS [DataTablePKName],
        D.[SystemID]      AS [DataSystemID],
        D.[Enable]        AS [DataEnable],
        DA.[ID]           AS [DataAccessID],
        DA.[Description]  AS [DataAccessDescription],
        DA.[EnAll]        AS [DataAccessEnAll],
        DA.[DataIDs]      AS [DataAccessDataIDs],
        DA.[Enable]       AS [DataAccessEnable]
    FROM [dbo].[SC_Data] D
    INNER JOIN [dbo].[SC_DataAccess] DA
        ON DA.[DataID] = D.[ID]
    INNER JOIN [dbo].[SC_RoleDataAccess] RDA
        ON RDA.[DataAccessID] = DA.[ID]
    INNER JOIN [dbo].[SC_System] S
        ON S.[ID] = D.[SystemID]
    INNER JOIN [dbo].[SC_Role] R
        ON R.[ID] = RDA.[RoleID]
       AND R.[SystemID] = S.[ID]
    INNER JOIN [dbo].[SC_UserRole] UR
        ON UR.[RoleID] = R.[ID]
       AND ((UR.[ID_Company] = R.[ID_Company]) OR (UR.[ID_Company] IS NULL AND R.[ID_Company] IS NULL))
    INNER JOIN [dbo].[SC_User] U
        ON U.[ID] = UR.[UserID]
       AND U.[SystemID] = S.[ID]
       AND ((U.[ID_Company] = UR.[ID_Company]) OR (U.[ID_Company] IS NULL AND UR.[ID_Company] IS NULL))
    WHERE S.[Code] = @SYSTEM_CODE
      AND D.[Code] LIKE @DATA_CODE
      AND U.[ID] = @USER_ID
      AND D.[Enable] = 1
      AND DA.[Enable] = 1
      AND S.[Enabled] = 1
      AND R.[Enable] = 1
      AND U.[Enable] = 1;
END
GO

