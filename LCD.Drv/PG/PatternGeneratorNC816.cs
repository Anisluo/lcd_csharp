using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LCD.Core.Abstractions;
using VisionCore;

namespace LCD.Drv.PG
{
    /// <summary>
    /// NC816 系列信号发生器 (Pattern Generator) 驱动。
    /// 迁移自 LCD/Ctrl/PG.cs，实现 <see cref="IPatternGenerator"/>。
    /// 底层走 NC816Sdk.dll 原生调用，见 <see cref="PGDLL"/>。
    /// </summary>
    public sealed class PatternGeneratorNC816 : IPatternGenerator
    {
        private NativePatternList nativeList;
        private List<string> patterns = new List<string>();

        public IReadOnlyList<string> PatternList => patterns;

        public int Initialize(string address)
        {
            return PGDLL.connectDevice(address);
        }

        public string GetDeviceInfo()
        {
            double version = PGDLL.GetSoftwareVersion();
            string serial  = Marshal.PtrToStringAnsi(PGDLL.GetSerialNumber()) ?? "";
            string ip      = Marshal.PtrToStringAnsi(PGDLL.GetDeviceIPAddress()) ?? "";
            string gateway = Marshal.PtrToStringAnsi(PGDLL.GetDeviceIPGateway()) ?? "";

            return $"Software version: {version}"
                 + $"Device serial number: {serial}"
                 + $"IP Address:{ip}"
                 + $"Gateway address:{gateway}";
        }

        public void RefreshPatternList()
        {
            IntPtr timingHandle = PGDLL.GetCurrentTiming();
            nativeList = default(NativePatternList);

            try
            {
                PGDLL.GetTimingPatternList(timingHandle, ref nativeList);
            }
            catch (Exception ex)
            {
                Log.Error("PG 读取 Pattern 列表失败：" + ex.Message);
                return;
            }

            // Project the marshaled struct into a managed string list
            var items = new List<string>(nativeList.Size);
            if (nativeList.ItemStrings != null)
            {
                for (int i = 0; i < nativeList.Size && i < nativeList.ItemStrings.Length; i++)
                {
                    items.Add(nativeList.ItemStrings[i].Name ?? "");
                }
            }
            patterns = items;
        }

        public bool ChangePattern(string patternName)
        {
            return PGDLL.changePattern(patternName);
        }

        public bool SetColor(byte r, byte g, byte b)
        {
            return PGDLL.colorControlD(r, g, b);
        }
    }
}
