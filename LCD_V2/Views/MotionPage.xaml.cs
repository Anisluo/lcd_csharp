using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LCD_V2.Views
{
    public partial class MotionPage : UserControl
    {
        private bool _loaded;
        private MotionProfile _editing;
        private bool _suppressSync;

        private ObservableCollection<MotionProfile> _library => MotionStore.Library;

        // editable copy bound to the axes DataGrid
        private ObservableCollection<AxisConfig> _editorAxes = new ObservableCollection<AxisConfig>();

        public MotionPage()
        {
            InitializeComponent();

            CmbAlgorithm.ItemsSource = MotionCatalog.Algorithms;

            var view = CollectionViewSource.GetDefaultView(_library);
            view.Filter = LibraryFilter;
            LibraryList.ItemsSource = view;

            AxesGrid.ItemsSource = _editorAxes;

            Loaded += (s, e) =>
            {
                _loaded = true;
                _editing = null;
                LibraryList.SelectedIndex = -1;
                SeedEmptyEditor();
            };
        }

        // ========== search filter ==========

        private string _searchTerm = "";

        private bool LibraryFilter(object item)
        {
            if (string.IsNullOrEmpty(_searchTerm)) return true;
            if (!(item is MotionProfile m)) return false;
            return (m.Name != null      && m.Name.IndexOf(_searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                || (m.Algorithm != null && m.Algorithm.IndexOf(_searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == "搜索名称 / 算法…")
            {
                TxtSearch.Text = "";
                TxtSearch.Foreground = (System.Windows.Media.Brush)FindResource("TextBrush");
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTerm = TxtSearch.Text == "搜索名称 / 算法…" ? "" : TxtSearch.Text ?? "";
            CollectionViewSource.GetDefaultView(_library).Refresh();
        }

        // ========== library selection ==========

        private void Library_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_loaded || _suppressSync) return;
            if (!(LibraryList.SelectedItem is MotionProfile m)) return;
            _editing = m;

            _suppressSync = true;
            TxtName.Text = m.Name;
            int idx = Array.IndexOf(MotionCatalog.Algorithms, m.Algorithm);
            CmbAlgorithm.SelectedIndex = idx >= 0 ? idx : 0;

            _editorAxes.Clear();
            foreach (var a in CloneAxes(m.Axes ?? MotionCatalog.NewDefaultAxes())) _editorAxes.Add(a);
            _suppressSync = false;
        }

        // ========== save / new / reset / delete ==========

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            _editing = null;
            LibraryList.SelectedIndex = -1;
            SeedEmptyEditor();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e) => BtnNew_Click(sender, e);

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Commit any in-progress DataGrid edit so the last typed cell ends up in the bound object.
            AxesGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            AxesGrid.CommitEdit(DataGridEditingUnit.Row,  true);

            var updated = new MotionProfile
            {
                Name      = string.IsNullOrWhiteSpace(TxtName.Text) ? "未命名" : TxtName.Text.Trim(),
                Algorithm = CmbAlgorithm.SelectedItem as string ?? MotionCatalog.Algorithms[0],
                Axes      = CloneAxes(_editorAxes),
            };

            bool treatAsNew = _editing == null
                           || !string.Equals(updated.Name, _editing.Name, StringComparison.Ordinal);
            string action;
            if (treatAsNew)
            {
                _library.Add(updated);
                action = "已新建";
            }
            else
            {
                int idx = _library.IndexOf(_editing);
                if (idx >= 0) _library[idx] = updated; else _library.Add(updated);
                action = "已更新";
            }

            _suppressSync = true;
            _editing = updated;
            LibraryList.SelectedItem = updated;
            _suppressSync = false;

            MessageBox.Show(Window.GetWindow(this),
                $"{action}平台配置：{updated.Name}（{updated.EnabledAxisCount} 轴启用）",
                "保存", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(((FrameworkElement)sender).Tag is MotionProfile item)) return;
            if (MessageBox.Show(Window.GetWindow(this),
                    $"确认删除平台配置 \"{item.Name}\" ？", "删除",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            _library.Remove(item);
            if (_editing == item)
            {
                _editing = null;
                SeedEmptyEditor();
            }
        }

        // ========== helpers ==========

        private void SeedEmptyEditor()
        {
            _suppressSync = true;
            TxtName.Text = "新平台";
            CmbAlgorithm.SelectedIndex = 0;
            _editorAxes.Clear();
            foreach (var a in MotionCatalog.NewDefaultAxes()) _editorAxes.Add(a);
            _suppressSync = false;
        }

        private static List<AxisConfig> CloneAxes(IEnumerable<AxisConfig> src)
        {
            return src.Select(a => new AxisConfig
            {
                AxisName    = a.AxisName,
                Enabled     = a.Enabled,
                HighSpeed   = a.HighSpeed,
                MidSpeed    = a.MidSpeed,
                LowSpeed    = a.LowSpeed,
                AccelTimeMs = a.AccelTimeMs,
                PulseUnit   = a.PulseUnit,
                Invert      = a.Invert,
            }).ToList();
        }
    }
}
