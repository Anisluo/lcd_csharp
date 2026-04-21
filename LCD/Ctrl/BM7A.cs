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
                Project.WriteLog("Bm7a串口发送：" + cmd + "，异常：" + ex.Message);
                return false;
            }
            Project.WriteLog("Bm7a已经发送命令：" + cmd);
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


            //设置通讯参数
            SerialPort.PortName = Project.cfg.BM7A.comName;
            SerialPort.BaudRate = int.Parse(Project.cfg.BM7A.bardRateText);
            SerialPort.DataBits = int.Parse(Project.cfg.BM7A.dataBitText);
            //serialPort.Parity = Project.cfg.SR3A.parityText;
            switch (Project.cfg.BM7A.stopBit)
            {
                case 0: SerialPort.StopBits = StopBits.None; break;
                case 1: SerialPort.StopBits = StopBits.One; break;
                case 2: SerialPort.StopBits = StopBits.Two; break;
            }
            switch (Project.cfg.BM7A.parity)
            {
                case 0: SerialPort.Parity = Parity.None; break;
                case 1: SerialPort.Parity = Parity.Odd; break;
                case 2: SerialPort.Parity = Parity.Even; break;
            }

            LogHelper.Instance.Write($"Bm7a:[{SerialPort.PortName}][{SerialPort.BaudRate}][{SerialPort.DataBits}][{SerialPort.Parity}][{Project.cfg.SR3A.stopBit}][{Project.cfg.SR3A.parity}]");

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
                Project.WriteLog("错误：Bm7a串口打开失败:" + ex.Message);
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
                Project.WriteLog(str);
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
            //Console.WriteLine(res);
            var datastrs = res.Split('\n');

            //for (int i = 0; i < datastrs.Length; i++)
            //{
            //    Console.WriteLine(datastrs[i]);
            //}
            //Console.WriteLine("X-->" + datastrs[5]);
            //Console.WriteLine("Y-->" + datastrs[6]);
            //Console.WriteLine("Z-->" + datastrs[7]);
            //Console.WriteLine("CX-->" + datastrs[8]);
            //Console.WriteLine("CY-->" + datastrs[9]);
            //Console.WriteLine("U-->" + datastrs[10]);
            //Console.WriteLine("V-->" + datastrs[11]);
            //Console.WriteLine("CCT-->" + datastrs[12]);

            IData Result = new IData();
            Result.L = Convert.ToDouble(datastrs[5]);
            Result.X = Convert.ToDouble(datastrs[4]);
            Result.Y = Convert.ToDouble(datastrs[5]); //Convert.ToDouble(datastrs[5]);
            Result.Z = Convert.ToDouble(datastrs[6]);
            Result.Cx = Convert.ToDouble(datastrs[7]);
            Result.Cy = Convert.ToDouble(datastrs[8]);
            Result.u = Convert.ToDouble(datastrs[9]);
            Result.v = Convert.ToDouble(datastrs[10]);
            Result.CCT = Convert.ToDouble(datastrs[11]);



            res.Trim();


            try
            {
                res = res.Substring(res.IndexOf("\n380"));
            }
            catch (Exception e)
            {
                Project.WriteLog("SR3【260行】" + e.Message);
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
                    Project.WriteLog("SR3【303行】" + e.Message + $"循环次数;【{i}】");
                }





                //Result.SpectrumData[i] =  Convert.ToDouble(  datastrs[i+14].Substring(datastrs[i+14].IndexOf(" ")));
            }


            return Result;
        }

        private IData DataParser(string res)
        {
            res = res.Replace(Environment.NewLine, "\n");
            var datastrs = res.Split('\n');



            IData Result = new IData();
            Result.L = Convert.ToDouble(datastrs[12]);
            Result.X = Convert.ToDouble(datastrs[13]);
            Result.Y = Convert.ToDouble(datastrs[14]); //Convert.ToDouble(datastrs[5]);
            Result.Z = Convert.ToDouble(datastrs[15]);
            Result.Cx = Convert.ToDouble(datastrs[16]);
            Result.Cy = Convert.ToDouble(datastrs[17]);
            Result.u = Convert.ToDouble(datastrs[18]);
            Result.v = Convert.ToDouble(datastrs[19]);
            Result.CCT = Convert.ToDouble(datastrs[20]);
            return Result;
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

        internal static TestMachine GetInstance()
        {
            if (m_bm7a == null) m_bm7a = new BM7A();
            return m_bm7a;
        }
    }
}
