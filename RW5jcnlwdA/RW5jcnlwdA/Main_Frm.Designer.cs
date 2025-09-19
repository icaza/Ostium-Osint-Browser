namespace RW5jcnlwdA
{
    partial class Main_Frm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Frm));
            label3 = new Label();
            label4 = new Label();
            Encrypt_Pnl = new Panel();
            Decrypt_Pnl = new Panel();
            Pwd_Txt = new TextBox();
            Close_Btn = new Button();
            DeleteFile_Chk = new CheckBox();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.White;
            label3.Location = new Point(17, 80);
            label3.Name = "label3";
            label3.Size = new Size(230, 14);
            label3.TabIndex = 2;
            label3.Text = "Drag the file to be Encrypted here";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.White;
            label4.Location = new Point(255, 80);
            label4.Name = "label4";
            label4.Size = new Size(232, 14);
            label4.TabIndex = 3;
            label4.Text = "Drag the file to be Decrypted here";
            // 
            // Encrypt_Pnl
            // 
            Encrypt_Pnl.AllowDrop = true;
            Encrypt_Pnl.BackColor = Color.FromArgb(41, 44, 51);
            Encrypt_Pnl.Location = new Point(15, 100);
            Encrypt_Pnl.Name = "Encrypt_Pnl";
            Encrypt_Pnl.Size = new Size(232, 176);
            Encrypt_Pnl.TabIndex = 4;
            // 
            // Decrypt_Pnl
            // 
            Decrypt_Pnl.AllowDrop = true;
            Decrypt_Pnl.BackColor = Color.FromArgb(41, 44, 51);
            Decrypt_Pnl.Location = new Point(255, 100);
            Decrypt_Pnl.Name = "Decrypt_Pnl";
            Decrypt_Pnl.Size = new Size(232, 176);
            Decrypt_Pnl.TabIndex = 5;
            // 
            // Pwd_Txt
            // 
            Pwd_Txt.BackColor = Color.FromArgb(41, 44, 51);
            Pwd_Txt.BorderStyle = BorderStyle.FixedSingle;
            Pwd_Txt.Font = new Font("Verdana", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Pwd_Txt.ForeColor = Color.White;
            Pwd_Txt.Location = new Point(7, 18);
            Pwd_Txt.Name = "Pwd_Txt";
            Pwd_Txt.Size = new Size(458, 22);
            Pwd_Txt.TabIndex = 6;
            Pwd_Txt.UseSystemPasswordChar = true;
            // 
            // Close_Btn
            // 
            Close_Btn.BackColor = Color.Red;
            Close_Btn.Cursor = Cursors.Hand;
            Close_Btn.FlatAppearance.BorderSize = 0;
            Close_Btn.FlatStyle = FlatStyle.Flat;
            Close_Btn.Location = new Point(489, 10);
            Close_Btn.Name = "Close_Btn";
            Close_Btn.Size = new Size(10, 10);
            Close_Btn.TabIndex = 7;
            Close_Btn.UseVisualStyleBackColor = false;
            Close_Btn.Click += Close_Btn_Click;
            // 
            // DeleteFile_Chk
            // 
            DeleteFile_Chk.AutoSize = true;
            DeleteFile_Chk.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            DeleteFile_Chk.ForeColor = Color.Red;
            DeleteFile_Chk.Location = new Point(153, 281);
            DeleteFile_Chk.Name = "DeleteFile_Chk";
            DeleteFile_Chk.Size = new Size(193, 18);
            DeleteFile_Chk.TabIndex = 8;
            DeleteFile_Chk.Text = "Delete original file at end";
            DeleteFile_Chk.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(Pwd_Txt);
            groupBox1.Font = new Font("Verdana", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.DimGray;
            groupBox1.Location = new Point(15, 20);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(472, 50);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "PASSWORD";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(groupBox1);
            groupBox2.Controls.Add(Close_Btn);
            groupBox2.Controls.Add(DeleteFile_Chk);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(Encrypt_Pnl);
            groupBox2.Controls.Add(Decrypt_Pnl);
            groupBox2.Font = new Font("Verdana", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox2.ForeColor = Color.DimGray;
            groupBox2.Location = new Point(12, 5);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(502, 305);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            // 
            // Main_Frm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(28, 28, 28);
            ClientSize = new Size(526, 321);
            Controls.Add(groupBox2);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(498, 282);
            Name = "Main_Frm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "RW5jcnlwdA";
            TopMost = true;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Label label3;
        private Label label4;
        private Panel Encrypt_Pnl;
        private Panel Decrypt_Pnl;
        private TextBox Pwd_Txt;
        private Button Close_Btn;
        private CheckBox DeleteFile_Chk;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
    }
}
