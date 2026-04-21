using LCD.Ctrl;
using System;
using System.Collections.Generic;
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
    /// ViewangleView.xaml 的交互逻辑
    /// </summary>
    public partial class ViewangleView : Window
    {
        public ViewangleView()
        {
            InitializeComponent();
            if ((Project.cfg.TESTMACHINE != Ctrl.ENUMMACHINE.SR3A)&&(
                Project.cfg.TESTMACHINE != Ctrl.ENUMMACHINE.SR5A) &&
               (Project.cfg.TESTMACHINE != Ctrl.ENUMMACHINE.CS2000) ) 
            {
                fdl01.IsEnabled = false;
                fdl02.IsEnabled = false;
                fdl1.IsEnabled = false; 
                fdl2.IsEnabled=false;
            }
            //cs2000只有3个啊
            if(Project.cfg.TESTMACHINE == Ctrl.ENUMMACHINE.CS2000)
            {
                fdl2.Visibility = Visibility.Collapsed;
            }
            if(Project.testMachine.IsOpen==false)
            {
                Project.testMachine.Init();
            }
        }

        private void OnFdl01(object sender, RoutedEventArgs e)
        {
            if (Project.cfg.TESTMACHINE == Ctrl.ENUMMACHINE.CS2000)
            {
                MainWindow.cs2000_send_cmd("STSS,2\r\n");
            }
            else
                MainWindow.sr3a_send_fdl_cmd("FLD4");            
        }

        private void OnFdl02(object sender, RoutedEventArgs e)
        {
            if (Project.cfg.TESTMACHINE == Ctrl.ENUMMACHINE.CS2000)
            {
                MainWindow.cs2000_send_cmd("STSS,1\r\n");
            }
            else
                MainWindow.sr3a_send_fdl_cmd("FLD3");           
        }

        private void OnFdl1(object sender, RoutedEventArgs e)
        {
            if (Project.cfg.TESTMACHINE == Ctrl.ENUMMACHINE.CS2000)
            {
                MainWindow.cs2000_send_cmd("STSS,0\r\n");
            }
            else
                MainWindow.sr3a_send_fdl_cmd("FLD2");           
        }

        private void OnFdl2(object sender, RoutedEventArgs e)
        {
            if (Project.cfg.TESTMACHINE == Ctrl.ENUMMACHINE.CS2000)
            {
                //MainWindow.cs2000_send_cmd("STSS,0\r\n");
            }
            else
                MainWindow.sr3a_send_fdl_cmd("FLD1");
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
