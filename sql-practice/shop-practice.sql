/* =====================================================================
   SQL Server / T-SQL practice  —  Topic 13 (data deep-dive)
   ---------------------------------------------------------------------
   Complements Topic 3: there EF Core GENERATED SQL against SQLite;
   here we write raw T-SQL by hand against real SQL Server (LocalDB).

   Run:
     sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i sql-practice\shop-practice.sql

   Safe to re-run: it drops and recreates the ShopPractice database.
   ===================================================================== */
SET NOCOUNT ON;   -- suppress the "(N rows affected)" chatter on writes
GO

/* ---------- 0. (Re)create the practice database --------------------- */
USE master;
GO
IF DB_ID('ShopPractice') IS NOT NULL
BEGIN
    -- kick any other sessions so DROP can proceed
    ALTER DATABASE ShopPractice SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ShopPractice;
END
GO
CREATE DATABASE ShopPractice;
GO
USE ShopPractice;
GO

/* ---------- 1. Schema: five related tables -------------------------- */
-- PRIMARY KEY on an IDENTITY column => a clustered index by default.
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name       NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Products (
    ProductId  INT IDENTITY(1,1) PRIMARY KEY,
    Name       NVARCHAR(100) NOT NULL,
    Price      DECIMAL(10,2) NOT NULL CHECK (Price > 0),   -- money => DECIMAL, never FLOAT
    CategoryId INT NOT NULL
        CONSTRAINT FK_Products_Categories REFERENCES Categories(CategoryId)
);

CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    Name       NVARCHAR(100) NOT NULL,
    City       NVARCHAR(50)  NULL
);

CREATE TABLE Orders (
    OrderId    INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL
        CONSTRAINT FK_Orders_Customers REFERENCES Customers(CustomerId),
    OrderDate  DATE NOT NULL
);

CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT NOT NULL
        CONSTRAINT FK_OrderItems_Orders REFERENCES Orders(OrderId),
    ProductId   INT NOT NULL
        CONSTRAINT FK_OrderItems_Products REFERENCES Products(ProductId),
    Quantity    INT NOT NULL CHECK (Quantity > 0),
    UnitPrice   DECIMAL(10,2) NOT NULL   -- price captured AT ORDER TIME (may differ from current)
);
GO

/* ---------- 2. Seed data -------------------------------------------- */
INSERT INTO Categories (Name) VALUES
    (N'Electronics'), (N'Books'), (N'Home'), (N'Toys');   -- Toys stays empty (for the LEFT JOIN demo)

INSERT INTO Products (Name, Price, CategoryId) VALUES
    (N'Laptop',       1200.00, 1),
    (N'Headphones',    150.00, 1),
    (N'Smartphone',    900.00, 1),
    (N'C# in Depth',    45.00, 2),
    (N'SQL Cookbook',   38.00, 2),
    (N'Desk Lamp',      28.50, 3),
    (N'Coffee Maker',   85.00, 3);

INSERT INTO Customers (Name, City) VALUES
    (N'Ava', N'Seattle'), (N'Ben', N'Portland'), (N'Cara', N'Seattle');

INSERT INTO Orders (CustomerId, OrderDate) VALUES
    (1, '2026-07-01'), (1, '2026-07-15'), (2, '2026-07-20');

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES
    (1, 1, 1, 1200.00),   -- Ava: 1 Laptop
    (1, 2, 2,  150.00),   -- Ava: 2 Headphones
    (2, 4, 1,   45.00),   -- Ava: 1 C# in Depth
    (3, 3, 2,  900.00);   -- Ben: 2 Smartphones
GO

PRINT '';
PRINT '========== 2b. Table contents (all rows in each table) ==========';
PRINT '--- Categories ---';
SELECT * FROM Categories;
PRINT '--- Products ---';
SELECT * FROM Products;
PRINT '--- Customers ---';
SELECT * FROM Customers;
PRINT '--- Orders ---';
SELECT * FROM Orders;
PRINT '--- OrderItems ---';
SELECT * FROM OrderItems;
GO

PRINT '';
PRINT '========== 3. Basic SELECT / WHERE / ORDER BY ==========';
SELECT ProductId, Name, Price
FROM Products
WHERE Price >= 100
ORDER BY Price DESC;
GO

PRINT '========== 4. INNER JOIN (product + its category) ==========';
SELECT p.Name AS Product, c.Name AS Category, p.Price
FROM Products p
JOIN Categories c ON c.CategoryId = p.CategoryId
ORDER BY c.Name, p.Price DESC;
GO

PRINT '========== 5. LEFT JOIN (every category, even with 0 products) ==========';
-- COUNT(p.ProductId) counts non-NULLs, so empty categories correctly show 0.
SELECT c.Name AS Category, COUNT(p.ProductId) AS ProductCount
FROM Categories c
LEFT JOIN Products p ON p.CategoryId = c.CategoryId
GROUP BY c.Name
ORDER BY ProductCount DESC, c.Name;
GO

PRINT '========== 6. GROUP BY + HAVING + aggregates ==========';
-- HAVING filters AFTER grouping (WHERE filters rows BEFORE grouping).
SELECT c.Name AS Category, COUNT(*) AS Items, AVG(p.Price) AS AvgPrice
FROM Products p
JOIN Categories c ON c.CategoryId = p.CategoryId
GROUP BY c.Name
HAVING AVG(p.Price) > 50
ORDER BY AvgPrice DESC;
GO

PRINT '========== 7. Subquery / NOT EXISTS (products never ordered) ==========';
SELECT p.Name
FROM Products p
WHERE NOT EXISTS (SELECT 1 FROM OrderItems oi WHERE oi.ProductId = p.ProductId)
ORDER BY p.Name;
GO

PRINT '========== 8. CTE + revenue per product ==========';
WITH ProductRevenue AS (
    SELECT oi.ProductId, SUM(oi.Quantity * oi.UnitPrice) AS Revenue
    FROM OrderItems oi
    GROUP BY oi.ProductId
)
SELECT p.Name, r.Revenue
FROM ProductRevenue r
JOIN Products p ON p.ProductId = r.ProductId
ORDER BY r.Revenue DESC;
GO

PRINT '========== 9. Window functions (rank within category; running total) ==========';
-- ROW_NUMBER over a PARTITION: rank each product by price WITHIN its category.
SELECT c.Name AS Category, p.Name AS Product, p.Price,
       ROW_NUMBER() OVER (PARTITION BY p.CategoryId ORDER BY p.Price DESC) AS PriceRankInCat
FROM Products p
JOIN Categories c ON c.CategoryId = p.CategoryId
ORDER BY Category, PriceRankInCat;

-- Running total of order revenue over time (SUM() OVER with an ordered frame).
WITH OrderRevenue AS (
    SELECT o.OrderId, o.OrderDate, SUM(oi.Quantity * oi.UnitPrice) AS Revenue
    FROM Orders o
    JOIN OrderItems oi ON oi.OrderId = o.OrderId
    GROUP BY o.OrderId, o.OrderDate
)
SELECT OrderId, OrderDate, Revenue,
       SUM(Revenue) OVER (ORDER BY OrderDate, OrderId
                          ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS RunningTotal
FROM OrderRevenue
ORDER BY OrderDate, OrderId;
GO

PRINT '========== 10. Pagination (OFFSET / FETCH) — page 2, size 2 ==========';
-- OFFSET/FETCH REQUIRES an ORDER BY. This is what EF Core emits for Skip/Take.
SELECT ProductId, Name, Price
FROM Products
ORDER BY Price DESC
OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY;
GO

PRINT '========== 11. Index + IO statistics ==========';
-- Nonclustered index with an INCLUDE column => a covering index for this query.
CREATE NONCLUSTERED INDEX IX_Products_CategoryId ON Products(CategoryId) INCLUDE (Price);
SET STATISTICS IO ON;
SELECT CategoryId, COUNT(*) AS Items, SUM(Price) AS TotalPrice
FROM Products
WHERE CategoryId = 1
GROUP BY CategoryId;
SET STATISTICS IO OFF;
GO

PRINT '========== 12. Stored procedure ==========';
GO
CREATE OR ALTER PROCEDURE usp_GetProductsByCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProductId, Name, Price
    FROM Products
    WHERE CategoryId = @CategoryId
    ORDER BY Price DESC;
END
GO
EXEC usp_GetProductsByCategory @CategoryId = 1;
GO

PRINT '========== 13. Transaction with explicit isolation ==========';
-- All-or-nothing: the order header and its line item commit together, or neither does.
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
BEGIN TRY
    BEGIN TRAN;
        INSERT INTO Orders (CustomerId, OrderDate) VALUES (3, '2026-07-29');
        DECLARE @newOrderId INT = SCOPE_IDENTITY();   -- id of the row THIS statement inserted
        INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
            VALUES (@newOrderId, 5, 3, 38.00);        -- 3x SQL Cookbook
    COMMIT;
    PRINT 'Transaction committed. New OrderId = ' + CAST(@newOrderId AS VARCHAR(10));
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    PRINT 'Transaction rolled back: ' + ERROR_MESSAGE();
END CATCH;
GO

PRINT '========== Done. ==========';
GO
