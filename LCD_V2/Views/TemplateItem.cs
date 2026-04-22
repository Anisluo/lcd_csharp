using System.Xml.Serialization;
using LCD.Core.Services;

namespace LCD_V2.Views
{
    /// <summary>
    /// A saved template instance in the library — the *result* of applying a
    /// generator config (5/9/13/...) to concrete product dimensions + params.
    /// Serialised to XML via TemplateStore; must have a parameterless ctor and
    /// [XmlIgnore] on any computed property.
    /// </summary>
    public sealed class TemplateItem
    {
        public string Name { get; set; } = "未命名";
        public PointLayoutType ConfigType { get; set; }
        public double H { get; set; }
        public double V { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }
        public bool   UseMm { get; set; }
        public int    PointCount { get; set; }

        [XmlIgnore]
        public string ConfigTypeLabel
        {
            get
            {
                switch (ConfigType)
                {
                    case PointLayoutType.Point5:      return "5 点";
                    case PointLayoutType.Point9:      return "9 点";
                    case PointLayoutType.Point13:     return "13 点";
                    case PointLayoutType.Point13Diag: return "13 点对角";
                    case PointLayoutType.Point17:     return "17 点";
                    default:                           return ConfigType.ToString();
                }
            }
        }

        [XmlIgnore]
        public string Summary => $"{ConfigTypeLabel} · {H:0}×{V:0} mm";
    }
}
