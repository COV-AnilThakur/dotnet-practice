IF OBJECT_ID('dbo.Inventory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Inventory
    (
        InventoryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ProductId INT NOT NULL UNIQUE,
        StockQuantity INT NOT NULL DEFAULT 0,
        LastUpdated DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Inventory_Products_ProductId
            FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId)
    );
END
