using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

    /// <summary>
    /// Per-axis parameters. One row per logical axis X/Y/Z/U/V.
    /// <see cref="AxisCode"/> is the primary motor channel driving this axis (e.g. "轴1").
    /// When <see cref="Interpolate"/> is true, <see cref="InterpolateCode"/> may hold a
    /// second motor channel (e.g. "轴2") to form a gantry dual-drive — the UI makes it
    /// editable only while Interpolate is checked, which is why this class raises
    /// PropertyChanged on Interpolate.
    /// </summary>
    public sealed class AxisConfig : INotifyPropertyChanged
    {
        private string _axisName        = "X";
        private string _axisCode        = "";
        private string _interpolateCode = "";
        private bool   _enabled         = true;
        private double _highSpeed       = 100;
        private double _midSpeed        = 50;
        private double _lowSpeed        = 10;
        private double _accelTimeMs     = 100;
        private double _pulseUnit       = 0.001;
        private bool   _invert          = false;
        private bool   _interpolate     = false;

        /// <summary>Logical axis name: X/Y/Z/U/V (row identity, not edited in the grid).</summary>
        public string AxisName        { get => _axisName;        set => Set(ref _axisName, value); }

        /// <summary>Primary motor-channel code (e.g. "轴1").</summary>
        public string AxisCode        { get => _axisCode;        set => Set(ref _axisCode, value); }

        /// <summary>Secondary motor-channel code for gantry dual-drive (e.g. "轴2"); only meaningful when Interpolate=true.</summary>
        public string InterpolateCode { get => _interpolateCode; set => Set(ref _interpolateCode, value); }

        public bool   Enabled         { get => _enabled;         set => Set(ref _enabled, value); }

        public double HighSpeed       { get => _highSpeed;       set => Set(ref _highSpeed, value); }
        public double MidSpeed        { get => _midSpeed;        set => Set(ref _midSpeed, value); }
        public double LowSpeed        { get => _lowSpeed;        set => Set(ref _lowSpeed, value); }

        public double AccelTimeMs     { get => _accelTimeMs;     set => Set(ref _accelTimeMs, value); }
        public double PulseUnit       { get => _pulseUnit;       set => Set(ref _pulseUnit, value); }   // mm or ° per pulse
        public bool   Invert          { get => _invert;          set => Set(ref _invert, value); }

        /// <summary>
        /// Whether this axis participates in coordinated / vector interpolation motion
        /// (LCD V1's Axies.IsSecondValue). Raising PropertyChanged on this is what makes
        /// the InterpolateCode cell's IsEnabled binding flip live when the checkbox toggles.
        /// </summary>
        public bool   Interpolate     { get => _interpolate;     set => Set(ref _interpolate, value); }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
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
                // Default primary mapping — X→1, Y→2, Z→3, U→4, V→5 (pure numeric channel ids).
                // For gantry dual-drive on X the operator enables Interpolate on the X row
                // and enters the secondary motor code (e.g. "2") in 插补轴代号.
                new AxisConfig { AxisName = "X", AxisCode = "1", Enabled = true, HighSpeed = 100, MidSpeed = 50, LowSpeed = 10, AccelTimeMs = 100, PulseUnit = 0.001, Interpolate = true  },
                new AxisConfig { AxisName = "Y", AxisCode = "2", Enabled = true, HighSpeed = 100, MidSpeed = 50, LowSpeed = 10, AccelTimeMs = 100, PulseUnit = 0.001, Interpolate = true  },
                new AxisConfig { AxisName = "Z", AxisCode = "3", Enabled = true, HighSpeed =  50, MidSpeed = 25, LowSpeed =  5, AccelTimeMs = 100, PulseUnit = 0.001, Interpolate = false },
                new AxisConfig { AxisName = "U", AxisCode = "4", Enabled = true, HighSpeed =  30, MidSpeed = 15, LowSpeed =  3, AccelTimeMs = 150, PulseUnit = 0.01,  Interpolate = false },
                new AxisConfig { AxisName = "V", AxisCode = "5", Enabled = true, HighSpeed =  30, MidSpeed = 15, LowSpeed =  3, AccelTimeMs = 150, PulseUnit = 0.01,  Interpolate = false },
            };
        }
    }
}
