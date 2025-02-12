﻿using LoadDirectory;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Ostium
{
    public partial class Keeptrack_Frm : Form
    {
        #region Var_

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        readonly string Keeptrack = Application.StartupPath + @"\KeepTrack\";

        #endregion

        public Keeptrack_Frm()
        {
            InitializeComponent();

            MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            MouseEnter += new EventHandler(Main_Frm_MouseEnter);
            MouseLeave += new EventHandler(Main_Frm_Leave);
            TrackRecord_Cbx.MouseEnter += new EventHandler(Main_Frm_MouseEnter);
            TrackRecord_Cbx.MouseLeave += new EventHandler(Main_Frm_Leave);
            Tags_Txt.MouseEnter += new EventHandler(Main_Frm_MouseEnter);
            Tags_Txt.MouseLeave += new EventHandler(Main_Frm_Leave);
        }

        void Keeptrack_Frm_Load(object sender, EventArgs e)
        {
            if (Application.OpenForms["Main_Frm"] is Main_Frm mainForm)
            {
                int x = mainForm.Location.X + mainForm.Width - Width - 32;
                int y = mainForm.Location.Y + mainForm.Height - Height - 39;
                Location = new Point(x, y);
            }

            Loaddir loadfiledir = new Loaddir();
            loadfiledir.LoadFileDirectory(Keeptrack, "csv", "cbx", TrackRecord_Cbx);
        }

        void Main_Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        void Main_Frm_MouseEnter(object sender, EventArgs e)
        {
            Opacity = 1;
        }

        void Main_Frm_Leave(object sender, EventArgs e)
        {
            Opacity = 0.3;
        }

        void AddTrack_Btn_Click(object sender, EventArgs e)
        {
            if (!IsCsvFile(TrackRecord_Cbx.Text))
                TrackRecord_Cbx.Text += ".csv";

            if (Tags_Txt.Text == string.Empty)
                Tags_Txt.Text = "No Tag";

            var logger = new VisitLogger(Path.Combine(Keeptrack, TrackRecord_Cbx.Text));
            logger.LogVisit(@Class_Var.URL_URI, Tags_Txt.Text);

            FaviconLoad();
        }

        static bool IsCsvFile(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) && Path.GetExtension(filePath).Equals(".csv", StringComparison.OrdinalIgnoreCase);
        }

        async void FaviconLoad()
        {
            var downloader = new FaviconDownloader();

            try
            {
                string Domain = new Uri(@Class_Var.URL_URI).Host;
                string icoName = GenerateFileName(Domain);

                if (!File.Exists(Path.Combine(Keeptrack, "thumbnails", icoName + ".ico")))
                {
                    var favicon = await downloader.GetFaviconAsync(@Class_Var.URL_URI);
                    File.WriteAllBytes(Path.Combine(Keeptrack, "thumbnails", icoName + ".ico"), favicon);
                    Console.WriteLine("Favicon sucess download !");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }

        string GenerateFileName(string sdata)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(sdata));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        void Close_Btn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
