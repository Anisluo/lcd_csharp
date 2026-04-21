using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LCD.View
{
    /// <summary>
    /// ShowMsg.xaml 的交互逻辑
    /// </summary>
    public partial class ShowMsg : Window
    {
        private string tille;
        public ShowMsg(string tille)
        {
            InitializeComponent();
            this.tille = tille;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Topmost = true;

            this.Loaded += ShowMsg_Loaded;
        }

        private void ShowMsg_Loaded(object sender, RoutedEventArgs e)
        {
            Title.Text = tille;            
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }  
}
