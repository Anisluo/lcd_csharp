using LCD.Data;
using System.IO.Ports;
using System.Web.UI.WebControls;
using System.Windows;

namespace LCD.View
{
    /// <summary>
    /// 仪器设置界面
    /// </summary>
    
    public partial class DevicesView : Window
    {
        Ctrl.ENUMMACHINE MACHINE;
        public DevModel devModel { get; set; } = new DevModel();//设备时间
        public DevicesView()
        {
            InitializeComponent();
            Init();
            this.DataContext = devModel;
        }


        private void Init()
        {
            //遍历comNames
            string[] portNames = SerialPort.GetPortNames();
            if (portNames != null) { myports.ItemsSource = portNames; }


            //测试模式
            if (Project.cfg != null)
            {
                MACHINE = Project.cfg.TESTMACHINE;
            }

            switch (MACHINE)
            {


                case Ctrl.ENUMMACHINE.BMA7:
                    //设置
                    BM7A.IsChecked = true;
                    {
                        Project.testMachine = Ctrl.BM7A.GetInstance();
                        if (Project.cfg.BM7A == null) { Project.cfg.BM7A = new ComDevice(); }
                        devModel.lum = Project.cfg.BM7A.lum;
                        devModel.cx = Project.cfg.BM7A.cx;
                        devModel.cy = Project.cfg.BM7A.cy;
                        devModel.comName = Project.cfg.BM7A.comName;
                        devModel.dataBit = Project.cfg.BM7A.dataBit;
                        devModel.bardRate = Project.cfg.BM7A.bardRate;
                        devModel.stopBit = Project.cfg.BM7A.stopBit;
                        devModel.Parity = Project.cfg.BM7A.parity;
                        devModel.startDelay = Project.cfg.BM7A.startDelay;
                        devModel.mesDelay = Project.cfg.BM7A.mesDelay;
                        devModel.resetDelay = Project.cfg.BM7A.resetDelay;

                        General.Visibility = Visibility.Visible;
                        General_Com.Visibility = Visibility.Visible;
                        USB2000.Visibility = Visibility.Hidden;

                    }
                    break;
                case Ctrl.ENUMMACHINE.CS2000:
                    CS2000.IsChecked = true;
                    {
                        Project.testMachine = Ctrl.CS2000.GetInstance();
                        if (Project.cfg.CS2000 == null) { Project.cfg.CS2000 = new ComDevice(); }
                        devModel.lum = Project.cfg.CS2000.lum;
                        devModel.cx = Project.cfg.CS2000.cx;
                        devModel.cy = Project.cfg.CS2000.cy;
                        devModel.comName = Project.cfg.CS2000.comName;
                        devModel.dataBit = Project.cfg.CS2000.dataBit;
                        devModel.stopBit = Project.cfg.CS2000.stopBit;
                        devModel.bardRate = Project.cfg.CS2000.bardRate;
                        devModel.Parity = Project.cfg.CS2000.parity;
                        devModel.startDelay = Project.cfg.CS2000.startDelay;
                        devModel.mesDelay = Project.cfg.CS2000.mesDelay;
                        devModel.resetDelay = Project.cfg.CS2000.resetDelay;





                        General.Visibility = Visibility.Visible;
                        General_Com.Visibility = Visibility.Visible;
                        USB2000.Visibility = Visibility.Hidden;
                    }
                    break;

                case Ctrl.ENUMMACHINE.Demo:
                    CS2000.IsChecked = true;
                    {
                        Project.testMachine = Ctrl.CS2000.GetInstance();
                        if (Project.cfg.Demo == null) { Project.cfg.Demo = new ComDevice(); }
                        devModel.lum = Project.cfg.Demo.lum;
                        devModel.cx = Project.cfg.Demo.cx;
                        devModel.cy = Project.cfg.Demo.cy;
                        devModel.comName = Project.cfg.Demo.comName;
                        devModel.dataBit = Project.cfg.Demo.dataBit;
                        devModel.stopBit = Project.cfg.Demo.stopBit;
                        devModel.bardRate = Project.cfg.Demo.bardRate;
                        devModel.Parity = Project.cfg.Demo.parity;
                        devModel.startDelay = Project.cfg.Demo.startDelay;
                        devModel.mesDelay = Project.cfg.Demo.mesDelay;
                        devModel.resetDelay = Project.cfg.Demo.resetDelay;

                        General.Visibility = Visibility.Visible;
                        General_Com.Visibility = Visibility.Visible;
                        USB2000.Visibility = Visibility.Hidden;
                    }
                    break;


                case Ctrl.ENUMMACHINE.USB2000:
                    CS200.IsChecked = true;
                    {
                        Project.testMachine = Ctrl.USB2000.GetInstance();
                        if (Project.cfg.USB2000 == null) { Project.cfg.USB2000 = new ComDevice(); }
                        devModel.lum = Project.cfg.USB2000.lum;
                        devModel.cx = Project.cfg.USB2000.cx;
                        devModel.cy = Project.cfg.USB2000.cy;
                        devModel.comName = Project.cfg.USB2000.comName;
                        devModel.dataBit = Project.cfg.USB2000.dataBit;
                        devModel.stopBit = Project.cfg.USB2000.stopBit;
                        devModel.bardRate = Project.cfg.USB2000.bardRate;
                        devModel.Parity = Project.cfg.USB2000.parity;
                        devModel.startDelay = Project.cfg.USB2000.startDelay;
                        devModel.mesDelay = Project.cfg.USB2000.mesDelay;
                        devModel.resetDelay = Project.cfg.USB2000.resetDelay;

                        General.Visibility = Visibility.Hidden;
                        General_Com.Visibility = Visibility.Hidden;
                        USB2000.Visibility = Visibility.Visible;
                    }
                    break;


                case Ctrl.ENUMMACHINE.SR3A:
                
                    Project.testMachine = Ctrl.SR3A.GetInstance();
                    SR3A.IsChecked = true;
                    {
                        if (Project.cfg.SR3A == null)
                        {
                            Project.cfg.SR3A = new ComDevice();
                            Project.cfg.SR3A.lum = 1;
                            Project.cfg.SR3A.bardRate = 38400;
                        }

                        devModel.lum = Project.cfg.SR3A.lum;
                        devModel.cx = Project.cfg.SR3A.cx;
                        devModel.cy = Project.cfg.SR3A.cy;
                        devModel.comName = Project.cfg.SR3A.comName;
                        devModel.dataBit = Project.cfg.SR3A.dataBit;
                        devModel.stopBit = Project.cfg.SR3A.stopBit;
                        devModel.bardRate = Project.cfg.SR3A.bardRate;
                        devModel.Parity = Project.cfg.SR3A.parity;
                        devModel.startDelay = Project.cfg.SR3A.startDelay;
                        devModel.mesDelay = Project.cfg.SR3A.mesDelay;
                        devModel.resetDelay = Project.cfg.SR3A.resetDelay;

                        General.Visibility = Visibility.Hidden;
                        General_Com.Visibility = Visibility.Visible;
                        USB2000.Visibility = Visibility.Hidden;

                        //myports.Text = Project.cfg.SR3A.comName;
                        //_bardRate.Text = Project.cfg.SR3A.bardRateText;
                        //_dataBit.Text = Project.cfg.SR3A.dataBitText;
                        //_stopBit.Text = Project.cfg.SR3A.stopBitText;
                        //_Parity.Text = Project.cfg.SR3A.parityText;


                        //USB2000.Visibility = Visibility.Hidden;
                    }
                    break;

                case Ctrl.ENUMMACHINE.SR5A:

                    Project.testMachine = Ctrl.SR3A.GetInstance();
                    SR5A.IsChecked = true;
                    {
                        if (Project.cfg.SR5A == null)
                        {
                            Project.cfg.SR5A = new ComDevice();
                            Project.cfg.SR5A.lum = 1;
                            Project.cfg.SR5A.bardRate = 38400;
                        }

                        devModel.lum = Project.cfg.SR5A.lum;
                        devModel.cx = Project.cfg.SR5A.cx;
                        devModel.cy = Project.cfg.SR5A.cy;
                        devModel.comName = Project.cfg.SR5A.comName;
                        devModel.dataBit = Project.cfg.SR5A.dataBit;
                        devModel.stopBit = Project.cfg.SR5A.stopBit;
                        devModel.bardRate = Project.cfg.SR5A.bardRate;
                        devModel.Parity = Project.cfg.SR5A.parity;
                        devModel.startDelay = Project.cfg.SR5A.startDelay;
                        devModel.mesDelay = Project.cfg.SR5A.mesDelay;
                        devModel.resetDelay = Project.cfg.SR5A.resetDelay;

                        General.Visibility = Visibility.Hidden;
                        General_Com.Visibility = Visibility.Visible;
                        USB2000.Visibility = Visibility.Hidden;

                        //myports.Text = Project.cfg.SR3A.comName;
                        //_bardRate.Text = Project.cfg.SR3A.bardRateText;
                        //_dataBit.Text = Project.cfg.SR3A.dataBitText;
                        //_stopBit.Text = Project.cfg.SR3A.stopBitText;
                        //_Parity.Text = Project.cfg.SR3A.parityText;


                        //USB2000.Visibility = Visibility.Hidden;
                    }
                    break;
				case Ctrl.ENUMMACHINE.MS01:

                    Project.testMachine = Ctrl.MS01.GetInstance();
                    MS01.IsChecked = true;
                    {
                        if (Project.cfg.MS01 == null)
                        {
                            Project.cfg.MS01 = new ComDevice();
                            Project.cfg.MS01.lum = 1;
                            Project.cfg.MS01.bardRate = 115200;
                        }

                        devModel.lum = Project.cfg.MS01.lum;
                        devModel.cx = Project.cfg.MS01.cx;
                        devModel.cy = Project.cfg.MS01.cy;
                        devModel.comName = Project.cfg.MS01.comName;
                        devModel.dataBit = Project.cfg.MS01.dataBit;
                        devModel.stopBit = Project.cfg.MS01.stopBit;
                        devModel.bardRate = Project.cfg.MS01.bardRate;
                        devModel.Parity = Project.cfg.MS01.parity;
                        devModel.startDelay = Project.cfg.MS01.startDelay;
                        devModel.mesDelay = Project.cfg.MS01.mesDelay;
                        devModel.resetDelay = Project.cfg.MS01.resetDelay;

                        General.Visibility = Visibility.Hidden;
                        General_Com.Visibility = Visibility.Visible;
                        USB2000.Visibility = Visibility.Hidden;
                    }
                    break;

                case Ctrl.ENUMMACHINE.Admesy:
                    Admesy.IsChecked = true;
                    break;


                default: break;
            }

            
            if(Project.cfg != null)
            {
                if(Project.cfg.Comm != null)
                {
                    myports.Text = Project.cfg.Comm.comName ;
                    _bardRate.Text = Project.cfg.Comm.bardRateText;
                    _dataBit.Text = Project.cfg.Comm.dataBitText;
                    _stopBit.Text = Project.cfg.Comm.stopBitText ;
                    _Parity.Text = Project.cfg.Comm.parityText;
                }
            }
            //Parity.SelectedIndex = devModel.parity;
        }
        /// <summary>
        /// 重启设备
        /// </summary>
        private void ResStartDevice()
        {

        }

        //ui界面刷新称Data数据
        private void UI2Data()
        {
            //赋值测试数据
            Project.cfg.TESTMACHINE = MACHINE;
            switch (MACHINE)
            {
                case Ctrl.ENUMMACHINE.BMA7:
                    //设置
                    BM7A.IsChecked = true;
                    {
                        Project.cfg.BM7A.lum = devModel.lum;
                        Project.cfg.BM7A.cx = devModel.cx;
                        Project.cfg.BM7A.cy = devModel.cy;

                        Project.cfg.BM7A.comName = myports.Text;
                        Project.cfg.BM7A.bardRate = _bardRate.SelectedIndex;
                        Project.cfg.BM7A.bardRateText = _bardRate.Text;
                        Project.cfg.BM7A.dataBit = _dataBit.SelectedIndex;
                        Project.cfg.BM7A.dataBitText = _dataBit.Text;
                        Project.cfg.BM7A.stopBit = _stopBit.SelectedIndex;
                        Project.cfg.BM7A.stopBitText = _stopBit.Text;
                        Project.cfg.BM7A.parity = _Parity.SelectedIndex;
                        Project.cfg.BM7A.parityText = _Parity.Text;


                        Project.cfg.BM7A.startDelay = devModel.startDelay;
                        Project.cfg.BM7A.mesDelay = devModel.mesDelay;
                        Project.cfg.BM7A.resetDelay = devModel.resetDelay;
                        Project.testMachine = Ctrl.BM7A.GetInstance();
                        Project.testMachine.Init();


                        //General.Visibility = Visibility.Visible;
                        //General_Com.Visibility = Visibility.Visible;
                        //USB2000.Visibility = Visibility.Hidden;
                    }
                    break;
                case Ctrl.ENUMMACHINE.CS2000:
                    CS2000.IsChecked = true;
                    {
                        Project.cfg.CS2000.lum =   devModel.lum;
                        Project.cfg.CS2000.cx = devModel.cx;
                        Project.cfg.CS2000.cy = devModel.cy;


                        Project.cfg.CS2000.comName = myports.Text;
                        Project.cfg.CS2000.bardRate = _bardRate.SelectedIndex;
                        Project.cfg.CS2000.bardRateText = _bardRate.Text;
                        Project.cfg.CS2000.dataBit = _dataBit.SelectedIndex;
                        Project.cfg.CS2000.dataBitText = _dataBit.Text;
                        Project.cfg.CS2000.stopBit = _stopBit.SelectedIndex;
                        Project.cfg.CS2000.stopBitText = _stopBit.Text;
                        Project.cfg.CS2000.parity = _Parity.SelectedIndex;
                        Project.cfg.CS2000.parityText = _Parity.Text;



                        Project.cfg.CS2000.startDelay = devModel.startDelay;
                        Project.cfg.CS2000.mesDelay = devModel.mesDelay;
                        Project.cfg.CS2000.resetDelay = devModel.resetDelay;
                    }
                    break;

                case Ctrl.ENUMMACHINE.USB2000:
                    CS200.IsChecked = true;
                    {
                        if (Project.cfg.USB2000 == null) { Project.cfg.USB2000 = new ComDevice(); }
                        Project.cfg.USB2000.lum = devModel.lum;
                        Project.cfg.USB2000.cx = devModel.cx;
                        Project.cfg.USB2000.cy = devModel.cy;
                        Project.cfg.USB2000.comName = devModel.comName;
                        Project.cfg.USB2000.bardRate = devModel.bardRate;
                        Project.cfg.USB2000.dataBit = devModel.dataBit;
                        Project.cfg.USB2000.stopBit = devModel.stopBit;
                        Project.cfg.USB2000.parity = devModel.Parity;
                        Project.cfg.USB2000.startDelay = devModel.startDelay;
                        Project.cfg.USB2000.mesDelay = devModel.mesDelay;
                        Project.cfg.USB2000.resetDelay = devModel.resetDelay;
                        Project.testMachine = Ctrl.USB2000.GetInstance();
                        Project.testMachine.Init();

                        //General.Visibility = Visibility.Hidden;
                        //General_Com.Visibility = Visibility.Hidden;
                        //USB2000.Visibility = Visibility.Visible;
                    }
                    break;

                case Ctrl.ENUMMACHINE.SR3A:
                    SR3A.IsChecked = true;
                    {
                        if (Project.cfg.SR3A == null) { Project.cfg.SR3A = new ComDevice(); }
                        Project.cfg.SR3A.lum = devModel.lum;
                        Project.cfg.SR3A.cx = devModel.cx;
                        Project.cfg.SR3A.cy = devModel.cy;
                        Project.cfg.SR3A.comName  = myports.Text;
                        Project.cfg.SR3A.bardRate = _bardRate.SelectedIndex;
                        Project.cfg.SR3A.dataBit  = _dataBit.SelectedIndex;
                        Project.cfg.SR3A.stopBit  = _stopBit.SelectedIndex;
                        Project.cfg.SR3A.parity   = _Parity.SelectedIndex;
                        Project.cfg.SR3A.startDelay = devModel.startDelay;
                        Project.cfg.SR3A.mesDelay = devModel.mesDelay;
                        Project.cfg.SR3A.resetDelay = devModel.resetDelay;
                        Project.testMachine = Ctrl.SR3A.GetInstance();
                        

                        //General.Visibility = Visibility.Hidden;
                        //General_Com.Visibility = Visibility.Hidden;
                        //USB2000.Visibility = Visibility.Visible;

                        Project.cfg.SR3A.bardRateText = _bardRate.Text;
                        Project.cfg.SR3A.dataBitText = _dataBit.Text;
                        Project.cfg.SR3A.stopBitText = _stopBit.Text;
                        Project.cfg.SR3A.parityText = _Parity.Text;

                        Project.testMachine.Init();
                    }
                    break;

                case Ctrl.ENUMMACHINE.SR5A:
                    SR5A.IsChecked = true;
                    {
                        if (Project.cfg.SR5A == null) { Project.cfg.SR5A = new ComDevice(); }
                        Project.cfg.SR5A.lum = devModel.lum;
                        Project.cfg.SR5A.cx = devModel.cx;
                        Project.cfg.SR5A.cy = devModel.cy;
                        Project.cfg.SR5A.comName = myports.Text;
                        Project.cfg.SR5A.bardRate = _bardRate.SelectedIndex;
                        Project.cfg.SR5A.dataBit = _dataBit.SelectedIndex;
                        Project.cfg.SR5A.stopBit = _stopBit.SelectedIndex;
                        Project.cfg.SR5A.parity = _Parity.SelectedIndex;
                        Project.cfg.SR5A.startDelay = devModel.startDelay;
                        Project.cfg.SR5A.mesDelay = devModel.mesDelay;
                        Project.cfg.SR5A.resetDelay = devModel.resetDelay;
                        Project.testMachine = Ctrl.SR3A.GetInstance();


                        //General.Visibility = Visibility.Hidden;
                        //General_Com.Visibility = Visibility.Hidden;
                        //USB2000.Visibility = Visibility.Visible;

                        Project.cfg.SR5A.bardRateText = _bardRate.Text;
                        Project.cfg.SR5A.dataBitText = _dataBit.Text;
                        Project.cfg.SR5A.stopBitText = _stopBit.Text;
                        Project.cfg.SR5A.parityText = _Parity.Text;

                        Project.testMachine.Init();
                    }
                    break;

                case Ctrl.ENUMMACHINE.MS01:
                    MS01.IsChecked = true;
                    {
                        if (Project.cfg.MS01 == null) 
                        { 
                            Project.cfg.MS01 = new ComDevice(); 
                        }
                        Project.cfg.MS01.lum = devModel.lum;
                        Project.cfg.MS01.cx = devModel.cx;
                        Project.cfg.MS01.cy = devModel.cy;
                        Project.cfg.MS01.comName = myports.Text;
                        Project.cfg.MS01.bardRate = _bardRate.SelectedIndex;
                        Project.cfg.MS01.dataBit = _dataBit.SelectedIndex;
                        Project.cfg.MS01.stopBit = _stopBit.SelectedIndex;
                        Project.cfg.MS01.parity = _Parity.SelectedIndex;
                        Project.cfg.MS01.startDelay = devModel.startDelay;
                        Project.cfg.MS01.mesDelay = devModel.mesDelay;
                        Project.cfg.MS01.resetDelay = devModel.resetDelay;
                        Project.testMachine = Ctrl.MS01.GetInstance();

                        Project.cfg.MS01.bardRateText = _bardRate.Text;
                        Project.cfg.MS01.dataBitText = _dataBit.Text;
                        Project.cfg.MS01.stopBitText = _stopBit.Text;
                        Project.cfg.MS01.parityText = _Parity.Text;

                        Project.testMachine.Init();
                    }
                    break;					

                case Ctrl.ENUMMACHINE.Admesy:
                    Admesy.IsChecked = true;
                    break;


                default: break;
            }

            //Project.cfg.Comm.comName= myports.Text;
            //Project.cfg.Comm.bardRateText = _bardRate.Text;
            //Project.cfg.Comm.dataBit = int.Parse(_dataBit.Text);
            //Project.cfg.Comm.dataBitText = _dataBit.Text;
            //Project.cfg.Comm.stopBit = int.Parse(_stopBit.Text);
            //Project.cfg.Comm.stopBitText = _stopBit.Text;
            //Project.cfg.Comm.parity = _Parity.SelectedIndex;
            //Project.cfg.Comm.parityText = _Parity.Text;
        }
        private void OnBnClickedEnsure(object sender, RoutedEventArgs e)
        {
            UI2Data();
            Project.WriteLog("保存设备配置文件");
            Project.SaveConfig("Config.xml");
            this.Close();
        }
        private void OnBnClickedBM7A(object sender, RoutedEventArgs e)
        {
            if (BM7A.IsChecked == true)
            {
                select_one_radio(BM7A);
                MACHINE = Ctrl.ENUMMACHINE.BMA7;

                myports.Text= Project.cfg.BM7A.comName ;
                _bardRate.Text = Project.cfg.BM7A.bardRateText;
                _dataBit.Text = Project.cfg.BM7A.dataBitText;
                _stopBit.Text = Project.cfg.BM7A.stopBitText;
                _Parity.Text = Project.cfg.BM7A.parityText;

                General.Visibility = Visibility.Visible;
                General_Com.Visibility = Visibility.Visible;
                USB2000.Visibility = Visibility.Hidden;
            }
        }
        private void OnBnClickedBM5A(object sender, RoutedEventArgs e)
        {
            if (BM5A.IsChecked == true)
            {
                select_one_radio(BM5A);
                MACHINE = Ctrl.ENUMMACHINE.BM5A;

                General.Visibility = Visibility.Visible;
                General_Com.Visibility = Visibility.Visible;
                USB2000.Visibility = Visibility.Hidden;
            }

        }
        private void OnBnClickedPR655(object sender, RoutedEventArgs e)
        {
            if (PR655.IsChecked == true)
            {
                select_one_radio(PR655);
                MACHINE = Ctrl.ENUMMACHINE.PR655;

                General.Visibility = Visibility.Visible;
                General_Com.Visibility = Visibility.Visible;
                USB2000.Visibility = Visibility.Hidden;

            }
        }
        private void OnBnClickedCS2000(object sender, RoutedEventArgs e)
        {
            if (CS2000.IsChecked == true)
            {
                select_one_radio(CS2000);
                MACHINE = Ctrl.ENUMMACHINE.CS2000;

                myports.Text = Project.cfg.CS2000.comName;

                _bardRate.Text = Project.cfg.CS2000.bardRateText;
                _dataBit.Text = Project.cfg.CS2000.dataBitText;
                _stopBit.Text = Project.cfg.CS2000.stopBitText;
                _Parity.Text = Project.cfg.CS2000.parityText;

                General.Visibility = Visibility.Visible;
                General_Com.Visibility = Visibility.Visible;
                USB2000.Visibility = Visibility.Hidden;
            }
        }

      

        private void OnBnClickedAdmesy(object sender, RoutedEventArgs e)
        {
            if (Admesy.IsChecked == true)
            {
                select_one_radio(Admesy);
                MACHINE = Ctrl.ENUMMACHINE.Admesy;

                General.Visibility = Visibility.Visible;
                General_Com.Visibility = Visibility.Visible;
                USB2000.Visibility = Visibility.Hidden;
            }
        }
        private void OnBnClickedSR3A(object sender, RoutedEventArgs e)
        {
            if (SR3A.IsChecked == true)
            {
                select_one_radio(SR3A);
                MACHINE = Ctrl.ENUMMACHINE.SR3A;
                select_sr3a();
                USB2000.Visibility = Visibility.Hidden;
            }
        }

        private void select_sr3a()
        {
            myports.Text = Project.cfg.SR3A.comName;
            _bardRate.Text = Project.cfg.SR3A.bardRateText;
            _dataBit.Text = Project.cfg.SR3A.dataBitText;
            _stopBit.Text = Project.cfg.SR3A.stopBitText;
            _Parity.Text = Project.cfg.SR3A.parityText;

            General.Visibility = Visibility.Visible;
            General_Com.Visibility = Visibility.Visible;
            USB2000.Visibility = Visibility.Hidden;
        }

        private void OnBnClickedBM5AS(object sender, RoutedEventArgs e)
        {
            if (BM5AS.IsChecked == true)
            {
                select_one_radio(BM5AS);
                MACHINE = Ctrl.ENUMMACHINE.BM5AS;

                General.Visibility = Visibility.Visible;
                General_Com.Visibility = Visibility.Visible;
                USB2000.Visibility = Visibility.Hidden;
            }
        }
        private void OnBnClickedCS200(object sender, RoutedEventArgs e)
        {

            if (CS200.IsChecked == true)
            {
                select_one_radio(CS200);
                MACHINE = Ctrl.ENUMMACHINE.USB2000;

                General.Visibility = Visibility.Hidden;
                General_Com.Visibility = Visibility.Hidden;
                USB2000.Visibility = Visibility.Visible;
            }
        }
        private void OnBnClickedDemo(object sender, RoutedEventArgs e)
        {
            if (demo.IsChecked == true)
            {
                select_one_radio(demo);
                MACHINE = Ctrl.ENUMMACHINE.Demo;

                General.Visibility = Visibility.Visible;
                General_Com.Visibility = Visibility.Visible;
                USB2000.Visibility = Visibility.Hidden;
            }
        }

        private void select_one_radio(System.Windows.Controls.RadioButton radio)
        {
            System.Windows.Controls.RadioButton[] buttons = new System.Windows.Controls.RadioButton[] { BM7A ,
                BM5A ,
                PR655,
            CS2000,
            Admesy,
            MS01,
            CS200,
            BM5AS,
            demo,
            BM5A,
            SR3A,
            SR5A
        };

            for(int i=0;i<buttons.Length;i++)
            {
                if(radio != buttons[i])
                {
                    buttons[i].IsChecked = false;
                }
                else
                {
                    buttons[i].IsChecked=true;
                }
            }
        }

        private void OnBnClickedSR5A(object sender, RoutedEventArgs e)
        {
            if (SR5A.IsChecked == true)
            {
                select_one_radio(SR5A);
                MACHINE = Ctrl.ENUMMACHINE.SR5A;
                select_sr5a();
                USB2000.Visibility = Visibility.Hidden;
            }
        }

        private void select_sr5a()
        {
            myports.Text = Project.cfg.SR5A.comName;
            _bardRate.Text = Project.cfg.SR5A.bardRateText;
            _dataBit.Text = Project.cfg.SR5A.dataBitText;
            _stopBit.Text = Project.cfg.SR5A.stopBitText;
            _Parity.Text = Project.cfg.SR5A.parityText;

            General.Visibility = Visibility.Visible;
            General_Com.Visibility = Visibility.Visible;
            USB2000.Visibility = Visibility.Hidden;
        }
		 private void select_ms01()
        {
            MACHINE = Ctrl.ENUMMACHINE.MS01;

            if (Project.cfg.MS01 != null)
            {
                myports.Text = Project.cfg.MS01.comName;
                _bardRate.Text = Project.cfg.MS01.bardRateText;
                _dataBit.Text = Project.cfg.MS01.dataBitText;
                _stopBit.Text = Project.cfg.MS01.stopBitText;
                _Parity.Text = Project.cfg.MS01.parityText;
            }

            General.Visibility = Visibility.Visible;
            General_Com.Visibility = Visibility.Visible;
            USB2000.Visibility = Visibility.Hidden;
        }

        private void OnBnClickedMs01(object sender, RoutedEventArgs e)
        {
            if (MS01.IsChecked == true)
            {
                select_one_radio(MS01);
                select_ms01();
            }
        }
    }

    //定义设备类型
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class DevModel :ViewBase
    {
        private double Lum;

        public double lum
        {
            get { return Lum; }
            set { Lum = value;
                OnPropertyChanged();
            }
        }

        private double CX;

        public double cx
        {
            get { return CX; }
            set { CX = value;
                OnPropertyChanged();
            }
        }
        private double CY;

        public double cy
        {
            get { return CY; }
            set { CY = value;
                OnPropertyChanged();
            }
        }

        private double StartDelay;

        public double startDelay
        {
            get { return StartDelay; }
            set { StartDelay = value;
                OnPropertyChanged();
            }
        }


        private double MesDelay;

        public double mesDelay
        {
            get { return MesDelay; }
            set { MesDelay = value;
                OnPropertyChanged();
            }
        }
        private double ResetDelay;

        public double resetDelay
        {
            get { return ResetDelay; }
            set { ResetDelay = value;
                OnPropertyChanged();
            }
        }

        private string ComName;

        public string comName
        {
            get { return ComName; }
            set { ComName = value;
                OnPropertyChanged();
            }
        }

        private int BardRate;

        public int bardRate
        {
            get { return BardRate; }
            set { BardRate = value;
                OnPropertyChanged();
            }
        }

        private int DataBit;

        public int dataBit
        {
            get { return DataBit; }
            set {
                DataBit = value;
                OnPropertyChanged();
            }
        }

        private int StopBit;

        public int stopBit
        {
            get { return StopBit; }
            set {
                StopBit = value;
                OnPropertyChanged();
            }
        }


        private int parity;

        public int Parity
        {
            get { return parity; }
            set {
                parity = value;
                OnPropertyChanged();
            }
        }

        private string BardRateText;

        public string bardRateText
        {
            get { return BardRateText; }
            set { BardRateText = value;
                OnPropertyChanged();
            }
        }

        private string DataBitText;

        public string dataBitText
        {
            get { return DataBitText; }
            set { DataBitText = value;
                OnPropertyChanged();
            }
        }

        private string StopBitText;

        public string stopBitText
        {
            get { return StopBitText; }
            set { StopBitText = value;
                OnPropertyChanged();
            }
        }

        private string parityText;

        public string ParityText
        {
            get { return parityText; }
            set { parityText = value;
                OnPropertyChanged();
            }
        }


        //public double lum { get; set; }
        //public double cx { get; set; }
        //public double cy { get; set; }
        //public double startDelay { get; set; }
        //public double mesDelay { get; set; }
        //public double resetDelay { get; set; }
        //public string comName { get; set; }
        //public int bardRate { get; set; }
        //public int dataBit { get; set; }
        //public int stopBit { get; set; }
        //public int parity { get; set; }
    }
}
