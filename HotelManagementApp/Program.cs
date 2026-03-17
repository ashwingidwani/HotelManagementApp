using HotelInvoiceApp.Forms;
using HotelInvoiceApp.Database;

namespace HotelInvoiceApp;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Ensure DB and tables exist on startup
        DatabaseSetup.Initialize();

        Application.Run(new MainForm());
    }
}
