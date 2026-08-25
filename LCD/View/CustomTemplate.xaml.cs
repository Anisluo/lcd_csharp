using System;
using System.Collections.Generic;
using System.Data;
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
    /// 自定义点位逻辑
    /// </summary>
    public partial class CustomTemplate : Window
    {

        public PtModel ptmodel = new PtModel();

        TranslateTransform totalTranslate = new TranslateTransform();
        TranslateTransform tempTranslate = new TranslateTransform();
        ScaleTransform totalScale = new ScaleTransform();
        Double scaleLevel = 1;

        public static DataTable dt = null;
        public static bool IsEnsure = false;

        public CustomTemplate()
        {
            InitializeComponent();
            Data2UI();
            InitCanvas();
        }


        private void GenerateData()
        {
            //DataRow dr = dt.NewRow();//添加新行    

            //dr["X(mm)"] = 1;
            //dr["Y(mm)"] = 1;
        }



        private void InitCanvas()
        {
            Canvas mytempcanvas = new Canvas();
            mytempcanvas.Width = 250;
            mytempcanvas.Height = 200;
            mytempcanvas.Background = Brushes.LightGray;
            double xmargin = 250 / 11;
            double ymargin = 200 / 11;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    EleTemplate ele = new EleTemplate();
                    //Ellipse ele = new Ellipse();
                    ele.Width = 10;
                    ele.Height = 10;
                    ele.ShowData += new EleTemplate.ShowDataDelegate(ShowData);
                    Canvas.SetLeft(ele, (i + 1) * xmargin);
                    Canvas.SetTop(ele, (j + 1) * ymargin);
                    mytempcanvas.Children.Add(ele);
                }
            }
            Canvas.SetLeft(mytempcanvas, 20);
            Canvas.SetTop(mytempcanvas, 10);
            myinnerCanvas.Children.Add(mytempcanvas);

        }

        private void ShowData(double x, double px, double y, double py)
        {
            ptmodel.Xmeter = x;
            ptmodel.Ymeter = y;
            ptmodel.Xpercent = px;
            ptmodel.Ypercent = py;
            this.DataContext = null;
            this.DataContext = ptmodel;
        }

        /// <summary>
        /// 保存模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickSaveTemplate(object sender, RoutedEventArgs e)
        {
            IsEnsure = true;
            this.Close();

        }




        private void GenerateNewTemplate()
        {
            Canvas mytempcanvas = new Canvas();
            double height = 300 * (ptmodel.productWidth / ptmodel.productLength);
            double length = 300;
            double percent = ptmodel.productLength / 300;//获取比例尺

            mytempcanvas.Width = length;
            mytempcanvas.Height = height;
            mytempcanvas.Background = Brushes.LightGray;
            double xmargin = (ptmodel.productLength - (ptmodel.Ameter * 2)) / (ptmodel.column - 1) / percent;
            double ymargin = (ptmodel.productWidth - (ptmodel.Bmeter * 2)) / (ptmodel.row - 1) / percent;


            dt.Rows.Clear();//清空数据
            for (int i = 0; i < ptmodel.row; i++)//ch 20220723
            {
                for (int j = 0; j < ptmodel.column; j++)
                {
                    DataRow dr = dt.NewRow();
                    EleTemplate ele = new EleTemplate();
                    ele.Width = 20;
                    ele.Height = 20;
                    ele.xmeter = j * xmargin * percent + ptmodel.Ameter;
                    ele.ymeter = i * ymargin * percent + ptmodel.Bmeter;
                    dr["X(mm)"] = ele.xmeter;
                    dr["Y(mm)"] = ele.ymeter;
                    dt.Rows.Add(dr);
                    ele.ShowData += new EleTemplate.ShowDataDelegate(ShowData);
                    Canvas.SetLeft(ele, j * xmargin + ptmodel.Ameter / percent - 10);
                    Canvas.SetTop(ele, i * ymargin + ptmodel.Bmeter / percent - 10);
                    mytempcanvas.Children.Add(ele);
                }
            }
            Canvas.SetLeft(mytempcanvas, 20);
            Canvas.SetTop(mytempcanvas, 10);
            myinnerCanvas.Children.Clear();
            myinnerCanvas.Children.Add(mytempcanvas);
        }





        //生成模板
        private void OnBnClickGenerateTemplate(object sender, RoutedEventArgs e)
        {
            GenerateNewTemplate();
        }


        private void Data2UI()
        {

            this.DataContext = ptmodel;
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point scaleCenter = e.GetPosition((Canvas)sender);
            if (e.Delta > 0)
            {
                scaleLevel *= 1.08;
            }
            else
            {
                scaleLevel /= 1.08;
            }
            totalScale.ScaleX = scaleLevel;
            totalScale.ScaleY = scaleLevel;
            totalScale.CenterX = scaleCenter.X;
            totalScale.CenterY = scaleCenter.Y;
            adjustGraph();
        }

        private static bool isMoving = false;
        Point startMovePosition;
        private void myinnerCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startMovePosition = e.GetPosition((Canvas)sender);
            isMoving = true;
        }

        private void myinnerCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMoving = false;
            Point endMovePosition = e.GetPosition((Canvas)sender);

            totalTranslate.X += (endMovePosition.X - startMovePosition.X) / scaleLevel;
            totalTranslate.Y += (endMovePosition.Y - startMovePosition.Y) / scaleLevel;
        }

        private void myinnerCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                Point currentMousePosition = e.GetPosition((Canvas)sender);//当前鼠标位置

                Point deltaPt = new Point(0, 0);
                deltaPt.X = (currentMousePosition.X - startMovePosition.X) / scaleLevel;
                deltaPt.Y = (currentMousePosition.Y - startMovePosition.Y) / scaleLevel;

                tempTranslate.X = totalTranslate.X + deltaPt.X;
                tempTranslate.Y = totalTranslate.Y + deltaPt.Y;
                adjustGraph();
            }
        }

        private void adjustGraph()
        {
            TransformGroup tfGroup = new TransformGroup();
            tfGroup.Children.Add(tempTranslate);
            tfGroup.Children.Add(totalScale);

            foreach (UIElement ue in myinnerCanvas.Children)
            {
                ue.RenderTransform = tfGroup;
            }
        }

        public class PtModel
        {
            public string tempName { get; set; }
            /// <summary>
            /// 屏幕长度
            /// </summary>
            public double productLength { get; set; }
            public double productWidth { get; set; }
            //毫米定位

            public int row { get; set; }
            public int column { get; set; }


            public bool IsMeter { get; set; }
            /// <summary>
            /// 定位参数
            /// </summary>
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

    }
}
