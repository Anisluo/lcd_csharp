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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LCD.View
{
    /// <summary>
    /// 电流显示
    /// </summary>
    public partial class EleTemplate : UserControl
    {
        public delegate void ShowDataDelegate(double x, double px, double y, double py);
        public event ShowDataDelegate ShowData;

        public double xmeter;
        public double ymeter;
        public double xpercent;
        public double ypercent;

        public EleTemplate()
        {
            InitializeComponent();
        }

        private void SetContent(int id)
        {
            mycontent.Content = id.ToString("000");//使用三个占位符
        }

        //显示数据
        private void OnBnClickedShowData(object sender, RoutedEventArgs e)
        {
            ShowData(xmeter, xpercent, ymeter, ypercent);
        }



    }
}
