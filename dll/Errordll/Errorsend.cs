using System;
using System.IO;
using System.Text;

namespace Errordll
{
    public class Errorsend
    {
        private const string DEFAULT_ERROR_LOG_FILENAME = "Error.log";
        private const int SEPARATOR_WIDTH = 39;

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
                throw new ArgumentException("Le répertoire ne peut pas être vide.", nameof(dir));
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
                throw new UnauthorizedAccessException($"Accès refusé pour écrire dans '{path}'.", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Erreur d'écriture dans le fichier log '{path}'.", ex);
            }
        }
    }
}
