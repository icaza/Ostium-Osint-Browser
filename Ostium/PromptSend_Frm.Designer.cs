namespace Ostium
{
    partial class PromptSend_Frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PromptSend_Frm));
            this.PromptSendForm_Txt = new System.Windows.Forms.TextBox();
            this.Cancel_Btn = new System.Windows.Forms.Button();
            this.Ok_Btn = new System.Windows.Forms.Button();
            this.Local_Chk = new System.Windows.Forms.CheckBox();
            this.Cloud_Chk = new System.Windows.Forms.CheckBox();
            this.SavePrompt_Btn = new System.Windows.Forms.Button();
            this.Prompt_Lst = new System.Windows.Forms.ListBox();
            this.Clear_Btn = new System.Windows.Forms.Button();
            this.RemovePromptList_Btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PromptSendForm_Txt
            // 
            this.PromptSendForm_Txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.PromptSendForm_Txt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PromptSendForm_Txt.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PromptSendForm_Txt.ForeColor = System.Drawing.SystemColors.Info;
            this.PromptSendForm_Txt.Location = new System.Drawing.Point(11, 32);
            this.PromptSendForm_Txt.Multiline = true;
            this.PromptSendForm_Txt.Name = "PromptSendForm_Txt";
            this.PromptSendForm_Txt.Size = new System.Drawing.Size(503, 127);
            this.PromptSendForm_Txt.TabIndex = 1;
            // 
            // Cancel_Btn
            // 
            this.Cancel_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.Cancel_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Cancel_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancel_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancel_Btn.Location = new System.Drawing.Point(422, 166);
            this.Cancel_Btn.Name = "Cancel_Btn";
            this.Cancel_Btn.Size = new System.Drawing.Size(92, 32);
            this.Cancel_Btn.TabIndex = 13;
            this.Cancel_Btn.Text = "✗ Cancel";
            this.Cancel_Btn.UseVisualStyleBackColor = false;
            this.Cancel_Btn.Click += new System.EventHandler(this.Cancel_Btn_Click);
            // 
            // Ok_Btn
            // 
            this.Ok_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.Ok_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Ok_Btn.FlatAppearance.BorderSize = 0;
            this.Ok_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Ok_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ok_Btn.ForeColor = System.Drawing.Color.White;
            this.Ok_Btn.Location = new System.Drawing.Point(320, 166);
            this.Ok_Btn.Name = "Ok_Btn";
            this.Ok_Btn.Size = new System.Drawing.Size(96, 32);
            this.Ok_Btn.TabIndex = 12;
            this.Ok_Btn.Text = "✓ OK";
            this.Ok_Btn.UseVisualStyleBackColor = false;
            this.Ok_Btn.Click += new System.EventHandler(this.Ok_Btn_Click);
            // 
            // Local_Chk
            // 
            this.Local_Chk.AutoSize = true;
            this.Local_Chk.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Local_Chk.ForeColor = System.Drawing.Color.DodgerBlue;
            this.Local_Chk.Location = new System.Drawing.Point(11, 8);
            this.Local_Chk.Name = "Local_Chk";
            this.Local_Chk.Size = new System.Drawing.Size(86, 18);
            this.Local_Chk.TabIndex = 14;
            this.Local_Chk.Text = "Localhost";
            this.Local_Chk.UseVisualStyleBackColor = true;
            // 
            // Cloud_Chk
            // 
            this.Cloud_Chk.AutoSize = true;
            this.Cloud_Chk.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cloud_Chk.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.Cloud_Chk.Location = new System.Drawing.Point(103, 8);
            this.Cloud_Chk.Name = "Cloud_Chk";
            this.Cloud_Chk.Size = new System.Drawing.Size(62, 18);
            this.Cloud_Chk.TabIndex = 15;
            this.Cloud_Chk.Text = "Cloud";
            this.Cloud_Chk.UseVisualStyleBackColor = true;
            // 
            // SavePrompt_Btn
            // 
            this.SavePrompt_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.SavePrompt_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SavePrompt_Btn.FlatAppearance.BorderSize = 0;
            this.SavePrompt_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SavePrompt_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SavePrompt_Btn.ForeColor = System.Drawing.Color.White;
            this.SavePrompt_Btn.Location = new System.Drawing.Point(195, 166);
            this.SavePrompt_Btn.Name = "SavePrompt_Btn";
            this.SavePrompt_Btn.Size = new System.Drawing.Size(119, 32);
            this.SavePrompt_Btn.TabIndex = 16;
            this.SavePrompt_Btn.Text = "💾 Save Prompt";
            this.SavePrompt_Btn.UseVisualStyleBackColor = false;
            this.SavePrompt_Btn.Click += new System.EventHandler(this.SavePrompt_Btn_Click);
            // 
            // Prompt_Lst
            // 
            this.Prompt_Lst.BackColor = System.Drawing.Color.Black;
            this.Prompt_Lst.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Prompt_Lst.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Prompt_Lst.ForeColor = System.Drawing.Color.White;
            this.Prompt_Lst.FormattingEnabled = true;
            this.Prompt_Lst.ItemHeight = 16;
            this.Prompt_Lst.Location = new System.Drawing.Point(11, 204);
            this.Prompt_Lst.Name = "Prompt_Lst";
            this.Prompt_Lst.Size = new System.Drawing.Size(503, 80);
            this.Prompt_Lst.TabIndex = 17;
            this.Prompt_Lst.SelectedIndexChanged += new System.EventHandler(this.Prompt_Lst_SelectedIndexChanged);
            // 
            // Clear_Btn
            // 
            this.Clear_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.Clear_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Clear_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Clear_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Clear_Btn.ForeColor = System.Drawing.Color.Black;
            this.Clear_Btn.Location = new System.Drawing.Point(109, 166);
            this.Clear_Btn.Name = "Clear_Btn";
            this.Clear_Btn.Size = new System.Drawing.Size(80, 32);
            this.Clear_Btn.TabIndex = 18;
            this.Clear_Btn.Text = "🗑 Empty";
            this.Clear_Btn.UseVisualStyleBackColor = false;
            this.Clear_Btn.Click += new System.EventHandler(this.Clear_Btn_Click);
            // 
            // RemovePromptList_Btn
            // 
            this.RemovePromptList_Btn.BackColor = System.Drawing.Color.Red;
            this.RemovePromptList_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RemovePromptList_Btn.FlatAppearance.BorderSize = 0;
            this.RemovePromptList_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RemovePromptList_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemovePromptList_Btn.ForeColor = System.Drawing.Color.White;
            this.RemovePromptList_Btn.Location = new System.Drawing.Point(11, 166);
            this.RemovePromptList_Btn.Name = "RemovePromptList_Btn";
            this.RemovePromptList_Btn.Size = new System.Drawing.Size(92, 32);
            this.RemovePromptList_Btn.TabIndex = 19;
            this.RemovePromptList_Btn.Text = "❌ Remove";
            this.RemovePromptList_Btn.UseVisualStyleBackColor = false;
            this.RemovePromptList_Btn.Click += new System.EventHandler(this.RemovePromptList_Btn_Click);
            // 
            // PromptSend_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(524, 294);
            this.Controls.Add(this.RemovePromptList_Btn);
            this.Controls.Add(this.Clear_Btn);
            this.Controls.Add(this.Prompt_Lst);
            this.Controls.Add(this.SavePrompt_Btn);
            this.Controls.Add(this.Cloud_Chk);
            this.Controls.Add(this.Local_Chk);
            this.Controls.Add(this.Cancel_Btn);
            this.Controls.Add(this.Ok_Btn);
            this.Controls.Add(this.PromptSendForm_Txt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PromptSend_Frm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OOBai Prompt";
            this.Load += new System.EventHandler(this.PromptSend_Frm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox PromptSendForm_Txt;
        private System.Windows.Forms.Button Cancel_Btn;
        private System.Windows.Forms.Button Ok_Btn;
        private System.Windows.Forms.CheckBox Local_Chk;
        private System.Windows.Forms.CheckBox Cloud_Chk;
        private System.Windows.Forms.Button SavePrompt_Btn;
        private System.Windows.Forms.ListBox Prompt_Lst;
        private System.Windows.Forms.Button Clear_Btn;
        private System.Windows.Forms.Button RemovePromptList_Btn;
    }
}