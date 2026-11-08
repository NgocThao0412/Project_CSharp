/*
  Migration: Shared desktop/web accounts for the existing quanlykho database.
  Applies to the ORIGINAL schema or to an already-upgraded schema.
  Does not insert demo users or touch existing password values.

  PREREQUISITES:
  - Back up quanlykho and test this on a database copy FIRST.
  - Run once per teammate's EXISTING database in SSMS.
  - Backend registration/login must still be implemented separately.
*/
USE [quanlykho];
GO
SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.TAIKHOAN', N'U') IS NULL
        THROW 51001, N'Missing dbo.TAIKHOAN. Initialize original database first.', 1;
    IF OBJECT_ID(N'dbo.KHACHHANG', N'U') IS NULL
        THROW 51002, N'Missing dbo.KHACHHANG. Initialize original database first.', 1;
    IF OBJECT_ID(N'dbo.NHOMQUYEN', N'U') IS NULL
        THROW 51003, N'Missing dbo.NHOMQUYEN. Initialize original database first.', 1;

    -- Add generated account ID without changing legacy employee IDs.
    IF COL_LENGTH(N'dbo.TAIKHOAN', N'MaTK') IS NULL
        EXEC(N'ALTER TABLE dbo.TAIKHOAN ADD MaTK INT IDENTITY(1,1) NOT NULL;');

    IF COLUMNPROPERTY(OBJECT_ID(N'dbo.TAIKHOAN'), N'MaTK', 'IsIdentity') <> 1
        THROW 51004, N'MaTK exists but is not an IDENTITY column; inspect schema.', 1;

    DECLARE @oldPK sysname;
    DECLARE @pkIsMaTK bit = 0;
    SELECT @oldPK = kc.name
    FROM sys.key_constraints AS kc
    WHERE kc.parent_object_id = OBJECT_ID(N'dbo.TAIKHOAN') AND kc.type = 'PK';

    IF @oldPK IS NOT NULL
    BEGIN
        SELECT @pkIsMaTK = CASE WHEN COUNT(*) = 1 AND MAX(c.name) = N'MaTK' THEN 1 ELSE 0 END
        FROM sys.key_constraints AS kc
        JOIN sys.index_columns AS ic
          ON ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id AND ic.key_ordinal > 0
        JOIN sys.columns AS c
          ON c.object_id = ic.object_id AND c.column_id = ic.column_id
        WHERE kc.parent_object_id = OBJECT_ID(N'dbo.TAIKHOAN') AND kc.type = 'PK';
    END;

    IF @pkIsMaTK = 0
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE referenced_object_id = OBJECT_ID(N'dbo.TAIKHOAN'))
            THROW 51005, N'Other tables reference TAIKHOAN. Review dependent foreign keys before replacing PK.', 1;
        IF @oldPK IS NOT NULL
            EXEC(N'ALTER TABLE dbo.TAIKHOAN DROP CONSTRAINT ' + QUOTENAME(@oldPK) + N';');
        EXEC(N'ALTER TABLE dbo.TAIKHOAN ADD CONSTRAINT PK_TAIKHOAN_MaTK PRIMARY KEY (MaTK);');
    END;

    -- MKH is a nullable reference: existing employee accounts keep MKH=NULL.
    IF COL_LENGTH(N'dbo.TAIKHOAN', N'MKH') IS NULL
        EXEC(N'ALTER TABLE dbo.TAIKHOAN ADD MKH INT NULL;');

    -- The old MNV PK was NOT NULL. Customers need MNV=NULL.
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.TAIKHOAN') AND name = N'MNV' AND is_nullable = 0)
        EXEC(N'ALTER TABLE dbo.TAIKHOAN ALTER COLUMN MNV INT NULL;');

    IF NOT EXISTS (
        SELECT 1 FROM sys.foreign_key_columns AS fkc
        JOIN sys.foreign_keys AS fk ON fk.object_id = fkc.constraint_object_id
        JOIN sys.columns AS c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id
        WHERE fk.parent_object_id = OBJECT_ID(N'dbo.TAIKHOAN')
          AND fk.referenced_object_id = OBJECT_ID(N'dbo.KHACHHANG')
          AND c.name = N'MKH'
    )
        EXEC(N'ALTER TABLE dbo.TAIKHOAN WITH CHECK ADD CONSTRAINT FK_TAIKHOAN_KHACHHANG FOREIGN KEY (MKH) REFERENCES dbo.KHACHHANG(MKH);');

    -- Use the generated MNQ value instead of assuming it must be 5.
    IF NOT EXISTS (SELECT 1 FROM dbo.NHOMQUYEN WHERE TEN = N'Khách hàng')
        INSERT INTO dbo.NHOMQUYEN (TEN, TT) VALUES (N'Khách hàng', 1);

    -- Protect the intended employee/customer ownership model.
    -- Only add CHECK if the existing rows already comply.
    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.TAIKHOAN') AND name = N'CK_TAIKHOAN_ONE_OWNER')
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.TAIKHOAN WHERE (CASE WHEN MNV IS NULL THEN 0 ELSE 1 END + CASE WHEN MKH IS NULL THEN 0 ELSE 1 END) <> 1)
            THROW 51006, N'Some TAIKHOAN rows have zero or two owners. Fix data before adding CHECK constraint.', 1;
        EXEC(N'ALTER TABLE dbo.TAIKHOAN WITH CHECK ADD CONSTRAINT CK_TAIKHOAN_ONE_OWNER CHECK ((MNV IS NOT NULL AND MKH IS NULL) OR (MNV IS NULL AND MKH IS NOT NULL));');
    END;

    COMMIT TRANSACTION;
    PRINT N'Account schema migration finished successfully.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

-- Verify account PK, nullability, and customer role after migration.
SELECT kc.name AS PrimaryKeyName, c.name AS PrimaryKeyColumn
FROM sys.key_constraints AS kc
JOIN sys.index_columns AS ic ON ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id AND ic.key_ordinal > 0
JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE kc.parent_object_id = OBJECT_ID(N'dbo.TAIKHOAN') AND kc.type = 'PK';

SELECT c.name AS ColumnName, c.is_nullable AS AllowsNull
FROM sys.columns AS c
WHERE c.object_id = OBJECT_ID(N'dbo.TAIKHOAN')
  AND c.name IN (N'MaTK', N'MNV', N'MKH');

SELECT MNQ, TEN, TT FROM dbo.NHOMQUYEN WHERE TEN = N'Khách hàng';
GO
