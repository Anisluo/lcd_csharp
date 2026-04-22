using System.Collections.Generic;
using System.Xml.Serialization;

namespace LCD_V2.Views
{
    /// <summary>
    /// Per-instrument tuning (correction factors, timing delays, serial settings).
    /// Modelled after the LCD V1 DevicesView parameter strip — same fields, same units.
    /// Persisted per-instrument by InstrumentConfigStore.
    /// </summary>
    public sealed class InstrumentConfig
    {
        public string Instrument { get; set; } = "BMA7";

        // 校正系数
        public double CorrLum { get; set; } = 1.0;
        public double CorrCx  { get; set; } = 0.0;
        public double CorrCy  { get; set; } = 0.0;

        // 测量延时 (ms) — startup / per-measurement / reset
        public int StartDelayMs   { get; set; } = 2000;
        public int MeasureDelayMs { get; set; } = 500;
        public int ResetDelayMs   { get; set; } = 1000;
        // 积分时间 (ms) — spectral instruments only
        public int IntegrationMs  { get; set; } = 100;

        // 串口
        public string ComPort  { get; set; } = "COM1";
        public int    BaudRate { get; set; } = 9600;
        public int    DataBits { get; set; } = 8;
        public int    StopBits { get; set; } = 1;
        public string Parity   { get; set; } = "None";
    }

    /// <summary>
    /// Known instrument → image + display name + whether it needs integration-time + whether it uses a serial port.
    /// Keeps the UI's per-instrument quirks out of the XAML.
    /// </summary>
    public static class InstrumentCatalog
    {
        public sealed class Info
        {
            public string Key;            // "BMA7"
            public string DisplayName;    // "BM-7A"
            public string ImageUri;       // pack:// URI or null if no image
            public bool   UsesSerial;     // show serial config box
            public bool   UsesIntegration;// show integration-time field (spectral)
        }

        // ordered list matching MetricCatalog.Instruments
        public static readonly Info[] All =
        {
            new Info { Key = "BMA7",   DisplayName = "BM-7A",    ImageUri = "pack://application:,,,/Image/BM7.png",    UsesSerial = true,  UsesIntegration = false },
            new Info { Key = "BM5A",   DisplayName = "BM-5A",    ImageUri = "pack://application:,,,/Image/BM5A.png",   UsesSerial = true,  UsesIntegration = false },
            new Info { Key = "BM5AS",  DisplayName = "BM-5AS",   ImageUri = "pack://application:,,,/Image/BM5AS.png",  UsesSerial = true,  UsesIntegration = false },
            new Info { Key = "PR655",  DisplayName = "PR-655/670",ImageUri = "pack://application:,,,/Image/PR655.png",  UsesSerial = true,  UsesIntegration = false },
            new Info { Key = "CS2000", DisplayName = "CS-2000",  ImageUri = "pack://application:,,,/Image/CS2000.png", UsesSerial = true,  UsesIntegration = false },
            new Info { Key = "SR3A",   DisplayName = "SR-3A",    ImageUri = "pack://application:,,,/Image/SR3A.png",   UsesSerial = true,  UsesIntegration = true  },
            new Info { Key = "SR5A",   DisplayName = "SR-5A",    ImageUri = "pack://application:,,,/Image/SR3A.png",   UsesSerial = true,  UsesIntegration = true  },
            new Info { Key = "MS01",   DisplayName = "MS-01",    ImageUri = null,                                      UsesSerial = true,  UsesIntegration = true  },
            new Info { Key = "USB2000",DisplayName = "USB-2000", ImageUri = "pack://application:,,,/Image/CS200.png",  UsesSerial = false, UsesIntegration = true  },
            new Info { Key = "Admesy", DisplayName = "Admesy",   ImageUri = null,                                      UsesSerial = false, UsesIntegration = false },
            new Info { Key = "Demo",   DisplayName = "Demo",     ImageUri = "pack://application:,,,/Image/Demo.png",   UsesSerial = false, UsesIntegration = false },
        };

        public static Info Find(string key)
        {
            foreach (var i in All) if (i.Key == key) return i;
            return null;
        }
    }
}

namespace LCD_V2.Views
{
    /// <summary>Wrapper for XML round-trip (list + dictionary get messy in XmlSerializer).</summary>
    public sealed class InstrumentConfigBag
    {
        public List<InstrumentConfig> Items { get; set; } = new List<InstrumentConfig>();
    }
}
