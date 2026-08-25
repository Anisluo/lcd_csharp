using LCD.ViewMode;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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
    /// Power.xaml 的交互逻辑
    /// </summary>
    public partial class Power : Window
    {
        public Power()
        {
            InitializeComponent();

            string[] portNames = SerialPort.GetPortNames();
            if (portNames != null) { myports.ItemsSource = portNames; }

            this.DataContext = powerViewMode;
        }
        public PowerViewMode powerViewMode { get; set; } = new PowerViewMode();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Project.cfg.power.Enabled = powerViewMode.IsCheckBox;
            Project.cfg.power.Bus.ComName = powerViewMode.comName;
            Project.cfg.power.Bus.comName = powerViewMode.comNameText;
            Project.cfg.power.Bus.bardRate = powerViewMode.bardRate;
            Project.cfg.power.Bus.BarRate = powerViewMode.bardRateText;
            Project.cfg.power.Bus.dataBit = powerViewMode.dataBit;
            Project.cfg.power.Bus.DataBit = powerViewMode.dataBitText;
            Project.cfg.power.Bus.stopBit = powerViewMode.stopBit;
            Project.cfg.power.Bus.StopBit = powerViewMode.stopBitText;
            Project.cfg.power.Bus.parity = powerViewMode.Parity;
            Project.cfg.power.Bus.Parity=powerViewMode.ParityText;

            Project.SaveConfig("Config.xml");//保存为配置文件

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          powerViewMode.IsCheckBox = Project.cfg.power.Enabled;
          powerViewMode.comName=Project.cfg.power.Bus.ComName ;  
          powerViewMode.comNameText=Project.cfg.power.Bus.comName ;  
          powerViewMode.bardRate=Project.cfg.power.Bus.bardRate;  
          powerViewMode.bardRateText=Project.cfg.power.Bus.BarRate ;  
          powerViewMode.dataBit=Project.cfg.power.Bus.dataBit ;  
          powerViewMode.dataBitText=Project.cfg.power.Bus.DataBit ;  
          powerViewMode.stopBit=Project.cfg.power.Bus.stopBit ;  
          powerViewMode.stopBitText=Project.cfg.power.Bus.StopBit ;  
          powerViewMode.Parity=Project.cfg.power.Bus.parity  ;
          powerViewMode.ParityText= Project.cfg.power.Bus.Parity;   
        }
    }
}
