using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LCD_V2.Views
{
    /// <summary>
    /// A saved metric profile: which instrument, which parameters we record,
    /// and the aggregation algorithm each parameter uses. Serialised to XML
    /// by MetricStore — parameterless ctor + plain props required.
    /// </summary>
    public sealed class InstrumentMetric
    {
        public string Name       { get; set; } = "未命名";
        public string Instrument { get; set; } = "BMA7";
        public List<MetricParam> Params { get; set; } = new List<MetricParam>();

        [XmlIgnore]
        public int EnabledCount => Params?.Count(p => p.Enabled) ?? 0;

        [XmlIgnore]
        public string Summary => $"{Instrument} · 启用 {EnabledCount} 参数";
    }

    /// <summary>Per-parameter row: the parameter name, whether to record it, and how to aggregate.</summary>
    public sealed class MetricParam
    {
        public string ParamName { get; set; } = "";
        public bool   Enabled   { get; set; }
        public string Algorithm { get; set; } = "均值";
    }

    /// <summary>Catalog of known measurable parameters (fixed schema, UI shows the same list every time).</summary>
    public static class MetricCatalog
    {
        // ordered for consistent UI — common luminance/chromaticity first, tristimulus, CCT/Δuv last
        public static readonly string[] AllParams =
        {
            "L (亮度)",
            "Cx (色坐标)",
            "Cy (色坐标)",
            "u' (色坐标)",
            "v' (色坐标)",
            "X (三刺激值)",
            "Y (三刺激值)",
            "Z (三刺激值)",
            "CCT (色温)",
            "Duv",
            "λp (峰值波长)",
            "λd (主波长)",
        };

        public static readonly string[] Algorithms =
        {
            "均值",
            "最大值",
            "最小值",
            "标准差",
            "中位数",
            "极差",
            "首次采样",
            "末次采样",
        };

        public static readonly string[] Instruments =
        {
            "BMA7",
            "BM5A",
            "BM5AS",
            "PR655",
            "CS2000",
            "SR3A",
            "SR5A",
            "MS01",
            "USB2000",
            "Admesy",
            "Demo",
        };
    }
}
