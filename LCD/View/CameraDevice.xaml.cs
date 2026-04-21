using LCD.Data;
using MvCamCtrl.NET;
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
        public double X { get { if (Project.cam == null) return 0; else return Project.cam.Left; } set { if (Project.cam != null)Project.cam.Left = value; } }
        public double Y { get { if (Project.cam == null) return 0; else return Project.cam.Top; } set { if (Project.cam != null) Project.cam.Top = value; } }
        public CameraDevice()
        {
            InitializeComponent();
            DataContext = this;
           
            Camer.SelectedIndex = Project.cfg.Camer;
            CamerIndex.Items.Clear();
            uint count  = CamView.find_camera_count();
            if(count >0)
            {
                for(int i=0;i<count;i++)
                {
                    CamerIndex.Items.Add(i.ToString());
                }
                CamerIndex.SelectedIndex = Project.cfg.CamIndex;
            }
            if (Project.cfg.Lang != 0)
            {
                Camer.Items.Clear();
                Camer.Items.Add("HIKVISION");
                Camer.SelectedIndex = 0;
            }
			
            //显示上次设置的
            for(int i=0;i< Rotation.Items.Count;i++)
            {
                ComboBoxItem cbi = (ComboBoxItem)Rotation.Items[i];
                string selectedText = cbi.Content.ToString();
                if(selectedText == Project.cfg.CamRotation.ToString())
                {
                    Rotation.SelectedIndex = i;
                    break;
                }
            }

        }

        //Cam
        private void OnBnClickedEnsure(object sender, RoutedEventArgs e)
        {
            // Project.cfg.ExposureTime = (uint)ScrollBar.Value;
            Project.cfg.CamTop = Y;
            Project.cfg.CamLeft = X;
            Project.cfg.Camer = Camer.SelectedIndex;
            Project.cfg.CamIndex = CamerIndex.SelectedIndex;//保存相机序号
            int angle = get_cam_rotation();
            Project.cfg.CamRotation = angle;
            //Project.cfg.ExposureTime=uint.Parse(Cam.Text.Trim());
            Project.SaveConfig("Config.xml");
            //Project.cam.CloseCam();
            this.Close();

        }

        private int get_cam_rotation()
        {
            ComboBoxItem cbi = (ComboBoxItem)Rotation.SelectedItem;
            string selectedText = cbi.Content.ToString();
            int angle = int.Parse(selectedText);
            return angle;
        }

        private void V110n_Selected(object sender, MouseButtonEventArgs e)
        {
            //V110.IsChecked = true;
        }

        private void HikVision_Selected(object sender, MouseButtonEventArgs e)
        {
            //Hikvision.IsChecked = true;
        }

        private void OnRotation(object sender, RoutedEventArgs e)
        {
            int angle = get_cam_rotation();
            if (Project.cam != null)
            {
                Project.cam.rotation(angle);
            }
        }
    }
}
