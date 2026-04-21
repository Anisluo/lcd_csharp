using LCD.Data;
using LCD.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LCD.View
{
    /// <summary>
    /// PG配置
    /// </summary>
    
    public partial class PGView : Window
    {
        private ENUMPG PGType = ENUMPG.NoPG;

        PGModel pg = new PGModel();

        int Test = 0;
        public PGView()
        {

            this.DataContext = pg;

            InitializeComponent();
            Init();



        }


        private void Init()
        {
            PGType = Project.cfg.PGType;
            if (Project.cfg != null)
            {
                switch (Project.cfg.PGType)
                {
                    case ENUMPG.SoftPG:
                        if (Project.cfg.softPG != null)
                        {
                            
                                softPg.IsChecked = true;
                                otherPG.IsChecked = false;
                                demoPG.IsChecked = false;
                            
                           


                            pg.ip = Project.cfg.softPG.ip;
                            pg.port = Project.cfg.softPG.port;
                        }
                        else
                        {
                            Project.cfg.softPG = new IPDevice() { ip = Helper_Net.GetLocalIP(), port = "8080" };
                        }
                        break;
                    case ENUMPG.OtherPG:

                      
                            softPg.IsChecked = false;
                            otherPG.IsChecked = true;
                            demoPG.IsChecked = false;
                       
                        pg.BusTypeIndex = Project.cfg.otherPG.CommunicationType;
                        if(Project.cfg.otherPG.CommunicationType == 0)
                        {
                            pg.comName = Project.cfg.otherPG.serialPort.comName;
                            pg.bardRate = Project.cfg.otherPG.serialPort.bardRate;
                            pg.dataBit = Project.cfg.otherPG.serialPort.dataBit;
                            pg.stopBit = Project.cfg.otherPG.serialPort.stopBit;
                            pg.Parity = Project.cfg.otherPG.serialPort.parity;

                        }
                        else if(Project.cfg.otherPG.CommunicationType == 1)
                        {
                            if (Project.cfg.otherPG != null)
                            {
                                otherPG.IsChecked = true;
                                ip.IsEnabled = true;
                                port.IsEnabled = true;
                                pg.ip = Project.cfg.otherPG.SDevice.ip;
                                pg.port = Project.cfg.otherPG.SDevice.port;
                            }
                            else
                            {
                                Project.cfg.otherPG.SDevice = new IPDevice() { ip = Helper_Net.GetLocalIP(), port = "8080" };
                            }
                        }
                       
                        break;
                    case ENUMPG.NoPG:
                        {
                           
                                softPg.IsChecked = false;
                                otherPG.IsChecked = false;
                                demoPG.IsChecked = true;
                          
                            demoPG.IsChecked = true;
                            ip.Text = "";
                            ip.IsEnabled = false;
                            port.Text = "";
                            port.IsEnabled = false;
                        }
                        break;
                    default:
                        {
                            demoPG.IsChecked = true;
                            ip.Text = "";
                            ip.IsEnabled = false;
                            port.Text = "";
                            port.IsEnabled = false;
                        }
                        break;
                }
            }
        }


        private void UI2Data()
        {
            Project.cfg.PGType = PGType;
            switch (PGType)
            {
                case ENUMPG.SoftPG: break;
                case ENUMPG.OtherPG: OtherPGTsn(); break;
                case ENUMPG.NoPG: break;
            }
        }
        public void OtherPGTsn()
        {
            Project.cfg.otherPG.CommunicationType = pg.BusTypeIndex;
            if (pg.BusTypeIndex == 0)
            {
                Project.cfg.otherPG.serialPort.comName = pg.comName;
                Project.cfg.otherPG.serialPort.bardRate=pg.bardRate;
                Project.cfg.otherPG.serialPort.dataBit = pg.dataBit;
                Project.cfg.otherPG.serialPort.stopBit = pg.stopBit;
                Project.cfg.otherPG.serialPort.parity = pg.Parity;
            }
            else if (pg.BusTypeIndex == 1)
            {
                Project.cfg.otherPG.SDevice.ip = pg.ip;
                Project.cfg.otherPG.SDevice.port = pg.port;
            }
        }
        private void OnBnClickedEnsure(object sender, RoutedEventArgs e)
        {
            UI2Data();//保存数据
            Project.SaveConfig("Config.xml");//保存为配置文件

            this.Close();
        }

        //设置softPG
        private void OnBnClickedSoftPG(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            string Temp = (string)radioButton.Content;

            pg.BusTypeIndex = Project.cfg.otherPG.CommunicationType;

            if (Temp.IndexOf("Soft") != -1)
            {
                pg. BusType = Visibility.Collapsed;
                otherPG.IsChecked = false;
                demoPG.IsChecked = false;
            }
            else if (Temp.IndexOf("外接") != -1)
            {
                pg.BusType = Visibility.Visible;
                softPg.IsChecked = false;
                demoPG.IsChecked = false;
            }
            else if (Temp.IndexOf("无") != -1)
            {
                pg.BusType = Visibility.Collapsed;
                softPg.IsChecked = false;
                otherPG.IsChecked = false;
            }


            if (softPg.IsChecked == true)
            {
                pg.SerialPortVisibility = Visibility.Collapsed;
                pg.TCPVisibility = Visibility.Visible;
                ip.IsEnabled = true;
                port.IsEnabled = true;
                
                pg.ip = Project.cfg.softPG.ip;
                pg.port = Project.cfg.softPG.port;
                PGType = ENUMPG.SoftPG;
            }
            else if (otherPG.IsChecked == true)
            {
                if (pg.BusTypeIndex==0)//SERIAL
                {
                    pg.SerialPortVisibility = Visibility.Visible;
                    pg.TCPVisibility = Visibility.Collapsed;

                    pg.ip = Project.cfg.otherPG.SDevice.ip;
                    pg.port = Project.cfg.otherPG.SDevice.port;
                    PGType = ENUMPG.OtherPG;
                }
                else if (pg.BusTypeIndex == 1)//TCP
                {
                    pg.SerialPortVisibility = Visibility.Collapsed;
                    pg.TCPVisibility = Visibility.Visible;
                    pg.comName = Project.cfg.otherPG.serialPort.comName;
                    pg.bardRate = Project.cfg.otherPG.serialPort.bardRate;
                    pg.dataBit = Project.cfg.otherPG.serialPort.dataBit;
                    pg.stopBit = Project.cfg.otherPG.serialPort.stopBit;
                    pg.Parity = Project.cfg.otherPG.serialPort.parity;
                    pg.ip = Project.cfg.otherPG.SDevice.ip;
                    pg.port = Project.cfg.otherPG.SDevice.port;
                    PGType = ENUMPG.OtherPG;
                }

                
            }
            else if (demoPG.IsChecked == true)
            {
                pg.SerialPortVisibility = Visibility.Collapsed;
                pg.TCPVisibility = Visibility.Collapsed;
                PGType = ENUMPG.NoPG;
            }
            else { }

            //Init(false);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {



            if (pg.BusTypeIndex == 0)
            {
                pg.TCPVisibility = Visibility.Collapsed;
                pg.SerialPortVisibility = Visibility.Visible;
            }
            else if(pg.BusTypeIndex == 1)
            {
                pg.TCPVisibility = Visibility.Visible;
                pg.SerialPortVisibility = Visibility.Collapsed;
            }
            //pg = gModel;
            
        }

        private void ComboBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        //设置外部otherPG
        //private void OnBnClickedOtherPG(object sender, RoutedEventArgs e)
        //{

        //}
    }
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class PGModel//: ViewBase
    {

        public string ip { get; set; }

        //private string Ip;

        //public string ip
        //{
        //    get { return Ip; }
        //    set { Ip = value;
        //        //OnPropertyChanged();
        //    }
        //}

        private string Port;

        public string port
        {
            get { return Port; }
            set { Port = value;
                //OnPropertyChanged();
            }
        }

        private Visibility tcpvisibility;

        public Visibility TCPVisibility
        {
            get { return tcpvisibility; }
            set { tcpvisibility = value; }
        }



        private Visibility serialportvisibility;
        public Visibility SerialPortVisibility { get { return serialportvisibility; } set
            {
                serialportvisibility = value;
                //OnPropertyChanged();
            } }


        private int bustypeindex;

        public int BusTypeIndex
        {
            get { return bustypeindex; }
            set { bustypeindex = value;
                //OnPropertyChanged();
            }
        }



        private String comname;
        /// <summary>
        /// 串口
        /// </summary>
        public String comName
        {
            get { return comname; }
            set { comname = value;
                //OnPropertyChanged();
            }
        }
        private int bardrate;
        /// <summary>
        /// 波特兰
        /// </summary>
        public int bardRate
        {
            get { return bardrate; }
            set { bardrate = value; }
        }
        private int databit;
        /// <summary>
        /// 数据位
        /// </summary>
        public int dataBit
        {
            get { return databit; }
            set { databit = value;
                //OnPropertyChanged();
            }
        }

        private int stopdit;
        /// <summary>
        /// 停止位
        /// </summary>
        public int stopBit
        {
            get { return stopdit; }
            set { stopdit = value;
                //OnPropertyChanged();
            }
        }

       
        private int parity;
        /// <summary>
        /// 校验位
        /// </summary>
        public int Parity
        {
            get { return parity; }
            set { parity = value;
                //OnPropertyChanged();
            }
        }
        private Visibility bustype;

        public Visibility BusType
        {
            get { return bustype; }
            set { bustype = value;
                //OnPropertyChanged();
            }
        }


    }

   
   
}
