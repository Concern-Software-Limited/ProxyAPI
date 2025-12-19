-- DATABASE UPDATE SCRIPT FOR PRODUCT ENHANCEMENTS (MySQL Version)
-- This script adds new columns to the Products table

USE proxydb;

-- Add Image column if it doesn't exist
SET @dbname = DATABASE();
SET @tablename = 'Products';
SET @columnname = 'Image';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      TABLE_SCHEMA = @dbname
      AND TABLE_NAME = @tablename
      AND COLUMN_NAME = @columnname
  ) > 0,
  'SELECT 1',
  CONCAT('ALTER TABLE ', @tablename, ' ADD COLUMN ', @columnname, ' VARCHAR(500) NULL')
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

SELECT 'Image column checked/added' AS Status;

-- Add IsActive column if it doesn't exist
SET @columnname = 'IsActive';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      TABLE_SCHEMA = @dbname
      AND TABLE_NAME = @tablename
      AND COLUMN_NAME = @columnname
  ) > 0,
  'SELECT 1',
  CONCAT('ALTER TABLE ', @tablename, ' ADD COLUMN ', @columnname, ' TINYINT(1) NOT NULL DEFAULT 1')
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

SELECT 'IsActive column checked/added' AS Status;

-- Update existing products to be active
UPDATE Products SET IsActive = 1 WHERE IsActive IS NULL OR IsActive = 0;

SELECT 'Database update completed successfully!' AS Status;
