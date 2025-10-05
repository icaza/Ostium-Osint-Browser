# Errordll.dll

using Errordll;

### Errordll.dll

_description: Send error message in text file._

var   => (string, string, string, string)
value => Section Error, Error Message, Form Name, Directory Error File

use:

Errorsend senderror = new Errorsend();
senderror.ErrorLog("Error! FunctionName: ", ex.Message, "Main_Frm", Application.StartupPath + @"\");
