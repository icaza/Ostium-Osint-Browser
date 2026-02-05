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
        const float MIN_ZOOM_SCALE = 0.1f;
        const float MAX_ZOOM_SCALE = 10.0f;
        const float ZOOM_INCREMENT = 0.1f;
        const int CACHE_UPDATE_TIMER_INTERVAL = 300;
        const float SCALE_COMPARISON_THRESHOLD = 0.15f;
        const int MAX_BITMAP_WIDTH = 8192;
        const int MAX_BITMAP_HEIGHT = 8192;
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
        Color defaultBackgroundColor = Color.LightGray; // Couleur de fond par défaut
        bool useTransparencyPattern = true; // Afficher le motif pour la transparence
        Color transparencyPatternColor1 = Color.FromArgb(240, 240, 240); // Couleur 1 du motif
        Color transparencyPatternColor2 = Color.White; // Couleur 2 du motif

        readonly Timer cacheUpdateTimer;
        #endregion

        public Main_Frm()
        {
            InitializeComponent();

            Text = "SVG Visualizer";
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            // Ajouter un menu contextuel pour changer la couleur de fond
            ContextMenuStrip = CreateBackgroundMenu();

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

        ContextMenuStrip CreateBackgroundMenu()
        {
            var menu = new ContextMenuStrip();

            // Titre
            menu.Items.Add("Couleur de fond :").Enabled = false;
            menu.Items.Add(new ToolStripSeparator());

            // Options de couleur
            var lightGrayItem = new ToolStripMenuItem("Gris clair");
            lightGrayItem.Click += (s, e) => SetBackgroundColor(Color.LightGray, "Gris clair");
            menu.Items.Add(lightGrayItem);

            var whiteItem = new ToolStripMenuItem("Blanc");
            whiteItem.Click += (s, e) => SetBackgroundColor(Color.White, "Blanc");
            menu.Items.Add(whiteItem);

            var blackItem = new ToolStripMenuItem("Noir");
            blackItem.Click += (s, e) => SetBackgroundColor(Color.Black, "Noir");
            menu.Items.Add(blackItem);

            var darkGrayItem = new ToolStripMenuItem("Gris foncé");
            darkGrayItem.Click += (s, e) => SetBackgroundColor(Color.DarkGray, "Gris foncé");
            menu.Items.Add(darkGrayItem);

            // Option pour désactiver le motif de transparence
            menu.Items.Add(new ToolStripSeparator());
            var patternToggle = new ToolStripMenuItem("Afficher motif de transparence");
            patternToggle.Checked = useTransparencyPattern;
            patternToggle.Click += (s, e) =>
            {
                useTransparencyPattern = !useTransparencyPattern;
                patternToggle.Checked = useTransparencyPattern;
                InvalidateCacheAndRedraw();
            };
            menu.Items.Add(patternToggle);

            // Personnaliser la couleur
            menu.Items.Add(new ToolStripSeparator());
            var customColorItem = new ToolStripMenuItem("Personnaliser...");
            customColorItem.Click += (s, e) => ChooseCustomColor();
            menu.Items.Add(customColorItem);

            return menu;
        }

        void SetBackgroundColor(Color color, string colorName)
        {
            defaultBackgroundColor = color;
            this.BackColor = color;
            Text = $"SVG Visualizer - Fond: {colorName}";
            InvalidateCacheAndRedraw();
        }

        void ChooseCustomColor()
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = defaultBackgroundColor;
                colorDialog.FullOpen = true;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    defaultBackgroundColor = colorDialog.Color;
                    this.BackColor = colorDialog.Color;
                    Text = $"SVG Visualizer - Fond: Personnalisé";
                    InvalidateCacheAndRedraw();
                }
            }
        }

        void InvalidateCacheAndRedraw()
        {
            cachedBitmap?.Dispose();
            cachedBitmap = null;
            Invalidate();
        }

        void Main_Frm_Load(object sender, EventArgs e)
        {
            try
            {
                svgDoc = SvgDocument.Open(SvgFileName);
                this.BackColor = defaultBackgroundColor;
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

                // Détecter si le SVG a un fond défini
                bool hasBackground = HasSvgBackground();

                if (cachedBitmap == null || Math.Abs(currentScale - cachedScale) > SCALE_COMPARISON_THRESHOLD)
                {
                    g.ScaleTransform(currentScale, currentScale);

                    // Si le SVG n'a pas de fond défini ET qu'on veut le motif de transparence
                    if (!hasBackground && useTransparencyPattern)
                    {
                        DrawTransparentBackground(g, svgDoc.Width, svgDoc.Height);
                    }
                    else if (!hasBackground && !useTransparencyPattern)
                    {
                        // Remplir avec la couleur de fond par défaut
                        g.Clear(defaultBackgroundColor);
                    }

                    svgDoc.Draw(g);
                }
                else
                {
                    float scaleFactor = currentScale / cachedScale;
                    g.ScaleTransform(scaleFactor, scaleFactor);

                    // Si le SVG n'a pas de fond défini ET qu'on veut le motif de transparence
                    if (!hasBackground && useTransparencyPattern)
                    {
                        DrawTransparentBackground(g, cachedBitmap.Width / scaleFactor, cachedBitmap.Height / scaleFactor);
                    }
                    else if (!hasBackground && !useTransparencyPattern)
                    {
                        // Remplir avec la couleur de fond par défaut
                        float width = cachedBitmap.Width / scaleFactor;
                        float height = cachedBitmap.Height / scaleFactor;
                        using (var brush = new SolidBrush(defaultBackgroundColor))
                        {
                            g.FillRectangle(brush, 0, 0, width, height);
                        }
                    }

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

        bool HasSvgBackground()
        {
            try
            {
                if (svgDoc != null && svgDoc.Children.Count > 0)
                {
                    foreach (var element in svgDoc.Children)
                    {
                        if (element is SvgRectangle rect)
                        {
                            if (Math.Abs(rect.Width - svgDoc.Width) < 1 && Math.Abs(rect.Height - svgDoc.Height) < 1)
                            {
                                return true;
                            }

                            if (rect.Fill != null && rect.Fill.ToString() != "none")
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        void DrawTransparentBackground(Graphics g, float width, float height)
        {
            const int tileSize = 20;

            for (float y = 0; y < height; y += tileSize)
            {
                for (float x = 0; x < width; x += tileSize)
                {
                    bool isLight = ((int)(x / tileSize) + (int)(y / tileSize)) % 2 == 0;

                    // Adapter les couleurs du motif en fonction de la couleur de fond
                    Color color1 = transparencyPatternColor1;
                    Color color2 = transparencyPatternColor2;

                    // Si le fond est sombre, utiliser des couleurs plus foncées pour le motif
                    if (defaultBackgroundColor.GetBrightness() < 0.5)
                    {
                        color1 = Color.FromArgb(60, 60, 60);
                        color2 = Color.FromArgb(80, 80, 80);
                    }

                    using (var brush = new SolidBrush(isLight ? color1 : color2))
                    {
                        g.FillRectangle(brush, x, y, tileSize, tileSize);
                    }
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

                    bool hasBackground = HasSvgBackground();

                    // Si le SVG n'a pas de fond défini, ajouter le fond approprié
                    if (!hasBackground)
                    {
                        if (useTransparencyPattern)
                        {
                            DrawTransparentBackground(g, docWidth, docHeight);
                        }
                        else
                        {
                            g.Clear(defaultBackgroundColor);
                        }
                    }

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
            else if (e.Button == MouseButtons.Right)
            {
                // Afficher le menu contextuel pour changer la couleur de fond
                ContextMenuStrip?.Show(this, e.Location);
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
            cacheUpdateTimer?.Stop();
            cacheUpdateTimer?.Dispose();
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