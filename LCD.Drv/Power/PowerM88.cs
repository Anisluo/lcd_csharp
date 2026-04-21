using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using LCD.Core.Abstractions;
using LCD.Core.Models;
using VisionCore;

namespace LCD.Drv.Power
{
    /// <summary>
    /// M88 可编程电源（串口 9600-8-N-1，RTS 使能）。
    /// 由 LCD/Ctrl/PowerM88.cs 迁移而来，实现 <see cref="IPowerSupply"/>。
    /// </summary>
    public sealed class PowerM88 : IPowerSupply
    {
        private readonly SerialPort serial = new SerialPort();
        private string dataRecv = "";

        public bool IsOpen => serial.IsOpen;

        public bool Open(string portName)
        {
            serial.PortName = portName;
            serial.BaudRate = 9600;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.RtsEnable = true; // 必须使能，否则收到乱码
            serial.DataReceived -= OnSerialDataReceived;
            serial.DataReceived += OnSerialDataReceived;
            try
            {
                serial.Open();
            }
            catch (Exception ex)
            {
                Log.Error("M88 串口打开失败：" + ex.Message);
                return false;
            }
            return true;
        }

        public void Close()
        {
            if (serial.IsOpen) serial.Close();
        }

        public bool SetOutput(bool on) =>
            SendCmd(on ? "OUTP 1" : "OUTP 0");

        public bool SetVoltage(double volts) =>
            SendCmd("VOLT " + volts);

        public bool SetCurrent(double amps) =>
            SendCmd("CURR " + amps);

        public PowerReading Query()
        {
            dataRecv = "";
            SendCmd("MEAS:VCM?");

            if (!WaitForNewline(timeoutTicks: 20))
            {
                Log.Warn("M88 查询接收超时");
                return null;
            }

            string[] parts = dataRecv.Trim().Split(',');
            if (parts.Length < 3)
            {
                Log.Warn("M88 查询返回格式错误：" + dataRecv);
                return null;
            }

            var reading = new PowerReading
            {
                Voltage = double.Parse(parts[0]),
            };

            SendCmd("MEAS:CURR?");
            if (!WaitForNewline(timeoutTicks: 20))
            {
                Log.Warn("M88 查询电流接收超时");
                return null;
            }
            reading.Current = double.Parse(dataRecv.Trim());

            return reading;
        }

        private bool WaitForNewline(int timeoutTicks)
        {
            for (int i = 0; i < timeoutTicks; i++)
            {
                if (dataRecv.Contains("\n")) return true;
                Thread.Sleep(100);
            }
            return false;
        }

        private bool SendCmd(string cmd)
        {
            dataRecv = "";
            if (!serial.IsOpen)
            {
                Log.Warn("M88 串口未打开，不能发送：" + cmd);
                return false;
            }
            try
            {
                serial.Write(cmd + "\n");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("M88 串口发送 " + cmd + " 失败：" + ex.Message);
                return false;
            }
        }

        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int len = serial.BytesToRead;
            byte[] buf = new byte[len];
            serial.Read(buf, 0, buf.Length);
            // 过滤非可见字符
            byte[] printable = buf.Where(b => b < 127).ToArray();
            dataRecv += Encoding.ASCII.GetString(printable);
        }
    }
}
