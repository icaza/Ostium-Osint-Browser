using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace QRCodeTool
{
    public partial class MainForm : Form
    {
        #region Var_
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        Color _foreColor = Color.Black;
        Color _backColor = Color.White;
        Bitmap _generatedQR;
        #endregion

        public MainForm()
        {
            InitializeComponent();
            AttachEvents();
        }

        void AttachEvents()
        {
            MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            tabMain.MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            pnlGenLeft.MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            tabPageGenerate.MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            tabPageScan.MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            pnlScanRight.MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
            TTS_Tts.MouseDown += new MouseEventHandler(Main_Frm_MouseDown);

            txtContent.ForeColor = Color.Gray;
            txtContent.Text = "Enter text, URL, contact info...";
            txtContent.Enter += TxtContent_Enter;
            txtContent.Leave += TxtContent_Leave;

            btnGenerate.Click += BtnGenerate_Click;
            btnSaveQR.Click += BtnSaveQR_Click;
            btnForeColor.Click += BtnForeColor_Click;
            btnBackColor.Click += BtnBackColor_Click;

            btnLoadImage.Click += BtnLoadImage_Click;
            btnCopyResult.Click += BtnCopyResult_Click;
            btnOpenUrl.Click += BtnOpenUrl_Click;

            picScanned.AllowDrop = true;
            picScanned.DragEnter += PicScanned_DragEnter;
            picScanned.DragDrop += PicScanned_DragDrop;

            cmbErrorCorrection.Text = "M  –  Medium  (15 %)";
        }

        void Main_Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        void TxtContent_Enter(object sender, EventArgs e)
        {
            if (txtContent.ForeColor == Color.Gray)
            {
                txtContent.Text = string.Empty;
                txtContent.ForeColor = SystemColors.WindowText;
            }
        }

        void TxtContent_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtContent.Text))
            {
                txtContent.ForeColor = Color.Gray;
                txtContent.Text = "Enter text, URL, contact info...";
            }
        }

        void BtnGenerate_Click(object sender, EventArgs e)
        {
            string content = GetContentText();
            if (string.IsNullOrWhiteSpace(content))
            {
                SetGenStatus("Please enter content to encode.", Color.Crimson);
                return;
            }

            try
            {
                int size = (int)numSize.Value;
                ErrorCorrectionLevel ecl = GetSelectedErrorLevel();

                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions
                    {
                        Width = size,
                        Height = size,
                        Margin = 1,
                        ErrorCorrection = ecl,
                        CharacterSet = "UTF-8"
                    }
                };

                BitMatrix matrix = writer.Encode(content);

                _generatedQR?.Dispose();
                _generatedQR = RenderMatrix(matrix, size, _foreColor, _backColor);
                picGenerated.Image = _generatedQR;
                btnSaveQR.Enabled = true;

                SetGenStatus(string.Format("QR Code generated ({0}\u00d7{0} px).", size), Color.Lime);
            }
            catch (Exception ex)
            {
                SetGenStatus("Error: " + ex.Message, Color.Crimson);
            }
        }

        ErrorCorrectionLevel GetSelectedErrorLevel()
        {
            switch (cmbErrorCorrection.SelectedIndex)
            {
                case 0: return ErrorCorrectionLevel.L;
                case 2: return ErrorCorrectionLevel.Q;
                case 3: return ErrorCorrectionLevel.H;
                default: return ErrorCorrectionLevel.M;
            }
        }

        void BtnSaveQR_Click(object sender, EventArgs e)
        {
            if (_generatedQR == null) return;

            using (var dlg = new SaveFileDialog
            {
                Title = "Save QR Code Image",
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap|*.bmp",
                DefaultExt = "png",
                FileName = "qrcode"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                ImageFormat fmt;
                switch (dlg.FilterIndex)
                {
                    case 2: fmt = ImageFormat.Jpeg; break;
                    case 3: fmt = ImageFormat.Bmp; break;
                    default: fmt = ImageFormat.Png; break;
                }

                _generatedQR.Save(dlg.FileName, fmt);
                SetGenStatus("Saved: " + Path.GetFileName(dlg.FileName), Color.Lime);
            }
        }

        void BtnForeColor_Click(object sender, EventArgs e) => PickColor(ref _foreColor, btnForeColor);

        void BtnBackColor_Click(object sender, EventArgs e) => PickColor(ref _backColor, btnBackColor);

        void BtnLoadImage_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Title = "Open QR Code Image",
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff|All Files|*.*"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    LoadAndDecodeImage(dlg.FileName);
            }
        }

        void BtnCopyResult_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtResult.Text))
            {
                Clipboard.SetText(txtResult.Text);
                SetScanStatus("Copied to clipboard!", Color.Lime);
            }
        }

        void BtnOpenUrl_Click(object sender, EventArgs e)
        {
            string url = txtResult.Text.Trim();
            if (string.IsNullOrEmpty(url)) return;
            try { System.Diagnostics.Process.Start(url); }
            catch { MessageBox.Show("Unable to open URL.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        void PicScanned_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void PicScanned_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
                LoadAndDecodeImage(files[0]);
        }

        void LoadAndDecodeImage(string filePath)
        {
            try
            {
                Image old = picScanned.Image;
                picScanned.Image = null;
                old?.Dispose();

                Bitmap bmp;
                using (var tmp = Image.FromFile(filePath))
                    bmp = new Bitmap(tmp);

                picScanned.Image = bmp;

                Result result = DecodeQRCode(bmp);

                if (result != null)
                {
                    txtResult.Text = result.Text;
                    btnCopyResult.Enabled = true;
                    bool isUrl = result.Text.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                              || result.Text.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
                    btnOpenUrl.Enabled = isUrl;
                    SetScanStatus("QR Code decoded successfully.", Color.Lime);
                }
                else
                {
                    txtResult.Text = string.Empty;
                    btnCopyResult.Enabled = false;
                    btnOpenUrl.Enabled = false;
                    SetScanStatus("No QR Code found in this image.", Color.DarkOrange);
                }
            }
            catch (Exception ex)
            {
                SetScanStatus("Error: " + ex.Message, Color.Crimson);
            }
        }

        // ── ZXing Decoding Pipeline ──────────────────────────────────────────────

        /// <summary>
        /// Multi-pass QR decoder that handles any color combination:
        ///
        ///  Pass 1 — standard RGBLuminanceSource + HybridBinarizer
        ///            Works for dark-on-light (classic black-on-white).
        ///
        ///  Pass 2 — RGBLuminanceSource + InvertedLuminanceSource + HybridBinarizer
        ///            Works for light-on-dark (white-on-black, yellow-on-black…)
        ///            InvertedLuminanceSource flips the luminance plane so ZXing sees
        ///            the modules as dark regardless of their actual hue.
        ///
        ///  Pass 3 — Grayscale bitmap (perceptual luma) + HybridBinarizer
        ///            Handles low-contrast or unusual tints where hue conversion helps.
        ///
        ///  Pass 4 — Grayscale + InvertedLuminanceSource
        ///            Inverted version of pass 3.
        /// </summary>
        static Result DecodeQRCode(Bitmap source)
        {
            var hints = new System.Collections.Generic.Dictionary<DecodeHintType, object>
            {
                { DecodeHintType.TRY_HARDER, true }
            };

            // Ensure 32-bpp ARGB for consistent LockBits stride
            Bitmap argb = EnsureArgb(source);

            try
            {
                int w, h;
                byte[] rawBytes = LockBitsToBytes(argb, out w, out h);

                // Pass 1 — normal luminance
                var lumNormal = new RGBLuminanceSource(rawBytes, w, h, RGBLuminanceSource.BitmapFormat.RGB32);
                Result r = TryDecode(new BinaryBitmap(new HybridBinarizer(lumNormal)), hints);
                if (r != null) return r;

                // Pass 2 — inverted luminance (handles light-modules on dark background)
                var lumInverted = new InvertedLuminanceSource(lumNormal);
                r = TryDecode(new BinaryBitmap(new HybridBinarizer(lumInverted)), hints);
                if (r != null) return r;

                // Pass 3 — perceptual grayscale conversion, normal
                byte[] grayBytes = ToGrayscaleBytes(rawBytes, w, h);
                var lumGray = new RGBLuminanceSource(grayBytes, w, h, RGBLuminanceSource.BitmapFormat.Gray8);
                r = TryDecode(new BinaryBitmap(new HybridBinarizer(lumGray)), hints);
                if (r != null) return r;

                // Pass 4 — perceptual grayscale, inverted
                var lumGrayInv = new InvertedLuminanceSource(lumGray);
                r = TryDecode(new BinaryBitmap(new HybridBinarizer(lumGrayInv)), hints);
                return r; // null if all passes fail
            }
            finally
            {
                // Only dispose the cloned bitmap, never the original displayed in the PictureBox
                if (!ReferenceEquals(argb, source))
                    argb.Dispose();
            }
        }

        /// <summary>Tries to decode; returns null instead of throwing on failure.</summary>
        static Result TryDecode(BinaryBitmap binBmp,
            System.Collections.Generic.IDictionary<DecodeHintType, object> hints)
        {
            try { return new ZXing.QrCode.QRCodeReader().decode(binBmp, hints); }
            catch { return null; }
        }

        /// <summary>
        /// Returns the bitmap as-is if already Format32bppArgb,
        /// otherwise returns a freshly drawn 32-bpp clone.
        /// </summary>
        static Bitmap EnsureArgb(Bitmap bmp)
        {
            if (bmp.PixelFormat == PixelFormat.Format32bppArgb) return bmp;
            var clone = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(clone))
                g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
            return clone;
        }

        /// <summary>
        /// Extracts raw BGRA bytes from a 32-bpp ARGB bitmap via LockBits.
        /// </summary>
        static byte[] LockBitsToBytes(Bitmap bmp, out int width, out int height)
        {
            width = bmp.Width;
            height = bmp.Height;
            var rect = new Rectangle(0, 0, width, height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int count = Math.Abs(bmpData.Stride) * height;
            var bytes = new byte[count];
            Marshal.Copy(bmpData.Scan0, bytes, 0, count);
            bmp.UnlockBits(bmpData);
            return bytes;
        }

        /// <summary>
        /// Converts 32-bpp BGRA bytes to an 8-bpp perceptual grayscale byte array
        /// (one byte per pixel, no padding) using the BT.601 luma coefficients:
        ///   Y = 0.299·R + 0.587·G + 0.114·B
        /// This correctly handles coloured QR modules (yellow, red, blue…) because
        /// the luma value reflects perceived brightness, not just raw RGB inversion.
        /// </summary>
        static byte[] ToGrayscaleBytes(byte[] bgra, int width, int height)
        {
            byte[] gray = new byte[width * height];
            int stride = width * 4; // always width*4 after EnsureArgb

            for (int row = 0; row < height; row++)
            {
                int rowOffset = row * stride;
                int grayBase = row * width;
                for (int col = 0; col < width; col++)
                {
                    int p = rowOffset + col * 4;
                    byte b = bgra[p];
                    byte g = bgra[p + 1];
                    byte rv = bgra[p + 2];
                    // BT.601 perceptual luma
                    gray[grayBase + col] = (byte)(0.299 * rv + 0.587 * g + 0.114 * b);
                }
            }
            return gray;
        }

        // ── Rendering ────────────────────────────────────────────────────────────

        /// <summary>
        /// Renders a ZXing BitMatrix to a Bitmap.
        /// Does NOT require ZXing.Net.Bindings.Windows.Compatibility.
        /// </summary>
        static Bitmap RenderMatrix(BitMatrix matrix, int size, Color fore, Color back)
        {
            int w = matrix.Width;
            int h = matrix.Height;
            var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(back);
                float cw = (float)size / w;
                float ch = (float)size / h;

                using (var brush = new SolidBrush(fore))
                {
                    for (int row = 0; row < h; row++)
                        for (int col = 0; col < w; col++)
                            if (matrix[col, row])
                                g.FillRectangle(brush,
                                    col * cw, row * ch,
                                    cw + 0.5f, ch + 0.5f); // +0.5 closes sub-pixel gaps
                }
            }
            return bmp;
        }

        // ── Shared Helpers ───────────────────────────────────────────────────────

        string GetContentText()
        {
            return (txtContent.ForeColor == Color.Gray)
                ? string.Empty
                : txtContent.Text.Trim();
        }

        void SetGenStatus(string msg, Color color)
        {
            lblGenStatus.ForeColor = color;
            lblGenStatus.Text = msg;
        }

        void SetScanStatus(string msg, Color color)
        {
            lblScanStatus.ForeColor = color;
            lblScanStatus.Text = msg;
        }

        void PickColor(ref Color target, Button btn)
        {
            using (var dlg = new ColorDialog { Color = target, FullOpen = true })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                target = dlg.Color;
                btn.ForeColor = (target.GetBrightness() > 0.6f) ? Color.Black : target;
                btn.BackColor = target;
            }
        }

        // ── Cleanup ──────────────────────────────────────────────────────────────
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _generatedQR?.Dispose();
            picScanned.Image?.Dispose();
            base.OnFormClosed(e);
        }

        void Quit_Btn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
