using LCD.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using System.Threading;

namespace LCD.Ctrl
{
    /// <summary>
    /// 串口
    /// </summary>
    [Category("仪器"), Description("CS2000"), DisplayName("CS2000")]
    public class CS2000 : TestMachine
    {
        /*
         *OK00,
         * 1.0380e-5,1.0294e-5,1.0177e-5,1.0147e-5,9.9894e-6,9.6159e-6,9.3324e-6,8.9295e-6,
         * 9.0212e-6,9.0435e-6,9.2668e-6,9.4252e-6,9.4114e-6,9.3175e-6,9.4858e-6,9.6914e-6,
         * 9.7346e-6,9.8434e-6,9.8414e-6,9.7693e-6,9.9662e-6,9.8168e-6,9.8799e-6,1.0269e-5,1.0282e-5,
         * 1.0265e-5,1.0626e-5,1.0806e-5,1.0545e-5,1.1004e-5,1.1047e-5,1.1188e-5,1.1006e-5,1.0880e-5,
         * 1.0918e-5,1.0749e-5,9.9273e-6,9.3969e-6,9.2099e-6,9.3145e-6,9.4564e-6,9.6279e-6,1.0013e-5,
         * 1.0133e-5,9.9801e-6,1.0006e-5,1.0044e-5,1.0176e-5,1.0655e-5,1.0973e-5,1.1401e-5,1.1761e-5,
         * 1.2311e-5,1.2696e-5,1.3226e-5,1.3713e-5,1.4185e-5,1.4391e-5,1.4305e-5,1.4907e-5,1.5252e-5,
         * 1.5321e-5,1.5517e-5,1.5871e-5,1.5961e-5,1.6562e-5,1.6685e-5,1.6800e-5,1.7319e-5,1.7655e-5,
         * 1.7563e-5,1.8173e-5,1.8434e-5,1.8694e-5,1.8434e-5,1.8060e-5,1.8153e-5,1.7807e-5,1.6038e-5,
         * 1.3710e-5,1.1686e-5,1.0878e-5,1.0678e-5,1.1378e-5,1.2541e-5,1.4154e-5,1.5999e-5,1.7247e-5,
         * 1.8412e-5,1.9826e-5,2.0227e-5,2.0534e-5,2.1675e-5,2.2494e-5,2.2580e-5,2.2417e-5,2.2328e-5,
         * 2.3159e-5,2.3585e-5,2.3517e-5,2.3721e-5
         *
         */
        public IData m_Result { get; set; }
        SerialPort SerialPort = null;
        public override bool IsOpen { get => SerialPort == null ? false : SerialPort.IsOpen; }

        public override string sLineBreak { get; set; } = "\r\n";//\r\n

        private static CS2000 mycs2000 = null;

        public static CS2000 GetInstance()
        {
            if (mycs2000 == null) { mycs2000 = new CS2000(); }
            return mycs2000;
        }
        //private new SerialPort SerialPort;

        private CS2000()
        {
            Init();
        }


        //初始化测试数据
        public override void Init()
        {
            if (SerialPort == null)
            {
                SerialPort = new SerialPort();
            }
            if (Config == null || string.IsNullOrEmpty(Config.ComName))
            {
                Log.Warn("CS2000.Init: Config 未设置或 ComName 为空，跳过");
                return;
            }

            if (SerialPort.IsOpen) { SerialPort.Close(); }

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

            SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            

            SerialPort.DtrEnable = true;
            SerialPort.Handshake = Handshake.RequestToSend;
            SerialPort.WriteTimeout = 30;//设置发送超时
            try
            {
                SerialPort.Open();
            }
            catch (Exception ex)
            {
                Log.Error("CS2000串口打开失败:" + ex.Message);
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

        private string signStr { get; set; } = "OK00";
        private string waitString(string Start, int TimeOut)
        {
            Stopwatch sw = Stopwatch.StartNew();
            signStr = Start;

            //RecStr = RecStr.Substring(RecStr.IndexOf(signStr));

            while (sw.Elapsed.TotalSeconds < TimeOut)
            {
                string aa = RecStr.ToUpper();
                if (aa.Contains(signStr))
                {
                    break;
                }
            }
            //Console.WriteLine("DQWC-->"+ RecStr);
            return RecStr;
        }
        private string waitString(string Start, int TimeOut, int Cont)
        {
            Stopwatch sw = Stopwatch.StartNew();
            signStr = Start;

            //RecStr = RecStr.Substring(RecStr.IndexOf(signStr));





            while (sw.Elapsed.TotalSeconds < TimeOut)
            {
                

                int _Cont= System.Text.RegularExpressions.Regex.Matches(RecStr, ",").Count;

                //Log.Info(_Cont.ToString());
                //Log.Info(RecStr);

                if ((_Cont >= Cont)&&(RecStr.EndsWith(sLineBreak)))
                {
                    break;
                }
            }

            return RecStr;
        }
        private string RecStr { get; set; } = "";
        

        public override void StopTest()
        {
            if (SerialPort == null || !SerialPort.IsOpen) { return; }
            RecStr = "";
            SendCmd("MEAS,0" + sLineBreak);
            
        }

        //光谱测量
        public override IData MeasureSpectrum()
        {
            if (SerialPort == null || !SerialPort.IsOpen) { return null; }
            RecStr = "";
            SendCmd("RMTS,0"+ sLineBreak);
            var _Str = waitString("OK", 1);
            if (string.IsNullOrEmpty(_Str))
            {
                Log.Info("RMTS 0超时");
                return null;
            }
            RecStr = "";
            SendCmd("RMTS,1" + sLineBreak);
            _Str = waitString("OK", 1);
            if (string.IsNullOrEmpty(_Str))
            {
                Log.Info("RMTS 1超时");
                return null;
            }

            RecStr = "";
            SendCmd("MEAS,1" + sLineBreak);
            int _Num = waitTestData("OK", 1);
            DateTime dateTime = DateTime.Now;
            while ((DateTime.Now - dateTime).TotalMilliseconds < ((_Num * 1000) + 1000))
            {
                Console.WriteLine($"等待【{_Num}s】当前剩余【{(_Num +1) - ((DateTime.Now - dateTime).TotalSeconds) }】");
                Thread.Sleep(1000);
            }
            //Thread.Sleep((_Num*1000)+1000);

            RecStr = "";//MEAS,1
            //SendCmd("MEAS,1" + sLineBreak);
            Thread.Sleep(10);
            SendCmd("MEDR,2,0,0" + sLineBreak);
            Thread.Sleep(300);
            SendCmd("MEDR,1,0,0" + sLineBreak);
            // SendCmd("MEDR,1,0,1" + sLineBreak);
            //Thread.Sleep(300);
            // SendCmd("MEDR,1,0,2" + sLineBreak);
            //Thread.Sleep(300);
            // SendCmd("MEDR,1,0,3" + sLineBreak);
            // Thread.Sleep(300);
            // SendCmd("MEDR,1,0,4" + sLineBreak);
            //IData Result = waitData("END"); waitSpectrum（）;
            IData Result = waitSpectrum("OK00",425);//425
            return Result;
        }

        private IData waitSpectrum(string Start,int Cont)
        {
            //Console.WriteLine("读取");
            m_Result = ParserDataParser(waitString(Start, 100, Cont));
            //Console.WriteLine("读取结束");
            return m_Result;
        }
        private IData ParserDataParser(string res)
        {
            string[] ary = res.Trim().Split("\r\n".ToCharArray());

            if(ary.Length < 2)
            {
                Log.Error("测量失败");
                return null;
            }
            //res = res.Replace("OK00", "");
            //res = res.Replace("\r\n", "");
            //// res = res.Replace("OK00", ",");


            //Console.WriteLine(res);
            res = res.Trim(',');
            //第一个是亮色度数据
            var datastrs = ary[0].Trim("OK00,".ToCharArray()).Split(',');

            //datastrs = datastrs.Where(s => s != "").ToArray();
            //datastrs = datastrs.Where(s => s != "\r\n").ToArray();

            IData Result = new IData();

            //4-->
            Result.L = Convert.ToDouble(datastrs[4-1]);
            Result.X = Convert.ToDouble(datastrs[3-1]);
            Result.Y = Convert.ToDouble(datastrs[4-1]); //Convert.ToDouble(datastrs[4]);Y显示和L一样
            Result.Z = Convert.ToDouble(datastrs[5 - 1]);
            Result.Cx = Convert.ToDouble(datastrs[6 - 1]);
            Result.Cy = Convert.ToDouble(datastrs[7 - 1]);
            Result.u = Convert.ToDouble(datastrs[8 - 1]);
            Result.v = Convert.ToDouble(datastrs[9 - 1]);
            Result.CCT = Convert.ToDouble(datastrs[10 - 1]);

            //最后一个是光谱数据
            datastrs = ary[ary.Length-1].Trim("OK00,".ToCharArray()).Split(',');
            if(datastrs.Length != 401)
            {
                Log.Error("测量失败，返回光谱数据数量错误："+datastrs.Length);
                return Result;
            }
            for (int i = 0; i < 401; i++)
            {
                Result.SpectrumData[i] = Convert.ToDouble(datastrs[i]);
            }


            return Result;
        }

        private bool send_cmd(string cmd)
        {
            try
            {
                SerialPort.Write(cmd);
                //LogHelper.Instance.Write("光谱串口已经发送：" + cmd);
            }
            catch (Exception ex)
            {
                Log.Info("Sr3ar串口发送：" + cmd + "，异常：" + ex.Message);
                return false;
            }
            Log.Info("已经发送命令：" + cmd);
            return true;
        }

        public bool SendCmd(string cmd)
        {
            if (SerialPort == null || !SerialPort.IsOpen)
                return false;

            return send_cmd(cmd);
        }

        //lxy色坐标测量
        public override IData MeasureLxy()
        {
            if (SerialPort == null || !SerialPort.IsOpen) { return null; }
            RecStr = "";
            SendCmd("RMTS,0"+ sLineBreak);
            var _Str = waitString("OK", 1);
            if(string.IsNullOrEmpty(_Str))
            {
                Log.Info("RMTS 0超时");
                return null;
            }
            RecStr = "";
           
            SendCmd("RMTS,1" + sLineBreak);
            _Str = waitString("OK", 1);
            if (string.IsNullOrEmpty(_Str))
            {
                Log.Info("RMTS 1超时");
                return null;
            }
            RecStr = "";
            SendCmd("MEAS,1"+ sLineBreak);
            int _Num = waitTestData("OK", 1);
            DateTime dateTime=DateTime.Now;
            while ((DateTime.Now- dateTime).TotalMilliseconds<((_Num * 1000) + 1000))
            {
                Console.WriteLine($"等待【{_Num}s】当前剩余【{(_Num +1)  - ((DateTime.Now - dateTime).TotalSeconds)}】");
                Thread.Sleep(1000);
            }
            //OK00,014
            RecStr = "";
            SendCmd("MEDR,2,0,0" + sLineBreak);
            IData Result = waitData("OK00",24);
            return Result;
        }
        private int waitTestData(string Start, int Cont)
        {
            string temp = waitString(Start, 4, Cont);
            string[] arry = temp.Split("\r\n".ToCharArray());
            var datastrs = arry[0].Split(',');            
            try
            {
                Log.Info($"延时-->{datastrs[1]}");
                return Convert.ToInt32(datastrs[datastrs.Length - 1]);
            }
            catch
            {
                return 0;
            }
        }

        private IData waitData(string Start,int Cont)
        {
            
            //waitString(Start, 60, Cont);
            m_Result = DataParser(waitString(Start, 20, Cont));
            Log.Info(m_Result.X.ToString());
            Log.Info(m_Result.Y.ToString());
            Log.Info(m_Result.Z.ToString());
            Log.Info(m_Result.Cx.ToString());
            Log.Info(m_Result.Cy.ToString());
            Log.Info(m_Result.Z.ToString());
            Log.Info(m_Result.u.ToString());
            return m_Result;
        }
        private IData DataParser(string res)
        {
            //res = res.Replace(Environment.NewLine, "\n");
            var datastrs = res.Split(',');

            IData Result = new IData();
            Result.L = Convert.ToDouble(datastrs[4]);
            Result.X = Convert.ToDouble(datastrs[3]);
            Result.Y = Convert.ToDouble(datastrs[4]); //Convert.ToDouble(datastrs[4]);Y显示和L一样
            Result.Z = Convert.ToDouble(datastrs[5]);
            Result.Cx = Convert.ToDouble(datastrs[6]);
            Result.Cy = Convert.ToDouble(datastrs[7]);
            Result.u = Convert.ToDouble(datastrs[8]);
            Result.v = Convert.ToDouble(datastrs[9]);
            Result.CCT = Convert.ToDouble(datastrs[10]);
            return Result;
        }
        //关闭仪器
        public override void Close()
        {
            if(SerialPort == null)
            {
                return;
            }
            if (SerialPort.IsOpen)
            {
                SerialPort.Close();
            }
        }
    }
}
