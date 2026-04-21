using System;
using System.Runtime.InteropServices;

namespace LCD.Drv.PG
{
    /// <summary>
    /// P/Invoke declarations for the NC816 Pattern Generator SDK (NC816Sdk.dll).
    /// Native DLL must be reachable at runtime (loaded from the process working dir
    /// or PATH). The file currently lives at LCD/Dll/NC816Sdk.dll.
    /// </summary>
    internal static class PGDLL
    {
        [DllImport("NC816Sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NC816NetSdk(int id, int timeout);

        [DllImport("NC816Sdk.dll", EntryPoint = "getSerialNumber", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetSerialNumber();

        [DllImport("NC816Sdk.dll", EntryPoint = "getSoftwareVersion", CallingConvention = CallingConvention.StdCall)]
        public static extern double GetSoftwareVersion();

        [DllImport("NC816Sdk.dll", EntryPoint = "getDeviceIPAddress", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetDeviceIPAddress();

        [DllImport("NC816Sdk.dll", EntryPoint = "getDeviceIPGateway", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetDeviceIPGateway();

        [DllImport("NC816Sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int connectDevice(string addr);

        [DllImport("NC816Sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rgbControl(string addr);

        [DllImport("NC816Sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool colorControlD(byte r, byte g, byte b);

        [DllImport("NC816Sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool changePattern(string name);

        [DllImport("NC816Sdk.dll", EntryPoint = "getTimingPatternList", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTimingPatternList(IntPtr name, [In, Out] ref NativePatternList pattern);

        [DllImport("NC816Sdk.dll", EntryPoint = "getCurrentTiming", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetCurrentTiming();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativePatternList
    {
        public int Size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public NativePatternName[] ItemStrings;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativePatternName
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Name;
    }
}
