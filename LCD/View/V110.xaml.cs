using LCD.Dll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LCD.View
{
    /// <summary>
    /// V110.xaml 的交互逻辑
    /// </summary>
    public partial class V110 : UserControl
    {
        public static IntPtr parentHwnd;
        public static IntPtr[] DeviceHandle;
        private static IntPtr currHwnd;
        //MV public variables
        public static IntPtr CurDevice;
        //CamSet mycamset;
        bool bLRRev, bUDRev, IsShow;
        uint nStandard, nFrame, nDspMode, nCaptureFormatID;
        public static IntPtr font;

        public V110()
        {
            InitializeComponent();
            uint nBdNum = 0;
            try
            {
                nBdNum = MVAPI.MV_GetDeviceNumber();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            if (nBdNum <= 0)
            {
                return;
            }
            DeviceHandle = new IntPtr[nBdNum];
            //采集并显示
            for (uint i = 0; i < nBdNum; i++)
            {
                DeviceHandle[i] = MVAPI.MV_OpenDevice(i, false);
                MVAPI.MV_GetLastError(true);


                Console.WriteLine(DeviceHandle[i]);
            }

            CurDevice = DeviceHandle[0];


            currHwnd = PictureBoxDisplay.Handle;
            MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_WHND, (uint)currHwnd);//set the current version of the world 
            //设置左侧宽度
            //设置右侧宽度
            MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.GARB_IN_HEIGHT, 576);//GARB_IN_HEIGHT			= 30, 
            MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.GARB_IN_WIDTH, 768);//GARB_IN_WIDTH			= 31, 					
            MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.GARB_HEIGHT, 576);//GARB_HEIGHT				= 28, 
            MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.GARB_WIDTH, 768);//GARB_WIDTH				= 29, 
            MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_TOP, 0);//DISP_HEIGHT				= 10, 
            MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_LEFT, 0);//DISP_HEIGHT				= 10, 

            MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_HEIGHT, 576);//DISP_HEIGHT				= 10, 
            MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_WIDTH, 768);//DISP_WIDTH				= 11,

        

            ////连续采集参数
            //Seqno = 1; nSavType = 0; nQuality = 100; bColor = true; nFColor = 0; strFileName = "f:\\test";

            ////OSD相关参数
            //nOSDMode = 0; nOSDXStrt = 0; nOSDYStrt = 0; nOSDWdth = 216; nOSDHght = 38;

            ////
            //LineIndex = 0; CircleIndex = 0; FileIndex = 0; nGraphType = -1;

            //bIsOneField = false; bIsReadData = false; bIsStart = true; bCompress = false; btext = false;

            //btime = false; bdate = false; bline = false; brect = false; bcircle = false;

            MVAPI.MV_OperateDevice(CurDevice, (int)RUNOPER.MVRUN);   // MVRUN =1
            //font = Win32API.CreateFont(16,                        // nHeight
            //                        0,                         // nWidth
            //                        0,                         // nEscapement
            //                        0,                         // nOrientation
            //                        FW_BOLD,                 // nWeight
            //                        0,                     // bItalic
            //                        0,                     // bUnderline
            //                        0,                         // cStrikeOut
            //                        ANSI_CHARSET,              // nCharSet
            //                        OUT_DEFAULT_PRECIS,        // nOutPrecision
            //                        CLIP_DEFAULT_PRECIS,       // nClipPrecision
            //                        DEFAULT_QUALITY,           // nQuality
            //                        FIXED_PITCH | FF_SWISS,  // nPitchAndFamily
            //                        "Arial");                 // lpszFacename)

            //MVAPI.MVD_InitDisplay(0, parentHwnd, 768, 576, (int)DISPCOLORFMT.DISP_RGB32, 0);
            //bool call = MVAPI.MV_SetCallBack(CurDevice, CallBack.callback, this.Handle, (int)CALLBACKTYPE.BEFORE_PROCESS);

        }
    }


}
