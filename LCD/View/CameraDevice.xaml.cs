using LCD.Data;
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
    /// CameraDevice.xaml 的交互逻辑
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class CameraDevice : Window
    {
        public double X { get { if (Project.cam == null) return 0; else return Project.cam.Left; } set { if (Project.cam != null) Project.cam.Left = value; } }
        public double Y { get { if (Project.cam == null) return 0; else return Project.cam.Top; } set { if (Project.cam != null) Project.cam.Top = value; } }
        public CameraDevice()
        {
            InitializeComponent();
            DataContext = this;
           
            Camer.SelectedIndex = Project.cfg.Camer;
        }

        //Cam
        private void OnBnClickedEnsure(object sender, RoutedEventArgs e)
        {
            // Project.cfg.ExposureTime = (uint)ScrollBar.Value;
            Project.cfg.CamTop = Y;
            Project.cfg.CamLeft = X;
            Project.cfg.Camer = Camer.SelectedIndex;
            //Project.cfg.ExposureTime=uint.Parse(Cam.Text.Trim());
            Project.SaveConfig("Config.xml");
            //Project.cam.CloseCam();
            this.Close();

        }

        private void V110n_Selected(object sender, MouseButtonEventArgs e)
        {
            //V110.IsChecked = true;
        }

        private void HikVision_Selected(object sender, MouseButtonEventArgs e)
        {
            //Hikvision.IsChecked = true;
        }

       
    }
}
