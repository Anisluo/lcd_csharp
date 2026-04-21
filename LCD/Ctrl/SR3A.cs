using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCD.Data;

namespace LCD.Ctrl
{
    /// <summary>
    /// 串口通信
    /// </summary>
    public class SR3A : TestMachine
    {
        private static SR3A m_Sr3a;
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
                Project.WriteLog("Sr3ar串口发送：" + cmd + "，异常：" + ex.Message);
                return false;
            }
            Project.WriteLog("已经发送命令："+cmd);
            return true;
        }

        //private string GetLumData()
        //{
        //    string res = "";
        //    if (serialPort.IsOpen)
        //    {
        //        buffer.Clear();

        //        send_cmd("D1 ST" + sLineBreak);//先发送D0

        //        List<byte> btdata = waitQbuffer(buffer);
        //        string strdata = Encoding.Default.GetString(btdata.ToArray());
        //    }
        //    return res;
        //}

        /// <summary>
        /// 等待数据读
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        //private List<byte> waitQbuffer(Queue<byte[]> buffer)
        //{
        //    List<byte> lsttmpbt = new List<byte>();//list列表缓存数据
        //    int curt = 0;
        //    int limt = 500;
        //    while (true)
        //    {
        //        if (buffer.Count != 0)
        //        { lsttmpbt.AddRange(buffer.Dequeue()); }
        //        else
        //        {
        //            if (curt >= limt) return lsttmpbt;
        //            curt += 50;
        //            Thread.Sleep(50);
        //        }
        //    }
        //}


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
            SerialPort.PortName = Project.cfg.SR3A.comName;
            SerialPort.BaudRate = int.Parse(Project.cfg.SR3A.bardRateText);
            SerialPort.DataBits = int.Parse(Project.cfg.SR3A.dataBitText);
            //serialPort.Parity = Project.cfg.SR3A.parityText;
            switch (Project.cfg.SR3A.stopBit)
            {
                case 0: SerialPort.StopBits = StopBits.None; break;
                case 1: SerialPort.StopBits = StopBits.One; break;
                case 2: SerialPort.StopBits = StopBits.Two; break;
            }
            switch (Project.cfg.SR3A.parity)
            {
                case 0: SerialPort.Parity = Parity.None; break;
                case 1: SerialPort.Parity = Parity.Odd; break;
                case 2: SerialPort.Parity = Parity.Even; break;
            }

            LogHelper.Instance.Write($"Sr3a:[{SerialPort.PortName}][{ SerialPort.BaudRate}][{ SerialPort.DataBits}][{ SerialPort.Parity}][{Project.cfg.SR3A.stopBit}][{Project.cfg.SR3A.parity}]");

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
            catch(Exception ex)
            {
                Project.WriteLog("错误：SR3A串口打开失败:"+ex.Message);
                return;
            }
            //send_cmd("D0 ST\r\n");
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

        //private static Queue<byte[]> buffer = new Queue<byte[]>();


        public bool SendCmd(string cmd)
        {
            if (SerialPort == null || !SerialPort.IsOpen)
                return false;

            return send_cmd(cmd);
        }
        /// <summary>
        /// 光谱测量
        /// </summary>
        public override IData MeasureSpectrum()
        {
            if (SerialPort == null || !SerialPort.IsOpen) { return null; }
            RecStr = "";
            SendCmd("RM"+ sLineBreak);
            var _Str = waitString("OK", 1);
            if(_Str.Contains("OK")==false)
            {
                Project.WriteLog("错误：通讯超时" );
                return null;
            }
            RecStr = "";
            SendCmd("D0 ST"+ sLineBreak);
            //IData Result = waitData("END"); waitSpectrum（）;
            IData Result = waitSpectrum("END");
            return Result;
        }
        //define MPG

        public bool SenFdlCmd(string cmd)
        {
            if (SerialPort == null || !SerialPort.IsOpen) 
            {
                Project.WriteLog("错误：SR3A串口未打开" );
                return false; 
            }
            RecStr = "";
            SendCmd("RM" + sLineBreak);
            var _Str = waitString("OK", 2);
            if(string.IsNullOrEmpty(_Str))
            {
                Project.WriteLog("错误：收OK超时");
                return false;
            }
            RecStr = "";
            return SendCmd(cmd + sLineBreak);
        }

        //色坐标测量
        //lxy色坐标测量
        public override IData MeasureLxy()
        {
            if (SerialPort == null || !SerialPort.IsOpen) { return null; }
            RecStr = "";
            bool st = send_cmd("RM"+sLineBreak);
            var _Str = waitString("OK", 1);
            if (_Str.Contains("OK") == false)
            {
                return null;
            }
            RecStr = "";
            send_cmd("D1 ST"+sLineBreak);
            IData Result = waitData("END");
            return Result;
        }

        private string signStr { get; set; } = "OK";
        private bool RecWait { get; set; }
        private string RecStr { get; set; } = "";

  
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
            if (RecStr.Contains(signStr)==false)
            {
                Project.WriteLog("错误：通讯超时" );
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
            string str = waitString(sign, 60);
            if(str.Contains(sign)==false)
            {
                Project.WriteLog("错误：通讯超时");
                return null;
            }
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
            string str = waitString(sign, 60);
            if(str.Contains(sign)==false)
            {
                Project.WriteLog("错误：通讯超时");
                return null;
            }
            //Console.WriteLine("读取");
            m_Result = ParserDataParser(RecStr);
            //Console.WriteLine("读取结束");
            return m_Result;
        }

        private IData ParserDataParser(string res)
        {
            res = res.Replace("OK","");
            res = res.TrimStart();
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
                res = res.Substring(res.IndexOf("\n380")).Trim();
            }
            catch (Exception e)
            {
                Project.WriteLog("SR3A 未找到380光谱数据:"+e.Message );
                return null;
            }

            datastrs = res.Split('\n');
            for (int i = 0; i < datastrs.Length; i++)
            {
                string[] data = datastrs[i].Trim().Split(' ');
                if(data.Length != 2)
                {
                    continue;
                }
                int Start = int.Parse(data[0]);
                Start -= 380;
                try
                {
                    Result.SpectrumData[Start] = Convert.ToDouble(data[1].Trim());
                    
                }
                catch (Exception e)
                {
                    Project.WriteLog("截取-->"+ datastrs[i] + $"循环次数;【{i}】");
                    Project.WriteLog("SR3【293行】" + e.Message+$"循环次数;【{i}】");
                    //res.Substring(Start, Cont).Trim();
                }          
            }
           
            return Result;
        }

        private IData DataParser(string res)
        {
            if (string.IsNullOrEmpty(res))
            {
                return null;
            }
            res = res.Replace("OK", "");
            res = res.TrimStart();
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


      
        //关闭仪器
        public override void Close()
        {
            if(SerialPort.IsOpen)
            {
                SerialPort.Close();
            }
        }

        internal static TestMachine GetInstance()
        {
            if (m_Sr3a == null) m_Sr3a = new SR3A();
            return m_Sr3a;
        }
    }
}
