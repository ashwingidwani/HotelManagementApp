using Microsoft.Data.SqlClient;
using HotelInvoiceApp.Database;
using HotelInvoiceApp.Models;

namespace HotelInvoiceApp.Services;

public class InvoiceRepository
{
    public int Save(Invoice invoice)
    {
        using var conn = DatabaseSetup.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Invoices (BookingId, InvoiceNumber, InvoiceDate, PdfPath)
            VALUES (@bid, @num, @date, @pdf);
            SELECT SCOPE_IDENTITY();";
        cmd.Parameters.AddWithValue("@bid",  invoice.BookingId);
        cmd.Parameters.AddWithValue("@num",  invoice.InvoiceNumber);
        cmd.Parameters.AddWithValue("@date", invoice.InvoiceDate);
        cmd.Parameters.AddWithValue("@pdf",  invoice.PdfPath ?? (object)DBNull.Value);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public List<Invoice> GetAll()
    {
        var list = new List<Invoice>();
        using var conn = DatabaseSetup.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT i.*, b.BookingRef, g.FullName
            FROM Invoices i
            JOIN Bookings b ON b.BookingId = i.BookingId
            JOIN Guests   g ON g.GuestId   = b.GuestId
            ORDER BY i.InvoiceDate DESC";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Invoice
            {
                InvoiceId     = r.GetInt32(r.GetOrdinal("InvoiceId")),
                BookingId     = r.GetInt32(r.GetOrdinal("BookingId")),
                InvoiceNumber = r.GetString(r.GetOrdinal("InvoiceNumber")),
                InvoiceDate   = r.GetDateTime(r.GetOrdinal("InvoiceDate")),
                PdfPath       = r.IsDBNull(r.GetOrdinal("PdfPath")) ? "" : r.GetString(r.GetOrdinal("PdfPath")),
            });
        }
        return list;
    }

    /// <summary>Generate next invoice number like INV-2024-0001</summary>
    public string GenerateInvoiceNumber()
    {
        using var conn = DatabaseSetup.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Invoices WHERE YEAR(InvoiceDate) = YEAR(GETDATE())";
        int count = (int)cmd.ExecuteScalar()! + 1;
        return $"INV-{DateTime.Today.Year}-{count:D4}";
    }
}
