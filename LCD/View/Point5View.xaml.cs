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
    /// Point5View.xaml 的交互逻辑
    /// </summary>
    public partial class Point5View : Window
    {

        //五点数据模型
        private Pt5Model ptModel = new Pt5Model();
        public Point5View()
        {
            InitializeComponent();
            Data2UI();

        }

        private void Data2UI()
        {
            //刷新数据

            //数据绑定
            this.DataContext = ptModel;
        }


        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedEnsure(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void OnBnClickedSave(object sender, RoutedEventArgs e)
        {


        }


        public class Pt5Model
        {
            public string tempName { get; set; }
            public double productLength { get; set; }
            public double productWidth { get; set; }
            //毫米定位
            public bool IsMeter { get; set; }
            public double Ameter { get; set; }
            public double Bmeter { get; set; }

            public double Apercent { get; set; }
            public double Bpercent { get; set; }

            public int SerNo { get; set; }
            public double Xmeter { get; set; }
            public double Xpercent { get; set; }
            public double Ymeter { get; set; }
            public double Ypercent { get; set; }
            public bool IsLchk { get; set; }
            public double Lmin { get; set; }
            public double Lmax { get; set; }
            public bool Isxchk { get; set; }
            public double xmin { get; set; }
            public double xmax { get; set; }
            public bool Isychk { get; set; }
            public double ymin { get; set; }
            public double ymax { get; set; }
        }

        private void Buttonsave_Click(object sender, RoutedEventArgs e)
        {

        }
    }


}
