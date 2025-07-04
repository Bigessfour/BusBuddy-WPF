-- Bus Buddy Test Database Setup Script
-- This script creates a dedicated test database on SQL Server Express
-- Separate from production for safe testing

USE master;
GO

-- Drop test database if it exists
IF DB_ID('BusBuddyTestDb') IS NOT NULL
BEGIN
    ALTER DATABASE BusBuddyTestDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BusBuddyTestDb;
END
GO

-- Create test database
CREATE DATABASE BusBuddyTestDb
ON (
    NAME = 'BusBuddyTestDb',
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\BusBuddyTestDb.mdf',
    SIZE = 100MB,
    MAXSIZE = 1GB,
    FILEGROWTH = 10MB
)
LOG ON (
    NAME = 'BusBuddyTestDb_Log',
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\BusBuddyTestDb_Log.ldf',
    SIZE = 10MB,
    MAXSIZE = 100MB,
    FILEGROWTH = 1MB
);
GO

-- Use the test database
USE BusBuddyTestDb;
GO

-- Verify database creation
SELECT 
    name as DatabaseName,
    database_id as DatabaseId,
    create_date as Created,
    compatibility_level as CompatibilityLevel
FROM sys.databases 
WHERE name = 'BusBuddyTestDb';
GO

PRINT 'Bus Buddy Test Database Created Successfully!';
PRINT 'Connection String: Server=.\SQLEXPRESS;Database=BusBuddyTestDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;Encrypt=false';
