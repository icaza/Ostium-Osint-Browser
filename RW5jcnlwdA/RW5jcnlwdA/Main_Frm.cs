using System.Runtime.InteropServices;
using System.Text;

namespace RW5jcnlwdA
{
    public partial class Main_Frm : Form
    {
        #region Win32 API

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

        #region Private Fields

        readonly SemaphoreSlim _operationSemaphore = new(Environment.ProcessorCount); // Limit concurrent operations
        readonly CancellationTokenSource _cancellationTokenSource = new();
        int _activeOperations = 0;

        // Constants for better maintainability
        static readonly Color DefaultPanelColor = Color.FromArgb(41, 44, 51);
        static readonly Color ErrorColor = Color.Red;
        static readonly Color SuccessColor = Color.Lime;
        static readonly Color ProcessingColor = Color.Orange;

        #endregion

        #region Main_

        public Main_Frm()
        {
            InitializeComponent();
            InitializeEventHandlers();
            SetupDragAndDrop();

            FormClosing += Main_Frm_FormClosing;
        }

        void InitializeEventHandlers()
        {
            MouseDown += Main_Frm_MouseDown;
            Encrypt_Pnl.MouseDown += Main_Frm_MouseDown;
            Decrypt_Pnl.MouseDown += Main_Frm_MouseDown;

            Encrypt_Pnl.DragDrop += Encrypt_Pnl_DragDrop;
            Encrypt_Pnl.DragEnter += Encrypt_Pnl_DragEnter;
            Decrypt_Pnl.DragDrop += Decrypt_Pnl_DragDrop;
            Decrypt_Pnl.DragEnter += Decrypt_Pnl_DragEnter;

            Pwd_Txt.DoubleClick += Pwd_Txt_DoubleClicked;
            Pwd_Txt.KeyPress += Pwd_Txt_KeyPress;
        }

        void SetupDragAndDrop()
        {
            Encrypt_Pnl.AllowDrop = true;
            Decrypt_Pnl.AllowDrop = true;
        }

        void Main_Frm_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        async void Main_Frm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_activeOperations > 0)
            {
                var result = MessageBox.Show(
                    $"There are {_activeOperations} operations in progress. Do you want to cancel them and exit?",
                    "Operations in Progress",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            _cancellationTokenSource.Cancel();

            try
            {
                await Task.Delay(2000, CancellationToken.None); // Don't use cancelled token here
            }
            catch
            {
                // Ignore timeout during shutdown
            }

            _operationSemaphore?.Dispose();
            _cancellationTokenSource?.Dispose();
        }

        #endregion

        #region Password_

        void Pwd_Txt_DoubleClicked(object? sender, EventArgs e)
        {
            Pwd_Txt.UseSystemPasswordChar = !Pwd_Txt.UseSystemPasswordChar;
        }

        void Pwd_Txt_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (Pwd_Txt.BackColor == ErrorColor)
            {
                Pwd_Txt.BackColor = DefaultPanelColor;
            }
        }

        bool ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(Pwd_Txt.Text))
            {
                ShowPasswordError("A password is required for encryption/decryption operations.");
                return false;
            }

            if (Pwd_Txt.Text.Length < 8)
            {
                ShowPasswordError("Password should be at least 8 characters long for better security.");
                return false;
            }

            return true;
        }

        void ShowPasswordError(string message)
        {
            Pwd_Txt.BackColor = ErrorColor;
            MessageBox.Show(message, "Password Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            _ = Task.Delay(2000).ContinueWith(_ =>
            {
                if (InvokeRequired)
                    Invoke(() => Pwd_Txt.BackColor = DefaultPanelColor);
                else
                    Pwd_Txt.BackColor = DefaultPanelColor;
            });
        }

        #endregion

        #region Encryption_

        void Encrypt_Pnl_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.All(File.Exists))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        async void Encrypt_Pnl_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) != true)
                return;

            if (!ValidatePassword())
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;

            var validFiles = files.Where(File.Exists).ToArray();

            if (validFiles.Length == 0)
            {
                MessageBox.Show("No valid files found to encrypt.", "Invalid Files",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (validFiles.Length != files.Length)
            {
                var result = MessageBox.Show(
                    $"Some files don't exist or are inaccessible. Continue with {validFiles.Length} valid files?",
                    "Some Files Invalid", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            string password = Pwd_Txt.Text;

            await ProcessFilesAsync(validFiles, password, isEncryption: true);
        }

        #endregion

        #region Decryption_

        void Decrypt_Pnl_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.All(f => File.Exists(f) && CouldBeEncryptedFile(f)))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        async void Decrypt_Pnl_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) != true)
                return;

            if (!ValidatePassword())
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;

            var validFiles = files.Where(f => File.Exists(f) && CouldBeEncryptedFile(f)).ToArray();

            if (validFiles.Length == 0)
            {
                MessageBox.Show("No valid encrypted files found.", "Invalid Files",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (validFiles.Length != files.Length)
            {
                var result = MessageBox.Show(
                    $"Some files don't appear to be encrypted files. Continue with {validFiles.Length} valid files?",
                    "Some Files Invalid", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            string password = Pwd_Txt.Text;

            await ProcessFilesAsync(validFiles, password, isEncryption: false);
        }

        #endregion

        #region FileProcess_

        async Task ProcessFilesAsync(string[] files, string password, bool isEncryption)
        {
            var panel = isEncryption ? Encrypt_Pnl : Decrypt_Pnl;
            var operation = isEncryption ? "encryption" : "decryption";

            SetPanelColor(panel, ProcessingColor);

            var results = new List<(string file, bool success, string? error)>();
            var tasks = new List<Task>();

            foreach (string file in files)
            {
                tasks.Add(ProcessSingleFileAsync(file, password, isEncryption, results));
            }

            try
            {
                await Task.WhenAll(tasks);

                var successful = results.Count(r => r.success);
                var failed = results.Count(r => !r.success);

                if (failed == 0)
                {
                    SetPanelColor(panel, SuccessColor);
                }
                else
                {
                    SetPanelColor(panel, ErrorColor);
                    var sb = new StringBuilder();
                    sb.AppendLine($"{operation} completed with {successful} successful and {failed} failed operations:");

                    foreach (var (file, success, error) in results.Where(r => !r.success))
                    {
                        sb.AppendLine($"• {Path.GetFileName(file)}: {error}");
                    }

                    MessageBox.Show(sb.ToString(), $"{operation} Results",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (OperationCanceledException)
            {
                SetPanelColor(panel, ErrorColor);
                MessageBox.Show($"{operation} operation was cancelled.", "Operation Cancelled",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                // Reset panel color
                _ = Task.Delay(1000).ContinueWith(_ =>
                {
                    if (!IsDisposed && !_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        if (InvokeRequired)
                            Invoke(() => SetPanelColor(panel, DefaultPanelColor));
                        else
                            SetPanelColor(panel, DefaultPanelColor);
                    }
                });
            }
        }

        async Task ProcessSingleFileAsync(string inputFile, string password, bool isEncryption,
                                                List<(string file, bool success, string? error)> results)
        {
            await _operationSemaphore.WaitAsync(_cancellationTokenSource.Token);

            try
            {
                Interlocked.Increment(ref _activeOperations);

                string outputFile = GenerateOutputFileName(inputFile, isEncryption);

                await Task.Run(() =>
                {
                    try
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (isEncryption)
                        {
                            FileEnCryptor.EncryptFile(inputFile, outputFile, password);
                        }
                        else
                        {
                            FileEnCryptor.DecryptFile(inputFile, outputFile, password);
                        }

                        if (DeleteFile_Chk.Checked)
                        {
                            SecureDeleteFileAsync(inputFile).Wait(_cancellationTokenSource.Token);
                        }

                        lock (results)
                        {
                            results.Add((inputFile, true, null));
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw; // Re-throw cancellation
                    }
                    catch (Exception ex)
                    {
                        lock (results)
                        {
                            results.Add((inputFile, false, ex.Message));
                        }
                    }
                }, _cancellationTokenSource.Token);
            }
            finally
            {
                Interlocked.Decrement(ref _activeOperations);
                _operationSemaphore.Release();
            }
        }

        #endregion

        static string GenerateOutputFileName(string inputFile, bool isEncryption)
        {
            string directory = Path.GetDirectoryName(inputFile) ?? Environment.CurrentDirectory;
            string fileName = Path.GetFileName(inputFile);

            if (isEncryption)
            {
                return Path.Combine(directory, $"Encrypted_{fileName}");
            }
            else
            {
                if (fileName.StartsWith("Encrypted_"))
                {
                    fileName = fileName["Encrypted_".Length..];
                }
                return Path.Combine(directory, $"Decrypted_{fileName}");
            }
        }

        static bool CouldBeEncryptedFile(string filePath)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs.Length < 4) return false;

                byte[] magic = new byte[4];
                fs.ReadExactly(magic, 0, 4);

                return magic.SequenceEqual("FENC"u8.ToArray());
            }
            catch
            {
                return false; // If we can't read it, assume it's not an encrypted file
            }
        }

        static async Task SecureDeleteFileAsync(string filename, int maxAttempts = 5)
        {
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    if (attempt > 0)
                        await Task.Delay(500 * attempt); // Exponential backoff

                    if (File.Exists(filename))
                    {
                        var fileInfo = new FileInfo(filename);
                        long fileSize = fileInfo.Length;

                        // Overwrite with random data (simple secure delete)
                        using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Write))
                        {
                            byte[] randomBytes = new byte[Math.Min(4096, fileSize)];
                            Random.Shared.NextBytes(randomBytes);

                            for (long pos = 0; pos < fileSize; pos += randomBytes.Length)
                            {
                                int bytesToWrite = (int)Math.Min(randomBytes.Length, fileSize - pos);
                                await fs.WriteAsync(randomBytes.AsMemory(0, bytesToWrite));
                            }

                            await fs.FlushAsync();
                        }

                        File.Delete(filename);
                    }
                    return;
                }
                catch (IOException) when (attempt < maxAttempts - 1)
                {
                    continue;
                }
                catch (UnauthorizedAccessException) when (attempt < maxAttempts - 1)
                {
                    continue;
                }
            }

            throw new InvalidOperationException($"Unable to delete file {filename} after {maxAttempts} attempts.");
        }

        void SetPanelColor(Control panel, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(() => panel.BackColor = color);
            }
            else
            {
                panel.BackColor = color;
            }
        }

        void Close_Btn_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}