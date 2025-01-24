namespace Ostium
{
    partial class Aes_Frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Aes_Frm));
            this.label1 = new System.Windows.Forms.Label();
            this.Pwd_Txt = new System.Windows.Forms.TextBox();
            this.Encrypt_Pnl = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Decrypt_Pnl = new System.Windows.Forms.Panel();
            this.DeleteFile_Chk = new System.Windows.Forms.CheckBox();
            this.Close_Btn = new System.Windows.Forms.Button();
            this.Detail_Lbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pwd";
            // 
            // Pwd_Txt
            // 
            this.Pwd_Txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Pwd_Txt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Pwd_Txt.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Pwd_Txt.ForeColor = System.Drawing.Color.White;
            this.Pwd_Txt.Location = new System.Drawing.Point(14, 38);
            this.Pwd_Txt.Name = "Pwd_Txt";
            this.Pwd_Txt.Size = new System.Drawing.Size(448, 22);
            this.Pwd_Txt.TabIndex = 1;
            this.Pwd_Txt.UseSystemPasswordChar = true;
            // 
            // Encrypt_Pnl
            // 
            this.Encrypt_Pnl.AllowDrop = true;
            this.Encrypt_Pnl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Encrypt_Pnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Encrypt_Pnl.Location = new System.Drawing.Point(14, 93);
            this.Encrypt_Pnl.Name = "Encrypt_Pnl";
            this.Encrypt_Pnl.Size = new System.Drawing.Size(218, 162);
            this.Encrypt_Pnl.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(14, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(218, 14);
            this.label2.TabIndex = 3;
            this.label2.Text = "Drag the file to be Encrypted here";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(244, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(218, 14);
            this.label3.TabIndex = 4;
            this.label3.Text = "Drag the file to be Decrypted here";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Decrypt_Pnl
            // 
            this.Decrypt_Pnl.AllowDrop = true;
            this.Decrypt_Pnl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Decrypt_Pnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Decrypt_Pnl.Location = new System.Drawing.Point(244, 93);
            this.Decrypt_Pnl.Name = "Decrypt_Pnl";
            this.Decrypt_Pnl.Size = new System.Drawing.Size(218, 162);
            this.Decrypt_Pnl.TabIndex = 3;
            // 
            // DeleteFile_Chk
            // 
            this.DeleteFile_Chk.AutoSize = true;
            this.DeleteFile_Chk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteFile_Chk.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteFile_Chk.ForeColor = System.Drawing.Color.Red;
            this.DeleteFile_Chk.Location = new System.Drawing.Point(80, 14);
            this.DeleteFile_Chk.Name = "DeleteFile_Chk";
            this.DeleteFile_Chk.Size = new System.Drawing.Size(181, 18);
            this.DeleteFile_Chk.TabIndex = 5;
            this.DeleteFile_Chk.Text = "Delete original file at end";
            this.DeleteFile_Chk.UseVisualStyleBackColor = true;
            // 
            // Close_Btn
            // 
            this.Close_Btn.BackColor = System.Drawing.Color.Red;
            this.Close_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Close_Btn.FlatAppearance.BorderSize = 0;
            this.Close_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.Close_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Close_Btn.Location = new System.Drawing.Point(462, 5);
            this.Close_Btn.Name = "Close_Btn";
            this.Close_Btn.Size = new System.Drawing.Size(10, 10);
            this.Close_Btn.TabIndex = 6;
            this.Close_Btn.UseVisualStyleBackColor = false;
            this.Close_Btn.Click += new System.EventHandler(this.Close_Btn_Click);
            // 
            // Detail_Lbl
            // 
            this.Detail_Lbl.AutoSize = true;
            this.Detail_Lbl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Detail_Lbl.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Detail_Lbl.ForeColor = System.Drawing.Color.DodgerBlue;
            this.Detail_Lbl.Location = new System.Drawing.Point(267, 16);
            this.Detail_Lbl.Name = "Detail_Lbl";
            this.Detail_Lbl.Size = new System.Drawing.Size(50, 14);
            this.Detail_Lbl.TabIndex = 7;
            this.Detail_Lbl.Text = "Details";
            this.Detail_Lbl.Click += new System.EventHandler(this.Detail_Lbl_Click);
            // 
            // Aes_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(476, 271);
            this.Controls.Add(this.Detail_Lbl);
            this.Controls.Add(this.Close_Btn);
            this.Controls.Add(this.DeleteFile_Chk);
            this.Controls.Add(this.Decrypt_Pnl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Encrypt_Pnl);
            this.Controls.Add(this.Pwd_Txt);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Aes_Frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Encryption ";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Pwd_Txt;
        private System.Windows.Forms.Panel Encrypt_Pnl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel Decrypt_Pnl;
        private System.Windows.Forms.CheckBox DeleteFile_Chk;
        private System.Windows.Forms.Button Close_Btn;
        private System.Windows.Forms.Label Detail_Lbl;
    }
}