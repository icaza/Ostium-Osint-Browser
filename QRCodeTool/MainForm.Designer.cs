// ─────────────────────────────────────────────────────────────────────────────
//  MainForm.Designer.cs
//  Auto-layout for QRCodeTool — generated/maintained manually to match the
//  Visual Studio WinForms Designer contract (partial class + InitializeComponent).
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using System.Windows.Forms;

namespace QRCodeTool
{
    partial class MainForm
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        // ── Tab structure ─────────────────────────────────────────────────────
        private TabControl   tabMain;
        private TabPage      tabPageGenerate;
        private TabPage      tabPageScan;

        // ── Generate tab controls ─────────────────────────────────────────────
        private Panel        pnlGenLeft;
        private Panel        pnlGenRight;

        private Label        lblContentHeader;
        private TextBox      txtContent;

        private Label        lblSizeHeader;
        private NumericUpDown numSize;

        private Label        lblEcHeader;
        private ComboBox     cmbErrorCorrection;

        private Label        lblColorsHeader;
        private Button       btnForeColor;
        private Button       btnBackColor;

        private Button       btnGenerate;
        private Button       btnSaveQR;
        private Label        lblGenStatus;

        private Label        lblGenPreviewHeader;
        private PictureBox   picGenerated;

        // ── Scan tab controls ─────────────────────────────────────────────────
        private Panel        pnlScanLeft;
        private Panel        pnlScanRight;

        private Label        lblScanPreviewHeader;
        private PictureBox   picScanned;

        private Button       btnLoadImage;
        private Label        lblResultHeader;
        private TextBox      txtResult;
        private Button       btnCopyResult;
        private Button       btnOpenUrl;
        private Label        lblScanStatus;
        private Label        lblDragTip;

        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Required method for Designer support — do not modify the contents
        /// of this method with the code editor (put logic in MainForm.cs).
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPageGenerate = new System.Windows.Forms.TabPage();
            this.pnlGenLeft = new System.Windows.Forms.Panel();
            this.lblContentHeader = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.lblSizeHeader = new System.Windows.Forms.Label();
            this.numSize = new System.Windows.Forms.NumericUpDown();
            this.lblEcHeader = new System.Windows.Forms.Label();
            this.cmbErrorCorrection = new System.Windows.Forms.ComboBox();
            this.lblColorsHeader = new System.Windows.Forms.Label();
            this.btnForeColor = new System.Windows.Forms.Button();
            this.btnBackColor = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnSaveQR = new System.Windows.Forms.Button();
            this.lblGenStatus = new System.Windows.Forms.Label();
            this.pnlGenRight = new System.Windows.Forms.Panel();
            this.lblGenPreviewHeader = new System.Windows.Forms.Label();
            this.picGenerated = new System.Windows.Forms.PictureBox();
            this.tabPageScan = new System.Windows.Forms.TabPage();
            this.pnlScanLeft = new System.Windows.Forms.Panel();
            this.lblScanPreviewHeader = new System.Windows.Forms.Label();
            this.picScanned = new System.Windows.Forms.PictureBox();
            this.pnlScanRight = new System.Windows.Forms.Panel();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.lblResultHeader = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnCopyResult = new System.Windows.Forms.Button();
            this.btnOpenUrl = new System.Windows.Forms.Button();
            this.lblScanStatus = new System.Windows.Forms.Label();
            this.lblDragTip = new System.Windows.Forms.Label();
            this.TTS_Tts = new System.Windows.Forms.ToolStrip();
            this.Quit_Btn = new System.Windows.Forms.ToolStripButton();
            this.tabMain.SuspendLayout();
            this.tabPageGenerate.SuspendLayout();
            this.pnlGenLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSize)).BeginInit();
            this.pnlGenRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGenerated)).BeginInit();
            this.tabPageScan.SuspendLayout();
            this.pnlScanLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picScanned)).BeginInit();
            this.pnlScanRight.SuspendLayout();
            this.TTS_Tts.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPageGenerate);
            this.tabMain.Controls.Add(this.tabPageScan);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 18);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Drawing.Point(10, 5);
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(703, 537);
            this.tabMain.TabIndex = 0;
            // 
            // tabPageGenerate
            // 
            this.tabPageGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.tabPageGenerate.Controls.Add(this.pnlGenLeft);
            this.tabPageGenerate.Controls.Add(this.pnlGenRight);
            this.tabPageGenerate.Location = new System.Drawing.Point(4, 28);
            this.tabPageGenerate.Name = "tabPageGenerate";
            this.tabPageGenerate.Padding = new System.Windows.Forms.Padding(8);
            this.tabPageGenerate.Size = new System.Drawing.Size(695, 505);
            this.tabPageGenerate.TabIndex = 0;
            this.tabPageGenerate.Text = "  Generate QR Code  ";
            // 
            // pnlGenLeft
            // 
            this.pnlGenLeft.BackColor = System.Drawing.Color.Transparent;
            this.pnlGenLeft.Controls.Add(this.lblContentHeader);
            this.pnlGenLeft.Controls.Add(this.txtContent);
            this.pnlGenLeft.Controls.Add(this.lblSizeHeader);
            this.pnlGenLeft.Controls.Add(this.numSize);
            this.pnlGenLeft.Controls.Add(this.lblEcHeader);
            this.pnlGenLeft.Controls.Add(this.cmbErrorCorrection);
            this.pnlGenLeft.Controls.Add(this.lblColorsHeader);
            this.pnlGenLeft.Controls.Add(this.btnForeColor);
            this.pnlGenLeft.Controls.Add(this.btnBackColor);
            this.pnlGenLeft.Controls.Add(this.btnGenerate);
            this.pnlGenLeft.Controls.Add(this.btnSaveQR);
            this.pnlGenLeft.Controls.Add(this.lblGenStatus);
            this.pnlGenLeft.Location = new System.Drawing.Point(8, 8);
            this.pnlGenLeft.Name = "pnlGenLeft";
            this.pnlGenLeft.Size = new System.Drawing.Size(316, 490);
            this.pnlGenLeft.TabIndex = 0;
            // 
            // lblContentHeader
            // 
            this.lblContentHeader.AutoSize = true;
            this.lblContentHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblContentHeader.ForeColor = System.Drawing.Color.Silver;
            this.lblContentHeader.Location = new System.Drawing.Point(3, 0);
            this.lblContentHeader.Name = "lblContentHeader";
            this.lblContentHeader.Size = new System.Drawing.Size(91, 15);
            this.lblContentHeader.TabIndex = 0;
            this.lblContentHeader.Text = "Content / Text:";
            // 
            // txtContent
            // 
            this.txtContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtContent.Location = new System.Drawing.Point(3, 20);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtContent.Size = new System.Drawing.Size(310, 98);
            this.txtContent.TabIndex = 1;
            // 
            // lblSizeHeader
            // 
            this.lblSizeHeader.AutoSize = true;
            this.lblSizeHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSizeHeader.ForeColor = System.Drawing.Color.Silver;
            this.lblSizeHeader.Location = new System.Drawing.Point(3, 132);
            this.lblSizeHeader.Name = "lblSizeHeader";
            this.lblSizeHeader.Size = new System.Drawing.Size(96, 15);
            this.lblSizeHeader.TabIndex = 2;
            this.lblSizeHeader.Text = "Image Size (px):";
            // 
            // numSize
            // 
            this.numSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numSize.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numSize.Location = new System.Drawing.Point(3, 152);
            this.numSize.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numSize.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numSize.Name = "numSize";
            this.numSize.Size = new System.Drawing.Size(310, 23);
            this.numSize.TabIndex = 3;
            this.numSize.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // lblEcHeader
            // 
            this.lblEcHeader.AutoSize = true;
            this.lblEcHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEcHeader.ForeColor = System.Drawing.Color.Silver;
            this.lblEcHeader.Location = new System.Drawing.Point(3, 192);
            this.lblEcHeader.Name = "lblEcHeader";
            this.lblEcHeader.Size = new System.Drawing.Size(133, 15);
            this.lblEcHeader.TabIndex = 4;
            this.lblEcHeader.Text = "Error Correction Level:";
            // 
            // cmbErrorCorrection
            // 
            this.cmbErrorCorrection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbErrorCorrection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbErrorCorrection.Items.AddRange(new object[] {
            "L  –  Low  (7 %)",
            "M  –  Medium  (15 %)",
            "Q  –  Quartile  (25 %)",
            "H  –  High  (30 %)"});
            this.cmbErrorCorrection.Location = new System.Drawing.Point(3, 212);
            this.cmbErrorCorrection.Name = "cmbErrorCorrection";
            this.cmbErrorCorrection.Size = new System.Drawing.Size(310, 23);
            this.cmbErrorCorrection.TabIndex = 5;
            // 
            // lblColorsHeader
            // 
            this.lblColorsHeader.AutoSize = true;
            this.lblColorsHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblColorsHeader.ForeColor = System.Drawing.Color.Silver;
            this.lblColorsHeader.Location = new System.Drawing.Point(3, 254);
            this.lblColorsHeader.Name = "lblColorsHeader";
            this.lblColorsHeader.Size = new System.Drawing.Size(89, 15);
            this.lblColorsHeader.TabIndex = 6;
            this.lblColorsHeader.Text = "Module Colors:";
            // 
            // btnForeColor
            // 
            this.btnForeColor.BackColor = System.Drawing.Color.Black;
            this.btnForeColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnForeColor.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btnForeColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForeColor.ForeColor = System.Drawing.Color.White;
            this.btnForeColor.Location = new System.Drawing.Point(3, 274);
            this.btnForeColor.Name = "btnForeColor";
            this.btnForeColor.Size = new System.Drawing.Size(150, 30);
            this.btnForeColor.TabIndex = 7;
            this.btnForeColor.Text = "Foreground";
            this.btnForeColor.UseVisualStyleBackColor = false;
            // 
            // btnBackColor
            // 
            this.btnBackColor.BackColor = System.Drawing.Color.White;
            this.btnBackColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackColor.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btnBackColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackColor.ForeColor = System.Drawing.Color.Black;
            this.btnBackColor.Location = new System.Drawing.Point(163, 274);
            this.btnBackColor.Name = "btnBackColor";
            this.btnBackColor.Size = new System.Drawing.Size(150, 30);
            this.btnBackColor.TabIndex = 8;
            this.btnBackColor.Text = "Background";
            this.btnBackColor.UseVisualStyleBackColor = false;
            // 
            // btnGenerate
            // 
            this.btnGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnGenerate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerate.FlatAppearance.BorderSize = 0;
            this.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerate.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(3, 322);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(310, 38);
            this.btnGenerate.TabIndex = 9;
            this.btnGenerate.Text = "Generate QR Code";
            this.btnGenerate.UseVisualStyleBackColor = false;
            // 
            // btnSaveQR
            // 
            this.btnSaveQR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(230)))));
            this.btnSaveQR.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveQR.Enabled = false;
            this.btnSaveQR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveQR.Location = new System.Drawing.Point(3, 370);
            this.btnSaveQR.Name = "btnSaveQR";
            this.btnSaveQR.Size = new System.Drawing.Size(310, 32);
            this.btnSaveQR.TabIndex = 10;
            this.btnSaveQR.Text = "Save Image";
            this.btnSaveQR.UseVisualStyleBackColor = false;
            // 
            // lblGenStatus
            // 
            this.lblGenStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblGenStatus.Location = new System.Drawing.Point(3, 414);
            this.lblGenStatus.Name = "lblGenStatus";
            this.lblGenStatus.Size = new System.Drawing.Size(310, 40);
            this.lblGenStatus.TabIndex = 11;
            this.lblGenStatus.Text = "Enter content and click Generate.";
            // 
            // pnlGenRight
            // 
            this.pnlGenRight.BackColor = System.Drawing.Color.White;
            this.pnlGenRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGenRight.Controls.Add(this.lblGenPreviewHeader);
            this.pnlGenRight.Controls.Add(this.picGenerated);
            this.pnlGenRight.Location = new System.Drawing.Point(332, 8);
            this.pnlGenRight.Name = "pnlGenRight";
            this.pnlGenRight.Size = new System.Drawing.Size(356, 490);
            this.pnlGenRight.TabIndex = 1;
            // 
            // lblGenPreviewHeader
            // 
            this.lblGenPreviewHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.lblGenPreviewHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblGenPreviewHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblGenPreviewHeader.Location = new System.Drawing.Point(0, 0);
            this.lblGenPreviewHeader.Name = "lblGenPreviewHeader";
            this.lblGenPreviewHeader.Size = new System.Drawing.Size(354, 28);
            this.lblGenPreviewHeader.TabIndex = 0;
            this.lblGenPreviewHeader.Text = "Preview";
            this.lblGenPreviewHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picGenerated
            // 
            this.picGenerated.BackColor = System.Drawing.Color.Black;
            this.picGenerated.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picGenerated.Location = new System.Drawing.Point(0, 0);
            this.picGenerated.Name = "picGenerated";
            this.picGenerated.Padding = new System.Windows.Forms.Padding(10);
            this.picGenerated.Size = new System.Drawing.Size(354, 488);
            this.picGenerated.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picGenerated.TabIndex = 1;
            this.picGenerated.TabStop = false;
            // 
            // tabPageScan
            // 
            this.tabPageScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.tabPageScan.Controls.Add(this.pnlScanLeft);
            this.tabPageScan.Controls.Add(this.pnlScanRight);
            this.tabPageScan.Location = new System.Drawing.Point(4, 28);
            this.tabPageScan.Name = "tabPageScan";
            this.tabPageScan.Padding = new System.Windows.Forms.Padding(8);
            this.tabPageScan.Size = new System.Drawing.Size(695, 505);
            this.tabPageScan.TabIndex = 1;
            this.tabPageScan.Text = "  Scan / Read QR Code  ";
            // 
            // pnlScanLeft
            // 
            this.pnlScanLeft.BackColor = System.Drawing.Color.White;
            this.pnlScanLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlScanLeft.Controls.Add(this.lblScanPreviewHeader);
            this.pnlScanLeft.Controls.Add(this.picScanned);
            this.pnlScanLeft.Location = new System.Drawing.Point(8, 8);
            this.pnlScanLeft.Name = "pnlScanLeft";
            this.pnlScanLeft.Size = new System.Drawing.Size(356, 490);
            this.pnlScanLeft.TabIndex = 0;
            // 
            // lblScanPreviewHeader
            // 
            this.lblScanPreviewHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.lblScanPreviewHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblScanPreviewHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblScanPreviewHeader.Location = new System.Drawing.Point(0, 0);
            this.lblScanPreviewHeader.Name = "lblScanPreviewHeader";
            this.lblScanPreviewHeader.Size = new System.Drawing.Size(354, 28);
            this.lblScanPreviewHeader.TabIndex = 0;
            this.lblScanPreviewHeader.Text = "Image Preview  —  drag & drop an image here";
            this.lblScanPreviewHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picScanned
            // 
            this.picScanned.BackColor = System.Drawing.Color.Black;
            this.picScanned.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picScanned.Location = new System.Drawing.Point(0, 0);
            this.picScanned.Name = "picScanned";
            this.picScanned.Padding = new System.Windows.Forms.Padding(10);
            this.picScanned.Size = new System.Drawing.Size(354, 488);
            this.picScanned.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picScanned.TabIndex = 1;
            this.picScanned.TabStop = false;
            // 
            // pnlScanRight
            // 
            this.pnlScanRight.BackColor = System.Drawing.Color.Transparent;
            this.pnlScanRight.Controls.Add(this.btnLoadImage);
            this.pnlScanRight.Controls.Add(this.lblResultHeader);
            this.pnlScanRight.Controls.Add(this.txtResult);
            this.pnlScanRight.Controls.Add(this.btnCopyResult);
            this.pnlScanRight.Controls.Add(this.btnOpenUrl);
            this.pnlScanRight.Controls.Add(this.lblScanStatus);
            this.pnlScanRight.Controls.Add(this.lblDragTip);
            this.pnlScanRight.Location = new System.Drawing.Point(372, 8);
            this.pnlScanRight.Name = "pnlScanRight";
            this.pnlScanRight.Size = new System.Drawing.Size(316, 490);
            this.pnlScanRight.TabIndex = 1;
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnLoadImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadImage.FlatAppearance.BorderSize = 0;
            this.btnLoadImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadImage.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnLoadImage.ForeColor = System.Drawing.Color.White;
            this.btnLoadImage.Location = new System.Drawing.Point(3, 0);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(310, 38);
            this.btnLoadImage.TabIndex = 0;
            this.btnLoadImage.Text = "Open Image File";
            this.btnLoadImage.UseVisualStyleBackColor = false;
            // 
            // lblResultHeader
            // 
            this.lblResultHeader.AutoSize = true;
            this.lblResultHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblResultHeader.ForeColor = System.Drawing.Color.Silver;
            this.lblResultHeader.Location = new System.Drawing.Point(3, 52);
            this.lblResultHeader.Name = "lblResultHeader";
            this.lblResultHeader.Size = new System.Drawing.Size(108, 15);
            this.lblResultHeader.TabIndex = 1;
            this.lblResultHeader.Text = "Decoded Content:";
            // 
            // txtResult
            // 
            this.txtResult.BackColor = System.Drawing.Color.White;
            this.txtResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResult.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtResult.Location = new System.Drawing.Point(3, 72);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResult.Size = new System.Drawing.Size(310, 200);
            this.txtResult.TabIndex = 2;
            // 
            // btnCopyResult
            // 
            this.btnCopyResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(230)))));
            this.btnCopyResult.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCopyResult.Enabled = false;
            this.btnCopyResult.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopyResult.Location = new System.Drawing.Point(3, 282);
            this.btnCopyResult.Name = "btnCopyResult";
            this.btnCopyResult.Size = new System.Drawing.Size(150, 32);
            this.btnCopyResult.TabIndex = 3;
            this.btnCopyResult.Text = "Copy to Clipboard";
            this.btnCopyResult.UseVisualStyleBackColor = false;
            // 
            // btnOpenUrl
            // 
            this.btnOpenUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(230)))));
            this.btnOpenUrl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenUrl.Enabled = false;
            this.btnOpenUrl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenUrl.Location = new System.Drawing.Point(163, 282);
            this.btnOpenUrl.Name = "btnOpenUrl";
            this.btnOpenUrl.Size = new System.Drawing.Size(150, 32);
            this.btnOpenUrl.TabIndex = 4;
            this.btnOpenUrl.Text = "Open URL";
            this.btnOpenUrl.UseVisualStyleBackColor = false;
            // 
            // lblScanStatus
            // 
            this.lblScanStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblScanStatus.Location = new System.Drawing.Point(3, 326);
            this.lblScanStatus.Name = "lblScanStatus";
            this.lblScanStatus.Size = new System.Drawing.Size(310, 40);
            this.lblScanStatus.TabIndex = 5;
            this.lblScanStatus.Text = "Open or drag & drop an image containing a QR code.";
            // 
            // lblDragTip
            // 
            this.lblDragTip.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            this.lblDragTip.ForeColor = System.Drawing.Color.Gray;
            this.lblDragTip.Location = new System.Drawing.Point(3, 374);
            this.lblDragTip.Name = "lblDragTip";
            this.lblDragTip.Size = new System.Drawing.Size(310, 34);
            this.lblDragTip.TabIndex = 6;
            this.lblDragTip.Text = "Tip: PNG, JPG, BMP, GIF and TIFF files are supported.";
            // 
            // TTS_Tts
            // 
            this.TTS_Tts.AutoSize = false;
            this.TTS_Tts.BackColor = System.Drawing.Color.Transparent;
            this.TTS_Tts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.TTS_Tts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Quit_Btn});
            this.TTS_Tts.Location = new System.Drawing.Point(0, 0);
            this.TTS_Tts.Name = "TTS_Tts";
            this.TTS_Tts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.TTS_Tts.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.TTS_Tts.Size = new System.Drawing.Size(703, 18);
            this.TTS_Tts.TabIndex = 1;
            this.TTS_Tts.Text = "toolStrip1";
            // 
            // Quit_Btn
            // 
            this.Quit_Btn.AutoSize = false;
            this.Quit_Btn.BackColor = System.Drawing.Color.Red;
            this.Quit_Btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Quit_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Quit_Btn.Image = ((System.Drawing.Image)(resources.GetObject("Quit_Btn.Image")));
            this.Quit_Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Quit_Btn.Name = "Quit_Btn";
            this.Quit_Btn.Size = new System.Drawing.Size(15, 15);
            this.Quit_Btn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.Quit_Btn.ToolTipText = "Quit";
            this.Quit_Btn.Click += new System.EventHandler(this.Quit_Btn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(703, 555);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.TTS_Tts);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(703, 555);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(703, 555);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QR Code Tool";
            this.tabMain.ResumeLayout(false);
            this.tabPageGenerate.ResumeLayout(false);
            this.pnlGenLeft.ResumeLayout(false);
            this.pnlGenLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSize)).EndInit();
            this.pnlGenRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picGenerated)).EndInit();
            this.tabPageScan.ResumeLayout(false);
            this.pnlScanLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picScanned)).EndInit();
            this.pnlScanRight.ResumeLayout(false);
            this.pnlScanRight.PerformLayout();
            this.TTS_Tts.ResumeLayout(false);
            this.TTS_Tts.PerformLayout();
            this.ResumeLayout(false);

        }
        private ToolStrip TTS_Tts;
        private ToolStripButton Quit_Btn;
    }
}
