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

namespace setirps
{
    public partial class Main_Frm : Form
    {
        #region Var_

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        readonly Errorsend senderror = new Errorsend();

        readonly string AppStart = Application.StartupPath + @"\";
        readonly string ProjectDir = Application.StartupPath + @"\setirps\";
        readonly string SpritesDir = Application.StartupPath + @"\setirps\sprites\";
        readonly string DiagramDir = Application.StartupPath + @"\diagram\";

        string valueABtmp = "";
        public ListBox List_Object;
        public ListBox ListSave_Object;

        public string SpriteSprite = "";
        public string SpriteSelect = "";
        public string ProjectOpn = "";
        public string ProjectOutput = "";
        public string OpnProject = "";

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

        private void Main_Frm_Load(object sender, EventArgs e)
        {
            LoadPicLstw(SpritesDir + "Site");
        }

        private void Main_Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public void Main_Frm_SizeChange(object sender, EventArgs e)
        {
            HideScroll_Pnl.Height = Height;
        }

        #endregion

        #region Button_Sprite

        private void SitePic_Btn_Click(object sender, EventArgs e)
        {
            ColorButton();
            SitePic_Btn.ForeColor = Color.White;
            SpriteSprite = "Site";
            LoadPicLstw(SpritesDir + "Site");
        }

        private void UserPic_Btn_Click(object sender, EventArgs e)
        {
            ColorButton();
            UserPic_Btn.ForeColor = Color.White;
            SpriteSprite = "User";
            LoadPicLstw(SpritesDir + "User");
        }

        private void HardwarePic_Btn_Click(object sender, EventArgs e)
        {
            ColorButton();
            HardwarePic_Btn.ForeColor = Color.White;
            SpriteSprite = "Hardware";
            LoadPicLstw(SpritesDir + "Hardware");
        }

        private void MiscPic_Btn_Click(object sender, EventArgs e)
        {
            ColorButton();
            MiscPic_Btn.ForeColor = Color.White;
            SpriteSprite = "Network";
            LoadPicLstw(SpritesDir + "Misc");
        }

        private void ServerPic_Btn_Click(object sender, EventArgs e)
        {
            ColorButton();
            ServerPic_Btn.ForeColor = Color.White;
            SpriteSprite = "Server";
            LoadPicLstw(SpritesDir + "Server");
        }

        private void ColorButton()
        {
            SitePic_Btn.ForeColor = Color.DodgerBlue;
            UserPic_Btn.ForeColor = Color.DodgerBlue;
            HardwarePic_Btn.ForeColor = Color.DodgerBlue;
            MiscPic_Btn.ForeColor = Color.DodgerBlue;
            ServerPic_Btn.ForeColor = Color.DodgerBlue;
        }

        #endregion

        private void LoadPicLstw(string DirPic)
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

        private void GetItemsPic(object sender, EventArgs e)
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

        private void CreateStringValue(string Sprites)
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

        private void EditSprite(string value)
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

        private void OpnXML_Btn_Click(object sender, EventArgs e)
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

        private void AppStartup_Btn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(ProjectDir))
                Process.Start(ProjectDir);
        }

        private void Close_Btn_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Size_

        private void Maximize_Btn_Click(object sender, EventArgs e)
        {
            Normal_Btn.Visible = true;
            Maximize_Btn.Visible = false;
            WindowState = FormWindowState.Maximized;

            ResizeOutput();
        }

        private void Minimize_Btn_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Normal_Btn_Click(object sender, EventArgs e)
        {
            Normal_Btn.Visible = false;
            Maximize_Btn.Visible = true;
            WindowState = FormWindowState.Normal;

            ResizeOutput();
        }

        private void ResizeOutput()
        {
            int s1 = tableLayoutPanel1.Width;
            int s2 = s1 * 40 / 100;
            Out_Txt.Width = s2;
        }

        #endregion

        private void Lst_Click(object sender, EventArgs e)
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

        private void FirstStep_Btn_Click(object sender, EventArgs e)
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

        private void AddValueA_Txt_Click(object sender, EventArgs e)
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

        private void AddValueB_Txt_Click(object sender, EventArgs e)
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

        private void InsertValueAB_Btn_Click(object sender, EventArgs e)
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

        private void Finalise_Btn_Click(object sender, EventArgs e)
        {
            string finalise = @"footer %filename() rendered with PlantUML version %version()\nWith Ostium Osint Browser
@enduml";

            Out_Txt.AppendText(finalise);
        }

        private void RemoveItem_Btn_Click(object sender, EventArgs e)
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

        private void OpnEditor_Btn_Click(object sender, EventArgs e)
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

        private void Add_Site_Val_Btn_Click(object sender, EventArgs e)
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

        private void Add_User_Val_Btn_Click(object sender, EventArgs e)
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

        private void Add_Hardware_Val_Btn_Click(object sender, EventArgs e)
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

        private void Add_Network_Val_Btn_Click(object sender, EventArgs e)
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

        private void Add_Server_Val_Btn_Click(object sender, EventArgs e)
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

        private void NewProject_Btn_Click(object sender, EventArgs e)
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

        private void OpnProject_Btn_Click(object sender, EventArgs e)
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
                        string strName = Path.GetFileName(openFileDialog.FileName);
                        ProjectOpn_Lbl.Text = "Project open: " + strName;
                    }
                    else
                        return;
                }

                ListSave_Object = Site_Lst;
                OpenXMLProject("Site/data");
                ListSave_Object = User_Lst;
                OpenXMLProject("User/data");
                ListSave_Object = Hardware_Lst;
                OpenXMLProject("Hardware/data");
                ListSave_Object = Network_Lst;
                OpenXMLProject("Network/data");
                ListSave_Object = Server_Lst;
                OpenXMLProject("Server/data");

                ListSave_Object = SiteMiror_Lst;
                OpenXMLProject("SiteMiror/data");
                ListSave_Object = UserMiror_Lst;
                OpenXMLProject("UserMiror/data");
                ListSave_Object = HardwareMiror_Lst;
                OpenXMLProject("HardwareMiror/data");
                ListSave_Object = NetworkMiror_Lst;
                OpenXMLProject("NetworkMiror/data");
                ListSave_Object = ServerMiror_Lst;
                OpenXMLProject("ServerMiror/data");

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

        private void SaveProject_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(ProjectOpn))
                    File.Delete(ProjectOpn);

                CreateXMLProject(ProjectOpn);

                ListSave_Object = Site_Lst;
                SaveXMLProject("Site", "data");
                ListSave_Object = User_Lst;
                SaveXMLProject("User", "data");
                ListSave_Object = Hardware_Lst;
                SaveXMLProject("Hardware", "data");
                ListSave_Object = Network_Lst;
                SaveXMLProject("Network", "data");
                ListSave_Object = Server_Lst;
                SaveXMLProject("Server", "data");

                ListSave_Object = SiteMiror_Lst;
                SaveXMLProject("SiteMiror", "data");
                ListSave_Object = UserMiror_Lst;
                SaveXMLProject("UserMiror", "data");
                ListSave_Object = HardwareMiror_Lst;
                SaveXMLProject("HardwareMiror", "data");
                ListSave_Object = NetworkMiror_Lst;
                SaveXMLProject("NetworkMiror", "data");
                ListSave_Object = ServerMiror_Lst;
                SaveXMLProject("ServerMiror", "data");

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

        private void VideList()
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

        private void CreateXMLProject(string filename)
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
                writer.WriteStartElement("SiteMiror");
                writer.WriteEndElement();
                writer.WriteStartElement("UserMiror");
                writer.WriteEndElement();
                writer.WriteStartElement("HardwareMiror");
                writer.WriteEndElement();
                writer.WriteStartElement("NetworkMiror");
                writer.WriteEndElement();
                writer.WriteStartElement("ServerMiror");
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

        private void SaveXMLProject(string nodeselect, string elementadd)
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

        private void OpenXMLProject(string nodedata)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(ProjectOpn);
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Table/" + nodedata);

                for (int i = 0; i < nodeList.Count; i++)
                {
                    string str = string.Format("{0}", nodeList[i].ChildNodes.Item(0).InnerText);
                    ListSave_Object.Items.Add(str);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenXMLProject: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #endregion

        private void CreateDiagram_Btn_Click(object sender, EventArgs e)
        {
            if (!File.Exists(DiagramDir + "plantuml.jar"))
            {
                MessageBox.Show("plantuml.jar is missing!", "Missing plantUML");
                return;
            }

            CreateDiagram_Thrd(ProjectOutput);
        }

        private void CreateDiagram_Thrd(string fileselect)
        {
            try
            {
                string argumentsIs = "java -jar plantuml.jar " + fileselect + " -tsvg -charset UTF-8";

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

        private void Out_Txt_TextChanged(object sender, EventArgs e)
        {
            if (OpnProject == "off")
            {
                SaveProject_Btn.ForeColor = Color.Red;
            }
        }

        private void Out_Txt_DoubleClicked(object sender, EventArgs e)
        {
            if (Out_Txt.ScrollBars == ScrollBars.None)
            {
                Out_Txt.ScrollBars = ScrollBars.Vertical;
            }
            else
            {
                Out_Txt.ScrollBars = ScrollBars.None;
            }            
        }
    }
}
