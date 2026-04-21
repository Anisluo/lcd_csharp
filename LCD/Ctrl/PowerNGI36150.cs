using Org.BouncyCastle.Asn1.X500;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LCD.Ctrl
{
    internal class PowerNGI36150 : IPowerDevice
    {
        private string data_recv = "";
        private SerialPort serial;
        public PowerNGI36150() 
        {
            serial = new SerialPort();
        }

        public bool start(string portname)
        {
            serial.PortName = portname;
            serial.BaudRate = 115200;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.DataReceived += Serial_DataReceived;
            try
            {
                serial.Open();
            }
            catch (Exception ex)
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
            send_cmd("MEASure:VOLTage?");
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
            string vol = data_recv.Trim();            
            send_cmd("MEASure:CURRent?");
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
            string current = data_recv.Trim();
            Result result = new Result();
            result.Voltage = double.Parse(vol);
            result.ElectricCurrent = double.Parse(current);
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
                    LogHelper.Instance.Write("NGI36150串口发送:" + cmd + "失败：" + ex.Message);
                    return false;
                }
                //Project.WriteLog("NGI36150已经发送：" + cmd);
                return true;
            }
            LogHelper.Instance.Write("NGI36150串口未打开，不能发送：" + cmd);
            return false;
        }

        public bool current_set(double val)
        {
            string cmd = "SOURce:CURRent " + val;
            return send_cmd(cmd);
        }

        public bool output(bool on_off)
        {
            if (on_off)
            {
                return send_cmd("OUTPut:ONOFF 1");
            }
            else
            {
                return send_cmd("OUTPut:ONOFF 0");
            }
        }

        public bool voltage_set(double val)
        {
            string cmd = "SOURce:VOLTage " + val;
            return send_cmd(cmd);
        }
    }
}
