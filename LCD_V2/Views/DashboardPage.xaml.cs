using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LCD.Core.Services;

namespace LCD_V2.Views
{
    public partial class DashboardPage : UserControl
    {
        // Crosshair position, stored as percentages of the viewport so resizing preserves layout.
        // 50 = centre (default behaviour before this feature existed).
        private double _hPct = 50.0;  // horizontal reference line — its Y as % of viewport height
        private double _vPct = 50.0;  // vertical   reference line — its X as % of viewport width

        private Line _hLine;
        private Line _vLine;

        // current simulated run state — what point we're "on"
        private TemplateItem _runningTemplate;
        private IReadOnlyList<PointPos> _runningPoints;
        private int _currentPointIndex;

        // Guards the initial restore — SelectionChanged fires during Load/SetSelected
        // and we don't want to overwrite saved state with the mid-init value.
        private bool _restoring = true;

        public DashboardPage()
        {
            InitializeComponent();

            TemplateCombo.ItemsSource = TemplateStore.Library;
            MetricCombo.ItemsSource   = MetricStore.Library;
            MotionCombo.ItemsSource   = MotionStore.Library;

            // Restore persisted selections — fall back to the first item when the saved
            // name is missing from the library (e.g. user deleted that template since
            // last run).
            var s = DashboardSettingsStore.Current;
            TemplateCombo.SelectedItem =
                FindByName(TemplateStore.Library, s.SelectedTemplateName, t => t.Name)
                ?? (TemplateStore.Library.Count > 0 ? TemplateStore.Library[0] : null);
            MetricCombo.SelectedItem =
                FindByName(MetricStore.Library, s.SelectedMetricName, m => m.Name)
                ?? (MetricStore.Library.Count > 0 ? MetricStore.Library[0] : null);
            MotionCombo.SelectedItem =
                FindByName(MotionStore.Library, s.SelectedMotionName, m => m.Name)
                ?? (MotionStore.Library.Count > 0 ? MotionStore.Library[0] : null);

            _hPct = Clamp(s.CrosshairHPct, 0, 100);
            _vPct = Clamp(s.CrosshairVPct, 0, 100);
            if (!string.IsNullOrWhiteSpace(s.StepPercent) && TxtStep != null)
                TxtStep.Text = s.StepPercent;

            _restoring = false;
        }

        private static T FindByName<T>(IEnumerable<T> src, string name, Func<T, string> getName)
            where T : class
        {
            if (string.IsNullOrEmpty(name) || src == null) return null;
            foreach (var item in src)
                if (string.Equals(getName(item), name, StringComparison.Ordinal)) return item;
            return null;
        }

        private void PersistSelections()
        {
            if (_restoring) return;
            var s = DashboardSettingsStore.Current;
            s.SelectedTemplateName = (TemplateCombo.SelectedItem as TemplateItem)?.Name;
            s.SelectedMetricName   = (MetricCombo.SelectedItem   as InstrumentMetric)?.Name;
            s.SelectedMotionName   = (MotionCombo.SelectedItem   as MotionProfile)?.Name;
            s.CrosshairHPct        = _hPct;
            s.CrosshairVPct        = _vPct;
            s.StepPercent          = TxtStep?.Text ?? s.StepPercent;
            DashboardSettingsStore.Save();
        }

        private InstrumentMetric _activeMetric;
        private MotionProfile    _activeMotion;

        private void MetricCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _activeMetric = MetricCombo.SelectedItem as InstrumentMetric;
            // No visual effect on the schematic yet — metric selection is consumed when
            // a real test run reads parameter / algorithm settings from the profile.
            PersistSelections();
        }

        private void MotionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _activeMotion = MotionCombo.SelectedItem as MotionProfile;
            // Reserved for future use — motion profile drives MovCtrl when run wiring lands.
            PersistSelections();
        }

        // ─────────────────────────────────────────────────────────────
        //  template selection + simulated run
        // ─────────────────────────────────────────────────────────────

        private void TemplateCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TemplateCombo.SelectedItem is TemplateItem t) SwitchTemplate(t);
            PersistSelections();
        }

        private void SwitchTemplate(TemplateItem t)
        {
            _runningTemplate = t;
            _runningPoints   = GeneratePoints(t);

            // Mock simulated progress — a reasonable "we're 30% through" snapshot so the
            // three colour states (done / current / pending) are all visible on-screen.
            _currentPointIndex = Math.Min(Math.Max((int)Math.Round(_runningPoints.Count * 0.3), 1),
                                          _runningPoints.Count - 1);

            DrawTemplateSchematic(t, _runningPoints, _currentPointIndex);
            RefreshResultsMock();
            UpdateProgressChrome();
            HideDashCallout();
        }

        private static IReadOnlyList<PointPos> GeneratePoints(TemplateItem t)
        {
            var input = new PointLayoutInput { H = t.H, V = t.V, UseMeter = t.UseMm };
            if (t.UseMm) { input.Amm = t.A; input.Bmm = t.B; input.Cmm = t.C; input.Dmm = t.D; }
            else         { input.Apct = t.A; input.Bpct = t.B; input.Cpct = t.C; input.Dpct = t.D; }
            return PointLayoutService.Generate(t.ConfigType, input);
        }

        private void UpdateProgressChrome()
        {
            int done    = _currentPointIndex;
            int total   = _runningPoints?.Count ?? 0;
            int current = total > 0 ? 1 : 0;

            RunProgressCurrent.Text = _currentPointIndex.ToString();
            RunProgressTotal.Text   = " / " + total;
            RunProgress.Maximum     = Math.Max(1, total);
            RunProgress.Value       = _currentPointIndex;

            TxtResultsMeasured.Text = $"{done} 已采";
            TxtResultsInFlight.Text = $"{current} 进行中";

            if (_runningTemplate != null)
            {
                TxtTemplateCount.Text = $"· {total} 点";
                TxtTemplateDims.Text  = $"{_runningTemplate.H:0}×{_runningTemplate.V:0} mm";
            }
        }

        private void RefreshResultsMock()
        {
            var rows = new List<TestResult>();
            var rng  = new Random(12345); // fixed seed so the demo looks stable
            for (int i = 0; i < _currentPointIndex; i++)
            {
                rows.Add(new TestResult
                {
                    No  = i + 1,
                    L   = 280 + rng.NextDouble() * 50,
                    Cx  = 0.3080 + rng.NextDouble() * 0.008,
                    Cy  = 0.3250 + rng.NextDouble() * 0.008,
                    CCT = 6400 + rng.Next(200),
                    Status = "OK",
                });
            }
            rows.Add(new TestResult { No = _currentPointIndex + 1, Status = "测量中" });
            ResultGrid.ItemsSource = rows;
            ResultGrid.SelectedIndex = rows.Count - 1;
            ResultGrid.ScrollIntoView(rows[rows.Count - 1]);
        }

        // ─────────────────────────────────────────────────────────────
        //  template schematic — colour-coded dots
        // ─────────────────────────────────────────────────────────────

        private static readonly SolidColorBrush DoneFill    = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)); // green
        private static readonly SolidColorBrush CurrentFill = new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B)); // amber
        private static readonly SolidColorBrush PendingFill = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF)); // gray
        private static readonly SolidColorBrush ProductStroke = new SolidColorBrush(Color.FromRgb(0xD1, 0xD5, 0xDB));
        private static readonly SolidColorBrush GridlineStroke = new SolidColorBrush(Color.FromArgb(0x40, 0xE5, 0xE7, 0xEB));

        private void DrawTemplateSchematic(TemplateItem t, IReadOnlyList<PointPos> pts, int currentIndex)
        {
            TemplateCanvas.Children.Clear();
            if (t == null || t.H <= 0 || t.V <= 0) return;

            TemplateCanvas.Width  = t.H;
            TemplateCanvas.Height = t.V;

            // gridlines at 25/50/75%
            for (int i = 1; i <= 3; i++)
            {
                TemplateCanvas.Children.Add(new Line
                {
                    X1 = t.H * i / 4.0, Y1 = 0, X2 = t.H * i / 4.0, Y2 = t.V,
                    Stroke = GridlineStroke, StrokeThickness = 0.4,
                });
                TemplateCanvas.Children.Add(new Line
                {
                    X1 = 0, Y1 = t.V * i / 4.0, X2 = t.H, Y2 = t.V * i / 4.0,
                    Stroke = GridlineStroke, StrokeThickness = 0.4,
                });
            }

            // product boundary
            TemplateCanvas.Children.Add(new Rectangle
            {
                Width  = t.H,
                Height = t.V,
                Fill   = Brushes.White,
                Stroke = ProductStroke,
                StrokeThickness = 1.0,
            });

            double dotR   = Math.Max(6.0, Math.Min(t.H, t.V) * 0.035);
            double fontPt = Math.Max(4.0, dotR * 1.2);

            for (int i = 0; i < pts.Count; i++)
            {
                var p = pts[i];
                Brush fill;
                string statusLbl;
                if (i < currentIndex)      { fill = DoneFill;    statusLbl = "已测"; }
                else if (i == currentIndex) { fill = CurrentFill; statusLbl = "测量中"; }
                else                        { fill = PendingFill; statusLbl = "待测"; }

                var dot = new Ellipse
                {
                    Width  = dotR * 2,
                    Height = dotR * 2,
                    Fill   = fill,
                    Stroke = Brushes.White,
                    StrokeThickness = i == currentIndex ? 1.2 : 0.6,
                    Cursor  = Cursors.Hand,
                    Tag     = p,
                    ToolTip = $"#{p.Id}  {statusLbl}\nX = {p.XMm:0.00} mm  ({p.XPct:0.0}%)\nY = {p.YMm:0.00} mm  ({p.YPct:0.0}%)",
                };
                Canvas.SetLeft(dot, p.XMm - dotR);
                Canvas.SetTop(dot,  p.YMm - dotR);
                dot.MouseLeftButtonDown += DashDot_MouseLeftButtonDown;
                TemplateCanvas.Children.Add(dot);

                var label = new TextBlock
                {
                    Text       = p.Id.ToString(),
                    FontSize   = fontPt,
                    Foreground = new SolidColorBrush(Color.FromRgb(0x37, 0x41, 0x51)),
                    IsHitTestVisible = false,
                    FontWeight = FontWeights.SemiBold,
                };
                Canvas.SetLeft(label, p.XMm + dotR + 0.4);
                Canvas.SetTop(label,  p.YMm - dotR - 0.2);
                TemplateCanvas.Children.Add(label);
            }
        }

        private void DashDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Ellipse dot) || !(dot.Tag is PointPos p)) return;
            DashCalloutTitle.Text = $"#{p.Id}";
            DashCalloutBody.Text  = $"X = {p.XMm:0.00} mm  ({p.XPct:0.0}%)\nY = {p.YMm:0.00} mm  ({p.YPct:0.0}%)";
            DashCallout.Visibility = Visibility.Visible;
        }

        private void HideDashCallout()
        {
            if (DashCallout != null) DashCallout.Visibility = Visibility.Collapsed;
        }

        // ─────────────────────────────────────────────────────────────
        // Crosshair reference-line adjustment
        // ─────────────────────────────────────────────────────────────

        private static readonly SolidColorBrush CrossStroke =
            new SolidColorBrush(Color.FromRgb(0x9F, 0x12, 0x39));

        private void CrosshairCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            EnsureLines();
            UpdateCrosshair();
        }

        private void EnsureLines()
        {
            if (_hLine == null)
            {
                _hLine = new Line { Stroke = CrossStroke, StrokeThickness = 0.8, SnapsToDevicePixels = true };
                CrosshairCanvas.Children.Add(_hLine);
            }
            if (_vLine == null)
            {
                _vLine = new Line { Stroke = CrossStroke, StrokeThickness = 0.8, SnapsToDevicePixels = true };
                CrosshairCanvas.Children.Add(_vLine);
            }
        }

        private void UpdateCrosshair()
        {
            if (CrosshairCanvas == null) return;
            EnsureLines();

            double w = CrosshairCanvas.ActualWidth;
            double h = CrosshairCanvas.ActualHeight;
            if (w <= 0 || h <= 0) return;

            double yPx = h * _hPct / 100.0;
            double xPx = w * _vPct / 100.0;

            _hLine.X1 = 0;  _hLine.X2 = w;
            _hLine.Y1 = yPx; _hLine.Y2 = yPx;

            _vLine.Y1 = 0;  _vLine.Y2 = h;
            _vLine.X1 = xPx; _vLine.X2 = xPx;

            if (TxtHPct != null) TxtHPct.Text = _hPct.ToString("0.0", CultureInfo.InvariantCulture) + "%";
            if (TxtVPct != null) TxtVPct.Text = _vPct.ToString("0.0", CultureInfo.InvariantCulture) + "%";
        }

        private double ReadStep()
        {
            if (TxtStep == null) return 1.0;
            if (!double.TryParse(TxtStep.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var s)
                && !double.TryParse(TxtStep.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out s))
                s = 1.0;
            return Math.Max(0.1, Math.Min(20.0, s));
        }

        private static double Clamp(double v, double lo, double hi)
            => v < lo ? lo : (v > hi ? hi : v);

        private void BtnHUp_Click(object sender, RoutedEventArgs e)
        {
            _hPct = Clamp(_hPct - ReadStep(), 0, 100);
            UpdateCrosshair();
            PersistSelections();
        }

        private void BtnHDown_Click(object sender, RoutedEventArgs e)
        {
            _hPct = Clamp(_hPct + ReadStep(), 0, 100);
            UpdateCrosshair();
            PersistSelections();
        }

        private void BtnVLeft_Click(object sender, RoutedEventArgs e)
        {
            _vPct = Clamp(_vPct - ReadStep(), 0, 100);
            UpdateCrosshair();
            PersistSelections();
        }

        private void BtnVRight_Click(object sender, RoutedEventArgs e)
        {
            _vPct = Clamp(_vPct + ReadStep(), 0, 100);
            UpdateCrosshair();
            PersistSelections();
        }

        private void BtnCenterCross_Click(object sender, RoutedEventArgs e)
        {
            _hPct = 50; _vPct = 50;
            UpdateCrosshair();
            PersistSelections();
        }

        private void TxtStep_TextChanged(object sender, TextChangedEventArgs e)
        {
            PersistSelections();
        }
    }

    public sealed class TemplatePoint
    {
        public int No { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double U { get; set; }
        public double V { get; set; }
        public string Remark { get; set; }
    }

    public sealed class TestResult
    {
        public int No { get; set; }
        public double? L { get; set; }
        public double? Cx { get; set; }
        public double? Cy { get; set; }
        public double? CCT { get; set; }
        public string Status { get; set; }
    }
}
