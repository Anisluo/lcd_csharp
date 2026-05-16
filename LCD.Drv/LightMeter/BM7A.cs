using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using LCD.Data;
using System.ComponentModel;
using System.Diagnostics;
using VisionCore;

namespace LCD.Ctrl
{
    /// <summary>
    /// 串口通信
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    [Category("仪器"), Description("BM7A"), DisplayName("BM7A")]
    public class BM7A : TestMachine//serialPort.SendStr("ST" + sLineBreak);
    {
        private static BM7A m_bm7a;
        public IData m_Result { get; set; }
        public override string sLineBreak { get; set; } = "\r\n";//结束时使用的分割符号
        public string devName;

        private SerialPort SerialPort;
        public override bool IsOpen { get => SerialPort == null ? false : SerialPort.IsOpen; }

        private bool send_cmd(string cmd)
        {
            try
            {
                SerialPort.Write(cmd);
                //LogHelper.Instance.Write("光谱串口已经发送：" + cmd);
            }
            catch (Exception ex)
            {
                Log.Error("Bm7a串口发送：" + cmd + "，异常：" + ex.Message);
                return false;
            }
            Log.Info("Bm7a已经发送命令：" + cmd);
            return true;
        }

        /// <summary>
        /// 等待数据读
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private List<byte> waitQbuffer(Queue<byte[]> buffer)
        {
            List<byte> lsttmpbt = new List<byte>();//list列表缓存数据
            int curt = 0;
            int limt = 500;
            while (true)
            {
                if (buffer.Count != 0)
                { lsttmpbt.AddRange(buffer.Dequeue()); }
                else
                {
                    if (curt >= limt) return lsttmpbt;
                    curt += 50;
                    Thread.Sleep(50);
                }
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            if (SerialPort == null)
            {
                SerialPort = new SerialPort();
            }
            if (SerialPort.IsOpen) { SerialPort.Close(); }

            //serialPort.ReceiveString += ECommunacation_ReceiveString;


            //设置通讯参数（由调用方通过 this.Config 注入）
            if (Config == null)
            {
                Log.Error("BM7A.Init: Config 未设置，无法打开串口");
                return;
            }
            SerialPort.PortName = Config.ComName;
            SerialPort.BaudRate = Config.BaudRate;
            SerialPort.DataBits = Config.DataBits;
            switch (Config.StopBits)
            {
                case 0: SerialPort.StopBits = StopBits.None; break;
                case 1: SerialPort.StopBits = StopBits.One; break;
                case 2: SerialPort.StopBits = StopBits.Two; break;
            }
            switch (Config.Parity)
            {
                case 0: SerialPort.Parity = Parity.None; break;
                case 1: SerialPort.Parity = Parity.Odd; break;
                case 2: SerialPort.Parity = Parity.Even; break;
            }

            Log.Info($"BM7A:[{SerialPort.PortName}][{SerialPort.BaudRate}][{SerialPort.DataBits}][{SerialPort.Parity}][{Config.StopBits}][{Config.Parity}]");

            //SerialPort.DataReceived += SerialPort_DataReceived;
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            //SerialPort.CtsHolding = false;
            SerialPort.DtrEnable = true;
            SerialPort.Handshake = Handshake.RequestToSend;
            SerialPort.WriteTimeout = 30;//设置发送超时
            try
            {
                SerialPort.Open();
            }
            catch (Exception ex)
            {
                Log.Error("Bm7a串口打开失败:" + ex.Message);
                return;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string str = SerialPort.ReadExisting();
            //Console.WriteLine(str);
            RecStr += str;

            str = str.Trim(new char[] { '\r', '\n' });
            if (str.Length > 0)
            {
                Log.Info(str);
            }
        }


        /// <summary>
        /// 光谱测量
        /// </summary>
        public override IData MeasureSpectrum()
        {
            
            return new IData();
        }
        //define MPG

        //色坐标测量
        //lxy色坐标测量
        public override IData MeasureLxy()
        {
            if (SerialPort == null || !SerialPort.IsOpen) { return null; }
            RecStr = "";
            send_cmd("ST"+ sLineBreak);
            IData Result = waitData("END");
            return Result;
        }

        private string signStr { get; set; } = "OK";
        private bool RecWait { get; set; }
        private string RecStr { get; set; } = "";



        private string waitString(string sign, int TimeOut)
        {
            Stopwatch sw = Stopwatch.StartNew();
            signStr = sign.Trim();
            while (sw.Elapsed.TotalSeconds < TimeOut)
            {
                if (RecStr.Contains(signStr))
                {
                    Console.WriteLine(RecStr.Contains(signStr));
                    break;
                }
            }
            //Console.WriteLine("DQWC-->"+ RecStr);
            return RecStr;
        }

        /// <summary>
        /// 色坐标
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        private IData waitData(string sign)
        {
            waitString(sign, 60);
            m_Result = DataParser(RecStr);
            return m_Result;
        }
        /// <summary>
        /// 等待光谱数据
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        private IData waitSpectrum(string sign)
        {
            waitString(sign, 60);
            //Console.WriteLine("读取");
            m_Result = ParserDataParser(RecStr);
            //Console.WriteLine("读取结束");
            return m_Result;
        }

        private IData ParserDataParser(string res)
        {


            res = res.Replace(Environment.NewLine, "\n");
            var datastrs = res.Split('\n');

            IData Result = new IData();
            Result.L = SafeParse(GetLine(datastrs, 5));
            Result.X = SafeParse(GetLine(datastrs, 4));
            Result.Y = SafeParse(GetLine(datastrs, 5));
            Result.Z = SafeParse(GetLine(datastrs, 6));
            Result.Cx = SafeParse(GetLine(datastrs, 7));
            Result.Cy = SafeParse(GetLine(datastrs, 8));
            Result.u = SafeParse(GetLine(datastrs, 9));
            Result.v = SafeParse(GetLine(datastrs, 10));
            Result.CCT = SafeParse(GetLine(datastrs, 11));



            res.Trim();


            try
            {
                res = res.Substring(res.IndexOf("\n380"));
            }
            catch (Exception e)
            {
                Log.Error("BM7A 解析异常(L260): " + e.Message);
            }


            for (int i = 0; i < 400; i++)
            {

                int Start = res.IndexOf($"{i + 380}") + 3;
                int Cont = 0;
                int Temp = 0;


                if (res.IndexOf($"{i + 380 + 1}") == -1)
                {
                    Cont = res.IndexOf($"END") - Start;
                    Temp = res.IndexOf($"END");
                }
                else
                {
                    Cont = res.IndexOf($"{i + 380 + 1}") - Start;
                    Temp = res.IndexOf($"{i + 380 + 1}");
                }


                try
                {
                    Result.SpectrumData[i] = Convert.ToDouble(res.Substring(Start, Cont).Replace(" ", ""));

                }
                catch (Exception e)
                {

                    //Project.WriteLog("截取-->"+res.Substring(Start, Cont).Replace(" ", "") + $"循环次数;【{i}】");
                    //Project.WriteLog("SR3【293行】" + e.Message+$"循环次数;【{i}】");
                    //res.Substring(Start, Cont).Trim();
                }

                try
                {
                    res = res.Substring(Temp - 1);
                }
                catch (Exception e)
                {
                    Log.Error("BM7A 解析异常(L303): " + e.Message + $"循环次数;【{i}】");
                }





                //Result.SpectrumData[i] =  Convert.ToDouble(  datastrs[i+14].Substring(datastrs[i+14].IndexOf(" ")));
            }


            return Result;
        }

        private IData DataParser(string res)
        {
            res = res.Replace(Environment.NewLine, "\n");
            var datastrs = res.Split('\n');

            // BM-7A response layout for a ST (color-coordinate) measurement:
            //   [0]   "OK"
            //   [1-11] config bytes (D0 / TS / MA / X2 / Y1 / Z3 / UC / F3 / K0 / FG0 / GK0)
            //   [12]   L      (luminance)
            //   [13]   X tristimulus
            //   [14]   Y tristimulus
            //   [15]   Z tristimulus
            //   [16]   Cx
            //   [17]   Cy
            //   [18]   u'
            //   [19]   v'
            //   [20]   Tc / CCT   — may be "*****" when colour falls outside CCT locus (e.g. pure blue)
            //   [21]   Duv        — may be "******" in the same case
            //   [22]   "END"
            // Asterisks must NOT crash the parser — use SafeParse which yields NaN for non-numeric values.
            IData Result = new IData();
            Result.L = SafeParse(GetLine(datastrs, 12));
            Result.X = SafeParse(GetLine(datastrs, 13));
            Result.Y = SafeParse(GetLine(datastrs, 14));
            Result.Z = SafeParse(GetLine(datastrs, 15));
            Result.Cx = SafeParse(GetLine(datastrs, 16));
            Result.Cy = SafeParse(GetLine(datastrs, 17));
            Result.u = SafeParse(GetLine(datastrs, 18));
            Result.v = SafeParse(GetLine(datastrs, 19));
            Result.CCT = SafeParse(GetLine(datastrs, 20));
            Result.Tc = Result.CCT;
            return Result;
        }

        /// <summary>
        /// Safe numeric parse for BM-7A response lines. Returns <see cref="double.NaN"/>
        /// when the line is asterisks (instrument reports "*****" for unmeasurable
        /// quantities like CCT on pure blue), empty, or otherwise non-numeric.
        /// Callers should check <see cref="double.IsNaN"/> rather than treating NaN
        /// as a real reading.
        /// </summary>
        private static double SafeParse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return double.NaN;
            var t = s.Trim();
            if (t.Length == 0 || t[0] == '*') return double.NaN;
            double v;
            if (double.TryParse(t, System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture, out v))
                return v;
            Log.Warn("BM7A 数值解析失败: \"" + t + "\" → NaN");
            return double.NaN;
        }

        private static string GetLine(string[] lines, int idx)
        {
            return (lines != null && idx >= 0 && idx < lines.Length) ? lines[idx] : null;
        }


        //等待数据读取完毕
        private List<byte> waitQbuffer(Queue<byte[]> buffer, int length)
        {
            List<byte> lsttmpbt = new List<byte>();//list列表缓存数据
            int curt = 0;
            int limt = 65000;
            while (true)
            {
                if (lsttmpbt.Count >= length)
                {
                    return lsttmpbt;
                }

                if (buffer.Count != 0)
                { lsttmpbt.AddRange(buffer.Dequeue()); }
                else
                {
                    if (curt >= limt) return lsttmpbt;
                    curt += 50;
                    Thread.Sleep(50);
                }
            }
        }

        //关闭仪器
        public override void Close()
        {

        }

        public static TestMachine GetInstance()
        {
            if (m_bm7a == null) m_bm7a = new BM7A();
            return m_bm7a;
        }
    }
}
