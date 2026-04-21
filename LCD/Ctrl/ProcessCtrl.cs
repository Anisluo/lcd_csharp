using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using LCD.Core.Services;
using LCD.Data;
using LCD.dataBase;
using LCD.View;

namespace LCD.Ctrl
{
    /// <summary>
    /// 程序处理
    /// </summary>
    public class ProcessCtrl
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
        public event AddSingleResultDelegate AddResponseResult;
        public event AddSingleResultDelegate SpectrumResults;
        public event AddSingleResultDelegate warmupResult;
        public event AddSingleResultDelegate crosstalkResult;

        public delegate void UpdateResponseLinesDelegate(List<double> slist, RiseFallPositon positon);
        public event UpdateResponseLinesDelegate UpdateResponseLine;

        //执行测量标准检查啊
        public delegate void RunTestStdDelegate(InfoData info);
        //点测试的测量标准检查
        public event RunTestStdDelegate RunPointTestStd;

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

        public void OnMove2Point(double dx, double dy, double dz, double du, double dv, double dball,bool Home)
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
                else {
                    Project.WriteLog("Y轴单轴运动");//20260311，兼容普通运动方式和Y轴插补运动
                    mvctrl.MoveAbsolute(Project.cfg.ax_y, dy-Project.Yorg, Home,Y); }
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
            if(Project.cfg.AxiesDoneDelay>0)
            {
                Thread.Sleep((int)Project.cfg.AxiesDoneDelay);
            }
            else
            {
                LogHelper.Instance.Write("轴运行结束等待时间未设置");
                Thread.Sleep(20);//这是默认延时时间
            }
        }

        public static ProcessCtrl obj;
        private ProcessCtrl() { }
        public static ProcessCtrl GetInstance()
        {
            if (obj == null) { obj = new ProcessCtrl(); }
            return obj;
        }

        private void check_pause()
        {
            while(Project.TstPause)
            {
                Thread.Sleep(10);
            }
        }

        private void RebindMachineIfNeeded(params ENUMMACHINE[] machines)
        {
            ENUMMACHINE current = Project.cfg.TESTMACHINE;
            bool match = false;
            for (int i = 0; i < machines.Length; i++)
            {
                if (machines[i] == current) { match = true; break; }
            }
            if (!match) return;
            Project.testMachine = LightMeterFactory.Create(current);
            if (Project.testMachine == null) return;
            Project.testMachine.Config = Project.cfg.GetBusConfigFor(current);
            if (Project.testMachine.IsOpen == false)
            {
                Project.testMachine.Init();
                if (current == ENUMMACHINE.MS01) Project.testMachine.AutoCheck();
            }
        }

        /// <summary>
        /// 执行测试
        /// </summary>
        public void Run(string BarCode)
        {
            for (int i = 0; i < Project.lstInfos.Count; i++)
            {
                if (Project.FstStop) { return; }
                check_pause();

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

                Project.testMachine = LightMeterFactory.Create(Project.cfg.TESTMACHINE);
                if (Project.testMachine != null)
                {
                    Project.testMachine.Config = Project.cfg.GetBusConfigFor(Project.cfg.TESTMACHINE);

                    if (Project.testMachine.IsOpen == false)
                    {
                        Project.testMachine.Init();
                    }
                    if (Project.cfg.TESTMACHINE == ENUMMACHINE.MS01)
                    {
                        Project.testMachine.AutoCheck();
                    }
                }

                check_pause();

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
                    check_pause();

                    //没有选中的不需要执行啊
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


                    check_pause();

                    string TestName = Project.lstInfos[i].Name;
                    DataTable dt = InitDataTemplate(Project.lstInfos[i].lstdata, Project.lstInfos[i].MESTYPE, TestName);//委托到Ui
                    InitResult(Project.lstInfos[i].MESTYPE);//刷新数据

                    check_pause();


                    UpDataUi(Project.lstInfos[i].MESTYPE);

                    check_pause();

                    double height = Project.lstInfos[i].height;

                    switch (Project.lstInfos[i].MESTYPE)
                    {
                        case ENUMMESSTYLE._01_POINT:
                            RebindMachineIfNeeded(ENUMMACHINE.SR3A, ENUMMACHINE.SR5A);
                            ProcessPointTemplateXYZ(dt, ENUMMESSTYLE._01_POINT, TestName, Project.lstInfos[i].id,height); break;

                        case ENUMMESSTYLE._02_RESPONSE:
                            RebindMachineIfNeeded(ENUMMACHINE.Admesy);
                            ProcessPointTemplateXY(dt, ENUMMESSTYLE._01_POINT, TestName);
                            break;
                        case ENUMMESSTYLE._03_SPECTRUM:
                            RebindMachineIfNeeded(ENUMMACHINE.SR3A, ENUMMACHINE.SR5A, ENUMMACHINE.MS01);
                            ProcessPointTemplateXYZ(dt, ENUMMESSTYLE._03_SPECTRUM, TestName, Project.lstInfos[i].id,height); ; break;
                        case ENUMMESSTYLE._04_FLICKER: ProcessPointTemplateXY(dt, ENUMMESSTYLE._01_POINT, TestName); ; break;
                        case ENUMMESSTYLE._05_CROSSTALK: ProcessPointTemplateXY(dt, ENUMMESSTYLE._05_CROSSTALK, TestName); ; break;
                        case ENUMMESSTYLE._06_ACR: ProcessPointTemplateXY(dt, ENUMMESSTYLE._01_POINT, TestName); ; break;
                        case ENUMMESSTYLE._07_warmup: ProcessPointTemplateXYZ(dt, ENUMMESSTYLE._07_warmup, TestName, Project.lstInfos[i].id, height); ; break;
                        case ENUMMESSTYLE.TCO: ProcessPointTemplateXYZ(dt, ENUMMESSTYLE._01_POINT, TestName, Project.lstInfos[i].id, height); break;
                        case ENUMMESSTYLE.Power:

                            MessageBox.Show("请将产品设置为待机状态");
                            UpDataUi(ENUMMESSTYLE.Power);

                            Project.ProjectID = ProjectMode.Insert(new ProjectModeClass()
                            {
                                UserID = Project.BarCodeID,
                                ModeType = (int)ENUMMESSTYLE.Power,
                                projectName = "PowerTest"
                            });

                            if(Project.power == null)
                            {
                                Project.power = new Ctrl.Power();
                                Project.power.Init();
                            }
                            else
                            {
                                if(Project.power.IsOpen==false)
                                {
                                    Project.power.Init();
                                }
                            }

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
                        default: ProcessPointTemplateXY(dt, ENUMMESSTYLE._01_POINT, TestName);  break;
                    }
                    DataTable table = GetTable();
                    ResultData rest = new ResultData();
                    rest.Name = Project.lstInfos[i].Name;
                    rest.table = table;
                    Project.lstDatas.Add(rest);//添加测试数据至测试结果界面
                    Project.WriteLog("模板：" + Project.lstInfos[i].Name + "测量完成!");
                }
                Project.TestFlag = false;
                check_pause();
                //OnMove2Point(0, Project.Yorg, 0, 0, 0, 0,false);
                if (Project.cfg.ax_y.IsSecondValue) {
                    OnMove2Point(0, Project.Yorg, 0, 0, 0, 0,false);
                    //OnMove2Point(0, 0+Project.cfg.yorg, 0, 0, 0, 0, false);
                }
                else
                {
                    OnMove2Point(0, 0, 0, 0, 0, 0, false);
                }
                  
                Project.TestFlag = false;
                Project.WriteLog("模板组测量完毕！");
                Project.WriteLog("返回起始点位置！");
                Project.TestFlag = false;
                ShowMessage?.Invoke("模板组测量完毕","测试信息");
            }


            Project.TestFlag = false;

            //返回至起始点位置

            //}

            //测试结束了啊，把选中标志都清除掉,这个功能不要用了
            //for (int i = 0; i < Project.lstInfos.Count; i++)
            //{
            //    if (Project.lstInfos[i].IsSelected)
            //    {
            //        Project.lstInfos[i].IsSelected = false;
            //    }
            //}
            ////最后保存一下啊
            //Project.SaveTemplate("Template.xml");
        }

        //测试策略
        //处理两轴点位数据
        public void ProcessPointTemplateXY(DataTable dt, ENUMMESSTYLE eNUMMESSTYLE, string testitem)
        {
            //在这里测试响应啊
            Console.WriteLine();

            //05的需要测两次
            int count = 0;
            CROSSTALK_BACK:            

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                check_pause();

                double dx = dt.Columns.Contains("X(mm)") && double.TryParse(dt.Rows[i]["X(mm)"].ToString(), out dx) ? dx : 0.0;
                double dy = dt.Columns.Contains("Y(mm)") && double.TryParse(dt.Rows[i]["Y(mm)"].ToString(), out dy) ? dy : 0.0;
                double dz = dt.Columns.Contains("Z(mm)") && double.TryParse(dt.Rows[i]["Z(mm)"].ToString(), out dz) ? dz : 0.0;
                double du = dt.Columns.Contains("U(°)") && double.TryParse(dt.Rows[i]["U(°)"].ToString(), out du) ? du : 0.0;
                double dv = dt.Columns.Contains("V(°)") && double.TryParse(dt.Rows[i]["V(°)"].ToString(), out dv) ? dv : 0.0;
                double dball = dt.Columns.Contains("Ball(mm)") && double.TryParse(dt.Rows[i]["Ball(mm)"].ToString(), out dball) ? dball : 0.0;

                string SerialNumber = dt.Columns.Contains("PG(序号)") ? dt.Rows[i]["PG(序号)"].ToString() : "//";
                string PGHint = dt.Columns.Contains("PG(提示信息)") ? dt.Rows[i]["PG(提示信息)"].ToString() : "";
                ShowIndex(i);


                var pG = Project.PG;
                bool Ok = false;
                if (pG != null)
                {
                    if (SerialNumber.IndexOf(".") != -1)
                    {
                        string[] TempStr = SerialNumber.Split('.');
                        byte r = byte.Parse(TempStr[0]);
                        byte g = byte.Parse(TempStr[1]);
                        byte b = byte.Parse(TempStr[2]);
                        Ok = pG.SetColor(r, g, b);
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
                    else if (SerialNumber == "")
                    {

                    }
                    else
                    {
                        Ok = pG.ChangePattern(Project.PG.PatternList[int.Parse(SerialNumber)]);
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

                //if (equipmentType == EquipmentType.Type_D)
                {
                    Console.WriteLine($"【x】：{dx}【Y】：{dy}【Z】：{dz}【u】：{du}【v】：{dv}【ball】：{dball}");
                    check_pause();
                    OnMove2Point(dx, dy, dz, du, dv, dball, false);
                }
                //else
                //{
                //    PointF pointF = new PointF(dx, dy, dz, du, dv, equipmentType);

                //    double Temp1 = Project.lstInfos[0].height;

                //    PointF point = pointF.UpdateByAlgorithm1(Temp1, Project.PtCenter);

                //    Project.WriteLog($"补偿前:X--》{point.X} Y--》{point.Y} Z--》{point.Z} U--》{point.U} V--》{point.V} ");
                //    OnMove2Point(point.X, point.Y, point.Z, point.U, point.V, dball, false);
                //}

                Project.WriteLog("移动到指定点位");

                #endregion

                TestMachine ts = Project.testMachine;
                IData str = null;
                try
                {
                    //重新改下结果
                    //eNUMMESSTYLE = ENUMMESSTYLE._02_RESPONSE;
                    if (eNUMMESSTYLE ==ENUMMESSTYLE._01_POINT)
                    {
                        str = ts.Measure();
                        if (str != null)
                        {
                            Project.WriteLog("Response测试完成");

                            str.X = dx;
                            str.Y = dy;
                            str.Z = dz;
                            str.CoordX = dx;
                            str.CoordY = dy;
                            str.CoordZ = dz;
                            str.CoordU = du;
                            str.CoordV = dv;
                            double low = dt.Columns.Contains("Low(灰阶)") && double.TryParse(dt.Rows[i]["Low(灰阶)"].ToString(), out low) ? low : 0.0;
                            double high = dt.Columns.Contains("High(灰阶)") && double.TryParse(dt.Rows[i]["High(灰阶)"].ToString(), out high) ? high : 0.0;
                            str.Low = low;
                            str.High = high;

                            AddResponseResult(str, testitem);

                            Admesy admesy = (Admesy)ts;
                            if (UpdateResponseLine != null)
                            {
                                if (admesy.vlist != null)
                                {
                                    UpdateResponseLine(admesy.vlist, admesy.positon);
                                }
                                else
                                {
                                    Project.WriteLog("Admesy测量数据为空,不能绘图");
                                }
                            }
                            else
                            {
                                Project.WriteLog("UpdateResponseLine为空,不能绘图");
                            }
                        }
                        else
                        {
                            Project.WriteLog("测试失败，请检查设备连接");
                            //Project.WriteLog("Response测试失败,");
                            break;
                        }
                    }
                    else if(eNUMMESSTYLE == ENUMMESSTYLE._02_RESPONSE)
                    {
                        
                    }
                    else if (eNUMMESSTYLE == ENUMMESSTYLE._05_CROSSTALK)
                    {
                        str = ts.MeasureLxy();
                        if (str == null)
                        {
                            Project.WriteLog("测试失败，请检查设备连接或者串口打开失败");
                            break;
                        }
                        else
                        {
                            str.Point_Count = dt.Rows.Count;
                            str.Remark = PGHint;
                            str.CoordX = dx;
                            str.CoordY = dy;
                            str.CoordZ = dz;
                            str.CoordU = du;
                            str.CoordV = dv;
                            //计算lab
                            IData lab = ColorService.ToLab(str.X, str.Y, str.Z);
                            str.Lcolor = lab.Lcolor;
                            str.Acolor = lab.Acolor;
                            str.Bcolor = lab.Bcolor;
                            if (count == 0)
                            {
                                str.CT_done = false;
                            }
                            else
                            {
                                str.CT_done = true;
                            }
                            //计算lab啊
                            crosstalkResult(str, testitem);
                        }
                    }
                }
                catch (Exception e)
                {
                    Project.WriteLog("读取失败-->" + e.Message+Environment.NewLine+e.StackTrace);
                    //Console.WriteLine("读取失败-->" + e.Message);
                }
            }

            count++;
            if ((eNUMMESSTYLE == ENUMMESSTYLE._05_CROSSTALK)&&(count==1))
            {

                //提示切换PG
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ShowMsg form = new ShowMsg("请切换串扰 PG");
                    form.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    form.Topmost = true;
                    form.ShowDialog();
                }));
                

                goto CROSSTALK_BACK;
            }
        }

        //处理三轴点位数据
        public void ProcessPointTemplateXYZ(DataTable dt, ENUMMESSTYLE eNUMMESSTYLE,string testitem,int id,double height)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
			     if (Project.FstStop) { return; }
                check_pause();

                double dx = dt.Columns.Contains("X(mm)") && double.TryParse(dt.Rows[i]["X(mm)"].ToString(), out dx) ? dx : 0.0;
                double dy = dt.Columns.Contains("Y(mm)") && double.TryParse(dt.Rows[i]["Y(mm)"].ToString(), out dy) ? dy : 0.0;
                double dz = dt.Columns.Contains("Z(mm)") && double.TryParse(dt.Rows[i]["Z(mm)"].ToString(), out dz) ? dz : 0.0;
                double du = dt.Columns.Contains("U(°)") && double.TryParse(dt.Rows[i]["U(°)"].ToString(), out du) ? du : 0.0;
                double dv = dt.Columns.Contains("V(°)") && double.TryParse(dt.Rows[i]["V(°)"].ToString(), out dv) ? dv : 0.0;
                double dball = dt.Columns.Contains("Ball(mm)") && double.TryParse(dt.Rows[i]["Ball(mm)"].ToString(), out dball) ? dball : 0.0;

                string SerialNumber = dt.Columns.Contains("PG(序号)") ? dt.Rows[i]["PG(序号)"].ToString() : "//";
                string PGHint = dt.Columns.Contains("PG(提示信息)") ? dt.Rows[i]["PG(提示信息)"].ToString() : "";


               

                ShowIndex(i);


                var pG = Project.PG;
                bool Ok=false;
                if (pG != null)
                {
                    if (SerialNumber.IndexOf(".") != -1)
                    {
                        string[] TempStr = SerialNumber.Split('.');
                        byte r = byte.Parse(TempStr[0]);
                        byte g = byte.Parse(TempStr[1]);
                        byte b = byte.Parse(TempStr[2]);
                        Ok= pG.SetColor(r, g, b);
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
                        Ok=pG.ChangePattern(Project.PG.PatternList[int.Parse(SerialNumber)]);
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



                check_pause();
                EquipmentType equipmentType = (EquipmentType)Project.cfg.EQType;

                if (equipmentType == EquipmentType.Type_D)
                {
                    Console.WriteLine($"【x】：{dx}【Y】：{dy}【Z】：{dz}【u】：{du}【v】：{dv}【ball】：{dball}");
                    check_pause();
                    OnMove2Point(dx, dy, dz, du, dv, dball, false);//移动到指定位置
                }
                else
                {
                    PointF pointF = new PointF(dx, dy, dz, du, dv, equipmentType);

                    //double height = Project.lstInfos[i].height;
                    double Temp1 = height;// Project.lstInfos[0].height;

                    if(Project.cfg.ax_z.CompensationDirection)
                    {                        
                        Temp1 = -Temp1;
                        Project.WriteLog("厚度补偿取反："+ Temp1);
                    }
                    PointF point = null;
                    


                    if ((du == 0) && (dv == 0))
                    {
                        //u和v为0的时候，不调整z的值
                        point = new PointF(dx, dy, dz, du, dv, equipmentType);

                        if (Project.cfg.ax_y.IsSecondValue)
                        {
                            point.Y += Project.Yorg;
                        }


                    }
                    else
                    {
                        point = pointF.UpdateByAlgorithm1(Temp1, Project.PtCenter);
                        //底下是新方法计算
                        //var xyz = PointF.RotatePoint(dx,dy,Temp1,du,dv, Project.PtCenter);
                        //point = new PointF( xyz.newX-(Project.Xorg - Project.PtCenter.X) , 
                        //                    xyz.newY-(Project.Yorg - Project.PtCenter.Y),
                        //                    xyz.newH- (Project.Zorg - Project.PtCenter.Z), 
                        //                    du+ Project.Uorg - Project.PtCenter.U, 
                        //                    dv + Project.Vorg - Project.PtCenter.V, 
                        //                    equipmentType);
                        //
                 
                    }
                    check_pause();
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

                check_pause();

                TestMachine ts = Project.testMachine;
                IData str = null;
                try
                {
                    if (eNUMMESSTYLE== ENUMMESSTYLE._01_POINT)
                    {
                        
                        str = ts.MeasureLxy();
                        if (str == null)
                        {
                            Project.WriteLog("测试失败，请检查设备连接或者串口打开失败");
                            break;
                        }
                        else
                        {
                            str.Remark = PGHint;
                            str.CoordX = dx;
                            str.CoordY = dy; 
                            str.CoordZ = dz;
                            str.CoordU = du;
                            str.CoordV = dv;
                            //计算lab
                            IData lab = ColorService.ToLab(str.X,str.Y,str.Z);
                            str.Lcolor = lab.Lcolor;
                            str.Acolor =lab.Acolor;
                            str.Bcolor = lab.Bcolor;
                            //计算lab啊
                            AddSingleResult(str, testitem);
                        }
                    }
                    else if (eNUMMESSTYLE== ENUMMESSTYLE._03_SPECTRUM)
                    {
                        //Console.WriteLine("_03_SPECTRUM");

                        str = ts.MeasureSpectrum();
                        if(str == null)
                        {
                            Project.WriteLog("测试失败，请检查设备连接");
                            break;
                        }
                        str.Remark = PGHint;
                        str.CoordX = dx;
                        str.CoordY = dy;
                        str.CoordZ = dz;
                        str.CoordU = du;
                        str.CoordV = dv;
                        //计算lab
                        IData lab = ColorService.ToLab(str.X, str.Y, str.Z);
                        str.Lcolor = lab.Lcolor;
                        str.Acolor = lab.Acolor;
                        str.Bcolor = lab.Bcolor;
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
                            check_pause();

                            str = ts.MeasureLxy();
                            if(str==null)
                            {
                                Project.WriteLog("测试失败，请检查设备连接");
                                break;
                            }
                            if (str != null)
                            {
                                str.Remark = PGHint;
                                str.CoordX = dx;
                                str.CoordY = dy;
                                str.CoordZ = dz;
                                str.CoordU = du;
                                str.CoordV = dv;
                                warmupResult?.Invoke(str, testitem);

                                System.Threading.Thread.Sleep((int)(sometimes * 1000));
                            }
                        }
                        
                    }

                }
                catch (Exception e)
                {
                    Project.WriteLog("读取失败-->" + e.Message + Environment.NewLine + e.StackTrace);
                    //Console.WriteLine("读取失败-->" + e.Message);
                }


                //Console.WriteLine("测试值--->"+str);

                //string[] res = ParseLxy(str);
                //object[] res = { 100, 100, 100, 100, 100, 100, 100, 100, 100 };
                //AddSingleResult(str, testitem);
            }

            //测数完成。如果是点测试，分析数据啊，就是测量标准判断啊
            if(eNUMMESSTYLE == ENUMMESSTYLE._01_POINT)
            {
                InfoData infoData = Project.lstInfos.FirstOrDefault(p => p.id == id);
                if(infoData != null)
                {
                    //这里只能回调啊，由ResutView类那边去计算啊
                    //把InfoData，传递过去
                    if (infoData.Isxchk|| infoData.Isychk || infoData.IsLchk||infoData.IsBalancechk)
                    {
                        //设置了测量标准啊，需要掉用resutView去检测啊
                        //调 Update_Color 这个函数啊
                        if(RunPointTestStd != null)
                        {
                            RunPointTestStd(infoData);
                        }
                    }
                }
                Project.Results.save_point_statics();
            }
        }

        public bool ProcessPointSingle()
        {
            TestMachine ts = Project.testMachine;           
            if (ts.IsOpen==false)
            {
               ts.Init();
            }
            IData str = ts.MeasureLxy();
            if (str != null)
            {
                str.CoordX = str.X;
                str.CoordY = str.Y;
                str.CoordZ = str.Z;
                str.CoordU = str.u;
                str.CoordV = str.v;
                IData lab = ColorService.ToLab(str.X, str.Y, str.Z);
                str.Lcolor = lab.Lcolor;
                str.Acolor = lab.Acolor;
                str.Bcolor = lab.Bcolor;
                //string[] res = ParseLxy(str);
                AddSingleResult(str, "");
                return true;
            }
            return false;
        }

        public bool ProcessSpectrumSingle()
        {
            TestMachine ts = Project.testMachine;           
            if (ts.IsOpen == false)
            {
                ts.Init();
            }
            IData str = ts.MeasureSpectrum();
            if (str != null)
            {
                str.CoordX = str.X;
                str.CoordY = str.Y;
                str.CoordZ = str.Z;
                str.CoordU = str.u;
                str.CoordV = str.v;
                IData lab = ColorService.ToLab(str.X, str.Y, str.Z);
                str.Lcolor = lab.Lcolor;
                str.Acolor = lab.Acolor;
                str.Bcolor = lab.Bcolor;
                //string[] res = ParseLxy(str);
                SpectrumResults(str, "");
                return true;
            }
            return false;
        }

    }
}
