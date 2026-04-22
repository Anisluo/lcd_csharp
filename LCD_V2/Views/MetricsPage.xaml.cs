using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LCD_V2.Views
{
    public partial class MetricsPage : UserControl
    {
        private bool _loaded;
        private InstrumentMetric _editing;
        private bool _suppressSync;
        private bool _suppressInstrumentSave; // guard while loading instrument fields

        private ObservableCollection<InstrumentMetric> _library => MetricStore.Library;

        // params bound to the DataGrid — a fresh editable list each time a metric is loaded
        private ObservableCollection<MetricParam> _editorParams = new ObservableCollection<MetricParam>();

        public MetricsPage()
        {
            InitializeComponent();

            // combo + combo-column choices are static
            CmbInstrument.ItemsSource = MetricCatalog.Instruments;
            AlgoCol.ItemsSource       = MetricCatalog.Algorithms;
            CmbInstrument.SelectionChanged += CmbInstrument_SelectionChanged;

            // library view with client-side filter
            var view = CollectionViewSource.GetDefaultView(_library);
            view.Filter = LibraryFilter;
            LibraryList.ItemsSource = view;

            ParamsGrid.ItemsSource = _editorParams;

            Loaded += (s, e) =>
            {
                _loaded = true;
                _editing = null;
                LibraryList.SelectedIndex = -1;
                SeedEmptyEditor();
                // SeedEmptyEditor may set SelectedIndex to the same value it already had,
                // in which case SelectionChanged won't fire — force a config load here.
                LoadInstrumentConfig(CmbInstrument.SelectedItem as string);
            };
        }

        // ========== instrument config: image + params ==========

        private void CmbInstrument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadInstrumentConfig(CmbInstrument.SelectedItem as string);
        }

        private void LoadInstrumentConfig(string key)
        {
            var info = InstrumentCatalog.Find(key);
            if (info == null) return;

            // image + display
            InstrumentDisplayName.Text = info.DisplayName;
            InstrumentKey.Text         = info.Key;
            if (!string.IsNullOrEmpty(info.ImageUri))
            {
                try
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource   = new Uri(info.ImageUri, UriKind.Absolute);
                    bmp.EndInit(); bmp.Freeze();
                    InstrumentImage.Source = bmp;
                    InstrumentImagePlaceholder.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    InstrumentImage.Source = null;
                    InstrumentImagePlaceholder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                InstrumentImage.Source = null;
                InstrumentImagePlaceholder.Visibility = Visibility.Visible;
            }

            // show/hide sections per instrument
            IntegrationPanel.Visibility = info.UsesIntegration ? Visibility.Visible : Visibility.Collapsed;
            SerialPanel.Visibility      = info.UsesSerial      ? Visibility.Visible : Visibility.Collapsed;

            // populate fields
            var cfg = InstrumentConfigStore.Get(info.Key);
            _suppressInstrumentSave = true;
            TxtCorrLum.Text      = cfg.CorrLum.ToString("0.###", CultureInfo.InvariantCulture);
            TxtCorrCx.Text       = cfg.CorrCx.ToString("0.####", CultureInfo.InvariantCulture);
            TxtCorrCy.Text       = cfg.CorrCy.ToString("0.####", CultureInfo.InvariantCulture);
            TxtStartDelay.Text   = cfg.StartDelayMs.ToString();
            TxtMeasureDelay.Text = cfg.MeasureDelayMs.ToString();
            TxtResetDelay.Text   = cfg.ResetDelayMs.ToString();
            TxtIntegration.Text  = cfg.IntegrationMs.ToString();
            TxtComPort.Text      = cfg.ComPort ?? "";
            CmbBaud.SelectedItem     = PickItem(CmbBaud,     cfg.BaudRate.ToString());
            CmbDataBits.SelectedItem = PickItem(CmbDataBits, cfg.DataBits.ToString());
            CmbStopBits.SelectedItem = PickItem(CmbStopBits, cfg.StopBits.ToString());
            CmbParity.SelectedItem   = PickItem(CmbParity,   cfg.Parity ?? "None");
            _suppressInstrumentSave = false;
        }

        private static ComboBoxItem PickItem(ComboBox cb, string text)
        {
            foreach (var o in cb.Items)
                if (o is ComboBoxItem it && string.Equals(it.Content as string, text, StringComparison.Ordinal))
                    return it;
            return cb.Items.Count > 0 ? cb.Items[0] as ComboBoxItem : null;
        }

        private void InstrumentField_Changed(object sender, TextChangedEventArgs e)
        {
            SaveInstrumentConfig();
        }

        private void InstrumentCombo_Changed(object sender, SelectionChangedEventArgs e)
        {
            SaveInstrumentConfig();
        }

        private void SaveInstrumentConfig()
        {
            if (!_loaded || _suppressInstrumentSave) return;
            var key = CmbInstrument.SelectedItem as string;
            if (string.IsNullOrEmpty(key)) return;

            var cfg = new InstrumentConfig
            {
                Instrument     = key,
                CorrLum        = ParseD(TxtCorrLum.Text, 1.0),
                CorrCx         = ParseD(TxtCorrCx.Text, 0.0),
                CorrCy         = ParseD(TxtCorrCy.Text, 0.0),
                StartDelayMs   = ParseI(TxtStartDelay.Text, 2000),
                MeasureDelayMs = ParseI(TxtMeasureDelay.Text, 500),
                ResetDelayMs   = ParseI(TxtResetDelay.Text, 1000),
                IntegrationMs  = ParseI(TxtIntegration.Text, 100),
                ComPort        = (TxtComPort.Text ?? "").Trim(),
                BaudRate       = ParseI((CmbBaud.SelectedItem     as ComboBoxItem)?.Content as string, 9600),
                DataBits       = ParseI((CmbDataBits.SelectedItem as ComboBoxItem)?.Content as string, 8),
                StopBits       = ParseI((CmbStopBits.SelectedItem as ComboBoxItem)?.Content as string, 1),
                Parity         = (CmbParity.SelectedItem   as ComboBoxItem)?.Content as string ?? "None",
            };
            InstrumentConfigStore.Put(cfg);
        }

        private static double ParseD(string s, double def)
        {
            if (string.IsNullOrWhiteSpace(s)) return def;
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)
                || double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out v)
                ? v : def;
        }
        private static int ParseI(string s, int def)
        {
            if (string.IsNullOrWhiteSpace(s)) return def;
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : def;
        }

        // ========== library filter (search box) ==========

        private string _searchTerm = "";

        private bool LibraryFilter(object item)
        {
            if (string.IsNullOrEmpty(_searchTerm)) return true;
            if (!(item is InstrumentMetric m)) return false;
            return (m.Name != null       && m.Name.IndexOf(_searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                || (m.Instrument != null && m.Instrument.IndexOf(_searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == "搜索名称 / 仪器…")
            {
                TxtSearch.Text = "";
                TxtSearch.Foreground = (System.Windows.Media.Brush)FindResource("TextBrush");
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTerm = TxtSearch.Text == "搜索名称 / 仪器…" ? "" : TxtSearch.Text ?? "";
            CollectionViewSource.GetDefaultView(_library).Refresh();
        }

        // ========== library selection ==========

        private void Library_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_loaded || _suppressSync) return;
            if (!(LibraryList.SelectedItem is InstrumentMetric m)) return;
            _editing = m;

            _suppressSync = true;
            TxtName.Text = m.Name;
            int idx = Array.IndexOf(MetricCatalog.Instruments, m.Instrument);
            int wanted = idx >= 0 ? idx : 0;
            if (CmbInstrument.SelectedIndex == wanted)
                LoadInstrumentConfig(CmbInstrument.SelectedItem as string); // force reload
            else
                CmbInstrument.SelectedIndex = wanted; // SelectionChanged fires → loads

            _editorParams.Clear();
            foreach (var p in BuildFullParamList(m)) _editorParams.Add(p);
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
            var updated = new InstrumentMetric
            {
                Name       = string.IsNullOrWhiteSpace(TxtName.Text) ? "未命名" : TxtName.Text.Trim(),
                Instrument = CmbInstrument.SelectedItem as string ?? MetricCatalog.Instruments[0],
                Params     = _editorParams
                             .Where(p => p.Enabled)
                             .Select(p => new MetricParam
                             {
                                 ParamName = p.ParamName,
                                 Enabled   = true,
                                 Algorithm = string.IsNullOrEmpty(p.Algorithm) ? "均值" : p.Algorithm,
                             })
                             .ToList(),
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
                $"{action}指标配置：{updated.Name}（启用 {updated.Params.Count} 参数）",
                "保存", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(((FrameworkElement)sender).Tag is InstrumentMetric item)) return;
            if (MessageBox.Show(Window.GetWindow(this),
                    $"确认删除指标配置 \"{item.Name}\" ？", "删除",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            _library.Remove(item);
            if (_editing == item)
            {
                _editing = null;
                SeedEmptyEditor();
            }
        }

        // ========== param table helpers ==========

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ApplyEnabled(true);
        }

        private void BtnSelectNone_Click(object sender, RoutedEventArgs e)
        {
            ApplyEnabled(false);
        }

        private void ApplyEnabled(bool value)
        {
            // Must rebuild collection items because MetricParam isn't INotifyPropertyChanged;
            // mutating in place won't repaint the DataGrid cells.
            var snapshot = _editorParams.Select(p => new MetricParam
            {
                ParamName = p.ParamName,
                Enabled   = value,
                Algorithm = p.Algorithm,
            }).ToList();
            _editorParams.Clear();
            foreach (var p in snapshot) _editorParams.Add(p);
        }

        private void SeedEmptyEditor()
        {
            _suppressSync = true;
            TxtName.Text = "新指标";
            CmbInstrument.SelectedIndex = 0;
            _editorParams.Clear();
            foreach (var p in BuildFullParamList(null)) _editorParams.Add(p);
            _suppressSync = false;
        }

        /// <summary>
        /// Always returns the full catalog of params in canonical order. If an existing
        /// metric is passed, the matching params are pre-checked with their saved algorithm;
        /// the rest are disabled with a sensible default algorithm.
        /// </summary>
        private static IEnumerable<MetricParam> BuildFullParamList(InstrumentMetric seed)
        {
            var saved = seed?.Params?.ToDictionary(p => p.ParamName, p => p) ?? new Dictionary<string, MetricParam>();
            foreach (var name in MetricCatalog.AllParams)
            {
                if (saved.TryGetValue(name, out var existing))
                {
                    yield return new MetricParam
                    {
                        ParamName = name,
                        Enabled   = existing.Enabled,
                        Algorithm = existing.Algorithm ?? "均值",
                    };
                }
                else
                {
                    yield return new MetricParam
                    {
                        ParamName = name,
                        Enabled   = false,
                        Algorithm = "均值",
                    };
                }
            }
        }
    }
}
