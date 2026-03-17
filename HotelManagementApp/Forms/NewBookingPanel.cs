using HotelInvoiceApp.Models;
using HotelInvoiceApp.Services;

namespace HotelInvoiceApp.Forms;

public class NewBookingPanel : Panel
{
    public event Action? InvoiceGenerated;

    // Guest fields
    private TextBox _txtName    = null!;
    private TextBox _txtEmail   = null!;
    private TextBox _txtPhone   = null!;
    private ComboBox _cmbIdType = null!;
    private TextBox _txtIdNum   = null!;
    private PictureBox _picId   = null!;
    private Button _btnUpload   = null!;
    private Label _lblFileName  = null!;
    private byte[]? _idImageBytes;
    private string  _idImagePath = "";

    // Booking fields
    private TextBox      _txtRoomNo    = null!;
    private ComboBox     _cmbRoomType  = null!;
    private DateTimePicker _dtpCheckIn = null!;
    private DateTimePicker _dtpCheckOut= null!;
    private NumericUpDown  _nudRate    = null!;
    private NumericUpDown  _nudGst     = null!;
    private TextBox      _txtNotes     = null!;

    // Calculated
    private Label _lblNights   = null!;
    private Label _lblSubTotal = null!;
    private Label _lblGstAmt   = null!;
    private Label _lblGrandTotal = null!;

    public NewBookingPanel()
    {
        AutoScroll = true;
        BuildUI();
    }

    private void BuildUI()
    {
        var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };
        Controls.Add(scroll);

        int y = 10;
        int leftW  = 380;
        int rightX = 430;
        int rightW = 380;

        // ── GUEST INFORMATION ──────────────────────────────────────────
        scroll.Controls.Add(SectionLabel("Guest Information", 10, y));
        y += 30;

        scroll.Controls.Add(FieldLabel("Full Name *", 10, y));
        _txtName = TextInput(10, y + 18, leftW); scroll.Controls.Add(_txtName);

        scroll.Controls.Add(FieldLabel("Email", rightX, y));
        _txtEmail = TextInput(rightX, y + 18, rightW); scroll.Controls.Add(_txtEmail);
        y += 56;

        scroll.Controls.Add(FieldLabel("Phone", 10, y));
        _txtPhone = TextInput(10, y + 18, 180); scroll.Controls.Add(_txtPhone);

        scroll.Controls.Add(FieldLabel("ID Type *", 210, y));
        _cmbIdType = new ComboBox
        {
            Location = new Point(210, y + 18), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9.5f)
        };
        _cmbIdType.Items.AddRange(new[] { "Aadhaar Card", "PAN Card", "Driving License" });
        _cmbIdType.SelectedIndex = 0;
        scroll.Controls.Add(_cmbIdType);

        scroll.Controls.Add(FieldLabel("ID Number *", rightX, y));
        _txtIdNum = TextInput(rightX, y + 18, rightW); scroll.Controls.Add(_txtIdNum);
        y += 56;

        // ID Upload
        scroll.Controls.Add(FieldLabel("Upload ID Document (Aadhaar / PAN / DL) *", 10, y));
        y += 18;

        _btnUpload = new Button
        {
            Text     = "📂  Browse Image...",
            Location = new Point(10, y),
            Width    = 180, Height = 32,
            BackColor = Color.FromArgb(46, 117, 182),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f),
            Cursor    = Cursors.Hand,
        };
        _btnUpload.FlatAppearance.BorderSize = 0;
        _btnUpload.Click += BtnUpload_Click;
        scroll.Controls.Add(_btnUpload);

        _lblFileName = new Label
        {
            Location  = new Point(200, y + 6),
            Size      = new Size(250, 20),
            ForeColor = Color.Gray,
            Text      = "No file selected",
            Font      = new Font("Segoe UI", 9f, FontStyle.Italic),
        };
        scroll.Controls.Add(_lblFileName);
        y += 40;

        _picId = new PictureBox
        {
            Location    = new Point(10, y),
            Size        = new Size(250, 150),
            BorderStyle = BorderStyle.FixedSingle,
            SizeMode    = PictureBoxSizeMode.Zoom,
            BackColor   = Color.FromArgb(240, 242, 245),
        };
        scroll.Controls.Add(_picId);
        y += 166;

        // Divider
        scroll.Controls.Add(Divider(10, y, 800)); y += 20;

        // ── BOOKING DETAILS ────────────────────────────────────────────
        scroll.Controls.Add(SectionLabel("Booking Details", 10, y)); y += 30;

        scroll.Controls.Add(FieldLabel("Room Number *", 10, y));
        _txtRoomNo = TextInput(10, y + 18, 150); scroll.Controls.Add(_txtRoomNo);

        scroll.Controls.Add(FieldLabel("Room Type *", 180, y));
        _cmbRoomType = new ComboBox
        {
            Location = new Point(180, y + 18), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9.5f)
        };
        _cmbRoomType.Items.AddRange(new[] {
            "Standard Single", "Standard Double", "Deluxe Single",
            "Deluxe Double", "Suite", "Executive Suite", "Presidential Suite"
        });
        _cmbRoomType.SelectedIndex = 0;
        scroll.Controls.Add(_cmbRoomType);
        y += 56;

        scroll.Controls.Add(FieldLabel("Check-In Date *", 10, y));
        _dtpCheckIn = new DateTimePicker
        {
            Location = new Point(10, y + 18), Width = 180,
            Format   = DateTimePickerFormat.Short,
            Value    = DateTime.Today,
            Font     = new Font("Segoe UI", 9.5f),
        };
        _dtpCheckIn.ValueChanged += RecalcTotals;
        scroll.Controls.Add(_dtpCheckIn);

        scroll.Controls.Add(FieldLabel("Check-Out Date *", 210, y));
        _dtpCheckOut = new DateTimePicker
        {
            Location = new Point(210, y + 18), Width = 180,
            Format   = DateTimePickerFormat.Short,
            Value    = DateTime.Today.AddDays(1),
            Font     = new Font("Segoe UI", 9.5f),
        };
        _dtpCheckOut.ValueChanged += RecalcTotals;
        scroll.Controls.Add(_dtpCheckOut);

        scroll.Controls.Add(FieldLabel("Total Nights", rightX, y));
        _lblNights = new Label
        {
            Location  = new Point(rightX, y + 18),
            Size      = new Size(100, 26),
            Text      = "1",
            Font      = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 64, 115),
        };
        scroll.Controls.Add(_lblNights);
        y += 56;

        scroll.Controls.Add(FieldLabel("Rate Per Night (₹) *", 10, y));
        _nudRate = new NumericUpDown
        {
            Location      = new Point(10, y + 18), Width = 160,
            Minimum       = 0, Maximum = 999999,
            DecimalPlaces = 2, Value   = 2500,
            Font          = new Font("Segoe UI", 9.5f),
        };
        _nudRate.ValueChanged += RecalcTotals;
        scroll.Controls.Add(_nudRate);

        scroll.Controls.Add(FieldLabel("GST Rate (%) *", 190, y));
        _nudGst = new NumericUpDown
        {
            Location      = new Point(190, y + 18), Width = 100,
            Minimum       = 0, Maximum = 28,
            DecimalPlaces = 2, Value   = 12,
            Font          = new Font("Segoe UI", 9.5f),
        };
        _nudGst.ValueChanged += RecalcTotals;
        scroll.Controls.Add(_nudGst);
        y += 56;

        scroll.Controls.Add(FieldLabel("Special Notes / Instructions", 10, y));
        _txtNotes = new TextBox
        {
            Location   = new Point(10, y + 18),
            Size       = new Size(800, 56),
            Multiline  = true,
            Font       = new Font("Segoe UI", 9.5f),
            BackColor  = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
        };
        scroll.Controls.Add(_txtNotes);
        y += 86;

        // Divider
        scroll.Controls.Add(Divider(10, y, 800)); y += 20;

        // ── TOTALS PANEL ───────────────────────────────────────────────
        scroll.Controls.Add(SectionLabel("Invoice Summary", 10, y)); y += 30;

        var totalsPanel = new Panel
        {
            Location  = new Point(10, y),
            Size      = new Size(380, 130),
            BackColor = Color.FromArgb(245, 247, 250),
            BorderStyle = BorderStyle.FixedSingle,
        };

        int ty = 10;
        void AddTotal(string label, out Label lbl)
        {
            var l1 = new Label { Text = label, Location = new Point(10, ty), Size = new Size(160, 22),
                Font = new Font("Segoe UI", 9.5f), ForeColor = Color.Gray };
            lbl = new Label { Location = new Point(180, ty), Size = new Size(180, 22),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 64, 115),
                TextAlign = ContentAlignment.MiddleRight };
            totalsPanel.Controls.AddRange(new Control[] { l1, lbl });
            ty += 28;
        }

        AddTotal("Sub Total:", out _lblSubTotal);
        AddTotal("GST Amount:", out _lblGstAmt);

        var sep = new Label { Location = new Point(10, ty), Size = new Size(360, 2),
            BorderStyle = BorderStyle.Fixed3D }; totalsPanel.Controls.Add(sep); ty += 8;

        var lGT = new Label { Text = "Grand Total:", Location = new Point(10, ty), Size = new Size(160, 26),
            Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 107, 60) };
        _lblGrandTotal = new Label { Location = new Point(180, ty), Size = new Size(180, 26),
            Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 107, 60),
            TextAlign = ContentAlignment.MiddleRight };
        totalsPanel.Controls.AddRange(new Control[] { lGT, _lblGrandTotal });

        scroll.Controls.Add(totalsPanel);
        y += 150;

        // ── ACTION BUTTONS ─────────────────────────────────────────────
        var btnSave = MakeButton("💾  Save & Generate Invoice", 10, y, Color.FromArgb(30, 107, 60));
        btnSave.Click += BtnSave_Click;
        scroll.Controls.Add(btnSave);

        var btnClear = MakeButton("🔄  Clear Form", 220, y, Color.FromArgb(100, 100, 120));
        btnClear.Click += (s, e) => ClearForm();
        scroll.Controls.Add(btnClear);

        y += 56;
        scroll.AutoScrollMinSize = new Size(0, y + 20);

        RecalcTotals(null, EventArgs.Empty);
    }

    private void BtnUpload_Click(object? sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Title  = "Select ID Document",
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*",
        };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        _idImagePath  = dlg.FileName;
        _idImageBytes = File.ReadAllBytes(_idImagePath);
        _lblFileName.Text = Path.GetFileName(_idImagePath);
        _lblFileName.ForeColor = Color.FromArgb(30, 107, 60);

        _picId.Image = Image.FromFile(_idImagePath);
    }

    private void RecalcTotals(object? sender, EventArgs e)
    {
        int nights = Math.Max(0, (int)(_dtpCheckOut.Value.Date - _dtpCheckIn.Value.Date).TotalDays);
        _lblNights.Text = nights.ToString();

        decimal sub    = (decimal)_nudRate.Value * nights;
        decimal gstAmt = sub * (decimal)_nudGst.Value / 100m;
        decimal grand  = sub + gstAmt;

        _lblSubTotal.Text   = $"₹ {sub:N2}";
        _lblGstAmt.Text     = $"₹ {gstAmt:N2}";
        _lblGrandTotal.Text = $"₹ {grand:N2}";
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (!Validate()) return;

        try
        {
            // Save Guest
            var guestRepo = new GuestRepository();
            var guest = new Guest
            {
                FullName     = _txtName.Text.Trim(),
                Email        = _txtEmail.Text.Trim(),
                Phone        = _txtPhone.Text.Trim(),
                IdType       = _cmbIdType.Text,
                IdNumber     = _txtIdNum.Text.Trim(),
                IdImagePath  = _idImagePath,
                IdImageData  = _idImageBytes,
            };
            int guestId = guestRepo.Save(guest);

            // Calculate
            int     nights   = Math.Max(1, (int)(_dtpCheckOut.Value.Date - _dtpCheckIn.Value.Date).TotalDays);
            decimal subTotal = (decimal)_nudRate.Value * nights;
            decimal gstAmt   = subTotal * (decimal)_nudGst.Value / 100m;
            decimal grand    = subTotal + gstAmt;

            // Save Booking
            var bookingRepo = new BookingRepository();
            var booking = new Booking
            {
                GuestId      = guestId,
                BookingRef   = $"BK-{DateTime.Now:yyyyMMddHHmmss}",
                RoomNumber   = _txtRoomNo.Text.Trim(),
                RoomType     = _cmbRoomType.Text,
                CheckIn      = _dtpCheckIn.Value.Date,
                CheckOut     = _dtpCheckOut.Value.Date,
                RatePerNight = (decimal)_nudRate.Value,
                TotalNights  = nights,
                SubTotal     = subTotal,
                GstRate      = (decimal)_nudGst.Value,
                GstAmount    = gstAmt,
                GrandTotal   = grand,
                Notes        = _txtNotes.Text.Trim(),
                Guest        = guest,
            };
            int bookingId   = bookingRepo.Save(booking);
            booking.BookingId = bookingId;

            // Generate Invoice number
            var invoiceRepo = new InvoiceRepository();
            string invoiceNum = invoiceRepo.GenerateInvoiceNumber();

            // Generate PDF
            string outputFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "HotelInvoices");
            var pdfService = new PdfInvoiceService();
            string pdfPath = pdfService.Generate(booking, invoiceNum, outputFolder);

            // Save Invoice record
            var invoice = new Invoice
            {
                BookingId     = bookingId,
                InvoiceNumber = invoiceNum,
                InvoiceDate   = DateTime.Today,
                PdfPath       = pdfPath,
            };
            invoiceRepo.Save(invoice);

            InvoiceGenerated?.Invoke();

            // Ask to open PDF
            var result = MessageBox.Show(
                $"✅ Invoice {invoiceNum} generated successfully!\n\nSaved to:\n{pdfPath}\n\nWould you like to open it now?",
                "Invoice Generated",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(pdfPath) { UseShellExecute = true });

            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving booking:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private new bool Validate()
    {
        if (string.IsNullOrWhiteSpace(_txtName.Text))
        { ShowError("Guest full name is required."); _txtName.Focus(); return false; }

        if (string.IsNullOrWhiteSpace(_txtIdNum.Text))
        { ShowError("ID number is required."); _txtIdNum.Focus(); return false; }

        if (_idImageBytes == null)
        { ShowError("Please upload the guest's ID document image."); return false; }

        if (string.IsNullOrWhiteSpace(_txtRoomNo.Text))
        { ShowError("Room number is required."); _txtRoomNo.Focus(); return false; }

        if (_dtpCheckOut.Value.Date <= _dtpCheckIn.Value.Date)
        { ShowError("Check-out date must be after check-in date."); return false; }

        if (_nudRate.Value <= 0)
        { ShowError("Rate per night must be greater than 0."); return false; }

        return true;
    }

    private void ShowError(string msg) =>
        MessageBox.Show(msg, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void ClearForm()
    {
        _txtName.Clear(); _txtEmail.Clear(); _txtPhone.Clear();
        _txtIdNum.Clear(); _txtRoomNo.Clear(); _txtNotes.Clear();
        _cmbIdType.SelectedIndex = 0;
        _cmbRoomType.SelectedIndex = 0;
        _dtpCheckIn.Value  = DateTime.Today;
        _dtpCheckOut.Value = DateTime.Today.AddDays(1);
        _nudRate.Value = 2500; _nudGst.Value = 12;
        _idImageBytes = null; _idImagePath = "";
        _picId.Image = null;
        _lblFileName.Text = "No file selected";
        _lblFileName.ForeColor = Color.Gray;
        RecalcTotals(null, EventArgs.Empty);
    }

    // ── UI Helper Factories ──────────────────────────────────────────
    private static Label SectionLabel(string text, int x, int y) => new()
    {
        Text = text, Location = new Point(x, y), AutoSize = true,
        Font = new Font("Segoe UI", 11, FontStyle.Bold),
        ForeColor = Color.FromArgb(30, 64, 115),
    };

    private static Label FieldLabel(string text, int x, int y) => new()
    {
        Text = text, Location = new Point(x, y), AutoSize = true,
        Font = new Font("Segoe UI", 8.5f),
        ForeColor = Color.FromArgb(80, 80, 100),
    };

    private static TextBox TextInput(int x, int y, int w) => new()
    {
        Location    = new Point(x, y), Width = w, Height = 28,
        Font        = new Font("Segoe UI", 9.5f),
        BackColor   = Color.White,
        BorderStyle = BorderStyle.FixedSingle,
    };

    private static Label Divider(int x, int y, int w) => new()
    {
        Location    = new Point(x, y), Size = new Size(w, 2),
        BorderStyle = BorderStyle.Fixed3D,
    };

    private static Button MakeButton(string text, int x, int y, Color color) => new()
    {
        Text      = text, Location = new Point(x, y), Size = new Size(200, 38),
        BackColor = color, ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
        Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
    };
}
