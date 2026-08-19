-- =========================================================
-- Lab 1: Login, Registration & Logout
-- Student ID: 24-59060-3
-- =========================================================

-- 1. Create the database
IF DB_ID('24-59060-3_LoginDB') IS NULL
BEGIN
    CREATE DATABASE [24-59060-3_LoginDB];
END
GO

USE [24-59060-3_LoginDB];
GO

-- 2. Create the Users table
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserID       INT IDENTITY(1,1) PRIMARY KEY,
        Username     NVARCHAR(50)  NOT NULL UNIQUE,
        PasswordHash NVARCHAR(200) NOT NULL,
        Email        NVARCHAR(100) NULL,
        FullName     NVARCHAR(100) NULL,
        CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE()
    );
END
GO

-- =========================================================
-- BONUS (optional): LoginHistory table
-- Only needed if you attempt the LoginHistory bonus task.
-- Uncomment to use.
-- =========================================================
-- IF OBJECT_ID('dbo.LoginHistory', 'U') IS NULL
-- BEGIN
--     CREATE TABLE dbo.LoginHistory
--     (
--         LoginHistoryID INT IDENTITY(1,1) PRIMARY KEY,
--         UserID         INT NOT NULL,
--         LoginTime      DATETIME NOT NULL DEFAULT GETDATE(),
--         LogoutTime     DATETIME NULL,
--         CONSTRAINT FK_LoginHistory_Users FOREIGN KEY (UserID)
--             REFERENCES dbo.Users(UserID)
--     );
-- END
-- GO

-- 3. Sanity check
SELECT * FROM dbo.Users;
