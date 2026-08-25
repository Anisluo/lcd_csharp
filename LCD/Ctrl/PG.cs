using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace LCD.Ctrl
{
    public class PG
    {
        public PatternList PatternList { get; set; }
        public int BusType { get; set; }

        public PG(int BusType)
        {
            this.BusType = BusType;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public int init(string Ip)
        {
            return LCD.Ctrl.PGDLL.connectDevice(Ip);
        }
        /// <summary>
        /// 查询设备信息
        /// </summary>
        /// <returns></returns>
        public String getDeviceInformaction()
        {
            double A= LCD.Ctrl.PGDLL.getSoftwareVersion();
            IntPtr B = LCD.Ctrl.PGDLL.getSerialNumber();
            string BB = Marshal.PtrToStringAnsi(B);
            IntPtr C = LCD.Ctrl.PGDLL.getDeviceIPAddress();
            string CC = Marshal.PtrToStringAnsi(C);
            IntPtr D = LCD.Ctrl.PGDLL.getDeviceIPGateway();
            string DD = Marshal.PtrToStringAnsi(D);


            return $"Software version: {A}" + $"Device serial number: {BB}"+ $"IP Address:{CC}"+ $"Gateway address:{DD}";


        }
        /// <summary>
        /// 获取图片list
        /// </summary>
        public void getDevicePatternList()
        {
            IntPtr _TimingName = LCD.Ctrl.PGDLL.getCurrentTiming();
            string TimingName = Marshal.PtrToStringAnsi(_TimingName);
      
            PatternList pattern = new PatternList();
            try
            {
                int error = LCD.Ctrl.PGDLL.getTimingPatternList(_TimingName, ref pattern);
            }
            catch (Exception)
            {
            }
            
            PatternList = pattern;
            //string[] strs=new string[pattern.Size];
            //for (int i = 0; i < pattern.Size; i++)
            //{
            //    strs[i] = string.Concat(pattern.ItemStrings[i]);

            //}
            ////Marshal.PtrToStructure(intPtr, pattern);
            ////Console.WriteLine(pattern.Size);
            //return strs;
        }
        /// <summary>
        /// 切换图片
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool changePattern(string Name)
        {
            return LCD.Ctrl.PGDLL.changePattern(Name);
        }

        public bool colorControl(byte r, byte g, byte b)
        {
            return LCD.Ctrl.PGDLL.colorControlD( r,  g,  b);
        }
}
}
