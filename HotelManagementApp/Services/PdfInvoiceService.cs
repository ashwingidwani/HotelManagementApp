using PdfSharp.Drawing;
using PdfSharp.Pdf;
using HotelInvoiceApp.Models;

namespace HotelInvoiceApp.Services;

public class PdfInvoiceService
{
    // Hotel details — update these to match your property
    private const string HotelName    = "Grand Palace Hotel";
    private const string HotelAddress = "123 Main Street, Mumbai, Maharashtra - 400001";
    private const string HotelPhone   = "+91 22 1234 5678";
    private const string HotelEmail   = "reservations@grandpalace.com";
    private const string HotelGstin   = "27AABCG1234A1Z5";
    private const string HotelPan     = "AABCG1234A";

    // Brand colours
    private static readonly XColor HeaderBg    = XColor.FromArgb(30, 64, 115);
    private static readonly XColor AccentBlue  = XColor.FromArgb(46, 117, 182);
    private static readonly XColor LightGray   = XColor.FromArgb(245, 247, 250);
    private static readonly XColor BorderGray  = XColor.FromArgb(189, 215, 238);
    private static readonly XColor DarkText    = XColor.FromArgb(26, 26, 46);
    private static readonly XColor MutedText   = XColor.FromArgb(100, 100, 120);
    private static readonly XColor GreenAccent = XColor.FromArgb(30, 107, 60);

    public string Generate(Booking booking, string invoiceNumber, string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);
        string path = Path.Combine(outputFolder, $"{invoiceNumber}.pdf");

        using var doc  = new PdfDocument();
        doc.Info.Title  = $"Invoice {invoiceNumber}";
        doc.Info.Author = HotelName;

        var page = doc.AddPage();
        page.Size = PdfSharp.PageSize.A4;

        using var gfx = XGraphics.FromPdfPage(page);
        double w  = page.Width.Point;
        double h  = page.Height.Point;
        double ml = 40; // left margin
        double mr = 40; // right margin
        double cw = w - ml - mr; // content width
        double y  = 0;

        // ── HEADER BAR ──────────────────────────────────────────────
        gfx.DrawRectangle(new XSolidBrush(HeaderBg), 0, 0, w, 110);

        var fontHotelName  = new XFont("Arial", 22, XFontStyleEx.Bold);
        var fontSmall      = new XFont("Arial", 9,  XFontStyleEx.Regular);
        var fontSmallBold  = new XFont("Arial", 9,  XFontStyleEx.Bold);
        var fontBody       = new XFont("Arial", 10, XFontStyleEx.Regular);
        var fontBodyBold   = new XFont("Arial", 10, XFontStyleEx.Bold);
        var fontH2         = new XFont("Arial", 13, XFontStyleEx.Bold);
        var fontLabel      = new XFont("Arial", 8,  XFontStyleEx.Regular);
        var fontLabelBold  = new XFont("Arial", 8,  XFontStyleEx.Bold);

        gfx.DrawString(HotelName, fontHotelName, XBrushes.White,
            new XRect(ml, 18, cw, 30), XStringFormats.TopLeft);
        gfx.DrawString(HotelAddress, fontSmall, new XSolidBrush(BorderGray),
            new XRect(ml, 48, cw, 14), XStringFormats.TopLeft);
        gfx.DrawString($"Tel: {HotelPhone}  |  Email: {HotelEmail}", fontSmall,
            new XSolidBrush(BorderGray), new XRect(ml, 62, cw, 14), XStringFormats.TopLeft);
        gfx.DrawString($"GSTIN: {HotelGstin}  |  PAN: {HotelPan}", fontSmall,
            new XSolidBrush(BorderGray), new XRect(ml, 76, cw, 14), XStringFormats.TopLeft);

        // "TAX INVOICE" label top-right
        gfx.DrawString("TAX INVOICE", new XFont("Arial", 16, XFontStyleEx.Bold),
            XBrushes.White, new XRect(0, 20, w - mr, 24), XStringFormats.TopRight);

        y = 120;

        // ── INVOICE META (two-column) ────────────────────────────────
        double col1x = ml;
        double col2x = ml + cw / 2 + 10;
        double colW  = cw / 2 - 10;

        // Left: Bill To
        gfx.DrawString("BILL TO", fontLabelBold, new XSolidBrush(MutedText),
            new XRect(col1x, y, colW, 14), XStringFormats.TopLeft);
        y += 14;
        gfx.DrawString(booking.Guest!.FullName, fontBodyBold, new XSolidBrush(DarkText),
            new XRect(col1x, y, colW, 14), XStringFormats.TopLeft);
        y += 14;
        if (!string.IsNullOrEmpty(booking.Guest.Phone))
        {
            gfx.DrawString($"Phone: {booking.Guest.Phone}", fontBody, new XSolidBrush(DarkText),
                new XRect(col1x, y, colW, 13), XStringFormats.TopLeft);
            y += 13;
        }
        if (!string.IsNullOrEmpty(booking.Guest.Email))
        {
            gfx.DrawString($"Email: {booking.Guest.Email}", fontBody, new XSolidBrush(DarkText),
                new XRect(col1x, y, colW, 13), XStringFormats.TopLeft);
            y += 13;
        }
        gfx.DrawString($"ID ({booking.Guest.IdType}): {booking.Guest.IdNumber}", fontBody,
            new XSolidBrush(DarkText), new XRect(col1x, y, colW, 13), XStringFormats.TopLeft);

        // Right: Invoice details
        double ry = 120;
        void DrawMetaRow(string label, string value)
        {
            gfx.DrawString(label, fontLabelBold, new XSolidBrush(MutedText),
                new XRect(col2x, ry, 80, 13), XStringFormats.TopLeft);
            gfx.DrawString(value, fontBodyBold, new XSolidBrush(DarkText),
                new XRect(col2x + 82, ry, colW - 82, 13), XStringFormats.TopLeft);
            ry += 15;
        }
        DrawMetaRow("Invoice No :", invoiceNumber);
        DrawMetaRow("Invoice Date:", DateTime.Today.ToString("dd MMM yyyy"));
        DrawMetaRow("Booking Ref :", booking.BookingRef);

        y = Math.Max(y, ry) + 20;

        // ── ROOM / STAY DETAILS ──────────────────────────────────────
        double boxH = 50;
        gfx.DrawRectangle(new XSolidBrush(LightGray), ml, y, cw, boxH);
        gfx.DrawRectangle(new XPen(BorderGray, 0.5), ml, y, cw, boxH);

        double qcw = cw / 4;
        void DrawStayCell(string label, string val, double x)
        {
            gfx.DrawString(label, fontLabel, new XSolidBrush(MutedText),
                new XRect(x + 8, y + 8, qcw - 16, 12), XStringFormats.TopLeft);
            gfx.DrawString(val, fontBodyBold, new XSolidBrush(DarkText),
                new XRect(x + 8, y + 22, qcw - 16, 14), XStringFormats.TopLeft);
        }

        DrawStayCell("ROOM NUMBER", booking.RoomNumber, ml);
        DrawStayCell("ROOM TYPE",   booking.RoomType,   ml + qcw);
        DrawStayCell("CHECK-IN",    booking.CheckIn.ToString("dd MMM yyyy"),  ml + 2 * qcw);
        DrawStayCell("CHECK-OUT",   booking.CheckOut.ToString("dd MMM yyyy"), ml + 3 * qcw);

        // vertical separators inside the box
        for (int i = 1; i <= 3; i++)
            gfx.DrawLine(new XPen(BorderGray, 0.5), ml + i * qcw, y + 6, ml + i * qcw, y + boxH - 6);

        y += boxH + 18;

        // ── LINE ITEMS TABLE ─────────────────────────────────────────
        double[] colWidths = { cw * 0.40, cw * 0.15, cw * 0.15, cw * 0.15, cw * 0.15 };
        string[] headers   = { "Description", "Nights", "Rate/Night (₹)", "Amount (₹)", "Total (₹)" };

        // Header row
        double cx = ml;
        gfx.DrawRectangle(new XSolidBrush(AccentBlue), ml, y, cw, 22);
        for (int i = 0; i < headers.Length; i++)
        {
            gfx.DrawString(headers[i], fontSmallBold, XBrushes.White,
                new XRect(cx + 4, y + 5, colWidths[i] - 8, 14),
                i == 0 ? XStringFormats.TopLeft : XStringFormats.TopRight);
            cx += colWidths[i];
        }
        y += 22;

        // Data row
        string desc = $"Room {booking.RoomNumber} — {booking.RoomType}";
        DrawTableRow(gfx, ml, y, colWidths, LightGray, fontBody,
            desc,
            booking.TotalNights.ToString(),
            $"{booking.RatePerNight:N2}",
            $"{booking.SubTotal:N2}",
            $"{booking.SubTotal:N2}");
        y += 24;

        // Bottom border of table
        gfx.DrawLine(new XPen(BorderGray, 0.5), ml, y, ml + cw, y);
        y += 12;

        // ── TOTALS BOX ───────────────────────────────────────────────
        double totBoxW = 230;
        double totBoxX = ml + cw - totBoxW;

        void DrawTotalRow(string label, string val, bool bold = false, XColor? bg = null)
        {
            if (bg.HasValue)
                gfx.DrawRectangle(new XSolidBrush(bg.Value), totBoxX, y, totBoxW, 22);
            gfx.DrawLine(new XPen(BorderGray, 0.3), totBoxX, y, totBoxX + totBoxW, y);
            var lf = bold ? fontBodyBold : fontBody;
            var vf = bold ? fontBodyBold : fontBody;
            var lc = bold ? DarkText : MutedText;
            gfx.DrawString(label, lf, new XSolidBrush(lc),
                new XRect(totBoxX + 8, y + 5, 120, 14), XStringFormats.TopLeft);
            gfx.DrawString(val, vf, bold ? new XSolidBrush(GreenAccent) : new XSolidBrush(DarkText),
                new XRect(totBoxX, y + 5, totBoxW - 8, 14), XStringFormats.TopRight);
            y += 22;
        }

        gfx.DrawRectangle(new XPen(BorderGray, 0.5), totBoxX, y, totBoxW, 22 * 4);
        DrawTotalRow("Sub Total",                  $"₹ {booking.SubTotal:N2}");
        DrawTotalRow($"GST @ {booking.GstRate:0.##}%", $"₹ {booking.GstAmount:N2}");
        DrawTotalRow("GRAND TOTAL",                $"₹ {booking.GrandTotal:N2}", true, LightGray);

        y += 20;

        // ── NOTES ────────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(booking.Notes))
        {
            gfx.DrawString("Notes:", fontBodyBold, new XSolidBrush(DarkText),
                new XRect(ml, y, cw, 14), XStringFormats.TopLeft);
            y += 14;
            gfx.DrawString(booking.Notes, fontBody, new XSolidBrush(MutedText),
                new XRect(ml, y, cw, 30), XStringFormats.TopLeft);
            y += 30;
        }

        // ── FOOTER ───────────────────────────────────────────────────
        double footerY = h - 50;
        gfx.DrawLine(new XPen(BorderGray, 0.5), ml, footerY, ml + cw, footerY);
        gfx.DrawString("Thank you for staying with us. We hope to welcome you again soon!",
            fontSmall, new XSolidBrush(MutedText),
            new XRect(ml, footerY + 8, cw, 14), XStringFormats.TopLeft);
        gfx.DrawString($"This is a computer-generated invoice. | {HotelName}",
            fontSmall, new XSolidBrush(MutedText),
            new XRect(ml, footerY + 22, cw, 14), XStringFormats.TopLeft);

        doc.Save(path);
        return path;
    }

    private static void DrawTableRow(XGraphics gfx, double x, double y, double[] widths,
        XColor bg, XFont font, params string[] cells)
    {
        double totalW = widths.Sum();
        gfx.DrawRectangle(new XSolidBrush(bg), x, y, totalW, 24);
        gfx.DrawLine(new XPen(XColor.FromArgb(200, 210, 230), 0.3), x, y + 24, x + totalW, y + 24);

        double cx = x;
        for (int i = 0; i < cells.Length; i++)
        {
            var fmt = i == 0 ? XStringFormats.TopLeft : XStringFormats.TopRight;
            double pad = i == 0 ? 6 : 0;
            gfx.DrawString(cells[i], font, new XSolidBrush(XColor.FromArgb(26, 26, 46)),
                new XRect(cx + pad, y + 6, widths[i] - pad - 6, 14), fmt);
            cx += widths[i];
        }
    }
}
