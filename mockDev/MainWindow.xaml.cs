using System;
using System.Windows;
using MockDev.Instruments;

namespace MockDev
{
    public partial class MainWindow : Window
    {
        private readonly Com0Com _com0com = new Com0Com();
        private SerialSimulator _sim;
        private int _pair = -1;   // 本次创建的 com0com pair 号，用于退出时删除

        public MainWindow()
        {
            InitializeComponent();

            // 注册可模拟的仪器（新增仪器：在此 Add 一个实例即可）
            InstrumentBox.Items.Add(new SR3AInstrument());
            InstrumentBox.SelectedIndex = 0;

            // 自动探测 com0com
            var path = Com0Com.AutoFindSetupc();
            if (path != null) { SetupcBox.Text = path; Log("INFO", "已找到 com0com: " + path); }
            else Log("INFO", "未找到 com0com，请安装后点“自动检测”或“浏览”指定 setupc.exe。下载: https://sourceforge.net/projects/com0com/ (用已签名的 3.0.0.0 版本)");
        }

        private void DetectBtn_Click(object sender, RoutedEventArgs e)
        {
            var path = Com0Com.AutoFindSetupc();
            if (path != null) { SetupcBox.Text = path; Status("已找到 setupc: " + path); }
            else Status("未找到 setupc.exe，请手动浏览指定。");
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "setupc.exe|setupc.exe|所有文件|*.*" };
            if (dlg.ShowDialog() == true) SetupcBox.Text = dlg.FileName;
        }

        private void InstrumentBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (InstrumentBox.SelectedItem is IInstrument inst) BaudBox.Text = inst.BaudRate.ToString();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SetupcBox.Text) || !System.IO.File.Exists(SetupcBox.Text))
            { Status("请先指定有效的 com0com setupc.exe"); return; }
            var inst = InstrumentBox.SelectedItem as IInstrument;
            if (inst == null) { Status("请选择一个仪器"); return; }
            string lcdPort = LcdPortBox.Text.Trim();
            string mockPort = MockPortBox.Text.Trim();
            if (!int.TryParse(BaudBox.Text.Trim(), out int baud)) baud = 9600;

            try
            {
                _com0com.SetupcPath = SetupcBox.Text;
                Log("INFO", $"创建虚拟串口对 {lcdPort} ↔ {mockPort} …");
                _pair = _com0com.CreatePair(lcdPort, mockPort, out string outp);
                Log("INFO", outp);
                if (_pair < 0)
                {
                    Status("创建虚拟串口对失败，请看日志（端口可能被占用或 com0com 未正确安装）。");
                    return;
                }
                Log("INFO", $"虚拟串口对已创建 (pair {_pair})。");

                _sim = new SerialSimulator(inst);
                _sim.Log += Log;
                _sim.Start(mockPort, baud);

                StartBtn.IsEnabled = false;
                StopBtn.IsEnabled = true;
                SetupcBox.IsEnabled = LcdPortBox.IsEnabled = MockPortBox.IsEnabled = InstrumentBox.IsEnabled = false;
                Status($"运行中：LCD 连 {lcdPort}，mockDev 在 {mockPort} 模拟 {inst.Name}。");
            }
            catch (Exception ex)
            {
                Log("INFO", "启动失败: " + ex.Message);
                Status("启动失败: " + ex.Message);
                Cleanup();
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Cleanup();
            StartBtn.IsEnabled = true;
            StopBtn.IsEnabled = false;
            SetupcBox.IsEnabled = LcdPortBox.IsEnabled = MockPortBox.IsEnabled = InstrumentBox.IsEnabled = true;
            Status("已停止，虚拟串口对已删除。");
        }

        /// <summary>停止模拟并删除创建的虚拟串口对。</summary>
        private void Cleanup()
        {
            try { _sim?.Stop(); } catch { }
            _sim = null;
            if (_pair >= 0)
            {
                try
                {
                    Log("INFO", $"删除虚拟串口对 (pair {_pair}) …");
                    string outp = _com0com.RemovePair(_pair, out int exit);
                    Log("INFO", outp);
                }
                catch (Exception ex) { Log("INFO", "删除虚拟串口对异常: " + ex.Message); }
                _pair = -1;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // 关闭 mockDev 时，一并删除创建的虚拟串口对
            Cleanup();
            base.OnClosing(e);
        }

        private void Status(string s)
        {
            Dispatcher.Invoke(() => StatusText.Text = s);
        }

        private void Log(string dir, string text)
        {
            if (text == null) return;
            string line = $"[{DateTime.Now:HH:mm:ss}] {dir,-4} {text.Replace("\r", "\\r").Replace("\n", "\\n")}";
            Dispatcher.Invoke(() =>
            {
                LogBox.AppendText(line + Environment.NewLine);
                LogBox.ScrollToEnd();
            });
        }
    }
}
