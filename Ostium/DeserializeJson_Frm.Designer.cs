namespace Ostium
{
    partial class DeserializeJson_Frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeserializeJson_Frm));
            this.Output_Data = new System.Windows.Forms.RichTextBox();
            this.Fetch_Btn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DomainURL_Txt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ExportData_Btn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Output_Data
            // 
            this.Output_Data.BackColor = System.Drawing.Color.Black;
            this.Output_Data.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Output_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Output_Data.ForeColor = System.Drawing.Color.Lime;
            this.Output_Data.Location = new System.Drawing.Point(0, 40);
            this.Output_Data.Name = "Output_Data";
            this.Output_Data.Size = new System.Drawing.Size(800, 410);
            this.Output_Data.TabIndex = 0;
            this.Output_Data.Text = "";
            // 
            // Fetch_Btn
            // 
            this.Fetch_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Fetch_Btn.FlatAppearance.BorderSize = 0;
            this.Fetch_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.Fetch_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Fetch_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Fetch_Btn.ForeColor = System.Drawing.Color.White;
            this.Fetch_Btn.Location = new System.Drawing.Point(382, 8);
            this.Fetch_Btn.Name = "Fetch_Btn";
            this.Fetch_Btn.Size = new System.Drawing.Size(75, 23);
            this.Fetch_Btn.TabIndex = 1;
            this.Fetch_Btn.Text = "Fetch";
            this.Fetch_Btn.UseVisualStyleBackColor = true;
            this.Fetch_Btn.Click += new System.EventHandler(this.Fetch_Btn_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.panel1.Controls.Add(this.ExportData_Btn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.DomainURL_Txt);
            this.panel1.Controls.Add(this.Fetch_Btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 40);
            this.panel1.TabIndex = 2;
            // 
            // DomainURL_Txt
            // 
            this.DomainURL_Txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DomainURL_Txt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DomainURL_Txt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DomainURL_Txt.ForeColor = System.Drawing.Color.White;
            this.DomainURL_Txt.Location = new System.Drawing.Point(69, 9);
            this.DomainURL_Txt.Name = "DomainURL_Txt";
            this.DomainURL_Txt.Size = new System.Drawing.Size(307, 22);
            this.DomainURL_Txt.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Silver;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Domain";
            // 
            // ExportData_Btn
            // 
            this.ExportData_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExportData_Btn.FlatAppearance.BorderSize = 0;
            this.ExportData_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.ExportData_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportData_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExportData_Btn.ForeColor = System.Drawing.Color.White;
            this.ExportData_Btn.Location = new System.Drawing.Point(474, 8);
            this.ExportData_Btn.Name = "ExportData_Btn";
            this.ExportData_Btn.Size = new System.Drawing.Size(75, 23);
            this.ExportData_Btn.TabIndex = 3;
            this.ExportData_Btn.Text = "Export";
            this.ExportData_Btn.UseVisualStyleBackColor = true;
            this.ExportData_Btn.Click += new System.EventHandler(this.ExportData_Btn_Click);
            // 
            // DeserializeJson_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Output_Data);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DeserializeJson_Frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TXT NS MX A";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox Output_Data;
        private System.Windows.Forms.Button Fetch_Btn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox DomainURL_Txt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ExportData_Btn;
    }
}