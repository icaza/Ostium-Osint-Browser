namespace OOBpdfC
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Run(new MainForm());
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowErrorMessage("An error occurred in the application..", e.Exception);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                ShowErrorMessage("A critical error has occurred.", ex);
            }
        }

        static void ShowErrorMessage(string title, Exception ex)
        {
            string message = $"{title}\n\n" +
                           $"Message: {ex.Message}\n\n" +
                           $"Type: {ex.GetType().Name}";

            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}