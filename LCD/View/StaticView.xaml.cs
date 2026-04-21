using SciChart.Charting3D.Model;
using SciChart.Examples.ExternalDependencies.Data;
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
    /// 3D视图
    /// </summary>
    public partial class StaticView : UserControl
    {

        public Action<double[] ,double[] ,double[] > Data { get; set; }

        public StaticView()
        {
            InitializeComponent();


            var xyzDataSeries3D = new XyzDataSeries3D<double>();
            var random = new Random(0);
            for (var i = 0; i < 1000; i++)
            {
                var x = 5 * Math.Sin(i);
                var y = i;
                var z = 5 * Math.Cos(i);
                Color? randomColor = Color.FromArgb(0xFF, (byte)random.Next(50, 255), (byte)random.Next(50, 255), (byte)random.Next(50, 255));
                var scale = (float)((random.NextDouble() + 0.5) * 3.0);
                xyzDataSeries3D.Append(x, y, z, new PointMetadata3D(randomColor, scale));
            }
            PointLineSeries3D.DataSeries = xyzDataSeries3D;


            //var xyzDataSeries3D = new XyzDataSeries3D<double>();
            //for (var i = 0; i < 100; i++)
            //{
            //    var x = DataManager.Instance.GetGaussianRandomNumber(5, 1.5);
            //    var y = DataManager.Instance.GetGaussianRandomNumber(5, 1.5);
            //    var z = DataManager.Instance.GetGaussianRandomNumber(5, 1.5);
            //    xyzDataSeries3D.Append(x, y, z);
            //}


            //ScatterSeries3D.DataSeries = xyzDataSeries3D;
            ////PointMarkerCombo.SelectedIndex = 0;

        }
    }
}
