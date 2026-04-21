using System.Collections.Generic;

namespace LCD.Core.Abstractions
{
    /// <summary>
    /// PG (Pattern Generator) 抽象：信号发生器，给 LCD 屏输出测试图案。
    /// 对应 LCD.Drv 里的实现：PGNet / PGDLL / PGP（分别走网口/DLL/并口）。
    /// </summary>
    public interface IPatternGenerator
    {
        /// <summary>连接/初始化 PG 设备。IP 可为 IP 地址或串口名，取决于总线类型。</summary>
        int Initialize(string address);

        /// <summary>查询设备信息（型号 / 固件版本等）。</summary>
        string GetDeviceInfo();

        /// <summary>刷新设备当前加载的 Pattern 列表。</summary>
        void RefreshPatternList();

        /// <summary>设备当前已知的 Pattern 名称列表（只读视图）。</summary>
        IReadOnlyList<string> PatternList { get; }

        /// <summary>切换到指定 Pattern。</summary>
        bool ChangePattern(string patternName);

        /// <summary>设置纯色 RGB 输出。</summary>
        bool SetColor(byte r, byte g, byte b);
    }
}
