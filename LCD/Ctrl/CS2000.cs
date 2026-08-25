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
    class CS2000 : TestMachine
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
        SerialPort seral = null;
        public override bool IsOpen { get => seral == null ? false : seral.IsOpen; }

        public override string sLineBreak { get; set; } = "";//\r\n

        private static CS2000 mycs2000 = null;

        public static CS2000 GetInstance()
        {
            if (mycs2000 == null) { mycs2000 = new CS2000(); }
            return mycs2000;
        }

        /// <summary>
        /// 直接向 CS2000 发送指令（视场角等）。参照 guoxian 分支移植。
        /// </summary>
        public bool SendCmd(string cmd)
        {
            if (serialPort == null || !serialPort.IsOpen)
                return false;
            return serialPort.SendStr(cmd);
        }
        //private new SerialPort serialPort;

        private CS2000()
        {
            //定义CS2000三维坐标位置
            var key = EComManageer.CreateECom(CommunicationModel.COM);//创建串口;
            serialPort = EComManageer.GetECommunacation(key);

            serialPort.PortName = "COM1";//
            serialPort.BaudRate = "115200";
            serialPort.DataBits = "8";
            serialPort.StopBits = "One";
            serialPort.Parity = "None";

            //serialPort.WriteBufferSize = 1024;
            //serialPort.ReadBufferSize = 1024;
            //serialPort.RtsEnable = true;
            //serialPort.DtrEnable = true;
            //serialPort.ReceivedBytesThreshold = 1;
        }


        //初始化测试数据
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

            serialPort.PortName = Project.cfg.CS2000.comName;
            serialPort.BaudRate = Project.cfg.CS2000.bardRateText;
            serialPort.DataBits = Project.cfg.CS2000.dataBitText;
            serialPort.Parity = Project.cfg.CS2000.parityText;
            serialPort.StopBits = Project.cfg.CS2000.stopBitText;



            //serialPort.PortName = Project.cfg.SR3A.comName;
            //serialPort.BaudRate = Project.cfg.SR3A.bardRate.ToString();
            //serialPort.DataBits = Project.cfg.SR3A.dataBit.ToString();

            //serialPort.StopBits = StopBits.None.ToString();

            //serialPort.serialPort.ReadTimeout = 2000;
            //serialPort.serialPort.WriteBufferSize = 1024;
            //serialPort.serialPort.ReadBufferSize = 1024;
            //serialPort.serialPort.RtsEnable = true;
            //serialPort.serialPort.DtrEnable = true;
            //serialPort.serialPort.ReceivedBytesThreshold = 1;

            serialPort.ReceiveString += new ReceiveString(serialPort_DataReceivedEventHandler);

            if (serialPort.IsOpen) { serialPort.DisConnect(); }
            serialPort.Connect();

            //serialPort.SendStr("RMTS,0");  //Chw20220622
            //var _Str = waitString("OK00", 5);//OK00
            //serialPort.SendStr("RMTS,1");//RMTS,0

            //_Str = waitString("OK00", 5);//OK00
            //等待远程设置OK
            //Task.Run(() =>
            //{
            //    var _Str = waitString("OK",30000);
            //});
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

                //Project.WriteLog(_Cont.ToString());
                //Project.WriteLog(RecStr);

                if (_Cont >= Cont)
                {
                    break;
                }
            }

            return RecStr;
        }
        private string RecStr { get; set; } = "";
        private void serialPort_DataReceivedEventHandler(string res)
        {

            RecStr += res; /*+ Environment.NewLine;*/

        }

        public override void StopTest()
        {
            if (serialPort == null || !serialPort.IsOpen) { return; }
            RecStr = "";
            serialPort.SendStr("MEAS,0" + sLineBreak);
            
        }

        //光谱测量
        public override IData MeasureSpectrum()
        {
            if (serialPort == null || !serialPort.IsOpen) { return null; }
            RecStr = "";
            serialPort.SendStr("RMTS,0"+ sLineBreak);
            var _Str = waitString("OK", 1);
            RecStr = "";
            serialPort.SendStr("RMTS,1" + sLineBreak);
            _Str = waitString("OK", 1);

            RecStr = "";
            serialPort.SendStr("MEAS,1");
            int _Num = waitTestData("OK", 1);
            DateTime dateTime = DateTime.Now;
            while ((DateTime.Now - dateTime).TotalMilliseconds < ((_Num * 1000) + 1000))
            {
                Console.WriteLine($"等待【{_Num}s】当前剩余【{(_Num +1) - ((DateTime.Now - dateTime).TotalSeconds) }】");
                Thread.Sleep(1000);
            }
            //Thread.Sleep((_Num*1000)+1000);

            RecStr = "";//MEAS,1
            //serialPort.SendStr("MEAS,1" + sLineBreak);
            Thread.Sleep(10);
            serialPort.SendStr("MEDR,2,0,0" + sLineBreak);
            Thread.Sleep(300);
            serialPort.SendStr("MEDR,1,0,1" + sLineBreak);
           Thread.Sleep(300);
            serialPort.SendStr("MEDR,1,0,2" + sLineBreak);
           Thread.Sleep(300);
            serialPort.SendStr("MEDR,1,0,3" + sLineBreak);
            Thread.Sleep(300);
            serialPort.SendStr("MEDR,1,0,4" + sLineBreak);
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

            res = res.Replace("OK00", ",");
           // res = res.Replace("OK00", ",");


            //Console.WriteLine(res);
            var datastrs = res.Split(',');

            datastrs = datastrs.Where(s => s != "").ToArray();
            datastrs = datastrs.Where(s => s != "\r\n").ToArray();

            IData Result = new IData();

            Result.L = Convert.ToDouble(datastrs[1]);
            Result.X = Convert.ToDouble(datastrs[2]);
            Result.Y = Convert.ToDouble(datastrs[3]);//Convert.ToDouble(datastrs[4]);
            Result.Z = Convert.ToDouble(datastrs[4]);
            Result.Cx = Convert.ToDouble(datastrs[5]);
            Result.Cy = Convert.ToDouble(datastrs[6]);
            Result.u = Convert.ToDouble(datastrs[7]);
            Result.v = Convert.ToDouble(datastrs[8]);
            Result.CCT = Convert.ToDouble(datastrs[9]);

            for (int i = 0; i < 401; i++)
            {
                Result.SpectrumData[i] = Convert.ToDouble(datastrs[i + 24]);
            }


            return Result;
        }
        //lxy色坐标测量
        public override IData MeasureLxy()
        {
            if (serialPort == null || !serialPort.IsOpen) { return null; }
            RecStr = "";
            serialPort.SendStr("RMTS,0");
            var _Str = waitString("OK", 1);
            RecStr = "";
           
            serialPort.SendStr("RMTS,1");
            _Str = waitString("OK", 1);
            RecStr = "";
            serialPort.SendStr("MEAS,1");
            int _Num = waitTestData("OK00", 1);
            DateTime dateTime=DateTime.Now;
            while ((DateTime.Now- dateTime).TotalMilliseconds<((_Num * 1000) + 1000))
            {
                Console.WriteLine($"等待【{_Num}s】当前剩余【{(_Num +1)  - ((DateTime.Now - dateTime).TotalSeconds)}】");
                Thread.Sleep(1000);
            }
            //OK00,014
            RecStr = "";
            serialPort.SendStr("MEDR,2,0,0");
            IData Result = waitData("OK00",24);
            return Result;
        }
        private int waitTestData(string Start, int Cont)
        {
            string temp = waitString(Start, 100, Cont);
            var datastrs = temp.Split(',');
            Project.WriteLog($"延时-->{datastrs[1]}");
            return Convert.ToInt32(datastrs[1]);
        }

        private IData waitData(string Start,int Cont)
        {
            
            //waitString(Start, 60, Cont);
            m_Result = DataParser(waitString(Start, 20, Cont));
            Project.WriteLog(m_Result.X.ToString());
            Project.WriteLog(m_Result.Y.ToString());
            Project.WriteLog(m_Result.Z.ToString());
            Project.WriteLog(m_Result.Cx.ToString());
            Project.WriteLog(m_Result.Cy.ToString());
            Project.WriteLog(m_Result.Z.ToString());
            Project.WriteLog(m_Result.u.ToString());
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
            if (serialPort == null) { }
            if (serialPort.IsOpen)
            {

            }
        }
    }
}
