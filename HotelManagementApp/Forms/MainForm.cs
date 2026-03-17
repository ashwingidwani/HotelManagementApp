using HotelInvoiceApp.Services;

namespace HotelInvoiceApp.Forms;

public class MainForm : Form
{
    private TabControl _tabs = null!;
    private TabPage _tabNewBooking  = null!;
    private TabPage _tabBookings    = null!;
    private TabPage _tabInvoices    = null!;

    public MainForm()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        Text            = "Hotel Invoice Manager";
        Size            = new Size(1100, 750);
        MinimumSize     = new Size(900, 650);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Color.FromArgb(245, 247, 250);
        Font            = new Font("Segoe UI", 9.5f);

        // ── TOP HEADER BAR ──
        var header = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 60,
            BackColor = Color.FromArgb(30, 64, 115),
        };
        var lblTitle = new Label
        {
            Text      = "🏨  Hotel Invoice Manager",
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 16, FontStyle.Bold),
            AutoSize  = true,
            Location  = new Point(16, 14),
        };
        header.Controls.Add(lblTitle);
        Controls.Add(header);

        // ── TAB CONTROL ──
        _tabs = new TabControl
        {
            Dock     = DockStyle.Fill,
            Font     = new Font("Segoe UI", 10f),
            Padding  = new Point(18, 6),
        };

        _tabNewBooking = new TabPage("  ➕  New Booking  ");
        _tabBookings   = new TabPage("  📋  All Bookings  ");
        _tabInvoices   = new TabPage("  🧾  Invoices  ");

        _tabs.TabPages.AddRange(new[] { _tabNewBooking, _tabBookings, _tabInvoices });
        Controls.Add(_tabs);

        _tabs.Selected += (s, e) => RefreshActiveTab(e.TabPage);

        LoadNewBookingTab();
        LoadBookingsTab();
        LoadInvoicesTab();
    }

    private void RefreshActiveTab(TabPage? tab)
    {
        if (tab == _tabBookings)
            (_tabBookings.Controls[0] as BookingsListPanel)?.Refresh();
        else if (tab == _tabInvoices)
            (_tabInvoices.Controls[0] as InvoicesListPanel)?.Refresh();
    }

    private void LoadNewBookingTab()
    {
        var panel = new NewBookingPanel();
        panel.InvoiceGenerated += () =>
        {
            (_tabBookings.Controls[0] as BookingsListPanel)?.Refresh();
            (_tabInvoices.Controls[0] as InvoicesListPanel)?.Refresh();
        };
        panel.Dock = DockStyle.Fill;
        _tabNewBooking.Controls.Add(panel);
    }

    private void LoadBookingsTab()
    {
        var panel = new BookingsListPanel { Dock = DockStyle.Fill };
        _tabBookings.Controls.Add(panel);
    }

    private void LoadInvoicesTab()
    {
        var panel = new InvoicesListPanel { Dock = DockStyle.Fill };
        _tabInvoices.Controls.Add(panel);
    }
}
