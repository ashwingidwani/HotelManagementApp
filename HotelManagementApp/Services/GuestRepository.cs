using Microsoft.Data.SqlClient;
using HotelInvoiceApp.Database;
using HotelInvoiceApp.Models;

namespace HotelInvoiceApp.Services;

public class GuestRepository
{
    public int Save(Guest guest)
    {
        using var conn = DatabaseSetup.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Guests (FullName, Email, Phone, IdType, IdNumber, IdImagePath, IdImageData)
            VALUES (@name, @email, @phone, @idType, @idNum, @imgPath, @imgData);
            SELECT SCOPE_IDENTITY();";
        cmd.Parameters.AddWithValue("@name",    guest.FullName);
        cmd.Parameters.AddWithValue("@email",   guest.Email ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@phone",   guest.Phone ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@idType",  guest.IdType);
        cmd.Parameters.AddWithValue("@idNum",   guest.IdNumber);
        cmd.Parameters.AddWithValue("@imgPath", guest.IdImagePath ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@imgData", guest.IdImageData ?? (object)DBNull.Value);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public List<Guest> Search(string query)
    {
        var list = new List<Guest>();
        using var conn = DatabaseSetup.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM Guests
            WHERE FullName LIKE @q OR IdNumber LIKE @q OR Phone LIKE @q
            ORDER BY CreatedAt DESC";
        cmd.Parameters.AddWithValue("@q", $"%{query}%");
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    public List<Guest> GetAll()
    {
        var list = new List<Guest>();
        using var conn = DatabaseSetup.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM Guests ORDER BY FullName";
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    public Guest? GetById(int id)
    {
        using var conn = DatabaseSetup.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM Guests WHERE GuestId = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Map(r) : null;
    }

    private static Guest Map(SqlDataReader r) => new()
    {
        GuestId      = r.GetInt32(r.GetOrdinal("GuestId")),
        FullName     = r.GetString(r.GetOrdinal("FullName")),
        Email        = r.IsDBNull(r.GetOrdinal("Email")) ? "" : r.GetString(r.GetOrdinal("Email")),
        Phone        = r.IsDBNull(r.GetOrdinal("Phone")) ? "" : r.GetString(r.GetOrdinal("Phone")),
        IdType       = r.GetString(r.GetOrdinal("IdType")),
        IdNumber     = r.GetString(r.GetOrdinal("IdNumber")),
        IdImagePath  = r.IsDBNull(r.GetOrdinal("IdImagePath")) ? "" : r.GetString(r.GetOrdinal("IdImagePath")),
        IdImageData  = r.IsDBNull(r.GetOrdinal("IdImageData")) ? null : (byte[])r["IdImageData"],
        CreatedAt    = r.GetDateTime(r.GetOrdinal("CreatedAt")),
    };
}
