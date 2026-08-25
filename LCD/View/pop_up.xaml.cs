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
    /// pop_up.xaml 的交互逻辑
    /// </summary>
    public partial class pop_up : Window
    {
        public string Time = "";
        public pop_up( string Time,string Tip)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Time = Time;
            InitializeComponent();
            lblName.Text= Time;
            Titel.Text = Tip;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            this.Time = lblName.Text;
            this.Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
