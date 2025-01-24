using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ostium
{
    public partial class Aes_Frm : Form
    {
        public Aes_Frm()
        {
            InitializeComponent();

            Encrypt_Pnl.DragDrop += new DragEventHandler(Encrypt_Pnl_DragDrop);
            Encrypt_Pnl.DragEnter += new DragEventHandler(Encrypt_Pnl_DragEnter);
            Decrypt_Pnl.DragDrop += new DragEventHandler(Decrypt_Pnl_DragDrop);
            Decrypt_Pnl.DragEnter += new DragEventHandler(Decrypt_Pnl_DragEnter);
            Pwd_Txt.DoubleClick += new EventHandler(Pwd_Txt_DoubleClicked);
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
            await DeleteFileWhenReadyAsync(inputFile);
        }

        static async Task DeleteFileWhenReadyAsync(string filename, int maxAttempts = 10, int delayMs = 1000)
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
    }
}
