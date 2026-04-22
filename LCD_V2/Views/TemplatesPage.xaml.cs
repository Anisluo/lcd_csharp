using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LCD.Core.Services;

namespace LCD_V2.Views
{
    public partial class TemplatesPage : UserControl
    {
        private bool _loaded;
        private readonly ObservableCollection<TemplateItem> _library = new ObservableCollection<TemplateItem>();
        private TemplateItem _editing; // the library row currently being edited, or null for a fresh one
        private bool _suppressSync;    // block Regenerate → write-back loops

        public TemplatesPage()
        {
            InitializeComponent();

            // seed a few example templates so the library isn't empty on first launch
            _library.Add(new TemplateItem { Name = "13 寸屏 · 对角",  ConfigType = PointLayoutType.Point13Diag, H = 286, V = 179, A = 10, B = 10, C = 25, D = 25 });
            _library.Add(new TemplateItem { Name = "中控屏 9 点", ConfigType = PointLayoutType.Point9,      H = 250, V = 150, A = 10, B = 10, C = 25, D = 25 });

            LibraryList.ItemsSource = _library;

            Loaded += (s, e) =>
            {
                _loaded = true;
                // start in "new" mode with 13对角 defaults
                _editing = null;
                LibraryList.SelectedIndex = -1;
                Regenerate();
            };
        }

        /// <summary>Reference diagram for each generator config, shipped as embedded resource.</summary>
        private static readonly Dictionary<PointLayoutType, string> PresetImages
            = new Dictionary<PointLayoutType, string>
        {
            { PointLayoutType.Point5,      "pack://application:,,,/Image/5Points.jpg"       },
            { PointLayoutType.Point9,      "pack://application:,,,/Image/9Points.jpg"       },
            { PointLayoutType.Point13,     "pack://application:,,,/Image/13Points.jpg"      },
            { PointLayoutType.Point13Diag, "pack://application:,,,/Image/13Points_new.jpg"  },
            { PointLayoutType.Point17,     "pack://application:,,,/Image/std17poi.jpeg"     },
        };

        /// <summary>Cache decoded BitmapImages so that switching configs is free after first load.</summary>
        private static readonly Dictionary<PointLayoutType, BitmapImage> _imageCache
            = new Dictionary<PointLayoutType, BitmapImage>();

        /// <summary>The image currently displayed. Re-used when the config doesn't change.</summary>
        private PointLayoutType? _currentImageType;

        // ========== click handlers ==========

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            _editing = null;
            LibraryList.SelectedIndex = -1;
            _suppressSync = true;
            TxtName.Text = "新模板";
            CmbType.SelectedIndex = 3; // 13 对角
            TxtH.Text = "300.00";
            TxtV.Text = "200.00";
            TxtA.Text = "10"; TxtB.Text = "10";
            TxtC.Text = "25"; TxtD.Text = "25";
            RadioPct.IsChecked = true;
            _suppressSync = false;
            Regenerate();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e) => BtnNew_Click(sender, e);

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Always allocate a fresh instance so ObservableCollection.Replace sees a
            // different reference. Mutating the existing item in place wouldn't repaint
            // the ListBox row (TemplateItem is not INotifyPropertyChanged).
            var updated = new TemplateItem();
            FillFromForm(updated);
            updated.PointCount = PointsGrid.Items.Count;

            if (_editing == null)
            {
                _library.Add(updated);
            }
            else
            {
                int idx = _library.IndexOf(_editing);
                if (idx >= 0) _library[idx] = updated;
                else          _library.Add(updated);
            }

            _suppressSync = true; // selection change will re-invoke Library_SelectionChanged
            _editing = updated;
            LibraryList.SelectedItem = updated;
            _suppressSync = false;

            MessageBox.Show(Window.GetWindow(this),
                $"已保存到模板库：{updated.Name}（{updated.PointCount} 个点位）",
                "保存模板", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(((FrameworkElement)sender).Tag is TemplateItem item)) return;
            var result = MessageBox.Show(Window.GetWindow(this),
                $"确认删除模板 “{item.Name}” ？", "删除模板",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            _library.Remove(item);
            if (_editing == item)
            {
                _editing = null;
                BtnNew_Click(this, null);
            }
        }

        private void Library_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(LibraryList.SelectedItem is TemplateItem item)) return;
            _editing = item;
            _suppressSync = true;
            TxtName.Text = item.Name;
            CmbType.SelectedIndex = ConfigIndex(item.ConfigType);
            TxtH.Text = item.H.ToString("0.00", CultureInfo.InvariantCulture);
            TxtV.Text = item.V.ToString("0.00", CultureInfo.InvariantCulture);
            TxtA.Text = item.A.ToString(CultureInfo.InvariantCulture);
            TxtB.Text = item.B.ToString(CultureInfo.InvariantCulture);
            TxtC.Text = item.C.ToString(CultureInfo.InvariantCulture);
            TxtD.Text = item.D.ToString(CultureInfo.InvariantCulture);
            if (item.UseMm) RadioMm.IsChecked = true; else RadioPct.IsChecked = true;
            _suppressSync = false;
            Regenerate();
        }

        private void AnyParam_Changed(object sender, TextChangedEventArgs e) => Regenerate();
        private void Unit_Changed(object sender, RoutedEventArgs e)           => Regenerate();
        private void Type_Changed(object sender, SelectionChangedEventArgs e) => Regenerate();

        // ========== core generation ==========

        private void Regenerate()
        {
            if (!_loaded || PointsGrid == null || _suppressSync) return;

            var type = SelectedType();
            var input = BuildInput();
            var pts = PointLayoutService.Generate(type, input);
            PointsGrid.ItemsSource = pts;
            TxtCount.Text = pts.Count + " 个";
            TxtDims.Text  = $"{input.H:0}×{input.V:0} mm";
            TxtHint.Text  = HintFor(type);

            LoadPresetImage(type);
            DrawSchematic(pts, input.H, input.V);
            HideCallout();
        }

        // ========== point-layout schematic ==========

        private readonly Dictionary<int, Ellipse> _dotByPointId = new Dictionary<int, Ellipse>();

        private static readonly SolidColorBrush DotFillDefault  = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF)); // gray
        private static readonly SolidColorBrush DotFillSelected = new SolidColorBrush(Color.FromRgb(0x63, 0x66, 0xF1)); // accent
        private static readonly SolidColorBrush ProductFill     = Brushes.White;
        private static readonly SolidColorBrush ProductStroke   = new SolidColorBrush(Color.FromRgb(0xD1, 0xD5, 0xDB));
        private static readonly SolidColorBrush GridlineStroke  = new SolidColorBrush(Color.FromArgb(0x60, 0xE5, 0xE7, 0xEB));

        private void DrawSchematic(IReadOnlyList<PointPos> pts, double h, double v)
        {
            _dotByPointId.Clear();
            SchematicCanvas.Children.Clear();
            if (h <= 0 || v <= 0) return;

            SchematicCanvas.Width  = h;
            SchematicCanvas.Height = v;

            // gridlines at 25/50/75%
            for (int i = 1; i <= 3; i++)
            {
                var vx = new Line
                {
                    X1 = h * i / 4, Y1 = 0, X2 = h * i / 4, Y2 = v,
                    Stroke = GridlineStroke, StrokeThickness = 0.4,
                };
                var hy = new Line
                {
                    X1 = 0, Y1 = v * i / 4, X2 = h, Y2 = v * i / 4,
                    Stroke = GridlineStroke, StrokeThickness = 0.4,
                };
                SchematicCanvas.Children.Add(vx);
                SchematicCanvas.Children.Add(hy);
            }

            // product boundary
            var rect = new Rectangle
            {
                Width  = h,
                Height = v,
                Fill   = ProductFill,
                Stroke = ProductStroke,
                StrokeThickness = 1.0,
            };
            SchematicCanvas.Children.Add(rect);

            // points
            double dotR   = Math.Max(3.0, Math.Min(h, v) * 0.018);  // scales with product size
            double fontPt = Math.Max(2.0, dotR * 1.3);
            foreach (var p in pts)
            {
                var dot = new Ellipse
                {
                    Width           = dotR * 2,
                    Height          = dotR * 2,
                    Fill            = DotFillDefault,
                    Stroke          = Brushes.White,
                    StrokeThickness = 0.6,
                    Cursor          = Cursors.Hand,
                    Tag             = p,
                    ToolTip         = $"#{p.Id}\nX = {p.XMm:0.00} mm  ({p.XPct:0.0}%)\nY = {p.YMm:0.00} mm  ({p.YPct:0.0}%)",
                };
                Canvas.SetLeft(dot, p.XMm - dotR);
                Canvas.SetTop(dot,  p.YMm - dotR);
                dot.MouseLeftButtonDown += Dot_MouseLeftButtonDown;
                SchematicCanvas.Children.Add(dot);
                _dotByPointId[p.Id] = dot;

                // numeric label next to the dot
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
                SchematicCanvas.Children.Add(label);
            }
        }

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse dot && dot.Tag is PointPos p)
            {
                PointsGrid.SelectedItem = p;
                PointsGrid.ScrollIntoView(p);
                e.Handled = true;
            }
        }

        private void PointsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var p = PointsGrid.SelectedItem as PointPos;
            HighlightDot(p);
            if (p == null) HideCallout(); else ShowCallout(p);
        }

        private void HighlightDot(PointPos selected)
        {
            foreach (var kv in _dotByPointId)
            {
                var isSel = selected != null && kv.Key == selected.Id;
                kv.Value.Fill = isSel ? DotFillSelected : DotFillDefault;
                kv.Value.StrokeThickness = isSel ? 1.2 : 0.6;
            }
        }

        private void ShowCallout(PointPos p)
        {
            CalloutTitle.Text = $"#{p.Id}";
            CalloutBody.Text  = $"X = {p.XMm:0.00} mm  ({p.XPct:0.0}%)\nY = {p.YMm:0.00} mm  ({p.YPct:0.0}%)";
            CalloutBox.Visibility = Visibility.Visible;
        }

        private void HideCallout()
        {
            if (CalloutBox != null) CalloutBox.Visibility = Visibility.Collapsed;
        }

        private PointLayoutInput BuildInput()
        {
            double h = ParseD(TxtH.Text, 300);
            double v = ParseD(TxtV.Text, 200);
            double a = ParseD(TxtA.Text, 10);
            double b = ParseD(TxtB.Text, 10);
            double c = ParseD(TxtC.Text, 25);
            double d = ParseD(TxtD.Text, 25);
            bool useMm = RadioMm != null && RadioMm.IsChecked == true;

            var input = new PointLayoutInput { H = h, V = v, UseMeter = useMm };
            if (useMm) { input.Amm = a; input.Bmm = b; input.Cmm = c; input.Dmm = d; }
            else       { input.Apct = a; input.Bpct = b; input.Cpct = c; input.Dpct = d; }
            return input;
        }

        private void FillFromForm(TemplateItem item)
        {
            item.Name       = TxtName.Text;
            item.ConfigType = SelectedType();
            item.H          = ParseD(TxtH.Text, 300);
            item.V          = ParseD(TxtV.Text, 200);
            item.A          = ParseD(TxtA.Text, 10);
            item.B          = ParseD(TxtB.Text, 10);
            item.C          = ParseD(TxtC.Text, 25);
            item.D          = ParseD(TxtD.Text, 25);
            item.UseMm      = RadioMm.IsChecked == true;
        }

        private PointLayoutType SelectedType()
        {
            switch (CmbType.SelectedIndex)
            {
                case 0: return PointLayoutType.Point5;
                case 1: return PointLayoutType.Point9;
                case 2: return PointLayoutType.Point13;
                case 3: return PointLayoutType.Point13Diag;
                case 4: return PointLayoutType.Point17;
                default: return PointLayoutType.Point13Diag;
            }
        }

        private static int ConfigIndex(PointLayoutType t)
        {
            switch (t)
            {
                case PointLayoutType.Point5:      return 0;
                case PointLayoutType.Point9:      return 1;
                case PointLayoutType.Point13:     return 2;
                case PointLayoutType.Point13Diag: return 3;
                case PointLayoutType.Point17:     return 4;
                default:                           return 3;
            }
        }

        private void LoadPresetImage(PointLayoutType type)
        {
            // No-op if we're already showing this type's image — avoids hot-path IO when
            // Regenerate() runs on every TextChanged (Chinese IME makes this critical).
            if (_currentImageType == type) return;

            if (!PresetImages.TryGetValue(type, out var uri))
            {
                RefImage.Source = null;
                RefPlaceholder.Visibility = Visibility.Visible;
                _currentImageType = type;
                return;
            }

            if (!_imageCache.TryGetValue(type, out var bmp))
            {
                try
                {
                    bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource   = new Uri(uri, UriKind.Absolute);
                    bmp.EndInit();
                    bmp.Freeze();
                    _imageCache[type] = bmp;
                }
                catch
                {
                    RefImage.Source = null;
                    RefPlaceholder.Visibility = Visibility.Visible;
                    _currentImageType = type;
                    return;
                }
            }

            RefImage.Source = bmp;
            RefPlaceholder.Visibility = Visibility.Collapsed;
            _currentImageType = type;
        }

        private static double ParseD(string s, double fallback)
        {
            if (string.IsNullOrWhiteSpace(s)) return fallback;
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)
                || double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out v)
                ? v : fallback;
        }

        private static string HintFor(PointLayoutType type)
        {
            switch (type)
            {
                case PointLayoutType.Point5:
                    return "5 点：中心 + 四角。仅使用 A (X 边距) 和 B (Y 边距)。";
                case PointLayoutType.Point9:
                    return "9 点：3×3 均布。仅使用 A / B，C / D 不参与计算。";
                case PointLayoutType.Point13:
                    return "13 点：外框 + 内环。A / B 定义外边距，C / D 定义内环位置。";
                case PointLayoutType.Point13Diag:
                    return "13 点对角：X + 内菱形。使用固定 10% 网格，A / B / C / D 不参与计算。";
                case PointLayoutType.Point17:
                    return "17 点：外框 + 中线 + 内环。A / B 外边距，C / D 内环位置。";
                default:
                    return "";
            }
        }
    }

    /// <summary>
    /// A saved template instance in the library — the *result* of applying a
    /// generator config (5/9/13/...) to concrete product dimensions + params.
    /// </summary>
    public sealed class TemplateItem
    {
        public string Name { get; set; } = "未命名";
        public PointLayoutType ConfigType { get; set; }
        public double H { get; set; }
        public double V { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }
        public bool   UseMm { get; set; }
        public int    PointCount { get; set; }

        public string ConfigTypeLabel
        {
            get
            {
                switch (ConfigType)
                {
                    case PointLayoutType.Point5:      return "5 点";
                    case PointLayoutType.Point9:      return "9 点";
                    case PointLayoutType.Point13:     return "13 点";
                    case PointLayoutType.Point13Diag: return "13 点对角";
                    case PointLayoutType.Point17:     return "17 点";
                    default:                           return ConfigType.ToString();
                }
            }
        }

        public string Summary => $"{ConfigTypeLabel} · {H:0}×{V:0} mm";
    }
}
