using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shell;
using LCD_V2.Controls;
using LCD_V2.Views;

namespace LCD_V2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var items = new List<NavItem>
            {
                new NavItem { Title = "仪表板",   Icon = (Geometry)FindResource("IconHome"),     PageType = typeof(DashboardPage) },
                new NavItem { Title = "模板管理", Icon = (Geometry)FindResource("IconGrid"),     PageType = typeof(TemplatesPage) },
                new NavItem { Title = "测试结果", Icon = (Geometry)FindResource("IconChart"),    PageType = typeof(ResultsPage)   },
                new NavItem { Title = "设备状态", Icon = (Geometry)FindResource("IconDevices"),  PageType = typeof(DevicesPage)   },
                new NavItem { Title = "运行日志", Icon = (Geometry)FindResource("IconLogs"),     PageType = typeof(LogsPage)      },
                new NavItem { Title = "系统设置", Icon = (Geometry)FindResource("IconSettings"), PageType = typeof(SettingsPage)  },
            };
            Nav.ItemsSource = items;
            Nav.SelectedIndex = 0;
        }

        private void Nav_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(Nav.SelectedItem is NavItem item)) return;
            PageHost.Content = Activator.CreateInstance(item.PageType);
            PageTitle.Text = item.Title;
            PageSubtitle.Text = SubtitleFor(item.Title);
        }

        // --- custom title bar ---

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => SystemCommands.MinimizeWindow(this);

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) SystemCommands.RestoreWindow(this);
            else                                       SystemCommands.MaximizeWindow(this);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => SystemCommands.CloseWindow(this);

        /// <summary>
        /// When a WindowStyle=None window maximises, WPF lets it extend past the work
        /// area by the resize-border thickness on every side. Compensate by padding
        /// the root grid with 7px so the content stays clear of the taskbar / monitor
        /// edge. Restore to 0 when back to normal.
        /// </summary>
        private void Root_StateChanged(object sender, EventArgs e)
        {
            RootGrid.Margin = WindowState == WindowState.Maximized
                ? new Thickness(7)
                : new Thickness(0);

            // Toggle the max/restore icon glyph
            if (MaxIcon != null)
            {
                MaxIcon.Data = WindowState == WindowState.Maximized
                    // restore: two overlapping rectangles
                    ? Geometry.Parse("M 0,2 L 8,2 L 8,10 L 0,10 Z M 2,0 L 10,0 L 10,8 L 8,8")
                    // maximize: single rectangle
                    : Geometry.Parse("M 0,0 L 10,0 L 10,10 L 0,10 Z");
            }
        }

        private static string SubtitleFor(string title)
        {
            switch (title)
            {
                case "仪表板":   return "实时运行状态 / 最近测试概览";
                case "模板管理": return "管理点阵模板、测试流程配置";
                case "测试结果": return "浏览、导出测试结果数据";
                case "设备状态": return "光度计、PG、电源、运动平台状态";
                case "运行日志": return "系统运行日志与调试信息";
                case "系统设置": return "通讯、设备、显示等全局设置";
                default:         return "";
            }
        }
    }

    /// <summary>
    /// Collapses a binding target when the source string is null or empty.
    /// </summary>
    public sealed class NullOrEmptyToCollapsed : IValueConverter
    {
        public static readonly NullOrEmptyToCollapsed Instance = new NullOrEmptyToCollapsed();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
