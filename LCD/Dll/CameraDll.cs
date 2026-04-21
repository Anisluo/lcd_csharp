//定义动态库调用dll
//调用非托管动态库
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LCD.Dll
{
    /********************************/
    //相机上绘制线条
    enum GRAPHMODE
    {
        GRAPH_STRING=0,
        GRAPH_LINE=1,
        GRAPH_CIRCLE=2
    }

    enum DISPCOLORFMT
    {
        DISP_Y8=0,
        DISP_RGB15=1,
        DISP_RGB16=2,
        DISP_RGB24=3,
        DISP_RGB32=4
    }


    /**************************************/
    //相机板卡运行信息
    enum RUNOPER
    {
        MVSTOP=0,//MVSTOP停止工作
        MVRUN=1,//MVRUN 卡开始工作
        MVPAUSE=2,//MVPAUSE 暂停卡的工作
        MVQUERYSTATU=3,//MVQUERYSTATU:查询卡的当前状态
        MVERROR=4//MVERROR:为错误的状态
    }





    /*********************************************************/
    /**************************板卡参数************************/
    enum MV_PARAMTER
    {
        GET_BOARD_TYPE = 0, GET_GRAPHICAL_INTERFACE = 1,
        SET_GARBIMAGEINFO = 2, SET_DISPIMAGEINFO = 3,
        BUFFERTYPE = 4, DEFAULT_PARAM = 5,

        // 控制显示的
        DISP_PRESENCE = 6, DISP_WHND = 7,
        DISP_TOP = 8, DISP_LEFT = 9,
        DISP_HEIGHT = 10, DISP_WIDTH = 11,

        // 控制A/D的调节参数
        ADJUST_STANDARD = 12, ADJUST_SOURCE = 13,
        ADJUST_CHANNEL = 14, ADJUST_LUMINANCE = 15,
        ADJUST_CHROMINANE = 16, ADJUST_SATURATION = 17,
        ADJUST_HUE = 18, ADJUST_CONTRAST = 19,

        //支持RGB卡
        ADJUST_R_LUM = 20, ADJUST_G_LUM = 21,
        ADJUST_B_LUM = 22, ADJUST_R_COARSE = 23,
        ADJUST_G_COARSE = 24, ADJUST_B_COARSE = 25,

        // 控制板卡的捕获参数
        GRAB_XOFF = 60, GRAB_YOFF = 61,
        GRAB_HEIGHT = 62, GRAB_WIDTH = 63,
        GRAB_IN_HEIGHT = 64, GRAB_IN_WIDTH = 65,
        GRAB_BITDESCRIBE = 66, GRAB_WHOLEWIDTH = 67,

        // 控制板卡的工作参数
        WORK_UPDOWN = 34, WORK_FLIP = 35,
        WORK_SKIP = 36, WORK_SYNC = 37,
        WORK_INTERLACE = 38, WORK_ISBLACK = 39,
        WORK_FIELD = 40, OSD_MODE = 41,

        //支持V500系列卡
        TENBIT_MODE = 42, OUTPUT_VIDEO = 43,
        FILERSELECT1 = 44, FILERSELECT2 = 45,

        // 控制板卡的捕获参数(保留,兼容老版本)
        GARB_XOFF = 26, GARB_YOFF = 27,
        GARB_HEIGHT = 28, GARB_WIDTH = 29,
        GARB_IN_HEIGHT = 30, GARB_IN_WIDTH = 31,
        GARB_BITDESCRIBE = 32, GARB_WHOLEWIDTH = 33,


        //shen add 
        //支持卡类型MVBOARD2.h中所有卡
        DISP_FLIP = 201, IMAGE_PROCESS = 202,
        VIDEO_SINGLE = 203, GET_BOARD_PASS = 204,
        //20050407新增
        RESTARTCAPTURE = 300, RESTOPCAPTURE = 301,
    };

    enum CALLBACKTYPE { BEFORE_PROCESS = 0, AFTER_PROCESS = 1, NO_USED = 2 };



    /***********************图像信息*************************/
    [StructLayout(LayoutKind.Explicit)]
    public struct MV_IMAGEINFO
    {
        [FieldOffset(0)]
        public uint Length;      // 图像的大小，以字节计
        [FieldOffset(8)]
        public uint nColor;      // 图像的颜色
        [FieldOffset(16)]
        public uint Heigth;      // 图像的高
        [FieldOffset(24)]
        public uint Width;       // 图像的宽
        [FieldOffset(32)]
        public uint SkipPixel;   // 每行跳过的像素
    }
    /******************************************************/


    /***********************像素点位*************************/
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int x;
        public int y;
    }
    /******************************************************/



    /***********************位图头部信息*************************/
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biSize;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biWidth;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biHeight;
        [MarshalAs(UnmanagedType.I2)]
        public short biPlanes;
        [MarshalAs(UnmanagedType.I2)]
        public short biBitCount;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biCompression;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biSizeImage;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biXPelsPerMeter;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biYPelsPerMeter;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biClrUsed;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biClrImportant;
    }
    /******************************************************/



    /***********************位图信息*************************/
    [StructLayout(LayoutKind.Sequential)]
    public struct bitmapinfo
    {
        [MarshalAs(UnmanagedType.Struct, SizeConst = 40)]
        public BITMAPINFOHEADER bmiHeader;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public Int32[] bmiColors;
    }
    /******************************************************/


    /***********************矩形信息*************************/
    [StructLayout(LayoutKind.Explicit)]
    public struct Rect
    {
        [FieldOffset(0)]
        public int left;
        [FieldOffset(4)]
        public int top;
        [FieldOffset(8)]
        public int right;
        [FieldOffset(12)]
        public int bottom;
    }
    /******************************************************/

    public unsafe class MVAPI
    {
        //  calls

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]    // 错误提示
        public static extern uint MV_GetLastError(bool bDisplayErrorStyring);

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern uint MV_GetDeviceNumber();   //板卡数

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MV_OpenDevice(uint Index, bool bRelese);  //创建设备

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern void MV_CloseDevice(IntPtr hDevice);  //删除设备

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern int MV_OperateDevice(IntPtr hDevice, int Oper);   //操纵设备

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern bool MV_SetDeviceParameter(IntPtr hDevice, uint Oper, uint Val); //设置设备属性

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern int MV_GetDeviceParameter(IntPtr hDevice, int Oper); //读取设备属性

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern bool MV_SaveDeviceParam(IntPtr hDevice); //储存设备属性

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern bool MV_ResetDeviceParam(IntPtr hDevice); //重置设备属性

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 捕获一帧
        public static extern IntPtr MV_CaptureSingle(IntPtr hDevice, bool IsProcess, int Buffer, int Bufflen, ref MV_IMAGEINFO pinfo);

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 保存文件
        public static extern bool MV_SaveFile([MarshalAs(UnmanagedType.LPStr)] String FileName, int FileType, IntPtr pImageData, ref MV_IMAGEINFO pinfo, int ImageTotal, bool IsUpDown, bool ColororNot, int Quality, bool m_bRGB15);

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 保存文件
        public static extern int MV_ReadPixel(ref MV_IMAGEINFO pinfo, IntPtr pImageData, ref Point pt, byte[] pVal);

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]
        public static extern bool MV_SaveFile([MarshalAs(UnmanagedType.LPStr)] String FileName, int FileType, IntPtr pImageData, ref MV_IMAGEINFO pinfo, uint ImageTotal, bool IsUpDown, bool ColororNot, int Quality);

        [DllImport("MVAPI.dll", CallingConvention = CallingConvention.StdCall)] // 设置设备的回调
        public static extern bool MV_SetCallBack(IntPtr hDevice, CALLBACKFUNC pCallBack, IntPtr pUserData, int CallType);
        //	MVAPI BOOL    WINAPI MV_SetCallBack( HANDLE hDevice, CALLBACKFUNC pCallBack, PVOID pUserData, CALLBACKTYPE CallType );

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 输出控制
        public static extern bool MV_SetOutputState(IntPtr hDevice, uint Index, uint HorL);

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  //输入状态读取
        public static extern uint MV_GetInputState(IntPtr hDevice, uint Index);

        [DllImport("MVAPI.dll", CallingConvention = CallingConvention.StdCall)]  // 输入触发控制
        public static extern bool MV_SetInputCallBack(IntPtr hDevice, uint Index, uint UniquelyID, PTRIGGEROUTINE pTirggerCall, IntPtr pContext);
        //	MVAPI BOOL  WINAPI MV_SetInputCallBack( HANDLE hDevice, ULONG Index, ULONG UniquelyID, PTRIGGEROUTINE pTirggerCall, PVOID pContext );


        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 对设备分配大帧存
        public static extern bool MV_AllocSequenceFrameMemory(IntPtr hDevice, uint Action, uint Number, int MemoryType);

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 对设备释放大帧存
        public static extern bool MV_FreeSequenceFrameMemory(IntPtr hDevice);

        [DllImport("MVAPI.dll", CallingConvention = CallingConvention.StdCall)]  // 对大帧存开始连续捕获
        public static extern bool MV_StartSequenceCapture(IntPtr hDevice, CONTINUEGARBMECHANISM pContinueCall, IntPtr pContext);
        //MVAPI BOOL WINAPI MV_StartSequenceCapture( HANDLE hDevice, CONTINUEGARBMECHANISM pContinueCall, PVOID pContext );

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 对大帧存停止连续捕获
        public static extern int MV_StopSequenceCapture(int hDevice);

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 根据FrameNo返回帧号地址和帧属性
        public static extern void MV_GetSequenceFrameAddress(int hDevice, uint FrameNo, ref MV_IMAGEINFO pProperty);

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)] // 按OsdMode模式使能OSD(掩码)功能:
        public static extern bool MV_SetMaskFunction(int hDevice, uint OsdMode);

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)] // 将OSD图案发送到设备
        public static extern bool MV_SetMaskBit(IntPtr hDevice, Rect MaskArea, // byte* pBitPattern );
                                                [MarshalAs(UnmanagedType.LPArray)] byte[] pBitPattern);
        //MVAPI BOOL   WINAPI MV_SetMaskBit( HANDLE hDevice, RECT MaskArea, LPBYTE pBitPattern );

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]
        // public delegate byte[]  MV_MakeMaskBit( int hDevice, IntPtr hWnd, Rect Area, int color );
        public static extern byte[] MV_MakeMaskBit(IntPtr hDevice, IntPtr hWnd, Rect Area, int color);
        //public static extern byte*  MV_MakeMaskBit( int hDevice, IntPtr hWnd, Rect Area, int color );
        //MVAPI LPBYTE WINAPI MV_MakeMaskBit( HANDLE hDevice, HWND hWnd, RECT Area, COLORREF Color );

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)] // 侦测信号
        public static extern bool MV_TestSignal(IntPtr hDevice, uint XSize, uint YSize);


        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)] // 获得参数
        public static extern bool MV_GetSignalParam(IntPtr hDevice, int Signal, IntPtr FloatVal, IntPtr IntVal);

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)] // 保存到配置文件。
        public static extern bool MV_SaveSignalParamToIni(IntPtr hDevice);

        [DllImport("MVCDISPFUN.dll", CharSet = CharSet.Auto)]
        public static extern bool MVD_InitDisplay(uint WinID, IntPtr hWnd, uint dwWidth, uint dwHeight, int ColorFmt, int DispFlip);

        [DllImport("MVCDISPFUN.dll", CharSet = CharSet.Auto)]
        public static extern bool MVD_SetDispSourceRect(uint WinID, Rect rcSrc);

        [DllImport("MVCDISPFUN.dll", CharSet = CharSet.Auto)]
        public static extern bool MVD_SetDispDestRect(uint WinID, Rect rcSrc);

        [DllImport("MVCDISPFUN.dll", CharSet = CharSet.Auto)]
        public static extern bool MVD_SetDispGDIText(uint WinID, uint StrNo, uint PosX, uint PosY, uint TextColor, IntPtr pStr, IntPtr TextFont);

        [DllImport("MVCDISPFUN.dll", CharSet = CharSet.Auto)]
        public static extern bool MVD_SetDispGDIGraph(uint WinID, uint GraphNo, int Line_Arc, uint Pos0X, uint Pos0Y, uint Pos1X, uint Pos1Y, IntPtr GDIPen);

        [DllImport("MVCDISPFUN.dll", CharSet = CharSet.Auto)]
        public static extern bool MVD_SetDispGDICanCalOne(uint WinID, int Line_Arc, uint No);

        [DllImport("MVCDISPFUN.dll", CharSet = CharSet.Auto)]
        public static extern bool MVD_SetDispCanCalAll(uint WinID, int Line_Arc);

        [DllImport("MVCDISPFUN.dll", CharSet = CharSet.Auto)]
        public static extern bool MVD_FiniDisplay(uint WinID);

        [DllImport("MVCDISPFUN.dll", CharSet = CharSet.Auto)]
        public static extern bool MVD_DisplayData(uint WinID, IntPtr buffer, uint BufSize, Rect rcDest, uint GDIMODE);

        [DllImport("MVCDISPFUN.dll", CharSet = CharSet.Auto)]
        public static extern bool MVD_GetOSDData(uint WinID, IntPtr buffer, uint BufSize);

        [DllImport("mvavi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool MV_AVIFileOpen(int index, byte[] lpszFileName, ref bitmapinfo alpb, ushort wSkipRate, int CompressType);

        [DllImport("mvavi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void MV_AVIFileInit(uint filetype, short framerate);

        [DllImport("mvavi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void MV_AVIFileFini();

        [DllImport("mvavi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool MV_AVIFileAddFrame(int index, ref bitmapinfo alpb, IntPtr alpImageBits, bool keyframe);
    }

    public delegate bool CALLBACKFUNC(IntPtr pData, ref MV_IMAGEINFO pImageInfo, IntPtr pUserData, uint Index);

    public delegate void PTRIGGEROUTINE(uint UniquelyID, uint Reson, IntPtr pContext);

    public delegate void CONTINUEGARBMECHANISM(IntPtr pData, ref MV_IMAGEINFO pImage, uint ImageNumber, uint wholeLength, IntPtr pUserData);
}
