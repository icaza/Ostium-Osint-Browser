namespace OstiumE
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Frm));
            this.Tools_Tls = new System.Windows.Forms.ToolStrip();
            this.File_Tls = new System.Windows.Forms.ToolStripDropDownButton();
            this.NewFile_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.NewWindow_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFile_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseFile_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Save_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAs_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Exit_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.Edit_Tls = new System.Windows.Forms.ToolStripDropDownButton();
            this.FindReplace = new System.Windows.Forms.ToolStripMenuItem();
            this.Find_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.Replace_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.Regiontls = new System.Windows.Forms.ToolStripMenuItem();
            this.Collapse_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.Expand_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.Lang_Tls = new System.Windows.Forms.ToolStripDropDownButton();
            this.Custom_Lang = new System.Windows.Forms.ToolStripMenuItem();
            this.CSharp_Lang = new System.Windows.Forms.ToolStripMenuItem();
            this.VB_Lang = new System.Windows.Forms.ToolStripMenuItem();
            this.HTML_Lang = new System.Windows.Forms.ToolStripMenuItem();
            this.XML_Lang = new System.Windows.Forms.ToolStripMenuItem();
            this.SQL_Lang = new System.Windows.Forms.ToolStripMenuItem();
            this.PHP_Lang = new System.Windows.Forms.ToolStripMenuItem();
            this.JS_Lang = new System.Windows.Forms.ToolStripMenuItem();
            this.LUA_Lang = new System.Windows.Forms.ToolStripMenuItem();
            this.Window_Tls = new System.Windows.Forms.ToolStripDropDownButton();
            this.TopNo_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.Config_Tls = new System.Windows.Forms.ToolStripDropDownButton();
            this.SetColors_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.Reload_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.About_Tools = new System.Windows.Forms.ToolStripButton();
            this.Status_Strip = new System.Windows.Forms.StatusStrip();
            this.Lang_Sts = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.FileDir_Sts = new System.Windows.Forms.ToolStripStatusLabel();
            this.Output_Txt = new FastColoredTextBoxNS.FastColoredTextBox();
            this.Menu_Mnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Cut_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.Copy_Mnu = new System.Windows.Forms.ToolStripMenuItem();
            this.Paste_Mnu = new System.Windows.Forms.ToolStripMenuItem();
            this.Delete_Mnu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.SelectAll_Mnu = new System.Windows.Forms.ToolStripMenuItem();
            this.SetColor_Lbl = new System.Windows.Forms.ListBox();
            this.Tools_Tls.SuspendLayout();
            this.Status_Strip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Output_Txt)).BeginInit();
            this.Menu_Mnu.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tools_Tls
            // 
            this.Tools_Tls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Tools_Tls.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tools_Tls.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.Tools_Tls.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.File_Tls,
            this.Edit_Tls,
            this.Lang_Tls,
            this.Window_Tls,
            this.Config_Tls,
            this.About_Tools});
            this.Tools_Tls.Location = new System.Drawing.Point(0, 0);
            this.Tools_Tls.Name = "Tools_Tls";
            this.Tools_Tls.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.Tools_Tls.Size = new System.Drawing.Size(800, 25);
            this.Tools_Tls.TabIndex = 0;
            this.Tools_Tls.Text = "toolStrip1";
            // 
            // File_Tls
            // 
            this.File_Tls.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.File_Tls.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewFile_Tools,
            this.NewWindow_Tools,
            this.OpenFile_Tools,
            this.CloseFile_Tools,
            this.toolStripSeparator1,
            this.Save_Tools,
            this.SaveAs_Tools,
            this.toolStripSeparator2,
            this.Exit_Tools});
            this.File_Tls.ForeColor = System.Drawing.Color.White;
            this.File_Tls.Image = ((System.Drawing.Image)(resources.GetObject("File_Tls.Image")));
            this.File_Tls.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.File_Tls.Name = "File_Tls";
            this.File_Tls.Size = new System.Drawing.Size(42, 22);
            this.File_Tls.Text = "File";
            // 
            // NewFile_Tools
            // 
            this.NewFile_Tools.Name = "NewFile_Tools";
            this.NewFile_Tools.Size = new System.Drawing.Size(156, 22);
            this.NewFile_Tools.Text = "New";
            this.NewFile_Tools.Click += new System.EventHandler(this.NewFile_Tools_Click);
            // 
            // NewWindow_Tools
            // 
            this.NewWindow_Tools.Name = "NewWindow_Tools";
            this.NewWindow_Tools.Size = new System.Drawing.Size(156, 22);
            this.NewWindow_Tools.Text = "New window";
            this.NewWindow_Tools.Click += new System.EventHandler(this.NewWindow_Tools_Click);
            // 
            // OpenFile_Tools
            // 
            this.OpenFile_Tools.Name = "OpenFile_Tools";
            this.OpenFile_Tools.Size = new System.Drawing.Size(156, 22);
            this.OpenFile_Tools.Text = "Open";
            this.OpenFile_Tools.Click += new System.EventHandler(this.OpenFile_Tools_Click);
            // 
            // CloseFile_Tools
            // 
            this.CloseFile_Tools.Name = "CloseFile_Tools";
            this.CloseFile_Tools.Size = new System.Drawing.Size(156, 22);
            this.CloseFile_Tools.Text = "Close";
            this.CloseFile_Tools.Click += new System.EventHandler(this.CloseFile_Tools_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(153, 6);
            // 
            // Save_Tools
            // 
            this.Save_Tools.Name = "Save_Tools";
            this.Save_Tools.Size = new System.Drawing.Size(156, 22);
            this.Save_Tools.Text = "Save";
            this.Save_Tools.Click += new System.EventHandler(this.Save_Tools_Click);
            // 
            // SaveAs_Tools
            // 
            this.SaveAs_Tools.Name = "SaveAs_Tools";
            this.SaveAs_Tools.Size = new System.Drawing.Size(156, 22);
            this.SaveAs_Tools.Text = "Save As";
            this.SaveAs_Tools.Click += new System.EventHandler(this.SaveAs_Tools_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(153, 6);
            // 
            // Exit_Tools
            // 
            this.Exit_Tools.Name = "Exit_Tools";
            this.Exit_Tools.Size = new System.Drawing.Size(156, 22);
            this.Exit_Tools.Text = "Exit";
            this.Exit_Tools.Click += new System.EventHandler(this.Exit_Tools_Click);
            // 
            // Edit_Tls
            // 
            this.Edit_Tls.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Edit_Tls.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FindReplace,
            this.Regiontls});
            this.Edit_Tls.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Edit_Tls.ForeColor = System.Drawing.Color.White;
            this.Edit_Tls.Image = ((System.Drawing.Image)(resources.GetObject("Edit_Tls.Image")));
            this.Edit_Tls.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Edit_Tls.Name = "Edit_Tls";
            this.Edit_Tls.Size = new System.Drawing.Size(45, 22);
            this.Edit_Tls.Text = "Edit";
            // 
            // FindReplace
            // 
            this.FindReplace.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Find_Tools,
            this.Replace_Tools});
            this.FindReplace.Name = "FindReplace";
            this.FindReplace.Size = new System.Drawing.Size(186, 22);
            this.FindReplace.Text = "Find and Replace";
            // 
            // Find_Tools
            // 
            this.Find_Tools.Name = "Find_Tools";
            this.Find_Tools.Size = new System.Drawing.Size(125, 22);
            this.Find_Tools.Text = "Find";
            this.Find_Tools.Click += new System.EventHandler(this.Find_Tools_Click);
            // 
            // Replace_Tools
            // 
            this.Replace_Tools.Name = "Replace_Tools";
            this.Replace_Tools.Size = new System.Drawing.Size(125, 22);
            this.Replace_Tools.Text = "Replace";
            this.Replace_Tools.Click += new System.EventHandler(this.Replace_Tools_Click);
            // 
            // Regiontls
            // 
            this.Regiontls.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Collapse_Tools,
            this.Expand_Tools});
            this.Regiontls.Name = "Regiontls";
            this.Regiontls.Size = new System.Drawing.Size(186, 22);
            this.Regiontls.Text = "Region (CSharp)";
            // 
            // Collapse_Tools
            // 
            this.Collapse_Tools.Name = "Collapse_Tools";
            this.Collapse_Tools.Size = new System.Drawing.Size(148, 22);
            this.Collapse_Tools.Text = "Collapse All";
            this.Collapse_Tools.Click += new System.EventHandler(this.Collapse_Tools_Click);
            // 
            // Expand_Tools
            // 
            this.Expand_Tools.Name = "Expand_Tools";
            this.Expand_Tools.Size = new System.Drawing.Size(148, 22);
            this.Expand_Tools.Text = "Expand All";
            this.Expand_Tools.Click += new System.EventHandler(this.Expand_Tools_Click);
            // 
            // Lang_Tls
            // 
            this.Lang_Tls.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Lang_Tls.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Custom_Lang,
            this.CSharp_Lang,
            this.VB_Lang,
            this.HTML_Lang,
            this.XML_Lang,
            this.SQL_Lang,
            this.PHP_Lang,
            this.JS_Lang,
            this.LUA_Lang});
            this.Lang_Tls.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lang_Tls.ForeColor = System.Drawing.Color.White;
            this.Lang_Tls.Image = ((System.Drawing.Image)(resources.GetObject("Lang_Tls.Image")));
            this.Lang_Tls.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Lang_Tls.Name = "Lang_Tls";
            this.Lang_Tls.Size = new System.Drawing.Size(83, 22);
            this.Lang_Tls.Text = "Language";
            // 
            // Custom_Lang
            // 
            this.Custom_Lang.Name = "Custom_Lang";
            this.Custom_Lang.Size = new System.Drawing.Size(180, 22);
            this.Custom_Lang.Text = "Custom";
            this.Custom_Lang.Visible = false;
            this.Custom_Lang.Click += new System.EventHandler(this.LanguageSelect);
            // 
            // CSharp_Lang
            // 
            this.CSharp_Lang.Name = "CSharp_Lang";
            this.CSharp_Lang.Size = new System.Drawing.Size(180, 22);
            this.CSharp_Lang.Text = "CSharp";
            this.CSharp_Lang.Click += new System.EventHandler(this.LanguageSelect);
            // 
            // VB_Lang
            // 
            this.VB_Lang.Name = "VB_Lang";
            this.VB_Lang.Size = new System.Drawing.Size(180, 22);
            this.VB_Lang.Text = "VB";
            this.VB_Lang.Click += new System.EventHandler(this.LanguageSelect);
            // 
            // HTML_Lang
            // 
            this.HTML_Lang.Name = "HTML_Lang";
            this.HTML_Lang.Size = new System.Drawing.Size(180, 22);
            this.HTML_Lang.Text = "HTML";
            this.HTML_Lang.Click += new System.EventHandler(this.LanguageSelect);
            // 
            // XML_Lang
            // 
            this.XML_Lang.Name = "XML_Lang";
            this.XML_Lang.Size = new System.Drawing.Size(180, 22);
            this.XML_Lang.Text = "XML";
            this.XML_Lang.Click += new System.EventHandler(this.LanguageSelect);
            // 
            // SQL_Lang
            // 
            this.SQL_Lang.Name = "SQL_Lang";
            this.SQL_Lang.Size = new System.Drawing.Size(180, 22);
            this.SQL_Lang.Text = "SQL";
            this.SQL_Lang.Click += new System.EventHandler(this.LanguageSelect);
            // 
            // PHP_Lang
            // 
            this.PHP_Lang.Name = "PHP_Lang";
            this.PHP_Lang.Size = new System.Drawing.Size(180, 22);
            this.PHP_Lang.Text = "PHP";
            this.PHP_Lang.Click += new System.EventHandler(this.LanguageSelect);
            // 
            // JS_Lang
            // 
            this.JS_Lang.Name = "JS_Lang";
            this.JS_Lang.Size = new System.Drawing.Size(180, 22);
            this.JS_Lang.Text = "JS";
            this.JS_Lang.Click += new System.EventHandler(this.LanguageSelect);
            // 
            // LUA_Lang
            // 
            this.LUA_Lang.Name = "LUA_Lang";
            this.LUA_Lang.Size = new System.Drawing.Size(180, 22);
            this.LUA_Lang.Text = "Lua";
            this.LUA_Lang.Click += new System.EventHandler(this.LanguageSelect);
            // 
            // Window_Tls
            // 
            this.Window_Tls.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Window_Tls.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TopNo_Tools});
            this.Window_Tls.ForeColor = System.Drawing.Color.White;
            this.Window_Tls.Image = ((System.Drawing.Image)(resources.GetObject("Window_Tls.Image")));
            this.Window_Tls.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Window_Tls.Name = "Window_Tls";
            this.Window_Tls.Size = new System.Drawing.Size(71, 22);
            this.Window_Tls.Text = "Window";
            // 
            // TopNo_Tools
            // 
            this.TopNo_Tools.Name = "TopNo_Tools";
            this.TopNo_Tools.Size = new System.Drawing.Size(98, 22);
            this.TopNo_Tools.Text = "Top";
            this.TopNo_Tools.Click += new System.EventHandler(this.TopNo_Tools_Click);
            // 
            // Config_Tls
            // 
            this.Config_Tls.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Config_Tls.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SetColors_Tools,
            this.Reload_Tools});
            this.Config_Tls.ForeColor = System.Drawing.Color.White;
            this.Config_Tls.Image = ((System.Drawing.Image)(resources.GetObject("Config_Tls.Image")));
            this.Config_Tls.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Config_Tls.Name = "Config_Tls";
            this.Config_Tls.Size = new System.Drawing.Size(61, 22);
            this.Config_Tls.Text = "Config";
            this.Config_Tls.Visible = false;
            // 
            // SetColors_Tools
            // 
            this.SetColors_Tools.Name = "SetColors_Tools";
            this.SetColors_Tools.Size = new System.Drawing.Size(180, 22);
            this.SetColors_Tools.Text = "Setcolors";
            this.SetColors_Tools.Click += new System.EventHandler(this.SetColors_Tools_Click);
            // 
            // Reload_Tools
            // 
            this.Reload_Tools.Name = "Reload_Tools";
            this.Reload_Tools.Size = new System.Drawing.Size(180, 22);
            this.Reload_Tools.Text = "Reload";
            this.Reload_Tools.Click += new System.EventHandler(this.Reload_Tools_Click);
            // 
            // About_Tools
            // 
            this.About_Tools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.About_Tools.ForeColor = System.Drawing.Color.DimGray;
            this.About_Tools.Image = ((System.Drawing.Image)(resources.GetObject("About_Tools.Image")));
            this.About_Tools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.About_Tools.Name = "About_Tools";
            this.About_Tools.Size = new System.Drawing.Size(50, 22);
            this.About_Tools.Text = "About";
            this.About_Tools.Click += new System.EventHandler(this.About_Tools_Click);
            // 
            // Status_Strip
            // 
            this.Status_Strip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.Status_Strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Lang_Sts,
            this.toolStripStatusLabel2,
            this.FileDir_Sts});
            this.Status_Strip.Location = new System.Drawing.Point(0, 428);
            this.Status_Strip.Name = "Status_Strip";
            this.Status_Strip.Size = new System.Drawing.Size(800, 22);
            this.Status_Strip.SizingGrip = false;
            this.Status_Strip.TabIndex = 1;
            // 
            // Lang_Sts
            // 
            this.Lang_Sts.ForeColor = System.Drawing.Color.Silver;
            this.Lang_Sts.Name = "Lang_Sts";
            this.Lang_Sts.Size = new System.Drawing.Size(45, 17);
            this.Lang_Sts.Text = "Default";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.DimGray;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel2.Text = "|";
            // 
            // FileDir_Sts
            // 
            this.FileDir_Sts.ForeColor = System.Drawing.Color.Gray;
            this.FileDir_Sts.Name = "FileDir_Sts";
            this.FileDir_Sts.Size = new System.Drawing.Size(0, 17);
            // 
            // Output_Txt
            // 
            this.Output_Txt.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.Output_Txt.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.Output_Txt.BackBrush = null;
            this.Output_Txt.CharHeight = 14;
            this.Output_Txt.CharWidth = 8;
            this.Output_Txt.ContextMenuStrip = this.Menu_Mnu;
            this.Output_Txt.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Output_Txt.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.Output_Txt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Output_Txt.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.Output_Txt.IsReplaceMode = false;
            this.Output_Txt.Location = new System.Drawing.Point(0, 25);
            this.Output_Txt.Name = "Output_Txt";
            this.Output_Txt.Paddings = new System.Windows.Forms.Padding(0);
            this.Output_Txt.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.Output_Txt.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("Output_Txt.ServiceColors")));
            this.Output_Txt.ServiceLinesColor = System.Drawing.Color.Black;
            this.Output_Txt.Size = new System.Drawing.Size(800, 403);
            this.Output_Txt.TabIndex = 2;
            this.Output_Txt.Zoom = 100;
            // 
            // Menu_Mnu
            // 
            this.Menu_Mnu.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Menu_Mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Cut_Tools,
            this.Copy_Mnu,
            this.Paste_Mnu,
            this.Delete_Mnu,
            this.toolStripSeparator3,
            this.SelectAll_Mnu});
            this.Menu_Mnu.Name = "Menu_Mnu";
            this.Menu_Mnu.Size = new System.Drawing.Size(136, 120);
            // 
            // Cut_Tools
            // 
            this.Cut_Tools.Name = "Cut_Tools";
            this.Cut_Tools.Size = new System.Drawing.Size(135, 22);
            this.Cut_Tools.Text = "Cut";
            this.Cut_Tools.Click += new System.EventHandler(this.Cut_Tools_Click);
            // 
            // Copy_Mnu
            // 
            this.Copy_Mnu.Name = "Copy_Mnu";
            this.Copy_Mnu.Size = new System.Drawing.Size(135, 22);
            this.Copy_Mnu.Text = "Copy";
            this.Copy_Mnu.Click += new System.EventHandler(this.Copy_Mnu_Click);
            // 
            // Paste_Mnu
            // 
            this.Paste_Mnu.Name = "Paste_Mnu";
            this.Paste_Mnu.Size = new System.Drawing.Size(135, 22);
            this.Paste_Mnu.Text = "Paste";
            this.Paste_Mnu.Click += new System.EventHandler(this.Paste_Mnu_Click);
            // 
            // Delete_Mnu
            // 
            this.Delete_Mnu.Name = "Delete_Mnu";
            this.Delete_Mnu.Size = new System.Drawing.Size(135, 22);
            this.Delete_Mnu.Text = "Delete";
            this.Delete_Mnu.Click += new System.EventHandler(this.Delete_Mnu_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(132, 6);
            // 
            // SelectAll_Mnu
            // 
            this.SelectAll_Mnu.Name = "SelectAll_Mnu";
            this.SelectAll_Mnu.Size = new System.Drawing.Size(135, 22);
            this.SelectAll_Mnu.Text = "Select all";
            this.SelectAll_Mnu.Click += new System.EventHandler(this.SelectAll_Mnu_Click);
            // 
            // SetColor_Lbl
            // 
            this.SetColor_Lbl.FormattingEnabled = true;
            this.SetColor_Lbl.Location = new System.Drawing.Point(661, 430);
            this.SetColor_Lbl.Name = "SetColor_Lbl";
            this.SetColor_Lbl.Size = new System.Drawing.Size(136, 17);
            this.SetColor_Lbl.TabIndex = 3;
            this.SetColor_Lbl.Visible = false;
            // 
            // Main_Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SetColor_Lbl);
            this.Controls.Add(this.Output_Txt);
            this.Controls.Add(this.Status_Strip);
            this.Controls.Add(this.Tools_Tls);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main_Frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Main_Frm_Load);
            this.Tools_Tls.ResumeLayout(false);
            this.Tools_Tls.PerformLayout();
            this.Status_Strip.ResumeLayout(false);
            this.Status_Strip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Output_Txt)).EndInit();
            this.Menu_Mnu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip Tools_Tls;
        private System.Windows.Forms.StatusStrip Status_Strip;
        private FastColoredTextBoxNS.FastColoredTextBox Output_Txt;
        private System.Windows.Forms.ToolStripDropDownButton Edit_Tls;
        private System.Windows.Forms.ToolStripDropDownButton Lang_Tls;
        private System.Windows.Forms.ToolStripMenuItem FindReplace;
        private System.Windows.Forms.ToolStripMenuItem Find_Tools;
        private System.Windows.Forms.ToolStripMenuItem Replace_Tools;
        private System.Windows.Forms.ToolStripMenuItem CSharp_Lang;
        private System.Windows.Forms.ToolStripMenuItem VB_Lang;
        private System.Windows.Forms.ToolStripMenuItem HTML_Lang;
        private System.Windows.Forms.ToolStripMenuItem XML_Lang;
        private System.Windows.Forms.ToolStripMenuItem SQL_Lang;
        private System.Windows.Forms.ToolStripMenuItem PHP_Lang;
        private System.Windows.Forms.ToolStripMenuItem JS_Lang;
        private System.Windows.Forms.ToolStripMenuItem LUA_Lang;
        private System.Windows.Forms.ToolStripMenuItem Custom_Lang;
        private System.Windows.Forms.ToolStripDropDownButton File_Tls;
        private System.Windows.Forms.ToolStripMenuItem Save_Tools;
        private System.Windows.Forms.ToolStripMenuItem SaveAs_Tools;
        private System.Windows.Forms.ToolStripMenuItem OpenFile_Tools;
        private System.Windows.Forms.ToolStripStatusLabel FileDir_Sts;
        private System.Windows.Forms.ToolStripMenuItem CloseFile_Tools;
        private System.Windows.Forms.ToolStripMenuItem Regiontls;
        private System.Windows.Forms.ToolStripMenuItem Collapse_Tools;
        private System.Windows.Forms.ToolStripMenuItem Expand_Tools;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem Exit_Tools;
        private System.Windows.Forms.ToolStripDropDownButton Window_Tls;
        private System.Windows.Forms.ToolStripMenuItem TopNo_Tools;
        private System.Windows.Forms.ListBox SetColor_Lbl;
        private System.Windows.Forms.ToolStripDropDownButton Config_Tls;
        private System.Windows.Forms.ToolStripMenuItem SetColors_Tools;
        private System.Windows.Forms.ToolStripMenuItem Reload_Tools;
        private System.Windows.Forms.ToolStripButton About_Tools;
        private System.Windows.Forms.ContextMenuStrip Menu_Mnu;
        private System.Windows.Forms.ToolStripMenuItem Paste_Mnu;
        private System.Windows.Forms.ToolStripMenuItem Copy_Mnu;
        private System.Windows.Forms.ToolStripMenuItem Cut_Tools;
        private System.Windows.Forms.ToolStripMenuItem NewFile_Tools;
        private System.Windows.Forms.ToolStripStatusLabel Lang_Sts;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripMenuItem Delete_Mnu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem SelectAll_Mnu;
        private System.Windows.Forms.ToolStripMenuItem NewWindow_Tools;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

