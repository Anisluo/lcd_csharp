using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LCD.Ctrl
{
    internal class PowerPld6003 : IPowerDevice
    {
        private SerialPort serial;
        private string data_recv = "";
        public PowerPld6003()
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
            serial.DataReceived += Serial_DataReceived;
            try
            {
                serial.Open();
            }
            catch(Exception ex)
            {
                Project.WriteLog("串口打开失败：" + ex.Message);
                return false;
            }
            return true;
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            data_recv += serial.ReadExisting();
        }

        public Result query()
        {
            data_recv = "";//首先清空一下
            send_cmd(":MEAS:VOLT?");
            //接下来获取数据并解析啊
            int timeout = 20;
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
                LogHelper.Instance.Write("查询接收超时");
                return null;
            }
            Result result = new Result();
            result.Voltage = double.Parse(data_recv.Trim());
            
            send_cmd(":MEAS:CURR?");
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
                LogHelper.Instance.Write("查询接收超时");
                return null;
            }
            result.ElectricCurrent = double.Parse(data_recv.Trim());
            return result;
        }

        public void stop()
        {
            if(serial.IsOpen)
            {
                serial.Close();
            }
        }

        private bool send_cmd(string cmd)
        {
            data_recv = "";
            if(serial.IsOpen)
            {
                try
                {
                    serial.Write(cmd + "\n");//添加结束符号0xa
                }
                catch(Exception ex)
                {
                    LogHelper.Instance.Write("PLD6003发送:"+cmd+"失败：" + ex.Message);
                    return false;
                }
                //LogHelper.Instance.Write("PLD6003串口已经发送：" + cmd);
                return true;
            }
            LogHelper.Instance.Write("PLD6003串口未打开，不能发送：" + cmd);
            return false;
        }

        public bool current_set(double val)
        {
            string cmd = ":CURR " + val;
            return send_cmd(cmd);
        }

        public bool output(bool on_off)
        {
            if(on_off)
            {
                return send_cmd(":OUTP ON");
            }
            else
            {
                return send_cmd(":OUTP OFF");
            }
        }

       

        public bool voltage_set(double val)
        {
            string cmd = ":VOLT " + val;
            return send_cmd(cmd);
        }
    }
}
