namespace LCD.Core.Models
{
    /// <summary>
    /// 串口通讯配置 POCO —— 光度计/电源等串口设备 Init() 的输入。
    /// 把驱动与 LCD.Data.Project.cfg 的全局静态耦合解开：
    /// 调用方在构造驱动后、调用 Init() 前设置 Config 即可。
    /// </summary>
    public sealed class SerialBusConfig
    {
        public string ComName { get; set; }

        public int BaudRate { get; set; } = 9600;

        public int DataBits { get; set; } = 8;

        /// <summary>0=None / 1=One / 2=Two（与旧 LCD.Data.ComDevice.stopBit 编码兼容）</summary>
        public int StopBits { get; set; } = 1;

        /// <summary>0=None / 1=Odd / 2=Even（与旧 LCD.Data.ComDevice.parity 编码兼容）</summary>
        public int Parity { get; set; } = 0;
    }
}
