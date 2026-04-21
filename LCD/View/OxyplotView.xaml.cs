using OxyPlot.Series;
using OxyPlot;
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
using OxyPlot.Axes;
using OxyPlot.Wpf;
using LCD.Ctrl;
using Microsoft.DwayneNeed.Win32.User32;
using OxyPlot.Annotations;

namespace LCD.View
{
    /// <summary>
    /// OxyplotView.xaml 的交互逻辑
    /// </summary>
    public partial class OxyplotView : UserControl
    {
        public PlotModel _model;
        private LineSeries lineSeries;

        public OxyplotView()
        {
            InitializeComponent();

            string title = "采样曲线";
            if (Project.cfg.Lang != 0)
            {
                title = "sampling graph";
            }
            _model = new PlotModel { Title = title };
            _model.TitleColor = OxyColors.Black;//标题字体颜色
            PlotChart.Model = _model;
            OxyPlot.Axes.Axis xa = new OxyPlot.Axes.LinearAxis();//创建x轴，对数刻度
            xa.Position = OxyPlot.Axes.AxisPosition.Bottom;//坐标轴位置，底部
            title = "采样点";
            if (Project.cfg.Lang != 0)
            {
                title = "sampling point";
            }
            xa.Title = title;//坐标轴名称
            xa.Minimum = 0; //坐标轴最小值
            int BytesRead = 100000;
            xa.Maximum = BytesRead;//坐标轴最大值          
            xa.MinorGridlineStyle = LineStyle.Solid;  //x轴网格线，线类型为实线                 
            _model.Axes.Add(xa);//绘图模块添加坐标轴
            OxyPlot.Axes.Axis xb = new OxyPlot.Axes.LinearAxis(); //实例化y轴
            xb.Position = OxyPlot.Axes.AxisPosition.Left;//y轴位置
            title = "采样值";
            if (Project.cfg.Lang != 0)
            {
                title = "sampling value";
            }
            xb.Title = title;//坐标轴名称
            _model.Axes.Add(xb);//绘图模块添加坐标轴
            lineSeries = new LineSeries();//实例化绘图线
            lineSeries.MarkerStroke = OxyColors.Red;
            //测试点啊
            //for (int i = 0; i < 1000; i++)
            //{
            //    lineSeries.Points.Add(new DataPoint(i, i));//将valuenow为x值，value为y值
            //}
            _model.Series.Add(lineSeries);//绘图模块条件绘制线条
            _model.InvalidatePlot(true);
        }

        public void clear()
        {
            lineSeries.Points.Clear();
            _model.InvalidatePlot(true);//刷新绘图区域
        }

        //绘制曲线啊
        public void draw(List<double> slist,RiseFallPositon positon)
        {
            lineSeries.Points.Clear();
            //清除注释啊
            _model.Annotations.Clear();
            for (int i = 0; i < slist.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, slist[i]));//将valuenow为x值，value为y值
            }
            int size = 2;
            if (positon.rise_start >= 0)
            {
                //画点注释
                var pointAnnotation = (new PointAnnotation
                {
                    X = positon.rise_start,
                    Y = slist[positon.rise_start],
                    Size = size,
                    Fill = OxyColors.Red,
                    Shape = MarkerType.Circle,
                });
                _model.Annotations.Add(pointAnnotation);
                pointAnnotation = (new PointAnnotation
                {
                    X = positon.rise_end,
                    Y = slist[positon.rise_end],
                    Size = size,
                    Fill = OxyColors.Red,
                    Shape = MarkerType.Circle,
                });

                _model.Annotations.Add(pointAnnotation);
            }
            if (positon.fall_start > 0)
            {
                var pointAnnotation = (new PointAnnotation
                {
                    X = positon.fall_start,
                    Y = slist[positon.fall_start],
                    Size = size,
                    Fill = OxyColors.BlueViolet,
                    Shape = MarkerType.Circle,
                });
                _model.Annotations.Add(pointAnnotation);
                pointAnnotation = (new PointAnnotation
                {
                    X = positon.fall_end,
                    Y = slist[positon.fall_end],
                    Size = size,
                    Fill = OxyColors.BlueViolet,
                    Shape = MarkerType.Circle,
                });
                _model.Annotations.Add(pointAnnotation);
            }
            _model.InvalidatePlot(true);//刷新绘图区域
        }
    }
}
