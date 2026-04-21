using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// 自检视图
    /// </summary>
    public partial class SelfCheck : Window
    {
        //ObservableCollection<ModelInfo> lstModel = new ObservableCollection<ModelInfo>();
        DataTable table = new DataTable();
        
        public SelfCheck()
        {
            InitializeComponent();
            table.Columns.Add("设备名称");
            table.Columns.Add("设备状态");


            table.Rows.Add(new object[] { "X运动轴", "待定" });
            table.Rows.Add(new object[] { "Y运动轴", "待定" });
            table.Rows.Add(new object[] { "Z运动轴", "待定" });
            table.Rows.Add(new object[] { "U运动轴", "待定" });
            table.Rows.Add(new object[] { "V运动轴", "待定" });
            table.Rows.Add(new object[] { "响应仪器", "待定" });
            table.Rows.Add(new object[] { "相机", "待定" });
            table.Rows.Add(new object[] { "PG", "待定" });

            //lstModel.Add(new ModelInfo() {Name="X运动轴",Status="待定" });
            //lstModel.Add(new ModelInfo() {Name="Y运动轴",Status="待定" });
            //lstModel.Add(new ModelInfo() {Name="Z运动轴",Status="待定" });
            //lstModel.Add(new ModelInfo() {Name="U运动轴",Status="待定" });
            //lstModel.Add(new ModelInfo() {Name="V运动轴",Status="待定" });
            //lstModel.Add(new ModelInfo() {Name="Ball运动轴",Status="待定" });
            //lstModel.Add(new ModelInfo() {Name="响应仪器",Status="待定" });
            //lstModel.Add(new ModelInfo() {Name="相机",Status="待定" });
            DataView dv = new DataView(table);

            mydata.ItemsSource = dv;
            mydata.HorizontalGridLinesBrush = Brushes.Transparent;
            //mydata.Columns[0].Width = 120;
            //mydata.Columns[1].Width = 300;

        }

        private void OnBnClickedEnsure(object sender,RoutedEventArgs e)
        {

        }
    }
    public class ModelInfo
    {
        public string Name { get; set; }
        public string Status { get; set; }

    }
}
