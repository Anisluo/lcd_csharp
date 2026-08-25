using LCD.ViewMode;
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
    /// PG.xaml 的交互逻辑
    /// </summary>
    public partial class PGDebug : UserControl
    {
        List<PGViewsModel> list = new List<PGViewsModel>();// InfoList
        public PGDebug()
        {
            InitializeComponent();
            list.Clear();
            for (int i = 0; i < Project.PG.PatternList.Size; i++)
            {
                PGViewsModel infoList = new PGViewsModel();
                infoList.Name2 = i.ToString();
                infoList.Name1 = Project.PG.PatternList.ItemStrings[i].name;
                list.Add(infoList);
            }
            mylist1.ItemsSource = list;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Image = ImageSwitching.Text;
            int Imag = 0;
            try
            {
                Imag = int.Parse(Image);
            }
            catch (Exception E)
            {

                MessageBox.Show("请输入数字");
            }

            Project.PG.changePattern(Project.PG.PatternList.ItemStrings[Imag].name);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            byte r=0, g=0, b = 0;
            try
            {
                r= byte.Parse(R.Text);
                g= byte.Parse(G.Text);
                b= byte.Parse(B.Text);
            }
            catch (Exception E)
            {
                MessageBox.Show("请输入数字");
            }
            Project.PG.colorControl(r,g,b);
        }
    }
}
