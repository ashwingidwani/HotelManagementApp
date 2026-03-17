using HotelInvoiceApp.Services;
using HotelInvoiceApp.Models;

namespace HotelInvoiceApp.Forms;

public class BookingsListPanel : Panel
{
    private DataGridView _grid = null!;
    private TextBox _txtSearch = null!;

    public BookingsListPanel()
    {
        BuildUI();
        Refresh();
    }

    private void BuildUI()
    {
        var top = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.White, Padding = new Padding(10, 10, 10, 0) };

        var lblSearch = new Label { Text = "🔍  Search:", AutoSize = true, Location = new Point(10, 16),
            Font = new Font("Segoe UI", 9.5f) };
        _txtSearch = new TextBox { Location = new Point(80, 13), Width = 280, Height = 26,
            Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle };
        _txtSearch.TextChanged += (s, e) => Refresh();

        var btnRefresh = new Button { Text = "🔄 Refresh", Location = new Point(375, 11), Width = 100, Height = 28,
            BackColor = Color.FromArgb(46, 117, 182), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9.5f) };
        btnRefresh.Click += (s, e) => Refresh();

        top.Controls.AddRange(new Control[] { lblSearch, _txtSearch, btnRefresh });
        Controls.Add(top);

        _grid = new DataGridView
        {
            Dock                  = DockStyle.Fill,
            ReadOnly              = true,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor       = Color.White,
            BorderStyle           = BorderStyle.None,
            RowHeadersVisible     = false,
            Font                  = new Font("Segoe UI", 9.5f),
            ColumnHeadersHeight   = 36,
            RowTemplate           = { Height = 30 },
            GridColor             = Color.FromArgb(220, 230, 245),
        };
        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 64, 115);
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        _grid.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        _grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 252);

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Booking Ref",   Name = "BookingRef",  FillWeight = 12 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Guest Name",     Name = "GuestName",   FillWeight = 16 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID Type",        Name = "IdType",      FillWeight = 10 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID Number",      Name = "IdNum",       FillWeight = 12 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Room",           Name = "Room",        FillWeight = 8  });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Room Type",      Name = "RoomType",    FillWeight = 12 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Check-In",       Name = "CheckIn",     FillWeight = 10 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Check-Out",      Name = "CheckOut",    FillWeight = 10 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nights",         Name = "Nights",      FillWeight = 6  });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Grand Total (₹)", Name = "GrandTotal", FillWeight = 12 });

        Controls.Add(_grid);
    }

    public override void Refresh()
    {
        base.Refresh();
        try
        {
            var repo     = new BookingRepository();
            var bookings = repo.GetAll();
            var q        = _txtSearch?.Text?.ToLower() ?? "";

            if (!string.IsNullOrEmpty(q))
                bookings = bookings.Where(b =>
                    (b.Guest?.FullName ?? "").ToLower().Contains(q) ||
                    b.BookingRef.ToLower().Contains(q) ||
                    b.RoomNumber.ToLower().Contains(q)).ToList();

            _grid.Rows.Clear();
            foreach (var b in bookings)
            {
                _grid.Rows.Add(
                    b.BookingRef,
                    b.Guest?.FullName,
                    b.Guest?.IdType,
                    b.Guest?.IdNumber,
                    b.RoomNumber,
                    b.RoomType,
                    b.CheckIn.ToString("dd MMM yyyy"),
                    b.CheckOut.ToString("dd MMM yyyy"),
                    b.TotalNights,
                    $"₹ {b.GrandTotal:N2}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading bookings:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
