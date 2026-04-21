using System;
using System.IO.Ports;
using System.Threading;
using LCD.Core.Abstractions;
using LCD.Core.Models;
using VisionCore;

namespace LCD.Drv.Power
{
    /// <summary>
    /// Rohde &amp; Schwarz / NGI36150 可编程电源（串口 115200-8-N-1）。
    /// 由 LCD/Ctrl/PowerNGI36150.cs 迁移而来。
    /// </summary>
    public sealed class PowerNGI36150 : IPowerSupply
    {
        private readonly SerialPort serial = new SerialPort();
        private string dataRecv = "";

        public bool IsOpen => serial.IsOpen;

        public bool Open(string portName)
        {
            serial.PortName = portName;
            serial.BaudRate = 115200;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.DataReceived -= OnSerialDataReceived;
            serial.DataReceived += OnSerialDataReceived;
            try
            {
                serial.Open();
            }
            catch (Exception ex)
            {
                Log.Error("NGI36150 串口打开失败：" + ex.Message);
                return false;
            }
            return true;
        }

        public void Close()
        {
            if (serial.IsOpen) serial.Close();
        }

        public bool SetOutput(bool on) =>
            SendCmd(on ? "OUTPut:ONOFF 1" : "OUTPut:ONOFF 0");

        public bool SetVoltage(double volts) =>
            SendCmd("SOURce:VOLTage " + volts);

        public bool SetCurrent(double amps) =>
            SendCmd("SOURce:CURRent " + amps);

        public PowerReading Query()
        {
            dataRecv = "";
            SendCmd("MEASure:VOLTage?");
            if (!WaitForNewline(20))
            {
                Log.Warn("NGI36150 查询电压接收超时");
                return null;
            }
            string voltStr = dataRecv.Trim();

            SendCmd("MEASure:CURRent?");
            if (!WaitForNewline(20))
            {
                Log.Warn("NGI36150 查询电流接收超时");
                return null;
            }
            string currStr = dataRecv.Trim();

            return new PowerReading
            {
                Voltage = double.Parse(voltStr),
                Current = double.Parse(currStr),
            };
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
                Log.Warn("NGI36150 串口未打开，不能发送：" + cmd);
                return false;
            }
            try
            {
                serial.Write(cmd + "\n");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("NGI36150 串口发送 " + cmd + " 失败：" + ex.Message);
                return false;
            }
        }

        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            dataRecv += serial.ReadExisting();
        }
    }
}
