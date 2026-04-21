using LCD.Ctrl;
using LCD.Data;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using VisionCore;

namespace LCD.View
{
    /// <summary>
    /// 仪器设置界面
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class EComView : UserControl
    {
        public static event SetEComDg SetEComEvent;

        public ObservableCollection<ECom> TestDevices { get; set; } = new ObservableCollection<ECom>();
        public int DevTypeIndex { get; set; } = 0;
        public double volt { get; set; }
        public string SendStr { get; set; }
        public string RecvStr { get; set; }

        //DevModel devModel = new DevModel();//设备时间
        public EComView()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            //遍历comNames
            string[] portNames = SerialPort.GetPortNames();
            //if (portNames != null) { myports.ItemsSource = portNames; }

            //根据当前仪器布置界面
            if (Project.cfg != null)
            {
                switch (Project.cfg.TESTMACHINE)
                {
                    case Ctrl.ENUMMACHINE.BMA7:
                        //设置

                        break;
                    case Ctrl.ENUMMACHINE.CS2000:

                        break;

                    case Ctrl.ENUMMACHINE.Demo:

                        break;


                    case Ctrl.ENUMMACHINE.USB2000:

                        break;


                    default: break;
                }
            }

            //Parity.SelectedIndex = devModel.parity;
        }

        //private void ReConnect_Click(object sender, RoutedEventArgs e)
        //{
        //    TestDevice.Init();
        //}

        //private void ReadData_Click(object sender, RoutedEventArgs e)
        //{
        //    var DataStr = TestDevice.Measure();
        //    if (DataStr != null)
        //    {
        //        Project.Results.AddSingleData(DataStr);
        //    }
        //}

        //private void ReadConfig_Click(object sender, RoutedEventArgs e)
        //{
        //    var DataStr = TestDevice.Measure();
        //}

        //private void SettingCur_Click(object sender, RoutedEventArgs e)
        //{
        //    var DataStr = TestDevice.Measure();
        //}

        //private void SettingVol_Click(object sender, RoutedEventArgs e)
        //{
        //    var DataStr = TestDevice.Set("", volt.ToString());
        //}

        private void AddDevice_Click(object sender, RoutedEventArgs e)
        {
            string m_CurKey;
            ECom eCommunacation = null;
            switch (DevTypeIndex)
            {
                case 2:
                    m_CurKey = EComManageer.CreateECom(CommunicationModel.TcpServer);//创建tcp服务端
                    eCommunacation = EComManageer.GetECommunacation(m_CurKey);
                    //eCommunacation.ReceiveString += ECommunacation_ReceiveString;
                    //设置通讯参数
                    //不需要设置监听的ip 默认是0.0.0.0 就可以监听所有ip段
                    eCommunacation.LocalPort = 8000;//设置端口
                    break;
                case 3:
                    m_CurKey = EComManageer.CreateECom(CommunicationModel.TcpClient);//创建tcp客户端
                    eCommunacation = EComManageer.GetECommunacation(m_CurKey);
                    //eCommunacation.ReceiveString += ECommunacation_ReceiveString;
                    //设置通讯参数
                    eCommunacation.RemoteIP = "127.0.0.1";//
                    eCommunacation.RemotePort = 8001;
                    break;
                case 0:
                    m_CurKey = EComManageer.CreateECom(CommunicationModel.COM);//创建串口
                    eCommunacation = EComManageer.GetECommunacation(m_CurKey);
                    //eCommunacation.ReceiveString += ECommunacation_ReceiveString;
                    //设置通讯参数
                    eCommunacation.PortName = "COM1";//
                    eCommunacation.BaudRate = "9600";
                    eCommunacation.Parity = "None";
                    eCommunacation.DataBits = "8";
                    eCommunacation.StopBits = "One";
                    break;
                case 1:
                    m_CurKey = EComManageer.CreateECom(CommunicationModel.UDP);//创建UDP
                    eCommunacation = EComManageer.GetECommunacation(m_CurKey);
                    //eCommunacation.ReceiveString += ECommunacation_ReceiveString;
                    //设置通讯参数
                    eCommunacation.RemoteIP = "127.0.0.1";//
                    eCommunacation.RemotePort = 8002;
                    eCommunacation.LocalPort = 8003;
                    break;
            }
            SetEComEvent?.Invoke(eCommunacation, EComType.Add);
            EComChang(eCommunacation, EComType.Add);
        }

        /// <summary>
        /// ECom变化事件
        /// </summary>
        /// <param name="eCom"></param>
        /// <param name="type"></param>
        private void EComChang(ECom eCom, EComType type)
        {
            if (eCom == null & type != EComType.Load) { return; }
            TestDevices.Add(eCom);

            switch (type)
            {
                case EComType.Add:
                    break;
                case EComType.Load:
                    break;
                case EComType.Remov:
                    TestDevices.Remove(eCom);
                    EComManageer.DeleteECom(eCom.Key);
                    break;
            }
        }

        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletDevice_Click(object sender, RoutedEventArgs e)
        {
            var item = listView.SelectedValue as ECom;
            if (item != null && item.IsOpen) item.IsOpen = false;
            TestDevices.Remove(item);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            var item = listView.SelectedValue as ECom;

            if (item != null && item.IsOpen)
            {
                item.SendStr(SendStr);
            }
        }
    }
}
