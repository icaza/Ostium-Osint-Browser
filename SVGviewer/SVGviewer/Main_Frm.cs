using Svg;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SVGviewer
{
    public partial class Main_Frm : Form
    {
        #region Constants

        private const float MIN_ZOOM_SCALE = 0.1f;
        private const float MAX_ZOOM_SCALE = 10.0f;
        private const float ZOOM_INCREMENT = 0.1f;
        private const int CACHE_UPDATE_TIMER_INTERVAL = 300;
        private const float SCALE_COMPARISON_THRESHOLD = 0.15f;
        private const int MAX_BITMAP_WIDTH = 8192;
        private const int MAX_BITMAP_HEIGHT = 8192;

        #endregion

        #region Fields

        public string SvgFileName { get; set; } = @"svg.svg";

        SvgDocument svgDoc;
        float currentScale = 1.0f;
        float cachedScale = 1.0f;
        float offsetX = 0;
        float offsetY = 0;
        bool isPanning = false;
        Point lastMousePos;

        Bitmap cachedBitmap = null;

        readonly Timer cacheUpdateTimer;

        #endregion

        public Main_Frm()
        {
            InitializeComponent();

            Text = "SVG Visualizer";
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            cacheUpdateTimer = new Timer
            {
                Interval = CACHE_UPDATE_TIMER_INTERVAL
            };
            cacheUpdateTimer.Tick += CacheUpdateTimer_Tick;

            Load += Main_Frm_Load;
            Paint += Main_Frm_Paint;
            MouseDown += Main_Frm_MouseDown;
            MouseMove += Main_Frm_MouseMove;
            MouseUp += Main_Frm_MouseUp;
            MouseWheel += Main_Frm_MouseWheel;
            Resize += Main_Frm_Resize;
        }

        void Main_Frm_Load(object sender, EventArgs e)
        {
            try
            {
                svgDoc = SvgDocument.Open(SvgFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading SVG : " + ex.Message);
            }
        }

        void Main_Frm_Paint(object sender, PaintEventArgs e)
        {
            if (svgDoc == null)
                return;

            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                g.ResetTransform();
                g.TranslateTransform(offsetX, offsetY);

                if (cachedBitmap == null || Math.Abs(currentScale - cachedScale) > SCALE_COMPARISON_THRESHOLD)
                {
                    g.ScaleTransform(currentScale, currentScale);
                    svgDoc.Draw(g);
                }
                else
                {
                    float scaleFactor = currentScale / cachedScale;
                    g.ScaleTransform(scaleFactor, scaleFactor);
                    g.DrawImage(cachedBitmap, 0, 0);
                }
            }
            catch (Exception ex)
            {
                using (var brush = new SolidBrush(Color.Red))
                {
                    e.Graphics.DrawString("Rendering error: " + ex.Message, Font, brush, 10, 10);
                }
            }
        }

        bool ValidateBitmapDimensions(int width, int height)
        {
            return width > 0 && height > 0 &&
                   width <= MAX_BITMAP_WIDTH && height <= MAX_BITMAP_HEIGHT;
        }

        void UpdateCache()
        {
            if (svgDoc == null)
                return;

            try
            {
                float docWidth = svgDoc.Width != 0 ? svgDoc.Width : ClientSize.Width;
                float docHeight = svgDoc.Height != 0 ? svgDoc.Height : ClientSize.Height;

                if (docWidth <= 0) docWidth = ClientSize.Width;
                if (docHeight <= 0) docHeight = ClientSize.Height;

                int bmpWidth = (int)(docWidth * currentScale);
                int bmpHeight = (int)(docHeight * currentScale);

                if (!ValidateBitmapDimensions(bmpWidth, bmpHeight))
                    return;

                cachedBitmap?.Dispose();
                cachedBitmap = new Bitmap(bmpWidth, bmpHeight);

                using (Graphics g = Graphics.FromImage(cachedBitmap))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    Matrix m = new Matrix();
                    m.Scale(currentScale, currentScale);
                    g.Transform = m;

                    svgDoc.Draw(g);
                }
                cachedScale = currentScale;
            }
            catch (Exception ex)
            {
                cachedBitmap?.Dispose();
                cachedBitmap = null;
                MessageBox.Show("Error updating cache: " + ex.Message);
            }
        }

        void CacheUpdateTimer_Tick(object sender, EventArgs e)
        {
            cacheUpdateTimer.Stop();
            if (Math.Abs(currentScale - cachedScale) > SCALE_COMPARISON_THRESHOLD)
            {
                UpdateCache();
                Invalidate();
            }
        }

        void Main_Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPanning = true;
                lastMousePos = e.Location;
                Cursor = Cursors.Hand;
            }
        }

        void Main_Frm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                int dx = e.X - lastMousePos.X;
                int dy = e.Y - lastMousePos.Y;
                offsetX += dx;
                offsetY += dy;
                lastMousePos = e.Location;
                Invalidate();
            }
        }

        void Main_Frm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPanning = false;
                Cursor = Cursors.Default;
            }
        }

        void Main_Frm_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldScale = currentScale;
            currentScale += e.Delta > 0 ? ZOOM_INCREMENT : -ZOOM_INCREMENT;

            if (currentScale < MIN_ZOOM_SCALE)
                currentScale = MIN_ZOOM_SCALE;
            if (currentScale > MAX_ZOOM_SCALE)
                currentScale = MAX_ZOOM_SCALE;

            float scaleChange = currentScale / oldScale;
            offsetX = e.X - scaleChange * (e.X - offsetX);
            offsetY = e.Y - scaleChange * (e.Y - offsetY);

            cacheUpdateTimer.Stop();
            cacheUpdateTimer.Start();

            Invalidate();
        }

        void Main_Frm_Resize(object sender, EventArgs e)
        {
            cachedBitmap?.Dispose();
            cachedBitmap = null;
            Invalidate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            cachedBitmap?.Dispose();
            cachedBitmap = null;
            svgDoc = null;
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main_Frm());
        }
    }
}