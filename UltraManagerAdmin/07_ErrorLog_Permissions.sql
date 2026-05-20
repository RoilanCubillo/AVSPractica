IF OBJECT_ID('dbo.UEP_ERROR_LOG', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UEP_ERROR_LOG
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UEP_ERROR_LOG PRIMARY KEY,
        UserID INT NOT NULL CONSTRAINT DF_UEP_ERROR_LOG_UserID DEFAULT (0),
        UserName NVARCHAR(150) NULL,
        UserAccount NVARCHAR(80) NULL,
        Screen NVARCHAR(120) NULL,
        ActionName NVARCHAR(120) NULL,
        ErrorMessage NVARCHAR(4000) NULL,
        ErrorDetail NVARCHAR(MAX) NULL,
        RequestData NVARCHAR(MAX) NULL,
        Url NVARCHAR(500) NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_UEP_ERROR_LOG_CreatedAt DEFAULT (GETDATE())
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_UEP_ERROR_LOG_CreatedAt'
      AND object_id = OBJECT_ID('dbo.UEP_ERROR_LOG')
)
BEGIN
    CREATE INDEX IX_UEP_ERROR_LOG_CreatedAt
        ON dbo.UEP_ERROR_LOG (CreatedAt DESC)
        INCLUDE (UserName, UserAccount, Screen, ActionName);
END;
GO
