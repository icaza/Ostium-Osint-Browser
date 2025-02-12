namespace Ostium
{
    partial class Keeptrack_Frm
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
            this.label1 = new System.Windows.Forms.Label();
            this.Tags_Txt = new System.Windows.Forms.TextBox();
            this.AddTrack_Btn = new System.Windows.Forms.Button();
            this.TrackRecord_Cbx = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Close_Btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(11, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tag";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Tags_Txt
            // 
            this.Tags_Txt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tags_Txt.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tags_Txt.Location = new System.Drawing.Point(61, 48);
            this.Tags_Txt.Name = "Tags_Txt";
            this.Tags_Txt.Size = new System.Drawing.Size(206, 22);
            this.Tags_Txt.TabIndex = 1;
            // 
            // AddTrack_Btn
            // 
            this.AddTrack_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AddTrack_Btn.FlatAppearance.BorderSize = 0;
            this.AddTrack_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.AddTrack_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddTrack_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddTrack_Btn.ForeColor = System.Drawing.Color.White;
            this.AddTrack_Btn.Location = new System.Drawing.Point(272, 46);
            this.AddTrack_Btn.Name = "AddTrack_Btn";
            this.AddTrack_Btn.Size = new System.Drawing.Size(83, 23);
            this.AddTrack_Btn.TabIndex = 2;
            this.AddTrack_Btn.Text = "Add Track";
            this.AddTrack_Btn.UseVisualStyleBackColor = true;
            this.AddTrack_Btn.Click += new System.EventHandler(this.AddTrack_Btn_Click);
            // 
            // TrackRecord_Cbx
            // 
            this.TrackRecord_Cbx.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TrackRecord_Cbx.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackRecord_Cbx.FormattingEnabled = true;
            this.TrackRecord_Cbx.Location = new System.Drawing.Point(61, 16);
            this.TrackRecord_Cbx.Name = "TrackRecord_Cbx";
            this.TrackRecord_Cbx.Size = new System.Drawing.Size(294, 22);
            this.TrackRecord_Cbx.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label2.Location = new System.Drawing.Point(11, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 22);
            this.label2.TabIndex = 4;
            this.label2.Text = "Track";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Close_Btn
            // 
            this.Close_Btn.BackColor = System.Drawing.Color.Red;
            this.Close_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Close_Btn.FlatAppearance.BorderSize = 0;
            this.Close_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Close_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Close_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Close_Btn.ForeColor = System.Drawing.Color.White;
            this.Close_Btn.Location = new System.Drawing.Point(355, 1);
            this.Close_Btn.Name = "Close_Btn";
            this.Close_Btn.Size = new System.Drawing.Size(10, 10);
            this.Close_Btn.TabIndex = 5;
            this.Close_Btn.UseVisualStyleBackColor = false;
            this.Close_Btn.Click += new System.EventHandler(this.Close_Btn_Click);
            // 
            // Keeptrack_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(366, 84);
            this.Controls.Add(this.Close_Btn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TrackRecord_Cbx);
            this.Controls.Add(this.AddTrack_Btn);
            this.Controls.Add(this.Tags_Txt);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Keeptrack_Frm";
            this.Opacity = 0.1D;
            this.ShowInTaskbar = false;
            this.Text = "Keep";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Keeptrack_Frm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Tags_Txt;
        private System.Windows.Forms.Button AddTrack_Btn;
        private System.Windows.Forms.ComboBox TrackRecord_Cbx;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Close_Btn;
    }
}