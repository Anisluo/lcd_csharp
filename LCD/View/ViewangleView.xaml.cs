using System.Windows;

namespace LCD.View
{
    /// <summary>
    /// ViewangleView.xaml 的交互逻辑（SR3A / CS2000 视场角设置）
    /// 参照 guoxian 分支移植；suqian 无多语言，改用中文文案，机型枚举无 SR5A。
    /// </summary>
    public partial class ViewangleView : Window
    {
        public ViewangleView()
        {
            InitializeComponent();

            // 仅 SR3A / CS2000 支持视场角，其它机型禁用按钮
            if (Project.cfg.TESTMACHINE != Ctrl.ENUMMACHINE.SR3A &&
                Project.cfg.TESTMACHINE != Ctrl.ENUMMACHINE.CS2000)
            {
                fdl01.IsEnabled = false;
                fdl02.IsEnabled = false;
                fdl1.IsEnabled = false;
                fdl2.IsEnabled = false;
            }

            // CS2000 只有 3 个档位
            if (Project.cfg.TESTMACHINE == Ctrl.ENUMMACHINE.CS2000)
            {
                fdl2.Visibility = Visibility.Collapsed;
            }

            if (Project.testMachine != null && Project.testMachine.IsOpen == false)
            {
                Project.testMachine.Init();
            }
        }

        private void OnFdl01(object sender, RoutedEventArgs e)
        {
            if (Project.cfg.TESTMACHINE == Ctrl.ENUMMACHINE.CS2000)
                MainWindow.cs2000_send_cmd("STSS,2\r\n");
            else
                MainWindow.sr3a_send_fdl_cmd("FLD4");
        }

        private void OnFdl02(object sender, RoutedEventArgs e)
        {
            if (Project.cfg.TESTMACHINE == Ctrl.ENUMMACHINE.CS2000)
                MainWindow.cs2000_send_cmd("STSS,1\r\n");
            else
                MainWindow.sr3a_send_fdl_cmd("FLD3");
        }

        private void OnFdl1(object sender, RoutedEventArgs e)
        {
            if (Project.cfg.TESTMACHINE == Ctrl.ENUMMACHINE.CS2000)
                MainWindow.cs2000_send_cmd("STSS,0\r\n");
            else
                MainWindow.sr3a_send_fdl_cmd("FLD2");
        }

        private void OnFdl2(object sender, RoutedEventArgs e)
        {
            // CS2000 无此档（fdl2 已隐藏），仅 SR3A
            MainWindow.sr3a_send_fdl_cmd("FLD1");
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
