using Microsoft.Data.SqlClient;

namespace HotelInvoiceApp.Database;

public static class DatabaseSetup
{
    // ---------------------------------------------------------------
    // Change this connection string to point at your SQL Express instance
    // Default: (localdb)\MSSQLLocalDB  — works with Visual Studio out-of-the-box
    // For SQL Express: "Server=.\\SQLEXPRESS;Database=HotelInvoiceDB;Trusted_Connection=True;TrustServerCertificate=True;"
    // ---------------------------------------------------------------
    public static string ConnectionString { get; set; } =
        "Server=localhost\\SQLEXPRESS;Database=HotelInvoiceDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public static void Initialize()
    {
        // Create database if it doesn't exist
        CreateDatabase();
        CreateTables();
    }

    private static void CreateDatabase()
    {
        // Connect to master to create the DB
        var masterConn = ConnectionString.Replace("Database=HotelInvoiceDB;", "Database=master;");
        using var conn = new SqlConnection(masterConn);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HotelInvoiceDB')
                CREATE DATABASE HotelInvoiceDB;";
        cmd.ExecuteNonQuery();
    }

    private static void CreateTables()
    {
        using var conn = new SqlConnection(ConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            -- Guests table
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Guests')
            CREATE TABLE Guests (
                GuestId       INT IDENTITY(1,1) PRIMARY KEY,
                FullName      NVARCHAR(200)  NOT NULL,
                Email         NVARCHAR(200),
                Phone         NVARCHAR(30),
                IdType        NVARCHAR(50)   NOT NULL,   -- Aadhaar / PAN / Driving License
                IdNumber      NVARCHAR(100)  NOT NULL,
                IdImagePath   NVARCHAR(500),             -- path to stored image file
                IdImageData   VARBINARY(MAX),             -- raw image bytes
                CreatedAt     DATETIME       DEFAULT GETDATE()
            );

            -- Bookings table
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

            -- Invoices table
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Invoices')
            CREATE TABLE Invoices (
                InvoiceId     INT IDENTITY(1,1) PRIMARY KEY,
                BookingId     INT            NOT NULL REFERENCES Bookings(BookingId),
                InvoiceNumber NVARCHAR(30)   NOT NULL UNIQUE,
                InvoiceDate   DATE           NOT NULL,
                PdfPath       NVARCHAR(500),
                CreatedAt     DATETIME       DEFAULT GETDATE()
            );";
        cmd.ExecuteNonQuery();
    }

    public static SqlConnection GetConnection()
    {
        var conn = new SqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}
