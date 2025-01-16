namespace Ostium
{
    partial class Doc_Frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Doc_Frm));
            this.Sortie_Txt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Sortie_Txt
            // 
            this.Sortie_Txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Sortie_Txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Sortie_Txt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Sortie_Txt.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Sortie_Txt.ForeColor = System.Drawing.Color.White;
            this.Sortie_Txt.Location = new System.Drawing.Point(0, 0);
            this.Sortie_Txt.Multiline = true;
            this.Sortie_Txt.Name = "Sortie_Txt";
            this.Sortie_Txt.Size = new System.Drawing.Size(597, 281);
            this.Sortie_Txt.TabIndex = 1;
            // 
            // Doc_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 281);
            this.Controls.Add(this.Sortie_Txt);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Doc_Frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Doc_Frm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Sortie_Txt;
    }
}