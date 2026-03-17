# Hotel Invoice Manager
A C# .NET 8 WinForms desktop application for creating hotel booking invoices with guest ID upload and SQL Express storage.

---

## Features
- **Guest registration** — full name, email, phone
- **ID upload** — supports Aadhaar Card, PAN Card, Driving License (image stored in SQL DB)
- **Booking creation** — room number, room type, check-in/out, rate, GST breakdown
- **Auto-calculated totals** — sub-total, GST, grand total computed in real-time
- **PDF Invoice export** — professional branded invoice saved to Documents\HotelInvoices\
- **Bookings list** — searchable grid of all bookings
- **Invoices list** — all invoices with double-click to open PDF

---

## Prerequisites
- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express or SQL Server LocalDB (comes with Visual Studio)
- Visual Studio 2022 (Community edition is free)

---

## Setup Instructions

### 1. Open the project
Open `HotelInvoiceApp.csproj` in Visual Studio 2022.

### 2. Configure the database connection
Edit `Database/DatabaseSetup.cs` and set your connection string:

```csharp
// For SQL LocalDB (comes with Visual Studio — default):
"Server=(localdb)\\MSSQLLocalDB;Database=HotelInvoiceDB;Trusted_Connection=True;TrustServerCertificate=True;"

// For SQL Server Express:
"Server=.\\SQLEXPRESS;Database=HotelInvoiceDB;Trusted_Connection=True;TrustServerCertificate=True;"

// For named SQL Express instance:
"Server=YOUR_PC_NAME\\SQLEXPRESS;Database=HotelInvoiceDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 3. Restore NuGet packages
Visual Studio will do this automatically. Or run:
```
dotnet restore
```

### 4. Run the application
Press **F5** in Visual Studio or:
```
dotnet run
```

The app will **automatically create** the `HotelInvoiceDB` database and all tables on first launch.

> If auto-creation fails, run `Database/CreateDatabase.sql` manually in SQL Server Management Studio (SSMS).

---

## Customise Hotel Details
Edit the constants at the top of `Services/PdfInvoiceService.cs`:

```csharp
private const string HotelName    = "Grand Palace Hotel";
private const string HotelAddress = "123 Main Street, Mumbai, Maharashtra - 400001";
private const string HotelPhone   = "+91 22 1234 5678";
private const string HotelEmail   = "reservations@grandpalace.com";
private const string HotelGstin   = "27AABCG1234A1Z5";
private const string HotelPan     = "AABCG1234A";
```

---

## Invoice PDF Output
Invoices are saved to:
```
C:\Users\<YourName>\Documents\HotelInvoices\INV-2024-0001.pdf
```

---

## Project Structure
```
HotelInvoiceApp/
├── Program.cs                     # Entry point
├── HotelInvoiceApp.csproj         # Project file (NuGet refs)
├── Database/
│   ├── DatabaseSetup.cs           # Auto DB + table creation, connection helper
│   └── CreateDatabase.sql         # Manual SQL script (fallback)
├── Models/
│   └── Models.cs                  # Guest, Booking, Invoice classes
├── Services/
│   ├── GuestRepository.cs         # Guest CRUD
│   ├── BookingRepository.cs       # Booking CRUD
│   ├── InvoiceRepository.cs       # Invoice CRUD + number generation
│   └── PdfInvoiceService.cs       # PDF generation (PdfSharp)
└── Forms/
    ├── MainForm.cs                # Main window with tab control
    ├── NewBookingPanel.cs         # New booking + invoice form
    ├── BookingsListPanel.cs       # All bookings grid
    └── InvoicesListPanel.cs       # All invoices grid
```

---

## NuGet Packages Used
| Package | Purpose |
|---|---|
| `Microsoft.Data.SqlClient` | SQL Server connectivity |
| `PdfSharp` | PDF generation |
| `System.Drawing.Common` | Image handling |
