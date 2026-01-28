using System;
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
        public ToolStripSplitButton SplitButton_Obj;

        public void LoadFileDirectory(string dirDirectory, string extExtension, string objectype, object objectsend)
        {
            if (string.IsNullOrWhiteSpace(dirDirectory))
                throw new ArgumentException("The directory cannot be empty.", nameof(dirDirectory));

            if (!Directory.Exists(dirDirectory))
                throw new DirectoryNotFoundException($"The directory '{dirDirectory}' does not exist.");

            if (string.IsNullOrWhiteSpace(extExtension))
                throw new ArgumentException("The extension cannot be empty.", nameof(extExtension));

            extExtension = extExtension.TrimStart('.');

            switch (objectype?.ToLower())
            {
                case "lst":
                    ListBox_Obj = objectsend as ListBox;
                    if (ListBox_Obj == null)
                        throw new ArgumentException("The object sent is not a ListBox.", nameof(objectsend));
                    ListBox_Obj.Items.Clear();
                    break;

                case "cbxts":
                    ComboBoxTs_Obj = objectsend as ToolStripComboBox;
                    if (ComboBoxTs_Obj == null)
                        throw new ArgumentException("The item sent is not a ToolStripComboBox.", nameof(objectsend));
                    ComboBoxTs_Obj.Items.Clear();
                    break;

                case "cbx":
                    ComboBox_Obj = objectsend as ComboBox;
                    if (ComboBox_Obj == null)
                        throw new ArgumentException("The item sent is not a ComboBox.", nameof(objectsend));
                    ComboBox_Obj.Items.Clear();
                    break;

                case "splitb":
                    SplitButton_Obj = objectsend as ToolStripSplitButton;
                    if (SplitButton_Obj == null)
                        throw new ArgumentException("The item sent is not a ToolStripSplitButton.", nameof(objectsend));
                    SplitButton_Obj.DropDownItems.Clear();
                    break;

                default:
                    throw new ArgumentException($"Object type '{objectype}' not recognized. Use 'lst', 'cbx', 'cbxts' 'splitb' ou .", nameof(objectype));
            }

            try
            {
                var files = Directory.EnumerateFiles(dirDirectory, $"*.{extExtension}", SearchOption.AllDirectories)
                                    .Select(file => Path.GetFileName(file))
                                    .OrderBy(fileName => fileName);

                foreach (var strExt in files)
                {
                    switch (objectype.ToLower())
                    {
                        case "lst":
                            ListBox_Obj.Items.Add(strExt);
                            break;

                        case "cbxts":
                            ComboBoxTs_Obj.Items.Add(strExt);
                            break;

                        case "cbx":
                            ComboBox_Obj.Items.Add(strExt);
                            break;

                        case "splitb":
                            SplitButton_Obj.DropDownItems.Add(strExt);
                            break;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Access denied to the directory '{dirDirectory}'.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading files : {ex.Message}", ex);
            }
        }
    }
}