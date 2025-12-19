-- 1. USERS TABLE (Authentication)
CREATE TABLE IF NOT EXISTS `proxydb`.`users` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `Name` VARCHAR(255) NULL,
  `Email` VARCHAR(255) NOT NULL UNIQUE,
  `PasswordHash` VARCHAR(255) NULL,
  `Role` ENUM('Admin', 'User') DEFAULT 'User',
  `CreatedAt` TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. USER DETAILS TABLE (Balance & Profile)
CREATE TABLE IF NOT EXISTS `proxydb`.`user_details` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `user_id` INT NOT NULL, -- Links to users.Id
  `first_name` VARCHAR(100) NULL,
  `last_name` VARCHAR(100) NULL,
  `email` VARCHAR(255) NOT NULL,
  `balance` DECIMAL(10,2) DEFAULT 0.00,
  `status` TINYINT(1) DEFAULT 1,
  `creation_date` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`user_id`) REFERENCES `users`(`Id`) ON DELETE CASCADE
);

-- 3. PRODUCTS TABLE (Parent Products like "ABC GB")
CREATE TABLE IF NOT EXISTS `proxydb`.`products` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `Name` VARCHAR(255) NOT NULL,
  `Description` TEXT NULL,
  `Image` VARCHAR(255) NULL,       -- Product Icon
  `IsActive` TINYINT(1) DEFAULT 1, -- Show/Hide Product
  `CreatedAt` TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 4. VARIANTS TABLE (Options like "1GB", "2GB")
CREATE TABLE IF NOT EXISTS `proxydb`.`variants` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `ProductId` INT NOT NULL,
  `Name` VARCHAR(100) NOT NULL,    -- e.g. "1GB"
  `Price` DECIMAL(10,2) NOT NULL,  -- e.g. 5.00
  `Sku` VARCHAR(50) NULL,          -- e.g. "ABC-1GB"
  `TotalKeys` INT DEFAULT 0,       -- Cache count
  `AvailableKeys` INT DEFAULT 0,   -- Cache count
  `SoldKeys` INT DEFAULT 0,        -- Cache count
  `CreatedAt` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`ProductId`) REFERENCES `products`(`Id`) ON DELETE CASCADE
);

-- 5. ORDERS TABLE (Transaction History)
CREATE TABLE IF NOT EXISTS `proxydb`.`orders` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `OrderNumber` VARCHAR(50) NULL UNIQUE, -- e.g. "ORD-2024-001"
  `UserId` INT NOT NULL,
  `TotalAmount` DECIMAL(10,2) NOT NULL,
  `Status` ENUM('Pending', 'Completed', 'Cancelled') DEFAULT 'Pending',
  `OrderDate` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`UserId`) REFERENCES `users`(`Id`)
);

-- 6. CDKEYS TABLE (The Core Logic for Dashboard)
CREATE TABLE IF NOT EXISTS `proxydb`.`cdkeys` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `ProductId` INT NOT NULL,
  `VariantId` INT NOT NULL,
  
  `KeyValue` TEXT NOT NULL,         -- The actual key
  `KeyHash` VARCHAR(64) NULL,       -- MD5/SHA256 for Duplicate Check
  
  `Status` ENUM('Available', 'Sold', 'Locked') DEFAULT 'Available',
  
  `AddedDate` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `SoldDate` TIMESTAMP NULL,
  
  `UserId` INT NULL,                -- Filled when sold
  `OrderId` INT NULL,               -- Filled when sold
  
  -- Constraints & Indexes
  FOREIGN KEY (`VariantId`) REFERENCES `variants`(`Id`),
  FOREIGN KEY (`OrderId`) REFERENCES `orders`(`Id`),
  UNIQUE INDEX `idx_key_hash` (`KeyHash`) -- PREVENTS DUPLICATES
);

-- 7. OTPS TABLE (Security)
CREATE TABLE IF NOT EXISTS `proxydb`.`otps` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `Email` VARCHAR(255) NOT NULL,
  `OtpCode` VARCHAR(10) NOT NULL,
  `IsUsed` TINYINT(1) DEFAULT 0,
  `CreatedAt` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `ExpiresAt` TIMESTAMP NULL
);