namespace OOBpdfC
{
    public partial class MainForm : Form
    {
        readonly PdfProcessor _pdfProcessor;
        readonly List<string> _selectedFiles = [];
        bool _isProcessing = false;

        public PdfProcessor PdfProcessor => _pdfProcessor;

        public MainForm()
        {
            InitializeComponent();
            _pdfProcessor = new PdfProcessor();
            InitializeUI();
        }

        void InitializeUI()
        {
            AllowDrop = true;
            DragEnter += MainForm_DragEnter;
            DragDrop += MainForm_DragDrop;

            lvFiles.View = View.Details;
            lvFiles.FullRowSelect = true;
            lvFiles.GridLines = true;

            lvFiles.MultiSelect = true;

            UpdateUI();
        }

        void MainForm_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        void MainForm_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files)
            {
                AddFiles(files.Where(f => f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)));
            }
        }

        void BtnAddFiles_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new()
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Multiselect = true,
                Title = "Select PDF files"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddFiles(ofd.FileNames);
            }
        }

        void BtnAddFolder_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog fbd = new()
            {
                Description = "Select a folder containing PDF files",
                ShowNewFolderButton = false
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var pdfFiles = Directory.GetFiles(fbd.SelectedPath, "*.pdf", SearchOption.AllDirectories);
                AddFiles(pdfFiles);
            }
        }

        void AddFiles(IEnumerable<string> files)
        {
            int addedCount = 0;
            int skippedCount = 0;

            foreach (var file in files)
            {
                if (!_selectedFiles.Contains(file))
                {
                    var fileInfo = new FileInfo(file);

                    if (fileInfo.Length > PdfProcessor.MaxFileSizeBytes)
                    {
                        skippedCount++;
                        continue;
                    }

                    _selectedFiles.Add(file);

                    var item = new ListViewItem(Path.GetFileName(file));
                    item.SubItems.Add(FormatFileSize(fileInfo.Length));
                    item.SubItems.Add("On hold");
                    item.Tag = file;
                    item.ImageIndex = 0;

                    lvFiles.Items.Add(item);
                    addedCount++;
                }
            }

            if (skippedCount > 0)
            {
                MessageBox.Show(
                    $"{skippedCount} File(s) ignored (maximum size exceeded) File(s) ignored (maximum size exceeded) {PdfProcessor.MaxFileSizeMb} MB).",
                    "Files ignored",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            lblFileCount.Text = $"{_selectedFiles.Count} selected file(s)";
            UpdateUI();
        }

        void BtnRemoveSelected_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            foreach (ListViewItem item in lvFiles.SelectedItems)
            {
                if (item.Tag is string filePath)
                {
                    _selectedFiles.Remove(filePath);
                }
                lvFiles.Items.Remove(item);
            }

            lblFileCount.Text = $"{_selectedFiles.Count} selected file(s)";
            UpdateUI();
        }

        void BtnClearAll_Click(object sender, EventArgs e)
        {
            _selectedFiles.Clear();
            lvFiles.Items.Clear();
            lblFileCount.Text = "0 selected file(s)";
            UpdateUI();
        }

        async void BtnConvert_Click(object sender, EventArgs e)
        {
            if (_selectedFiles.Count == 0)
            {
                MessageBox.Show("No file selected.", "Select file",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isProcessing = true;
            UpdateUI();

            int successCount = 0;
            int errorCount = 0;

            var options = new ConversionOptions
            {
                OutputFormat = rbBoth.Checked ? OutputFormat.Both :
                              rbTxt.Checked ? OutputFormat.Txt : OutputFormat.Markdown,
                ExtractMetadata = chkMetadata.Checked,
                ExtractImages = chkImages.Checked,
                ExtractTables = chkTables.Checked,
                PreserveFormatting = chkFormatting.Checked
            };

            foreach (var filePath in _selectedFiles.ToList())
            {
                var item = lvFiles.Items.Cast<ListViewItem>()
                    .FirstOrDefault(i => i.Tag as string == filePath);

                if (item != null)
                {
                    item.SubItems[2].Text = "In progress...";
                    item.ImageIndex = 1;
                }

                try
                {
                    await PdfProcessor.ConvertPdfAsync(filePath, options);

                    if (item != null)
                    {
                        item.SubItems[2].Text = "Finished ✓";
                        item.ImageIndex = 2;
                        item.ForeColor = Color.Green;
                    }
                    successCount++;
                }
                catch (Exception ex)
                {
                    if (item != null)
                    {
                        item.SubItems[2].Text = $"Error: {ex.Message}";
                        item.ImageIndex = 3;
                        item.ForeColor = Color.Red;
                    }
                    errorCount++;
                }
            }

            _isProcessing = false;
            UpdateUI();

            string message = $"Conversion complete!\n\n" +
                           $"Success: {successCount}\n" +
                           $"Errors: {errorCount}";

            MessageBox.Show(message, "Result",
                MessageBoxButtons.OK,
                errorCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        void UpdateUI()
        {
            bool hasFiles = _selectedFiles.Count > 0;
            bool hasSelection = lvFiles.SelectedItems.Count > 0;

            btnConvert.Enabled = hasFiles && !_isProcessing;
            btnRemoveSelected.Enabled = hasSelection && !_isProcessing;
            btnClearAll.Enabled = hasFiles && !_isProcessing;
            btnAddFiles.Enabled = !_isProcessing;
            btnAddFolder.Enabled = !_isProcessing;

            rbTxt.Enabled = !_isProcessing;
            rbMd.Enabled = !_isProcessing;
            rbBoth.Enabled = !_isProcessing;
            chkMetadata.Enabled = !_isProcessing;
            chkImages.Enabled = !_isProcessing;
            chkTables.Enabled = !_isProcessing;
            chkFormatting.Enabled = !_isProcessing;
            btnConvert.Enabled = !_isProcessing;
        }

        static string FormatFileSize(long bytes)
        {
            string[] sizes = ["B", "KB", "MB", "GB"];
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        void LvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }
    }
}