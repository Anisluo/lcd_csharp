using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LCD.Ctrl
{
    internal class PowerM88 : IPowerDevice
    {
        private string data_recv = "";
        private SerialPort serial;
        public PowerM88()
        {
            serial = new SerialPort();
        }
        public bool start(string portname)
        {
            serial.PortName = portname;
            serial.BaudRate = 9600;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.RtsEnable = true;//必须使能这个，不然收到的是乱码
            serial.DataReceived += Serial_DataReceived;
            try
            {
                serial.Open();
            }
            catch (Exception ex)
            {
                Project.WriteLog("串口打开失败："+ex.Message);
                return false;
            }
            return true;
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //data_recv += serial.ReadExisting();
            int len = serial.BytesToRead;
            byte[] ary = new byte[len];
            serial.Read(ary, 0, ary.Length);//读取数据
            //滤掉非可见字符
            byte[] bytes = ary.Where(p => p < 127 ).ToArray();
            string str = Encoding.ASCII.GetString(bytes);
            data_recv += str;
        }

        public Result query()
        {
            data_recv = "";//首先清空一下
            send_cmd("MEAS:VCM?");
            //接下来获取数据并解析啊
            int timeout = 20;
            for(int i=0;i<timeout;i++)
            {
                if(data_recv.Contains("\n"))
                {
                    break;
                }
                Thread.Sleep(100);
            }
            if(data_recv.Contains("\n")==false)
            {
                LogHelper.Instance.Write("查询接收超时");
                return null;
            }
            string[] ary = data_recv.Trim().Split(',');
            if(ary.Length <3)
            {
                LogHelper.Instance.Write("查询返回错误格式数据："+data_recv);
                return null;
            }
            Result result = new Result();
            result.Voltage = double.Parse(ary[0]);
            send_cmd("MEAS:CURR?");
            //接下来获取数据并解析啊
            for (int i = 0; i < timeout; i++)
            {
                if (data_recv.Contains("\n"))
                {
                    break;
                }
                Thread.Sleep(100);
            }
            if (data_recv.Contains("\n") == false)
            {
                LogHelper.Instance.Write("查询电流接收超时");
                return null;
            }
            result.ElectricCurrent = double.Parse(data_recv.Trim());
            return result;
        }

        public void stop()
        {
            if (serial.IsOpen)
            {
                serial.Close();
            }
        }

        private bool send_cmd(string cmd)
        {
            data_recv = "";
            if (serial.IsOpen)
            {
                try
                {
                    serial.Write(cmd + "\n");//添加结束符号0xa
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Write("M88串口发送:"+cmd+"失败：" + ex.Message);
                    return false;
                }
                //Project.WriteLog("M88已经发送：" + cmd);
                return true;
            }
            LogHelper.Instance.Write("M88串口未打开，不能发送：" + cmd);
            return false;
        }

        public bool current_set(double val)
        {
            string cmd = "CURR " + val;
            return send_cmd(cmd);
        }

        public bool output(bool on_off)
        {
            if (on_off)
            {
                return send_cmd("OUTP 1");
            }
            else
            {
                return send_cmd("OUTP 0");
            }
        }

        public bool voltage_set(double val)
        {
            string cmd = "VOLT " + val;
            return send_cmd(cmd);
        }
    }
}
