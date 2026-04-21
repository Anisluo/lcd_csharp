namespace LCD.Core.Models
{
    /// <summary>
    /// 电源读数：电压/电流/功率 快照。
    /// 取代 LCD.Ctrl.Result (Power.cs)。
    /// </summary>
    public sealed class PowerReading
    {
        public double Voltage { get; set; }
        public double Current { get; set; }
        public double Power { get; set; }
    }
}
