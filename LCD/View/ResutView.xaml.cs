using System;
using System.Collections.Generic;
using System.Data;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        private ENUMMESSTYLE ENUMMESSTYLE { get; set; }
        DataTable ResultDatatemp = new DataTable();
        public Type ResutType { get; set; }
        //public List<IData> Datas { get; set; } = new List<IData>();
        //private ENUMMESSTYLE MESSTYLE = ENUMMESSTYLE._01_POINT;//设置为01Point点位



        private bool ResBool = false;


        //计算统计数据
        public void calculateStatics(ENUMMESSTYLE eNUMMESSTYLE)
        {
            //var dataL = ResultDatatemp.AsEnumerable().Select(c => c.Field<string>("L")).ToList;

            string[] columL = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("L")).ToArray();

            string[] columX = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("X")).ToArray();
            string[] columY = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("Y")).ToArray();

            string[] columCx = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("Cx")).ToArray();
            string[] columCy = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("Cy")).ToArray();
            string[] columU = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("u")).ToArray();
            string[] columV = ResultDatatemp.AsEnumerable().Select(row => row.Field<string>("v")).ToArray();

            List<double> lstLs = new List<double>();
            List<double> lstXs = new List<double>();
            List<double> lstYs = new List<double>();

            List<double> lstcxs = new List<double>();
            List<double> lstcys = new List<double>();
            List<double> lstus = new List<double>();
            List<double> lstvs = new List<double>();
            //double columLMax = columL;
            //double columLMin = 0;
            for (int i = 0; i < ResultDatatemp.Rows.Count - 3; i++)
            {
                lstLs.Add(double.Parse(columL[i]));
                lstXs.Add(double.Parse(columX[i]));
                lstYs.Add(double.Parse(columY[i]));

                lstcxs.Add(double.Parse(columCx[i]));
                lstcys.Add(double.Parse(columCy[i]));
                lstus.Add(double.Parse(columU[i]));
                lstvs.Add(double.Parse(columV[i]));
            }

            int cnt = ResultDatatemp.Rows.Count;

            double meanL = lstLs.Average();
            double meanX = lstXs.Average();

            double meanY = lstYs.Average();

            double meanCx = lstcxs.Average();
            double meanCy = lstcys.Average();
            double meanU = lstus.Average();
            double meanV = lstvs.Average();

            double maxL = lstLs.Max();
            double minL = lstLs.Min();


            double maxX = lstXs.Max();
            double minX = lstXs.Min();


            double maxY = lstYs.Max();
            double minY = lstYs.Min();


            double maxCx = lstcxs.Max();
            double minCx = lstcxs.Min();

            double maxCy = lstcys.Max();
            double minCy = lstcys.Min();

            double maxU = lstus.Max();
            double minU = lstus.Min();

            double maxV = lstvs.Max();
            double minV = lstvs.Min();



            ResultDatatemp.Rows[cnt-3]["L"] = maxL.ToString("0.0");
            ResultDatatemp.Rows[cnt-3]["X"] = maxL.ToString("0.0000");
            ResultDatatemp.Rows[cnt-3]["Y"] = maxL.ToString("0.0000");
            ResultDatatemp.Rows[cnt-3]["Cx"] = maxCx.ToString("0.0000");
            ResultDatatemp.Rows[cnt-3]["Cy"] = maxCy.ToString("0.0000");
            ResultDatatemp.Rows[cnt-3]["u"] = maxU.ToString("0.0000");
            ResultDatatemp.Rows[cnt-3]["v"] = maxV.ToString("0.0000");

            ResultDatatemp.Rows[cnt-2]["L"] = minL.ToString("0.0");
            ResultDatatemp.Rows[cnt-2]["X"] = minX.ToString("0.0");
            ResultDatatemp.Rows[cnt-2]["Y"] = minY.ToString("0.0");
            ResultDatatemp.Rows[cnt-2]["Cx"] = minCx.ToString("0.0000");
            ResultDatatemp.Rows[cnt-2]["Cy"] = minCy.ToString("0.0000");
            ResultDatatemp.Rows[cnt-2]["u"] = minU.ToString("0.0000");
            ResultDatatemp.Rows[cnt-2]["v"] = minV.ToString("0.0000");



            ResultDatatemp.Rows[cnt-1]["L"] = meanL.ToString("0.0");
            ResultDatatemp.Rows[cnt-1]["X"] = meanX.ToString("0.0");
            ResultDatatemp.Rows[cnt-1]["Y"] = meanY.ToString("0.0");
            ResultDatatemp.Rows[cnt-1]["Cx"] = meanCx.ToString("0.0000");
            ResultDatatemp.Rows[cnt-1]["Cy"] = meanCy.ToString("0.0000");
            ResultDatatemp.Rows[cnt-1]["u"] = meanU.ToString("0.0000");
            ResultDatatemp.Rows[cnt-1]["v"] = meanV.ToString("0.0000");





        }


        public void initResultDataTemp(ENUMMESSTYLE eNUMMESSTYLE)
        {

            if(eNUMMESSTYLE==ENUMMESSTYLE._01_POINT||ENUMMESSTYLE==ENUMMESSTYLE._03_SPECTRUM)
            {
            List<String> itemName=new List<string>();
            itemName.Add("Max");
            itemName.Add("Min");
            itemName.Add("Avg");

            for (int i = 0; i < 3; i++)
            { 
            
            DataRow dataRow = ResultDatatemp.NewRow();
            dataRow["测试项"] = itemName[i];
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
            if (eNUMMESSTYLE == ENUMMESSTYLE._01_POINT)
            {


                ResultDatatemp.Columns.Add("测试项");
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
                ResultDatatemp.Columns.Add("完成时间");
                ResultDatatemp.Columns.Add("备注");

                //for (int i = 0; i < 4; i++)
                //{ 

                //initResultDataTemp(eNUMMESSTYLE);
                //}

              //  initResultDataTemp(eNUMMESSTYLE);

            }
            else if (eNUMMESSTYLE == ENUMMESSTYLE._02_RESPONSE)
            {

            }
            else if (eNUMMESSTYLE==ENUMMESSTYLE._03_SPECTRUM)
            {
                ResultDatatemp.Columns.Add("测试项");
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
                ResultDatatemp.Columns.Add("完成时间");
                ResultDatatemp.Columns.Add("备注");
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

            }
            else if ((eNUMMESSTYLE == ENUMMESSTYLE._07_warmup))
            {
                ResultDatatemp.Columns.Add("测试项");
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
                ResultDatatemp.Columns.Add("完成时间");
                ResultDatatemp.Columns.Add("备注");
            }
            else if ((eNUMMESSTYLE==ENUMMESSTYLE.Power))
            {
                ResultDatatemp.Columns.Add("测试项");
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("电压");
                //ResultDatatemp.Columns.Add("");
                //ResultDatatemp.Columns.Add("");
                //ResultDatatemp.Columns.Add("");
                ResultDatatemp.Columns.Add("电流");
                ResultDatatemp.Columns.Add("功率");
            }

            initResultDataTemp(eNUMMESSTYLE);

           // initResultDataTemp(eNUMMESSTYLE);

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
        }

        public void ShowAll(bool ShowAll)
        {
            ResBool = ShowAll;

            if (ShowAll)
            {
                //this.ResultData.Columns[0].Visibility = Visibility.Visible;
                //this.ResultData.Columns[1].Visibility = Visibility.Visible;
                //this.ResultData.Columns[5].Visibility = Visibility.Visible;
                //this.ResultData.Columns[6].Visibility = Visibility.Visible;
                //this.ResultData.Columns[7].Visibility = Visibility.Visible;
                //this.ResultData.Columns[8].Visibility = Visibility.Visible;
                //this.ResultData.Columns[9].Visibility = Visibility.Visible;
                //this.ResultData.Columns[10].Visibility = Visibility.Visible;
                //this.ResultData.Columns[11].Visibility = Visibility.Visible;
                //this.ResultData.Columns[12].Visibility = Visibility.Visible;

                for (int i=0;i<this.ResultData.Columns.Count; i++) {
                    this.ResultData.Columns[i].Visibility = Visibility.Visible;

                }


            }
            else
            {
                if (ENUMMESSTYLE == ENUMMESSTYLE._03_SPECTRUM)
                {
                    return;
                }
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

        public DataTable GetTale()
        {
            return dt;
        }

        public void AddSingleData(IData objs,string testname)
        {

            if (objs == null) { Project.WriteLog("采集数据失败！"); return; }
            DataRow dataRow = null;
            if (ENUMMESSTYLE == ENUMMESSTYLE._01_POINT)
            {
                //if (ResultDatatemp == null)
                //   ResultDatatemp = new DataTable(); 
                if (objs.Remark == null) {
                    objs.Remark = "";
                }
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = (ResultDatatemp.Rows.Count-3 + 1).ToString(),
                    //Num="3",
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
                    Remark = objs.Remark.Trim()
                }) ;
                dataRow = ResultDatatemp.NewRow();
                dataRow["测试项"] = testname;
                dataRow["Num"] = ResultDatatemp.Rows.Count-3 + 1;
                dataRow["L"] = objs.L;
                dataRow["X"] = objs.X;
                dataRow["Y"] = objs.L;
                dataRow["Z"] = objs.Z;
                dataRow["Cx"] = objs.Cx;
                dataRow["Cy"] = objs.Cy;
                dataRow["u"] = objs.u;
                dataRow["v"] = objs.v;
                dataRow["CCT"] = objs.CCT;
                dataRow["完成时间"] = objs.Time;
                dataRow["备注"] = objs.Remark;


                


            }
            else if (ENUMMESSTYLE == ENUMMESSTYLE._02_RESPONSE)
            {

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
                    Num = (ResultDatatemp.Rows.Count-3 + 1).ToString(),
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
                    Remark = objs.Remark.Trim()
                });
                dataRow = ResultDatatemp.NewRow();
                dataRow["测试项"] = testname;
                dataRow["Num"] = ResultDatatemp.Rows.Count -3+ 1;
                dataRow["L"] = objs.L;
                dataRow["X"] = objs.X;
                dataRow["Y"] = objs.L;
                dataRow["Z"] = objs.Z;
                dataRow["Cx"] = objs.Cx;
                dataRow["Cy"] = objs.Cy;
                dataRow["u"] = objs.u;
                dataRow["v"] = objs.v;
                dataRow["CCT"] = objs.CCT;
                dataRow["完成时间"] = objs.Time;
                dataRow["备注"] = objs.Remark;
                for (int i = 0; i < 401; i++)
                {
                    SpectrumData.Insert(new SpectrumDataMode()
                    {
                        Project_id = Project.ProjectID,
                        DataName = $"{i + 380}",
                        dataValue = objs.SpectrumData[i].ToString("E")
                     });
                    // 与正常工作的 guoxian 版本一致：写入字符串(.ToString("E"))而非裸 double。
                    dataRow[$"{i + 380}"] = objs.SpectrumData[i].ToString("E");
                }

            }
            else if (ENUMMESSTYLE == ENUMMESSTYLE._04_FLICKER)
            {

            }
            else if (ENUMMESSTYLE == ENUMMESSTYLE._05_CROSSTALK)
            {

            }
            else if ((ENUMMESSTYLE == ENUMMESSTYLE._07_warmup))
            {
                TestData.Insert(new TestDataMode()
                {
                    Project_id = Project.ProjectID,
                    Num = (ResultDatatemp.Rows.Count-3 + 1).ToString(),
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
                    Remark = objs.Remark.Trim()
                });
                //Console.WriteLine($"-->{testname}");
                dataRow = ResultDatatemp.NewRow();
                dataRow["测试项"] = testname;
                dataRow["Num"] = ResultDatatemp.Rows.Count-3 + 1;
                dataRow["L"] = objs.L;
                dataRow["X"] = objs.X;
                dataRow["Y"] = objs.L;
                dataRow["Z"] = objs.Z;
                dataRow["Cx"] = objs.Cx;
                dataRow["Cy"] = objs.Cy;
                dataRow["u"] = objs.u;
                dataRow["v"] = objs.v;
                dataRow["CCT"] = objs.CCT;
                dataRow["完成时间"] = objs.Time;
                dataRow["备注"] = objs.Remark;
            }



            ResultDatatemp.Rows.InsertAt(dataRow, ResultDatatemp.Rows.Count-3 );


            calculateStatics(ENUMMESSTYLE);

            DataView dv = new DataView(ResultDatatemp);


            
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.ResultData.ItemsSource = dv;
                ShowAll(ResBool);
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
            dataRow["测试项"] = testname;
            dataRow["Num"] = ResultDatatemp.Rows.Count;
            dataRow["电压"] = objs.Voltage.ToString();
            dataRow["电流"] = objs.ElectricCurrent.ToString();
            dataRow["功率"] = objs.Power.ToString();
           



            ResultDatatemp.Rows.InsertAt(dataRow, ResultDatatemp.Rows.Count + 1);

            DataView dv = new DataView(ResultDatatemp);



            this.Dispatcher.Invoke(new Action(() =>
            {
                this.ResultData.ItemsSource = dv;
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
    }
}
