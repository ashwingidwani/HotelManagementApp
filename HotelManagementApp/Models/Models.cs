namespace HotelInvoiceApp.Models;

public class Guest
{
    public int GuestId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string IdType { get; set; } = "";      // Aadhaar / PAN / Driving License
    public string IdNumber { get; set; } = "";
    public string IdImagePath { get; set; } = "";
    public byte[]? IdImageData { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Booking
{
    public int BookingId { get; set; }
    public int GuestId { get; set; }
    public string BookingRef { get; set; } = "";
    public string RoomNumber { get; set; } = "";
    public string RoomType { get; set; } = "";
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal RatePerNight { get; set; }
    public int TotalNights { get; set; }
    public decimal SubTotal { get; set; }
    public decimal GstRate { get; set; } = 12m;
    public decimal GstAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string Notes { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Guest? Guest { get; set; }
}

public class Invoice
{
    public int InvoiceId { get; set; }
    public int BookingId { get; set; }
    public string InvoiceNumber { get; set; } = "";
    public DateTime InvoiceDate { get; set; }
    public string PdfPath { get; set; } = "";

    // Navigation
    public Booking? Booking { get; set; }
}
