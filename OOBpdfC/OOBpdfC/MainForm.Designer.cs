namespace OOBpdfC
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnAddFiles;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.RadioButton rbTxt;
        private System.Windows.Forms.RadioButton rbMd;
        private System.Windows.Forms.RadioButton rbBoth;
        private System.Windows.Forms.CheckBox chkMetadata;
        private System.Windows.Forms.CheckBox chkImages;
        private System.Windows.Forms.CheckBox chkTables;
        private System.Windows.Forms.CheckBox chkFormatting;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Panel pnlBottom;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            pnlTop = new Panel();
            lblTitle = new Label();
            pnlButtons = new Panel();
            btnAddFiles = new Button();
            btnAddFolder = new Button();
            btnRemoveSelected = new Button();
            btnClearAll = new Button();
            lvFiles = new ListView();
            colFileName = new ColumnHeader();
            colSize = new ColumnHeader();
            colStatus = new ColumnHeader();
            imageList = new ImageList(components);
            chkFormatting = new CheckBox();
            chkTables = new CheckBox();
            btnConvert = new Button();
            chkImages = new CheckBox();
            chkMetadata = new CheckBox();
            rbBoth = new RadioButton();
            rbMd = new RadioButton();
            rbTxt = new RadioButton();
            pnlBottom = new Panel();
            lblFileCount = new Label();
            pnlRight = new Panel();
            pnlTop.SuspendLayout();
            pnlButtons.SuspendLayout();
            pnlBottom.SuspendLayout();
            pnlRight.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(41, 128, 185);
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(926, 43);
            pnlTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Verdana", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 13);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(203, 18);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "📄 OOB PDF Converter";
            // 
            // pnlButtons
            // 
            pnlButtons.BackColor = Color.FromArgb(41, 44, 45);
            pnlButtons.Controls.Add(btnAddFiles);
            pnlButtons.Controls.Add(btnAddFolder);
            pnlButtons.Controls.Add(btnRemoveSelected);
            pnlButtons.Controls.Add(btnClearAll);
            pnlButtons.Dock = DockStyle.Top;
            pnlButtons.Location = new Point(0, 43);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Padding = new Padding(10);
            pnlButtons.Size = new Size(926, 60);
            pnlButtons.TabIndex = 1;
            // 
            // btnAddFiles
            // 
            btnAddFiles.BackColor = Color.FromArgb(46, 204, 113);
            btnAddFiles.Cursor = Cursors.Hand;
            btnAddFiles.FlatAppearance.BorderSize = 0;
            btnAddFiles.FlatStyle = FlatStyle.Flat;
            btnAddFiles.Font = new Font("Verdana", 9.75F, FontStyle.Bold);
            btnAddFiles.ForeColor = Color.White;
            btnAddFiles.Location = new Point(15, 13);
            btnAddFiles.Name = "btnAddFiles";
            btnAddFiles.Size = new Size(150, 35);
            btnAddFiles.TabIndex = 0;
            btnAddFiles.Text = "➕ Add Files";
            btnAddFiles.UseVisualStyleBackColor = false;
            btnAddFiles.Click += BtnAddFiles_Click;
            // 
            // btnAddFolder
            // 
            btnAddFolder.BackColor = Color.FromArgb(52, 152, 219);
            btnAddFolder.Cursor = Cursors.Hand;
            btnAddFolder.FlatAppearance.BorderSize = 0;
            btnAddFolder.FlatStyle = FlatStyle.Flat;
            btnAddFolder.Font = new Font("Verdana", 9.75F, FontStyle.Bold);
            btnAddFolder.ForeColor = Color.White;
            btnAddFolder.Location = new Point(175, 13);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Size = new Size(150, 35);
            btnAddFolder.TabIndex = 1;
            btnAddFolder.Text = "📁 Add Folder";
            btnAddFolder.UseVisualStyleBackColor = false;
            btnAddFolder.Click += BtnAddFolder_Click;
            // 
            // btnRemoveSelected
            // 
            btnRemoveSelected.BackColor = Color.FromArgb(231, 76, 60);
            btnRemoveSelected.Cursor = Cursors.Hand;
            btnRemoveSelected.FlatAppearance.BorderSize = 0;
            btnRemoveSelected.FlatStyle = FlatStyle.Flat;
            btnRemoveSelected.Font = new Font("Verdana", 9.75F, FontStyle.Bold);
            btnRemoveSelected.ForeColor = Color.White;
            btnRemoveSelected.Location = new Point(335, 13);
            btnRemoveSelected.Name = "btnRemoveSelected";
            btnRemoveSelected.Size = new Size(150, 35);
            btnRemoveSelected.TabIndex = 2;
            btnRemoveSelected.Text = "❌ Remove";
            btnRemoveSelected.UseVisualStyleBackColor = false;
            btnRemoveSelected.Click += BtnRemoveSelected_Click;
            // 
            // btnClearAll
            // 
            btnClearAll.BackColor = Color.FromArgb(149, 165, 166);
            btnClearAll.Cursor = Cursors.Hand;
            btnClearAll.FlatAppearance.BorderSize = 0;
            btnClearAll.FlatStyle = FlatStyle.Flat;
            btnClearAll.Font = new Font("Verdana", 9.75F, FontStyle.Bold);
            btnClearAll.ForeColor = Color.White;
            btnClearAll.Location = new Point(495, 13);
            btnClearAll.Name = "btnClearAll";
            btnClearAll.Size = new Size(150, 35);
            btnClearAll.TabIndex = 3;
            btnClearAll.Text = "🗑️ Delete all";
            btnClearAll.UseVisualStyleBackColor = false;
            btnClearAll.Click += BtnClearAll_Click;
            // 
            // lvFiles
            // 
            lvFiles.BackColor = Color.White;
            lvFiles.BorderStyle = BorderStyle.FixedSingle;
            lvFiles.Columns.AddRange(new ColumnHeader[] { colFileName, colSize, colStatus });
            lvFiles.Dock = DockStyle.Fill;
            lvFiles.Font = new Font("Segoe UI", 9.75F);
            lvFiles.ForeColor = Color.Black;
            lvFiles.Location = new Point(0, 103);
            lvFiles.Name = "lvFiles";
            lvFiles.Size = new Size(678, 396);
            lvFiles.SmallImageList = imageList;
            lvFiles.TabIndex = 2;
            lvFiles.UseCompatibleStateImageBehavior = false;
            lvFiles.SelectedIndexChanged += LvFiles_SelectedIndexChanged;
            // 
            // colFileName
            // 
            colFileName.Text = "Nom du fichier";
            colFileName.Width = 350;
            // 
            // colSize
            // 
            colSize.Text = "Taille";
            colSize.Width = 100;
            // 
            // colStatus
            // 
            colStatus.Text = "Statut";
            colStatus.Width = 225;
            // 
            // imageList
            // 
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(16, 16);
            imageList.TransparentColor = Color.Transparent;
            // 
            // chkFormatting
            // 
            chkFormatting.AutoSize = true;
            chkFormatting.Checked = true;
            chkFormatting.CheckState = CheckState.Checked;
            chkFormatting.Font = new Font("Segoe UI", 9.75F);
            chkFormatting.Location = new Point(20, 253);
            chkFormatting.Name = "chkFormatting";
            chkFormatting.Size = new Size(186, 21);
            chkFormatting.TabIndex = 6;
            chkFormatting.Text = "📜 Preserve the formatting";
            chkFormatting.UseVisualStyleBackColor = true;
            // 
            // chkTables
            // 
            chkTables.AutoSize = true;
            chkTables.Checked = true;
            chkTables.CheckState = CheckState.Checked;
            chkTables.Font = new Font("Segoe UI", 9.75F);
            chkTables.Location = new Point(20, 218);
            chkTables.Name = "chkTables";
            chkTables.Size = new Size(148, 21);
            chkTables.TabIndex = 5;
            chkTables.Text = "📊 Extract the tables";
            chkTables.UseVisualStyleBackColor = true;
            // 
            // btnConvert
            // 
            btnConvert.BackColor = Color.FromArgb(155, 89, 182);
            btnConvert.Cursor = Cursors.Hand;
            btnConvert.Dock = DockStyle.Bottom;
            btnConvert.FlatStyle = FlatStyle.Flat;
            btnConvert.Font = new Font("Verdana", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConvert.ForeColor = Color.White;
            btnConvert.Location = new Point(0, 351);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(248, 45);
            btnConvert.TabIndex = 2;
            btnConvert.Text = "CONVERT";
            btnConvert.UseVisualStyleBackColor = false;
            btnConvert.Click += BtnConvert_Click;
            // 
            // chkImages
            // 
            chkImages.AutoSize = true;
            chkImages.Checked = true;
            chkImages.CheckState = CheckState.Checked;
            chkImages.Font = new Font("Segoe UI", 9.75F);
            chkImages.Location = new Point(20, 183);
            chkImages.Name = "chkImages";
            chkImages.Size = new Size(156, 21);
            chkImages.TabIndex = 4;
            chkImages.Text = "🖼️ Extract the images";
            chkImages.UseVisualStyleBackColor = true;
            // 
            // chkMetadata
            // 
            chkMetadata.AutoSize = true;
            chkMetadata.Checked = true;
            chkMetadata.CheckState = CheckState.Checked;
            chkMetadata.Font = new Font("Segoe UI", 9.75F);
            chkMetadata.Location = new Point(20, 148);
            chkMetadata.Name = "chkMetadata";
            chkMetadata.Size = new Size(147, 21);
            chkMetadata.TabIndex = 3;
            chkMetadata.Text = "ℹ️ Extract metadata";
            chkMetadata.UseVisualStyleBackColor = true;
            // 
            // rbBoth
            // 
            rbBoth.AutoSize = true;
            rbBoth.Checked = true;
            rbBoth.Font = new Font("Segoe UI", 9.75F);
            rbBoth.Location = new Point(20, 98);
            rbBoth.Name = "rbBoth";
            rbBoth.Size = new Size(73, 21);
            rbBoth.TabIndex = 2;
            rbBoth.TabStop = true;
            rbBoth.Text = "📑 Both";
            rbBoth.UseVisualStyleBackColor = true;
            // 
            // rbMd
            // 
            rbMd.AutoSize = true;
            rbMd.Font = new Font("Segoe UI", 9.75F);
            rbMd.Location = new Point(20, 68);
            rbMd.Name = "rbMd";
            rbMd.Size = new Size(169, 21);
            rbMd.TabIndex = 1;
            rbMd.Text = "📋 Markdown only (.md)";
            rbMd.UseVisualStyleBackColor = true;
            // 
            // rbTxt
            // 
            rbTxt.AutoSize = true;
            rbTxt.Font = new Font("Segoe UI", 9.75F);
            rbTxt.Location = new Point(20, 38);
            rbTxt.Name = "rbTxt";
            rbTxt.Size = new Size(128, 21);
            rbTxt.TabIndex = 0;
            rbTxt.Text = "📝 Text only (.txt)";
            rbTxt.UseVisualStyleBackColor = true;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.FromArgb(41, 44, 45);
            pnlBottom.BorderStyle = BorderStyle.FixedSingle;
            pnlBottom.Controls.Add(lblFileCount);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 499);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(926, 23);
            pnlBottom.TabIndex = 4;
            // 
            // lblFileCount
            // 
            lblFileCount.AutoSize = true;
            lblFileCount.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblFileCount.ForeColor = Color.Lime;
            lblFileCount.Location = new Point(2, 1);
            lblFileCount.Name = "lblFileCount";
            lblFileCount.Size = new Size(119, 16);
            lblFileCount.TabIndex = 0;
            lblFileCount.Text = "0 selected file(s)";
            // 
            // pnlRight
            // 
            pnlRight.BackColor = SystemColors.ControlLightLight;
            pnlRight.Controls.Add(rbTxt);
            pnlRight.Controls.Add(btnConvert);
            pnlRight.Controls.Add(rbMd);
            pnlRight.Controls.Add(chkFormatting);
            pnlRight.Controls.Add(rbBoth);
            pnlRight.Controls.Add(chkMetadata);
            pnlRight.Controls.Add(chkTables);
            pnlRight.Controls.Add(chkImages);
            pnlRight.Dock = DockStyle.Right;
            pnlRight.Location = new Point(678, 103);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(248, 396);
            pnlRight.TabIndex = 7;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(926, 522);
            Controls.Add(lvFiles);
            Controls.Add(pnlRight);
            Controls.Add(pnlBottom);
            Controls.Add(pnlButtons);
            Controls.Add(pnlTop);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PDF Converter";
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlButtons.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            pnlRight.ResumeLayout(false);
            pnlRight.PerformLayout();
            ResumeLayout(false);
        }

        private Panel pnlRight;
    }
}