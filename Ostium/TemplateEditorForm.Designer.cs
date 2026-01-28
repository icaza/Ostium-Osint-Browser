namespace Ostium
{
    partial class TemplateEditorForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.lblContent = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.IconTextSelect_Cbx = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(382, 304);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 32);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "✗ Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(282, 304);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 32);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "✓ OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // txtContent
            // 
            this.txtContent.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtContent.Location = new System.Drawing.Point(12, 89);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtContent.Size = new System.Drawing.Size(460, 200);
            this.txtContent.TabIndex = 9;
            // 
            // lblContent
            // 
            this.lblContent.AutoSize = true;
            this.lblContent.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContent.ForeColor = System.Drawing.Color.White;
            this.lblContent.Location = new System.Drawing.Point(12, 69);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(63, 14);
            this.lblContent.TabIndex = 8;
            this.lblContent.Text = "Content:";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(12, 34);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(312, 22);
            this.txtName.TabIndex = 7;
            this.txtName.Text = "📝 My template";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Location = new System.Drawing.Point(12, 14);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(113, 14);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "Template name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(330, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 14);
            this.label1.TabIndex = 12;
            this.label1.Text = "Select icon:";
            // 
            // IconTextSelect_Cbx
            // 
            this.IconTextSelect_Cbx.FormattingEnabled = true;
            this.IconTextSelect_Cbx.Items.AddRange(new object[] {
            "⏸ ",
            "⏹",
            "⏯",
            "⏭",
            "⏮",
            "⏏",
            "▶️",
            "🔼",
            "🔽",
            "⏩",
            "⏪",
            "🔁",
            "🔀",
            "🔢",
            "1️⃣",
            "📶",
            "🔢",
            "🆙",
            "⬇️",
            "🔄",
            "🅰️ ",
            "⏳",
            "⏰",
            "⏱ ",
            "⏲ ",
            "🕛",
            "🎵",
            "🎶",
            "🔊",
            "🔉",
            "🔈",
            "🔇",
            "🎤",
            "🎧",
            "🖥",
            "💻",
            "🖥️",
            "🖱",
            "⌨",
            "🎛",
            "🛠",
            "⌨ ",
            "🖲",
            "🏷",
            "📡",
            "🛜",
            "🖧",
            "📱",
            "🔋",
            "🔌",
            "📡",
            "🖨",
            "💾",
            "📀",
            "💿",
            "🗄",
            "📍",
            "🧭",
            "📌",
            "🗺",
            "🚩",
            "🚀",
            "🌍",
            "🔬",
            "🔭",
            "⚛",
            "⚙",
            "⚠️",
            "❗",
            "❓",
            "ℹ ",
            "🚫",
            "⛔",
            "❌",
            "✅",
            "☑",
            "🔴",
            "🟢",
            "🟡",
            "🔵",
            "🐞",
            "🏗",
            "🧩",
            "📟",
            "🎯",
            "📦",
            "📝",
            "📜",
            "📊",
            "🌐",
            "🔗",
            "📩",
            "📤",
            "🔒",
            "🔓",
            "⚠️",
            "🏠",
            "🏴‍☠️",
            "⚡",
            "🔌",
            "📞",
            "🗂",
            "💀",
            "🔍",
            "📛"});
            this.IconTextSelect_Cbx.Location = new System.Drawing.Point(417, 35);
            this.IconTextSelect_Cbx.Name = "IconTextSelect_Cbx";
            this.IconTextSelect_Cbx.Size = new System.Drawing.Size(55, 21);
            this.IconTextSelect_Cbx.TabIndex = 13;
            this.IconTextSelect_Cbx.Text = "⏹";
            this.IconTextSelect_Cbx.SelectedIndexChanged += new System.EventHandler(this.IconTextSelect_Cbx_SelectedIndexChanged);
            // 
            // TemplateEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(484, 351);
            this.Controls.Add(this.IconTextSelect_Cbx);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.lblContent);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateEditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add a custom template";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox IconTextSelect_Cbx;
    }
}