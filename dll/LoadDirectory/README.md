# LoadDirectory.dll

using LoadDirectory;

**description**: _Load files from a directory into ListBox ComboBox and ToolStripComboBox._

var   => (string, string, string, object)
value => Directory select, File Extension, Object Type, Object select

use:

- ListBox

Loaddir loadfiledir = new Loaddir();
loadfiledir.LoadFileDirectory(@"C:\", "exe", "lst", listBox1);

- ComboBox

Loaddir loadfiledir = new Loaddir();
loadfiledir.LoadFileDirectory(@"C:\", "exe", "cbx", comboBox1);

- ToolStripComboBox

Loaddir loadfiledir = new Loaddir();
loadfiledir.LoadFileDirectory(@"C:\", "exe", "cbxts", toolStripComboBox1);

- ToolStripSplitButton

Loaddir loadfiledir = new Loaddir();
loadfiledir.LoadFileDirectory(@"C:\", "exe", "splitb", ToolStripSplitButton1);
