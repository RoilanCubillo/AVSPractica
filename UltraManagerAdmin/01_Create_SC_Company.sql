-- ============================================================
-- SCRIPT 01: Crear tabla SC_Company (Registro de Tenants)
-- Base de datos destino: ejecutar conectado a la base de seguridad requerida
-- ============================================================

-- Verificar si la tabla ya existe antes de crearla
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SC_Company' AND type = 'U')
BEGIN

    CREATE TABLE [dbo].[SC_Company]
    (
        [ID_Company]        INT             NOT NULL IDENTITY(1,1),
        [Code]              NVARCHAR(50)    NOT NULL,
        [Name]              NVARCHAR(200)   NOT NULL,
        [Connection_String] NVARCHAR(1000)  NOT NULL,
        [Enable]            BIT             NOT NULL CONSTRAINT [DF_SC_Company_Enable] DEFAULT (1),

        CONSTRAINT [PK_SC_Company]      PRIMARY KEY CLUSTERED ([ID_Company] ASC),
        CONSTRAINT [UQ_SC_Company_Code] UNIQUE NONCLUSTERED ([Code])
    );

    PRINT 'Tabla SC_Company creada correctamente.';
END
ELSE
BEGIN
    PRINT 'La tabla SC_Company ya existe. Script omitido.';
END
GO

-- Comentarios descriptivos en columnas
EXEC sys.sp_addextendedproperty
    @name = N'MS_Description', @value = N'Identificador único del tenant/empresa',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'SC_Company',
    @level2type = N'COLUMN', @level2name = N'ID_Company';

EXEC sys.sp_addextendedproperty
    @name = N'MS_Description', @value = N'Código corto único de la empresa (ej: EMP001)',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'SC_Company',
    @level2type = N'COLUMN', @level2name = N'Code';

EXEC sys.sp_addextendedproperty
    @name = N'MS_Description', @value = N'Cadena de conexión a la base de datos operativa del tenant',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'SC_Company',
    @level2type = N'COLUMN', @level2name = N'Connection_String';
GO
