using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LCD.View
{
    /// <summary>
    /// 测试项视图
    /// </summary>
    public partial class WortItemView : UserControl
    {


        TranslateTransform totalTranslate = new TranslateTransform();
        TranslateTransform tempTranslate = new TranslateTransform();
        ScaleTransform totalScale = new ScaleTransform();
        Double scaleLevel = 1;

        public WortItemView()
        {
            InitializeComponent();
            //Canvas mytempcanvas = new Canvas();
            //mytempcanvas.Width =350;
            //mytempcanvas.Height = 200;
            //mytempcanvas.Background = Brushes.LightGray;
            //double xmargin = 350 / 11;
            //double ymargin = 200 / 11;

            //for (int i = 0; i < 10; i++)
            //{
            //    for (int j = 0; j < 10; j++)
            //    {
            //        Ellipse ele = new Ellipse();
            //        ele.Width = 10;
            //        ele.Height = 10;
            //        Canvas.SetLeft(ele,(i+1) * xmargin);
            //        Canvas.SetTop(ele,(j+1) * ymargin);
            //        ele.Fill = Brushes.Red;
            //        mytempcanvas.Children.Add(ele);
            //    }
            //}
            //Canvas.SetLeft(mytempcanvas, 100);
            //Canvas.SetTop(mytempcanvas, 10);
            //myinnerCanvas.Children.Add(mytempcanvas);
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


        private void canvas1_MouseWheel(object sender, MouseWheelEventArgs e)
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
            //Console.WriteLine("scaleLevel: {0}", scaleLevel);

            totalScale.ScaleX = scaleLevel;
            totalScale.ScaleY = scaleLevel;
            totalScale.CenterX = scaleCenter.X;
            totalScale.CenterY = scaleCenter.Y;

            adjustGraph();
        }
    }
}
