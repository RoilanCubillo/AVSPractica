-- ============================================================
-- SCRIPT 05: Integración con RMH (Retail Management Hero)
-- Registra el código BC_SC (Business Company - Security) en la
-- base de datos RMH para vincular ID_Company de AVS_Security
-- ============================================================

-- ─────────────────────────────────────────────────────────────
-- EJECUTAR EN: Base de datos RMH
-- ─────────────────────────────────────────────────────────────
-- Ejecutar conectado a la base RMH del tenant correspondiente.

-- ─────────────────────────────────────────────────────────────
-- SP: SP_RMH_REGISTER_BCSC
--     Registra (o actualiza) el código BC_SC con el ID de empresa
--     de AVS_Security. El campo Valor es el puente entre ambos sistemas.
--
-- Tabla real detectada en RMH:
--   AVS_Parametros (CODIGO varchar, DESCRIPCION varchar, VALOR varchar)
-- ─────────────────────────────────────────────────────────────

IF OBJECT_ID('dbo.SP_RMH_REGISTER_BCSC', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_RMH_REGISTER_BCSC];
GO

CREATE PROCEDURE [dbo].[SP_RMH_REGISTER_BCSC]
    @ID_Company  INT,           -- ID_Company de AVS_Security (valor puente)
    @CompanyName NVARCHAR(200)  -- Nombre descriptivo de la empresa
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StrCompanyID NVARCHAR(20) = CAST(@ID_Company AS NVARCHAR(20));

    -- En este esquema RMH existe una sola fila BC_SC por base de datos.
    -- El campo VALOR almacena el ID_Company de AVS_SECURITY como puente.
    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[AVS_Parametros]
        WHERE [CODIGO] = 'BC_SC'
    )
    BEGIN
        -- Insertar nuevo código BC_SC vinculado al tenant
        INSERT INTO [dbo].[AVS_Parametros] ([CODIGO], [DESCRIPCION], [VALOR], [SyncGuid])
        VALUES (
            'BC_SC',
            'Código Compañía Seguridad',
            @StrCompanyID,
            NEWID()
        );

        SELECT 'INSERTED' AS [Result], @ID_Company AS [ID_Company];
    END
    ELSE
    BEGIN
        -- Actualizar descripción si ya existe
        UPDATE [dbo].[AVS_Parametros]
        SET [DESCRIPCION] = 'Código Compañía Seguridad',
            [VALOR] = @StrCompanyID
        WHERE [CODIGO] = 'BC_SC';

        SELECT 'UPDATED' AS [Result], @ID_Company AS [ID_Company];
    END
END
GO

-- ─────────────────────────────────────────────────────────────
-- USO DESDE AVS_Security:
--   El servicio C# RmhIntegrationService.RegisterCompanySecurity()
--   ejecuta este SP en la DB de RMH con el ID_Company correspondiente.
--
-- Flujo de enlace:
--   AVS_SECURITY.SC_Company.ID_Company  (clave origen)
--        ↓  (valor puente)
--   RMH.AVS_Parametros.VALOR WHERE CODIGO = 'BC_SC'
--
-- Ejemplo de registro resultante en RMH:
--   CODIGO      = 'BC_SC'
--   DESCRIPCION = 'Código Compañía Seguridad'
--   VALOR       = '3'          ← ID_Company en AVS_SECURITY
-- ─────────────────────────────────────────────────────────────

