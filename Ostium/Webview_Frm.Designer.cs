namespace Ostium
{
    partial class Webview_Frm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Webview_Frm));
            this.WBrowse = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panel3 = new System.Windows.Forms.Panel();
            this.URLbrowse_Cbx = new System.Windows.Forms.ComboBox();
            this.CopyURL_Btn = new System.Windows.Forms.Button();
            this.Trad_Btn = new System.Windows.Forms.Button();
            this.Home_Btn = new System.Windows.Forms.Button();
            this.Refresh_Btn = new System.Windows.Forms.Button();
            this.Forward_Btn = new System.Windows.Forms.Button();
            this.Back_Btn = new System.Windows.Forms.Button();
            this.Go_Btn = new System.Windows.Forms.Button();
            this.Status_Strip = new System.Windows.Forms.StatusStrip();
            this.JavaScriptToggle_Btn = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.URLtxt_txt = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.WBrowse)).BeginInit();
            this.panel3.SuspendLayout();
            this.Status_Strip.SuspendLayout();
            this.SuspendLayout();
            // 
            // WBrowse
            // 
            this.WBrowse.AllowExternalDrop = true;
            this.WBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.WBrowse.CreationProperties = null;
            this.WBrowse.DefaultBackgroundColor = System.Drawing.Color.White;
            this.WBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WBrowse.Location = new System.Drawing.Point(0, 24);
            this.WBrowse.Name = "WBrowse";
            this.WBrowse.Size = new System.Drawing.Size(800, 404);
            this.WBrowse.Source = new System.Uri("http://192.168.1.36/", System.UriKind.Absolute);
            this.WBrowse.TabIndex = 16;
            this.WBrowse.ZoomFactor = 1D;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.panel3.Controls.Add(this.URLbrowse_Cbx);
            this.panel3.Controls.Add(this.CopyURL_Btn);
            this.panel3.Controls.Add(this.Trad_Btn);
            this.panel3.Controls.Add(this.Home_Btn);
            this.panel3.Controls.Add(this.Refresh_Btn);
            this.panel3.Controls.Add(this.Forward_Btn);
            this.panel3.Controls.Add(this.Back_Btn);
            this.panel3.Controls.Add(this.Go_Btn);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(800, 24);
            this.panel3.TabIndex = 15;
            // 
            // URLbrowse_Cbx
            // 
            this.URLbrowse_Cbx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.URLbrowse_Cbx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.URLbrowse_Cbx.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.URLbrowse_Cbx.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.URLbrowse_Cbx.ForeColor = System.Drawing.Color.Gold;
            this.URLbrowse_Cbx.FormattingEnabled = true;
            this.URLbrowse_Cbx.Location = new System.Drawing.Point(297, 0);
            this.URLbrowse_Cbx.Name = "URLbrowse_Cbx";
            this.URLbrowse_Cbx.Size = new System.Drawing.Size(445, 24);
            this.URLbrowse_Cbx.TabIndex = 6;
            // 
            // CopyURL_Btn
            // 
            this.CopyURL_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.CopyURL_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CopyURL_Btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.CopyURL_Btn.FlatAppearance.BorderSize = 0;
            this.CopyURL_Btn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.CopyURL_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.CopyURL_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.CopyURL_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CopyURL_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CopyURL_Btn.ForeColor = System.Drawing.Color.White;
            this.CopyURL_Btn.Location = new System.Drawing.Point(259, 0);
            this.CopyURL_Btn.Name = "CopyURL_Btn";
            this.CopyURL_Btn.Size = new System.Drawing.Size(38, 24);
            this.CopyURL_Btn.TabIndex = 8;
            this.CopyURL_Btn.Text = "Cp";
            this.CopyURL_Btn.UseVisualStyleBackColor = false;
            this.CopyURL_Btn.Click += new System.EventHandler(this.CopyURL_Btn_Click);
            // 
            // Trad_Btn
            // 
            this.Trad_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.Trad_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Trad_Btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.Trad_Btn.FlatAppearance.BorderSize = 0;
            this.Trad_Btn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.Trad_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.Trad_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.Trad_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Trad_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Trad_Btn.ForeColor = System.Drawing.Color.White;
            this.Trad_Btn.Location = new System.Drawing.Point(212, 0);
            this.Trad_Btn.Name = "Trad_Btn";
            this.Trad_Btn.Size = new System.Drawing.Size(47, 24);
            this.Trad_Btn.TabIndex = 7;
            this.Trad_Btn.Text = "Trad";
            this.Trad_Btn.UseVisualStyleBackColor = false;
            this.Trad_Btn.Click += new System.EventHandler(this.Trad_Btn_Click);
            // 
            // Home_Btn
            // 
            this.Home_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.Home_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Home_Btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.Home_Btn.FlatAppearance.BorderSize = 0;
            this.Home_Btn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.Home_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.Home_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.Home_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Home_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Home_Btn.ForeColor = System.Drawing.Color.White;
            this.Home_Btn.Location = new System.Drawing.Point(159, 0);
            this.Home_Btn.Name = "Home_Btn";
            this.Home_Btn.Size = new System.Drawing.Size(53, 24);
            this.Home_Btn.TabIndex = 5;
            this.Home_Btn.Text = "H";
            this.Home_Btn.UseVisualStyleBackColor = false;
            this.Home_Btn.Click += new System.EventHandler(this.Home_Btn_Click);
            // 
            // Refresh_Btn
            // 
            this.Refresh_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.Refresh_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Refresh_Btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.Refresh_Btn.FlatAppearance.BorderSize = 0;
            this.Refresh_Btn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.Refresh_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.Refresh_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.Refresh_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Refresh_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Refresh_Btn.ForeColor = System.Drawing.Color.White;
            this.Refresh_Btn.Location = new System.Drawing.Point(106, 0);
            this.Refresh_Btn.Name = "Refresh_Btn";
            this.Refresh_Btn.Size = new System.Drawing.Size(53, 24);
            this.Refresh_Btn.TabIndex = 4;
            this.Refresh_Btn.Text = "R";
            this.Refresh_Btn.UseVisualStyleBackColor = false;
            this.Refresh_Btn.Click += new System.EventHandler(this.Refresh_Btn_Click);
            // 
            // Forward_Btn
            // 
            this.Forward_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.Forward_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Forward_Btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.Forward_Btn.FlatAppearance.BorderSize = 0;
            this.Forward_Btn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.Forward_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.Forward_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.Forward_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Forward_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Forward_Btn.ForeColor = System.Drawing.Color.White;
            this.Forward_Btn.Location = new System.Drawing.Point(53, 0);
            this.Forward_Btn.Name = "Forward_Btn";
            this.Forward_Btn.Size = new System.Drawing.Size(53, 24);
            this.Forward_Btn.TabIndex = 3;
            this.Forward_Btn.Text = "F";
            this.Forward_Btn.UseVisualStyleBackColor = false;
            this.Forward_Btn.Click += new System.EventHandler(this.Forward_Btn_Click);
            // 
            // Back_Btn
            // 
            this.Back_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.Back_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Back_Btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.Back_Btn.FlatAppearance.BorderSize = 0;
            this.Back_Btn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.Back_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.Back_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.Back_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Back_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Back_Btn.ForeColor = System.Drawing.Color.White;
            this.Back_Btn.Location = new System.Drawing.Point(0, 0);
            this.Back_Btn.Name = "Back_Btn";
            this.Back_Btn.Size = new System.Drawing.Size(53, 24);
            this.Back_Btn.TabIndex = 2;
            this.Back_Btn.Text = "B";
            this.Back_Btn.UseVisualStyleBackColor = false;
            this.Back_Btn.Click += new System.EventHandler(this.Back_Btn_Click);
            // 
            // Go_Btn
            // 
            this.Go_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.Go_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Go_Btn.Dock = System.Windows.Forms.DockStyle.Right;
            this.Go_Btn.FlatAppearance.BorderSize = 0;
            this.Go_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.Go_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.Go_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Go_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Go_Btn.ForeColor = System.Drawing.Color.White;
            this.Go_Btn.Location = new System.Drawing.Point(742, 0);
            this.Go_Btn.Name = "Go_Btn";
            this.Go_Btn.Size = new System.Drawing.Size(58, 24);
            this.Go_Btn.TabIndex = 1;
            this.Go_Btn.Text = "Go";
            this.Go_Btn.UseVisualStyleBackColor = false;
            this.Go_Btn.Click += new System.EventHandler(this.Go_Btn_Click);
            // 
            // Status_Strip
            // 
            this.Status_Strip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.Status_Strip.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Status_Strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.JavaScriptToggle_Btn,
            this.toolStripStatusLabel2,
            this.URLtxt_txt});
            this.Status_Strip.Location = new System.Drawing.Point(0, 428);
            this.Status_Strip.Name = "Status_Strip";
            this.Status_Strip.Size = new System.Drawing.Size(800, 22);
            this.Status_Strip.SizingGrip = false;
            this.Status_Strip.TabIndex = 17;
            this.Status_Strip.Text = "statusStrip1";
            // 
            // JavaScriptToggle_Btn
            // 
            this.JavaScriptToggle_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JavaScriptToggle_Btn.ForeColor = System.Drawing.Color.Lime;
            this.JavaScriptToggle_Btn.Name = "JavaScriptToggle_Btn";
            this.JavaScriptToggle_Btn.Size = new System.Drawing.Size(123, 17);
            this.JavaScriptToggle_Btn.Text = "JavaScript Enable";
            this.JavaScriptToggle_Btn.Click += new System.EventHandler(this.JavaScriptToggle_Btn_Click);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoToolTip = true;
            this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.DimGray;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(14, 17);
            this.toolStripStatusLabel2.Text = "|";
            // 
            // URLtxt_txt
            // 
            this.URLtxt_txt.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.URLtxt_txt.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.URLtxt_txt.Name = "URLtxt_txt";
            this.URLtxt_txt.Size = new System.Drawing.Size(22, 17);
            this.URLtxt_txt.Text = "url";
            // 
            // Webview_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.WBrowse);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.Status_Strip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Webview_Frm";
            this.Text = "Webview_Frm";
            this.Load += new System.EventHandler(this.Webview_Frm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.WBrowse)).EndInit();
            this.panel3.ResumeLayout(false);
            this.Status_Strip.ResumeLayout(false);
            this.Status_Strip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 WBrowse;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox URLbrowse_Cbx;
        private System.Windows.Forms.Button CopyURL_Btn;
        private System.Windows.Forms.Button Trad_Btn;
        private System.Windows.Forms.Button Home_Btn;
        private System.Windows.Forms.Button Refresh_Btn;
        private System.Windows.Forms.Button Forward_Btn;
        private System.Windows.Forms.Button Back_Btn;
        private System.Windows.Forms.Button Go_Btn;
        private System.Windows.Forms.StatusStrip Status_Strip;
        private System.Windows.Forms.ToolStripStatusLabel URLtxt_txt;
        private System.Windows.Forms.ToolStripStatusLabel JavaScriptToggle_Btn;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}