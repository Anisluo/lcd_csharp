//设置三维坐标信息系统
//设置三维分度坐标
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LCD.Ctrl;
using LCD.Data;
using LCD.dataBase;
using LCD.View;
using static LCD.Ctrl.MovCtrl;

//设置位置坐标系统
namespace LCD
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public bool Flag { get; set; } = false;
        private EnumMoveSpeed MovSpeed = EnumMoveSpeed.SLOW;
        private EnumMoveSpeed RotateSpeed = EnumMoveSpeed.SLOW;
        View.DataTemplateView datatemp = null;
        PointInfo PtInfo = new PointInfo();

        //Action action=new Action(Messagebox);
        //Action<double,double> action=new Action<double, double>(Messagebox);
       // Action<string> ShowError=new Action<string>(ShowErrors);

        private bool Sta = false;

        private bool IszERO = false;

        private bool Starterror1 = true;
        private bool Starterror2 = true;
        private Thread tesThread = null;
        DateTime loginTime=DateTime.Now;
        private ResutView Results = null;
        private ResutView spectrumResults = null;
        private ResutView warmupResult = null;
        private ResutView PowerResult = null;
        public double dltx = 0, dlty = 0, dltz = 0, dltw = 0, dltq = 0, dltm = 0;
        public MainWindow()
        {
            InitializeComponent();

            Database.Open();

            Enabled(false);

            #region 测试

            //user_id user = new user_id();
            //int A = user.Insert(new UserIdMode()
            //{
            //    BarCode = "202205210002",
            //    CreationTime = DateTime.Now.ToString()
            //});

            //ProjectMode projectMode=new ProjectMode();

            //projectMode.Insert(new ProjectModeClass()
            //{
            //    ModeType = 3,
            //    projectName = "1_POINT",
            //    UserID = A
            //});


            //user.RederList();




            #endregion
            //Project.V110 = new View.V110();
            //cam.Content = Project.V110;


            Stopwatch sw = Stopwatch.StartNew();
            //结果视图
            Results = new View.ResutView(ENUMMESSTYLE._01_POINT);
            Project.Results = Results;//new View.ResutView(ENUMMESSTYLE._01_POINT);
            Result.Content = Project.Results;


            //光谱结果

            //SpectrumResults SpectrumResults
            spectrumResults = new View.ResutView(ENUMMESSTYLE._03_SPECTRUM);
            Project.SpectrumResults = spectrumResults;
            SpectrumResults.Content = Project.SpectrumResults;

            //warmup

            warmupResult= new View.ResutView(ENUMMESSTYLE._07_warmup);
            Project.warmupResult = warmupResult;
            warmup.Content = Project.warmupResult;


            PowerResult = new ResutView(ENUMMESSTYLE.Power);
            Project.PowerResult= PowerResult;
            Power.Content = Project.PowerResult;

            //工作视图
            View.WortItemView wt = new View.WortItemView();
            workItem.Content = wt;



            //设备视图
            Project.deviceview = new View.DeviceView();
            device.Content = Project.deviceview;

          

            //设备视图
            var ecomv = new View.EComView();
            ecom.Content = ecomv;

            //波形视图
            View.WaveView wave = new View.WaveView();
            Waves.Content = wave;

            //全局视图
            View.StaticView stat = new View.StaticView();
            Statics.Content = stat;
            Project.myrichtextbox = mylog;

            var t1 = sw.Elapsed.TotalSeconds;
            //数据视图
            datatemp = new View.DataTemplateView();
            workData.Content = datatemp;

            Project.Init("Template.xml");//初始化参数
            Project.WriteLog("初始化完成，进入测试界面");
            Project.WriteLog("************************");
            Project.WriteLog("检查设备连接");

            AddAxiesRoute();//添加运动轴路由事件

            mypanel.DataContext = PtInfo;
            mvctrl = MovCtrl.GetInstance();

            var t2 = sw.Elapsed.TotalSeconds;
            //心跳函数
            //System.Timers.Timer timer = new System.Timers.Timer();//声明timer对象
            //timer.Interval = 500;//100ms刷新一次  150
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            //timer.Start();

            Thread thread1=new Thread(Timer_Elapsed);
            thread1.SetApartmentState(ApartmentState.STA);
            thread1.IsBackground = true;
            thread1.Start();

            //System.Timers.Timer timer1 = new System.Timers.Timer();//声明timer对象
            //timer1.Interval = 500;//100ms刷新一次  150
            //timer1.Elapsed += Timer1_Elapsed; ;
            //timer1.Start();

            Thread th=new Thread(Timer1_Elapsed);
            th.IsBackground = true;
            th.Start();


            //低速选择
            SpeelLow_1.IsChecked = true;
            SpeedLow_2.IsChecked = true;

            //ProcessCtrl pctrl = ProcessCtrl.GetInstance();
            pctrl = ProcessCtrl.GetInstance();
            pctrl.AddSingleResult += new ProcessCtrl.AddSingleResultDelegate(Project.Results.AddSingleData);
            pctrl.SpectrumResults += new ProcessCtrl.AddSingleResultDelegate(Project.SpectrumResults.AddSingleData);
            pctrl.warmupResult += new ProcessCtrl.AddSingleResultDelegate(Project.warmupResult.AddSingleData);

            pctrl.PowerResult+=new ProcessCtrl.SingleResultDelegate(Project.PowerResult.SingleData);


            pctrl.InitResult += new ProcessCtrl.InitResultDelegate(Project.Results.Init);
            pctrl.InitDataTemplate += new ProcessCtrl.InitDataTemplateDelegate(datatemp.Init);
            pctrl.GetTable += new ProcessCtrl.GetTableDelegate(Project.Results.GetTale);//获取data数据
            pctrl.ShowIndex += new ProcessCtrl.ShowIndexDelegate(datatemp.ShowIndex);
            
           
            pctrl.UpDataUi += Pctrl_UpDataUi;

            var t3 = sw.Elapsed.TotalSeconds;

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
                Project.testMachine = new Ctrl.DemoMachine();
            }
            Project.testMachine.Init();


            switch (Project.cfg.PGType)
            {
                case ENUMPG.SoftPG:

                    break;
                case ENUMPG.OtherPG:
                    Project.PG = new PG(Project.cfg.otherPG.CommunicationType);
                    int A = Project.PG.init(Project.cfg.otherPG.SDevice.ip);
                    if (A == 1)
                    {
                        Project.WriteLog("PG连接成功");
                        try
                        {
                            String Info = Project.PG.getDeviceInformaction();
                            Project.WriteLog(Info);

                            Project.PG.getDevicePatternList();


                            Project.PGDebug = new View.PGDebug();
                            PG.Content = Project.PGDebug;
                            //bool bo= Project.PG.changePattern(Project.PG.PatternList.ItemStrings[2].name);

                            //if (bo==true)
                            //{
                            //    Project.WriteLog("图片切换成功");
                            //}
                            //else
                            //{
                            //    Project.WriteLog("图片切换失败");
                            //}
                            //bo = Project.PG.colorControl(100, 200, 150);
                            //if (bo == true)
                            //{
                            //    Project.WriteLog("RGB切换成功");
                            //}
                            //else
                            //{
                            //    Project.WriteLog("RGB切换失败");
                            //}

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else
                    {
                        Project.WriteLog("PG连接失败");
                    }
                    break;
                case ENUMPG.NoPG:

                    break;
                default:
                    break;
            }


          

            if (Project.cfg.power.Enabled == true)
            {
                Project.power = new Ctrl.Power();
                Project.power.Init();
            }

            //相机视图
            if (Project.cfg.Camer == 0)
            {
                Project.cam = new View.CamView();
                cam.Content = Project.cam;
            }
            else
            {
                Project.V110 = new View.V110();
                cam.Content = Project.V110;
            }

             

            Zero();

            List<string> ShowInfo = null;
            ENUMMESSTYLE ShowTestType = ENUMMESSTYLE._01_POINT;
            string TempName = "";
            for (int i = 0; i < Project.lstInfos.Count; i++)
            {
                if (Project.lstInfos[i].IsSelected == true)
                {
                    ShowInfo = Project.lstInfos[i].lstdata;
                    ShowTestType = Project.lstInfos[i].MESTYPE;
                    TempName = Project.lstInfos[i].Name;
                    break;
                }

            }

            datatemp.Init(ShowInfo, ShowTestType, TempName);
        }



        private void Pctrl_UpDataUi(object obj)//增加Tab需要更改
        {
            ENUMMESSTYLE eNUMMESSTYLE = (ENUMMESSTYLE)obj;

            if (eNUMMESSTYLE == ENUMMESSTYLE._01_POINT)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.ResTab.SelectedIndex = 0;
                }));
               
            }
            else if (eNUMMESSTYLE == ENUMMESSTYLE._03_SPECTRUM)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.ResTab.SelectedIndex = 1;
                }));
            }
            else if (eNUMMESSTYLE == ENUMMESSTYLE._07_warmup)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.ResTab.SelectedIndex = 2;
                }));
            }
            else if (eNUMMESSTYLE == ENUMMESSTYLE.Power)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.ResTab.SelectedIndex = 3;
                }));
            }
        }

        private ProcessCtrl pctrl = null;
        private MovCtrl mvctrl = null;

        //速度1设置
        private void OnBnClickedSpeedLow_1(object sender, RoutedEventArgs e)
        {
            SpeelLow_1.IsChecked = true;
            SpeedMedium_1.IsChecked = false;
            SpeedHigh_1.IsChecked = false;
            MovSpeed = EnumMoveSpeed.SLOW;
        }

        //速度1设置
        private void OnBnClickedSpeedMedium_1(object sender, RoutedEventArgs e)
        {
            SpeedMedium_1.IsChecked = true;
            SpeelLow_1.IsChecked = false;
            SpeedHigh_1.IsChecked = false;
            MovSpeed = EnumMoveSpeed.MEDIUM;
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedClear(object sender, RoutedEventArgs e)
        {
            Project.Results.Clear();
            Project.WriteLog("清空数据");
        }

        /// <summary>
        /// 速度1设置高速
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedSpeedHigh_1(object sender, RoutedEventArgs e)
        {
            SpeedMedium_1.IsChecked = false;
            SpeelLow_1.IsChecked = false;
            SpeedHigh_1.IsChecked = true;
            MovSpeed = EnumMoveSpeed.HIGH;
        }

        /// <summary>
        /// 速度2设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedSpeedLow_2(object sender, RoutedEventArgs e)
        {
            SpeedLow_2.IsChecked = true;
            SpeedMedium_2.IsChecked = false;
            SpeedHigh_2.IsChecked = false;
            RotateSpeed = EnumMoveSpeed.SLOW;
        }

        //速度1设置
        private void OnBnClickedSpeedMedium_2(object sender, RoutedEventArgs e)
        {
            SpeedMedium_2.IsChecked = true;
            SpeedLow_2.IsChecked = false;
            SpeedHigh_2.IsChecked = false;
            RotateSpeed = EnumMoveSpeed.MEDIUM;
        }

        //速度1设置
        private void OnBnClickedSpeedHigh_2(object sender, RoutedEventArgs e)
        {
            SpeedMedium_2.IsChecked = false;
            SpeedLow_2.IsChecked = false;
            SpeedHigh_2.IsChecked = true;
            RotateSpeed = EnumMoveSpeed.HIGH;
        }

        private bool IsZStop = false;
        private bool MoniterError = false;
        private double zz = 0;
        private double uu = 0;
        double CCC = 0;
        double UMIN = 0;
        double UMAX = 0;
        private bool UIsZStop = false;
        private static void ShowErrors(string str)
        {
            MessageBox.Show(str);
        }
        private static void ShowErrors(string str,String caption)
        {
            
            MessageBox.Show(str, caption, MessageBoxButton.OK,MessageBoxImage.Information);
        }
        private void Timer_Elapsed()//object sender, System.Timers.ElapsedEventArgs e
        {
            
            while (true)
            {
                //action.Invoke(100, 100);

             
                this.Dispatcher?.Invoke(new Action(() =>
                {
                    if ((DateTime.Now - loginTime).Minutes > 5)
                    {
                        Enabled(false);
                    }
                    if (mvctrl != null)
                    {
                        double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
                        mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
                        Data2UI(dx, dy, dz, du, dv, dball);
                        int xerror = 10, yerror = 10, zerror = 10, uerror = 10, verror = 10, ballerror = 10, TestSignal1error = 10, TestSignal2error = 10;
                        mvctrl.UpdataIO(ref xerror, ref yerror, ref zerror, ref uerror, ref verror, ref ballerror, ref TestSignal1error, ref TestSignal2error);
                        //Console.WriteLine($"【{DateTime.Now}】：【{xerror}】【{yerror}】【{zerror}】【{uerror}】【{verror}】【{ballerror}】【{TestSignal1error}】【{TestSignal2error}】");
                        if (xerror == 0)
                        {
                            
                            x.Text = "连接成功";
                            x.Foreground = new SolidColorBrush(Colors.Green); //"DarkRed"; // "DarkRed"
                        }
                        else
                        {
                            if (MoniterError == false)
                            {
                                
                                ////MessageBox.Show("电机报警，请检查");
                                //ShowError.BeginInvoke("电机报警，请检查",null,null);

                                if (Project.cfg.ax_x.AlarmEnable)
                                {
                                    MoniterError = true;
                                    this.Dispatcher.BeginInvoke(new Action<string>(ShowErrors), new object[] { "电机报警，请检查" });
                                }
                            }

                            x.Text = "未连接";
                            x.Foreground = new SolidColorBrush(Colors.DarkRed);
                        }
                        if (yerror == 0)
                        {
                           // MoniterError = false;
                            y.Text = "连接成功";
                            y.Foreground = new SolidColorBrush(Colors.Green); //"DarkRed"; // "DarkRed"
                        }
                        else
                        {
                            if (MoniterError == false)
                            {
                                
                                if (Project.cfg.ax_y.AlarmEnable)
                                {
                                    MoniterError = true;
                                    this.Dispatcher.BeginInvoke(new Action<string>(ShowErrors), new object[] { "电机报警，请检查" });
                                }

                            }

                            y.Text = "未连接";
                            y.Foreground = new SolidColorBrush(Colors.DarkRed);
                        }
                        if (zerror == 0)
                        {
                            //MoniterError = false;
                            z.Text = "连接成功";
                            z.Foreground = new SolidColorBrush(Colors.Green); //"DarkRed"; // "DarkRed"
                        }
                        else
                        {
                            if (MoniterError == false)
                            {
                                
                                if (Project.cfg.ax_z.AlarmEnable)
                                {
                                    MoniterError = true;
                                    this.Dispatcher.BeginInvoke(new Action<string>(ShowErrors), new object[] { "电机报警，请检查" });
                                }

                            }

                            z.Text = "未连接";
                            z.Foreground = new SolidColorBrush(Colors.DarkRed);
                        }
                        if (uerror == 0)
                        {

                            //MoniterError = false;
                            u.Text = "连接成功";
                            u.Foreground = new SolidColorBrush(Colors.Green); //"DarkRed"; // "DarkRed"
                        }
                        else
                        {
                            if (MoniterError == false)
                            {
                                
                                if (Project.cfg.ax_u.AlarmEnable)
                                {
                                    MoniterError = true;
                                    this.Dispatcher.BeginInvoke(new Action<string>(ShowErrors), new object[] { "电机报警，请检查" });
                                }

                            }

                            u.Text = "未连接";
                            u.Foreground = new SolidColorBrush(Colors.DarkRed);
                        }
                        if (verror == 0)
                        {
                           // MoniterError = false;
                            v.Text = "连接成功";
                            v.Foreground = new SolidColorBrush(Colors.Green); //"DarkRed"; // "DarkRed"
                        }
                        else
                        {
                            if (MoniterError == false)
                            {
                               
                                if (Project.cfg.ax_v.AlarmEnable)
                                {
                                    MoniterError = true;
                                    this.Dispatcher.BeginInvoke(new Action<string>(ShowErrors), new object[] { "电机报警，请检查" });
                                }

                            }

                            v.Text = "未连接";
                            v.Foreground = new SolidColorBrush(Colors.DarkRed);
                        }
                        if (TestSignal1error == 0)
                        {
                            Starterror1 = false;
                            TestSignal1.Text = "产品轴到位信号1";
                            TestSignal1.Foreground = new SolidColorBrush(Colors.Green); //"DarkRed"; // "DarkRed"
                        }
                        else
                        {
                            Starterror1 = true;
                            TestSignal1.Text = "产品轴到位信号1";
                            TestSignal1.Foreground = new SolidColorBrush(Colors.DarkRed);
                        }
                        if (TestSignal2error == 0)
                        {
                            Starterror2 = false;
                            TestSignal2.Text = "产品轴到位信号2";
                            TestSignal2.Foreground = new SolidColorBrush(Colors.Green); //"DarkRed"; // "DarkRed"
                        }
                        else
                        {
                            Starterror2 = true;
                            TestSignal2.Text = "产品轴到位信号2";
                            TestSignal2.Foreground = new SolidColorBrush(Colors.DarkRed);
                        }


                        if (xerror==0&&yerror==0&&zerror==0&&uerror==0&&verror==0)
                        {
                            MoniterError = false;
                        }

                    }
                    else
                    {
                        ShowWar();
                    }
                }));
                Thread.Sleep(500);
            }
            
        }

        private static void Messagebox(Double DZ,Double DU)
        {
            MessageBox.Show(
                $"U轴在设定安全范围以外【{Project.cfg.USeftMin}<{DU}<{Project.cfg.USeftMax}】Z轴超过设定值【{DZ}>{Project.cfg.ZSeft}】,请检查");
            
        }
        
        private void Timer1_Elapsed()//object sender, System.Timers.ElapsedEventArgs e
        {
            while (true)
            {

                //this.Dispatcher.BeginInvoke(new Action<Double, Double>(Messagebox), new object[] { 100, 100 });
                //action.Invoke(100, 100);
                double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
                mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置


                double aaa = dz - Project.cfg.ZSeft;
                Double UMINU = du - Project.cfg.USeftMin;
                Double UmAX = du - Project.cfg.USeftMax;

                // Console.WriteLine($"[{DateTime.Now}]【UmAX > UMAX：{UmAX > UMAX}】【UMINU < UMIN:{UMINU < UMIN}】");

                if ((du > Project.cfg.USeftMax || du < Project.cfg.USeftMin) && aaa >= CCC && IszERO == true)
                {

                    if ((zz != dz || uu != du))
                    {
                        Sta = true;
                        zz = dz;
                        uu = du;
                        Console.WriteLine($"【{DateTime.Now}】当前X轴【{dx}】当前Y轴【{dy}】当前Z轴【{dz}】当前U轴【{du}】当前V轴【{dv}】");
                        mvctrl.Stop(MovCtrol.X);
                        mvctrl.Stop(MovCtrol.Y);
                        mvctrl.Stop(MovCtrol.Z);
                        mvctrl.Stop(MovCtrol.U);
                        mvctrl.Stop(MovCtrol.V);

                        try
                        {
                            Project.TestFlag = false;
                            tesThread.Abort();
                        }
                        catch (Exception e)
                        {
                            Project.WriteLog(e.Message);
                        }

                        //action.Invoke(du, dz);
                        this.Dispatcher.BeginInvoke(new Action<Double, Double>(Messagebox), new object[] { dz, du });
                        //action.BeginInvoke(du, dz, null, null);

                    }
                    else
                    {
                    }
                    CCC = dz - Project.cfg.ZSeft;
                    UMIN = du - Project.cfg.USeftMin;
                    UMAX = du - Project.cfg.USeftMax;
                }
                else
                {
                    Sta = false;
                    if (dz - Project.cfg.ZSeft < CCC)
                    {
                        // CCC = dz - Project.cfg.ZSeft;
                    }
                    //CCC = dz - Project.cfg.ZSeft;
                    IsZStop = false;
                }


                Thread.Sleep(300);
            }
          
        }

        //运动模式
        private void OnBnClickMoveStyle(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 读取单个数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickReadSingleData(object sender, RoutedEventArgs e)
        {

            Thread thr = new Thread(() =>
            {
                Project.WriteLog("开始单点测试");
                pctrl.ProcessPointSingle();
                Project.WriteLog("单点测试完成");
            })
            { IsBackground = true };
            thr.Start();

        }
        private void onBnClickSpecturm(object sender, RoutedEventArgs e) {
            Thread thr = new Thread(() =>
            {
                Project.WriteLog("开始光谱测试");
                pctrl.ProcessSpectrumSingle();
                Project.WriteLog("光谱测试完成");
            })
            { IsBackground = true };
            thr.Start();

        }



        private void ShowWar()
        {
            XPos.Text = "Error: No Data";
            YPos.Text = "Error: No Data";
            ZPos.Text = "Error: No Data";
            UPos.Text = "Error: No Data";
            VPos.Text = "Error: No Data";
            BallPos.Text = "Error: No Data";
        }


        private void Data2UI(double dx, double dy, double dz, double du, double dv, double dball)
        {
            if (Project.cfg.ax_x==null| Project.cfg.ax_y==null| Project.cfg.ax_z==null| Project.cfg.ax_v==null| Project.cfg.ax_ball == null)
            {
                Project.WriteLog("参数加载失败");
                return;
            }
            if (Project.cfg.ax_x.IsEnable)
                XPos.Text = (dx - Project.Xorg).ToString("0.00") + " mm";
            else { XPos.Text = "----"; }

            if (Project.cfg.ax_y.IsEnable)
                YPos.Text = (dy - Project.Yorg).ToString("0.00") + " mm";
            else { YPos.Text = "----"; }

            if (Project.cfg.ax_z.IsEnable)
                ZPos.Text = (dz - Project.Zorg).ToString("0.00") + " mm";
            else { ZPos.Text = "----"; }

            if (Project.cfg.ax_u.IsEnable)
                UPos.Text = (du - Project.Uorg).ToString("0.00") + " °";
            else { UPos.Text = "----"; }

            if (Project.cfg.ax_v.IsEnable)
                VPos.Text = (dv - Project.Vorg).ToString("0.00") + " °";
            else { VPos.Text = "----"; }

            if (Project.cfg.ax_ball.IsEnable)
                BallPos.Text = (dball - Project.Ballorg).ToString("0.00") + " mm";
            else { BallPos.Text = "----"; }

        }


        /// <summary>
        /// 轴路由事件
        /// </summary>
        private void AddAxiesRoute()
        {
            btnUP.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Up_MouseDown), true);
            btnUP.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Up_MouseUp), true);

            btnLeft.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Left_MouseDown), true);
            btnLeft.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Left_MouseUp), true);

            btnRight.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Right_MouseDown), true);
            btnRight.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Right_MouseUp), true);

            btnDown.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Down_MouseDown), true);
            btnDown.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Down_MouseUp), true);


            btnIn.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(In_MouseDown), true);
            btnIn.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(In_MouseUp), true);

            btnOut.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Out_MouseDown), true);
            btnOut.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Out_MouseUp), true);

            btnULeft.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(URotatLeft_MouseDown), true);
            btnULeft.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(URotatLeft_MouseUp), true);


            btnURight.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(URotatRight_MouseDown), true);
            btnURight.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(URotatRight_MouseUp), true);


            btnVLeft.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(VRotatLeft_MouseDown), true);
            btnVLeft.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(VRotatLeft_MouseUp), true);

            btnVRight.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(VRotatRight_MouseDown), true);
            btnVRight.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(VRotatRight_MouseUp), true);
            /***********************************************************************************************/
        }



        private void OnBnClickedCustomTemplate(object sender,
            RoutedEventArgs e)
        {
            View.CustomTemplate custom = new View.CustomTemplate();
            custom.ShowDialog();
        }

        private void OnBnClickMPC(object sender, RoutedEventArgs e)
        {
            View.MPCView mpc = new View.MPCView();
            mpc.ShowDialog();
        }

        /// <summary>
        /// 手动控制移动窗口进行测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Handle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnBnClickedDevice(object sender, RoutedEventArgs e)
        {
            View.DevicesView dev = new View.DevicesView();
            dev.ShowDialog();
        }

        // ===== SR3A / CS2000 视场角设置（参照 guoxian 分支移植）=====
        private void OnBnClickedFdl(object sender, RoutedEventArgs e)
        {
            View.ViewangleView dev = new View.ViewangleView();
            dev.ShowDialog();
        }

        public static void sr3a_send_fdl_cmd(string cmd)
        {
            if (!(Project.testMachine is Ctrl.SR3A sr3))
            {
                MessageBox.Show("当前仪器不是 SR3A", "提示");
                return;
            }
            if (sr3.IsOpen == false) sr3.Init();
            bool st = sr3.SenFdlCmd(cmd);
            MessageBox.Show(st ? "发送成功" : "发送失败", "提示");
        }

        public static void cs2000_send_cmd(string cmd)
        {
            if (!(Project.testMachine is Ctrl.CS2000 cs))
            {
                MessageBox.Show("当前仪器不是 CS2000", "提示");
                return;
            }
            if (cs.IsOpen == false) cs.Init();
            bool st = cs.SendCmd(cmd);
            MessageBox.Show(st ? "发送成功" : "发送失败", "提示");
        }


        private void OnBnClickedResultFormat(object sender, RoutedEventArgs e)
        {
            View.CritView crt = new View.CritView();
            crt.ShowDialog();
        }

        private void OnBnClicked5PointView(object sender, RoutedEventArgs e)
        {
            View.Point5View pt5 = new View.Point5View();
            pt5.ShowDialog();
        }

        private void OnBnClicked9PointView(object sender, RoutedEventArgs e)
        {
            View.Point5View pt9 = new View.Point5View();
            pt9.ShowDialog();
        }


        private void OnBnClicked13PointView(object sender, RoutedEventArgs e)
        {
            View.Point13View pt13 = new View.Point13View();
            pt13.ShowDialog();
        }

        private void OnBnClickedResp(object sender, RoutedEventArgs e)
        {
            View.RespView resp = new View.RespView();
            resp.ShowDialog();
        }

        private void OnBnClickedCustomView(object sender, RoutedEventArgs e)
        {
            View.CustomView cstm = new View.CustomView();
            cstm.ShowDialog();

            ////使用当前模板组
            //if(View.CustomView.IsOK)
            //{
            //    Project.WriteLog("模板组内容：");
            //}
        }

        private void OnBnClickedPGView(object sender, RoutedEventArgs e)
        {
            View.PGView pg = new View.PGView();
            pg.ShowDialog();
        }

        //向左
        private void Right_MouseDown(object sender, RoutedEventArgs e)
        {
            if (IszERO == false)
            {
                MessageBox.Show("请先回零电机");
                return;

            }
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveXAxisUp(MovSpeed);
        }

        private void Right_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_x.value);
        }

        //向右
        private void Left_MouseDown(object sender, RoutedEventArgs e)
        {
            if (IszERO == false)
            {
                MessageBox.Show("请先回零电机");
                return;

            }
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveXAxisDown(MovSpeed);
        }

        //停止
        private void Left_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_x.value);
        }

        /// <summary>
        /// Y轴向上运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Up_MouseDown(object sender, RoutedEventArgs e)
        {


            if (IszERO == false)
            {
                MessageBox.Show("请先回零电机");
                return;


            }

            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveYAxiesDown(MovSpeed);

        }

        /// <summary>
        /// Y轴停止运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Up_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            if (Project.cfg.ax_y.IsSecondValue)
            {
                mvctrl.MoveStop2(Project.cfg.ax_y.value, Project.cfg.ax_y.secondvalue);
            }
            else
            {
                mvctrl.MoveStop(Project.cfg.ax_y.value);
            }
        }

        /// <summary>
        /// Y轴向下运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Down_MouseDown(object sender, RoutedEventArgs e)
        {
            if (IszERO == false)
            {
                MessageBox.Show("请先回零电机");
                return;

            }
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveYAxiesUp(MovSpeed);
        }

        /// <summary>
        /// Y轴停止运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Down_MouseUp(object sender, RoutedEventArgs e)
        {
            if (Project.cfg.ax_y.IsSecondValue)
            {
                mvctrl.MoveStop2(Project.cfg.ax_y.value, Project.cfg.ax_y.secondvalue);
            }
            else
            {
                mvctrl.MoveStop(Project.cfg.ax_y.value);
            }
        }

        private void In_MouseDown(object sender, RoutedEventArgs e)
        {

            MovCtrl mvctrl = MovCtrl.GetInstance();
            double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
            mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
            if (IszERO == false)
            {
                MessageBox.Show("请先回零电机");
                return;

            }

           

            mvctrl.MoveZAxiesDown(MovSpeed);
        }

        private void In_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_z.value);
        }
        private void Out_MouseDown(object sender, RoutedEventArgs e)//zzz
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
            mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
            if (IszERO == false)
            {
                MessageBox.Show("请先回零电机");
                return;

            }
            if (Sta == true)
            {
                //action.BeginInvoke(du, dz, null, null);
                this.Dispatcher.BeginInvoke(new Action<Double, Double>(Messagebox), new object[] { dz, du });
                return;
            }
            mvctrl.MoveZAxiesUp(MovSpeed);
        }
        private void Out_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_z.value);
        }

        //M轴左侧鼠标按下
        private void MAxeLeft_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveBallAxiesUp(MovSpeed);
        }

        private void MAxeLeft_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_ball.value);
        }

        //M轴左侧鼠标按下
        private void MAxeRight_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveBallAxiesDown(MovSpeed);
        }

        private void MAxeRight_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_ball.value);
        }

        private void URotatLeft_MouseDown(object sender, RoutedEventArgs e)
        {
            if (IszERO == false)
            {
                MessageBox.Show("请先回零电机");
                return;

            }
            MovCtrl mvctrl = MovCtrl.GetInstance();
            double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
            mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
            if (Sta == true)
            {
                //action.BeginInvoke(du, dz, null, null);
                this.Dispatcher.BeginInvoke(new Action<Double, Double>(Messagebox), new object[] { dz, du });
                return;
            }
            mvctrl.MoveUAxiesDown(RotateSpeed);
        }

        private void URotatLeft_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_u.value);
        }

        private void URotatRight_MouseDown(object sender, RoutedEventArgs e)
        {
            if (IszERO == false)
            {
                MessageBox.Show("请先回零电机");
                return;

            }
            MovCtrl mvctrl = MovCtrl.GetInstance();
            double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
            mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
            if (Sta == true)
            {
                //action.BeginInvoke(du, dz, null, null);
                this.Dispatcher.BeginInvoke(new Action<Double, Double>(Messagebox), new object[] { dz, du });
                return;
            }
            mvctrl.MoveUAxiesUp(RotateSpeed);
        }

        private void URotatRight_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_u.value);
        }

        private void VRotatLeft_MouseDown(object sender, RoutedEventArgs e)
        {
            if (IszERO == false)
            {
                MessageBox.Show("请先回零电机");
                return;

            }
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveVAxiesDown(RotateSpeed);
        }


        private void OnBnClicked25PointView(object sender, RoutedEventArgs e)
        {

        }

        private void OnBnClickedFstStop(object sender, RoutedEventArgs e)
        {
            Project.TestFlag = false;
            Project.WriteLog("急停!");

            Project.testMachine.StopTest();

            Project.FstStop = true;
            MovCtrl mvctrl = MovCtrl.GetInstance();
            //mvctrl.MoveVAxiesDown(RotateSpeed);
            mvctrl.StopAll();
            Project.Stop = true;
            try
            {
                thr1?.Abort();
            }
            catch (Exception exception)
            {
                Project.WriteLog(exception.Message);
            }
            try
            {
                Project.TestFlag = false;
                tesThread?.Abort();//interrupt

            }
            catch (Exception exception)
            {
                Project.WriteLog(exception.Message);
            }
        }

        private void VRotatLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// 设置MouseUp位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VRotatLeft_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_v.value);
        }

        /// <summary>
        /// 设置Left_MouseUp位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VRotatRight_MouseDown(object sender, RoutedEventArgs e)
        {
            if (IszERO == false)
            {
                MessageBox.Show("请先回零电机");
                return;
            }
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveVAxiesUp(RotateSpeed);
        }

        private void VRotatRight_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_v.value);
        }


        private void btnO_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult messageBoxResult= MessageBox.Show("确定设置当前点位为相对原点","",MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes )
            {
                return;
            }

            //设置参考原点
            Thread thr = new Thread(() =>
            {
                Flag = true;
                Project.FstStop = false;
                MovCtrl mvctrl = MovCtrl.GetInstance();
                double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
                mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);
                Project.Xorg = dx;
                Project.Yorg = dy;
                Project.Zorg = dz;
                Project.Uorg = du;
                Project.Vorg = dv;
                Project.Ballorg = dball;
                //Project.SaveConfig("Config.xml");//保存首点坐标
                Project.WriteLog("相对原点设置完成");
                Project.WriteLog($"【X:{Project.Xorg}】【Y;{Project.Yorg}】【Z:{Project.Zorg}】【U:{Project.Uorg}】【V:{Project.Vorg}】【Ball:{Project.Ballorg}】");
            });
            thr.IsBackground = true;
            thr.Start();
        }

        private void History_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            View.HistoryView hst = new View.HistoryView();
            hst.ShowDialog();
        }

        private void ToolBar_MouseDown(object sender, RoutedEventArgs e)
        {
            View.HistoryView hst = new View.HistoryView();
            hst.ShowDialog();
        }

        /// <summary>
        /// 任务执行/暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedStart(object sender, RoutedEventArgs e)
        {
            View.HistoryView hst = new View.HistoryView();
            hst.ShowDialog();
        }

        /// <summary>
        /// 急停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedStop(object sender, RoutedEventArgs e)
        {
            View.HistoryView hst = new View.HistoryView();
            hst.ShowDialog();
        }

        private void OnBnClicked2Center(object sender, RoutedEventArgs e)
        {
            Zero();
        }
        Thread thr1 = null;
        private void Zero()
        {
            MessageBoxResult message = MessageBox.Show("请确保周围安全后点击OK按钮", "电机回零", MessageBoxButton.OKCancel);

            if (message == MessageBoxResult.Cancel)
            {
                return;
            }
            thr1 = new Thread(() =>
                {
                    Project.WriteLog("运动轴初始化开始");
                    MovCtrl mvctrl = MovCtrl.GetInstance();
                    mvctrl.MoveHome();
                    Console.WriteLine($"【x】：{Project.cfg.XCenter}【Y】：{Project.cfg.YCenter}【Z】：{Project.cfg.ZCenter}【u】：{Project.cfg.UCenter}【v】：{Project.cfg.VCenter}【ball】：{Project.cfg.BallCenter}");



                    mvctrl.Move2Points(Project.cfg.XCenter,
                        Project.cfg.YCenter,
                        Project.cfg.ZCenter,
                        Project.cfg.UCenter,
                        Project.cfg.VCenter,
                        Project.cfg.BallCenter, true);

                    //改为回到参考点
                    //屏蔽回中心指令
                    //mvctrl.Move2Points(Project.cfg.XCenter,
                    //    Project.cfg.YCenter,
                    //    Project.cfg.ZCenter,
                    //    Project.cfg.UCenter,
                    //    Project.cfg.VCenter,
                    //    Project.cfg.BallCenter, true);



                    mvctrl.WaitFiveAxeMoveFinish();
                    //double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
                    //mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
                    IszERO = true;
                    Project.WriteLog("运动轴初始化完成");
                })
                { IsBackground = true };
            thr1.Start();
        }

        private void OnBnClickedSelfCheck(object sender, RoutedEventArgs e)
        {
            View.SelfCheck self = new View.SelfCheck();
            self.ShowDialog();
        }


        private void OnBnClickedHistory(object sender, RoutedEventArgs e)
        {
            View.HistoryView hst = new View.HistoryView();
            hst.ShowDialog();
        }

        private void OnBnClickedResponseTemplate(object sende, RoutedEventArgs e)
        {
            View.ResponseView resp = new View.ResponseView();
            resp.ShowDialog();
        }

        /// <summary>
        /// 相机设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickCamera(object sender, RoutedEventArgs e)
        {
            View.CameraDevice camdev = new View.CameraDevice();
            camdev.ShowDialog();
            
        }

        //显示自定义模板组
        private void OnBnClickedSelfTemplate(object sender, RoutedEventArgs e)
        {
            View.CustomView cstm = new View.CustomView();
            cstm.ShowDialog();
            //使用当前模板组
            if (View.CustomView.IsOK)
            {
                Project.WriteLog("------------------");
                Project.WriteLog("模板组内容：");

                for (int i = 0; i < Project.lstInfos.Count; i++)
                {
                    Project.WriteLog(Project.lstInfos[i].Name);
                }
                Project.WriteLog("------------------");
                Project.WriteLog("模板组载入完毕!");
                Project.Results.Init(Project.lstInfos[0].MESTYPE);
                List<string> ShowInfo = null;
                ENUMMESSTYLE ShowTestType = ENUMMESSTYLE._01_POINT;
                string TempName = "";
                for (int i = 0; i < Project.lstInfos.Count; i++)
                {
                    if (Project.lstInfos[i].IsSelected==true)
                    {
                        ShowInfo = Project.lstInfos[i].lstdata;
                        ShowTestType = Project.lstInfos[i].MESTYPE;
                        TempName = Project.lstInfos[i].Name;
                        break;
                    }
                    
                }

                datatemp.Init(ShowInfo, ShowTestType, TempName);

                //InitDataTemplate(Project.lstInfos[0].lstdata, Project.lstInfos[0].MESTYPE, Project.lstInfos[0].Name);//委托到Ui

            }
        }

        private void OnBnClickedStartRun(object sender, RoutedEventArgs e)
        {
            //action.BeginInvoke(100, 100,null,null);
            if (Flag == false)
            {

                MessageBox.Show("请设置相对原点");
                return;

            }
            if (IszERO == false)
            {

                MessageBox.Show("请先回零电机");
                return;

            }
            if (Project.cfg.Signal1Enable&&Project.cfg.Signal2Enable)
            {
                if (Starterror1 == true || Starterror2 == true)
                {
                    MessageBox.Show("产品轴未设置移动到最上方,无法测试");//请检查测试信号
                    return;
                }
            }
            

            if (MoniterError == true)
            {
                MessageBox.Show("请检查电机状态");
                return;
            }
            Project.Results.Clear();//清出表格
            Project.SpectrumResults.Clear();
            Project.warmupResult.Clear();
            Project.PowerResult.Clear();
            Project.FstStop = false;

            //pop_up barCode = new pop_up("","请输入产品ID:");

            //barCode.ShowDialog();

            //string Temp = barCode.Time.Replace(" ","");
            string Temp = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (Temp=="")
            {
                return;
            }

            tesThread = new Thread(() =>
            {
                pctrl.Run(Temp);//开启线程执行测试
            })
            { IsBackground = true };
            tesThread.Start();
            LogIndex.SelectedIndex = 2;
        }

        private void OnBnClickedSwitchUser(object sender, RoutedEventArgs e)
        {
            View.Login login = new View.Login();
            bool Result= (bool)login.ShowDialog();
            if (Result==true)
            {
                Enabled(true);
              
                loginTime=DateTime.Now;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ((View.CamView)(cam.Content)).CloseCam();
            Environment.Exit(0);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            //MenuItem obj= (MenuItem)sender; 

            Results.ShowAction?.Invoke(true);
            //spectrumResults.ShowAction?.Invoke(true);
            //warmupResult.ShowAction?.Invoke(true);
            //Results.ShowAction?.Invoke(true);
            //Results.ShowAction?.Invoke(true);
            //Project.SpectrumResults?.ShowAction?.Invoke(true);
            //Project.warmupResult?.ShowAction?.Invoke(true);
        }

        private void ShowPortion(object sender, RoutedEventArgs e)
        {
            Results.ShowAction?.Invoke(false);
            //spectrumResults.ShowAction?.Invoke(false);
            //warmupResult.ShowAction?.Invoke(false);
            //Results.ShowAction?.Invoke(false);
            //Results.ShowAction?.Invoke(false);
            //Project.SpectrumResults?.ShowAction?.Invoke(false);
            //Project.warmupResult?.ShowAction?.Invoke(false);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void spectrumResult(object sender, RoutedEventArgs e)
        {
            spectrumResults.ShowAction?.Invoke(true);
        }

        private void Enabled(bool Value)
        {
            //TempButton.IsEnabled = Value;
            MPcItemMenuItem.IsEnabled = Value;


        }
        private void spectrumResultFalse(object sender, RoutedEventArgs e)
        {
            spectrumResults.ShowAction?.Invoke(false);
        }

        private void warmupResultOK(object sender, RoutedEventArgs e)
        {
            warmupResult.ShowAction?.Invoke(true);
        }

        private void warmupResultFalse(object sender, RoutedEventArgs e)
        {
            warmupResult.ShowAction?.Invoke(false);
        }


        private void Open_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Project.LoadTempLate(fileDialog.FileName);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Project.Xorg==0)
            {
                MessageBox.Show("当前未设置相对点位，无法复位");
                return;
            }
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.Move2Points(0, 0, 0, 0, 0, 0, false);
        }

        private void MenuItem_PowerClick(object sender, RoutedEventArgs e)
        {
            View.Power dev = new View.Power();
            dev.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Project.cam.CloseCam();
        }

        private void xxx_TextChanged(object sender, TextChangedEventArgs e)
        {
            xxx.ScrollToEnd();
        }

        //private void mylog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    xxx.ScrollToEnd();
        //}
        private double dX1, dY1, dX2, dY2;
        /// <summary>
        /// 参考
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
            mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
            dX1 = dx;//记录dx1位置
            dY1 = dy;//记录dy1位置
            Project.WriteLog("已设置参照点X1: (" + dX1.ToString("f2") + "," + dY1.ToString("f2") + ")");
        }
        /// <summary>
        /// 修正
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
            mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置

            dX2 = dx;
            dY2 = dy;

            


            double dltX = (dX2 - dX1);
            double dltY = (dY2 - dY1);

            Console.WriteLine($"△dltX{dltX}");
            Console.WriteLine($"△dltY{dltY}");

            double Tran = dltY / dltX;
            Console.WriteLine($"△Y/△X{Tran}");

            if (dltX != 0.0)
            {
                dltw = Math.Atan(Tran) / Math.PI * 180;
                Console.WriteLine($"修正角度{dltw}");

            }
            else
            {
                dltw = 0;
            }


            
            mvctrl.Move2Points(
                dx- (double)Project.Xorg, 
                dy - (double)Project.Yorg, 
                dz - (double)Project.Zorg,
                du - (double)Project.Uorg,
                dv+dltw - (double)Project.Vorg, 
                dball - (double)Project.Ballorg, false);
        }

        private void MenuItem_OnClickup(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
            mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
            double angle = Project.cfg.Angle;
            //走绝对定位
            mvctrl.Move2Points(
                dx - (double)Project.Xorg,
                dy - (double)Project.Yorg,
                dz - (double)Project.Zorg,
                angle - (double)Project.Uorg,
                 dv ,
                dball - (double)Project.Ballorg, false);
        }
    }


    //点位坐标
    public class PointInfo
    {
        public double XPos { get; set; }
        public double YPos { get; set; }

        public double ZPos { get; set; }
        public double UPos { get; set; }
        public double VPos { get; set; }
        public double BallPos { get; set; }
    }
}
