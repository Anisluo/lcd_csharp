using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace LCD.Ctrl
{
    public class Power
    {
        protected ECom serialPort = null;
        public Result result { get; set; }
        public bool IsOpen { get; set; } = false;
        public Power()
        {
            var key = EComManageer.CreateECom(CommunicationModel.COM);//创建串口;
            serialPort = EComManageer.GetECommunacation(key);
            serialPort.PortName = "COM10";//
            serialPort.BaudRate = "115200";
            serialPort.DataBits = "8";
            serialPort.StopBits = "One";
            serialPort.Parity = "None";
        }
        public void Init()
        {
            if (serialPort == null)
            {
                var key = EComManageer.CreateECom(CommunicationModel.COM);//创建串口;
                serialPort = EComManageer.GetECommunacation(key);
            }
            serialPort.PortName = Project.cfg.power.Bus.comName;
            serialPort.BaudRate = Project.cfg.power.Bus.BarRate;
            serialPort.DataBits = Project.cfg.power.Bus.DataBit;
            serialPort.Parity = Project.cfg.power.Bus.Parity;
            serialPort.StopBits = Project.cfg.power.Bus.StopBit;

            serialPort.ReceiveString += new ReceiveString(serialPort_DataReceivedEventHandler);

            if (serialPort.IsOpen) { serialPort.DisConnect(); }
            serialPort.Connect();
            RecStr = "";
            serialPort.SendStr("*IDN?"+"\r\n");

            var _Str = waitString("GEW832098", 5);//OK00
            if (_Str!="")
            {
                Project.WriteLog(_Str);
                IsOpen = true;
            }
        }
        private string RecStr { get; set; } = "";
        private void serialPort_DataReceivedEventHandler(string res)
        {

            RecStr += res; /*+ Environment.NewLine;*/

        }
        //private string signStr { get; set; } = "GEW832098";
        private string waitString(string Start, int TimeOut)
        {
            Stopwatch sw = Stopwatch.StartNew();
            string signStr = Start;
 
            while (sw.Elapsed.TotalSeconds < TimeOut)
            {
                if (RecStr.Contains(signStr))
                {
                    break;
                }
            }
            //Console.WriteLine("DQWC-->"+ RecStr);
            return RecStr;
        }
        private string waitString( int TimeOut, int Cont)
        {
            Stopwatch sw = Stopwatch.StartNew();

            //RecStr = RecStr.Substring(RecStr.IndexOf(signStr));





            while (sw.Elapsed.TotalSeconds < TimeOut)
            {


                int _Cont = System.Text.RegularExpressions.Regex.Matches(RecStr, ",").Count;

                //Project.WriteLog(_Cont.ToString());
                //Project.WriteLog(RecStr);

                if (_Cont >= Cont)
                {
                    break;
                }
            }
            //Console.WriteLine("DQWC-->"+ RecStr);
            return RecStr;
        }
        public Result Query()
        {
            RecStr = "";
            serialPort.SendStr(":NUMERIC:NORMAL:VALUE?" + "\r\n");

            return waitPower(3);
        }
        public Result waitPower(int Cont)
        {
            result = ParserDataParser(waitString(5, 3));
            return result;
        }
        private Result ParserDataParser(string res)
        {
            if (res == "")
            {
                return null;
            }

            //Console.WriteLine(res);
            var datastrs = res.Split(',');

            Result Result = new Result();

            Result.Voltage= Convert.ToDouble(datastrs[0]);
            Result.ElectricCurrent = Convert.ToDouble(datastrs[1]);
            Result.Power = Convert.ToDouble(datastrs[2]);

            return Result;
        }

        public void Close()
        {
            if(serialPort != null)
            {
                if(serialPort.IsConnected)
                {
                    serialPort.DisConnect();
                }
            }
        }
        
    }
    public class Result
    {
        public double Voltage { get; set; }
        public double ElectricCurrent  { get; set; }
        public double Power { get; set; }
    }
}
