namespace Ostium
{
    partial class OpenSource_Frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenSource_Frm));
            this.Sortie_Lst = new System.Windows.Forms.ListBox();
            this.Tools_Bar = new System.Windows.Forms.ToolStrip();
            this.SaveData_Btn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TopNo_Btn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Status_Bar = new System.Windows.Forms.StatusStrip();
            this.Count_Lbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.Sorted_Btn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Tools_Bar.SuspendLayout();
            this.Status_Bar.SuspendLayout();
            this.SuspendLayout();
            // 
            // Sortie_Lst
            // 
            this.Sortie_Lst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Sortie_Lst.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Sortie_Lst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Sortie_Lst.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Sortie_Lst.ForeColor = System.Drawing.Color.White;
            this.Sortie_Lst.FormattingEnabled = true;
            this.Sortie_Lst.ItemHeight = 16;
            this.Sortie_Lst.Location = new System.Drawing.Point(0, 25);
            this.Sortie_Lst.Name = "Sortie_Lst";
            this.Sortie_Lst.Size = new System.Drawing.Size(496, 230);
            this.Sortie_Lst.TabIndex = 5;
            this.Sortie_Lst.SelectedIndexChanged += new System.EventHandler(this.Sortie_Lst_SelectedIndexChanged);
            // 
            // Tools_Bar
            // 
            this.Tools_Bar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Tools_Bar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.Tools_Bar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveData_Btn,
            this.toolStripSeparator1,
            this.TopNo_Btn,
            this.toolStripSeparator2,
            this.Sorted_Btn,
            this.toolStripSeparator3});
            this.Tools_Bar.Location = new System.Drawing.Point(0, 0);
            this.Tools_Bar.Name = "Tools_Bar";
            this.Tools_Bar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.Tools_Bar.Size = new System.Drawing.Size(496, 25);
            this.Tools_Bar.TabIndex = 3;
            // 
            // SaveData_Btn
            // 
            this.SaveData_Btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.SaveData_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveData_Btn.ForeColor = System.Drawing.Color.White;
            this.SaveData_Btn.Image = ((System.Drawing.Image)(resources.GetObject("SaveData_Btn.Image")));
            this.SaveData_Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveData_Btn.Name = "SaveData_Btn";
            this.SaveData_Btn.Size = new System.Drawing.Size(42, 22);
            this.SaveData_Btn.Text = "Save";
            this.SaveData_Btn.Click += new System.EventHandler(this.SaveData_Btn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // TopNo_Btn
            // 
            this.TopNo_Btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TopNo_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TopNo_Btn.ForeColor = System.Drawing.Color.White;
            this.TopNo_Btn.Image = ((System.Drawing.Image)(resources.GetObject("TopNo_Btn.Image")));
            this.TopNo_Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TopNo_Btn.Name = "TopNo_Btn";
            this.TopNo_Btn.Size = new System.Drawing.Size(33, 22);
            this.TopNo_Btn.Text = "Top";
            this.TopNo_Btn.Click += new System.EventHandler(this.TopNo_Btn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // Status_Bar
            // 
            this.Status_Bar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Status_Bar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Count_Lbl});
            this.Status_Bar.Location = new System.Drawing.Point(0, 255);
            this.Status_Bar.Name = "Status_Bar";
            this.Status_Bar.Size = new System.Drawing.Size(496, 22);
            this.Status_Bar.SizingGrip = false;
            this.Status_Bar.TabIndex = 4;
            // 
            // Count_Lbl
            // 
            this.Count_Lbl.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Count_Lbl.ForeColor = System.Drawing.Color.Gray;
            this.Count_Lbl.Name = "Count_Lbl";
            this.Count_Lbl.Size = new System.Drawing.Size(19, 17);
            this.Count_Lbl.Text = "...";
            // 
            // Sorted_Btn
            // 
            this.Sorted_Btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Sorted_Btn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Sorted_Btn.ForeColor = System.Drawing.Color.White;
            this.Sorted_Btn.Image = ((System.Drawing.Image)(resources.GetObject("Sorted_Btn.Image")));
            this.Sorted_Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Sorted_Btn.Name = "Sorted_Btn";
            this.Sorted_Btn.Size = new System.Drawing.Size(53, 22);
            this.Sorted_Btn.Text = "Sorted";
            this.Sorted_Btn.Click += new System.EventHandler(this.Sorted_Btn_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // OpenSource_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(496, 277);
            this.Controls.Add(this.Sortie_Lst);
            this.Controls.Add(this.Tools_Bar);
            this.Controls.Add(this.Status_Bar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OpenSource_Frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.OpenSource_Frm_Load);
            this.Tools_Bar.ResumeLayout(false);
            this.Tools_Bar.PerformLayout();
            this.Status_Bar.ResumeLayout(false);
            this.Status_Bar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox Sortie_Lst;
        private System.Windows.Forms.ToolStrip Tools_Bar;
        private System.Windows.Forms.ToolStripButton SaveData_Btn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton TopNo_Btn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.StatusStrip Status_Bar;
        private System.Windows.Forms.ToolStripStatusLabel Count_Lbl;
        private System.Windows.Forms.ToolStripButton Sorted_Btn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}