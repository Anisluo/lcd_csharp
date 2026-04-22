using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using LCD.Core.Services;
using Microsoft.Win32;

namespace LCD_V2.Views
{
    public partial class TemplatesPage : UserControl
    {
        private bool _loaded;

        public TemplatesPage()
        {
            InitializeComponent();
            Loaded += (s, e) => { _loaded = true; Regenerate(); };
        }

        /// <summary>
        /// Preset reference image shipped with LCD_V2 for each point-layout type.
        /// User can still override via the "选择参考图片…" button (sets _userOverrodeImage = true).
        /// </summary>
        private static readonly System.Collections.Generic.Dictionary<PointLayoutType, string> PresetImages
            = new System.Collections.Generic.Dictionary<PointLayoutType, string>
        {
            { PointLayoutType.Point5,      "pack://application:,,,/Image/5Points.jpg"       },
            { PointLayoutType.Point9,      "pack://application:,,,/Image/9Points.jpg"       },
            { PointLayoutType.Point13,     "pack://application:,,,/Image/13Points.jpg"      },
            { PointLayoutType.Point13Diag, "pack://application:,,,/Image/13Points_new.jpg"  },
            { PointLayoutType.Point17,     "pack://application:,,,/Image/std17poi.jpeg"     },
        };

        private bool _userOverrodeImage;

        // === event handlers wired from XAML ===

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            // Reset to a fresh 13 点对角 template as "new"
            TxtName.Text = "新模板";
            CmbType.SelectedIndex = 3;
            TxtH.Text = "300.00";
            TxtV.Text = "200.00";
            TxtA.Text = "10";
            TxtB.Text = "10";
            TxtC.Text = "25";
            TxtD.Text = "25";
            RadioPct.IsChecked = true;
            _userOverrodeImage = false;
            Regenerate();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e) => BtnNew_Click(sender, e);

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                Window.GetWindow(this),
                $"模板 \"{TxtName.Text}\" 已保存（演示）。\n共 {PointsGrid.Items.Count} 个点位。",
                "保存模板",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnPickImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title  = "选择参考图片",
                Filter = "图片文件 (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|所有文件|*.*",
            };
            if (dlg.ShowDialog() != true) return;
            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource   = new Uri(dlg.FileName);
                bmp.EndInit();
                bmp.Freeze();
                RefImage.Source = bmp;
                RefPlaceholder.Visibility = Visibility.Collapsed;
                _userOverrodeImage = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Window.GetWindow(this), "加载图片失败：" + ex.Message,
                    "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AnyParam_Changed(object sender, TextChangedEventArgs e) => Regenerate();
        private void Unit_Changed(object sender, RoutedEventArgs e)           => Regenerate();
        private void Type_Changed(object sender, SelectionChangedEventArgs e) => Regenerate();

        // === the core: call PointLayoutService with current form values ===

        private void Regenerate()
        {
            if (!_loaded || PointsGrid == null) return;

            double h  = ParseD(TxtH.Text, 300);
            double v  = ParseD(TxtV.Text, 200);
            double a  = ParseD(TxtA.Text, 10);
            double b  = ParseD(TxtB.Text, 10);
            double c  = ParseD(TxtC.Text, 25);
            double d  = ParseD(TxtD.Text, 25);

            bool useMm = RadioMm != null && RadioMm.IsChecked == true;

            var input = new PointLayoutInput
            {
                H        = h,
                V        = v,
                UseMeter = useMm,
            };
            if (useMm)
            {
                input.Amm = a; input.Bmm = b; input.Cmm = c; input.Dmm = d;
            }
            else
            {
                input.Apct = a; input.Bpct = b; input.Cpct = c; input.Dpct = d;
            }

            PointLayoutType type;
            switch (CmbType.SelectedIndex)
            {
                case 0: type = PointLayoutType.Point5;      break;
                case 1: type = PointLayoutType.Point9;      break;
                case 2: type = PointLayoutType.Point13;     break;
                case 3: type = PointLayoutType.Point13Diag; break;
                case 4: type = PointLayoutType.Point17;     break;
                default: type = PointLayoutType.Point13Diag; break;
            }

            var pts = PointLayoutService.Generate(type, input);
            PointsGrid.ItemsSource = pts;
            TxtCount.Text = pts.Count + " 个";
            TxtHint.Text  = HintFor(type);

            // Auto-swap reference image to match the selected type, unless the user
            // has picked a custom one via the file dialog.
            if (!_userOverrodeImage) LoadPresetImage(type);
        }

        private void LoadPresetImage(PointLayoutType type)
        {
            if (!PresetImages.TryGetValue(type, out var uri))
            {
                RefImage.Source = null;
                RefPlaceholder.Visibility = Visibility.Visible;
                return;
            }
            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource   = new Uri(uri, UriKind.Absolute);
                bmp.EndInit();
                bmp.Freeze();
                RefImage.Source = bmp;
                RefPlaceholder.Visibility = Visibility.Collapsed;
            }
            catch
            {
                RefImage.Source = null;
                RefPlaceholder.Visibility = Visibility.Visible;
            }
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
}
