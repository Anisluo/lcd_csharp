using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LCD_V2.Views
{
    /// <summary>
    /// A saved motion-platform profile: algorithm + per-axis config for X/Y/Z/U/V.
    /// </summary>
    public sealed class MotionProfile
    {
        public string Name      { get; set; } = "未命名";
        public string Algorithm { get; set; } = "Type D (直动)";
        public List<AxisConfig> Axes { get; set; } = new List<AxisConfig>();

        [XmlIgnore]
        public int EnabledAxisCount => Axes?.Count(a => a.Enabled) ?? 0;

        [XmlIgnore]
        public string Summary => $"{Algorithm} · {EnabledAxisCount} 轴启用";
    }

    /// <summary>Per-axis parameters (X/Y/Z are linear mm; U/V are rotational degrees).</summary>
    public sealed class AxisConfig
    {
        public string AxisName    { get; set; } = "X";
        public bool   Enabled     { get; set; } = true;

        public double HighSpeed   { get; set; } = 100;
        public double MidSpeed    { get; set; } = 50;
        public double LowSpeed    { get; set; } = 10;

        public double AccelTimeMs { get; set; } = 100;
        public double PulseUnit   { get; set; } = 0.001;   // mm or ° per pulse
        public bool   Invert      { get; set; } = false;
    }

    /// <summary>Static catalog: the 5 fixed axis names + the list of motion algorithms.</summary>
    public static class MotionCatalog
    {
        public static readonly string[] AxisNames = { "X", "Y", "Z", "U", "V" };

        /// <summary>
        /// Motion / compensation algorithms, mirrors LCD V1's EquipmentType enum.
        /// Displayed name -> internal key is lossless because we save the display string directly.
        /// </summary>
        public static readonly string[] Algorithms =
        {
            "Type D (直动 · 无补偿)",
            "Type A (中表 · 旋转后补偿)",
            "Type B (下表 · 下置旋转台)",
            "Type C (边表 · 边置旋转台)",
            "Type E (边表 · 变种)",
        };

        /// <summary>Returns a profile populated with 5 enabled axes at default speeds.</summary>
        public static List<AxisConfig> NewDefaultAxes()
        {
            return new List<AxisConfig>
            {
                new AxisConfig { AxisName = "X", Enabled = true, HighSpeed = 100, MidSpeed = 50, LowSpeed = 10, AccelTimeMs = 100, PulseUnit = 0.001 },
                new AxisConfig { AxisName = "Y", Enabled = true, HighSpeed = 100, MidSpeed = 50, LowSpeed = 10, AccelTimeMs = 100, PulseUnit = 0.001 },
                new AxisConfig { AxisName = "Z", Enabled = true, HighSpeed =  50, MidSpeed = 25, LowSpeed =  5, AccelTimeMs = 100, PulseUnit = 0.001 },
                new AxisConfig { AxisName = "U", Enabled = true, HighSpeed =  30, MidSpeed = 15, LowSpeed =  3, AccelTimeMs = 150, PulseUnit = 0.01  },
                new AxisConfig { AxisName = "V", Enabled = true, HighSpeed =  30, MidSpeed = 15, LowSpeed =  3, AccelTimeMs = 150, PulseUnit = 0.01  },
            };
        }
    }
}
