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