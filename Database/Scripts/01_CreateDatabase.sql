-- =========================================================
-- 01_CreateDatabase.sql
-- Creates the MiniMartDb database.
-- Run this first, on its own, connected to the 'master' database.
-- =========================================================

IF DB_ID('MiniMartDb') IS NULL
BEGIN
    CREATE DATABASE MiniMartDb;
END
GO
