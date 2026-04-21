using System;
using System.IO.Ports;
using System.Threading;
using LCD.Core.Abstractions;
using LCD.Core.Models;
using VisionCore;

namespace LCD.Drv.Power
{
    /// <summary>
    /// PLD6003 可编程电源（串口 9600-8-N-1）。
    /// 由 LCD/Ctrl/PowerPld6003.cs 迁移而来。
    /// </summary>
    public sealed class PowerPld6003 : IPowerSupply
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
            serial.DataReceived -= OnSerialDataReceived;
            serial.DataReceived += OnSerialDataReceived;
            try
            {
                serial.Open();
            }
            catch (Exception ex)
            {
                Log.Error("PLD6003 串口打开失败：" + ex.Message);
                return false;
            }
            return true;
        }

        public void Close()
        {
            if (serial.IsOpen) serial.Close();
        }

        public bool SetOutput(bool on) =>
            SendCmd(on ? ":OUTP ON" : ":OUTP OFF");

        public bool SetVoltage(double volts) =>
            SendCmd(":VOLT " + volts);

        public bool SetCurrent(double amps) =>
            SendCmd(":CURR " + amps);

        public PowerReading Query()
        {
            dataRecv = "";
            SendCmd(":MEAS:VOLT?");
            if (!WaitForNewline(20))
            {
                Log.Warn("PLD6003 查询电压接收超时");
                return null;
            }
            var reading = new PowerReading
            {
                Voltage = double.Parse(dataRecv.Trim()),
            };

            SendCmd(":MEAS:CURR?");
            if (!WaitForNewline(20))
            {
                Log.Warn("PLD6003 查询电流接收超时");
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
                Log.Warn("PLD6003 串口未打开，不能发送：" + cmd);
                return false;
            }
            try
            {
                serial.Write(cmd + "\n");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("PLD6003 串口发送 " + cmd + " 失败：" + ex.Message);
                return false;
            }
        }

        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            dataRecv += serial.ReadExisting();
        }
    }
}
