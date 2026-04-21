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
            if (portNames != null) { 
                myports.ItemsSource = portNames; 
                powerports.ItemsSource = portNames;
            }

            PowerTypes.Items.Add("M8800");
            PowerTypes.Items.Add("PLD6003");
            PowerTypes.Items.Add("NGI36150");

            this.DataContext = powerViewMode;
        }
        public PowerViewMode powerViewMode { get; set; } = new PowerViewMode();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (powerViewMode.EnablePowerControl)
            {
                if (powerports.SelectedIndex<0)
                {
                    MessageBox.Show("请选择程控电源串口");
                    return;
                }
            }
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
            Project.cfg.power.PowerType = powerViewMode.PowerTypeText;
            Project.cfg.power.EnablePowerControl = powerViewMode.EnablePowerControl;
            Project.cfg.power.PowerSerialName = powerViewMode.PowerSerialName;
            Project.SaveConfig("Config.xml");//保存为配置文件

            this.Close();
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
          powerViewMode.PowerType = Project.cfg.power.PowerType;
          powerViewMode.PowerTypeText = Project.cfg.power.PowerType;
            powerViewMode.EnablePowerControl = Project.cfg.power.EnablePowerControl ;
            powerViewMode.PowerSerialName = Project.cfg.power.PowerSerialName ;
        }
    }
}
