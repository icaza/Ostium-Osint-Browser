# Dirsize.dll

using Dirsize;

### Dirsize.dll

_description: Return size directory._

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
