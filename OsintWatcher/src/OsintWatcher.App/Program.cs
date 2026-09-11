using System;
using System.Windows.Forms;

namespace OsintWatcher.App;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.ThreadException += (_, e) =>
            MessageBox.Show("Unexpected error: " + e.Exception.Message, "OSINT Watcher",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        Application.Run(new MainForm());
    }
}
