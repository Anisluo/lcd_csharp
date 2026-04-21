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
        public delegate void ShowDataDelegate(object sender,int SerNo,double x, double px, double y, double py);
        public event ShowDataDelegate ShowData;

        public int SerNo;
        public double xmeter;
        public double ymeter;
        public double xpercent;
        public double ypercent;
        private bool is_selected = false;

        public int id = 0;

        public EleTemplate()
        {
            InitializeComponent();
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public void SetContent(int serno)
        {
            //默认的SerNo就是ID啊
            SerNo = serno;
            mycontent.Content = serno.ToString();//使用2个占位符
        }

        public void SetColor(Brush color)
        {
            brd.Background = color;
            var background1 = brd.Background as SolidColorBrush;
            if (background1.Color == Colors.Red)
            {
                is_selected =  true;
            }
            else
            {
                is_selected = false;
            }
        }

        public bool is_select()
        {
            return is_selected;
        }

        //显示数据
        public void OnBnClickedShowData(object sender, RoutedEventArgs e)
        {
            ShowData(this,SerNo,xmeter, xpercent, ymeter, ypercent);
        }
    }
}
