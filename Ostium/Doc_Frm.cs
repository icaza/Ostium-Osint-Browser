using Icaza;
using System;
using System.IO;
using System.Windows.Forms;

namespace Ostium
{
    public partial class Doc_Frm : Form
    {
        readonly string AppStart = Application.StartupPath + @"\";

        public Doc_Frm()
        {
            InitializeComponent();
            Sortie_Txt.DoubleClick += new EventHandler(Sortie_Txt_DoubleClick);
        }
        ///
        /// <summary>
        /// Open Text or File
        /// </summary>
        /// <param name="Class_Var.textload">Open text</param>
        /// <param name="Class_Var.File_Open">Open file</param>
        /// 
        void Doc_Frm_Load(object sender, EventArgs e)
        {
            try
            {
                string strName = Path.GetFileName(Class_Var.File_Open);

                if (strName == "textload")
                {
                    Sortie_Txt.Text = Class_Var.Text_Load;
                    return;
                }

                if (File.Exists(Class_Var.File_Open))
                {
                    using (StreamReader sr = new StreamReader(Class_Var.File_Open))
                    {
                        Sortie_Txt.Text = sr.ReadToEnd();
                    }

                    Sortie_Txt.Select(Sortie_Txt.Text.Length, 0);
                    Text = "File open: " + strName + " [ Double-click to display the scrollbar ]";
                }
            }
            catch (Exception ex)
            {
                IcazaClass senderror = new IcazaClass();
                senderror.ErrorLog("Error! Doc_Frm_Load: ", ex.ToString(), "Doc_Frm", AppStart);
            }
        }

        void Sortie_Txt_DoubleClick(object sender, EventArgs e)
        {
            if (Sortie_Txt.ScrollBars == ScrollBars.None)
                Sortie_Txt.ScrollBars = ScrollBars.Vertical;
            else
                Sortie_Txt.ScrollBars = ScrollBars.None;
        }
    }
}
