-- =========================================================
-- 02_CreateTables.sql
-- Creates all MiniMart tables, keys, constraints and indexes.
-- Run this after 01_CreateDatabase.sql, connected to MiniMartDb.
-- =========================================================

USE MiniMartDb;
GO

-- =========================================================
-- Users
-- Every login account: both Admins and Employees.
-- =========================================================
CREATE TABLE Users
(
    user_id         INT IDENTITY(1,1)   NOT NULL,
    username        NVARCHAR(50)        NOT NULL,
    password_hash   NVARCHAR(200)       NOT NULL,   -- "iterations.salt.hash", never plain text
    role            NVARCHAR(20)        NOT NULL,
    is_active       BIT                 NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    created_at      DATETIME2           NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Users PRIMARY KEY (user_id),
    CONSTRAINT UQ_Users_Username UNIQUE (username),
    CONSTRAINT CK_Users_Role CHECK (role IN ('Admin', 'Employee'))
);
GO

-- =========================================================
-- Employees
-- One row per Employee-role user. Admins do NOT get a row here
-- (see README for why this is 0..1 rather than a strict 1-1).
-- =========================================================
CREATE TABLE Employees
(
    employee_id     INT IDENTITY(1,1)   NOT NULL,
    user_id         INT                 NOT NULL,
    first_name      NVARCHAR(50)        NOT NULL,
    last_name       NVARCHAR(50)        NOT NULL,
    phone           NVARCHAR(20)        NULL,
    email           NVARCHAR(100)       NULL,
    address         NVARCHAR(200)       NULL,
    hire_date       DATE                NOT NULL CONSTRAINT DF_Employees_HireDate DEFAULT (CAST(SYSDATETIME() AS DATE)),
    status          NVARCHAR(20)        NOT NULL CONSTRAINT DF_Employees_Status DEFAULT ('Active'),

    CONSTRAINT PK_Employees PRIMARY KEY (employee_id),
    CONSTRAINT UQ_Employees_UserId UNIQUE (user_id),       -- enforces 0..1 per user
    CONSTRAINT FK_Employees_Users FOREIGN KEY (user_id)
        REFERENCES Users (user_id),
    CONSTRAINT CK_Employees_Status CHECK (status IN ('Active', 'Inactive'))
);
GO

-- =========================================================
-- Categories
-- =========================================================
CREATE TABLE Categories
(
    category_id     INT IDENTITY(1,1)   NOT NULL,
    category_name   NVARCHAR(50)        NOT NULL,
    description     NVARCHAR(200)       NULL,
    is_active       BIT                 NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT (1),

    CONSTRAINT PK_Categories PRIMARY KEY (category_id),
    CONSTRAINT UQ_Categories_Name UNIQUE (category_name)
);
GO

-- =========================================================
-- Products
-- =========================================================
CREATE TABLE Products
(
    product_id      INT IDENTITY(1,1)   NOT NULL,
    category_id     INT                 NOT NULL,
    product_code    NVARCHAR(30)        NOT NULL,
    product_name    NVARCHAR(100)       NOT NULL,
    purchase_price  DECIMAL(10,2)       NOT NULL,
    selling_price   DECIMAL(10,2)       NOT NULL,
    unit            NVARCHAR(20)        NOT NULL,
    min_stock       INT                 NOT NULL CONSTRAINT DF_Products_MinStock DEFAULT (0),
    is_active       BIT                 NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT (1),
    created_at      DATETIME2           NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT (SYSDATETIME()),
    image_path      NVARCHAR(500)       NULL,   -- relative path under the app folder, e.g. "ProductImages/xxx.jpg"; NULL = no photo yet

    CONSTRAINT PK_Products PRIMARY KEY (product_id),
    CONSTRAINT UQ_Products_Code UNIQUE (product_code),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (category_id)
        REFERENCES Categories (category_id),
    CONSTRAINT CK_Products_PurchasePrice CHECK (purchase_price >= 0),
    CONSTRAINT CK_Products_SellingPrice CHECK (selling_price >= 0),
    CONSTRAINT CK_Products_MinStock CHECK (min_stock >= 0)
);
GO

CREATE INDEX IX_Products_CategoryId ON Products (category_id);
GO

-- =========================================================
-- Inventory
-- One inventory record per product (1-1).
-- =========================================================
CREATE TABLE Inventory
(
    inventory_id    INT IDENTITY(1,1)   NOT NULL,
    product_id      INT                 NOT NULL,
    quantity        INT                 NOT NULL CONSTRAINT DF_Inventory_Quantity DEFAULT (0),
    last_updated    DATETIME2           NOT NULL CONSTRAINT DF_Inventory_LastUpdated DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Inventory PRIMARY KEY (inventory_id),
    CONSTRAINT UQ_Inventory_ProductId UNIQUE (product_id),  -- enforces 1-1 with Products
    CONSTRAINT FK_Inventory_Products FOREIGN KEY (product_id)
        REFERENCES Products (product_id),
    CONSTRAINT CK_Inventory_Quantity CHECK (quantity >= 0)
);
GO

-- =========================================================
-- Sales
-- One row per completed POS transaction.
-- =========================================================
CREATE TABLE Sales
(
    sale_id         INT IDENTITY(1,1)   NOT NULL,
    employee_id     INT                 NOT NULL,
    sale_date       DATETIME2           NOT NULL CONSTRAINT DF_Sales_SaleDate DEFAULT (SYSDATETIME()),
    subtotal        DECIMAL(10,2)       NOT NULL,
    discount        DECIMAL(10,2)       NOT NULL CONSTRAINT DF_Sales_Discount DEFAULT (0),
    total            DECIMAL(10,2)      NOT NULL,
    payment         DECIMAL(10,2)       NOT NULL,
    change_amount   DECIMAL(10,2)       NOT NULL,

    CONSTRAINT PK_Sales PRIMARY KEY (sale_id),
    CONSTRAINT FK_Sales_Employees FOREIGN KEY (employee_id)
        REFERENCES Employees (employee_id),
    CONSTRAINT CK_Sales_Subtotal CHECK (subtotal >= 0),
    CONSTRAINT CK_Sales_Discount CHECK (discount >= 0),
    CONSTRAINT CK_Sales_Total CHECK (total >= 0),
    CONSTRAINT CK_Sales_Payment CHECK (payment >= total),
    CONSTRAINT CK_Sales_ChangeAmount CHECK (change_amount >= 0)
);
GO

CREATE INDEX IX_Sales_EmployeeId ON Sales (employee_id);
CREATE INDEX IX_Sales_SaleDate ON Sales (sale_date);
GO

-- =========================================================
-- SaleItems
-- Line items belonging to a Sale (composition: Sale -> SaleItems).
-- =========================================================
CREATE TABLE SaleItems
(
    sale_item_id    INT IDENTITY(1,1)   NOT NULL,
    sale_id         INT                 NOT NULL,
    product_id      INT                 NOT NULL,
    quantity        INT                 NOT NULL,
    unit_price      DECIMAL(10,2)       NOT NULL,
    subtotal        DECIMAL(10,2)       NOT NULL,

    CONSTRAINT PK_SaleItems PRIMARY KEY (sale_item_id),
    CONSTRAINT FK_SaleItems_Sales FOREIGN KEY (sale_id)
        REFERENCES Sales (sale_id)
        ON DELETE CASCADE,                          -- deleting a Sale removes its SaleItems
    CONSTRAINT FK_SaleItems_Products FOREIGN KEY (product_id)
        REFERENCES Products (product_id),
    CONSTRAINT CK_SaleItems_Quantity CHECK (quantity > 0),
    CONSTRAINT CK_SaleItems_UnitPrice CHECK (unit_price >= 0),
    CONSTRAINT CK_SaleItems_Subtotal CHECK (subtotal >= 0)
);
GO

CREATE INDEX IX_SaleItems_SaleId ON SaleItems (sale_id);
CREATE INDEX IX_SaleItems_ProductId ON SaleItems (product_id);
GO
