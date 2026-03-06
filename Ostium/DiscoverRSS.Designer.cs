namespace Ostium
{
    partial class DiscoverRSS
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.DiscoverRSS_Btn = new System.Windows.Forms.Button();
            this.lblUrl = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.CopyRSSdiscover_Btn = new System.Windows.Forms.Button();
            this.BatchDiscover_Btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // urlTextBox
            // 
            this.urlTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.urlTextBox.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.urlTextBox.Location = new System.Drawing.Point(12, 27);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(551, 22);
            this.urlTextBox.TabIndex = 1;
            // 
            // titleTextBox
            // 
            this.titleTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.titleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.titleTextBox.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleTextBox.Location = new System.Drawing.Point(12, 73);
            this.titleTextBox.Multiline = true;
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(551, 51);
            this.titleTextBox.TabIndex = 4;
            // 
            // DiscoverRSS_Btn
            // 
            this.DiscoverRSS_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DiscoverRSS_Btn.FlatAppearance.BorderSize = 0;
            this.DiscoverRSS_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DiscoverRSS_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DiscoverRSS_Btn.ForeColor = System.Drawing.Color.DodgerBlue;
            this.DiscoverRSS_Btn.Location = new System.Drawing.Point(453, 131);
            this.DiscoverRSS_Btn.Name = "DiscoverRSS_Btn";
            this.DiscoverRSS_Btn.Size = new System.Drawing.Size(110, 24);
            this.DiscoverRSS_Btn.TabIndex = 2;
            this.DiscoverRSS_Btn.Text = "Discover RSS";
            this.DiscoverRSS_Btn.UseVisualStyleBackColor = true;
            this.DiscoverRSS_Btn.Click += new System.EventHandler(this.DiscoverRSS_Btn_Click);
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUrl.ForeColor = System.Drawing.Color.White;
            this.lblUrl.Location = new System.Drawing.Point(9, 10);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(91, 14);
            this.lblUrl.TabIndex = 0;
            this.lblUrl.Text = "Website URL:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(9, 56);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(73, 14);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Feed Title:";
            // 
            // CopyRSSdiscover_Btn
            // 
            this.CopyRSSdiscover_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CopyRSSdiscover_Btn.Enabled = false;
            this.CopyRSSdiscover_Btn.FlatAppearance.BorderSize = 0;
            this.CopyRSSdiscover_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CopyRSSdiscover_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CopyRSSdiscover_Btn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.CopyRSSdiscover_Btn.Location = new System.Drawing.Point(179, 131);
            this.CopyRSSdiscover_Btn.Name = "CopyRSSdiscover_Btn";
            this.CopyRSSdiscover_Btn.Size = new System.Drawing.Size(110, 24);
            this.CopyRSSdiscover_Btn.TabIndex = 5;
            this.CopyRSSdiscover_Btn.Text = "Copy RSS url";
            this.CopyRSSdiscover_Btn.UseVisualStyleBackColor = true;
            this.CopyRSSdiscover_Btn.Click += new System.EventHandler(this.CopyRSSdiscover_Btn_Click);
            // 
            // BatchDiscover_Btn
            // 
            this.BatchDiscover_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BatchDiscover_Btn.FlatAppearance.BorderSize = 0;
            this.BatchDiscover_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BatchDiscover_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BatchDiscover_Btn.ForeColor = System.Drawing.Color.GreenYellow;
            this.BatchDiscover_Btn.Location = new System.Drawing.Point(295, 131);
            this.BatchDiscover_Btn.Name = "BatchDiscover_Btn";
            this.BatchDiscover_Btn.Size = new System.Drawing.Size(152, 24);
            this.BatchDiscover_Btn.TabIndex = 6;
            this.BatchDiscover_Btn.Text = "Discover RSS batch";
            this.BatchDiscover_Btn.UseVisualStyleBackColor = true;
            this.BatchDiscover_Btn.Click += new System.EventHandler(this.BatchDiscover_Btn_Click);
            // 
            // DiscoverRSS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(573, 163);
            this.Controls.Add(this.BatchDiscover_Btn);
            this.Controls.Add(this.CopyRSSdiscover_Btn);
            this.Controls.Add(this.lblUrl);
            this.Controls.Add(this.urlTextBox);
            this.Controls.Add(this.DiscoverRSS_Btn);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.titleTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "DiscoverRSS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.Button DiscoverRSS_Btn;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button CopyRSSdiscover_Btn;
        private System.Windows.Forms.Button BatchDiscover_Btn;
    }
}