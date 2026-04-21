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
    /// WarnMachineConfig.xaml 的交互逻辑
    /// </summary>
    public partial class WarnMachineConfig : Window
    {
        public bool is_set = false;
        public int minutes = 0;
        public WarnMachineConfig()
        {
            InitializeComponent();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            is_set = false;
            this.Close();
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (checbox.IsChecked == true)
            {
                if (time.Text.Length == 0)
                {
                    MessageBox.Show("请输入暖机时间");
                    time.Focus();
                    return;
                }
                int val = 0;
                try
                {
                    val = int.Parse(time.Text);
                }
                catch
                {
                    MessageBox.Show("请输入正确的暖机时间");
                    time.Focus();
                    return;
                }
                if(val<=0)
                {
                    MessageBox.Show("请输入正确的暖机时间");
                    time.Focus();
                    return;
                }
                minutes = val;
                is_set = true;
                this.Close();
                return;
            }
            else
            {
                is_set = false;
            }            
            this.Close();
        }

        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
           
            time.IsEnabled = false;
        }

        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            time.IsEnabled = true;
        }
    }
}
