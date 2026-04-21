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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LCD.Data;

namespace LCD.View
{
    /// <summary>
    /// 数据显示视图(目前是所有的全都转成string来实现数据传递)
    /// </summary>
    public partial class DataTemplateView : UserControl
    {
        //设置数据表
        private DataTable dt = new DataTable();
        public DataTemplateView()
        {
            InitializeComponent();
            Init(null, ENUMMESSTYLE._01_POINT,"");
            mydata.SelectedIndex = 0;
        }

        public void ShowIndex(int index)
        {
            //mydata.SelectedIndex = index;
        }

        /// <summary>
        /// 定义初始化列表
        /// </summary>
        public DataTable Init(List<string> lstdata, ENUMMESSTYLE MESTYPE , string InitDataTemplate)
        {
            DataTable mytable = null;

            

            if (lstdata == null)
            {
                if (dt != null) { dt.Dispose(); }
                dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("X(mm)");
                dt.Columns.Add("Y(mm)");
                dt.Columns.Add("Z(mm)");
                dt.Columns.Add("U(°)");
                dt.Columns.Add("V(°)");
                dt.Columns.Add("PG提示信息");

                DataView dv = new DataView(dt);
                ///定义数据表
                mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
                mydata.VerticalGridLinesBrush = Brushes.Gray;
                //mydata.HorizontalAlignment = HorizontalAlignment.Center;
                mydata.MinColumnWidth = 70;
                mydata.CanUserAddRows = false;
                mydata.CanUserDeleteRows = false;

                mydata.ItemsSource = dv;
                //for (int i = 0; i < 40; i++)
                //{
                //    DataRow dr = dt.NewRow();
                //    dr["ID"] = i;
                //    dr["X(mm)"] = 100.00;
                //    dr["Y(mm)"] = 120.00;
                //    dr["Z(mm)"] = 120.00;
                //    dr["U(°)"] = 120.00;
                //    dr["V(°)"] = 120.00;
                //    dt.Rows.Add(dr);
                //}
                //初始化三维坐标空间
                mytable = dt;
            }
            //非null控制
            else
            {
                switch (MESTYPE)
                {
                    case ENUMMESSTYLE._01_POINT: Init_POINT(lstdata, InitDataTemplate); break;
                    case ENUMMESSTYLE._02_RESPONSE: Init_POINT(lstdata, InitDataTemplate); break;
                    case ENUMMESSTYLE._03_SPECTRUM: Init_POINT(lstdata, InitDataTemplate); break;
                    case ENUMMESSTYLE._04_FLICKER: Init_POINT(lstdata, InitDataTemplate); break;
                    case ENUMMESSTYLE._05_CROSSTALK: Init_POINT(lstdata, InitDataTemplate); break;
                    case ENUMMESSTYLE._06_ACR: Init_POINT(lstdata, InitDataTemplate); break;
                    case ENUMMESSTYLE._07_warmup: Init_POINT(lstdata, InitDataTemplate); break;
                    case ENUMMESSTYLE.TCO: Init_POINT(lstdata, InitDataTemplate); break;

                }


            }
            mytable = dt;
            return mytable;
        }




        /// <summary>
        /// 定义初始化列表
        /// </summary>
        public void Init_POINT(List<string> lstdata ,String InitDataTemplate)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                TextBlock.Text = InitDataTemplate;
                if (lstdata != null)
                {
                    if (dt != null) { dt.Dispose(); }

                    dt = new DataTable();

                    string[] strheader = lstdata[0].Split(',');
                    for (int i = 0; i < strheader.Length; i++)
                    {
                        if (strheader[i] != "") { dt.Columns.Add(strheader[i]); }
                    }
                    DataView dv = new DataView(dt);
                    ///定义数据表
                    mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
                    mydata.VerticalGridLinesBrush = Brushes.Gray;
                    //mydata.HorizontalAlignment = HorizontalAlignment.Center;
                    mydata.MinColumnWidth = 70;
                    mydata.CanUserAddRows = false;
                    mydata.CanUserDeleteRows = false;
                    mydata.ItemsSource = dv;

                    for (int i = 1; i < lstdata.Count; i++)
                    {
                        string[] strs = lstdata[i].Split(',');
                        DataRow dr = dt.NewRow();

                        for (int k = 0; k < strheader.Length; k++)
                        {
                            if (strs[k] != "") { dr[strheader[k]] = strs[k]; }
                        }
                        dt.Rows.Add(dr);
                    }
                    //初始化三维坐标空间
                }


            }));
        }
    }
}
