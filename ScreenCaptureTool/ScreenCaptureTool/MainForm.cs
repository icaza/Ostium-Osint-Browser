using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenCaptureTool
{
    public partial class MainForm : Form
    {
        Bitmap _capturedImage;
        string _lastFileName;

        public MainForm()
        {
            InitializeComponent();
            UpdateSaveButtonState();
        }

        void BtnFullScreen_Click(object sender, EventArgs e)
        {
            CaptureFullScreen();
        }

        void BtnActiveWindow_Click(object sender, EventArgs e)
        {
            CaptureActiveWindow();
        }

        void BtnRegion_Click(object sender, EventArgs e)
        {
            CaptureRegion();
        }

        void CaptureFullScreen()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Hide();

                int ts = 200;
                if (Delay_Chk.Checked)
                    ts = 5000;

                System.Threading.Thread.Sleep(ts);

                Rectangle bounds = SystemInformation.VirtualScreen;
                using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                    }
                    _capturedImage?.Dispose();
                    _capturedImage = (Bitmap)bmp.Clone();
                }
                pictureBoxPreview.Image = _capturedImage;
                UpdateSaveButtonState();
                StatusText.Text = "Full screen captured.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Capture failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Show();
                Activate();
                Cursor = Cursors.Default;
            }
        }

        void CaptureActiveWindow()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Hide();

                int ts = 200;
                if (Delay_Chk.Checked)
                    ts = 5000;

                System.Threading.Thread.Sleep(ts);

                IntPtr hwnd = GetForegroundWindow();
                if (hwnd == IntPtr.Zero)
                {
                    Show();
                    return;
                }
                if (!GetWindowRect(hwnd, out RECT rect))
                {
                    Show();
                    return;
                }
                Rectangle bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                    }
                    _capturedImage?.Dispose();
                    _capturedImage = (Bitmap)bmp.Clone();
                }
                pictureBoxPreview.Image = _capturedImage;
                UpdateSaveButtonState();
                StatusText.Text = "Active window captured.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Capture failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Show();
                Activate();
                Cursor = Cursors.Default;
            }
        }

        void CaptureRegion()
        {
            try
            {
                Cursor = Cursors.Cross;
                Hide();

                System.Threading.Thread.Sleep(200);

                using (var selector = new RegionSelectorForm())
                {
                    if (selector.ShowDialog() == DialogResult.OK)
                    {
                        Rectangle bounds = selector.SelectedRectangle;
                        using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb))
                        {
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                            }
                            _capturedImage?.Dispose();
                            _capturedImage = (Bitmap)bmp.Clone();
                        }
                        pictureBoxPreview.Image = _capturedImage;
                        UpdateSaveButtonState();
                        StatusText.Text = "Region captured.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Capture failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Show();
                Activate();
                Cursor = Cursors.Default;
            }
        }

        void BtnSave_Click(object sender, EventArgs e)
        {
            if (_capturedImage == null) return;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|SVG Image|*.svg";
                sfd.FilterIndex = 1;
                sfd.FileName = "screenshot_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        string ext = Path.GetExtension(sfd.FileName).ToLower();
                        switch (ext)
                        {
                            case ".png":
                                _capturedImage.Save(sfd.FileName, ImageFormat.Png);
                                break;
                            case ".jpg":
                            case ".jpeg":
                                using (Bitmap bmp = new Bitmap(_capturedImage.Width, _capturedImage.Height, PixelFormat.Format24bppRgb))
                                {
                                    using (Graphics g = Graphics.FromImage(bmp))
                                    {
                                        g.Clear(Color.White);
                                        g.DrawImage(_capturedImage, 0, 0, _capturedImage.Width, _capturedImage.Height);
                                    }
                                    bmp.Save(sfd.FileName, ImageFormat.Jpeg);
                                }
                                break;
                            case ".svg":
                                SaveAsSvg(sfd.FileName);
                                break;
                            default:
                                MessageBox.Show("Unsupported format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                        }
                        _lastFileName = sfd.FileName;
                        StatusText.Text = "Saved to " + _lastFileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Save failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        void SaveAsSvg(string fileName)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                _capturedImage.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                string base64 = Convert.ToBase64String(imageBytes);
                string svg = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""{_capturedImage.Width}"" height=""{_capturedImage.Height}"" viewBox=""0 0 {_capturedImage.Width} {_capturedImage.Height}"">
  <image x=""0"" y=""0"" width=""{_capturedImage.Width}"" height=""{_capturedImage.Height}"" xlink:href=""data:image/png;base64,{base64}"" />
</svg>";
                File.WriteAllText(fileName, svg);
            }
        }

        void UpdateSaveButtonState()
        {
            btnSave.Enabled = _capturedImage != null;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D1))
            {
                btnFullScreen.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D2))
            {
                btnActiveWindow.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D3))
            {
                btnRegion.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.S))
            {
                btnSave.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _capturedImage?.Dispose();
            base.OnFormClosed(e);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    }

    internal class RegionSelectorForm : Form
    {
        Rectangle _selection;
        Point _startPoint;
        bool _selecting;
        Rectangle _virtualScreen;

        public Rectangle SelectedRectangle => _selection;

        public RegionSelectorForm()
        {
            _virtualScreen = SystemInformation.VirtualScreen;
            StartPosition = FormStartPosition.Manual;
            Bounds = _virtualScreen;
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            ShowInTaskbar = false;
            Cursor = Cursors.Cross;
            DoubleBuffered = true;
            KeyPreview = true;
            BackColor = Color.Black;
            Opacity = 0.4;
            MouseDown += RegionSelectorForm_MouseDown;
            MouseMove += RegionSelectorForm_MouseMove;
            MouseUp += RegionSelectorForm_MouseUp;
            KeyDown += RegionSelectorForm_KeyDown;
            Paint += RegionSelectorForm_Paint;
        }

        void RegionSelectorForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _startPoint = e.Location;
                _selection = new Rectangle(e.Location, Size.Empty);
                _selecting = true;
                Invalidate();
            }
        }

        void RegionSelectorForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_selecting)
            {
                int x = Math.Min(_startPoint.X, e.X);
                int y = Math.Min(_startPoint.Y, e.Y);
                int width = Math.Abs(e.X - _startPoint.X);
                int height = Math.Abs(e.Y - _startPoint.Y);
                _selection = new Rectangle(x, y, width, height);
                Invalidate();
            }
        }

        void RegionSelectorForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (_selecting && e.Button == MouseButtons.Left)
            {
                _selecting = false;
                _selection = new Rectangle(
                    _virtualScreen.X + _selection.X,
                    _virtualScreen.Y + _selection.Y,
                    _selection.Width,
                    _selection.Height);
                if (_selection.Width > 4 && _selection.Height > 4)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    _selection = Rectangle.Empty;
                    Invalidate();
                }
            }
        }

        void RegionSelectorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        void RegionSelectorForm_Paint(object sender, PaintEventArgs e)
        {
            if (_selecting && _selection.Width > 0 && _selection.Height > 0)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                {
                    e.Graphics.FillRectangle(brush, _selection);
                }
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, _selection);
                }
            }
        }
    }
}