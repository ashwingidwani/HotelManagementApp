using HotelInvoiceApp.Services;

namespace HotelInvoiceApp.Forms;

public class InvoicesListPanel : Panel
{
    private DataGridView _grid = null!;

    public InvoicesListPanel()
    {
        BuildUI();
        Refresh();
    }

    private void BuildUI()
    {
        var top = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.White };

        var lbl = new Label { Text = "All generated invoices are listed below. Double-click a row to open the PDF.",
            AutoSize = true, Location = new Point(12, 16), Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(80, 80, 100) };

        var btnRefresh = new Button { Text = "🔄 Refresh", Location = new Point(680, 11), Width = 100, Height = 28,
            BackColor = Color.FromArgb(46, 117, 182), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9.5f) };
        btnRefresh.Click += (s, e) => Refresh();

        top.Controls.AddRange(new Control[] { lbl, btnRefresh });
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

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Invoice Number", Name = "InvNum",  FillWeight = 18 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Invoice Date",   Name = "InvDate", FillWeight = 14 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Booking Ref",    Name = "BookRef", FillWeight = 16 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Guest Name",     Name = "Guest",   FillWeight = 20 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "PDF Path",       Name = "Pdf",     FillWeight = 32 });

        // Open PDF on double-click
        _grid.CellDoubleClick += (s, e) =>
        {
            if (e.RowIndex < 0) return;
            string path = _grid.Rows[e.RowIndex].Cells["Pdf"].Value?.ToString() ?? "";
            if (File.Exists(path))
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true });
            else
                MessageBox.Show("PDF file not found at:\n" + path, "File Not Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        };

        Controls.Add(_grid);
    }

    public override void Refresh()
    {
        base.Refresh();
        try
        {
            var repo     = new InvoiceRepository();
            var invoices = repo.GetAll();

            _grid.Rows.Clear();
            foreach (var inv in invoices)
            {
                _grid.Rows.Add(
                    inv.InvoiceNumber,
                    inv.InvoiceDate.ToString("dd MMM yyyy"),
                    inv.Booking?.BookingRef ?? $"Booking #{inv.BookingId}",
                    inv.Booking?.Guest?.FullName ?? "—",
                    inv.PdfPath);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading invoices:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
