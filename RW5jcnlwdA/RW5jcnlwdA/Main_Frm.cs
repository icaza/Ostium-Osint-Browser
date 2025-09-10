using System.Runtime.InteropServices;

namespace RW5jcnlwdA
{
    public partial class Main_Frm : Form
    {
        #region Var_

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll", SetLastError = true)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
        private static extern bool ReleaseCapture();
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

        #endregion

        public Main_Frm()
        {
            InitializeComponent();

            MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            Encrypt_Pnl.MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            Decrypt_Pnl.MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            Encrypt_Pnl.DragDrop += new DragEventHandler(Encrypt_Pnl_DragDrop);
            Encrypt_Pnl.DragEnter += new DragEventHandler(Encrypt_Pnl_DragEnter);
            Decrypt_Pnl.DragDrop += new DragEventHandler(Decrypt_Pnl_DragDrop);
            Decrypt_Pnl.DragEnter += new DragEventHandler(Decrypt_Pnl_DragEnter);
            Pwd_Txt.DoubleClick += new EventHandler(Pwd_Txt_DoubleClicked);
        }

        void Main_Frm_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        void Encrypt_Pnl_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                if (string.IsNullOrEmpty(Pwd_Txt.Text))
                {
                    Pwd_Txt.BackColor = Color.Red;
                    MessageBox.Show("A password is always better :)");
                    Pwd_Txt.BackColor = Color.FromArgb(41, 44, 51);
                    return;
                }

                Encrypt_Pnl.BackColor = Color.Lime;

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;

                foreach (string file in files)
                {
                    string inputFile = Path.GetFullPath(file);
                    string strName = Path.GetFileName(inputFile);
                    string? dir = Path.GetDirectoryName(inputFile) ?? Environment.CurrentDirectory;
                    string encryptedFile = Path.Combine(dir, "Encrypted_" + strName);

                    Thread encryptThread = new(() => EncryptThread(inputFile, encryptedFile));
                    encryptThread.Start();
                }
            }
        }

        void EncryptThread(string inputFile, string outputFile)
        {
            try
            {
                var argEx = FileEnCryptor.GetArgumentOutOfRangeException2();
                FileEnCryptor.EncryptFile(inputFile, outputFile, Pwd_Txt.Text, argEx);

                Encrypt_Pnl.BackColor = Color.FromArgb(41, 44, 51);

                if (DeleteFile_Chk.Checked)
                    DeleteFile(inputFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error encrypting {inputFile}: {ex}");
                Encrypt_Pnl.BackColor = Color.FromArgb(41, 44, 51);
            }
        }

        void Encrypt_Pnl_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        void Decrypt_Pnl_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                if (string.IsNullOrEmpty(Pwd_Txt.Text))
                {
                    Pwd_Txt.BackColor = Color.Red;
                    MessageBox.Show("A password is always better :)");
                    Pwd_Txt.BackColor = Color.FromArgb(41, 44, 51);
                    return;
                }

                Decrypt_Pnl.BackColor = Color.Lime;

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;

                foreach (string file in files)
                {
                    string inputFile = Path.GetFullPath(file);
                    string strName = Path.GetFileName(inputFile);
                    string? dir = Path.GetDirectoryName(inputFile) ?? Environment.CurrentDirectory;
                    string decryptedFile = Path.Combine(dir, "Decrypted_" + strName);

                    Thread decryptThread = new(() => DecryptThread(inputFile, decryptedFile));
                    decryptThread.Start();
                }
            }
        }

        void DecryptThread(string inputFile, string outputFile)
        {
            try
            {
                var argEx = FileEnCryptor.GetArgumentOutOfRangeException2();
                FileEnCryptor.DecryptFile(inputFile, outputFile, Pwd_Txt.Text, argEx);

                Decrypt_Pnl.BackColor = Color.FromArgb(41, 44, 51);

                if (DeleteFile_Chk.Checked)
                    DeleteFile(inputFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when decrypting {inputFile}: {ex}");
                Decrypt_Pnl.BackColor = Color.FromArgb(41, 44, 51);
            }
        }

        void Decrypt_Pnl_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private static void DeleteFile(string inputFile)
        {
            DeleteFileAsync(inputFile).Wait();
        }

        static async Task DeleteFileAsync(string filename, int maxAttempts = 10, int delayMs = 1000)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    await Task.Delay(delayMs);
                    await Task.Run(() => File.Delete(filename));
                    return;
                }
                catch (IOException)
                { }
            }
            throw new TimeoutException($"Unable to delete file {filename} after {maxAttempts} attempts.");
        }

        void Pwd_Txt_DoubleClicked(object? sender, EventArgs e)
        {
            Pwd_Txt.UseSystemPasswordChar = !Pwd_Txt.UseSystemPasswordChar;
        }

        void Close_Btn_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
