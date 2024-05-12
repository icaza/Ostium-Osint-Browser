using System;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using LoadDirectory;
using System.IO;
using System.Text.RegularExpressions;
using Icaza;
using Microsoft.Ajax.Utilities;
using System.Runtime.InteropServices;

namespace Ostium
{
    public partial class Bookmarklets_Frm : Form
    {
        #region Var_

        [DllImport("kernel32.dll")]
        static extern bool Beep(int freq, int duration);
        
        readonly string AppStart = Application.StartupPath + @"\";
        readonly string Scripts = Application.StartupPath + @"\scripts\bookmarklet\";

        readonly Loaddir loadfiledir = new Loaddir();
        readonly IcazaClass openfile = new IcazaClass();
        readonly IcazaClass senderror = new IcazaClass();

        #endregion

        #region Form_

        public Bookmarklets_Frm()
        {
            InitializeComponent();
        }

        void Bookmarklets_Frm_Load(object sender, EventArgs e)
        {
            loadfiledir.LoadFileDirectory(Scripts, "xml", "lst", Bookmarklet_Lst);
        }

        #endregion

        void NewScript_Btn_Click(object sender, EventArgs e)
        {
            EraseItem();
        }

        void EraseItem()
        {
            NameBkmklt_Txt.Text = "";
            Description_Txt.Text = "";
            ScriptTxt_Txt.Text = "";
            ScriptMinify_Txt.Text = "";
        }

        void OpnScript_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Bookmarklet_Lst.SelectedIndex != -1)
                {
                    string strName = Regex.Replace(Bookmarklet_Lst.Text, ".xml", "");
                    string strFile = Scripts + Bookmarklet_Lst.Text;

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(strFile);
                    XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Table/Bkmklt/" + strName);

                    string strNode = string.Format("{0}", nodeList[0].ChildNodes.Item(0).InnerText);
                    string strNamexml = string.Format("{0}", nodeList[0].Attributes.Item(0).InnerText);
                    string strDesc = string.Format("{0}", nodeList[0].Attributes.Item(1).InnerText);
                    string strMini = string.Format("{0}", nodeList[0].Attributes.Item(2).InnerText);

                    ScriptTxt_Txt.Text = strNode;
                    NameBkmklt_Txt.Text = strNamexml;
                    Description_Txt.Text = strDesc;
                    ScriptMinify_Txt.Text = strMini;
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnScript_Btn_Click: ", ex.Message, "Bookmarklets_Frm", AppStart);
            }
        }

        void SaveScript_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                MinifyJs();

                string NameFile = Scripts + NameBkmklt_Txt.Text + ".xml";

                if (File.Exists(NameFile))
                    File.Delete(NameFile);

                XmlTextWriter writer = new XmlTextWriter(NameFile, Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;

                writer.WriteStartElement("Table");
                writer.WriteStartElement("Bkmklt");
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(NameFile);
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Table/Bkmklt") is XmlElement node1)
                {
                    XmlElement elem = doc.CreateElement(NameBkmklt_Txt.Text);
                    elem.SetAttribute("name", NameBkmklt_Txt.Text);
                    elem.SetAttribute("desc", Description_Txt.Text);
                    elem.SetAttribute("mini", ScriptMinify_Txt.Text);
                    elem.InnerText = ScriptTxt_Txt.Text;
                    node1.AppendChild(elem);
                }

                xmlReader.Close();
                doc.Save(NameFile);

                Bookmarklet_Lst.Items.Clear();
                loadfiledir.LoadFileDirectory(Scripts, "xml", "lst", Bookmarklet_Lst);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! SaveScript_Btn_Click: ", ex.Message, "Bookmarklets_Frm", AppStart);
            }
        }

        void OpnJSfile_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string fileopen = openfile.Fileselect("c:\\", "txt files (*.js)|*.js|All files (*.*)|*.*", 2);

                if (File.Exists(fileopen) == true)
                {
                    using (StreamReader sr = new StreamReader(fileopen))
                    {
                        ScriptTxt_Txt.Text = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnJSfile_Btn_Click: ", ex.Message, "Bookmarklets_Frm", AppStart);
            }
        }

        void MinifyScript_Btn_Click(object sender, EventArgs e)
        {
            MinifyJs();
        }

        void MinifyJs()
        {
            try
            {
                var minifer = new Minifier();

                CodeSettings settings = new CodeSettings
                {
                    EvalTreatment = EvalTreatment.MakeImmediateSafe,
                    PreserveImportantComments = false,
                    LocalRenaming = LocalRenaming.CrunchAll,
                    RemoveUnneededCode = true,
                    PreserveFunctionNames = true,
                    OutputMode = OutputMode.SingleLine
                };

                var content = ScriptTxt_Txt.Text;
                var minified = minifer.MinifyJavaScript(content, settings);

                ScriptMinify_Txt.Text = minified;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! MinifyJs: ", ex.Message, "Bookmarklets_Frm", AppStart);
            }
        }

        void DelScript_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Bookmarklet_Lst.SelectedIndex != -1)
                {
                    string message = "Are you sure to delete a Bookmarklet?";
                    string caption = "Delete";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        File.Delete(Scripts + NameBkmklt_Txt.Text + ".xml");
                    }

                    Bookmarklet_Lst.Items.Clear();
                    loadfiledir.LoadFileDirectory(Scripts, "xml", "lst", Bookmarklet_Lst);

                    EraseItem();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DelScript_Btn_Click: ", ex.Message, "Bookmarklets_Frm", AppStart);
            }
        }

        void CopyScriptMini_Btn_Click(object sender, EventArgs e)
        {
            if (ScriptMinify_Txt.Text != "")
            {
                Clipboard.SetData(DataFormats.Text, ScriptMinify_Txt.Text);
                Beep(300, 200);
            }
        }

        #region ContextMenu_

        void Cut_Tools_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^" + "x");
        }

        void Copy_Mnu_Click(object sender, EventArgs e)
        {
            if (ScriptTxt_Txt.SelectedText != "")
            {
                Clipboard.SetDataObject(ScriptTxt_Txt.SelectedText);
            }
        }

        void Paste_Mnu_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^" + "v");
        }

        void Delete_Mnu_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{DEL}");
        }

        void SelectAll_Mnu_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^" + "a");
        }

        #endregion
    }
}
