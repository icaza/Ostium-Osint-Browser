using System;
using System.Windows.Forms;

namespace Ostium
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string arg = (args != null && args.Length > 0) ? args[0] : string.Empty;

            Application.Run(new Main_Frm(arg));
        }
    }
}
