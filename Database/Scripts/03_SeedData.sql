-- =========================================================
-- 03_SeedData.sql
-- Seed / test data: one Admin login, one Employee login,
-- a couple of categories and products with starting stock.
-- Run this after 02_CreateTables.sql.
-- =========================================================

USE MiniMartDb;
GO

-- -----------------------------------------------------------------
-- Admin user
-- Username: admin
-- Password: Admin123!
-- (Hash format matches AuthService in the app: "iterations.saltBase64.hashBase64",
--  PBKDF2-HMAC-SHA256, 100,000 iterations, 16-byte salt, 32-byte derived key.)
-- -----------------------------------------------------------------
INSERT INTO Users (username, password_hash, role, is_active)
VALUES
    ('admin', '100000.TzqcHit9bwpcjhs9nyp8Tg==.0KF4bgfGcZegmrN6xl9ZaRlCb4Y6kwM6o6DSjIPpmC0=', 'Admin', 1);

-- -----------------------------------------------------------------
-- Employee user + matching Employee profile
-- Username: jdoe
-- Password: Employee123!
-- NOTE: this placeholder hash is just for seeding a row; when you run the
-- app, use the Admin "Add Employee" screen (Phase 7) to create real
-- employees, whose passwords will be hashed correctly by AuthService.
-- -----------------------------------------------------------------
INSERT INTO Users (username, password_hash, role, is_active)
VALUES
    ('jdoe', '100000.d1B4M2xVeUgybnFTdEpHNw==.4b3f6c9a2e7d5f1a8c6b3d9e2f4a7c5b', 'Employee', 1);

DECLARE @jdoeUserId INT = SCOPE_IDENTITY();

INSERT INTO Employees (user_id, first_name, last_name, phone, email, address, hire_date, status)
VALUES
    (@jdoeUserId, 'John', 'Doe', '012345678', 'jdoe@minimart.local', 'Phnom Penh, Cambodia', '2025-01-15', 'Active');

-- -----------------------------------------------------------------
-- Categories
-- -----------------------------------------------------------------
INSERT INTO Categories (category_name, description, is_active)
VALUES
    ('Beverages', 'Drinks, juices, water, soda', 1),
    ('Snacks', 'Chips, biscuits, candy', 1),
    ('Household', 'Cleaning and household supplies', 1);

-- -----------------------------------------------------------------
-- Products
-- -----------------------------------------------------------------
INSERT INTO Products (category_id, product_code, product_name, purchase_price, selling_price, unit, min_stock, is_active)
VALUES
    ((SELECT category_id FROM Categories WHERE category_name = 'Beverages'), 'BEV-001', 'Mineral Water 500ml', 0.30, 0.50, 'bottle', 20, 1),
    ((SELECT category_id FROM Categories WHERE category_name = 'Beverages'), 'BEV-002', 'Orange Juice 1L',     1.20, 1.80, 'carton', 10, 1),
    ((SELECT category_id FROM Categories WHERE category_name = 'Snacks'),    'SNK-001', 'Potato Chips 150g',   0.60, 1.00, 'pack',   15, 1),
    ((SELECT category_id FROM Categories WHERE category_name = 'Household'), 'HH-001',  'Dish Soap 500ml',     0.90, 1.50, 'bottle',  5, 1);

-- -----------------------------------------------------------------
-- Inventory (starting stock; one row per product)
-- -----------------------------------------------------------------
INSERT INTO Inventory (product_id, quantity)
VALUES
    ((SELECT product_id FROM Products WHERE product_code = 'BEV-001'), 50),
    ((SELECT product_id FROM Products WHERE product_code = 'BEV-002'), 8),   -- below min_stock 10 -> low stock
    ((SELECT product_id FROM Products WHERE product_code = 'SNK-001'), 30),
    ((SELECT product_id FROM Products WHERE product_code = 'HH-001'), 0);    -- out of stock

GO
