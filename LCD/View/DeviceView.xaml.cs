using LCD.Ctrl;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LCD.View
{
    /// <summary>
    /// 仪器设置界面
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class DeviceView : UserControl
    {
        /// <summary>
        /// 设备模型信息
        /// </summary>
        public List<MachinePluginInfo> Infos { get; set; } = new List<MachinePluginInfo>();

        public List<MachinePluginInfo> Infos2 { get; set; }

        /// <summary>
        /// 电压
        /// </summary>
        public double volt { get; set; }
        public string SendStr { get; set; }
        public string RecvStr { get; set; }

        /// <summary>
        /// 测试设备
        /// </summary>
        public TestMachine TestDevice { get; set; } = new TestMachine();
        public ObservableCollection<TestMachine> Devices { get; set; } = new ObservableCollection<TestMachine>();

        public DeviceView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            Infos = Managers.Manager_Plugins.MachineInfos;
            //Assembly ab = Assembly.Load(File.ReadAllBytes(""));
        }

        private void ReConnect_Click(object sender, RoutedEventArgs e)
        {
            TestDevice.Init();
        }

        private void ReadData_Click(object sender, RoutedEventArgs e)
        {
            if ((listView.SelectedValue as TestMachine).IsOpen)
            {
                var DataStr = (listView.SelectedValue as TestMachine).MeasureLxy();
                if (DataStr != null)
                {
                    Project.Results.AddSingleData(DataStr,"");
                }
            }
        }

        private void ReadConfig_Click(object sender, RoutedEventArgs e)
        {
            var DataStr = TestDevice.Measure();
        }

        private void SettingCur_Click(object sender, RoutedEventArgs e)
        {
            var DataStr = TestDevice.Measure();
        }

        private void SettingVol_Click(object sender, RoutedEventArgs e)
        {
            var DataStr = TestDevice.Set("", volt.ToString());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void AddDevice_Click(object sender, RoutedEventArgs e)
        {
            Devices.Add(new SR3A() { Key = "SR-3A" });
        }

        private void Enable_Click(object sender, RoutedEventArgs e)
        {
            if (!(listView.SelectedValue as TestMachine).IsOpen)
                (listView.SelectedValue as TestMachine).Init();
        }

        private void SendStr_Click(object sender, RoutedEventArgs e)
        {
            if ((listView.SelectedValue as TestMachine).IsOpen)
                (listView.SelectedValue as TestMachine).GetECom().SendStr(SendStr);
        }

        private void CleanStr_Click(object sender, RoutedEventArgs e)
        {
            SendStr = "";
        }
    }
}
