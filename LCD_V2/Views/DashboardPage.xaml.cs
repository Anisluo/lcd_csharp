using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

        public DashboardPage()
        {
            InitializeComponent();

            // Mock "当前测试" session: 13 点对角模板, simulated progress 到第 4 点
            var templatePoints = new List<TemplatePoint>
            {
                new TemplatePoint { No = 1,  X = 0.00,  Y = 0.00,  Z = 0.00, U = 0.00, V = 0.00, Remark = "中心"   },
                new TemplatePoint { No = 2,  X = -57.15, Y = -34.29, Z = 0.00, U = 0.00, V = 0.00, Remark = "左上"   },
                new TemplatePoint { No = 3,  X = 57.15,  Y = -34.29, Z = 0.00, U = 0.00, V = 0.00, Remark = "右上"   },
                new TemplatePoint { No = 4,  X = 57.15,  Y = 34.29,  Z = 0.00, U = 0.00, V = 0.00, Remark = "右下"   },
                new TemplatePoint { No = 5,  X = -57.15, Y = 34.29,  Z = 0.00, U = 0.00, V = 0.00, Remark = "左下"   },
                new TemplatePoint { No = 6,  X = -28.58, Y = -17.15, Z = 0.00, U = 0.00, V = 0.00, Remark = "左上内" },
                new TemplatePoint { No = 7,  X = 28.58,  Y = -17.15, Z = 0.00, U = 0.00, V = 0.00, Remark = "右上内" },
                new TemplatePoint { No = 8,  X = 28.58,  Y = 17.15,  Z = 0.00, U = 0.00, V = 0.00, Remark = "右下内" },
                new TemplatePoint { No = 9,  X = -28.58, Y = 17.15,  Z = 0.00, U = 0.00, V = 0.00, Remark = "左下内" },
                new TemplatePoint { No = 10, X = 0.00,  Y = -34.29, Z = 0.00, U = 0.00, V = 0.00, Remark = "上中"   },
                new TemplatePoint { No = 11, X = 57.15,  Y = 0.00,  Z = 0.00, U = 0.00, V = 0.00, Remark = "右中"   },
                new TemplatePoint { No = 12, X = 0.00,  Y = 34.29,  Z = 0.00, U = 0.00, V = 0.00, Remark = "下中"   },
                new TemplatePoint { No = 13, X = -57.15, Y = 0.00,  Z = 0.00, U = 0.00, V = 0.00, Remark = "左中"   },
            };
            TemplateGrid.ItemsSource = templatePoints;
            // 光标停在第 4 点：前 3 点已完成并显示结果, 第 4 点正在测量
            TemplateGrid.SelectedIndex = 3;
            TemplateGrid.ScrollIntoView(templatePoints[3]);

            var results = new List<TestResult>
            {
                new TestResult { No = 1, L = 315.4, Cx = 0.3127, Cy = 0.3291, CCT = 6503, Status = "OK" },
                new TestResult { No = 2, L = 298.7, Cx = 0.3133, Cy = 0.3284, CCT = 6478, Status = "OK" },
                new TestResult { No = 3, L = 306.2, Cx = 0.3124, Cy = 0.3298, CCT = 6521, Status = "OK" },
                new TestResult { No = 4, L = null,  Cx = null,   Cy = null,   CCT = null, Status = "测量中" },
            };
            ResultGrid.ItemsSource = results;
            ResultGrid.SelectedIndex = 3;
            ResultGrid.ScrollIntoView(results[3]);
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
        }

        private void BtnHDown_Click(object sender, RoutedEventArgs e)
        {
            _hPct = Clamp(_hPct + ReadStep(), 0, 100);
            UpdateCrosshair();
        }

        private void BtnVLeft_Click(object sender, RoutedEventArgs e)
        {
            _vPct = Clamp(_vPct - ReadStep(), 0, 100);
            UpdateCrosshair();
        }

        private void BtnVRight_Click(object sender, RoutedEventArgs e)
        {
            _vPct = Clamp(_vPct + ReadStep(), 0, 100);
            UpdateCrosshair();
        }

        private void BtnCenterCross_Click(object sender, RoutedEventArgs e)
        {
            _hPct = 50; _vPct = 50;
            UpdateCrosshair();
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
