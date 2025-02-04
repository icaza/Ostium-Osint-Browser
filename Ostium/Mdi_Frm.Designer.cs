namespace Ostium
{
    partial class Mdi_Frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mdi_Frm));
            this.Tools_Tls = new System.Windows.Forms.ToolStrip();
            this.NewFrm_Mnu = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.URLlist_Cbx = new System.Windows.Forms.ToolStripComboBox();
            this.AddUrlGrp_Btn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.Cascade_Btn = new System.Windows.Forms.ToolStripMenuItem();
            this.Vertical_Btn = new System.Windows.Forms.ToolStripMenuItem();
            this.Horizontal_Btn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseAllForm_Btn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ReplaceChild_Btn = new System.Windows.Forms.ToolStripButton();
            this.UrlOpn_Lst = new System.Windows.Forms.ListBox();
            this.Tools_Tls.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tools_Tls
            // 
            this.Tools_Tls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Tools_Tls.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tools_Tls.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.Tools_Tls.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewFrm_Mnu,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.URLlist_Cbx,
            this.AddUrlGrp_Btn,
            this.toolStripSeparator1,
            this.toolStripDropDownButton1,
            this.toolStripSeparator3,
            this.ReplaceChild_Btn});
            this.Tools_Tls.Location = new System.Drawing.Point(0, 0);
            this.Tools_Tls.Name = "Tools_Tls";
            this.Tools_Tls.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.Tools_Tls.Size = new System.Drawing.Size(884, 30);
            this.Tools_Tls.TabIndex = 4;
            this.Tools_Tls.Text = "toolStrip1";
            // 
            // NewFrm_Mnu
            // 
            this.NewFrm_Mnu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.NewFrm_Mnu.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewFrm_Mnu.ForeColor = System.Drawing.Color.Gainsboro;
            this.NewFrm_Mnu.Image = ((System.Drawing.Image)(resources.GetObject("NewFrm_Mnu.Image")));
            this.NewFrm_Mnu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NewFrm_Mnu.Name = "NewFrm_Mnu";
            this.NewFrm_Mnu.Size = new System.Drawing.Size(79, 27);
            this.NewFrm_Mnu.Text = "New Form";
            this.NewFrm_Mnu.Click += new System.EventHandler(this.NewFrm_Mnu_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 30);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel1.ForeColor = System.Drawing.Color.Gray;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(122, 27);
            this.toolStripLabel1.Text = "Open URL Group:";
            // 
            // URLlist_Cbx
            // 
            this.URLlist_Cbx.AutoSize = false;
            this.URLlist_Cbx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.URLlist_Cbx.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.URLlist_Cbx.ForeColor = System.Drawing.Color.White;
            this.URLlist_Cbx.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.URLlist_Cbx.Name = "URLlist_Cbx";
            this.URLlist_Cbx.Size = new System.Drawing.Size(170, 22);
            // 
            // AddUrlGrp_Btn
            // 
            this.AddUrlGrp_Btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.AddUrlGrp_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddUrlGrp_Btn.ForeColor = System.Drawing.Color.Lime;
            this.AddUrlGrp_Btn.Image = ((System.Drawing.Image)(resources.GetObject("AddUrlGrp_Btn.Image")));
            this.AddUrlGrp_Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddUrlGrp_Btn.Name = "AddUrlGrp_Btn";
            this.AddUrlGrp_Btn.Size = new System.Drawing.Size(67, 27);
            this.AddUrlGrp_Btn.Text = "Add URL";
            this.AddUrlGrp_Btn.Click += new System.EventHandler(this.AddUrlGrp_Btn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Cascade_Btn,
            this.Vertical_Btn,
            this.Horizontal_Btn,
            this.toolStripSeparator4,
            this.CloseAllForm_Btn});
            this.toolStripDropDownButton1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripDropDownButton1.ForeColor = System.Drawing.Color.Gainsboro;
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(74, 27);
            this.toolStripDropDownButton1.Text = "Window";
            // 
            // Cascade_Btn
            // 
            this.Cascade_Btn.Name = "Cascade_Btn";
            this.Cascade_Btn.Size = new System.Drawing.Size(169, 22);
            this.Cascade_Btn.Text = "Cascade";
            this.Cascade_Btn.Click += new System.EventHandler(this.Cascade_Btn_Click);
            // 
            // Vertical_Btn
            // 
            this.Vertical_Btn.Name = "Vertical_Btn";
            this.Vertical_Btn.Size = new System.Drawing.Size(169, 22);
            this.Vertical_Btn.Text = "Vertical";
            this.Vertical_Btn.Click += new System.EventHandler(this.Vertical_Btn_Click);
            // 
            // Horizontal_Btn
            // 
            this.Horizontal_Btn.Name = "Horizontal_Btn";
            this.Horizontal_Btn.Size = new System.Drawing.Size(169, 22);
            this.Horizontal_Btn.Text = "Horizontal";
            this.Horizontal_Btn.Click += new System.EventHandler(this.Horizontal_Btn_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(166, 6);
            // 
            // CloseAllForm_Btn
            // 
            this.CloseAllForm_Btn.Name = "CloseAllForm_Btn";
            this.CloseAllForm_Btn.Size = new System.Drawing.Size(169, 22);
            this.CloseAllForm_Btn.Text = "Close All Form";
            this.CloseAllForm_Btn.Click += new System.EventHandler(this.CloseAllForm_Btn_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 30);
            // 
            // ReplaceChild_Btn
            // 
            this.ReplaceChild_Btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ReplaceChild_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReplaceChild_Btn.ForeColor = System.Drawing.Color.Gainsboro;
            this.ReplaceChild_Btn.Image = ((System.Drawing.Image)(resources.GetObject("ReplaceChild_Btn.Image")));
            this.ReplaceChild_Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ReplaceChild_Btn.Name = "ReplaceChild_Btn";
            this.ReplaceChild_Btn.Size = new System.Drawing.Size(63, 27);
            this.ReplaceChild_Btn.Text = "Replace";
            this.ReplaceChild_Btn.Click += new System.EventHandler(this.ReplaceChild_Btn_Click);
            // 
            // UrlOpn_Lst
            // 
            this.UrlOpn_Lst.FormattingEnabled = true;
            this.UrlOpn_Lst.Location = new System.Drawing.Point(3, 231);
            this.UrlOpn_Lst.Name = "UrlOpn_Lst";
            this.UrlOpn_Lst.Size = new System.Drawing.Size(19, 17);
            this.UrlOpn_Lst.TabIndex = 5;
            this.UrlOpn_Lst.Visible = false;
            // 
            // Mdi_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(884, 461);
            this.Controls.Add(this.Tools_Tls);
            this.Controls.Add(this.UrlOpn_Lst);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "Mdi_Frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multiple Windows";
            this.Load += new System.EventHandler(this.Mdi_Frm_Load);
            this.Tools_Tls.ResumeLayout(false);
            this.Tools_Tls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip Tools_Tls;
        private System.Windows.Forms.ToolStripButton NewFrm_Mnu;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem Cascade_Btn;
        private System.Windows.Forms.ToolStripMenuItem Vertical_Btn;
        private System.Windows.Forms.ToolStripMenuItem Horizontal_Btn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton ReplaceChild_Btn;
        private System.Windows.Forms.ListBox UrlOpn_Lst;
        private System.Windows.Forms.ToolStripComboBox URLlist_Cbx;
        private System.Windows.Forms.ToolStripButton AddUrlGrp_Btn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem CloseAllForm_Btn;
    }
}