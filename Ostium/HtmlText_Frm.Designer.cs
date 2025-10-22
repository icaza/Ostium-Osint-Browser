namespace Ostium
{
    partial class HtmlText_Frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HtmlText_Frm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.URLbrowse_Cbx = new System.Windows.Forms.ComboBox();
            this.EmptyCbx_Btn = new System.Windows.Forms.Button();
            this.Go_Btn = new System.Windows.Forms.Button();
            this.SavePageTxt_Btn = new System.Windows.Forms.Button();
            this.CopyUrl_Btn = new System.Windows.Forms.Button();
            this.WbrowseTxt = new System.Windows.Forms.RichTextBox();
            this.ListLinks_Lst = new System.Windows.Forms.ListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.UserAgent_Txt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.URLbrowse_Cbx);
            this.panel1.Controls.Add(this.EmptyCbx_Btn);
            this.panel1.Controls.Add(this.Go_Btn);
            this.panel1.Controls.Add(this.SavePageTxt_Btn);
            this.panel1.Controls.Add(this.CopyUrl_Btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 24);
            this.panel1.TabIndex = 3;
            // 
            // URLbrowse_Cbx
            // 
            this.URLbrowse_Cbx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.URLbrowse_Cbx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.URLbrowse_Cbx.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.URLbrowse_Cbx.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.URLbrowse_Cbx.ForeColor = System.Drawing.Color.Gold;
            this.URLbrowse_Cbx.FormattingEnabled = true;
            this.URLbrowse_Cbx.Location = new System.Drawing.Point(130, 0);
            this.URLbrowse_Cbx.Name = "URLbrowse_Cbx";
            this.URLbrowse_Cbx.Size = new System.Drawing.Size(610, 24);
            this.URLbrowse_Cbx.TabIndex = 10;
            // 
            // EmptyCbx_Btn
            // 
            this.EmptyCbx_Btn.BackColor = System.Drawing.Color.Red;
            this.EmptyCbx_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EmptyCbx_Btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.EmptyCbx_Btn.FlatAppearance.BorderSize = 0;
            this.EmptyCbx_Btn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.EmptyCbx_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.EmptyCbx_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.EmptyCbx_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EmptyCbx_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmptyCbx_Btn.ForeColor = System.Drawing.Color.White;
            this.EmptyCbx_Btn.Location = new System.Drawing.Point(106, 0);
            this.EmptyCbx_Btn.Name = "EmptyCbx_Btn";
            this.EmptyCbx_Btn.Size = new System.Drawing.Size(24, 22);
            this.EmptyCbx_Btn.TabIndex = 11;
            this.EmptyCbx_Btn.Text = "#";
            this.EmptyCbx_Btn.UseVisualStyleBackColor = false;
            this.EmptyCbx_Btn.Click += new System.EventHandler(this.EmptyCbx_Btn_Click);
            // 
            // Go_Btn
            // 
            this.Go_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Go_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Go_Btn.Dock = System.Windows.Forms.DockStyle.Right;
            this.Go_Btn.FlatAppearance.BorderSize = 0;
            this.Go_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.Go_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.Go_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Go_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Go_Btn.ForeColor = System.Drawing.Color.White;
            this.Go_Btn.Location = new System.Drawing.Point(740, 0);
            this.Go_Btn.Name = "Go_Btn";
            this.Go_Btn.Size = new System.Drawing.Size(58, 22);
            this.Go_Btn.TabIndex = 7;
            this.Go_Btn.Text = "Go";
            this.Go_Btn.UseVisualStyleBackColor = false;
            this.Go_Btn.Click += new System.EventHandler(this.Go_Btn_Click);
            // 
            // SavePageTxt_Btn
            // 
            this.SavePageTxt_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.SavePageTxt_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SavePageTxt_Btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.SavePageTxt_Btn.FlatAppearance.BorderSize = 0;
            this.SavePageTxt_Btn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.SavePageTxt_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.SavePageTxt_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.SavePageTxt_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SavePageTxt_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SavePageTxt_Btn.ForeColor = System.Drawing.Color.White;
            this.SavePageTxt_Btn.Location = new System.Drawing.Point(53, 0);
            this.SavePageTxt_Btn.Name = "SavePageTxt_Btn";
            this.SavePageTxt_Btn.Size = new System.Drawing.Size(53, 22);
            this.SavePageTxt_Btn.TabIndex = 12;
            this.SavePageTxt_Btn.Text = "Save";
            this.SavePageTxt_Btn.UseVisualStyleBackColor = false;
            this.SavePageTxt_Btn.Click += new System.EventHandler(this.SavePageTxt_Btn_Click);
            // 
            // CopyUrl_Btn
            // 
            this.CopyUrl_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.CopyUrl_Btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CopyUrl_Btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.CopyUrl_Btn.FlatAppearance.BorderSize = 0;
            this.CopyUrl_Btn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.CopyUrl_Btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.CopyUrl_Btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.CopyUrl_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CopyUrl_Btn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CopyUrl_Btn.ForeColor = System.Drawing.Color.White;
            this.CopyUrl_Btn.Location = new System.Drawing.Point(0, 0);
            this.CopyUrl_Btn.Name = "CopyUrl_Btn";
            this.CopyUrl_Btn.Size = new System.Drawing.Size(53, 22);
            this.CopyUrl_Btn.TabIndex = 8;
            this.CopyUrl_Btn.Text = "Cp";
            this.CopyUrl_Btn.UseVisualStyleBackColor = false;
            this.CopyUrl_Btn.Click += new System.EventHandler(this.CopyUrl_Btn_Click);
            // 
            // WbrowseTxt
            // 
            this.WbrowseTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.WbrowseTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.WbrowseTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WbrowseTxt.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WbrowseTxt.ForeColor = System.Drawing.Color.Lime;
            this.WbrowseTxt.Location = new System.Drawing.Point(0, 0);
            this.WbrowseTxt.Name = "WbrowseTxt";
            this.WbrowseTxt.Size = new System.Drawing.Size(800, 310);
            this.WbrowseTxt.TabIndex = 4;
            this.WbrowseTxt.Text = "";
            // 
            // ListLinks_Lst
            // 
            this.ListLinks_Lst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ListLinks_Lst.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ListLinks_Lst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListLinks_Lst.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListLinks_Lst.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ListLinks_Lst.FormattingEnabled = true;
            this.ListLinks_Lst.ItemHeight = 15;
            this.ListLinks_Lst.Location = new System.Drawing.Point(0, 0);
            this.ListLinks_Lst.Name = "ListLinks_Lst";
            this.ListLinks_Lst.Size = new System.Drawing.Size(800, 97);
            this.ListLinks_Lst.TabIndex = 5;
            this.ListLinks_Lst.SelectedIndexChanged += new System.EventHandler(this.ListLinks_Lst_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.WbrowseTxt);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ListLinks_Lst);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(800, 426);
            this.splitContainer1.SplitterDistance = 310;
            this.splitContainer1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.UserAgent_Txt);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 97);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(800, 15);
            this.panel2.TabIndex = 6;
            // 
            // UserAgent_Txt
            // 
            this.UserAgent_Txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.UserAgent_Txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.UserAgent_Txt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UserAgent_Txt.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserAgent_Txt.ForeColor = System.Drawing.SystemColors.Info;
            this.UserAgent_Txt.Location = new System.Drawing.Point(85, 0);
            this.UserAgent_Txt.Name = "UserAgent_Txt";
            this.UserAgent_Txt.Size = new System.Drawing.Size(715, 13);
            this.UserAgent_Txt.TabIndex = 0;
            this.UserAgent_Txt.Text = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
    "Chrome/140.0.0.0 Safari/537.36";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "User-Agent:";
            // 
            // HtmlText_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HtmlText_Frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HTML Text";
            this.Load += new System.EventHandler(this.HtmlText_Frm_Load);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox URLbrowse_Cbx;
        private System.Windows.Forms.Button EmptyCbx_Btn;
        private System.Windows.Forms.Button Go_Btn;
        private System.Windows.Forms.Button CopyUrl_Btn;
        private System.Windows.Forms.RichTextBox WbrowseTxt;
        private System.Windows.Forms.ListBox ListLinks_Lst;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox UserAgent_Txt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SavePageTxt_Btn;
    }
}