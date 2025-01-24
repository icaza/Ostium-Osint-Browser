using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Ostium
{
    public partial class Aes_Frm : Form
    {
        #region Var_

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        #endregion

        public Aes_Frm()
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

        void Main_Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        void Encrypt_Pnl_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (Pwd_Txt.Text == "")
                {
                    Pwd_Txt.BackColor = Color.Red;
                    MessageBox.Show("A password is always better :)");
                    Pwd_Txt.BackColor = Color.FromArgb(41, 44, 51);
                    return;
                }

                Encrypt_Pnl.BackColor = Color.Lime;

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    string inputFile = Path.GetFullPath(file);
                    string strName = Path.GetFileName(inputFile);
                    string encryptedFile = Path.Combine(Path.GetDirectoryName(inputFile),
                                                        "Encrypted_" + strName);

                    Thread encryptThread = new Thread(() => EncryptThread(inputFile, encryptedFile));
                    encryptThread.Start();
                }
            }
        }

        void EncryptThread(string inputFile, string outputFile)
        {
            try
            {
                FileEnCryptor.EncryptFile(inputFile, outputFile, Pwd_Txt.Text);

                Encrypt_Pnl.BackColor = Color.FromArgb(41, 44, 51);

                if (DeleteFile_Chk.Checked)
                    DeleteFile(inputFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error encrypting {inputFile}: {ex.Message}");
                Encrypt_Pnl.BackColor = Color.FromArgb(41, 44, 51);
            }
        }

        void Encrypt_Pnl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        void Decrypt_Pnl_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (Pwd_Txt.Text == "")
                {
                    Pwd_Txt.BackColor = Color.Red;
                    MessageBox.Show("A password is always better :)");
                    Pwd_Txt.BackColor = Color.FromArgb(41, 44, 51);
                    return;
                }

                Decrypt_Pnl.BackColor = Color.Lime;

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    string inputFile = Path.GetFullPath(file);
                    string strName = Path.GetFileName(inputFile);
                    string decryptedFile = Path.Combine(Path.GetDirectoryName(inputFile),
                                                        "Decrypted_" + strName);

                    Thread decryptThread = new Thread(() => DecryptThread(inputFile, decryptedFile));
                    decryptThread.Start();
                }
            }
        }

        void DecryptThread(string inputFile, string outputFile)
        {
            try
            {
                FileEnCryptor.DecryptFile(inputFile, outputFile, Pwd_Txt.Text);

                Decrypt_Pnl.BackColor = Color.FromArgb(41, 44, 51);

                if (DeleteFile_Chk.Checked)
                    DeleteFile(inputFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when decrypting {inputFile}: {ex.Message}");
                Decrypt_Pnl.BackColor = Color.FromArgb(41, 44, 51);
            }
        }

        void Decrypt_Pnl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        async void DeleteFile(string inputFile)
        {
            await DeleteFileAsync(inputFile);
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
                {}
            }
            throw new TimeoutException($"Unable to delete file {filename} after {maxAttempts} attempts.");
        }

        void Pwd_Txt_DoubleClicked(object sender, EventArgs e)
        {
            Pwd_Txt.UseSystemPasswordChar = !Pwd_Txt.UseSystemPasswordChar;
        }

        void Close_Btn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Detail_Lbl_Click(object sender, EventArgs e)
        {
            string message = "Security Characteristics:\n\n" +
                             "• Use of the AES (Advanced Encryption Standard) algorithm with a 256-bit key.\n" +
                             "• Generation of a random salt for each encrypted file.\n" +
                             "• Key derivation from the password and salt using PBKDF2\n  (Password-Based Key Derivation Function 2) with 100,000 iterations.\n" +
                             "• Generation of a random initialization vector (IV) for each encryption operation.\n" +
                             "• Use of CryptoStream to perform block encryption.";

            MessageBox.Show(message, "Security Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
