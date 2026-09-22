-- =========================================================
-- 04_AddProductImage.sql
-- Migration for databases created BEFORE the product-image feature:
-- adds the image_path column to an existing Products table.
--
-- Safe to run any number of times (checks for the column first).
-- Not needed on a brand-new install - 02_CreateTables.sql already
-- creates the column, so only run this against an older database.
-- =========================================================

USE MiniMartDb;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Products') AND name = 'image_path'
)
BEGIN
    ALTER TABLE Products ADD image_path NVARCHAR(500) NULL;
END
GO
