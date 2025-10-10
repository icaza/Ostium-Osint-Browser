namespace StylometryAnonymizer
{
    partial class Main_Frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Frm));
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.BtnAnonymize = new System.Windows.Forms.Button();
            this.numVariations = new System.Windows.Forms.NumericUpDown();
            this.chkSyntax = new System.Windows.Forms.CheckBox();
            this.chkVocabulary = new System.Windows.Forms.CheckBox();
            this.chkPunctuation = new System.Windows.Forms.CheckBox();
            this.chkStructure = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnLoadList = new System.Windows.Forms.Button();
            this.BtnClear = new System.Windows.Forms.Button();
            this.BtnCopy = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.numVariations)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(3, 3);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(432, 251);
            this.txtInput.TabIndex = 0;
            // 
            // txtOutput
            // 
            this.txtOutput.BackColor = System.Drawing.Color.White;
            this.txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(441, 3);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(433, 251);
            this.txtOutput.TabIndex = 1;
            this.txtOutput.Text = "";
            // 
            // BtnAnonymize
            // 
            this.BtnAnonymize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.BtnAnonymize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnAnonymize.FlatAppearance.BorderSize = 0;
            this.BtnAnonymize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnAnonymize.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnAnonymize.ForeColor = System.Drawing.Color.White;
            this.BtnAnonymize.Location = new System.Drawing.Point(468, 80);
            this.BtnAnonymize.Name = "BtnAnonymize";
            this.BtnAnonymize.Size = new System.Drawing.Size(394, 31);
            this.BtnAnonymize.TabIndex = 2;
            this.BtnAnonymize.Text = "ANONYMISER";
            this.BtnAnonymize.UseVisualStyleBackColor = false;
            this.BtnAnonymize.Click += new System.EventHandler(this.BtnAnonymize_Click);
            // 
            // numVariations
            // 
            this.numVariations.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.numVariations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numVariations.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numVariations.ForeColor = System.Drawing.Color.White;
            this.numVariations.Location = new System.Drawing.Point(602, 20);
            this.numVariations.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numVariations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numVariations.Name = "numVariations";
            this.numVariations.Size = new System.Drawing.Size(43, 23);
            this.numVariations.TabIndex = 3;
            this.numVariations.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // chkSyntax
            // 
            this.chkSyntax.AutoSize = true;
            this.chkSyntax.Checked = true;
            this.chkSyntax.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSyntax.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSyntax.Location = new System.Drawing.Point(11, 30);
            this.chkSyntax.Name = "chkSyntax";
            this.chkSyntax.Size = new System.Drawing.Size(131, 18);
            this.chkSyntax.TabIndex = 4;
            this.chkSyntax.Text = "Varier la syntaxe";
            this.chkSyntax.UseVisualStyleBackColor = true;
            // 
            // chkVocabulary
            // 
            this.chkVocabulary.AutoSize = true;
            this.chkVocabulary.Checked = true;
            this.chkVocabulary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVocabulary.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkVocabulary.Location = new System.Drawing.Point(11, 63);
            this.chkVocabulary.Name = "chkVocabulary";
            this.chkVocabulary.Size = new System.Drawing.Size(182, 18);
            this.chkVocabulary.TabIndex = 5;
            this.chkVocabulary.Text = "Remplacer le vocabulaire";
            this.chkVocabulary.UseVisualStyleBackColor = true;
            // 
            // chkPunctuation
            // 
            this.chkPunctuation.AutoSize = true;
            this.chkPunctuation.Checked = true;
            this.chkPunctuation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPunctuation.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPunctuation.Location = new System.Drawing.Point(218, 30);
            this.chkPunctuation.Name = "chkPunctuation";
            this.chkPunctuation.Size = new System.Drawing.Size(169, 18);
            this.chkPunctuation.TabIndex = 6;
            this.chkPunctuation.Text = "Modifier la ponctuation";
            this.chkPunctuation.UseVisualStyleBackColor = true;
            // 
            // chkStructure
            // 
            this.chkStructure.AutoSize = true;
            this.chkStructure.Checked = true;
            this.chkStructure.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStructure.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkStructure.Location = new System.Drawing.Point(218, 64);
            this.chkStructure.Name = "chkStructure";
            this.chkStructure.Size = new System.Drawing.Size(181, 18);
            this.chkStructure.TabIndex = 7;
            this.chkStructure.Text = "Restructurer les phrases";
            this.chkStructure.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.label1.Size = new System.Drawing.Size(95, 27);
            this.label1.TabIndex = 8;
            this.label1.Text = "Texte Original";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(441, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.label2.Size = new System.Drawing.Size(126, 27);
            this.label2.TabIndex = 9;
            this.label2.Text = "Textes Anonymisés";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(466, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "Nombre de variations:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.panel1.Controls.Add(this.BtnLoadList);
            this.panel1.Controls.Add(this.BtnClear);
            this.panel1.Controls.Add(this.BtnCopy);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.numVariations);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.BtnAnonymize);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 294);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(877, 127);
            this.panel1.TabIndex = 11;
            // 
            // BtnLoadList
            // 
            this.BtnLoadList.BackColor = System.Drawing.Color.LimeGreen;
            this.BtnLoadList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnLoadList.FlatAppearance.BorderSize = 0;
            this.BtnLoadList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnLoadList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnLoadList.ForeColor = System.Drawing.Color.White;
            this.BtnLoadList.Location = new System.Drawing.Point(536, 52);
            this.BtnLoadList.Name = "BtnLoadList";
            this.BtnLoadList.Size = new System.Drawing.Size(181, 22);
            this.BtnLoadList.TabIndex = 14;
            this.BtnLoadList.Text = "RECHARGER SYNONYMES";
            this.BtnLoadList.UseVisualStyleBackColor = false;
            this.BtnLoadList.Click += new System.EventHandler(this.BtnLoadList_Click);
            // 
            // BtnClear
            // 
            this.BtnClear.BackColor = System.Drawing.Color.Red;
            this.BtnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnClear.FlatAppearance.BorderSize = 0;
            this.BtnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnClear.ForeColor = System.Drawing.Color.White;
            this.BtnClear.Location = new System.Drawing.Point(468, 52);
            this.BtnClear.Name = "BtnClear";
            this.BtnClear.Size = new System.Drawing.Size(62, 22);
            this.BtnClear.TabIndex = 13;
            this.BtnClear.Text = "VIDER";
            this.BtnClear.UseVisualStyleBackColor = false;
            this.BtnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // BtnCopy
            // 
            this.BtnCopy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BtnCopy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnCopy.FlatAppearance.BorderSize = 0;
            this.BtnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCopy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCopy.ForeColor = System.Drawing.Color.White;
            this.BtnCopy.Location = new System.Drawing.Point(723, 52);
            this.BtnCopy.Name = "BtnCopy";
            this.BtnCopy.Size = new System.Drawing.Size(139, 22);
            this.BtnCopy.TabIndex = 12;
            this.BtnCopy.Text = "COPIER LA SELECTION";
            this.BtnCopy.UseVisualStyleBackColor = false;
            this.BtnCopy.Click += new System.EventHandler(this.BtnCopy_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSyntax);
            this.groupBox1.Controls.Add(this.chkVocabulary);
            this.groupBox1.Controls.Add(this.chkPunctuation);
            this.groupBox1.Controls.Add(this.chkStructure);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(15, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(417, 100);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options de transformation";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.txtInput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtOutput, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 37);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(877, 257);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(877, 37);
            this.tableLayoutPanel2.TabIndex = 13;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 421);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(877, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 14;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.ForeColor = System.Drawing.Color.Silver;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(16, 17);
            this.lblStatus.Text = "...";
            // 
            // Main_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 443);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main_Frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Anonymiseur Stylométrique";
            ((System.ComponentModel.ISupportInitialize)(this.numVariations)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.RichTextBox txtOutput;
        private System.Windows.Forms.Button BtnAnonymize;
        private System.Windows.Forms.NumericUpDown numVariations;
        private System.Windows.Forms.CheckBox chkSyntax;
        private System.Windows.Forms.CheckBox chkVocabulary;
        private System.Windows.Forms.CheckBox chkPunctuation;
        private System.Windows.Forms.CheckBox chkStructure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button BtnCopy;
        private System.Windows.Forms.Button BtnClear;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Button BtnLoadList;
    }
}

