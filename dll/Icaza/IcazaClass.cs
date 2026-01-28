using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Icaza
{
    public class IcazaClass
    {
        private const string DEFAULT_ERROR_LOG_FILENAME = "Error.log";
        private const int SEPARATOR_WIDTH = 39;

        /// <summary>
        /// Opens a dialog box to select a directory.
        /// </summary>
        /// <param name="description">Description displayed in the dialog box</param>
        /// <param name="rootFolder">Starting root folder</param>
        /// <returns>Path of the selected directory or empty string if cancelled</returns>
        public string Dirselect(string description = "Select directory", Environment.SpecialFolder rootFolder = Environment.SpecialFolder.Desktop)
        {
            using (FolderBrowserDialog fB = new FolderBrowserDialog())
            {
                fB.RootFolder = rootFolder;
                fB.Description = description;
                fB.ShowNewFolderButton = true;

                if (fB.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fB.SelectedPath))
                {
                    return fB.SelectedPath;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Saves an error message to a log file.
        /// </summary>
        /// <param name="Data">Error data</param>
        /// <param name="Message">Error message</param>
        /// <param name="selform">Form name/error source</param>
        /// <param name="dir">Directory where to save the log file</param>
        public void ErrorLog(string Data, string Message, string selform, string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
            {
                throw new ArgumentException("The directory cannot be empty.", nameof(dir));
            }

            if (string.IsNullOrWhiteSpace(selform))
            {
                selform = "Unknown";
            }

            if (!dir.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !dir.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                dir += Path.DirectorySeparatorChar;
            }

            string path = Path.Combine(dir, DEFAULT_ERROR_LOG_FILENAME);

            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
                {
                    sw.WriteLine(DateTime.Now.ToString("F"));
                    sw.WriteLine(new string('-', SEPARATOR_WIDTH));
                    sw.WriteLine($"-               {selform}              -");
                    sw.WriteLine(new string('-', SEPARATOR_WIDTH));

                    if (!string.IsNullOrWhiteSpace(Data))
                    {
                        sw.WriteLine(Data);
                    }

                    if (!string.IsNullOrWhiteSpace(Message))
                    {
                        sw.WriteLine(Message);
                    }

                    sw.WriteLine(new string('-', SEPARATOR_WIDTH));
                    sw.WriteLine();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Access denied to write in '{path}'.", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Error writing to the log file '{path}'.", ex);
            }
        }

        /// <summary>
        /// Opens a dialog box to select a file.
        /// </summary>
        /// <param name="initialdir">Initial directory</param>
        /// <param name="xfilter">File type filter (e.g., "Text files (*.txt)|*.txt")</param>
        /// <param name="filterindex">Default filter index</param>
        /// <returns>Full path of the selected file or empty string if cancelled</returns>
        public string Fileselect(string initialdir = "", string xfilter = "All files (*.*)|*.*", int filterindex = 1)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (!string.IsNullOrWhiteSpace(initialdir) && Directory.Exists(initialdir))
                {
                    openFileDialog.InitialDirectory = initialdir;
                }
                else
                {
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }

                openFileDialog.Filter = xfilter;
                openFileDialog.FilterIndex = filterindex;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog.FileName))
                {
                    return openFileDialog.FileName;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Opens a dialog box to save a file.
        /// </summary>
        /// <param name="filter">File type filter (e.g., "Text files (*.txt)|*.txt")</param>
        /// <param name="filterindex">Default filter index</param>
        /// <param name="defaultFileName">Default file name</param>
        /// <returns>Full path of the file to save or empty string if cancelled</returns>
        public string Savefiledialog(string filter = "All files (*.*)|*.*", int filterindex = 1, string defaultFileName = "")
        {
            using (SaveFileDialog SFD = new SaveFileDialog())
            {
                SFD.Filter = filter;
                SFD.FilterIndex = filterindex;
                SFD.RestoreDirectory = true;
                SFD.OverwritePrompt = true;
                SFD.CheckPathExists = true;

                if (!string.IsNullOrWhiteSpace(defaultFileName))
                {
                    SFD.FileName = defaultFileName;
                }

                if (SFD.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(SFD.FileName))
                {
                    return SFD.FileName;
                }

                return string.Empty;
            }
        }
    }
}