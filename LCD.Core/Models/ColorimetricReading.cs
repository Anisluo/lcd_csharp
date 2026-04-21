namespace LCD.Core.Models
{
    /// <summary>
    /// 光度计/色度计的一次测量读数：
    ///   - XYZ 三刺激值
    ///   - Lv + (x, y) 色坐标
    ///   - Spectrum: 可选的光谱数据（波长采样数组）
    /// 取代 LCD.Data.IData 在驱动接口边界处的使用；
    /// IData 仍可在 UI/业务层继续保留直到 Phase 3 迁移。
    /// </summary>
    public sealed class ColorimetricReading
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Lv { get; set; }
        public double x { get; set; }
        public double y { get; set; }

        public double[] Spectrum { get; set; }

        public double SpectrumStartNm { get; set; }
        public double SpectrumStepNm { get; set; }
    }
}
