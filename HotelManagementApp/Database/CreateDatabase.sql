-- ============================================================
-- Hotel Invoice Manager – Database Setup Script
-- Run this manually in SSMS if the app cannot auto-create the DB
-- ============================================================

-- 1. Create database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HotelInvoiceDB')
    CREATE DATABASE HotelInvoiceDB;
GO

USE HotelInvoiceDB;
GO

-- 2. Guests
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Guests')
CREATE TABLE Guests (
    GuestId       INT IDENTITY(1,1) PRIMARY KEY,
    FullName      NVARCHAR(200)  NOT NULL,
    Email         NVARCHAR(200),
    Phone         NVARCHAR(30),
    IdType        NVARCHAR(50)   NOT NULL,   -- Aadhaar / PAN / Driving License
    IdNumber      NVARCHAR(100)  NOT NULL,
    IdImagePath   NVARCHAR(500),             -- file path of uploaded image
    IdImageData   VARBINARY(MAX),            -- raw binary of image
    CreatedAt     DATETIME       DEFAULT GETDATE()
);
GO

-- 3. Bookings
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Bookings')
CREATE TABLE Bookings (
    BookingId     INT IDENTITY(1,1) PRIMARY KEY,
    GuestId       INT            NOT NULL REFERENCES Guests(GuestId),
    BookingRef    NVARCHAR(20)   NOT NULL UNIQUE,
    RoomNumber    NVARCHAR(20)   NOT NULL,
    RoomType      NVARCHAR(100)  NOT NULL,
    CheckIn       DATE           NOT NULL,
    CheckOut      DATE           NOT NULL,
    RatePerNight  DECIMAL(10,2)  NOT NULL,
    TotalNights   INT            NOT NULL,
    SubTotal      DECIMAL(10,2)  NOT NULL,
    GstRate       DECIMAL(5,2)   NOT NULL DEFAULT 12.00,
    GstAmount     DECIMAL(10,2)  NOT NULL,
    GrandTotal    DECIMAL(10,2)  NOT NULL,
    Notes         NVARCHAR(500),
    CreatedAt     DATETIME       DEFAULT GETDATE()
);
GO

-- 4. Invoices
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Invoices')
CREATE TABLE Invoices (
    InvoiceId     INT IDENTITY(1,1) PRIMARY KEY,
    BookingId     INT            NOT NULL REFERENCES Bookings(BookingId),
    InvoiceNumber NVARCHAR(30)   NOT NULL UNIQUE,
    InvoiceDate   DATE           NOT NULL,
    PdfPath       NVARCHAR(500),
    CreatedAt     DATETIME       DEFAULT GETDATE()
);
GO

PRINT 'Database HotelInvoiceDB setup complete.';
