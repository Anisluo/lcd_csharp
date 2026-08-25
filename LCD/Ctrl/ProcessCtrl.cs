using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LCD.Data;
using LCD.dataBase;
using LCD.View;

namespace LCD.Ctrl
{
    /// <summary>
    /// 程序处理
    /// </summary>
    class ProcessCtrl
    {
        public event Action<string, string> ShowMessage; 
        /// <summary>
        /// 更新数据列表
        /// </summary>
        /// <param name="lst"></param>
        public delegate DataTable InitDataTemplateDelegate(List<string> lst, ENUMMESSTYLE mestype,string Name);
        public event InitDataTemplateDelegate InitDataTemplate;


      

        public delegate void InitResultDelegate(object mestype);
        public event InitResultDelegate InitResult;

        public event InitResultDelegate UpDataUi;

        public delegate void AddSingleResultDelegate(IData myobj,string TestItme);
        public event AddSingleResultDelegate AddSingleResult;
        public event AddSingleResultDelegate SpectrumResults;
        public event AddSingleResultDelegate warmupResult;
        /// <summary>
        /// Power
        /// </summary>
        /// <param name="myobj"></param>
        /// <param name="TestItme"></param>
        public delegate void SingleResultDelegate(Result myobj, string TestItme);
        public event SingleResultDelegate PowerResult;

        public delegate DataTable GetTableDelegate();
        public event GetTableDelegate GetTable;

        public delegate void ShowIndexDelegate(int index);
        public event ShowIndexDelegate ShowIndex;//显示index指标

        /// <summary>
        /// 计算补偿点位
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <param name="du"></param>
        /// <param name="dv"></param>
        /// <param name="dball"></param>
      

        private void CalcNewPoint_Mode1(ref double dx,
            ref double dy,
            ref double dz,
            ref double du,
            ref double dv,
            ref double dball)
        {
            dx += Project.Xorg;
            dy += Project.Yorg;
            dz += Project.Zorg;
            du += Project.Uorg;

            double deltdr = Project.cfg.machine.h0 * (Math.Sin(dv / 180 * Math.PI));//计算delta dr长度
            dx += deltdr * Math.Cos(du);
            dy += deltdr * Math.Sin(du);
            dv += Project.Vorg;
            dball += Project.Ballorg;
        }


        public void OnSinglePt()
        {
            TestMachine tst = Project.testMachine;
            IData str = tst.MeasureLxy();


        }


        private void OnMove2Point(double dx, double dy, double dz, double du, double dv, double dball,bool Home)
        {

            double zz = 0;
            double uu = 0;
            double CCC = 0;
            MovCtrl mvctrl = MovCtrl.GetInstance();
            double X=0, Y=0, Z=0, U=0, V=0, ball=0;
            mvctrl.UpdateCurAbsPos(ref X,ref Y,ref Z,ref U,ref V,ref ball);
            Console.WriteLine($"X[{X}],Y[{Y}],Z[{Z}],U[{U}],V[{V}]");

            Project.WriteLog("开始Z轴运动");
            if (Project.cfg.ax_z.IsEnable)
            {
                mvctrl.MoveAbsolute(Project.cfg.ax_z, dz, Home, Z);


            }
            Project.WriteLog("等待Z轴运动完成");
            mvctrl.WaitFiveAxeMoveFinish();//等待移动完成
            Project.WriteLog("Z轴运动完成，开始其他轴运动");

            if (Project.cfg.ax_x.IsEnable)
            {
                if (Project.cfg.ax_x.IsSecondValue) { mvctrl.MoveAbsoluteByVector(Project.cfg.ax_x, dx); }
                else { mvctrl.MoveAbsolute(Project.cfg.ax_x, dx,Home,X); }
            }

            if (Project.cfg.ax_y.IsEnable)
            {
                if (Project.cfg.ax_y.IsSecondValue) { mvctrl.MoveAbsoluteByVector(Project.cfg.ax_y, dy); }
                else { mvctrl.MoveAbsolute(Project.cfg.ax_y, dy, Home,Y); }
            }

            

            if (Project.cfg.ax_u.IsEnable)
            {
                mvctrl.MoveAbsolute(Project.cfg.ax_u, du, Home, U);
            }

            if (Project.cfg.ax_v.IsEnable)
            {

                mvctrl.MoveAbsolute(Project.cfg.ax_v, dv, Home,V);
            }
             
            if (Project.cfg.ax_ball.IsEnable)
            {

                mvctrl.MoveAbsolute(Project.cfg.ax_ball, dball, Home,ball);
            }

            mvctrl.WaitFiveAxeMoveFinish();//等待移动完成
        }

        public static ProcessCtrl obj;
        private ProcessCtrl() { }
        public static ProcessCtrl GetInstance()
        {
            if (obj == null) { obj = new ProcessCtrl(); }
            return obj;
        }
        /// <summary>
        /// 执行测试
        /// </summary>
        public void Run(string BarCode)
        {
            for (int i = 0; i < Project.lstInfos.Count; i++)
            {
                if (Project.FstStop) { return; }

                if (!Project.lstInfos[i].IsSelected)
                {
                    continue; ;
                }
                string TestName = Project.lstInfos[i].Name;
                DataTable dt = InitDataTemplate(Project.lstInfos[i].lstdata, Project.lstInfos[i].MESTYPE, TestName);//委托到Ui
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    double dx = dt.Columns.Contains("X(mm)") && double.TryParse(dt.Rows[j]["X(mm)"].ToString(), out dx) ? dx : 0.0;
                    double dy = dt.Columns.Contains("Y(mm)") && double.TryParse(dt.Rows[j]["Y(mm)"].ToString(), out dy) ? dy : 0.0;
                    double dz = dt.Columns.Contains("Z(mm)") && double.TryParse(dt.Rows[j]["Z(mm)"].ToString(), out dz) ? dz : 0.0;
                    double du = dt.Columns.Contains("U(°)") && double.TryParse(dt.Rows[j]["U(°)"].ToString(), out du) ? du : 0.0;
                    double dv = dt.Columns.Contains("V(°)") && double.TryParse(dt.Rows[j]["V(°)"].ToString(), out dv) ? dv : 0.0;
                    double dball = dt.Columns.Contains("Ball(mm)") && double.TryParse(dt.Rows[j]["Ball(mm)"].ToString(), out dball) ? dball : 0.0;

                    if ((Project.cfg.USeftMax<du+Project.Uorg||Project.cfg.USeftMin>du+Project.Uorg)&&Project.cfg.ZSeft<dz+Project.Zorg)
                    {
                        Console.WriteLine($"U轴最大安全距离【{Project.cfg.USeftMax}】-U轴相对原点【{Project.Uorg}】=【{Project.cfg.USeftMax - Project.Uorg}】<【{du}】");
                        Console.WriteLine($"U轴最小安全距离【{Project.cfg.USeftMin}】-U轴相对原点【{Project.Uorg}】=【{Project.cfg.USeftMin - Project.Uorg}】>【{du}】");
                        Console.WriteLine($"Z轴最小安全距离【{Project.cfg.ZSeft}】-U轴相对原点【{Project.Zorg}】=【{Project.cfg.ZSeft - Project.Zorg}】>【{dz}】");
                        MessageBox.Show($"【U】【U：{du}】或者【{dz}】超出安全范围");
                        return;
                    }
                }
            }
                //lock (this)
                //{
                Project.WriteLog(Project.TestFlag.ToString());
            if (Project.TestFlag==false)
            {
                Project.TestFlag = true;
                if (Project.cfg.TESTMACHINE == ENUMMACHINE.BMA7)
                {
                    Project.testMachine = Ctrl.BM7A.GetInstance();
                }
                else if (Project.cfg.TESTMACHINE == ENUMMACHINE.BM5A)
                {

                }
                else if (Project.cfg.TESTMACHINE == ENUMMACHINE.PR655)
                {

                }
                else if (Project.cfg.TESTMACHINE == ENUMMACHINE.CS2000)
                {
                    Project.testMachine = Ctrl.CS2000.GetInstance();
                }
                else if (Project.cfg.TESTMACHINE == ENUMMACHINE.SR3A)
                {
                    Project.testMachine = Ctrl.SR3A.GetInstance();
                }
                else if (Project.cfg.TESTMACHINE == ENUMMACHINE.BM5AS)
                {
                    //Project.testMachine = Ctrl.BM7A.GetInstance();
                }
                else if (Project.cfg.TESTMACHINE == ENUMMACHINE.CS2000)
                {
                    Project.testMachine = Ctrl.CS2000.GetInstance();
                }
                else if (Project.cfg.TESTMACHINE == ENUMMACHINE.Demo)
                {

                }

               

                user_id user = new user_id();

                Project.BarCodeID = user.Insert(new UserIdMode()
                {
                    BarCode = BarCode,
                    CreationTime = DateTime.Now.ToString()
                });

                //ProjectMode projectMode=new ProjectMode();

               

                //对仪器进行初始化
                //TestMachine ts = Project.testMachine;
                //ts.Init();//需要去掉，因为仪器默认开机初始化==已经打开了
                for (int i = 0; i < Project.lstInfos.Count; i++)
                {
                    if (Project.FstStop) { return; }

                    if (!Project.lstInfos[i].IsSelected)
                    {
                        continue; ;
                    }
                    

                    Project.ProjectID = ProjectMode.Insert(new ProjectModeClass()
                    {
                        UserID = Project.BarCodeID,
                        ModeType = (int)Project.lstInfos[i].MESTYPE,
                        projectName = Project.lstInfos[i].Name
                    });




                    string TestName = Project.lstInfos[i].Name;
                    DataTable dt = InitDataTemplate(Project.lstInfos[i].lstdata, Project.lstInfos[i].MESTYPE, TestName);//委托到Ui
                    InitResult(Project.lstInfos[i].MESTYPE);//刷新数据




                    UpDataUi(Project.lstInfos[i].MESTYPE);



                    switch (Project.lstInfos[i].MESTYPE)
                    {
                        case ENUMMESSTYLE._01_POINT: ProcessPointTemplateXYZ(dt, ENUMMESSTYLE._01_POINT, TestName); ; break;
                        case ENUMMESSTYLE._02_RESPONSE: ProcessPointTemplateXY(); ; break;
                        case ENUMMESSTYLE._03_SPECTRUM: ProcessPointTemplateXYZ(dt, ENUMMESSTYLE._03_SPECTRUM, TestName); ; break;
                        case ENUMMESSTYLE._04_FLICKER: ProcessPointTemplateXY(); ; break;
                        case ENUMMESSTYLE._05_CROSSTALK: ProcessPointTemplateXY(); ; break;
                        case ENUMMESSTYLE._06_ACR: ProcessPointTemplateXY(); ; break;
                        case ENUMMESSTYLE._07_warmup: ProcessPointTemplateXYZ(dt, ENUMMESSTYLE._07_warmup, TestName); ; break;
                        case ENUMMESSTYLE.Power:

                            MessageBox.Show("请将产品设置为待机状态");
                            UpDataUi(ENUMMESSTYLE.Power);

                            Project.ProjectID = ProjectMode.Insert(new ProjectModeClass()
                            {
                                UserID = Project.BarCodeID,
                                ModeType = (int)ENUMMESSTYLE.Power,
                                projectName = "PowerTest"
                            });

                            Project.WriteLog("功率测试开始");

                            Result result = Project.power.Query();

                            PowerResult?.Invoke(result, "PowerTest");

                            //Project.WriteLog(result.Voltage.ToString());
                            //Project.WriteLog(result.ElectricCurrent.ToString());
                            //Project.WriteLog(result.Power.ToString());

                            Project.WriteLog("功率测试结束");
                            MessageBox.Show("请连接线缆后点击确定按钮，完成其他测试项");
                            //if (Project.cfg.power.Enabled = true)
                            //{
                               
                            //}

                            break;
                        default: ProcessPointTemplateXY(); ; break;
                    }
                    DataTable table = GetTable();
                    ResultData rest = new ResultData();
                    rest.Name = Project.lstInfos[i].Name;
                    rest.table = table;
                    Project.lstDatas.Add(rest);//添加测试数据至测试结果界面
                    Project.WriteLog("模板：" + Project.lstInfos[i].Name + "测量完成!");
                }
                Project.TestFlag = false;
                OnMove2Point(0, Project.Yorg, 0, 0, 0, 0,false);
                Project.TestFlag = false;
                Project.WriteLog("模板组测量完毕！");
                Project.WriteLog("返回起始点位置！");
                Project.TestFlag = false;
                ShowMessage?.Invoke("模板组测量完毕","测试信息");
            }


            Project.TestFlag = false;

            //返回至起始点位置

            //}
        }

        //测试策略
        //处理两轴点位数据
        public void ProcessPointTemplateXY()
        {


        }

        //处理三轴点位数据
        public void ProcessPointTemplateXYZ(DataTable dt, ENUMMESSTYLE eNUMMESSTYLE,string testitem)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Project.FstStop) { return; }

                double dx = dt.Columns.Contains("X(mm)") && double.TryParse(dt.Rows[i]["X(mm)"].ToString(), out dx) ? dx : 0.0;
                double dy = dt.Columns.Contains("Y(mm)") && double.TryParse(dt.Rows[i]["Y(mm)"].ToString(), out dy) ? dy : 0.0;
                double dz = dt.Columns.Contains("Z(mm)") && double.TryParse(dt.Rows[i]["Z(mm)"].ToString(), out dz) ? dz : 0.0;
                double du = dt.Columns.Contains("U(°)") && double.TryParse(dt.Rows[i]["U(°)"].ToString(), out du) ? du : 0.0;
                double dv = dt.Columns.Contains("V(°)") && double.TryParse(dt.Rows[i]["V(°)"].ToString(), out dv) ? dv : 0.0;
                double dball = dt.Columns.Contains("Ball(mm)") && double.TryParse(dt.Rows[i]["Ball(mm)"].ToString(), out dball) ? dball : 0.0;

                string SerialNumber = dt.Columns.Contains("PG(序号)") ? dt.Rows[i]["PG(序号)"].ToString() : "//";
                string PGHint = dt.Columns.Contains("PG(提示信息)") ? dt.Rows[i]["PG(提示信息)"].ToString() : "";


               

                ShowIndex(i);


                PG pG = Project.PG;
                bool Ok=false;
                if (pG != null)
                {
                    if (SerialNumber.IndexOf(".") != -1)
                    {
                        string[] TempStr = SerialNumber.Split('.');
                        byte r = byte.Parse(TempStr[0]);
                        byte g = byte.Parse(TempStr[1]);
                        byte b = byte.Parse(TempStr[2]);
                        Ok= pG.colorControl(r, g, b);
                        if (Ok)
                        {
                            Project.WriteLog("RGB切换成功");
                        }
                        else
                        {
                            Project.WriteLog("RGB切换失败");
                        }
                    }
                    else if (SerialNumber.IndexOf("//") != -1)
                    {
                        MessageBox.Show("请手动切换PG");
                    }
                    else if (SerialNumber=="")
                    {

                    }
                    else
                    {
                        Ok=pG.changePattern(Project.PG.PatternList.ItemStrings[int.Parse(SerialNumber)].name);
                        if (Ok)
                        {
                            Project.WriteLog("图片切换成功");
                        }
                        else
                        {
                            Project.WriteLog("图片切换失败");
                        }
                    }

                    //pG.Send(SerialNumber);
                }
                else
                {
                     if (SerialNumber.IndexOf("//") != -1)
                    {
                        MessageBox.Show("请手动切换PG");
                    }
                }

                //CalcNewPoint(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);

                #region 测试注销




                EquipmentType equipmentType = (EquipmentType)Project.cfg.EQType;

                if (equipmentType == EquipmentType.Type_D)
                {
                    Console.WriteLine($"【x】：{dx}【Y】：{dy}【Z】：{dz}【u】：{du}【v】：{dv}【ball】：{dball}");

                    OnMove2Point(dx, dy, dz, du, dv, dball, false);//移动到指定位置
                }
                else
                {
                    PointF pointF = new PointF(dx, dy, dz, du, dv, equipmentType);

                    double Temp1 = Project.lstInfos[0].height;

                    PointF point = pointF.UpdateByAlgorithm1(Temp1, Project.PtCenter);

                    Project.WriteLog($"补偿前:X--》{point.X} Y--》{point.Y} Z--》{point.Z} U--》{point.U} V--》{point.V} ");
                    OnMove2Point(point.X, point.Y, point.Z, point.U, point.V, dball, false);
                }




                //OnMove2Point(dx, dy, dz, du, dv, dball);
                //OnMove2Point(point.X, point.Y, point.Z, point.U, point.V, dball);

                Project.WriteLog("移动到指定点位");


                //if (SerialNumber.IndexOf("//") != -1)
                //{
                //    System.Windows.Forms.MessageBox.Show($"请手动调PG【{PGHint}】");
                //}

                #endregion

                TestMachine ts = Project.testMachine;
                IData str = null;
                try
                {
                    if (eNUMMESSTYLE== ENUMMESSTYLE._01_POINT)
                    {
                        
                        str = ts.MeasureLxy();
                        str.Remark = PGHint;
                        AddSingleResult(str, testitem);
                    }
                    else if (eNUMMESSTYLE== ENUMMESSTYLE._03_SPECTRUM)
                    {
                        //Console.WriteLine("_03_SPECTRUM");

                        str = ts.MeasureSpectrum();
                        str.Remark = PGHint;
                        SpectrumResults?.Invoke(str, testitem);


                    }
                    else if(eNUMMESSTYLE== ENUMMESSTYLE._07_warmup)
                    {
                        double interval = dt.Columns.Contains("时长(s)") && double.TryParse(dt.Rows[i]["时长(s)"].ToString(), out dv) ? dv : 0.0;
                        double sometimes = dt.Columns.Contains("间隔(s)") && double.TryParse(dt.Rows[i]["间隔(s)"].ToString(), out dball) ? dball : 0.0;
                        int Cont = (int)(interval / sometimes);

                        for (int k = 0; k < Cont; k++)
                        {
                            if (Project.FstStop) { return; }

                            str = ts.MeasureLxy();
                            str.Remark = PGHint;
                            warmupResult?.Invoke(str, testitem);

                            System.Threading.Thread.Sleep((int)(sometimes*1000));

                        }
                        
                    }

                }
                catch (Exception e)
                {
                    Project.WriteLog("读取失败-->" + e.Message);
                    //Console.WriteLine("读取失败-->" + e.Message);
                }


                //Console.WriteLine("测试值--->"+str);

                //string[] res = ParseLxy(str);
                //object[] res = { 100, 100, 100, 100, 100, 100, 100, 100, 100 };
                //AddSingleResult(str, testitem);
            }
        }

        public void ProcessPointSingle()
        {
            TestMachine ts = Project.testMachine;
            IData str = ts.MeasureLxy();
            //string[] res = ParseLxy(str);
            AddSingleResult(str,"");
        }
        public void ProcessSpectrumSingle()
        {
            TestMachine ts = Project.testMachine;
            IData str = ts.MeasureSpectrum();
            //string[] res = ParseLxy(str);
            
            SpectrumResults(str, "");
            //AddSingleResult(str, "");
        }

        private string[] ParseLxy(string str)
        {
            switch (Project.cfg.TESTMACHINE)
            {
                case ENUMMACHINE.BMA7: return ParseLxy_BM7A(str);
                case ENUMMACHINE.USB2000: return ParseLxy_Common(str);
                default: return ParseLxy_Common(str);
            }
        }


        private string[] ParseLxy_Common(string str)
        {
            if (str == "") { return null; }
            string[] strs = str.Split(',');
            string[] data = new string[9];
            data[0] = strs[0];//L
            data[1] = strs[1];//X
            data[2] = strs[2];//Y
            data[3] = strs[3];//Z
            data[4] = strs[4];//cx
            data[5] = strs[5];//cy
            data[6] = strs[6];//u'
            data[7] = strs[7];//v'
            data[8] = strs[8];//Tc
            return data;
        }



        private string[] ParseLxy_BM7A(string str)
        {
            if (str == "") { return null; }
            string[] strs = str.Split('\n');

            string[] data = new string[9];
            data[0] = strs[12].Replace("\r", "");//L
            data[1] = strs[13].Replace("\r", "");//X
            data[2] = strs[14].Replace("\r", "");//Y
            data[3] = strs[15].Replace("\r", "");//Z
            data[4] = strs[16].Replace("\r", "");//cx
            data[5] = strs[17].Replace("\r", "");//cy
            data[6] = strs[18].Replace("\r", "");//u'
            data[7] = strs[19].Replace("\r", "");//v'
            data[8] = strs[20].Replace("\r", "");//Tc
            return data;
        }




        //处理五轴点位数据
        public void ProcessPointTemplateXYZUV()
        {

        }

        //处理crosstop测试数据
        public void ProcessXYCrossTop()
        {

        }

        //处理五轴加积分球
        public void ProcessPointTemplateXYZUVBall()
        {

        }

        //处理响应测试
        public void ProcessResponseXY()
        {

        }

        //处理ACR测试
        public void ProcessACRXY()
        {

        }

        //处理
        //Edit
    }

    //定义测量类型
    //public enum EnumMeasureStyle
    //{
    //    Point_XY,
    //    Point_XYZ,
    //    Point_XYZUV,
    //    CrossTop,
    //    Response,
    //    Spectrum,
    //}


}
