using Microsoft.Data.SqlClient;
using HotelInvoiceApp.Database;
using HotelInvoiceApp.Models;

namespace HotelInvoiceApp.Services;

public class BookingRepository
{
    public int Save(Booking booking)
    {
        using var conn = DatabaseSetup.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Bookings
                (GuestId, BookingRef, RoomNumber, RoomType, CheckIn, CheckOut,
                 RatePerNight, TotalNights, SubTotal, GstRate, GstAmount, GrandTotal, Notes)
            VALUES
                (@gId, @ref, @room, @type, @ci, @co,
                 @rate, @nights, @sub, @gstR, @gstA, @grand, @notes);
            SELECT SCOPE_IDENTITY();";

        cmd.Parameters.AddWithValue("@gId",    booking.GuestId);
        cmd.Parameters.AddWithValue("@ref",    booking.BookingRef);
        cmd.Parameters.AddWithValue("@room",   booking.RoomNumber);
        cmd.Parameters.AddWithValue("@type",   booking.RoomType);
        cmd.Parameters.AddWithValue("@ci",     booking.CheckIn);
        cmd.Parameters.AddWithValue("@co",     booking.CheckOut);
        cmd.Parameters.AddWithValue("@rate",   booking.RatePerNight);
        cmd.Parameters.AddWithValue("@nights", booking.TotalNights);
        cmd.Parameters.AddWithValue("@sub",    booking.SubTotal);
        cmd.Parameters.AddWithValue("@gstR",   booking.GstRate);
        cmd.Parameters.AddWithValue("@gstA",   booking.GstAmount);
        cmd.Parameters.AddWithValue("@grand",  booking.GrandTotal);
        cmd.Parameters.AddWithValue("@notes",  booking.Notes ?? (object)DBNull.Value);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public List<Booking> GetAll()
    {
        var list = new List<Booking>();
        using var conn = DatabaseSetup.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT b.*, g.FullName, g.IdType, g.IdNumber, g.Phone, g.Email
            FROM Bookings b
            JOIN Guests g ON g.GuestId = b.GuestId
            ORDER BY b.CreatedAt DESC";
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(MapWithGuest(r));
        return list;
    }

    public Booking? GetById(int id)
    {
        using var conn = DatabaseSetup.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT b.*, g.FullName, g.IdType, g.IdNumber, g.Phone, g.Email
            FROM Bookings b
            JOIN Guests g ON g.GuestId = b.GuestId
            WHERE b.BookingId = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? MapWithGuest(r) : null;
    }

    private static Booking MapWithGuest(SqlDataReader r)
    {
        var booking = new Booking
        {
            BookingId    = r.GetInt32(r.GetOrdinal("BookingId")),
            GuestId      = r.GetInt32(r.GetOrdinal("GuestId")),
            BookingRef   = r.GetString(r.GetOrdinal("BookingRef")),
            RoomNumber   = r.GetString(r.GetOrdinal("RoomNumber")),
            RoomType     = r.GetString(r.GetOrdinal("RoomType")),
            CheckIn      = r.GetDateTime(r.GetOrdinal("CheckIn")),
            CheckOut     = r.GetDateTime(r.GetOrdinal("CheckOut")),
            RatePerNight = r.GetDecimal(r.GetOrdinal("RatePerNight")),
            TotalNights  = r.GetInt32(r.GetOrdinal("TotalNights")),
            SubTotal     = r.GetDecimal(r.GetOrdinal("SubTotal")),
            GstRate      = r.GetDecimal(r.GetOrdinal("GstRate")),
            GstAmount    = r.GetDecimal(r.GetOrdinal("GstAmount")),
            GrandTotal   = r.GetDecimal(r.GetOrdinal("GrandTotal")),
            Notes        = r.IsDBNull(r.GetOrdinal("Notes")) ? "" : r.GetString(r.GetOrdinal("Notes")),
            CreatedAt    = r.GetDateTime(r.GetOrdinal("CreatedAt")),
        };

        // Join fields from Guests
        booking.Guest = new Guest
        {
            GuestId  = booking.GuestId,
            FullName = r.GetString(r.GetOrdinal("FullName")),
            IdType   = r.GetString(r.GetOrdinal("IdType")),
            IdNumber = r.GetString(r.GetOrdinal("IdNumber")),
            Phone    = r.IsDBNull(r.GetOrdinal("Phone")) ? "" : r.GetString(r.GetOrdinal("Phone")),
            Email    = r.IsDBNull(r.GetOrdinal("Email")) ? "" : r.GetString(r.GetOrdinal("Email")),
        };

        return booking;
    }
}
