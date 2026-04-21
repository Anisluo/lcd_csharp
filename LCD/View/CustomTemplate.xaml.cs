using LCD.Data;
using LCD.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VisionCore;
using static LCD.View.CustomView;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media;
using SciChart.Core.Extensions;
using Microsoft.DwayneNeed.Win32.Gdi32;



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

        public static System.Data.DataTable dt = null;
        public static bool IsEnsure = false;

        private Info info;
        private InfoData infoData;
        private DataTable ResultDatatemp;

        //软件是否在更新
        private bool manul_update = false;
        private bool is_closing = false;
        private DateTime last_time = DateTime.Now;

        public CustomTemplate(Info info, InfoData infoData)
        {
            InitializeComponent();
            this.info = info;    
            this.infoData = infoData;
            Data2UI();
            //InitCanvas5();
            this.info.table.DefaultView.AllowEdit = false;

            //是05类型的时候点数量不能改
            if (info.MESTYPE == ENUMMESSTYLE._05_CROSSTALK)
            {
                PointNum.IsEnabled = false;
            }

             init_set_datagrid();
            //标准数据赋值，如果为空的话表示是新建的
            if (infoData != null)
            {
                ptmodel.Isxchk = infoData.Isxchk;
                ptmodel.Isychk = infoData.Isychk;
                ptmodel.IsLchk = infoData.IsLchk;
                ptmodel.Lmax = infoData.Lmax;
                ptmodel.Lmin = infoData.Lmin;
                ptmodel.xmax = infoData.xmax;
                ptmodel.xmin = infoData.xmin;
                ptmodel.ymax = infoData.ymax;
                ptmodel.ymin = infoData.ymin;
                ptmodel.IsBalancechk = infoData.IsBalancechk;
                ptmodel.balancemin = infoData.balancemin;
                if ((infoData.warnR == 0) && (infoData.warnG == 0) && (infoData.warnB == 0))
                {
                    ptmodel.color = System.Drawing.Color.Red; 
                }
                else
                {
                    ptmodel.color = System.Drawing.Color.FromArgb(infoData.warnR, infoData.warnG, infoData.warnB);
                }
                update_warn_color(ptmodel.color);
                if (ptmodel.Isxchk)
                {
                    Xcheck.IsChecked = true;
                    xmax.Text = ptmodel.xmax.ToString();
                    xmin.Text = ptmodel.xmin.ToString();
                }
                if (ptmodel.Isychk)
                {
                    Ycheck.IsChecked = true;
                    ymax.Text = ptmodel.ymax.ToString();
                    ymin.Text = ptmodel.ymin.ToString();
                }
                if (ptmodel.IsLchk)
                {
                    Lcheck.IsChecked = true;
                    Lmax.Text = ptmodel.Lmax.ToString();
                    Lmin.Text = ptmodel.Lmin.ToString();
                }
                if (ptmodel.IsBalancechk)
                {
                    Balancechk.IsChecked = true;
                    balancemin.Text = ptmodel.balancemin.ToString();
                }

                //长和宽如果没值，给一个初始化值
                if(infoData.productLength ==0)
                {
                    infoData.productLength = 300;
                }
                if(infoData.productWidth == 0)
                {
                    infoData.productWidth = 200;
                }
                //参数设置
                ptmodel.productLength = infoData.productLength;
                ptmodel.productWidth = infoData.productWidth;
                ptmodel.Ameter = infoData.Ameter;
                ptmodel.Bmeter = infoData.Bmeter;
                ptmodel.Cmeter = infoData.Cmeter;
                ptmodel.Dmeter = infoData.Dmeter;
                ptmodel.Apercent = infoData.Apercent;
                ptmodel.Bpercent = infoData.Bpercent;
                ptmodel.Cpercent = infoData.Cpercent;
                ptmodel.Dpercent = infoData.Dpercent;
                ptmodel.IsMeter = infoData.IsMeter;
                if (ptmodel.IsMeter)
                {
                    isMeter.IsChecked = true;
                }
                else                    
                {
                    isPercent.IsChecked = true;
                }
            }

            ptmodel.PropertyChanged += Ptmodel_PropertyChanged;

            //展示啊
            int count = info.table.Rows.Count;
            switch (count)
            {
                case 4:
                    point_crosstalk.IsChecked = true;
                    PointChecked(point_crosstalk, null);
                    break;
                case 5:
                    point5.IsChecked = true;
                    PointChecked(point5,null);
                    break;
                case 9:
                    point9.IsChecked = true;
                    PointChecked(point9, null);
                    break;
                case 13:
                    point13.IsChecked = true;
                    PointChecked(point13, null);
                    break;
                default:
                    //pointother.IsChecked = true;
                    //PointChecked(pointother, null);
                    point5.IsChecked = true;
                    PointChecked(point5, null);
                    break;
            }

            update_draw_points();

            recalculate_meter_percent();

            if (info.MESTYPE == ENUMMESSTYLE._05_CROSSTALK)
            {
                point_crosstalk.IsChecked = true;
                PointChecked(point_crosstalk, null);
                //画_05_CROSSTALK图啊：中间一个框，四周4个点
                DrawRectangle();
                if (count==0)
                {
                    // 没有的时候自动创建4个点啊
                    var pts = PointLayoutService.Generate(PointLayoutType.CrossTalk4, BuildLayoutInput());
                    FillPointTable(pts);
                }
            }
        }

        private void init_set_datagrid()
        {
            ResultDatatemp = new DataTable();
            ResultDatatemp.Columns.Add("ID");
            ResultDatatemp.Columns.Add("X");
            ResultDatatemp.Columns.Add("Y");
            DataView dv = new DataView(ResultDatatemp);
            PointData.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            PointData.VerticalGridLinesBrush = System.Windows.Media.Brushes.Gray;
            PointData.CanUserSortColumns = false;
            PointData.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            PointData.ItemsSource = dv;

            init_datagrid_data();
        }

        private void init_datagrid_data()
        {
            ResultDatatemp.Rows.Clear();
            //添加数据啊
            int count = info.table.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow dataRow = ResultDatatemp.NewRow();
                dataRow["ID"] = info.table.Rows[i][0];
                dataRow["X"] = info.table.Rows[i][1];
                dataRow["Y"] = info.table.Rows[i][2];
                ResultDatatemp.Rows.InsertAt(dataRow, ResultDatatemp.Rows.Count + 1);
            }
            DataView dv = new DataView(ResultDatatemp);
            PointData.ItemsSource = dv;
        }

        private void update_datagrid_data()
        {
            ResultDatatemp.Rows.Clear();

            Canvas mytempcanvas = null;
            try
            {
                mytempcanvas = (Canvas)myinnerCanvas.Children[0];
            }
            catch(Exception ex)
            {
                return;
            }
            for (int i = 0; i < mytempcanvas.Children.Count; i++)
            {
                EleTemplate ele = find_element(i + 1);
                if(ele == null)
                {
                    continue;
                }
                string[] arys = new string[3];
                arys[0] = (i + 1).ToString();
                arys[1] = ele.xmeter.ToString("F2");
                arys[2] = ele.ymeter.ToString("F2");                
                DataRow dataRow = ResultDatatemp.NewRow();
                dataRow["ID"] = arys[0];
                dataRow["X"] = arys[1];
                dataRow["Y"] = arys[2];
                ResultDatatemp.Rows.InsertAt(dataRow, ResultDatatemp.Rows.Count + 1);
            }
            DataView dv = new DataView(ResultDatatemp);
            PointData.ItemsSource = dv;
        }

     
        private void Ptmodel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Console.WriteLine();
            //值变化了就进入了这个函数啊
            if (myinnerCanvas == null)
            {
                return;
            }
            if (myinnerCanvas.Children.Count <= 0)
            {
                return;
            }
            if(e.PropertyName == "tempName")
            {
                return;
            }
            if(manul_update)
            {
                manul_update = false;
                return;
            }
            if(is_closing)
            {
                return;
            }
            DateTime now = DateTime.Now;
            TimeSpan span = now - last_time;
            if(span.TotalMilliseconds < 100)
            {
                //防止进入死循环
                last_time = now;
                return;
            }
            last_time = now;
            //重新计算一下值，更新到数据表格，然后根据表格重新绘图
            recalculate();
        }

        private void recalculate()
        {
            if (infoData != null)
            {
                infoData.productLength = ptmodel.productLength;
                infoData.productWidth = ptmodel.productWidth;
            }

            // 毫米 ↔ 百分比 双向同步
            recalculate_meter_percent();

            // 走纯函数服务生成点集
            var layoutType = GetSelectedLayoutType();
            var points = PointLayoutService.Generate(layoutType, BuildLayoutInput());
            FillPointTable(points);

            update_draw_points();

            if (info.MESTYPE == ENUMMESSTYLE._05_CROSSTALK)
            {
                DrawRectangle();
            }
            update_datagrid_data();
        }

        private PointLayoutType GetSelectedLayoutType()
        {
            // CrossTalk 测试强制使用 4 点布局
            if (info.MESTYPE == ENUMMESSTYLE._05_CROSSTALK) return PointLayoutType.CrossTalk4;
            if (point5.IsChecked == true) return PointLayoutType.Point5;
            if (point9.IsChecked == true) return PointLayoutType.Point9;
            if (point13.IsChecked == true) return PointLayoutType.Point13;
            if (point13_diag.IsChecked == true) return PointLayoutType.Point13Diag;
            if (point15.IsChecked == true) return PointLayoutType.Point15;
            if (point17.IsChecked == true) return PointLayoutType.Point17;
            if (point17_ary.IsChecked == true) return PointLayoutType.Point17Array;
            if (point25.IsChecked == true) return PointLayoutType.Point25;
            return PointLayoutType.Custom;
        }

        private PointLayoutInput BuildLayoutInput()
        {
            return new PointLayoutInput
            {
                H = ptmodel.productLength,
                V = ptmodel.productWidth,
                Amm = ptmodel.Ameter,
                Bmm = ptmodel.Bmeter,
                Cmm = ptmodel.Cmeter,
                Dmm = ptmodel.Dmeter,
                Apct = ptmodel.Apercent,
                Bpct = ptmodel.Bpercent,
                Cpct = ptmodel.Cpercent,
                Dpct = ptmodel.Dpercent,
                UseMeter = ptmodel.IsMeter,
                Rows = ptmodel.row,
                Columns = ptmodel.column,
            };
        }

        private void FillPointTable(IReadOnlyList<PointPos> points)
        {
            info.table.Rows.Clear();
            foreach (var pt in points)
            {
                info.table.Rows.Add(new string[]
                {
                    pt.Id.ToString(),
                    pt.XMm.ToString("F2"),
                    pt.YMm.ToString("F2"),
                });
            }
        }

        private void recalculate_meter_percent()
        {
            if (ptmodel.IsMeter)
            {
                ptmodel.Apercent = ptmodel.Ameter * 100.0 / infoData.productLength;
                ptmodel.Bpercent = ptmodel.Bmeter * 100.0 / infoData.productWidth;
                ptmodel.Cpercent = ptmodel.Cmeter * 100.0 / infoData.productLength;
                ptmodel.Dpercent = ptmodel.Dmeter * 100.0 / infoData.productWidth;
            }
            else
            {
                ptmodel.Ameter = ptmodel.Apercent * infoData.productLength / 100.0;
                ptmodel.Bmeter = ptmodel.Bpercent * infoData.productWidth / 100.0;
                ptmodel.Cmeter = ptmodel.Cpercent * infoData.productLength / 100.0;
                ptmodel.Dmeter = ptmodel.Dpercent * infoData.productWidth / 100.0;
            }
        }

        private void DrawRectangle()
        {
            if (myinnerCanvas == null)
            {
                return;
            }

            //var Width = infoData.productLength;
            //var Height = infoData.productWidth;

            //double xscal = myinnerCanvas.Width / infoData.productLength;
            //double yscal = myinnerCanvas.Height / infoData.productWidth;

            //if (xscal > yscal)
            //{               
            //    Width = myinnerCanvas.Width*yscal;
            //    Height = myinnerCanvas.Height*yscal;
            //}
            //else
            //{
            //    Width = myinnerCanvas.Width * xscal;
            //    Height = myinnerCanvas.Height * xscal;
            //}
            //if(Width > myinnerCanvas.Width)
            //{
            //    Width = myinnerCanvas.Width;
            //    //Height = myinnerCanvas.Height* infoData.productWidth/ infoData.productLength;
            //}
            //if(Height > myinnerCanvas.Height)
            //{
            //    Height = myinnerCanvas.Height;
            //    //Width = myinnerCanvas.Width * infoData.productLength / infoData.productWidth;
            //}

            Canvas mytempcanvas = null;
            try
            {
                mytempcanvas = (Canvas)myinnerCanvas.Children[0];
            }
            catch (Exception ex)
            {
                return;
            }

            var width = mytempcanvas.Width;
            var height = mytempcanvas.Height;

            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
            {
                Width = width/3,
                Height = height/3,
                Stroke = System.Windows.Media.Brushes.Black,
                Fill = System.Windows.Media.Brushes.Black,
                StrokeThickness = 1,
            };

            Canvas.SetLeft(rectangle, width/3);
            Canvas.SetTop(rectangle, height/3);

            myinnerCanvas.Children.Add(rectangle);
        }


        private void update_draw_points()
        {
            if (myinnerCanvas == null)
            {
                return;
            }
            myinnerCanvas.Children.Clear();
            Canvas mytempcanvas = new Canvas();
            //显示范围是300*300，实际的显示器要缩放到这个区域啊
            //mytempcanvas.Width = 250;
            //mytempcanvas.Height = 200;
            double xscal = 1;
            double yscal = 1;
            if(infoData.productLength <=0)
            {
                return;
            }
            if (infoData.productWidth <= 0)
            {
                return;
            }
            mytempcanvas.Width = infoData.productLength;
            mytempcanvas.Height = infoData.productWidth;
            if (infoData.productLength != 300)
            {
                xscal = myinnerCanvas.Width / infoData.productLength;
                mytempcanvas.Width = myinnerCanvas.Width;
                yscal = xscal;
                mytempcanvas.Height = mytempcanvas.Height * yscal;                
            }

            if (mytempcanvas.Height > myinnerCanvas.Height)
            {
                yscal = myinnerCanvas.Height / infoData.productWidth;
                mytempcanvas.Height = myinnerCanvas.Height;
                xscal = yscal;
                mytempcanvas.Width = mytempcanvas.Width * xscal;
            }

            //不能超出去啊
            if (mytempcanvas.Height > myinnerCanvas.Height)
            {
                mytempcanvas.Height = myinnerCanvas.Height;
            }
            if (mytempcanvas.Width > myinnerCanvas.Width)
            {
                mytempcanvas.Width = myinnerCanvas.Width;
            }

            mytempcanvas.Background = System.Windows.Media.Brushes.LightGray;
            double xmargin = 250 / 11;
            double ymargin = 200 / 11;
            int ids = 0;
            int count = info.table.Rows.Count;
            {
                //按照数据来绘图啊
                for (int i = 0; i < count; i++)
                {
                    EleTemplate ele = new EleTemplate();
                    ele.Width = 18;
                    ele.Height = 18;
                    ele.ShowData += new EleTemplate.ShowDataDelegate(ShowData);

                    //有可能是没有数据的啊
                    try
                    {
                        ids = int.Parse(info.table.Rows[i][0].ToString());
                        xmargin = double.Parse(info.table.Rows[i][1].ToString());
                        ymargin = double.Parse(info.table.Rows[i][2].ToString());
                    }
                    catch 
                    {
                        continue;
                    }

                    double left = xmargin * xscal-ele.Width/2;
                    double top = ymargin * yscal-ele.Height/2;
                    if (left + ele.Width/2 > mytempcanvas.Width)
                    {
                        left -= ele.Width/2;
                    }
                    if (top + ele.Height/2 > mytempcanvas.Height)
                    {
                        top -= ele.Height/2;
                    }
                    Canvas.SetLeft(ele, left);
                    Canvas.SetTop(ele, top);
                    //首先做默认值的，如果传入过来有值的话，使用传入的啊
                    ele.xmeter = xmargin;
                    ele.ymeter = ymargin;
                    ele.xpercent = 100 * xmargin / ptmodel.productLength;
                    ele.ypercent = 100 * ymargin / ptmodel.productWidth;
                    ele.SetId(i);
                    //设置显示值
                    ele.SetContent(ids);
                    mytempcanvas.Children.Add(ele);
                }
            }
            myinnerCanvas.Children.Add(mytempcanvas);

            //画完后，选中第一个啊
            if (mytempcanvas.Children.Count > 0)
            {
                EleTemplate ele1 = (EleTemplate)mytempcanvas.Children[0];
                if (ele1 != null)
                {
                    ele1.OnBnClickedShowData(ele1, null);
                }
            }
        }

        private void ShowData(object sender,int SerNo1, double x, double px, double y, double py)
        {
            //显示设置值啊
            SerNo.Text = SerNo1.ToString();
            Xmeter.Text = x.ToString();
            Ymeter.Text = y.ToString();
            Xpercent.Text = px.ToString();
            Ypercent.Text = py.ToString();

            // 对于 13点对角 模式，第 1 个点位于中心而非 (A,B) 角点，
            // 不能把它的 x%/y% 回写到 Apercent/Bpercent，否则会覆盖 A/B 输入。
            if (point13_diag != null && point13_diag.IsChecked == true)
            {
                return;
            }

            //if (ptmodel.IsMeter)
            {
                if (ptmodel.Apercent != px)
                {
                    manul_update = true;
                    ptmodel.Apercent = px;
                }
                if (ptmodel.Bpercent != py)
                {
                    manul_update = true;
                    ptmodel.Bpercent = py;
                }
                if(ptmodel.Cpercent != (100*ptmodel.Cmeter/ptmodel.productLength))
                {
                    manul_update = true;
                    ptmodel.Cpercent = (100 * ptmodel.Cmeter / ptmodel.productLength);
                }
                if (ptmodel.Dpercent != (100 * ptmodel.Dmeter / ptmodel.productWidth))
                {
                    manul_update = true;
                    ptmodel.Dpercent = (100 * ptmodel.Dmeter / ptmodel.productWidth);
                }
            }
           
            //ptmodel.SerNo = SerNo1;
            //ptmodel.Ameter = x;
            //ptmodel.Bmeter = y;
            //ptmodel.Apercent = px;
            //ptmodel.Bpercent= py;
            //ptmodel.Xmeter = x;
            //ptmodel.Ymeter = y;
            //ptmodel.Xpercent = px;
            //ptmodel.Ypercent = py;
            //ptmodel.productLength = 100;
            //ptmodel.productWidth = 100;
            //this.DataContext = null;            
            //this.DataContext = ptmodel;

            //InputNumDialog inputNum = new InputNumDialog(1,5);
            //inputNum.ShowDialog();
            EleTemplate select = (EleTemplate)sender;
            //选中的颜色要变下
            Canvas mytempcanvas = (Canvas)myinnerCanvas.Children[0];
            for (int i = 0; i < mytempcanvas.Children.Count; i++)
            {
                EleTemplate ele = (EleTemplate)mytempcanvas.Children[i];
                if(ele.id == select.id)
                {
                    ele.SetColor(new SolidColorBrush(Colors.Red));
                }
                else
                {
                    ele.SetColor(new SolidColorBrush(Colors.Blue));
                }
            }
        }


        public static bool check_input(TextBox box, string name)
        {
            if(box.Text.Length == 0)
            {
                MessageBox.Show("请输入:"+name);
                box.Focus();
                return false;
            }
            double val = 0;
            try
            {
                val = double.Parse(box.Text);
            }
            catch
            {
                MessageBox.Show("请输入:" + name);
                box.Focus();
                return false;
            }
            return true;
        }


        /// <summary>
        /// 保存模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickSaveTemplate(object sender, RoutedEventArgs e)
        {
            //判断一下数据有效性
            //这里只能做安全检查，不能赋值，赋值会导致界面重❀，点顺序就变回去了
            if(Xcheck.IsChecked==true)
            {
                if(check_input(xmax,"X最大值")==false)
                {
                    return;
                }
                if (check_input(xmin, "X最小值") == false)
                {
                    return;
                }
                if (double.Parse(xmax.Text) < double.Parse(xmin.Text))
                {
                    MessageBox.Show("错误：测量标准中的X最大值比最小值小");
                    return;
                }               
            }
            
            if (Ycheck.IsChecked==true)
            {
                if (check_input(ymax, "Y最大值") == false)
                {
                    return;
                }
                if (check_input(ymin, "Y最小值") == false)
                {
                    return;
                }
                if (double.Parse(ymax.Text) < double.Parse(ymin.Text))
                {
                    MessageBox.Show("错误：测量标准中的Y最大值比最小值小");
                    return;
                }               
            }
            
            if (Lcheck.IsChecked ==true)
            {
                if (check_input(Lmax, "L最大值") == false)
                {
                    return;
                }
                if (check_input(Lmin, "L最小值") == false)
                {
                    return;
                }
                if (double.Parse(Lmax.Text) < double.Parse(Lmin.Text))
                {
                    MessageBox.Show("错误：测量标准中的L最大值比最小值小");
                    return;
                }               
            }
            
            if (Balancechk.IsChecked == true)
            {
                if(check_input(balancemin,"一致性")==false)
                {
                    return;
                }               
            }
            
           
            Canvas mytempcanvas = (Canvas)myinnerCanvas.Children[0];
            //判断数据的有效性，主要是序号有没有重复的啊
            List<int> sernos = new List<int>();
            for(int i=0;i< mytempcanvas.Children.Count;i++)
            {
                EleTemplate ele = (EleTemplate)mytempcanvas.Children[i];
                sernos.Add(ele.SerNo);
            }
            bool hasDuplicates = sernos.Distinct().Count() != sernos.Count;
            if(hasDuplicates)
            {
                MessageBox.Show("序号有重复，请检查");
                return;
            }
            for (int i = 0; i < mytempcanvas.Children.Count; i++)
            {
                EleTemplate ele = find_element(i + 1);
                if(ele == null)
                {
                    MessageBox.Show("序号不连续，请检查");
                    return;
                }
            }
            //把数据更新到table里面，要看下table里面的数据格式啊
            //数据点个数啊,遍历数据点啊
            info.table.Clear();
            is_closing = true;//窗口即将关闭，不能刷新了
            for (int i = 0; i < mytempcanvas.Children.Count; i++)
            {
                EleTemplate ele = find_element(i+1);                
                string[] arys = new string[3];
                arys[0] = (i + 1).ToString();
                arys[1] = ele.xmeter.ToString("F2");
                arys[2] = ele.ymeter.ToString("F2");                               
                info.table.Rows.Add(arys);
            }            

            //获取颜色啊
            SolidColorBrush solidColorBrush = RecColor.Fill as SolidColorBrush;
            System.Windows.Media.Color color = solidColorBrush.Color;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            ptmodel.color = System.Drawing.Color.FromArgb(r, g, b);

            if (Xcheck.IsChecked == true)
            {
                ptmodel.Isxchk = true;
                ptmodel.xmax = double.Parse(xmax.Text);
                ptmodel.xmin = double.Parse(xmin.Text);
            }
            else
            {
                ptmodel.Isxchk = false;
            }
            if (Ycheck.IsChecked == true)
            {
                ptmodel.Isychk = true;
                ptmodel.ymax = double.Parse(ymax.Text);
                ptmodel.ymin = double.Parse(ymin.Text);
            }
            else
            {
                ptmodel.Isychk = false;
            }
            if (Lcheck.IsChecked == true)
            {
                ptmodel.IsLchk = true;
                ptmodel.Lmax = double.Parse(Lmax.Text);
                ptmodel.Lmin = double.Parse(Lmin.Text);
            }
            else
            {
                ptmodel.IsLchk = false;
            }
            if (Balancechk.IsChecked == true)
            {                
                ptmodel.balancemin = double.Parse(balancemin.Text);
                ptmodel.IsBalancechk = true;
            }
            else
            {
                ptmodel.IsBalancechk = false;
            }
            if (isMeter.IsChecked == true)
            {
                ptmodel.IsMeter = true;
            }
            else
            {
                ptmodel.IsMeter=false;
            }
            IsEnsure = true;
            this.Close();
        }


        private EleTemplate find_element(int serno)
        {
            Canvas mytempcanvas = (Canvas)myinnerCanvas.Children[0];
            for (int i = 0; i < mytempcanvas.Children.Count; i++)
            {
                EleTemplate ele = (EleTemplate)mytempcanvas.Children[i];
                if(ele.SerNo == serno)
                {
                    return ele;
                }
            }
            return null;
        }

        private void GenerateNewTemplate()
        {
            Canvas mytempcanvas = new Canvas();
            double height = 300 * (ptmodel.productWidth / ptmodel.productLength);
            double length = 300;
            double percent = ptmodel.productLength / 300;//获取比例尺

            mytempcanvas.Width = length;
            mytempcanvas.Height = height;
            mytempcanvas.Background = System.Windows.Media.Brushes.LightGray;
            double xmargin = (ptmodel.productLength - (ptmodel.Ameter * 2)) / (ptmodel.column - 1) / percent;
            double ymargin = (ptmodel.productWidth - (ptmodel.Bmeter * 2)) / (ptmodel.row - 1) / percent;


            dt.Rows.Clear();//清空数据
            for (int i = 0; i < ptmodel.column; i++)
            {
                for (int j = 0; j < ptmodel.row; j++)
                {
                    DataRow dr = dt.NewRow();
                    EleTemplate ele = new EleTemplate();
                    ele.Width = 18;
                    ele.Height = 18;
                    ele.xmeter = i * xmargin * percent + ptmodel.Ameter;
                    ele.ymeter = j * ymargin * percent + ptmodel.Bmeter;
                    dr["X(mm)"] = ele.xmeter;
                    dr["Y(mm)"] = ele.ymeter;
                    dt.Rows.Add(dr);
                    ele.ShowData += new EleTemplate.ShowDataDelegate(ShowData);
                    Canvas.SetLeft(ele, i * xmargin + ptmodel.Ameter / percent - 10);
                    Canvas.SetTop(ele, j * ymargin + ptmodel.Bmeter / percent - 10);
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
            //GenerateNewTemplate();
        }


        private void Data2UI()
        {

            this.DataContext = ptmodel;
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            System.Windows.Point scaleCenter = e.GetPosition((Canvas)sender);
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
        System.Windows.Point startMovePosition;
        private void myinnerCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startMovePosition = e.GetPosition((Canvas)sender);
            isMoving = true;
        }

        private void myinnerCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMoving = false;
            System.Windows.Point endMovePosition = e.GetPosition((Canvas)sender);

            totalTranslate.X += (endMovePosition.X - startMovePosition.X) / scaleLevel;
            totalTranslate.Y += (endMovePosition.Y - startMovePosition.Y) / scaleLevel;
        }

        private void myinnerCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                System.Windows.Point currentMousePosition = e.GetPosition((Canvas)sender);//当前鼠标位置

                System.Windows.Point deltaPt = new System.Windows.Point(0, 0);
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

        public class PtModel: INotifyPropertyChanged
        {
            private double _productLength=100;
            private double _productWidth=100;
            private double _Ameter=10;
            private double _Bmeter=10;
            private double _Apercent=10;
            private double _Bpercent=10;
            private double _Cmeter=20;
            private double _Cpercent=20;
            private double _Dmeter=20;
            private double _Dpercent=20;
            private int _row = 10;
            private int _col = 10;
            public string tempName { get; set; }
            /// <summary>
            /// 屏幕长度
            /// </summary>
            public double productLength {
                get
                {
                    return _productLength;
                }
                set
                {
                    _productLength = value;
                    OnPropertyChanged(nameof(productLength));
                }
            }
            public double productWidth { 
                get
                {
                    return _productWidth;
                }
                set
                {
                    _productWidth = value;
                    OnPropertyChanged(nameof(productWidth));
                }
            }
            //毫米定位

            public int row { 
                get
                {
                    return _row;
                }
                set
                {
                    _row = value;
                    OnPropertyChanged(nameof(row));
                }
            }
            public int column { 
                get
                {
                    return _col;
                }
                set
                {
                    _col = value;
                    OnPropertyChanged(nameof(column));
                }
            }


            public bool IsMeter { get; set; }
            /// <summary>
            /// 定位参数
            /// </summary>
            public double Ameter {
                get
                {
                    return _Ameter;
                }
                set
                {
                    _Ameter = value;
                    OnPropertyChanged(nameof(Ameter));
                }
            }
            public double Bmeter { 
                get
                {
                    return _Bmeter;
                }
                set
                {
                    _Bmeter = value;
                    OnPropertyChanged(nameof(Bmeter));
                }
            }

            public double Apercent { 
                get
                {
                    return _Apercent;
                }
                set
                {
                    _Apercent=value;
                    OnPropertyChanged(nameof(Apercent));
                }
            }
            public double Bpercent { 
                get
                {
                    return _Bpercent;
                }
                set
                {
                    _Bpercent=value;
                    OnPropertyChanged(nameof(Bpercent));
                }
            }

            public double Cmeter { 
                get
                {
                    return _Cmeter;
                }
                set
                {
                    _Cmeter = value;
                    OnPropertyChanged(nameof(Cmeter));
                }
            }
            public double Cpercent { 
                get
                {
                    return _Cpercent;
                }
                set
                {
                    _Cpercent=value;
                    OnPropertyChanged(nameof(Cpercent));
                }
            }

            public double Dmeter { 
                get
                {
                    return _Dmeter;
                }
                set
                {
                    _Dmeter=value;
                    OnPropertyChanged(nameof(Dmeter));
                }
            }
            public double Dpercent { 
                get
                {
                    return _Dpercent;
                }
                set
                {
                    _Dpercent=value;
                    OnPropertyChanged(nameof(Dpercent));
                }
            }

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
            public bool IsBalancechk { get; set; }
            public double balancemin { get; set; }
            public System.Drawing.Color color { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void set_paracd_visiable(bool st)
        {
            if(st)
            {
                ParaC.Visibility = Visibility.Visible;
                ParaD.Visibility = Visibility.Visible;
            }
            else
            {
                ParaC.Visibility = Visibility.Collapsed;
                ParaD.Visibility = Visibility.Collapsed;
            }
        }

        private void set_row_col_visiable(bool st)
        {
            if (st)
            {
                ParaRow.Visibility =Visibility.Visible;
                ParaCol.Visibility = Visibility.Visible;
            }
            else
            {
                ParaRow.Visibility = Visibility.Collapsed;
                ParaCol.Visibility = Visibility.Collapsed;
            }
        }

        private void PointChecked(object sender, RoutedEventArgs e)
        {
            if(myinnerCanvas == null)
            {
                return;
            }
            RadioButton radioButton = (sender as RadioButton);
            string name = radioButton.Content.ToString();
            if (radioButton == point5)
            {
                set_paracd_visiable(false);
                set_row_col_visiable(false);
                image.Source = new BitmapImage(new Uri(@"../Image/5Points.jpg", UriKind.Relative));
                //update_draw_points();
                label_name.Content = "5点常规";
            }
            else if (radioButton == point9)
            {
                set_paracd_visiable(false);
                set_row_col_visiable(false);
                image.Source = new BitmapImage(new Uri(@"../Image/9Points.jpg", UriKind.Relative));
                //InitCanvas9();
                label_name.Content = "9点常规";
            }
            else if (radioButton == point13)
            {
                set_paracd_visiable(true);
                set_row_col_visiable(false);
                image.Source = new BitmapImage(new Uri(@"../Image/13Points.jpg", UriKind.Relative));
                //InitCanvas13();
                label_name.Content = "13点常规";
            }
            else if (radioButton == point13_diag)
            {
                // 13点对角模式使用固定 10% 网格，不依赖 A/B/C/D，隐藏 C/D
                set_paracd_visiable(false);
                set_row_col_visiable(false);
                image.Source = new BitmapImage(new Uri(@"../Image/13Points_new.jpg", UriKind.Relative));
                label_name.Content = "13点对角";
            }
            else if (radioButton == point15)
            {
                set_paracd_visiable(true);
                set_row_col_visiable(false);
                image.Source = new BitmapImage(new Uri(@"../Image/std15poi.jpeg", UriKind.Relative));
                //InitCanvas13();
                label_name.Content = "15点常规";
            }
            else if (radioButton == point17)
            {
                set_paracd_visiable(true);
                set_row_col_visiable(false);
                image.Source = new BitmapImage(new Uri(@"../Image/std17poi.jpeg", UriKind.Relative));
                //InitCanvas13();
                label_name.Content = "17点常规";
            }
            else if (radioButton == point17_ary)
            {
                set_paracd_visiable(false);
                set_row_col_visiable(false);
                image.Source = new BitmapImage(new Uri(@"../Image/array17p.jpeg", UriKind.Relative));
                //InitCanvas13();
                label_name.Content = "17点点阵";
            }
            else if (radioButton == point25)
            {
                set_paracd_visiable(false);
                set_row_col_visiable(false);
                image.Source = new BitmapImage(new Uri(@"../Image/25Points.jpg", UriKind.Relative));
                //InitCanvas13();
                label_name.Content = "25点常规";
            }
            else if (radioButton == point_crosstalk)
            {
                set_paracd_visiable(false);
                set_row_col_visiable(false);
                image.Source = new BitmapImage(new Uri(@"../Image/5Points.jpg", UriKind.Relative));
                //update_draw_points();
                label_name.Content = "CrossTalk";
            }
            else
            {
                set_paracd_visiable(false);
                set_row_col_visiable(true);
                image.Source = new BitmapImage(new Uri(@"../Image/25Points.jpg", UriKind.Relative));
                //InitCanvasOther();
                label_name.Content = "自定义点阵";
            }   
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Xml files (*.xml)|*.xml";
            saveFileDialog.FilterIndex = 1;

            if (saveFileDialog.ShowDialog() == true)
            {
                // 获取文件路径
                string filePath = saveFileDialog.FileName;
                InfoData infodata = new InfoData();
                infodata.height = 0;
                infodata.IsSelected = true;
                infodata.MESTYPE = info.MESTYPE;
                infodata.Name = info.Name;
                infodata.productWidth = ptmodel.productWidth;
                infodata.productLength = ptmodel.productLength;
                if (isMeter.IsChecked == true)
                    infodata.IsMeter = true;
                else
                    infodata.IsMeter = false;
                infodata.Ameter = ptmodel.Ameter;
                infodata.Apercent = ptmodel.Apercent;   
                infodata.Bmeter = ptmodel.Bmeter;
                infodata.Bpercent = ptmodel.Bpercent;
                infodata.lstdata = new List<string>();
                //数据点个数啊,遍历数据点啊
                Canvas mytempcanvas = (Canvas)myinnerCanvas.Children[0];
                for (int i = 0; i < mytempcanvas.Children.Count; i++)
                {
                    EleTemplate ele = (EleTemplate)mytempcanvas.Children[i];
                    string data = null;
                    if (isMeter.IsChecked == true)
                    {
                        data = (i + 1) + "," + ele.xmeter + "," + ele.ymeter + ",,,,,,";
                    }
                    else
                    {
                        data = (i + 1) + "," + ele.xpercent + "," + ele.ypercent + ",,,,,,";
                    }
                    infodata.lstdata.Add(data);
                }
                try
                {
                    string Xml = XmlUtil.Serializer(typeof(InfoData), infodata);
                    StreamWriter sw = File.CreateText(filePath);
                    sw.Write(Xml);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex);
                }
            }
            
        }


        private bool check_double_input(TextBox box,string name)
        {
            if (string.IsNullOrEmpty(box.Text))
            {
                string title = name + "，输入错误";
                if (Project.cfg.Lang != 0)
                {
                    title = "Input error:"+name;
                }
                MessageBox.Show(title);
                box.Focus();
                return false;
            }
            double id = 0;
            try
            {
                id = double.Parse(box.Text);
            }
            catch
            {
                string title = name + "，输入错误，应为正数";
                if (Project.cfg.Lang != 0)
                {
                    title = "Input error,not positive number:" + name;
                }
                MessageBox.Show(title);
                box.Focus();
                return false;
            }
            if(id< 0)
            {
                string title = name + "，输入错误，应为正数";
                if (Project.cfg.Lang != 0)
                {
                    title = "Input error,not positive number:" + name;
                }
                MessageBox.Show(title);
                box.Focus();
                return false;
            }
            return true;
        }
       

        private void Buttonsave_no_Click(object sender, RoutedEventArgs e)
        {            
            string txt = SerNo.Text;
            if(string.IsNullOrEmpty(txt))
            {
                MessageBox.Show("序号输入错误");
                SerNo.Focus();
                return;
            }
            int id = 0;
            try
            {
                id = int.Parse(txt);
            }
            catch
            {
                MessageBox.Show("序号输入错误");
                return;
            }
            double x = 0, y = 0, px = 0, py = 0;
            bool st = check_double_input(Xmeter,"X");
            if(st==false)
            {
                return;
            }
            st = check_double_input(Ymeter, "Y");
            if (st == false)
            {
                return;
            }
            st = check_double_input(Xpercent, "X百分比");
            if (st == false)
            {
                return;
            }
            st = check_double_input(Ypercent, "Y百分比");
            if (st == false)
            {
                return;
            }
            x = double.Parse(Xmeter.Text);
            y = double.Parse(Ymeter.Text);
            px = double.Parse(Xpercent.Text);
            py = double.Parse(Ypercent.Text);

            Canvas mytempcanvas = (Canvas)myinnerCanvas.Children[0];
            int count =  mytempcanvas.Children.Count;
            if(id <= 0 || id > count)
            {
                MessageBox.Show("序号输入错误");
                return;
            }

            //获取选中的图标啊
            for (int i = 0; i < mytempcanvas.Children.Count; i++)
            {
                EleTemplate ele = (EleTemplate)mytempcanvas.Children[i];
                if (ele.is_select())
                {
                    //更新序号和数据啊
                    //获取原来的SerNo
                    int no = ele.SerNo;
                    double xmeter = ele.xmeter;
                    double ymeter = ele.ymeter;
                    //ptmodel.SerNo = id;
                    ele.SetContent(id);
                    ele.xmeter = x;
                    ele.ymeter = y;
                    ele.xpercent = px;
                    ele.ypercent = py;

                    //需要保存到info.table.Rows表格和序号啊
                    DataRow row = find_data_row(no,xmeter,ymeter);
                    if (row != null)
                    {
                        row[0] = id;
                        row[1] = x.ToString();
                        row[2] = y.ToString();
                    }
                    break;
                }                
            }
            //更新表格显示值啊
            update_datagrid_data();
        }

        private DataRow find_data_row(int id,double x,double y)
        {
            int count = info.table.Rows.Count;
            int id1 = 0;
            double x1 = 0, y1 = 0;
            for(int i=0;i< count;i++)
            {
                id1 = int.Parse(info.table.Rows[i][0].ToString());
                x1 = double.Parse(info.table.Rows[i][1].ToString());
                y1 = double.Parse(info.table.Rows[i][2].ToString());
                if((id1 == id) && (x1 == x) &&(y1==y))
                {
                    return info.table.Rows[i];  
                }
            }
            return null;
        }

        private void txt_length_changed(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_width_changed(object sender, TextChangedEventArgs e)
        {

        }

        private void set_para_enable(bool st)
        {
            if(this.IsInitialized==false)
            {
                return;
            }
            try
            {
                ameter.IsEnabled = st;
                bmeter.IsEnabled = st;
                bmeter.IsEnabled = st;
                dmeter.IsEnabled = st;
                apercent.IsEnabled = !st;
                bpercent.IsEnabled = !st;
                cpercent.IsEnabled = !st;
                dpercent.IsEnabled = !st;
            }
            catch
            {

            }
        }

        private void RadioMode_Checked(object sender, RoutedEventArgs e)
        {
            if(isMeter.IsChecked ==true)
            {
                ptmodel.IsMeter = true;
                set_para_enable(true);
            }
            else
            {
                ptmodel.IsMeter =false;
                set_para_enable(false);
            }            
            //recalculate();
        }

        private void Color_select(object sender, MouseButtonEventArgs e)
        {
            // 创建一个ColorDialog实例
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            // 显示颜色选择面板
            var result = colorDialog.ShowDialog();
            // 检查用户是否选择了颜色
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // 获取所选颜色的RGB值
                System.Drawing.Color selectedColor = colorDialog.Color;
                update_warn_color(selectedColor);
                //保存颜色
                //ptmodel.color = selectedColor;
                System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);
                RecColor.Fill = new System.Windows.Media.SolidColorBrush(wpfColor);
            }
        }

        private void update_warn_color(System.Drawing.Color selectedColor)
        {            
            // 将所选颜色转换为WPF的颜色结构
            System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);
            // 将颜色应用到WPF控件上
            // 例如，可以将颜色应用到一个Rectangle控件的Fill属性
            RecColor.Fill = new System.Windows.Media.SolidColorBrush(wpfColor);
        }
    }

    public class EleIdNo
    {
        //id,0开始
        public int id { get; set; }
        //显示的No，从1开始啊
        public int No { get; set; }
    }
}
