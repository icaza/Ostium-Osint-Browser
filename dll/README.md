# Icaza.dll

using Icaza;

### Fbddll.dll

**description**: _Open folder browser dialogue to select save directory._

use:

IcazaClass selectdir = new IcazaClass();
string dirselect = selectdir.Dirselect();

### Errordll.dll

**description**: _Send error message in text file._

var   => (string, string, string, string)
value => Section Error, Error Message, Form Name, Directory Error File

use:

IcazaClass senderror = new IcazaClass();
senderror.ErrorLog("Error! FunctionName: ", ex.Message, "Main_Frm", Application.StartupPath + @"\");

### Ofddll.dll

**description**: _Open file directory._

var   => (string, string, int)
value => Initial Directory, Filter, FilterIndex

use:

IcazaClass openfile = new IcazaClass();
string fileopen = openfile.Fileselect("c:\\", "txt files (*.txt)|*.txt|All files (*.*)|*.*", 2);

### Sfddll.dll

**description**: _Save file dialog_.

var   => (string, int)
value => Filter, FilterIndex

use:

IcazaClass savefiledialog = new IcazaClass();
string savefile = savefiledialog.Savefiledialog("All files (*.*)|*.*", 2);

-------------------------------------------------------------------------------------

# Dirsize.dll

using Dirsize;

**description**: _Return size directory._

var   => (string)
value => Directory Name

use:

        readonly ReturnSize sizedireturn = new ReturnSize();

        async void button_Click(object sender, EventArgs e)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"C:\");
            long dirSize = await Task.Run(() => dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));
            MessageBox.Show(sizedireturn.GetSizeName(long.Parse(Convert.ToString(dirSize))));
        }

-------------------------------------------------------------------------------------

# Errordll.dll

using Errordll;

**description**: _Send error message in text file._

var   => (string, string, string, string)
value => Section Error, Error Message, Form Name, Directory Error File

use:

Errorsend senderror = new Errorsend();
senderror.ErrorLog("Error! FunctionName: ", ex.Message, "Main_Frm", Application.StartupPath + @"\");

-------------------------------------------------------------------------------------

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
