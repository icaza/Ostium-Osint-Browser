using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LoadDirectory
{
    public class Loaddir
    {
        public ToolStripComboBox ComboBoxTs_Obj;
        public ComboBox ComboBox_Obj;
        public ListBox ListBox_Obj;

        public void LoadFileDirectory(string dirDirectory, string extExtension, string objectype, object objectsend)
        {
            if (objectype == "lst")
            {
                ListBox_Obj = (ListBox)objectsend;
                ListBox_Obj.Items.Clear();
            }

            if (objectype == "cbxts")
            {
                ComboBoxTs_Obj = (ToolStripComboBox)objectsend;
                ComboBoxTs_Obj.Items.Clear();
            }

            if (objectype == "cbx")
            {
                ComboBox_Obj = (ComboBox)objectsend;
                ComboBox_Obj.Items.Clear();
            }

            var files = from file in Directory.EnumerateFiles(dirDirectory, "*." + extExtension, SearchOption.AllDirectories)

                        select new
                        {
                            File = file,
                        };

            foreach (var f in files)
            {
                string strExt = Path.GetFileName(f.File);

                if (objectype == "lst")
                {
                    ListBox_Obj.Items.Add($"{strExt}");
                }

                if (objectype == "cbxts")
                {
                    ComboBoxTs_Obj.Items.Add($"{strExt}");
                }

                if (objectype == "cbx")
                {
                    ComboBox_Obj.Items.Add($"{strExt}");
                }
            }
        }
    }
}
