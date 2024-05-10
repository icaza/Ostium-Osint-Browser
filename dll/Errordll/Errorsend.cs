using System;
using System.IO;

namespace Errordll
{
    public class Errorsend
    {
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
    }
}
