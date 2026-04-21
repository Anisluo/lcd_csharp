/********************/
using LCD.Core.Abstractions;
using LCD.Core.Runtime;
using LCD.Ctrl;
using LCD.Data;
using LCD.dataBase;
using LCD.Managers;
using LCD.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using VisionCore;
using static LCD.Ctrl.MovCtrl;

namespace LCD
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class Project
    {
        /// <summary>
        /// 急停按钮。实际存储在 LCD.Core.Runtime.MotionRuntime，保留这里作为转发入口以兼容历史调用。
        /// </summary>
        public static bool Stop
        {
            get => MotionRuntime.Stop;
            set => MotionRuntime.Stop = value;
        }

        public static double Xorg { get => MotionRuntime.Xorg; set => MotionRuntime.Xorg = value; }
        public static double Yorg { get => MotionRuntime.Yorg; set => MotionRuntime.Yorg = value; }
        public static double Zorg { get => MotionRuntime.Zorg; set => MotionRuntime.Zorg = value; }
        public static double Uorg { get => MotionRuntime.Uorg; set => MotionRuntime.Uorg = value; }
        public static double Vorg { get => MotionRuntime.Vorg; set => MotionRuntime.Vorg = value; }
        public static double Ballorg { get => MotionRuntime.Ballorg; set => MotionRuntime.Ballorg = value; }

        public static TestMachine testMachine { get { return UiRegistry.DeviceView == null ? null : UiRegistry.DeviceView.TestDevice; } set { if (UiRegistry.DeviceView != null) UiRegistry.DeviceView.TestDevice = value; } }

        public static IPatternGenerator PG { get; set; }
        public static LCD.Ctrl.Power power { get; set; }

        internal static DeviceView deviceview
        {
            get => UiRegistry.DeviceView;
            set => UiRegistry.DeviceView = value;
        }

        public static PGDebug PGDebug
        {
            get => UiRegistry.PGDebug;
            set => UiRegistry.PGDebug = value;
        }

        public static CamView cam
        {
            get => UiRegistry.Cam;
            set => UiRegistry.Cam = value;
        }

        public static V110 V110
        {
            get => UiRegistry.V110;
            set => UiRegistry.V110 = value;
        }

        public static void ShowMessage(LogLevel logLevel, string _Str_)
        {
            if (logLevel == LogLevel.Debug && !Project.cfg.LogCfg.ShowDebug)
            {
                return;
            }

            if (logLevel == LogLevel.Comm && !Project.cfg.LogCfg.ShowComm)
            {
                return;
            }

            WriteLog(_Str_, logLevel);

            if (logLevel == LogLevel.Warn)
                System.Windows.MessageBox.Show(_Str_);
        }

        /// <summary>
        /// 初始化格式
        /// </summary>
        public static SortedDictionary<string, bool> resultFormat = new SortedDictionary<string, bool>();

        /// <summary>
        /// 全局急停信号
        /// </summary>
        public static bool FstStop
        {
            get => MotionRuntime.FstStop;
            set => MotionRuntime.FstStop = value;
        }
        public static bool TstPause { get; set; } = false;//暂停测试

        public static List<ResultData> lstDatas = new List<ResultData>();//测试结果


        

        public static bool TestFlag = false;
        /// <summary>
        /// 初始化结果数据格式
        /// </summary>
        private void InitResultFormt()
        {
            resultFormat.Add("X(坐标:mm)", true);
            resultFormat.Add("Y(坐标:mm)", true);
            resultFormat.Add("Z(坐标:mm)", true);
            resultFormat.Add("U(坐标:mm)", true);
            resultFormat.Add("V(坐标:mm)", true);
            resultFormat.Add("ball(坐标:mm)", true);
            resultFormat.Add("L(亮度)", true);
            resultFormat.Add("cx(色坐标)", true);
            resultFormat.Add("cy(色坐标)", true);
            resultFormat.Add("Tc(色温)", true);
            resultFormat.Add("u'", true);
            resultFormat.Add("v'", true);
            resultFormat.Add("X(三刺激值)", true);
            resultFormat.Add("Y(三刺激值)", true);
            resultFormat.Add("Z(三刺激值)", true);
            resultFormat.Add("Voltage(电压:V)", true);
            resultFormat.Add("波长(光谱)", true);
        }

        /// <summary>
        /// 保存结果格式
        /// </summary>
        public static void SaveResultFormat()
        {

        }

        /// <summary>
        /// 富文本格式
        /// </summary>
        public static System.Windows.Documents.FlowDocument myrichtextbox = null;

        /// <summary>
        /// 配置文件
        /// </summary>
        public static Config cfg { get; set; } = new Config();

        public static PointF PtCenter { get; set; } = new PointF(0, 0, 0);

        public static List<UserIdMode> listBarCode
        {
            get => SessionState.ListBarCode;
            set => SessionState.ListBarCode = value;
        }

        public static List<TestDataMode> TestDataModes
        {
            get => SessionState.TestDataModes;
            set => SessionState.TestDataModes = value;
        }

        public static List<SpectrumDataMode> ListSpectrumData
        {
            get => SessionState.ListSpectrumData;
            set => SessionState.ListSpectrumData = value;
        }

        public static List<UserIdMode> SaveTestData
        {
            get => SessionState.SaveTestData;
            set => SessionState.SaveTestData = value;
        }

        public static int BarCodeID
        {
            get => SessionState.BarCodeID;
            set => SessionState.BarCodeID = value;
        }

        public static int ProjectID
        {
            get => SessionState.ProjectID;
            set => SessionState.ProjectID = value;
        }
        /// <summary>
        /// 测试模板信息
        /// </summary>
        public static List<InfoData> lstInfos = new List<InfoData>();

        //添加日志，进行测试
        public static void WriteLog(string message, LogLevel level = LogLevel.Info)
        {
            myrichtextbox?.Dispatcher.Invoke(() =>
                           {
                               Paragraph para = new Paragraph();
                               para.Foreground = Converters.LogLevelToColor.Convert(level);
                               para.Inlines.Add(new Run(DateTime.Now.ToString()));
                               para.Inlines.Add(new Run(">> "));
                               para.Inlines.Add(new Run(message));
                               Console.WriteLine($"{DateTime.Now}>>{message}");
                               myrichtextbox.Blocks.Add(para);
                           });
            LogHelper.Instance.Write(message);
        }

        /// <summary>
        /// 结果视图。以下 6 个 ResutView 引用存储在 UiRegistry，以下为转发属性。
        /// </summary>
        public static ResutView Results
        {
            get => UiRegistry.Results;
            set => UiRegistry.Results = value;
        }
        public static ResutView SpectrumResults
        {
            get => UiRegistry.SpectrumResults;
            set => UiRegistry.SpectrumResults = value;
        }
        public static ResutView ResponseResults
        {
            get => UiRegistry.ResponseResults;
            set => UiRegistry.ResponseResults = value;
        }
        public static ResutView CrossTalkResults
        {
            get => UiRegistry.CrossTalkResults;
            set => UiRegistry.CrossTalkResults = value;
        }
        public static ResutView warmupResult
        {
            get => UiRegistry.WarmupResult;
            set => UiRegistry.WarmupResult = value;
        }
        public static ResutView PowerResult
        {
            get => UiRegistry.PowerResult;
            set => UiRegistry.PowerResult = value;
        }

        /// <summary>
        /// 项目初始化
        /// </summary>
        public static void Init(string name)
        {
            Log.OnShowString += ShowMessage;
            InitConfig();
            LoadTempLate(name);//"Template.xml"
            InitDevice();
            InitTemplate();
            Manager_Plugins.InitPlugin();
        }

        /// <summary>
        /// 初始化测试设备
        /// </summary>
        private static void InitDevice()
        {
            Config config = Project.cfg;

            switch (Project.cfg.TESTMACHINE)//判断测试设备-考虑将此参数替换到模板
            {
                case ENUMMACHINE.BMA7: Project.testMachine = BM7A.GetInstance(); break;
                case ENUMMACHINE.CS2000: Project.testMachine = CS2000.GetInstance(); break;
                default: Project.testMachine = BM7A.GetInstance(); break;
            }
            if (testMachine == null) 
            { 
                Project.WriteLog4(LogLevel.Error, "testMachine为空");
                return;
            }
            testMachine.AutoCheck();
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        public static void InitConfig()
        {
            try
            {
                StreamReader sr = File.OpenText("Config.xml");
                string xml = sr.ReadToEnd();
                sr.Close();
                Project.cfg = XmlUtil.Deserialize(typeof(Config), xml) as Config;
                Project.PtCenter.X = Project.cfg.XCenter;
                Project.PtCenter.Y = Project.cfg.YCenter;
                Project.PtCenter.Z = Project.cfg.ZCenter;
                Project.PtCenter.U = Project.cfg.UCenter;
                Project.PtCenter.V = Project.cfg.VCenter;
            }
            catch (Exception)
            {
                Project.cfg=new Config();
            }
            //保存测试结果
            

        }


        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="fname"></param>
        public static void SaveConfig(string fname)
        {
            try
            {
                Config cfg = Project.cfg;
                string xml = XmlUtil.Serializer(typeof(Config), cfg);
                StreamWriter sw = File.CreateText(fname);
                sw.Write(xml);
                sw.Close();
            }
            catch (Exception e)
            {
                Log.Fatal(e);
            }
        }

        public static void LoadTempLate(String Name)
        {
            try
            {
                StreamReader sr = File.OpenText(Name);//"Template.xml"
                string xml = sr.ReadToEnd();
                sr.Close();
                Project.lstInfos = XmlUtil.Deserialize(typeof(List<InfoData>), xml) as List<InfoData>;
            }
            catch (Exception e)
            {
                Project.WriteLog(e.Message);
                MessageBox.Show(""+e.Message);
            }
            
        }

        /// <summary>
        /// 保存模板  
        /// </summary>
        public static void SaveTemplate(String Name)//lstInfos
        {
            try
            {
                string Xml = XmlUtil.Serializer(typeof(List<InfoData>),Project.lstInfos);
                StreamWriter sw = File.CreateText(Name);
                sw.Write(Xml);
                sw.Close();
            }
            catch (Exception e)
            {
                Log.Fatal(e);
            }
        }


        /// <summary>
        /// 保存模板组合
        /// </summary>
        /// <param name="lstInfos"></param>
        /// <param name="fname"></param>
        public static void SaveTemplateGroup(List<InfoData> lstInfos, string fname)
        {
            try
            {
                string xml = XmlUtil.Serializer(typeof(List<InfoData>), lstInfos);
                StreamWriter sw = File.CreateText(fname);
                sw.Write(xml);
                sw.Close();
            }
            catch (Exception e)
            {
                Log.Fatal(e);
            }
        }

        //读取测试模板
        private static void InitTemplate()
        {

        }

        /// <summary>
        /// 打印Log
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLog4(LogLevel Action, string message)
        {
            Log.Print(Action, message);
        }
    }

   


    public class CritInfo
    {
        public string Name;
        public bool IsCHK;
    }


    public class Config
    {


        /// <summary>
        /// 到位信号1
        /// </summary>
        public bool Signal1Enable;
        /// <summary>
        /// 到位信号2
        /// </summary>
        public bool Signal2Enable;
        /// <summary>
        /// 下料角度
        /// </summary>
        public double Angle;
        public int LightScreenSignal;
        public bool LightScreenAlarmEnable;
        public Axies ax_x;//运动轴1
        public Axies ax_y;//运动轴2
        public Axies ax_z;//运动轴3
        public Axies ax_u;//运动轴4
        public Axies ax_v;//运动轴5
        public Axies ax_ball;//运动轴6
        public uint AxiesDoneDelay; //运动轴运行结束后的延时时间
        public int EQType;
        public int ZSeft;
        public int USeftMin;
        public int USeftMax;
        public double UProductH;
        public double UMaxAngle;
        public double VMaxAngle;
        public int UReverse;
        public int VReverse;
        public bool IsFlipped => EQType == 5; // 仪器固定旋转式

        public Machine machine;//机械参数

        public ComDevice BM7A;
        public ComDevice CS2000;
        public ComDevice USB2000;
        public ComDevice SR3A;
        public ComDevice SR5A;
		public ComDevice MS01;
        public ComDevice Demo;
        public IPDevice softPG =new IPDevice();
        public OtherPG otherPG =new OtherPG();
        public Power power =new Power();
        public double XCenter;
        public double YCenter;
        public double ZCenter;
        public double UCenter;
        public double VCenter;

        public double xorg;//x参考点
        public double yorg;
        public double zorg;
        public double uorg;
        public double vorg;



        public double BallCenter;

        public uint ExposureTime;

        public int Camer;

        public double CamLeft;
        public double CamTop;

        public int Row;
        public int Colu1;

       

        //public SortedDictionary<string, bool> lstcrts;
        public List<CritInfo> lstCrts { get; set; }
        //海康威视相机
        public CameraDevic Hikvision { get; set; }
        //v110相机
        public CameraDevic V110 { get; set; }

        public DataViewHeaderConfig DataVisibility { get; set; } = new DataViewHeaderConfig();

        public ENUMPG PGType { get; set; }

        public ENUMMACHINE TESTMACHINE { get; set; }

        public EnumMoveSpeed movSpeed { get; set; }
        public EnumMoveSpeed movSpeed_Y { get; set; }
        public EnumMoveSpeed movSpeed_Z { get; set; }
        public EnumMoveSpeed movSpeed_U { get; set; }
        public EnumMoveSpeed movSpeed_V { get; set; }
        public EnumMoveSpeed movSpeed_Ball { get; set; }

        public CamTypeEnum CamType { get; set; }
        public int CamIndex { get; set; }
        public int CamRotation { get; set; }
        public ComDevice Comm { get; set; }//通讯设置
        public LogConfigModel LogCfg { get; set; } = new LogConfigModel();
        public int Lang { get; set; } //语言设置,默认是0表示中文，其他英文
        public bool ShowLab { get; set; }
    }


    public enum CamTypeEnum
    {
        HikVision,
        V110,
        SVS
    }
    public enum ENUMPG
    {
        SoftPG,
        OtherPG,
        NoPG
    }
    //串口设备用于
    public class ComDevice
    {
        public double startDelay;
        public double mesDelay;
        public double resetDelay;
        public double lum;
        public double cx;
        public double cy;
        public string comName;
        public int bardRate;
        public string bardRateText;
        public int dataBit;
        public string dataBitText;
        public int stopBit;
        public string stopBitText;
        public int parity;
        public string parityText;
    }

    public class OtherPG
    {
        /// <summary>
        /// 通讯类型
        /// </summary>
        public int CommunicationType=0;

        public IPDevice SDevice { get; set; }=new IPDevice();
        public SerialPortDevice serialPort  { get; set; }=new SerialPortDevice();
    }

    public class IPDevice
    {
        public string ip;
        public string port;

    }

    public class SerialPortDevice
    {
        public int ComName { get; set; }
        /// <summary>
        /// com号
        /// </summary>
        public string comName { get; set; }
       
        public int bardRate { get; set; }
        public string  BarRate { get; set; }
        public int dataBit { get; set; }
        public string DataBit { get; set; }
        public int stopBit { get; set; }
        public string StopBit { get; set; }
        public int parity { get; set; }
        public string Parity { get; set; }
    }
    public class CameraDevic
    {
        public string ip;
        public int port;
        public double expTime;
    }
    // Axies + AXiesName 已迁至 LCD.Core/Data/Axies.cs（保留 LCD.Data 命名空间）。
    public enum ENUMMESSTYLE//
    {
        _01_POINT,
        _02_RESPONSE,
        _03_SPECTRUM,
        _04_FLICKER,
        _05_CROSSTALK,
        _06_ACR,
        _07_warmup,
        TCO,
        None,
        Power
    }

    //机械参数
    public class Machine
    {
        public double h0;//

    }

    public class Power
    {
        public bool Enabled { get; set; }
        public string PowerType { get; set; } //电源型号
        public string PowerSerialName { get; set; }//电源控制的串口名称
        public bool EnablePowerControl { get; set; }//是否启动电源控制
        public SerialPortDevice Bus { get; set; } = new SerialPortDevice();
    }

    public class ResultData
    {
        public string Name;
        public DataTable table;
    }
}
