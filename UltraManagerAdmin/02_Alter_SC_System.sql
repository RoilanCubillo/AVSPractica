-- ============================================================
-- SCRIPT 02: Modificar SC_System → solo catálogo
-- Migra Connection_String existente a SC_Company antes de eliminarla
-- Base de datos destino: ejecutar conectado a la base de seguridad requerida
-- ============================================================

-- ─────────────────────────────────────────────────────────────
-- PASO 1: Migrar datos de SC_System a SC_Company
--         (solo si SC_System tenía Connection_String y ya existe SC_Company)
-- ─────────────────────────────────────────────────────────────
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.SC_System')
      AND name = 'StringConnection'
)
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SC_Company')
BEGIN

    INSERT INTO [dbo].[SC_Company] ([Code], [Name], [Connection_String], [Enable])
    SELECT
        s.[Code],
        s.[Name],
        s.[StringConnection],
        s.[Enabled]
    FROM [dbo].[SC_System] s
    WHERE s.[StringConnection] IS NOT NULL
      AND LTRIM(RTRIM(s.[StringConnection])) <> ''
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[SC_Company] c WHERE c.[Code] = s.[Code]
      );

    PRINT CAST(@@ROWCOUNT AS VARCHAR) + ' registros migrados de SC_System a SC_Company.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- PASO 2: Eliminar StringConnection de SC_System
--         SC_System queda como catálogo puro de sistemas
-- ─────────────────────────────────────────────────────────────
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.SC_System')
      AND name = 'StringConnection'
)
BEGIN
    ALTER TABLE [dbo].[SC_System] DROP COLUMN [StringConnection];
    PRINT 'Columna StringConnection eliminada de SC_System.';
END
ELSE
BEGIN
    PRINT 'La columna StringConnection ya no existe en SC_System. Script omitido.';
END
GO

-- ─────────────────────────────────────────────────────────────
-- RESULTADO: SC_System queda con columnas:
--   ID, Code, Name, Enabled, IsUpdatingSystem
-- SC_System = catálogo de sistemas registrados
-- SC_Company = tenant registry con Connection_String
-- ─────────────────────────────────────────────────────────────
