-- DATABASE UPDATE SCRIPT FOR PRODUCT ENHANCEMENTS
-- This script adds new columns to the Products table

-- Check if Image column exists, if not add it
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Products' AND COLUMN_NAME = 'Image')
BEGIN
    ALTER TABLE Products ADD Image VARCHAR(500) NULL;
    PRINT 'Added Image column to Products table';
END
ELSE
BEGIN
    PRINT 'Image column already exists in Products table';
END
GO

-- Check if IsActive column exists, if not add it
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Products' AND COLUMN_NAME = 'IsActive')
BEGIN
    ALTER TABLE Products ADD IsActive BIT NOT NULL DEFAULT 1;
    PRINT 'Added IsActive column to Products table';
END
ELSE
BEGIN
    PRINT 'IsActive column already exists in Products table';
END
GO

-- Update existing products to be active
UPDATE Products SET IsActive = 1 WHERE IsActive IS NULL;
GO

PRINT 'Database update completed successfully!';
GO
