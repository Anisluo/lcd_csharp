using LCD.Core.Models;

namespace LCD.Core.Abstractions
{
    /// <summary>
    /// 可编程电源抽象。
    /// 对应 LCD.Drv 里的实现：PowerM88 / PowerDC / PowerNGI36150 / PowerPld6003。
    /// 对应已有（待迁移）的 LCD.Ctrl.IPowerDevice。
    /// </summary>
    public interface IPowerSupply
    {
        bool IsOpen { get; }

        bool Open(string portName);

        void Close();

        bool SetOutput(bool on);

        bool SetVoltage(double volts);

        bool SetCurrent(double amps);

        /// <summary>读取当前电压 / 电流。</summary>
        PowerReading Query();
    }
}
