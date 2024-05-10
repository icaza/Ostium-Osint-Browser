using System;
using System.IO;
using System.Windows.Forms;

namespace Icaza
{
    public class IcazaClass
    {
        //
        // Fbddll
        //
        // description: Open folder browser dialogue to select save directory.
        //
        public string Dirselect()
        {
            FolderBrowserDialog fB = new FolderBrowserDialog()
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                Description = "Select directory"
            };
            fB.ShowDialog();
            if (string.IsNullOrEmpty(fB.SelectedPath))
            {
                return "";
            }
            else
            {
                return fB.SelectedPath;
            }
        }
        //
        // Errordll
        //
        // description: Send error message in text file.
        //
        public void ErrorLog(string Data, string Message, string selform, string dir)
        {
            string path = dir + "Error.log";
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(DateTime.Now.ToString("F"));
                sw.WriteLine("".PadRight(39, '-'));
                sw.WriteLine("-               " + selform + "              -");
                sw.WriteLine("".PadRight(39, '-'));
                sw.WriteLine(Data);
                sw.WriteLine(Message);
                sw.WriteLine("".PadRight(39, '-'));
            }
        }
        //
        // Ofddll
        //
        // description: Open file directory.
        //
        public string Fileselect(string initialdir, string xfilter, int filterindex)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = initialdir;
                openFileDialog.Filter = xfilter;
                openFileDialog.FilterIndex = filterindex;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
                else
                {
                    return "";
                }
            }
        }
        //
        // Sfddll
        //
        // description: Save file dialog.
        //
        public string Savefiledialog(string filter, int filterindex)
        {
            SaveFileDialog SFD = new SaveFileDialog
            {
                Filter = filter,
                FilterIndex = filterindex,
                RestoreDirectory = true
            };

            if (SFD.ShowDialog() == DialogResult.OK)
            {
                return SFD.FileName;
            }
            else
            {
                return "";
            }
        }
    }
}
