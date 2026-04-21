using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LCD.Ctrl;
using LCD.Data;
using LCD.dataBase;
using Microsoft.DwayneNeed.Win32.Gdi32;
using SciChart.Core.Extensions;
using SharpDX;

namespace LCD.View
{
    /// <summary>
    /// 结果数据设置
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class ResutView : UserControl
    {

        //TestData testData=new TestData();
        //SpectrumData spectrumData=new SpectrumData();
        public Action<bool> ShowAction { get; set; } 
        
        //执行拷贝啊
        public Action<bool> CopyAction { get; set; }

        private ENUMMESSTYLE ENUMMESSTYLE { get; set; }
        DataTable ResultDatatemp = new DataTable();
        public Type ResutType { get; set; }
        //public List<IData> Datas { get; set; } = new List<IData>();
        //private ENUMMESSTYLE MESSTYLE = ENUMMESSTYLE._01_POINT;//设置为01Point点位


        //默认全部显示
        private bool ResBool = true;

        //lock object for synchronization;
        private object _syncLock = new object();

        private string test_item_str = "测试项";
        private string done_time_str = "完成时间";
        private string note_str = "备注";
        private string voltage_str = "电压";
        private string current_str = "电流";
        private string power_str = "功率";

        private int Num_index = 1;

        //计算统计数据
        public void calculateStatics(ENUMMESSTYLE eNUMMESSTYLE)
        {
            //看看有没有统计行啊，单点测量的时候没有统计，不需要算统计啊
            if(ResultDatatemp.Rows[0][test_item_str].ToString() != "Max")
            {
                return;
            }
            //var dataL = ResultDatatemp.AsEnumerable().Select(c => c.Field<string>("L")).ToList;

            string[] columL = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("L")).ToArray();

            //string[] columX = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("X")).ToArray();
            //string[] columY = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("Y")).ToArray();

            string[] columCx = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("Cx")).ToArray();
            string[] columCy = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("Cy")).ToArray();
            //string[] columU = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("u")).ToArray();
            //string[] columV = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("v")).ToArray();

            List<double> lstLs = new List<double>();
            //List<double> lstXs = new List<double>();
            //List<double> lstYs = new List<double>();

            List<double> lstcxs = new List<double>();
            List<double> lstcys = new List<double>();
            //List<double> lstus = new List<double>();
            //List<double> lstvs = new List<double>();
            //double columLMax = columL;
            //double columLMin = 0;
            for (int i = 4; i < ResultDatatemp.Rows.Count; i++)
            {
                lstLs.Add(double.Parse(columL[i]));
                //lstXs.Add(double.Parse(columX[i]));
                //lstYs.Add(double.Parse(columY[i]));

                lstcxs.Add(double.Parse(columCx[i]));
                lstcys.Add(double.Parse(columCy[i]));
                //lstus.Add(double.Parse(columU[i]));
                //lstvs.Add(double.Parse(columV[i]));
            }
            if(lstLs.Count <=0)
            {
                return;
            }
            int cnt = ResultDatatemp.Rows.Count;

            double meanL = lstLs.Average();
            //double meanX = lstXs.Average();

            //double meanY = lstYs.Average();

            double meanCx = lstcxs.Average();
            double meanCy = lstcys.Average();
            //double meanU = lstus.Average();
            //double meanV = lstvs.Average();

            double maxL = lstLs.Max();
            double minL = lstLs.Min();


            //double maxX = lstXs.Max();
            //double minX = lstXs.Min();


            //double maxY = lstYs.Max();
            //double minY = lstYs.Min();


            double maxCx = lstcxs.Max();
            double minCx = lstcxs.Min();

            double maxCy = lstcys.Max();
            double minCy = lstcys.Min();

            //double maxU = lstus.Max();
            //double minU = lstus.Min();

            //double maxV = lstvs.Max();
            //double minV = lstvs.Min();



            ResultDatatemp.Rows[0]["L"] = maxL.ToString("0.0");
            ResultDatatemp.Rows[0]["X"] = "";// maxL.ToString("0.0000");
            ResultDatatemp.Rows[0]["Y"] = "";//maxL.ToString("0.0000");
            ResultDatatemp.Rows[0]["Z"] = "";//maxL.ToString("0.0000");
            ResultDatatemp.Rows[0]["Cx"] = maxCx.ToString("0.0000");
            ResultDatatemp.Rows[0]["Cy"] = maxCy.ToString("0.0000");
            ResultDatatemp.Rows[0]["u"] = "";//maxU.ToString("0.0000");
            ResultDatatemp.Rows[0]["v"] = "";//maxV.ToString("0.0000");

            ResultDatatemp.Rows[1]["L"] = minL.ToString("0.0");
            ResultDatatemp.Rows[1]["X"] = "";//minX.ToString("0.0");
            ResultDatatemp.Rows[1]["Y"] = "";// minY.ToString("0.0");
            ResultDatatemp.Rows[1]["Z"] = "";// minY.ToString("0.0");
            ResultDatatemp.Rows[1]["Cx"] = minCx.ToString("0.0000");
            ResultDatatemp.Rows[1]["Cy"] = minCy.ToString("0.0000");
            ResultDatatemp.Rows[1]["u"] = "";//minU.ToString("0.0000");
            ResultDatatemp.Rows[1]["v"] = "";// minV.ToString("0.0000");



            ResultDatatemp.Rows[2]["L"] = meanL.ToString("0.0");
            ResultDatatemp.Rows[2]["X"] = "";// meanX.ToString("0.0");
            ResultDatatemp.Rows[2]["Y"] = "";// meanY.ToString("0.0");
            ResultDatatemp.Rows[2]["Z"] = "";
            ResultDatatemp.Rows[2]["Cx"] = meanCx.ToString("0.0000");
            ResultDatatemp.Rows[2]["Cy"] = meanCy.ToString("0.0000");
            ResultDatatemp.Rows[2]["u"] = "";//meanU.ToString("0.0000");
            ResultDatatemp.Rows[2]["v"] = "";// meanV.ToString("0.0000");

            ResultDatatemp.Rows[3]["L"] = (100.0*minL/maxL).ToString("0.00")+"%";
            ResultDatatemp.Rows[3]["X"] = "";// meanX.ToString("0.0");
            ResultDatatemp.Rows[3]["Y"] = "";// meanY.ToString("0.0");
            ResultDatatemp.Rows[3]["Z"] = "";
            ResultDatatemp.Rows[3]["Cx"] = (100.0 * minCx / maxCx).ToString("0.00") + "%";
            ResultDatatemp.Rows[3]["Cy"] = (100.0 * minCy / maxCy).ToString("0.00") + "%";
            ResultDatatemp.Rows[3]["u"] = "";//meanU.ToString("0.0000");
            ResultDatatemp.Rows[3]["v"] = "";// meanV.ToString("0.0000");



        }


        public void initResultDataTemp(ENUMMESSTYLE eNUMMESSTYLE)
        {

            if(eNUMMESSTYLE==ENUMMESSTYLE._01_POINT||ENUMMESSTYLE==ENUMMESSTYLE._03_SPECTRUM)
            {
            List<String> itemName=new List<string>();
            itemName.Add("Max");
            itemName.Add("Min");
            itemName.Add("Avg");
            itemName.Add("Uniform");

            for (int i = 0; i < 4; i++)
            { 
            
            DataRow dataRow = ResultDatatemp.NewRow();
            dataRow[test_item_str] = itemName[i];
            //dataRow["Num"] = ResultDatatemp.Rows.Count + 1;
            dataRow["L"] = "0.0";
            dataRow["X"] = "0.0000";
            dataRow["Y"] ="0.0000";
            dataRow["Z"] = "0.0000";
            dataRow["Cx"] = "0.0000";
            dataRow["Cy"] = "0.0000";
            dataRow["u"] = "0.0000";
            dataRow["v"] = "0.0000";
            ResultDatatemp.Rows.Add(dataRow);
            }
            }
        }



        //设置数据表
        private DataTable dt = new DataTable();




        public ResutView(ENUMMESSTYLE eNUMMESSTYLE)
        {
            
            this.ENUMMESSTYLE = eNUMMESSTYLE;
            DataTable dataTable=new DataTable();

            InitializeComponent();

            if (Project.cfg.Lang != 0)
            {
                test_item_str = "Test item";
                done_time_str = "Time";
                note_str = "Note";
                voltage_str = "Voltage";
                current_str = "Curretn";
                power_str = "Power";
            }

            if (eNUMMESSTYLE == ENUMMESSTYLE._01_POINT)
            {
                ResultDatatemp.Columns.Add(test_item_str);
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("L");
                ResultDatatemp.Columns.Add("Cx");
                ResultDatatemp.Columns.Add("Cy");
                ResultDatatemp.Columns.Add("u");
                ResultDatatemp.Columns.Add("v");
                ResultDatatemp.Columns.Add("X");
                ResultDatatemp.Columns.Add("Y");
                ResultDatatemp.Columns.Add("Z");

                //增加lab
                ResultDatatemp.Columns.Add("L*");
                ResultDatatemp.Columns.Add("a*");
                ResultDatatemp.Columns.Add("b*");

                ResultDatatemp.Columns.Add("CCT");
                ResultDatatemp.Columns.Add(done_time_str);
                ResultDatatemp.Columns.Add(note_str);

                //for (int i = 0; i < 4; i++)
                //{ 

                //initResultDataTemp(eNUMMESSTYLE);
                //}

              //  initResultDataTemp(eNUMMESSTYLE);

            }
            else if (eNUMMESSTYLE == ENUMMESSTYLE._02_RESPONSE)
            {
                ResultDatatemp.Columns.Add(test_item_str);
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("X");
                ResultDatatemp.Columns.Add("Y");
                ResultDatatemp.Columns.Add("Z");
                ResultDatatemp.Columns.Add("Low");
                ResultDatatemp.Columns.Add("High");
                ResultDatatemp.Columns.Add("RiseTime");
                ResultDatatemp.Columns.Add("FallTime");
                ResultDatatemp.Columns.Add(done_time_str);
            }
            else if (eNUMMESSTYLE==ENUMMESSTYLE._03_SPECTRUM)
            {
                ResultDatatemp.Columns.Add(test_item_str);
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("L");
                ResultDatatemp.Columns.Add("Cx");
                ResultDatatemp.Columns.Add("Cy");
                ResultDatatemp.Columns.Add("u");
                ResultDatatemp.Columns.Add("v");
                ResultDatatemp.Columns.Add("X");
                ResultDatatemp.Columns.Add("Y");
                ResultDatatemp.Columns.Add("Z");

                ResultDatatemp.Columns.Add("L*");
                ResultDatatemp.Columns.Add("a*");
                ResultDatatemp.Columns.Add("b*");

                ResultDatatemp.Columns.Add("CCT");
                ResultDatatemp.Columns.Add(done_time_str);
                ResultDatatemp.Columns.Add(note_str);
                for (int i = 0; i < 401; i++)
                {
                    ResultDatatemp.Columns.Add($"{i+380}");
                }                
            }
            else if(eNUMMESSTYLE == ENUMMESSTYLE._04_FLICKER)
            {

            }
            else if (eNUMMESSTYLE == ENUMMESSTYLE._05_CROSSTALK)
            {
                ResultDatatemp.Columns.Add(test_item_str);
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("La");
                ResultDatatemp.Columns.Add("Lb");
                ResultDatatemp.Columns.Add("CT");
                ResultDatatemp.Columns.Add(done_time_str);
            }
            else if ((eNUMMESSTYLE == ENUMMESSTYLE._07_warmup))
            {
                ResultDatatemp.Columns.Add(test_item_str);
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("L");
                ResultDatatemp.Columns.Add("Cx");
                ResultDatatemp.Columns.Add("Cy");
                ResultDatatemp.Columns.Add("u");
                ResultDatatemp.Columns.Add("v");
                ResultDatatemp.Columns.Add("X");
                ResultDatatemp.Columns.Add("Y");
                ResultDatatemp.Columns.Add("Z");
                
                ResultDatatemp.Columns.Add("CCT");
                ResultDatatemp.Columns.Add(done_time_str);
                ResultDatatemp.Columns.Add(note_str);
            }
            else if ((eNUMMESSTYLE==ENUMMESSTYLE.Power))
            {
                ResultDatatemp.Columns.Add(test_item_str);
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add(voltage_str);
                //ResultDatatemp.Columns.Add("");
                //ResultDatatemp.Columns.Add("");
                //ResultDatatemp.Columns.Add("");
                ResultDatatemp.Columns.Add(current_str);
                ResultDatatemp.Columns.Add(power_str);
            }

            DataView dv = new DataView(ResultDatatemp);
            ResultData.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            ResultData.VerticalGridLinesBrush = Brushes.Gray;
            ResultData.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly =true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            //ResultData.MinColumnWidth = 70;
            ResultData.HorizontalScrollBarVisibility= ScrollBarVisibility.Visible;
            ResultData.ItemsSource = dv;            

            ShowAction = (a) => { new Action<bool>(ShowAll).Invoke(a); };
            CopyAction = (a) => { new Action<bool>(do_Copy).Invoke(a); };
        }

        public void do_Copy(bool st)
        {
            if(ResultDatatemp.Rows.Count == 0)
            {
                MessageBox.Show("当前无数据，不能拷贝");
                return;
            }
            //接下来是执行拷贝啊
            // 将表格数据格式化为字符串
            string tableString = FormatTableData();
            // 将格式化后的字符串复制到剪贴板
            Clipboard.SetText(tableString);
            MessageBox.Show("已经拷贝");
        }

        private string FormatTableData()
        {
            // 简单地以制表符分隔每个单元格，并以换行符分隔每一行
            string tableString = ""; 
            for(int i=0;i< ResultDatatemp.Rows.Count; i++)
            {
                tableString += string.Join("\t", ResultDatatemp.Rows[i].ItemArray)+"\n";               
            }
            return tableString;
        }

        public void ShowAll(bool ShowAll)
        {
            ResBool = ShowAll;

            if (ShowAll)
            {
                if (this.ResultData.Columns.Count > 12)
                {
                    this.ResultData.Columns[0].Visibility = Visibility.Visible;
                    this.ResultData.Columns[1].Visibility = Visibility.Visible;
                    this.ResultData.Columns[5].Visibility = Visibility.Visible;
                    this.ResultData.Columns[6].Visibility = Visibility.Visible;
                    this.ResultData.Columns[7].Visibility = Visibility.Visible;
                    this.ResultData.Columns[8].Visibility = Visibility.Visible;
                    this.ResultData.Columns[9].Visibility = Visibility.Visible;
                    this.ResultData.Columns[10].Visibility = Visibility.Visible;
                    this.ResultData.Columns[11].Visibility = Visibility.Visible;
                    this.ResultData.Columns[12].Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (this.ResultData.Columns.Count > 12)
                {
                    this.ResultData.Columns[0].Visibility = Visibility.Collapsed;
                    this.ResultData.Columns[1].Visibility = Visibility.Collapsed;
                    this.ResultData.Columns[5].Visibility = Visibility.Collapsed;
                    this.ResultData.Columns[6].Visibility = Visibility.Collapsed;
                    this.ResultData.Columns[7].Visibility = Visibility.Collapsed;
                    this.ResultData.Columns[8].Visibility = Visibility.Collapsed;
                    this.ResultData.Columns[9].Visibility = Visibility.Collapsed;
                    this.ResultData.Columns[10].Visibility = Visibility.Collapsed;
                    this.ResultData.Columns[11].Visibility = Visibility.Collapsed;
                    this.ResultData.Columns[12].Visibility = Visibility.Collapsed;
                }
            }
        }

        public DataTable GetTale()
        {
            return dt;
        }


        private void set_line_color(int row_index, InfoData infoData)
        {
            //跨线程调用啊
            this.Dispatcher.Invoke(new Action(() =>
            {
                ResultData.UpdateLayout();
                DataGridRow row = (DataGridRow)ResultData.ItemContainerGenerator.ContainerFromIndex(row_index);
                if (row == null)
                {
                    return;
                }
                System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromRgb((byte)infoData.warnR, (byte)infoData.warnG, (byte)infoData.warnB);
                row.Background = new SolidColorBrush(wpfColor);
            }));   
        }

        private void set_cell_color(int row_index,int colume, InfoData infoData)
        {
            //dataGrid.Rows[rowIndex].Cells[columnIndex].Style.BackColor = Color.Red;
            //跨线程调用啊
            this.Dispatcher.Invoke(new Action(() =>
            {
                ResultData.UpdateLayout();
                DataGridRow row = (DataGridRow)ResultData.ItemContainerGenerator.ContainerFromIndex(row_index);
                if (row == null)
                {
                    return;
                }
                // 获取第一列（如果DataGrid有列的话）
                DataGridCell cell = ResultData.Columns[colume].GetCellContent(row).Parent as DataGridCell;
                if (cell != null)
                {
                    // 设置背景颜色
                    //cell.Background = Brushes.Red;
                    System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromRgb((byte)infoData.warnR, (byte)infoData.warnG, (byte)infoData.warnB);
                    cell.Background = new SolidColorBrush(wpfColor);
                }               
            }));
        }

        //根据标准来设置颜色
        public void Update_Color(InfoData infoData)
        {
            for (int i = 4; i < ResultDatatemp.Rows.Count; i++)
            {
                if (infoData.Isxchk)
                {
                    double val = double.Parse(ResultDatatemp.Rows[i]["Cx"].ToString());
                    if (val > infoData.xmax || val < infoData.xmin)
                    {
                        //设置颜色啊
                        set_cell_color(i,3,infoData);
                    }
                }
                if (infoData.Isychk)
                {
                    double val = double.Parse(ResultDatatemp.Rows[i]["Cy"].ToString());
                    if (val > infoData.ymax || val < infoData.ymin)
                    {
                        //设置颜色啊
                        set_cell_color(i,4, infoData);
                    }
                }
                if (infoData.IsLchk)
                {
                    double val = double.Parse(ResultDatatemp.Rows[i]["L"].ToString());
                    if(val > infoData.Lmax || val < infoData.Lmin)
                    {
                        //设置颜色啊
                        set_cell_color(i,2, infoData);
                    }
                }                
            }
            //均衡性设定：用大于设定值来判定。  例如设置 uniformity >95%为OK，当前模版测试完毕后，用L最小值/最大值×100%， 如计算结果小于95%  ，均匀性栏位显示红色。  
            if (infoData.IsBalancechk)
            {
                double max = double.Parse(ResultDatatemp.Rows[0]["L"].ToString());
                double min = double.Parse(ResultDatatemp.Rows[1]["L"].ToString());
                if (100 * min / max < infoData.balancemin)
                {
                    //设置颜色,第三行是uniform
                    //set_line_color(3, infoData);
                    set_cell_color(3,2,infoData);
                }
            }
            
        }

        public void save_point_statics()
        {
            if (ResultDatatemp.Rows.Count > 4)
            {
                //把Max，Min,avg,uniform保存起来
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = "Max",
                    Remark = "Max",
                    L = ResultDatatemp.Rows[0]["L"].ToString(),
                    Cx = ResultDatatemp.Rows[0]["Cx"].ToString(),
                    Cy = ResultDatatemp.Rows[0]["Cy"].ToString()
                });
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = "Min",
                    Remark = "Min",
                    L = ResultDatatemp.Rows[1]["L"].ToString(),
                    Cx = ResultDatatemp.Rows[1]["Cx"].ToString(),
                    Cy = ResultDatatemp.Rows[1]["Cy"].ToString()
                });
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = "Avg",
                    Remark = "Avg",
                    L = ResultDatatemp.Rows[2]["L"].ToString(),
                    Cx = ResultDatatemp.Rows[2]["Cx"].ToString(),
                    Cy = ResultDatatemp.Rows[2]["Cy"].ToString()
                });
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = "Uniform",
                    Remark = "Uniform",
                    L = ResultDatatemp.Rows[3]["L"].ToString(),
                    Cx = ResultDatatemp.Rows[3]["Cx"].ToString(),
                    Cy = ResultDatatemp.Rows[3]["Cy"].ToString()
                });
            }
        }


        public void AddSingleData(IData objs,string testname)
        {
            if (objs == null) { Project.WriteLog("采集数据失败！"); return; }
            DataRow dataRow = null;
            if (ENUMMESSTYLE == ENUMMESSTYLE._01_POINT)
            {
                if(objs.Remark==null)
                {
                    objs.Remark = "";
                }
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = Num_index.ToString(),
                    L = objs.L.ToString(),
                    X = objs.X.ToString(),
                    Y = objs.Y.ToString(),
                    Z = objs.Z.ToString(),
                    Cx = objs.Cx.ToString(),
                    Cy = objs.Cy.ToString(),
                    u = objs.u.ToString(),
                    v = objs.v.ToString(),
                    CCT = objs.CCT.ToString(),
                    Time = objs.Time.ToString(),
                    Remark = objs.Remark.Trim(),
                    CoordX = objs.CoordX.ToString(),
                    CoordY = objs.CoordY.ToString(),
                    CoordZ = objs.CoordZ.ToString(),
                    CoordU = objs.CoordU.ToString(),
                    CoordV = objs.CoordV.ToString(),
                    Lcolor = objs.Lcolor.ToString(),
                    Acolor = objs.Acolor.ToString(),
                    Bcolor = objs.Bcolor.ToString()
                }) ;
                dataRow = ResultDatatemp.NewRow();
                dataRow[test_item_str] = testname;
                dataRow["Num"] = Num_index;
                dataRow["L"] = objs.L;
                dataRow["X"] = objs.X;
                dataRow["Y"] = objs.L;
                dataRow["Z"] = objs.Z;
                dataRow["Cx"] = objs.Cx;
                dataRow["Cy"] = objs.Cy;
                dataRow["u"] = objs.u;
                dataRow["v"] = objs.v;
                dataRow["CCT"] = objs.CCT;
                dataRow["L*"] = objs.Lcolor.ToString("f4");
                dataRow["a*"] = objs.Acolor.ToString("f4");
                dataRow["b*"] = objs.Bcolor.ToString("f4");
                dataRow[done_time_str] = objs.Time;
                dataRow[note_str] = objs.Remark;             
            }
            else if (ENUMMESSTYLE == ENUMMESSTYLE._02_RESPONSE)
            {
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = (Num_index).ToString(),
                    X = objs.X.ToString(),
                    Y = objs.Y.ToString(),
                    Z = objs.Z.ToString(),
                    High = objs.High.ToString(),
                    Low = objs.Low.ToString(),
                    RiseTime = objs.RiseTime.ToString(),
                    FallTime = objs.FallTime.ToString(),
                    Time = objs.Time.ToString(),
                    CoordX = objs.CoordX.ToString(),
                    CoordY = objs.CoordY.ToString(),
                    CoordZ = objs.CoordZ.ToString(),
                    CoordU = objs.CoordU.ToString(),
                    CoordV = objs.CoordV.ToString()
                });
                dataRow = ResultDatatemp.NewRow();
                dataRow[test_item_str] = testname;
                dataRow["Num"] = Num_index;
                dataRow["X"] = objs.X;
                dataRow["Y"] = objs.Y;
                dataRow["Z"] = objs.Z;
                dataRow["Low"] = objs.Low;//借用表示上升沿时间
                dataRow["High"] = objs.High;//借用表示下降沿时间
                dataRow["RiseTime"] = objs.RiseTime;//借用表示下降沿时间
                dataRow["FallTime"] = objs.FallTime;//借用表示下降沿时间
                dataRow[done_time_str] = objs.Time;
            }
            else if (ENUMMESSTYLE == ENUMMESSTYLE._03_SPECTRUM)
            {
                if (objs.Remark == null)
                {
                    objs.Remark = "";
                }
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = Num_index.ToString(),
                    L = objs.L.ToString(),
                    X = objs.X.ToString(),
                    Y=objs.Y.ToString(),
                    Z = objs.Z.ToString(),
                    Cx = objs.Cx.ToString(),
                    Cy = objs.Cy.ToString(),
                    u = objs.u.ToString(),
                    v = objs.v.ToString(),
                    CCT = objs.CCT.ToString(),
                    Time = objs.Time.ToString(),
                    Remark = objs.Remark.Trim(),
                    CoordX = objs.CoordX.ToString(),
                    CoordY = objs.CoordY.ToString(),
                    CoordZ = objs.CoordZ.ToString(),
                    CoordU = objs.CoordU.ToString(),
                    CoordV = objs.CoordV.ToString(),
                    Lcolor = objs.Lcolor.ToString(),
                    Acolor = objs.Acolor.ToString(),
                    Bcolor = objs.Bcolor.ToString()
                });
                dataRow = ResultDatatemp.NewRow();
                dataRow[test_item_str] = testname;
                dataRow["Num"] = Num_index;
                dataRow["L"] = objs.L;
                dataRow["X"] = objs.X;
                dataRow["Y"] = objs.L;
                dataRow["Z"] = objs.Z;
                dataRow["Cx"] = objs.Cx;
                dataRow["Cy"] = objs.Cy;
                dataRow["u"] = objs.u;
                dataRow["v"] = objs.v;
                dataRow["CCT"] = objs.CCT;
                dataRow["L*"] = objs.Lcolor.ToString("f4");
                dataRow["a*"] = objs.Acolor.ToString("f4");
                dataRow["b*"] = objs.Bcolor.ToString("f4");
                dataRow[done_time_str] = objs.Time;
                dataRow[note_str] = objs.Remark;
                for (int i = 0; i < 401; i++)
                {
                    SpectrumData.Insert(new SpectrumDataMode()
                    {
                        Project_id = Project.ProjectID,
                        DataName = $"{i + 380}",
                        dataValue = objs.SpectrumData[i].ToString("E")
                     });
                    dataRow[$"{i + 380}"] = objs.SpectrumData[i].ToString("E");
                }

            }
            else if (ENUMMESSTYLE == ENUMMESSTYLE._04_FLICKER)
            {

            }
            else if (ENUMMESSTYLE == ENUMMESSTYLE._05_CROSSTALK)
            {
                if (objs.Remark == null)
                {
                    objs.Remark = "";
                }

                //只有4个点
                if(Num_index > objs.Point_Count)
                {
                    Num_index = 1;
                }

                dataRow = ResultDatatemp.NewRow();
                dataRow[test_item_str] = testname;
                dataRow["Num"] = Num_index;
                dataRow["La"] = objs.L;
                dataRow["Lb"] = objs.Lb;
                dataRow["CT"] = objs.CT;
                //dataRow[done_time_str] = objs.Time;                
            }
            else if ((ENUMMESSTYLE == ENUMMESSTYLE._07_warmup))
            {
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = (Num_index).ToString(),
                    L = objs.L.ToString(),
                    X = objs.X.ToString(),
                    Y = objs.Y.ToString(),
                    Z = objs.Z.ToString(),
                    Cx = objs.Cx.ToString(),
                    Cy = objs.Cy.ToString(),
                    u = objs.u.ToString(),
                    v = objs.v.ToString(),
                    CCT = objs.CCT.ToString(),
                    Time = objs.Time.ToString(),
                    Remark = objs.Remark.Trim(),
                    CoordX = objs.CoordX.ToString(),
                    CoordY = objs.CoordY.ToString(),
                    CoordZ = objs.CoordZ.ToString(),
                    CoordU = objs.CoordU.ToString(),
                    CoordV = objs.CoordV.ToString()
                });
                //Console.WriteLine($"-->{testname}");
                dataRow = ResultDatatemp.NewRow();
                dataRow[test_item_str] = testname;
                dataRow["Num"] = Num_index;
                dataRow["L"] = objs.L;
                dataRow["X"] = objs.X;
                dataRow["Y"] = objs.L;
                dataRow["Z"] = objs.Z;
                dataRow["Cx"] = objs.Cx;
                dataRow["Cy"] = objs.Cy;
                dataRow["u"] = objs.u;
                dataRow["v"] = objs.v;
                dataRow["CCT"] = objs.CCT;
                dataRow[done_time_str] = objs.Time;
                dataRow[note_str] = objs.Remark;
            }


            if (objs.CT_done==false)
            {
                ResultDatatemp.Rows.InsertAt(dataRow, ResultDatatemp.Rows.Count);
            }
            else
            {
                //是更新表格，不是添加表格
                //根据Num来查找吧，2次测试的Num是相同的
                //取出前一次的测量结果，计算CT
                int index = Num_index-1;
                string la_str = ResultDatatemp.Rows[index].ItemArray[2].ToString();
                double la = double.Parse(la_str);
                double lb = objs.L;
                double ct = 100*Math.Abs(la-lb)/la;
                //更新值
                ResultDatatemp.Rows[index][3] = lb;
                ResultDatatemp.Rows[index][4] = ct;
                ResultDatatemp.Rows[index][5] = objs.Time;
                //到这里可以存数据库了啊
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = Num_index.ToString(),
                    La = la.ToString(),
                    Lb = lb.ToString(),
                    CT = ct.ToString("f2"),
                    Time = objs.Time.ToString(),
                    Remark = objs.Remark.Trim(),
                    CoordX = objs.CoordX.ToString(),
                    CoordY = objs.CoordY.ToString(),
                    CoordZ = objs.CoordZ.ToString(),
                    CoordU = objs.CoordU.ToString(),
                    CoordV = objs.CoordV.ToString(),
                    Lcolor = objs.Lcolor.ToString(),
                    Acolor = objs.Acolor.ToString(),
                    Bcolor = objs.Bcolor.ToString()
                });                
            }

            if ((ENUMMESSTYLE == ENUMMESSTYLE._01_POINT) || (ENUMMESSTYLE == ENUMMESSTYLE._03_SPECTRUM))
            {
                //点和光谱测量的时候计算L，cx和Cy最大值最小值和平均值
                calculateStatics(ENUMMESSTYLE);
            }
            //calculateStatics(ENUMMESSTYLE);

            DataView dv = new DataView(ResultDatatemp);


            Project.WriteLog("准备添加表格");
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                //for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        lock (_syncLock)
                        {
                            this.ResultData.ItemsSource = dv;
                        }
                    }
                    catch(Exception ex)
                    {
                        LogHelper.Instance.Write("刷新表格异常："+ex.Message);
                        //continue;
                    }
                    //没有异常就直接跳出
                    //break;
                }
                ShowAll(ResBool);

                if ((ENUMMESSTYLE == ENUMMESSTYLE._01_POINT) || (ENUMMESSTYLE == ENUMMESSTYLE._03_SPECTRUM))
                {
                    if (Project.cfg.ShowLab == false)
                    {
                        if (ResultData.Columns.Count >= 13)
                        {
                            //隐藏lab
                            ResultData.Columns[10].Visibility = Visibility.Collapsed;
                            ResultData.Columns[11].Visibility = Visibility.Collapsed;
                            ResultData.Columns[12].Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        if (ResultData.Columns.Count >= 13)
                        {
                            ResultData.Columns[10].Visibility = Visibility.Visible;
                            ResultData.Columns[11].Visibility = Visibility.Visible;
                            ResultData.Columns[12].Visibility = Visibility.Visible;
                        }
                    }
                }

                Project.WriteLog("添加表格完成");
            }));
			
			Num_index++;

            //dataRow.Delete();
            //ResultDatatemp.Rows.Add() = testname;


            //if (ResutType != objs.GetType())
            //{
            //    Init(objs);
            //}
            //objs.TestName = testname;
            //objs.ID = Datas.Count;
            //Datas.Add(objs);
            //Datas = new List<IData>(Datas);
        }

        public void SingleData(Ctrl.Result objs, string testname)
        {

            if (objs == null) { Project.WriteLog("采集数据失败！"); return; }
            DataRow dataRow = null;
            TestData.Insert(new TestDataMode()
            {
                Project_id = Project.ProjectID,
                Num = ResultDatatemp.Rows.Count.ToString(),
                Voltage=objs.Voltage.ToString(),
                ElectricCurrent=objs.ElectricCurrent.ToString(),
                Power=objs.Power.ToString(),
            });
            //Console.WriteLine($"-->{testname}");
            dataRow = ResultDatatemp.NewRow();
            dataRow[test_item_str] = testname;
            dataRow["Num"] = ResultDatatemp.Rows.Count;
            dataRow[voltage_str] = objs.Voltage.ToString();
            dataRow[current_str] = objs.ElectricCurrent.ToString();
            dataRow[power_str] = objs.Power.ToString();          



            ResultDatatemp.Rows.InsertAt(dataRow, ResultDatatemp.Rows.Count + 1);

            DataView dv = new DataView(ResultDatatemp);



            this.Dispatcher.Invoke(new Action(() =>
            {
                lock (_syncLock)
                {
                    this.ResultData.ItemsSource = dv;
                }
            }));

            //dataRow.Delete();
            //ResultDatatemp.Rows.Add() = testname;


            //if (ResutType != objs.GetType())
            //{
            //    Init(objs);
            //}
            //objs.TestName = testname;
            //objs.ID = Datas.Count;
            //Datas.Add(objs);
            //Datas = new List<IData>(Datas);
        }
        /// <summary>
        /// 清除
        /// </summary>
        public void SetDatas(List<IData> datas)
        {
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            ResultDatatemp.Clear();

            //编号也需要重置一下
            Num_index = 1;
            if (ResultDatatemp.Columns.Contains("L")&&
                ResultDatatemp.Columns.Contains("X")&&
                ResultDatatemp.Columns.Contains("Y")&&
                ResultDatatemp.Columns.Contains("Cx"))
            {
                ENUMMESSTYLE eNUMMESSTYLE = ENUMMESSTYLE._01_POINT;
                initResultDataTemp(eNUMMESSTYLE);
            }


            //ENUMMESSTYLE eNUMMESSTYLE=ENUMMESSTYLE._01_POINT;
            //initResultDataTemp(eNUMMESSTYLE);
            DataView dv = new DataView(ResultDatatemp);
            ResultData.ItemsSource = dv;
            //Datas?.Clear();
            //Datas = new List<IData>();
        }

        #region 废弃的
        //private void InitDataGrid_SPECTRUM()
        //{
        //    if (dt != null) { dt.Dispose(); }
        //    dt = new DataTable();
        //    dt.Columns.Add("ID");
        //    dt.Columns.Add("X(mm)");
        //    dt.Columns.Add("Y(mm)");
        //    dt.Columns.Add("Z(mm)");
        //    dt.Columns.Add("U(°)");
        //    dt.Columns.Add("V(°)");
        //    dt.Columns.Add("L");
        //    dt.Columns.Add("cx");
        //    dt.Columns.Add("cy");
        //    DataView dv = new DataView(dt);


        //    ///定义数据表
        //    mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
        //    mydata.VerticalGridLinesBrush = Brushes.Gray;
        //    mydata.MinColumnWidth = 70;
        //    mydata.ItemsSource = dv;
        //    mydata.CanUserReorderColumns = false;
        //    mydata.CanUserAddRows = false;
        //    mydata.CanUserDeleteRows = false;
        //    mydata.CanUserSortColumns = false;


        //    for (int i = 0; i < 30; i++)
        //    {
        //        DataRow dr = dt.NewRow();
        //        dr["ID"] = i;
        //        dr["X(mm)"] = 100.00;
        //        dr["Y(mm)"] = 120.00;
        //        dr["Z(mm)"] = 120.00;
        //        dr["U(°)"] = 120.00;
        //        dr["V(°)"] = 120.00;
        //        dr["L"] = 2013.000;
        //        dr["cx"] = 0.0002;
        //        dr["cy"] = 0.0009;
        //        dt.Rows.Add(dr);
        //    }

        //    //测试结束后显示该数据
        //    AddMin_Max_Per();
        //}

        //private void InitDataGrid_ACR()
        //{
        //    if (dt != null) { dt.Dispose(); }
        //    dt = new DataTable();
        //    dt.Columns.Add("ID");
        //    dt.Columns.Add("X(mm)");
        //    dt.Columns.Add("Y(mm)");
        //    dt.Columns.Add("Z(mm)");
        //    dt.Columns.Add("U(°)");
        //    dt.Columns.Add("V(°)");
        //    dt.Columns.Add("L");
        //    dt.Columns.Add("cx");
        //    dt.Columns.Add("cy");
        //    DataView dv = new DataView(dt);


        //    ///定义数据表
        //    mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
        //    mydata.VerticalGridLinesBrush = Brushes.Gray;
        //    mydata.MinColumnWidth = 70;
        //    mydata.ItemsSource = dv;
        //    mydata.CanUserReorderColumns = false;
        //    mydata.CanUserAddRows = false;
        //    mydata.CanUserDeleteRows = false;
        //    mydata.CanUserSortColumns = false;


        //    for (int i = 0; i < 30; i++)
        //    {
        //        DataRow dr = dt.NewRow();
        //        dr["ID"] = i;
        //        dr["X(mm)"] = 100.00;
        //        dr["Y(mm)"] = 120.00;
        //        dr["Z(mm)"] = 120.00;
        //        dr["U(°)"] = 120.00;
        //        dr["V(°)"] = 120.00;
        //        dr["L"] = 2013.000;
        //        dr["cx"] = 0.0002;
        //        dr["cy"] = 0.0009;
        //        dt.Rows.Add(dr);
        //    }

        //    //测试结束后显示该数据
        //    AddMin_Max_Per();
        //}

        //private void InitDataGrid_FLICKER()
        //{
        //    if (dt != null) { dt.Dispose(); }
        //    dt = new DataTable();
        //    dt.Columns.Add("ID");
        //    dt.Columns.Add("X(mm)");
        //    dt.Columns.Add("Y(mm)");
        //    dt.Columns.Add("Z(mm)");
        //    dt.Columns.Add("U(°)");
        //    dt.Columns.Add("V(°)");
        //    dt.Columns.Add("L");
        //    dt.Columns.Add("cx");
        //    dt.Columns.Add("cy");
        //    DataView dv = new DataView(dt);


        //    ///定义数据表
        //    mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
        //    mydata.VerticalGridLinesBrush = Brushes.Gray;
        //    mydata.MinColumnWidth = 70;
        //    mydata.ItemsSource = dv;
        //    mydata.CanUserReorderColumns = false;
        //    mydata.CanUserAddRows = false;
        //    mydata.CanUserDeleteRows = false;
        //    mydata.CanUserSortColumns = false;


        //    for (int i = 0; i < 30; i++)
        //    {
        //        DataRow dr = dt.NewRow();
        //        dr["ID"] = i;
        //        dr["X(mm)"] = 100.00;
        //        dr["Y(mm)"] = 120.00;
        //        dr["Z(mm)"] = 120.00;
        //        dr["U(°)"] = 120.00;
        //        dr["V(°)"] = 120.00;
        //        dr["L"] = 2013.000;
        //        dr["cx"] = 0.0002;
        //        dr["cy"] = 0.0009;
        //        dt.Rows.Add(dr);
        //    }

        //    //测试结束后显示该数据
        //    AddMin_Max_Per();
        //}

        //private void InitDataGrid_CROSSTALK()
        //{
        //    if (dt != null) { dt.Dispose(); }
        //    dt = new DataTable();
        //    dt.Columns.Add("ID");
        //    dt.Columns.Add("X(mm)");
        //    dt.Columns.Add("Y(mm)");
        //    dt.Columns.Add("Z(mm)");
        //    dt.Columns.Add("U(°)");
        //    dt.Columns.Add("V(°)");
        //    dt.Columns.Add("L");
        //    dt.Columns.Add("cx");
        //    dt.Columns.Add("cy");
        //    dt.Columns.Add("u'");
        //    dt.Columns.Add("v'");
        //    DataView dv = new DataView(dt);


        //    ///定义数据表
        //    mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
        //    mydata.VerticalGridLinesBrush = Brushes.Gray;
        //    mydata.MinColumnWidth = 70;
        //    mydata.ItemsSource = dv;
        //    mydata.CanUserReorderColumns = false;
        //    mydata.CanUserAddRows = false;
        //    mydata.CanUserDeleteRows = false;
        //    mydata.CanUserSortColumns = false;


        //    for (int i = 0; i < 30; i++)
        //    {
        //        DataRow dr = dt.NewRow();
        //        dr["ID"] = i;
        //        dr["X(mm)"] = 100.00;
        //        dr["Y(mm)"] = 120.00;
        //        dr["Z(mm)"] = 120.00;
        //        dr["U(°)"] = 120.00;
        //        dr["V(°)"] = 120.00;
        //        dr["L"] = 2013.000;
        //        dr["cx"] = 0.0002;
        //        dr["cy"] = 0.0009;

        //        dt.Rows.Add(dr);
        //    }

        //    //测试结束后显示该数据
        //    AddMin_Max_Per();
        //}

        //private void InitDataGrid_POINT()
        //{
        //    this.Dispatcher.Invoke(new Action(() =>
        //    {
        //        if (dt != null) { dt.Dispose(); }
        //        dt = new DataTable();
        //        dt.Columns.Add("ID");
        //        dt.Columns.Add("X(mm)");
        //        dt.Columns.Add("Y(mm)");
        //        dt.Columns.Add("Z(mm)");
        //        dt.Columns.Add("U(°)");
        //        dt.Columns.Add("V(°)");
        //        dt.Columns.Add("L");
        //        dt.Columns.Add("X(三刺激值)");
        //        dt.Columns.Add("Y(三刺激值)");
        //        dt.Columns.Add("Z(三刺激值)");
        //        dt.Columns.Add("cx");
        //        dt.Columns.Add("cy");
        //        dt.Columns.Add("u'");
        //        dt.Columns.Add("v'");
        //        dt.Columns.Add("Tc");
        //        DataView dv = new DataView(dt);


        //        ///定义数据表
        //        mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
        //        mydata.VerticalGridLinesBrush = Brushes.Gray;
        //        mydata.MinColumnWidth = 70;
        //        mydata.ItemsSource = dv;
        //        mydata.CanUserReorderColumns = false;
        //        mydata.CanUserAddRows = false;
        //        mydata.CanUserDeleteRows = false;
        //        mydata.CanUserSortColumns = false;
        //    }));
        //}
        #endregion

        /// <summary>
        /// 定义初始化列表
        /// </summary>
        public void Init(object data)
        {
        }

        private void AddMin_Max_Per()
        {
            for (int i = 0; i < 3; i++)
            {
                DataRow dr = dt.NewRow();
                dr["ID"] = "";
                dr["X(mm)"] = "";
                dr["Y(mm)"] = "";
                dt.Rows.Add(dr);
            }

            DataRow drmin = dt.NewRow();
            drmin["ID"] = "Min";
            drmin["L"] = 90;
            dt.Rows.Add(drmin);

            DataRow drmax = dt.NewRow();
            drmax["ID"] = "Max";

            drmax["L"] = 90;
            dt.Rows.Add(drmax);


            DataRow drUni = dt.NewRow();
            drUni["ID"] = "Uniformity";
            drUni["L"] = "90%";
            dt.Rows.Add(drUni);
        }

        private void CopyCol_Click(object sender, RoutedEventArgs e)
        {
            //得到列号
            var list = ResultData.SelectedCells;
            var cellinfo = list.LastOrDefault();
            if (cellinfo == null || cellinfo.Column == null)
                return;

            if (cellinfo.Column.GetType() == typeof(DataGridTextColumn))
            {
                int col = cellinfo.Column.DisplayIndex;
                string txt = "";
                for (int i = 0; i < ResultDatatemp.Rows.Count; i++)
                {
                    txt += ResultDatatemp.Rows[i].ItemArray[col] + "\t";
                }
                Clipboard.SetText(txt);
                MessageBox.Show("已经拷贝");
            }
        }

        private void CopyRow_Click(object sender, RoutedEventArgs e)
        {
            //if(ResultData.SelectedItems.Count > 0)
            //{
            //    var selectedItem = ResultData.SelectedItems[0];
            //    string tableString = string.Join("\t", selectedItem);
            //    Clipboard.SetText(tableString);
            //    MessageBox.Show("已经拷贝");
            //}

            var list = ResultData.SelectedCells;
            var cellinfo = list.LastOrDefault();
            if (cellinfo == null || cellinfo.Column == null)
                return;

            int rowIndex = ResultData.Items.IndexOf(cellinfo.Item);
           
            var text = string.Join("\t", ResultDatatemp.Rows[rowIndex].ItemArray);
            Clipboard.SetText(text);
            MessageBox.Show("已经拷贝");
            
        }

        private void CopyVal_Click(object sender, RoutedEventArgs e)
        {
            var list = ResultData.SelectedCells;

            if(list.Count <= 0)
            {
                return;
            }
            object last_row = null;
            string txt = "";
            foreach (var cellInfo in list)
            {
                // 获取单元格所在的行
                var row = cellInfo.Item;
                if (last_row != null)
                {
                    if (last_row != row)
                    {
                        txt = txt.Trim() + "\n";
                        last_row = row;
                    }
                }
                else
                {
                    last_row = row;
                }
                // 获取单元格的列
                var column = cellInfo.Column as DataGridColumn;
                // 获取单元格的值
                var cellValue = (column?.GetCellContent(row) as TextBlock)?.Text;
                txt += cellValue + "\t";                
            }
            txt = txt.Trim();
            Clipboard.SetText(txt);
            MessageBox.Show("已经复制");
            return;

            var cellinfo = list.LastOrDefault();
            if (cellinfo == null || cellinfo.Column == null) 
                return;

            if (cellinfo.Column.GetType() == typeof(DataGridTextColumn))
            {
                var text = ((TextBlock)cellinfo.Column.GetCellContent(cellinfo.Item)).Text;
                Clipboard.SetText(text);
                MessageBox.Show("已经拷贝");
            }
        }
    }
}
