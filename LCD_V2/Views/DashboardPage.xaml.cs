using System.Collections.Generic;
using System.Windows.Controls;

namespace LCD_V2.Views
{
    public partial class DashboardPage : UserControl
    {
        public DashboardPage()
        {
            InitializeComponent();

            // Mock "当前测试" session: 13 点对角模板, simulated progress 到第 4 点
            var templatePoints = new List<TemplatePoint>
            {
                new TemplatePoint { No = 1,  X = 0.00,  Y = 0.00,  Z = 0.00, U = 0.00, V = 0.00, Remark = "中心"   },
                new TemplatePoint { No = 2,  X = -57.15, Y = -34.29, Z = 0.00, U = 0.00, V = 0.00, Remark = "左上"   },
                new TemplatePoint { No = 3,  X = 57.15,  Y = -34.29, Z = 0.00, U = 0.00, V = 0.00, Remark = "右上"   },
                new TemplatePoint { No = 4,  X = 57.15,  Y = 34.29,  Z = 0.00, U = 0.00, V = 0.00, Remark = "右下"   },
                new TemplatePoint { No = 5,  X = -57.15, Y = 34.29,  Z = 0.00, U = 0.00, V = 0.00, Remark = "左下"   },
                new TemplatePoint { No = 6,  X = -28.58, Y = -17.15, Z = 0.00, U = 0.00, V = 0.00, Remark = "左上内" },
                new TemplatePoint { No = 7,  X = 28.58,  Y = -17.15, Z = 0.00, U = 0.00, V = 0.00, Remark = "右上内" },
                new TemplatePoint { No = 8,  X = 28.58,  Y = 17.15,  Z = 0.00, U = 0.00, V = 0.00, Remark = "右下内" },
                new TemplatePoint { No = 9,  X = -28.58, Y = 17.15,  Z = 0.00, U = 0.00, V = 0.00, Remark = "左下内" },
                new TemplatePoint { No = 10, X = 0.00,  Y = -34.29, Z = 0.00, U = 0.00, V = 0.00, Remark = "上中"   },
                new TemplatePoint { No = 11, X = 57.15,  Y = 0.00,  Z = 0.00, U = 0.00, V = 0.00, Remark = "右中"   },
                new TemplatePoint { No = 12, X = 0.00,  Y = 34.29,  Z = 0.00, U = 0.00, V = 0.00, Remark = "下中"   },
                new TemplatePoint { No = 13, X = -57.15, Y = 0.00,  Z = 0.00, U = 0.00, V = 0.00, Remark = "左中"   },
            };
            TemplateGrid.ItemsSource = templatePoints;
            // 光标停在第 4 点：前 3 点已完成并显示结果, 第 4 点正在测量
            TemplateGrid.SelectedIndex = 3;
            TemplateGrid.ScrollIntoView(templatePoints[3]);

            var results = new List<TestResult>
            {
                new TestResult { No = 1, L = 315.4, Cx = 0.3127, Cy = 0.3291, CCT = 6503, Status = "OK" },
                new TestResult { No = 2, L = 298.7, Cx = 0.3133, Cy = 0.3284, CCT = 6478, Status = "OK" },
                new TestResult { No = 3, L = 306.2, Cx = 0.3124, Cy = 0.3298, CCT = 6521, Status = "OK" },
                new TestResult { No = 4, L = null,  Cx = null,   Cy = null,   CCT = null, Status = "测量中" },
            };
            ResultGrid.ItemsSource = results;
            ResultGrid.SelectedIndex = 3;
            ResultGrid.ScrollIntoView(results[3]);
        }
    }

    public sealed class TemplatePoint
    {
        public int No { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double U { get; set; }
        public double V { get; set; }
        public string Remark { get; set; }
    }

    public sealed class TestResult
    {
        public int No { get; set; }
        public double? L { get; set; }
        public double? Cx { get; set; }
        public double? Cy { get; set; }
        public double? CCT { get; set; }
        public string Status { get; set; }
    }
}
