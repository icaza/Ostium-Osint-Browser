namespace RestartSession
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.SessionPathList = new System.Windows.Forms.ListBox();
            this.RestartSessionBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CancelRestartBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.InputSessionName = new System.Windows.Forms.TextBox();
            this.CreateSessionBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(1, 5, 0, 10);
            this.label1.Size = new System.Drawing.Size(455, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "CREATE SESSION";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SessionPathList
            // 
            this.SessionPathList.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.SessionPathList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SessionPathList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SessionPathList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SessionPathList.ForeColor = System.Drawing.Color.White;
            this.SessionPathList.FormattingEnabled = true;
            this.SessionPathList.ItemHeight = 16;
            this.SessionPathList.Location = new System.Drawing.Point(0, 84);
            this.SessionPathList.Name = "SessionPathList";
            this.SessionPathList.Size = new System.Drawing.Size(455, 274);
            this.SessionPathList.TabIndex = 1;
            this.SessionPathList.SelectedIndexChanged += new System.EventHandler(this.SessionPathList_SelectedIndexChanged);
            // 
            // RestartSessionBtn
            // 
            this.RestartSessionBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RestartSessionBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RestartSessionBtn.FlatAppearance.BorderSize = 0;
            this.RestartSessionBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Maroon;
            this.RestartSessionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RestartSessionBtn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RestartSessionBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.RestartSessionBtn.Location = new System.Drawing.Point(170, 0);
            this.RestartSessionBtn.Name = "RestartSessionBtn";
            this.RestartSessionBtn.Size = new System.Drawing.Size(285, 34);
            this.RestartSessionBtn.TabIndex = 2;
            this.RestartSessionBtn.Text = "RESTART SESSION";
            this.RestartSessionBtn.UseVisualStyleBackColor = true;
            this.RestartSessionBtn.Click += new System.EventHandler(this.RestartSessionBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RestartSessionBtn);
            this.panel1.Controls.Add(this.CancelRestartBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 358);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(455, 34);
            this.panel1.TabIndex = 3;
            // 
            // CancelRestartBtn
            // 
            this.CancelRestartBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CancelRestartBtn.Dock = System.Windows.Forms.DockStyle.Left;
            this.CancelRestartBtn.FlatAppearance.BorderSize = 0;
            this.CancelRestartBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Moccasin;
            this.CancelRestartBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelRestartBtn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelRestartBtn.ForeColor = System.Drawing.Color.Red;
            this.CancelRestartBtn.Location = new System.Drawing.Point(0, 0);
            this.CancelRestartBtn.Name = "CancelRestartBtn";
            this.CancelRestartBtn.Size = new System.Drawing.Size(170, 34);
            this.CancelRestartBtn.TabIndex = 3;
            this.CancelRestartBtn.Text = "ANNULER";
            this.CancelRestartBtn.UseVisualStyleBackColor = true;
            this.CancelRestartBtn.Click += new System.EventHandler(this.CancelRestartBtn_Click);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(0, 53);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(1, 8, 0, 10);
            this.label2.Size = new System.Drawing.Size(455, 31);
            this.label2.TabIndex = 4;
            this.label2.Text = "SELECT SESSION";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.panel2.Controls.Add(this.CreateSessionBtn);
            this.panel2.Controls.Add(this.InputSessionName);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 31);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(455, 22);
            this.panel2.TabIndex = 5;
            // 
            // InputSessionName
            // 
            this.InputSessionName.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.InputSessionName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.InputSessionName.Dock = System.Windows.Forms.DockStyle.Left;
            this.InputSessionName.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputSessionName.ForeColor = System.Drawing.Color.White;
            this.InputSessionName.Location = new System.Drawing.Point(0, 0);
            this.InputSessionName.Name = "InputSessionName";
            this.InputSessionName.Size = new System.Drawing.Size(354, 23);
            this.InputSessionName.TabIndex = 0;
            this.InputSessionName.Text = "Choose a session name...";
            // 
            // CreateSessionBtn
            // 
            this.CreateSessionBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CreateSessionBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CreateSessionBtn.FlatAppearance.BorderSize = 0;
            this.CreateSessionBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Maroon;
            this.CreateSessionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CreateSessionBtn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateSessionBtn.ForeColor = System.Drawing.Color.Yellow;
            this.CreateSessionBtn.Location = new System.Drawing.Point(354, 0);
            this.CreateSessionBtn.Name = "CreateSessionBtn";
            this.CreateSessionBtn.Size = new System.Drawing.Size(101, 22);
            this.CreateSessionBtn.TabIndex = 3;
            this.CreateSessionBtn.Text = "CREATE";
            this.CreateSessionBtn.UseVisualStyleBackColor = true;
            this.CreateSessionBtn.Click += new System.EventHandler(this.CreateSessionBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(455, 392);
            this.Controls.Add(this.SessionPathList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(471, 431);
            this.MinimumSize = new System.Drawing.Size(471, 431);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Restart Session or Create";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox SessionPathList;
        private System.Windows.Forms.Button RestartSessionBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button CancelRestartBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button CreateSessionBtn;
        private System.Windows.Forms.TextBox InputSessionName;
    }
}

