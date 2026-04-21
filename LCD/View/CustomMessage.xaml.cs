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
    /// CustomMessage.xaml 的交互逻辑
    /// </summary>
    public partial class CustomMessage : Window
    {
        public static bool IsOK;
        public CustomMessage(string message )
        {
            InitializeComponent();
            mymessage.Text = message;
            IsOK = false;
        }

        private void OnBnClickedEnsure(object sender,RoutedEventArgs e)
        {
            IsOK = true;
            this.Close();
        }

        private void OnBnClickedCancel(object sender,RoutedEventArgs e)
        {
            IsOK = false;
            this.Close();
        }




    }
}
