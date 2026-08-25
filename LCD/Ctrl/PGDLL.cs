using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LCD.Ctrl
{
    public class PGDLL
    {

        [DllImport(@"NC816Sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NC816NetSdk(int ID,int timeout);

        [DllImport(@"NC816Sdk.dll",EntryPoint = "getSerialNumber", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getSerialNumber();

        [DllImport(@"NC816Sdk.dll",EntryPoint = "getSoftwareVersion", CallingConvention = CallingConvention.StdCall)]
        public static extern double getSoftwareVersion();

        [DllImport(@"NC816Sdk.dll",EntryPoint = "getDeviceIPAddress", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getDeviceIPAddress();
        [DllImport(@"NC816Sdk.dll",EntryPoint = "getDeviceIPGateway", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getDeviceIPGateway();

        /// <summary>
        /// 连接   string getSerialNumber(void);
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        [DllImport(@"NC816Sdk.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int connectDevice(string addr);
        /// <summary>
        ///  RGB 信号控制
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        [DllImport(@"NC816Sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rgbControl(string addr);
        /// <summary>
        /// #r, #g, #b 红绿蓝三色颜色值
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [DllImport(@"NC816Sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool colorControlD(byte r, byte g, byte b);
        /// <summary>
        ///  切换图片
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport(@"NC816Sdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool changePattern(string name);
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [DllImport(@"NC816Sdk.dll", EntryPoint = "getTimingPatternList", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getTimingPatternList(IntPtr Name, [In,Out] ref PatternList pattern);//getTimingPatternList


        [DllImport(@"NC816Sdk.dll", EntryPoint = "getCurrentTiming", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getCurrentTiming();

        //[DllImport(@"NC816Sdk.dll", EntryPoint = "changePattern", CallingConvention = CallingConvention.Cdecl)]
        //public static extern bool changePattern(string Name);

        //[DllImport(@"NC816Sdk.dll", EntryPoint = "getCurrentTiming", CallingConvention = CallingConvention.Cdecl)]
        //public static extern IntPtr getCurrentTiming();

    }
    [StructLayout(LayoutKind.Sequential)]
    public struct PatternList
    {
        public int Size;
        [MarshalAsAttribute(UnmanagedType.ByValArray,SizeConst = 1024)]
        public str[] ItemStrings;
    }

    public struct str
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name;
    }

}
