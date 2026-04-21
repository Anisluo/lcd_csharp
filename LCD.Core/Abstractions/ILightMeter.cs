using LCD.Core.Models;

namespace LCD.Core.Abstractions
{
    /// <summary>
    /// 光度计 / 色度计抽象。
    /// 对应 LCD.Drv 里的实现：BM7A / BM5A / BM5AS / CS2000 / SR3A / MS01 / Admesy / USB2000 / PR655。
    /// 当前实现继承自 LCD.Ctrl.TestMachine（待迁移到 LCD.Drv 时一并实现本接口）。
    /// </summary>
    public interface ILightMeter
    {
        /// <summary>仪器型号标识，用于 UI 选择。</summary>
        string Key { get; }

        bool IsOpen { get; }

        /// <summary>初始化仪器（打开串口/上电/校准等）。</summary>
        void Init();

        void Close();

        /// <summary>停止一次正在进行的测量。</summary>
        void StopTest();

        /// <summary>测色度（Lv + x, y 或 X, Y, Z）。</summary>
        ColorimetricReading MeasureLxy();

        /// <summary>测光谱（返回 Spectrum / SpectrumStartNm / SpectrumStepNm）。</summary>
        ColorimetricReading MeasureSpectrum();

        /// <summary>通用 key-value 设置（曝光/积分时间/滤光片等）。</summary>
        bool Set(string key, string value);
    }
}
