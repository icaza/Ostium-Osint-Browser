using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SVGviewer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Main_Frm mainForm = new Main_Frm();

            if (args.Length > 0)
            {
                mainForm.SvgFileName = args[0];
            }

            Application.Run(mainForm);
        }
    }
}
