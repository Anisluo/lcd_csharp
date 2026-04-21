using LCD.Core.Models;

namespace LCD.Data
{
    /// <summary>
    /// 把 LCD 里的 ComDevice（旧配置 DTO）转换成 LCD.Core 的 SerialBusConfig（驱动边界 POCO）。
    /// 用于把 Project.cfg.BM7A / CS2000 / SR3A / MS01 注入到 LCD.Drv 的驱动实例。
    /// </summary>
    public static class ComDeviceExtensions
    {
        public static SerialBusConfig ToBusConfig(this ComDevice dev)
        {
            if (dev == null) return null;

            int baud = 9600;
            int dataBits = 8;
            int.TryParse(dev.bardRateText, out baud);
            int.TryParse(dev.dataBitText, out dataBits);

            return new SerialBusConfig
            {
                ComName = dev.comName,
                BaudRate = baud,
                DataBits = dataBits,
                StopBits = dev.stopBit,
                Parity = dev.parity,
            };
        }
    }
}
