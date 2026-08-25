using LCD.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisionCore;

namespace LCD.Ctrl
{
    /// <summary>
    /// 串口通信
    /// </summary>
    [Category("仪器"), Description("SR3A"), DisplayName("SR3A")]
    public class SR3A : TestMachine
    {
        private static SR3A m_Sr3a;
        public IData m_Result { get; set; }
        public override string sLineBreak { get; set; } = "\r\n";//结束时使用的分割符号
        public string devName;

        public override bool IsOpen { get => serialPort == null ? false : serialPort.IsOpen; }

        private string GetLumData()
        {
            string res = "";
            if (serialPort.IsOpen)
            {
                buffer.Clear();

                serialPort.SendStr("D1 ST" + sLineBreak);//先发送D0

                List<byte> btdata = waitQbuffer(buffer);
                string strdata = Encoding.Default.GetString(btdata.ToArray());
            }
            return res;
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
            if (serialPort == null)
            {
                var key = EComManageer.CreateECom(CommunicationModel.COM);//创建串口;
                serialPort = EComManageer.GetECommunacation(key);
            }

            //serialPort.ReceiveString += ECommunacation_ReceiveString;

            //设置通讯参数
            //serialPort.PortName = "COM4";//
            //serialPort.BaudRate = "38400";
            //serialPort.DataBits = "7";
            //serialPort.StopBits = "One";
            //serialPort.Parity = "Odd";



            //Console.WriteLine(Project.cfg.SR3A.comName);
            //Console.WriteLine(Project.cfg.SR3A.bardRateText);
            //Console.WriteLine(Project.cfg.SR3A.dataBitText);
            //Console.WriteLine(Project.cfg.SR3A.parityText);
            //Console.WriteLine(Project.cfg.BM7A.stopBit);

            serialPort.PortName = Project.cfg.SR3A.comName;
            serialPort.BaudRate = Project.cfg.SR3A.bardRateText;
            serialPort.DataBits = Project.cfg.SR3A.dataBitText;
            serialPort.Parity = Project.cfg.SR3A.parityText;


            Console.WriteLine($"[{serialPort.PortName}][{ serialPort.BaudRate}][{ serialPort.DataBits}][{ serialPort.Parity}][{Project.cfg.SR3A.stopBit}][{Project.cfg.SR3A.parity}]");



            //serialPort.PortName = Project.cfg.SR3A.comName;
            //serialPort.BaudRate = Project.cfg.SR3A.bardRate.ToString();
            //serialPort.DataBits = Project.cfg.SR3A.dataBit.ToString();

            switch (Project.cfg.SR3A.stopBit)
            {
                case 0: serialPort.StopBits = StopBits.None.ToString(); break;
                case 1: serialPort.StopBits = StopBits.One.ToString(); break;
                case 2: serialPort.StopBits = StopBits.Two.ToString(); break;
            }
            switch (Project.cfg.SR3A.parity)
            {
                case 0: serialPort.Parity = Parity.None.ToString(); break;
                case 1: serialPort.Parity = Parity.Odd.ToString(); break;
                case 2: serialPort.Parity = Parity.Even.ToString(); break;
            }

            //serialPort.serialPort.ReadTimeout = 2000;
            //serialPort.serialPort.WriteBufferSize = 1024;
            //serialPort.serialPort.ReadBufferSize = 1024;
            //serialPort.serialPort.RtsEnable = true;
            //serialPort.serialPort.DtrEnable = true;
            //serialPort.serialPort.ReceivedBytesThreshold = 1;

            serialPort.ReceiveString += new ReceiveString(serialPort_DataReceivedEventHandler);

            if (serialPort.IsOpen) { serialPort.DisConnect(); }
            serialPort.Connect();

            //serialPort.SendStr("RM");

            //var _Str = waitString("OK", 1);
            
            //等待远程设置OK
            //Task.Run(() =>
            //{
            //    var _Str = waitString("OK", 30000);
            //});

        }
        private static Queue<byte[]> buffer = new Queue<byte[]>();

        /// <summary>
        /// 光谱测量
        /// </summary>
        public override IData MeasureSpectrum()
        {
            if (serialPort == null || !serialPort.IsOpen) { return null; }
            RecStr = "";
            serialPort.SendStr("RM");
            var _Str = waitString("OK", 1);
            serialPort.SendStr("D0 ST");
            //IData Result = waitData("END"); waitSpectrum（）;
            IData Result = waitSpectrum("END");
            return Result;
        }
        //define MPG

        //色坐标测量
        //lxy色坐标测量
        public override IData MeasureLxy()
        {
            if (serialPort == null || !serialPort.IsOpen) { return null; }
            RecStr = "";
            serialPort.SendStr("RM");
            var _Str = waitString("OK", 1);
            serialPort.SendStr("D1 ST");
            IData Result = waitData("END");
            return Result;
        }

        /// <summary>
        /// 设置 SR3A 视场角：先发 RM 等 OK，再发 FLD1~FLD4（对应 2° / 1° / 0.2° / 0.1°）。
        /// 参照 guoxian 分支移植；发送方式与本类 RM/D0 ST 保持一致。
        /// </summary>
        public bool SenFdlCmd(string cmd)
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                Project.WriteLog("SR3A串口未打开");
                return false;
            }
            RecStr = "";
            serialPort.SendStr("RM");
            var _Str = waitString("OK", 2);
            if (string.IsNullOrEmpty(_Str) || !_Str.Contains("OK"))
            {
                Project.WriteLog("视场角设置：收 OK 超时");
                return false;
            }
            RecStr = "";
            return serialPort.SendStr(cmd);
        }

        private string signStr { get; set; } = "OK";
        private bool RecWait { get; set; }
        private string RecStr { get; set; } = "";

        private void serialPort_DataReceivedEventHandler(string res)
        {
            Console.WriteLine(res);
            RecStr += res; /*+ Environment.NewLine;*/

        }

        private string waitString(string sign,int TimeOut)
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
            Result.L= Convert.ToDouble(datastrs[5]);
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
                Project.WriteLog("SR3【260行】"+e.Message );
            }
            

            for (int i = 0; i < 400; i++)
            {
                
                int Start = res.IndexOf($"{i + 380}") + 3;
                int Cont = 0;
                int Temp = 0;


                if (res.IndexOf($"{i + 380 + 1}")==-1)
                {
                    Cont= res.IndexOf($"END") -Start;
                    Temp = res.IndexOf($"END");
                }
                else
                {
                    Cont= res.IndexOf($"{i + 380 + 1}") -Start;
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
            Result.L = Convert.ToDouble(datastrs[5]);
            Result.X = Convert.ToDouble(datastrs[4]);
            Result.Y = Convert.ToDouble(datastrs[5]); //Convert.ToDouble(datastrs[5]);
            Result.Z = Convert.ToDouble(datastrs[6]);
            Result.Cx = Convert.ToDouble(datastrs[7]);
            Result.Cy = Convert.ToDouble(datastrs[8]);
            Result.u = Convert.ToDouble(datastrs[9]);
            Result.v = Convert.ToDouble(datastrs[10]);
            Result.CCT = Convert.ToDouble(datastrs[11]);
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
            if (m_Sr3a == null) m_Sr3a = new SR3A();
            return m_Sr3a;
        }
    }
}
