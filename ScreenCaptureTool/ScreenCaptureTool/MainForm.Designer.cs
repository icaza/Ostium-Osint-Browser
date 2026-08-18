using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenCaptureTool
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnFullScreen;
        private Button btnActiveWindow;
        private Button btnRegion;
        private Button btnSave;
        private PictureBox pictureBoxPreview;
        private Label StatusText;
        private TableLayoutPanel tlpMain;
        private FlowLayoutPanel flpButtons;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnFullScreen = new System.Windows.Forms.Button();
            this.btnActiveWindow = new System.Windows.Forms.Button();
            this.btnRegion = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.StatusText = new System.Windows.Forms.Label();
            this.tlpMain.SuspendLayout();
            this.flpButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Controls.Add(this.flpButtons, 0, 0);
            this.tlpMain.Controls.Add(this.pictureBoxPreview, 0, 1);
            this.tlpMain.Controls.Add(this.StatusText, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tlpMain.Size = new System.Drawing.Size(771, 563);
            this.tlpMain.TabIndex = 0;
            // 
            // flpButtons
            // 
            this.flpButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.flpButtons.Controls.Add(this.btnFullScreen);
            this.flpButtons.Controls.Add(this.btnActiveWindow);
            this.flpButtons.Controls.Add(this.btnRegion);
            this.flpButtons.Controls.Add(this.btnSave);
            this.flpButtons.Controls.Add(this.label1);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.Location = new System.Drawing.Point(3, 3);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Padding = new System.Windows.Forms.Padding(3);
            this.flpButtons.Size = new System.Drawing.Size(765, 46);
            this.flpButtons.TabIndex = 0;
            this.flpButtons.WrapContents = false;
            // 
            // btnFullScreen
            // 
            this.btnFullScreen.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnFullScreen.FlatAppearance.BorderSize = 0;
            this.btnFullScreen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            this.btnFullScreen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Maroon;
            this.btnFullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFullScreen.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFullScreen.Location = new System.Drawing.Point(6, 6);
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.Size = new System.Drawing.Size(100, 34);
            this.btnFullScreen.TabIndex = 0;
            this.btnFullScreen.Text = "Full Screen";
            this.btnFullScreen.Click += new System.EventHandler(this.BtnFullScreen_Click);
            // 
            // btnActiveWindow
            // 
            this.btnActiveWindow.FlatAppearance.BorderSize = 0;
            this.btnActiveWindow.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            this.btnActiveWindow.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Maroon;
            this.btnActiveWindow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActiveWindow.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActiveWindow.Location = new System.Drawing.Point(112, 6);
            this.btnActiveWindow.Name = "btnActiveWindow";
            this.btnActiveWindow.Size = new System.Drawing.Size(147, 34);
            this.btnActiveWindow.TabIndex = 1;
            this.btnActiveWindow.Text = "Active Window";
            this.btnActiveWindow.Click += new System.EventHandler(this.BtnActiveWindow_Click);
            // 
            // btnRegion
            // 
            this.btnRegion.FlatAppearance.BorderSize = 0;
            this.btnRegion.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            this.btnRegion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Maroon;
            this.btnRegion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegion.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegion.Location = new System.Drawing.Point(265, 6);
            this.btnRegion.Name = "btnRegion";
            this.btnRegion.Size = new System.Drawing.Size(134, 34);
            this.btnRegion.TabIndex = 2;
            this.btnRegion.Text = "Select Region";
            this.btnRegion.Click += new System.EventHandler(this.BtnRegion_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Maroon;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(405, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(64, 34);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(475, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(290, 40);
            this.label1.TabIndex = 4;
            this.label1.Text = "Full Screen - CTRL+1 || Select Region - CTRL+3\r\nActive Window - CTRL+2 || Save - " +
    "CTRL+S";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.pictureBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPreview.Location = new System.Drawing.Point(3, 55);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(765, 479);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 1;
            this.pictureBoxPreview.TabStop = false;
            // 
            // StatusText
            // 
            this.StatusText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.StatusText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatusText.ForeColor = System.Drawing.Color.White;
            this.StatusText.Location = new System.Drawing.Point(3, 537);
            this.StatusText.Name = "StatusText";
            this.StatusText.Padding = new System.Windows.Forms.Padding(9, 0, 0, 0);
            this.StatusText.Size = new System.Drawing.Size(765, 26);
            this.StatusText.TabIndex = 2;
            this.StatusText.Text = "Ready";
            this.StatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(771, 563);
            this.Controls.Add(this.tlpMain);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(602, 439);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Screen Capture Tool";
            this.TopMost = true;
            this.tlpMain.ResumeLayout(false);
            this.flpButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);

        }

        private Label label1;
    }
}