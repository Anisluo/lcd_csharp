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
    /// 响应时间测试模板
    /// </summary>
    public partial class ResponseView : Window
    {
        public ResponseView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 点击Ensure进行测量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedEnsure(object sender,RoutedEventArgs e)
        {

        }
    }
}
