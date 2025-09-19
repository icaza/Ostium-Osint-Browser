using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.Xml;
using System.Diagnostics;
using Errordll;
using System.Threading;

namespace setirps
{
    public partial class Main_Frm : Form
    {
        #region Var_

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern bool ReleaseCapture();

        readonly Errorsend senderror = new Errorsend();

        readonly string AppStart = Application.StartupPath + @"\";
        readonly string ProjectDir = Application.StartupPath + @"\setirps\";
        readonly string SpritesDir = Application.StartupPath + @"\setirps\sprites\";
        readonly string DiagramDir = Application.StartupPath + @"\diagram\";

        string valueABtmp = "";
        ListBox List_Object;
        ListBox ListSave_Object;
        ListBox ListMirorSave_Object;

        string SpriteSprite = "";
        string SpriteSelect = "";
        string ProjectOpn = "";
        string ProjectOutput = "";
        string OpnProject = "";

        readonly string MessageStartDiagram = "When this window closes, the diagram creation process begins, be patient the time depends on the file size " +
    "and structure. In case of blockage! use Debug in the menu to kill the javaw process. Feel free to join the Discord channel for help.";
        string FileDiag = "";

        #endregion

        #region Form_

        public Main_Frm()
        {
            InitializeComponent();

            SizeChanged += new EventHandler(Main_Frm_SizeChange);
            ListPic_Lstw.Click += new EventHandler(GetItemsPic);
            MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            Tools_Tls.MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            Site_Lst.Click += new EventHandler(Lst_Click);
            User_Lst.Click += new EventHandler(Lst_Click);
            Hardware_Lst.Click += new EventHandler(Lst_Click);
            Network_Lst.Click += new EventHandler(Lst_Click);
            Server_Lst.Click += new EventHandler(Lst_Click);
            Out_Txt.TextChanged += new EventHandler(Out_Txt_TextChanged);
            Out_Txt.DoubleClick += new EventHandler(Out_Txt_DoubleClicked);
        }

        void Main_Frm_Load(object sender, EventArgs e)
        {
            LoadPicLstw(SpritesDir + "Site");
        }

        void Main_Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        void Main_Frm_SizeChange(object sender, EventArgs e)
        {
            HideScroll_Pnl.Height = Height;
        }

        #endregion

        #region Button_Sprite

        void SitePic_Btn_Click(object sender, EventArgs e)
        {
            ColorButton();
            SitePic_Btn.ForeColor = Color.White;
            SpriteSprite = "Site";
            LoadPicLstw(SpritesDir + "Site");
        }

        void UserPic_Btn_Click(object sender, EventArgs e)
        {
            ColorButton();
            UserPic_Btn.ForeColor = Color.White;
            SpriteSprite = "User";
            LoadPicLstw(SpritesDir + "User");
        }

        void HardwarePic_Btn_Click(object sender, EventArgs e)
        {
            ColorButton();
            HardwarePic_Btn.ForeColor = Color.White;
            SpriteSprite = "Hardware";
            LoadPicLstw(SpritesDir + "Hardware");
        }

        void MiscPic_Btn_Click(object sender, EventArgs e)
        {
            ColorButton();
            MiscPic_Btn.ForeColor = Color.White;
            SpriteSprite = "Network";
            LoadPicLstw(SpritesDir + "Misc");
        }

        void ServerPic_Btn_Click(object sender, EventArgs e)
        {
            ColorButton();
            ServerPic_Btn.ForeColor = Color.White;
            SpriteSprite = "Server";
            LoadPicLstw(SpritesDir + "Server");
        }

        void ColorButton()
        {
            SitePic_Btn.ForeColor = Color.DodgerBlue;
            UserPic_Btn.ForeColor = Color.DodgerBlue;
            HardwarePic_Btn.ForeColor = Color.DodgerBlue;
            MiscPic_Btn.ForeColor = Color.DodgerBlue;
            ServerPic_Btn.ForeColor = Color.DodgerBlue;
        }

        #endregion

        void LoadPicLstw(string DirPic)
        {
            try
            {
                if (!Directory.Exists(DirPic))
                    return;

                ListPic_Lstw.Items.Clear();
                ListPicDir.Images.Clear();
                ListDirPic_Lst.Items.Clear();

                DirectoryInfo dir = new DirectoryInfo(DirPic);
                foreach (FileInfo file in dir.GetFiles())
                {
                    try
                    {
                        ListPicDir.Images.Add(Image.FromFile(file.FullName));
                        string strName = Path.GetFileNameWithoutExtension(file.Name);
                        ListDirPic_Lst.Items.Add(strName);
                    }
                    catch
                    { }
                }

                ListPic_Lstw.View = View.LargeIcon;
                ListPicDir.ImageSize = new Size(64, 64);
                ListPic_Lstw.LargeImageList = ListPicDir;

                for (int pic = 0; pic < ListPicDir.Images.Count; pic++)
                {
                    ListPic_Lstw.Items.Add(ListDirPic_Lst.Items[pic].ToString(), pic);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadPicLstw: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void GetItemsPic(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (object item in ListPic_Lstw.SelectedIndices)
                {
                    int idexpic = Convert.ToInt16(item);
                    sb.Append(ListDirPic_Lst.Items[idexpic].ToString());
                }
                SpriteSelect = sb.ToString();
                CreateStringValue(sb.ToString());
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! GetItemsPic: ", ex.Message, "Main_Frm", Application.StartupPath + @"\");
            }
        }

        void CreateStringValue(string Sprites)
        {
            try
            {
                string[] pairs = { Sprites };
                foreach (var pair in pairs)
                {
                    int position = pair.LastIndexOf("_");
                    if (position < 0)
                        continue;

                    string v2 = pair.Substring(position + 1);
                    EditSprite(v2);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateStringValue: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void EditSprite(string value)
        {
            switch (SpriteSprite)
            {
                case "Site":
                    Site_Sprite_Select.Text = SpriteSelect;
                    Site_Sprite_Var.Text = value;
                    break;
                case "User":
                    User_Sprite_Select.Text = SpriteSelect;
                    break;
                case "Hardware":
                    Hardware_Sprite_Select.Text = SpriteSelect;
                    Hardware_Value_1.Text = value;
                    break;
                case "Network":
                    Network_Sprite_Select.Text = SpriteSelect;
                    Network_Value_1.Text = value;
                    break;
                case "Server":
                    Server_Sprite_Select.Text = SpriteSelect;
                    Server_Sprite_Var.Text = value;
                    break;
            }
        }

        #region Button_Menu

        void OpnXML_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(AppStart + "OstiumE.exe"))
                {
                    MessageBox.Show("The OstiumE editor is missing!", "Missing editor");
                    return;
                }

                if (File.Exists(ProjectOpn))
                {
                    using (Process objProcess = new Process())
                    {
                        objProcess.StartInfo.FileName = AppStart + "OstiumE.exe";
                        objProcess.StartInfo.Arguments = "/input=\"" + ProjectOpn + "\"";
                        objProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        objProcess.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnXML_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void AppStartup_Btn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(ProjectDir))
                Process.Start(ProjectDir);
        }

        void Close_Btn_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Size_

        void Maximize_Btn_Click(object sender, EventArgs e)
        {
            Normal_Btn.Visible = true;
            Maximize_Btn.Visible = false;
            WindowState = FormWindowState.Maximized;

            ResizeOutput();
        }

        void Minimize_Btn_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        void Normal_Btn_Click(object sender, EventArgs e)
        {
            Normal_Btn.Visible = false;
            Maximize_Btn.Visible = true;
            WindowState = FormWindowState.Normal;

            ResizeOutput();
        }

        void ResizeOutput()
        {
            int s1 = tableLayoutPanel1.Width;
            int s2 = s1 * 40 / 100;
            Out_Txt.Width = s2;
        }

        #endregion

        void Lst_Click(object sender, EventArgs e)
        {
            int A = (sender as ListBox).SelectedIndex;

            if ((sender as ListBox).SelectedIndex != -1)
            {
                string B = (sender as ListBox).Name;

                try
                {
                    switch (B)
                    {
                        case "Site_Lst":
                            {
                                valueABtmp = SiteMiror_Lst.Items[A].ToString();
                                List_Object = Site_Lst;
                                break;
                            }
                        case "User_Lst":
                            {
                                valueABtmp = UserMiror_Lst.Items[A].ToString();
                                List_Object = User_Lst;
                                break;
                            }
                        case "Hardware_Lst":
                            {
                                valueABtmp = HardwareMiror_Lst.Items[A].ToString();
                                List_Object = Hardware_Lst;
                                break;
                            }
                        case "Network_Lst":
                            {
                                valueABtmp = NetworkMiror_Lst.Items[A].ToString();
                                List_Object = Network_Lst;
                                break;
                            }
                        case "Server_Lst":
                            {
                                valueABtmp = ServerMiror_Lst.Items[A].ToString();
                                List_Object = Server_Lst;
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    senderror.ErrorLog("Error! Lst_Click: ", ex.Message, "Main_Frm", AppStart);
                }
            }
        }

        #region Button_Diagram

        void FirstStep_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string entete = @"@startuml
!define osaPuml stdliblocal
!include osaPuml/Common.puml
!include osaPuml/User/all.puml
!include osaPuml/Hardware/all.puml
!include osaPuml/Misc/all.puml
!include osaPuml/Server/all.puml
!include osaPuml/Site/all.puml" + "\r\n";

                Out_Txt.AppendText(entete + "\r\n");
                foreach (string itm in Site_Lst.Items)
                    Out_Txt.AppendText(itm + "\r\n");
                Out_Txt.AppendText("\r\n");
                foreach (string itm in User_Lst.Items)
                    Out_Txt.AppendText(itm + "\r\n");
                Out_Txt.AppendText("\r\n");
                foreach (string itm in Hardware_Lst.Items)
                    Out_Txt.AppendText(itm + "\r\n");
                Out_Txt.AppendText("\r\n");
                foreach (string itm in Network_Lst.Items)
                    Out_Txt.AppendText(itm + "\r\n");
                Out_Txt.AppendText("\r\n");
                foreach (string itm in Server_Lst.Items)
                    Out_Txt.AppendText(itm + "\r\n");
                Out_Txt.AppendText("\r");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! FirstStep_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void AddValueA_Txt_Click(object sender, EventArgs e)
        {
            try
            {
                ValueA_Txt.Text = valueABtmp;

                if (Convert.ToString(List_Object) != "")
                {
                    if (List_Object.SelectedIndex != -1)
                    {
                        List_Object.ClearSelected();
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddValueA_Txt_Click: ", ex.Message, "Main_Frm", AppStart);
            }           
        }

        void AddValueB_Txt_Click(object sender, EventArgs e)
        {
            try
            {
                ValueB_Txt.Text = valueABtmp;

                if (Convert.ToString(List_Object) != "")
                {
                    if (List_Object.SelectedIndex != -1)
                    {
                        List_Object.ClearSelected();
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddValueB_Txt_Click: ", ex.Message, "Main_Frm", AppStart);
            }                
        }

        void InsertValueAB_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValueA_Txt.Text == "" || ValueB_Txt.Text == "")
                {
                    ValueA_Txt.BackColor = Color.Red;
                    ValueB_Txt.BackColor = Color.Red;
                    MessageBox.Show("Insert all value!");
                    ValueA_Txt.BackColor = Color.White;
                    ValueB_Txt.BackColor = Color.White;
                    return;
                }

                if (CursorPosition_Chk.Checked)
                {
                    Out_Txt.Text = Out_Txt.Text.Insert(Out_Txt.SelectionStart, ValueA_Txt.Text + " --> " + ValueB_Txt.Text);
                }
                else
                {
                    Out_Txt.AppendText(ValueA_Txt.Text + " --> " + ValueB_Txt.Text + "\r\n");
                }

                ValueA_Txt.Text = "";
                ValueB_Txt.Text = "";
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! InsertValueAB_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void InsertValue_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ListSelectTxt_Txt.Text == "")
                {
                    ListSelectTxt_Txt.BackColor = Color.Red;
                    MessageBox.Show("Insert all value!");
                    ListSelectTxt_Txt.BackColor = Color.White;
                    return;
                }

                if (CursorPosition_Chk.Checked)
                {
                    Out_Txt.Text = Out_Txt.Text.Insert(Out_Txt.SelectionStart, ListSelectTxt_Txt.Text);
                }
                else
                {
                    Out_Txt.AppendText(ListSelectTxt_Txt.Text + "\r\n");
                }

                ListSelectTxt_Txt.Text = "";
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! InsertValueAB_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void Finalise_Btn_Click(object sender, EventArgs e)
        {
            string finalise = @"footer %filename() rendered with PlantUML version %version()\nWith Ostium Osint Browser
@enduml";

            Out_Txt.AppendText(finalise);
        }

        void RemoveItem_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToString(List_Object) != "")
                {
                    if (List_Object.SelectedIndex != -1)
                    {
                        string message = "Do you want to delete the item?";
                        string caption = "Delete Item";
                        var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            string B = List_Object.Name;

                            switch (B)
                            {
                                case "Site_Lst":
                                    {
                                        SiteMiror_Lst.Items.Remove(SiteMiror_Lst.Items[List_Object.SelectedIndex]);
                                        break;
                                    }
                                case "User_Lst":
                                    {
                                        UserMiror_Lst.Items.Remove(UserMiror_Lst.Items[List_Object.SelectedIndex]);
                                        break;
                                    }
                                case "Hardware_Lst":
                                    {
                                        HardwareMiror_Lst.Items.Remove(HardwareMiror_Lst.Items[List_Object.SelectedIndex]);
                                        break;
                                    }
                                case "Network_Lst":
                                    {
                                        NetworkMiror_Lst.Items.Remove(NetworkMiror_Lst.Items[List_Object.SelectedIndex]);
                                        break;
                                    }
                                case "Server_Lst":
                                    {
                                        ServerMiror_Lst.Items.Remove(ServerMiror_Lst.Items[List_Object.SelectedIndex]);
                                        break;
                                    }
                            }

                            List_Object.Items.Remove(List_Object.SelectedItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! RemoveItem_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #endregion

        void OpnEditor_Btn_Click(object sender, EventArgs e)
        {
            if (Out_Txt.Visible)
            {
                OpnEditor_Btn.Text = "Open Editor";
                Out_Txt.Visible = false;
            }
            else
            {
                OpnEditor_Btn.Text = "Close Editor";
                ResizeOutput();
                Out_Txt.Visible = true;
            }
        }

        #region Add_Value

        void Add_Site_Val_Btn_Click(object sender, EventArgs e)
        {
            string i0 = Site_Sprite_Select.Text;
            string i1 = Site_Sprite_Var.Text;
            string v1 = Site_Value_1.Text;
            string v2 = Site_Value_2.Text;
            string v3 = Site_Value_3.Text;

            string valAdd = i0 + "(" + i1 + ", \"" + v1 + "\", \"" + v2 + "\", \"" + v3 + "\")";

            Site_Lst.Items.Add(valAdd);
            SiteMiror_Lst.Items.Add(i1);

            SaveProject_Btn.ForeColor = Color.Red;
        }

        void Add_User_Val_Btn_Click(object sender, EventArgs e)
        {
            string i0 = User_Sprite_Select.Text;
            string v1 = User_Value_1.Text;
            string v2 = User_Value_2.Text;
            string v3 = User_Value_3.Text;

            string valAdd = i0 + "(" + v1 + ", \"" + v1 + "\", \"" + v2 + "\", \"" + v3 + "\")";

            User_Lst.Items.Add(valAdd);
            UserMiror_Lst.Items.Add(v1);

            SaveProject_Btn.ForeColor = Color.Red;
        }

        void Add_Hardware_Val_Btn_Click(object sender, EventArgs e)
        {
            string i0 = Hardware_Sprite_Select.Text;
            string v1 = Hardware_Value_1.Text;
            string v2 = Hardware_Value_2.Text;
            string v3 = Hardware_Value_3.Text;
            string v4 = Hardware_Value_4.Text;

            string valAdd = i0 + "(" + v1 + ", \"" + v2 + "\", \"" + v3 + "\", \"" + v4 + "\")";

            Hardware_Lst.Items.Add(valAdd);
            HardwareMiror_Lst.Items.Add(v1);

            SaveProject_Btn.ForeColor = Color.Red;
        }

        void Add_Network_Val_Btn_Click(object sender, EventArgs e)
        {
            string i0 = Network_Sprite_Select.Text;
            string v1 = Network_Value_1.Text;
            string v2 = Network_Value_2.Text;
            string v3 = Network_Value_3.Text;

            string valAdd = i0 + "(" + v1 + ", \"" + v2 + "\", \"" + v3 + "\")";

            Network_Lst.Items.Add(valAdd);
            NetworkMiror_Lst.Items.Add(v1);

            SaveProject_Btn.ForeColor = Color.Red;
        }

        void Add_Server_Val_Btn_Click(object sender, EventArgs e)
        {
            string i0 = Server_Sprite_Select.Text;
            string i1 = Server_Sprite_Var.Text;
            string v1 = Server_Value_1.Text;
            string v2 = Server_Value_2.Text;
            string v3 = Server_Value_3.Text;

            string valAdd = i0 + "(" + i1 + ", \"" + v1 + "\", \"" + v2 + "\", \"" + v3 + "\")";

            Server_Lst.Items.Add(valAdd);
            ServerMiror_Lst.Items.Add(i1);

            SaveProject_Btn.ForeColor = Color.Red;
        }

        #endregion

        #region Button_Project

        void NewProject_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string message, title;
                object NameProject;

                message = "Enter a project Name.";
                title = "New Project";

                NameProject = Interaction.InputBox(message, title);
                string FileName = Convert.ToString(NameProject);

                if (FileName == "")
                    return;

                if (File.Exists(ProjectDir + FileName + ".xml"))
                {
                    string averts = "The file exists! Do you want to overwrite it?";
                    string caption = "File exist";
                    var result = MessageBox.Show(averts, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        VideList();
                        CreateXMLProject(ProjectDir + FileName + ".xml");
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    VideList();
                    CreateXMLProject(ProjectDir + FileName + ".xml");
                }

                ProjectOutput = ProjectDir + FileName + ".txt";
                FileDiag = ProjectDir + FileName + ".svg";

                using (StreamWriter file_create = new StreamWriter(ProjectOutput))
                {
                    file_create.Write(Out_Txt.Text);
                }

                ProjectOpn = ProjectDir + FileName + ".xml";
                ProjectOpn_Lbl.Text = "Project open: " + FileName + ".xml";

                SaveProject_Btn.ForeColor = Color.Gold;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! NewProject_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void OpnProject_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = ProjectDir;
                    openFileDialog.Filter = "xml files (*.xml)|*.xml";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        VideList();
                        OpnProject = "on";
                        ProjectOpn = openFileDialog.FileName;
                        string strOutput = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        ProjectOutput = ProjectDir + strOutput + ".txt";
                        FileDiag = ProjectDir + strOutput + ".svg";
                        string strName = Path.GetFileName(openFileDialog.FileName);
                        ProjectOpn_Lbl.Text = "Project open: " + strName;
                    }
                    else
                        return;
                }

                ListSave_Object = Site_Lst;
                ListMirorSave_Object = SiteMiror_Lst;
                OpenXMLProject("Site/data");
                ListSave_Object = User_Lst;
                ListMirorSave_Object = UserMiror_Lst;
                OpenXMLProject("User/data");
                ListSave_Object = Hardware_Lst;
                ListMirorSave_Object = HardwareMiror_Lst;
                OpenXMLProject("Hardware/data");
                ListSave_Object = Network_Lst;
                ListMirorSave_Object = NetworkMiror_Lst;
                OpenXMLProject("Network/data");
                ListSave_Object = Server_Lst;
                ListMirorSave_Object = ServerMiror_Lst;
                OpenXMLProject("Server/data");

                if (File.Exists(ProjectOutput))
                {
                    using (StreamReader sr = new StreamReader(ProjectOutput))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            Out_Txt.AppendText(line + "\r\n");
                        }
                    }
                }

                OpnProject = "off";
                SaveProject_Btn.ForeColor = Color.Gold;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnProject_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void SaveProject_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(ProjectOpn))
                    File.Delete(ProjectOpn);

                CreateXMLProject(ProjectOpn);

                ListSave_Object = Site_Lst;
                ListMirorSave_Object = SiteMiror_Lst;
                SaveXMLProject("Site", "data");
                ListSave_Object = User_Lst;
                ListMirorSave_Object = UserMiror_Lst;
                SaveXMLProject("User", "data");
                ListSave_Object = Hardware_Lst;
                ListMirorSave_Object = HardwareMiror_Lst;
                SaveXMLProject("Hardware", "data");
                ListSave_Object = Network_Lst;
                ListMirorSave_Object = NetworkMiror_Lst;
                SaveXMLProject("Network", "data");
                ListSave_Object = Server_Lst;
                ListMirorSave_Object = ServerMiror_Lst;
                SaveXMLProject("Server", "data");

                using (StreamWriter file_create = new StreamWriter(ProjectOutput))
                {
                    file_create.Write(Out_Txt.Text);
                }

                SaveProject_Btn.ForeColor = Color.Gold;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! SaveProject_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void VideList()
        {
            Site_Lst.Items.Clear();
            User_Lst.Items.Clear();
            Hardware_Lst.Items.Clear();
            Network_Lst.Items.Clear();
            Server_Lst.Items.Clear();

            SiteMiror_Lst.Items.Clear();
            UserMiror_Lst.Items.Clear();
            HardwareMiror_Lst.Items.Clear();
            NetworkMiror_Lst.Items.Clear();
            ServerMiror_Lst.Items.Clear();

            Out_Txt.Text = "";
        }

        #endregion

        #region XML_

        void CreateXMLProject(string filename)
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;

                writer.WriteStartElement("Table");

                writer.WriteStartElement("Site");
                writer.WriteEndElement();
                writer.WriteStartElement("User");
                writer.WriteEndElement();
                writer.WriteStartElement("Hardware");
                writer.WriteEndElement();
                writer.WriteStartElement("Network");
                writer.WriteEndElement();
                writer.WriteStartElement("Server");
                writer.WriteEndElement();

                writer.WriteEndDocument();
                writer.Close();

                MessageBox.Show("Project save! ");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateXMLProject: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void SaveXMLProject(string nodeselect, string elementadd)
        {
            try
            {
                if (ListSave_Object.Items.Count == 0)
                    return;

                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(ProjectOpn);
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Table/" + nodeselect) is XmlElement node1)
                {
                    for (int i = 0; i < ListSave_Object.Items.Count; i++)
                    {
                        XmlElement elem = doc.CreateElement(elementadd);
                        elem.SetAttribute("Value", ListMirorSave_Object.Items[i].ToString());
                        elem.InnerText = ListSave_Object.Items[i].ToString();
                        node1.AppendChild(elem);
                    }
                }

                xmlReader.Close();
                doc.Save(ProjectOpn);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! SaveXMLProject: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void OpenXMLProject(string nodedata)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(ProjectOpn);
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Table/" + nodedata);

                for (int i = 0; i < nodeList.Count; i++)
                {
                    string str = string.Format("{0}", nodeList[i].ChildNodes.Item(0).InnerText);
                    string strmiror = string.Format("{0}", nodeList[i].Attributes.Item(0).InnerText);
                    ListSave_Object.Items.Add(str);
                    ListMirorSave_Object.Items.Add(strmiror);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenXMLProject: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #endregion

        void CreateDiagram_Btn_Click(object sender, EventArgs e)
        {
            if (!File.Exists(DiagramDir + "plantuml.jar"))
            {
                MessageBox.Show("plantuml.jar is missing!", "Missing plantUML");
                return;
            }

            MessageBox.Show(MessageStartDiagram);
            Timo.Enabled = true;

            CreateDiagram_Thrd(ProjectOutput);
        }

        void CreateDiagram_Thrd(string fileselect)
        {
            try
            {
                string limitsize = "";

                if (Limitsize_Chk.Checked)
                    limitsize = "-DPLANTUML_LIMIT_SIZE=8192";

                string argumentsIs = "java " + limitsize + " -jar plantuml.jar " + fileselect + " -tsvg " + CharsetPlant_Txt.Text;

                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = DiagramDir + "plantuml.jar";
                    proc.StartInfo.Arguments = argumentsIs;
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                    proc.Close();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateDiagram_Thrd: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void Out_Txt_TextChanged(object sender, EventArgs e)
        {
            if (OpnProject == "off")
                SaveProject_Btn.ForeColor = Color.Red;
        }

        void Out_Txt_DoubleClicked(object sender, EventArgs e)
        {
            if (Out_Txt.ScrollBars == ScrollBars.None)
                Out_Txt.ScrollBars = ScrollBars.Vertical;
            else
                Out_Txt.ScrollBars = ScrollBars.None;
        }

        void Timo_Tick(object sender, EventArgs e)
        {
            Process[] localByName = Process.GetProcessesByName("javaw");
            if (localByName.Length > 0)
            { }  // Process True
            else
            {
                Timo.Enabled = false;
                Thread.Sleep(1000);
                CreateDiagram_Invk(FileDiag); // Process False
            }
        }

        void CreateDiagram_Invk(string value)
        {
            Process.Start(value);
        }

        void Site_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Site_Lst.SelectedIndex != -1)
            {
                ListSelectTxt_Txt.Text = Site_Lst.SelectedItem.ToString(); ;
            }
        }

        void User_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (User_Lst.SelectedIndex != -1)
            {
                ListSelectTxt_Txt.Text = User_Lst.SelectedItem.ToString(); ;
            }
        }

        void Hardware_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Hardware_Lst.SelectedIndex != -1)
            {
                ListSelectTxt_Txt.Text = Hardware_Lst.SelectedItem.ToString(); ;
            }
        }

        void Network_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Network_Lst.SelectedIndex != -1)
            {
                ListSelectTxt_Txt.Text = Network_Lst.SelectedItem.ToString(); ;
            }
        }
            
        void Server_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Server_Lst.SelectedIndex != -1)
            {
                ListSelectTxt_Txt.Text = Server_Lst.SelectedItem.ToString(); ;
            }
        }
    }
}
