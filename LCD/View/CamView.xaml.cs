using LCD.Data;
using LCD.Dll;
using LCD.Utils;
using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MvCamCtrl.NET.MyCamera;

namespace LCD.View
{
    /// <summary>
    /// CamView.xaml 的交互逻辑
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class CamView : System.Windows.Controls.UserControl
    {
        public static int[] DeviceHandle;
        //MV public variables
        public static int CurDevice;

        private double height;
        private double width;

        MyCamera.cbOutputExdelegate ImageCallback;
        //Thread m_hReceiveThread = null;
        private MyCamera m_MyCamera = new MyCamera();
        private MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
        MyCamera.MV_FRAME_OUT_INFO_EX m_stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();

        // ch:用于从驱动获取图像的缓存 | en:Buffer for getting image from driver
        UInt32 m_nBufSizeForDriver = 0;
        IntPtr m_BufForDriver = IntPtr.Zero;
        private static Object BufForDriverLock = new Object();

        public Thickness TopMargin { set { }
            get {
                return new Thickness(0, height * Top / 100.0, 0, 0); } 
        }
        public Thickness LeftMargin { set { }
            get { 
                return new Thickness(width * Left / 100.0, 0, 0, 0); 
            } }


        //rotate=0默认不旋转
        private double top=0,left=0,rotate=0;
        public double Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
                topline.Margin = new Thickness(0, height * Top / 100.0, 0, 0);
                //this.UpdateLayout();
            }        
        }

        public double Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
                leftline.Margin = new Thickness(width * Left / 100.0, 0, 0, 0);
                //this.UpdateLayout();
            }
        }


        public CamView()
        {
            InitCam();
            InitializeComponent();
            ScrollBar.Value = Project.cfg.ExposureTime;

            //更新旋转角度
            rotate = Project.cfg.CamRotation;

            this.DataContext = this;

            this.Loaded += CamView_Loaded;
        }

        private void CamView_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_MyCamera != null)
            {
                MVCC_FLOATVALUE mVCC = new MVCC_FLOATVALUE();
                int val = m_MyCamera.MV_CC_GetExposureTime_NET(ref mVCC);
                if (val == 0)
                {
                    //设置最小值啊
                    ScrollBar.Minimum = mVCC.fMin;
                }
            }
        }

        public static uint find_camera_count()
        {
            MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_stDeviceList);
            if (0 != nRet)
            {
                LogHelper.Instance.Write("Enumerate devices failed:" + nRet);
                return 0;
            }
            return m_stDeviceList.nDeviceNum;
        }

        //搜索海康相机
        private void DeviceListAcq()
        {
            // ch:创建设备列表 | en:Create Device List
            System.GC.Collect();
            m_stDeviceList.nDeviceNum = 0;
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_stDeviceList);
            if (0 != nRet)
            {
                LogHelper.Instance.Write("Enumerate devices fail:"+nRet);
                return;
            }
            LogHelper.Instance.Write("Enumerate camera count:"+ m_stDeviceList.nDeviceNum);

            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                    if (gigeInfo.chUserDefinedName != "")
                    {
                        LogHelper.Instance.Write("GEV: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        LogHelper.Instance.Write("GEV: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chUserDefinedName != "")
                    {
                        LogHelper.Instance.Write("U3V: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        LogHelper.Instance.Write("U3V: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                    }
                }
            }
            
        }

        /// <summary>
        /// 初始化相机设备
        /// </summary>
        public void InitCam()
        {
            try
            {

                switch (Project.cfg.CamType)
                {
                    case CamTypeEnum.HikVision://暂时没有必要加ICamera
                        {
                            string a = Project.cfg.ExposureTime.ToString();
                            Project.WriteLog("曝光时间：：：：" + Project.cfg.ExposureTime.ToString());

                            DeviceListAcq();
                            if (m_stDeviceList.nDeviceNum > 0)
                            {                               
                                int index = Project.cfg.CamIndex;
                                if((index <0) ||(index >= m_stDeviceList.nDeviceNum))
                                {
                                    LogHelper.Instance.Write("配置文件相机序号："+index+"，超出范围，设置为默认值0");
                                    index = 0;
                                }

                                // ch:获取选择的设备信息 | en:Get selected device information
                                MyCamera.MV_CC_DEVICE_INFO device =
                                    (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[index],
                                                                                  typeof(MyCamera.MV_CC_DEVICE_INFO));
                                // ch:打开设备 | en:Open device
                                if (null == m_MyCamera)
                                {
                                    m_MyCamera = new MyCamera();
                                    if (null == m_MyCamera)
                                    {
                                        LogHelper.Instance.Write("初始化m_MyCamera失败");
                                        return;
                                    }
                                }
                                m_pMyCamera = m_MyCamera;

                                var nRet = MyCamera.MV_OK;
                                nRet = m_MyCamera.MV_CC_CreateDevice_NET(ref device);
                                if (MyCamera.MV_OK != nRet)
                                {
                                    LogHelper.Instance.Write("MV_CC_CreateDevice_NET返回失败："+nRet);
                                    return;
                                }

                                nRet = m_MyCamera.MV_CC_OpenDevice_NET();
                                if (MyCamera.MV_OK != nRet)
                                {
                                    m_MyCamera.MV_CC_DestroyDevice_NET();
                                    LogHelper.Instance.Write("Device open fail:"+ nRet);
                                    return;
                                }

                                // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                                {
                                    int nPacketSize = m_MyCamera.MV_CC_GetOptimalPacketSize_NET();
                                    if (nPacketSize > 0)
                                    {
                                        nRet = m_MyCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                                        if (nRet != MyCamera.MV_OK)
                                        {
                                            LogHelper.Instance.Write("Set Packet Size failed:"+ nRet);
                                        }
                                    }
                                    else
                                    {
                                        LogHelper.Instance.Write("Get Packet Size failed:"+ nPacketSize);
                                    }
                                }

                                // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
                                nRet = m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                                nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);

                                int Error = m_MyCamera.MV_CC_SetEnumValue_NET("PixelFormat", (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed);
                                //int Error= m_pMyCamera.MV_CC_SetPixelFormat_NET(0x02180014);//mono8=0x1080001
                                nRet = m_MyCamera.MV_CC_SetAutoExposureTimeLower_NET(Project.cfg.ExposureTime);
                                nRet = m_MyCamera.MV_CC_SetAutoExposureTimeUpper_NET(Project.cfg.ExposureTime);
                                //nRet = m_MyCamera.MV_CC_SetExposureTime_NET(Project.cfg.ExposureTime);                                

                                Int32 nWidth, nHeight = 0;
                                MyCamera.MVCC_INTVALUE nIntValue = new MyCamera.MVCC_INTVALUE();
                                int ret = m_MyCamera.MV_CC_GetIntValue_NET("Width", ref nIntValue);
                                nWidth = (Int32)nIntValue.nCurValue;
                                ret = m_MyCamera.MV_CC_GetIntValue_NET("Height", ref nIntValue);
                                nHeight = (Int32)nIntValue.nCurValue;
                                LogHelper.Instance.Write("相机长和宽：" + nWidth+","+nHeight);
                                System.GC.Collect();
                                //设置心跳超时时间
                                ret = m_MyCamera.MV_CC_SetHeartBeatTimeout_NET(5000);
                                if (MyCamera.MV_OK != ret)
                                {
                                    Project.WriteLog("设置相机心跳时间失败:"+ret);
                                }
                                else
                                {
                                    Project.WriteLog("设置相机心跳时间OK");
                                }

                                m_MyCamera.MV_CC_SetExposureAutoMode_NET((uint)MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_CONTINUOUS);

                                m_stFrameInfo.nFrameLen = 0;//取流之前先清除帧长度
                                m_stFrameInfo.enPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;

                                ImageCallback = new MyCamera.cbOutputExdelegate(ImageCallBack1);
                                nRet = m_pMyCamera.MV_CC_RegisterImageCallBackEx_NET(ImageCallback, IntPtr.Zero);
                                if (MyCamera.MV_OK != nRet)
                                {
                                    LogHelper.Instance.Write("Register image callback failed!");
                                    //break;
                                }

                                // ch:开始采集 | en:Start Grabbing
                                nRet = m_MyCamera.MV_CC_StartGrabbing_NET();
                                if (MyCamera.MV_OK != nRet)
                                {
                                    m_MyCamera.MV_CC_DestroyDevice_NET();
                                    LogHelper.Instance.Write("Start Grabbing Fail:"+ nRet);
                                    return;
                                }


                                Thread.Sleep(100);
                                //设置曝光
                                //m_MyCamera.MV_CC_SetEnumValue_NET("ExposureAuto", 0);
                                nRet = m_MyCamera.MV_CC_SetFloatValue_NET("ExposureTime", 800000);
                                if (nRet != MyCamera.MV_OK)
                                {
                                    LogHelper.Instance.Write("Set Exposure Time Fail:" + nRet);
                                }
                                m_MyCamera.MV_CC_SetEnumValue_NET("GainAuto", 0);
                                nRet = m_MyCamera.MV_CC_SetFloatValue_NET("Gain", 0);
                                if (nRet != MyCamera.MV_OK)
                                {
                                    LogHelper.Instance.Write("Set Gain Fail:" + nRet);
                                }
                                nRet = m_MyCamera.MV_CC_SetFloatValue_NET("AcquisitionFrameRate", 5);
                                if (nRet != MyCamera.MV_OK)
                                {
                                    LogHelper.Instance.Write("Set Frame Rate Fail:" + nRet);
                                }

                                //m_hReceiveThread = new Thread(ReceiveThreadProcess);
                                //m_hReceiveThread.Start();
                            }
                        }
                        break;
                    case CamTypeEnum.V110:

                        //DeviceHandle = new int[nBdNum];
                        ////采集并显示
                        //for (uint i = 0; i < nBdNum; i++)
                        //    DeviceHandle[i] = MVAPI.MV_OpenDevice(i, false);
                        //CurDevice = DeviceHandle[0];
                        //MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_WHND, (uint)currHwnd);//set the current version of the world 
                        ////设置左侧宽度
                        ////设置右侧宽度
                        ////mycamset = GlobalConfig.myCurrentSeries.myCamSet;
                        //MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_TOP, (uint)0);//DISP_HEIGHT				= 10, 
                        //MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_LEFT, (uint)0);//DISP_HEIGHT				= 10, 
                        //MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_HEIGHT, 420);//DISP_HEIGHT				= 10, 
                        //MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_WIDTH, 550);//DISP_WIDTH				= 11,
                        //MVAPI.MV_OperateDevice(CurDevice, (int)RUNOPER.MVRUN);   // MVRUN =1
                        //                                                         //if (mycamset.format == "NTSC")
                        //                                                         //{
                        //MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.ADJUST_STANDARD, (uint)0);

                        break;
                    case CamTypeEnum.SVS:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LogHelper.Instance.Write("初始化相机异常:"+e.Message+","+e.StackTrace);
            }

        }

        public void CloseCam()
        {            
            if (m_MyCamera != null)
            {
                int nRet = m_MyCamera.MV_CC_StopGrabbing_NET();
                if (nRet != MyCamera.MV_OK)
                {
                    LogHelper.Instance.Write("Stop Grabbing Fail:" + nRet);
                }

                //if (m_BufForDriver != IntPtr.Zero)
                //{
                //    Marshal.Release(m_BufForDriver);
                //}               
                // ch:关闭设备 | en:Close Device
                nRet = m_MyCamera.MV_CC_CloseDevice_NET();
                if (nRet != MyCamera.MV_OK)
                {
                    LogHelper.Instance.Write("MV_CC_CloseDevice_NET Fail:" + nRet);
                }
                LogHelper.Instance.Write("MV_CC_DestroyDevice_NET...start");
                //m_MyCamera.MV_CC_DestroyDevice_NET();
                LogHelper.Instance.Write("MV_CC_DestroyDevice_NET...end");
            }
        }

        #region HikVision部分
        MyCamera.cbOutputExdelegate cbImage;
        /// <summary>
        /// 相机主体
        /// </summary>
        public MyCamera m_pMyCamera { get; set; }
        /// <summary>
        /// 拍摄完成回调函数
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="FrameInfo"></param>
        /// <param name="pUser"></param>
        private unsafe void ImageCallBack1(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX FrameInfo, IntPtr pUser)
        {
            //bool ShowTime = false;
            try//这里出错一般是相机掉线了
            {
                if (FrameInfo.nLostPacket > 0)//丢帧
                {
                    var logStr = DateTime.Now + Environment.NewLine
                         + "PixelType:" + FrameInfo.enPixelType.ToString() + Environment.NewLine
                         + "ExposureTime:" + FrameInfo.fExposureTime + Environment.NewLine
                         + "FrameLen:" + FrameInfo.nFrameLen + Environment.NewLine
                         + "LostPacket:" + FrameInfo.nLostPacket + Environment.NewLine;

                    //OnInfoLog?.Invoke(CameraLogType.Error, "LostPacket:" + logStr);

                    //if (TryCount > 3)//连续丢帧
                    //{
                    //    OnInfoLog?.Invoke(CameraLogType.MaskMessage, "丢帧");//通知到界面上
                    //    OnGrabError(new Exception("丢帧次数过多，相机可能已经掉线：当前曝光-" + FrameInfo.fExposureTime), "102");
                    //    TryCount = 0;//重置丢帧次数
                    //    StopGrab();
                    //    return;
                    //}

                    //TryCount++;
                    //FaildCount++;
                    //TimeCounting.Restart();
                    //var evel = new MyCamera.MVCC_ENUMVALUE();
                    //m_pMyCamera.MV_CC_GetTriggerMode_NET(ref evel);
                    //if (evel.nCurValue == 1)//根据触发模式重新拍摄
                    //{
                    //    nRet = m_pMyCamera.MV_CC_SetTriggerSource_NET(7);
                    //    nRet = m_pMyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
                    //}
                    return;
                }
                var Width = FrameInfo.nWidth;
                var Height = FrameInfo.nHeight;

                if (rotate > 0)
                {
                    MV_CC_ROTATE_IMAGE_PARAM mV_CC_ROTATE_ = new MV_CC_ROTATE_IMAGE_PARAM();
                    mV_CC_ROTATE_.enPixelType = FrameInfo.enPixelType;
                    mV_CC_ROTATE_.nWidth = Width;
                    mV_CC_ROTATE_.nHeight = Height;
                    mV_CC_ROTATE_.pSrcData = pData;
                    int size = Width * Height * 3;
                    mV_CC_ROTATE_.nSrcDataLen = (uint)(size);
                    IntPtr intPtr = Marshal.AllocHGlobal(size);
                    mV_CC_ROTATE_.pDstBuf = intPtr;
                    mV_CC_ROTATE_.nDstBufSize = (uint)size;
                    switch(rotate)
                    {
                        case 90:
                            mV_CC_ROTATE_.enRotationAngle = MV_IMG_ROTATION_ANGLE.MV_IMAGE_ROTATE_90;
                            break;
                        case 180:
                            mV_CC_ROTATE_.enRotationAngle = MV_IMG_ROTATION_ANGLE.MV_IMAGE_ROTATE_180;
                            break;
                        case 270:
                            mV_CC_ROTATE_.enRotationAngle = MV_IMG_ROTATION_ANGLE.MV_IMAGE_ROTATE_270;
                            break;
                        default:
                            mV_CC_ROTATE_.enRotationAngle = MV_IMG_ROTATION_ANGLE.MV_IMAGE_ROTATE_180;
                            break;
                    }                    
                    int ret = m_pMyCamera.MV_CC_RotateImage_NET(ref mV_CC_ROTATE_);
                    if (ret == 0)
                    {
                        //90和270度的时候需要对调一下长宽
                        if ((mV_CC_ROTATE_.enRotationAngle == MV_IMG_ROTATION_ANGLE.MV_IMAGE_ROTATE_90)||
                            (mV_CC_ROTATE_.enRotationAngle == MV_IMG_ROTATION_ANGLE.MV_IMAGE_ROTATE_270))
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                SetMainImagePtrRGB24(Height, Width, mV_CC_ROTATE_.pDstBuf);
                                Marshal.FreeHGlobal(intPtr); // 释放内存
                            }), null);
                        }
                        else
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                SetMainImagePtrRGB24(Width,Height, mV_CC_ROTATE_.pDstBuf);
                                Marshal.FreeHGlobal(intPtr); // 释放内存
                            }), null);
                        }
                    }
                }
                else
                {
                    if (FrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                        Dispatcher.Invoke(new Action(() =>
                        {
                            SetMainImagePtr8(Width, Height, pData);

                    }), null);
                else
                    Dispatcher.Invoke(new Action(() =>
                    {
                        SetMainImagePtrRGB24(Width, Height, pData);

                        }), null);
                }
                return;

            }
            catch(Exception ex)
            {
                LogHelper.Instance.Write("相机异常："+ex.Message+","+ex.StackTrace);
            }
            finally
            {
            }
        }

        public void rotation(int angle)
        {
            rotate = angle;
        }

        private void SetMainImagePtrRGB24(int _Width, int _Height, IntPtr buffer)
        {
            //byte[] ImageBuffer = new byte[_Width * _Height * 3 + 4096];
            //GCHandle handle = GCHandle.Alloc(ImageBuffer, GCHandleType.Pinned);
            //Marshal.Copy(buffer, ImageBuffer, 0, (int)(_Width * _Height * 3));
            //IntPtr pImage = handle.AddrOfPinnedObject();
            //System.Drawing.Bitmap bmp = new Bitmap(_Width, _Height, _Width * 3,System.Drawing.Imaging.PixelFormat.Format24bppRgb, pImage);
            ////接下来划线
            //try
            //{
            //    System.Drawing.Graphics g = Graphics.FromImage(bmp);
            //    System.Drawing.Pen mypen = new System.Drawing.Pen(System.Drawing.Color.Red, 4);
            //    System.Drawing.Brush nBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
            //    int a;
            //    System.Drawing.Font font = new Font("微软雅黑", 56);
            //    g.DrawImage(bmp, 0, 0, _Width, _Height);
            //    a = _Height / 2;
            //    g.DrawLine(mypen, 0, a, _Width, a);
            //    a = _Width / 2;
            //    g.DrawLine(mypen, a, 0, a, _Height);
            //    mypen.Dispose();
            //    nBrush.Dispose();
            //    g.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    if (handle.IsAllocated)
            //    {
            //        handle.Free();
            //    }
            //    return;
            //}
            //if (handle.IsAllocated)
            //{
            //    handle.Free();
            //}
            ////显示出来啊
            //pictureHost.Source = BitmapSourceConvert.BitmapToWriteableBitmap(bmp);
            //return;

            if (wbBitmap == null || wbBitmap.Width != _Width || wbBitmap.Height != _Height || wbBitmap.Format != PixelFormats.Rgb24)
            {
                wbBitmap = new WriteableBitmap(_Width, _Height, 96, 96, PixelFormats.Rgb24, null);
            }

            unsafe
            {
                wbBitmap.Lock();
                CopyMemory(wbBitmap.BackBuffer, buffer, (uint)(_Width * _Height * 3));
                wbBitmap.AddDirtyRect(new Int32Rect(0, 0, _Width, _Height));
                wbBitmap.Unlock();
            }
        }
        #endregion

        public WriteableBitmap wbBitmap { set; get; }
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        public void SetMainImagePtr8(int _Width, int _Height, IntPtr buffer)
        {
            if (wbBitmap == null || wbBitmap.Width != _Width || wbBitmap.Height != _Height || wbBitmap.Format != PixelFormats.Gray8)
            {
                wbBitmap = new WriteableBitmap(_Width, _Height, 96, 96, PixelFormats.Gray8, null);
            }

            unsafe
            {
                wbBitmap.Lock();
                CopyMemory(wbBitmap.BackBuffer, buffer, (uint)(_Width * _Height));
                wbBitmap.AddDirtyRect(new Int32Rect(0, 0, _Width, _Height));
                wbBitmap.Unlock();
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //height = this.ActualHeight;
            //width = this.ActualHeight;
            height = grid0.ActualHeight;
            width = grid0.ActualWidth;

            //需要刷新一下TopMargin
            if (height > 0 && width > 0)
            {
                if (Math.Abs(leftline.Margin.Left - width * Left / 100.0) > width * Left / 10000.0)
                {
                    leftline.Margin = new Thickness(width * Left / 100.0, 0, 0, 0);
                }
                if (Math.Abs(topline.Margin.Top - height * Top / 100.0) > height * Top / 10000.0)
                {
                    topline.Margin = new Thickness(0, height * Top / 100.0, 0, 0);
                }
            }


            try
            {
                //MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_TOP, (uint)0);//DISP_HEIGHT				= 10, 
                //MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_LEFT, (uint)0);//DISP_HEIGHT				= 10, 
                //MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_HEIGHT, (uint)height);//DISP_HEIGHT				= 10, 
                //MVAPI.MV_SetDeviceParameter(CurDevice, (int)MV_PARAMTER.DISP_WIDTH, (uint)width);//DISP_WIDTH				= 11,
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Top = ActualHeight / 2;
            // Left = ActualWidth / 2;

            Top = Project.cfg.CamTop;
            Left = Project.cfg.CamLeft;
        }

        private void ScrollBar_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_pMyCamera == null)
            {
                return;
            }
            for (int i = 0; i < 3; i++)
            {
                int ret = m_pMyCamera.MV_CC_SetExposureTime_NET((uint)ScrollBar.Value);
                MVCC_FLOATVALUE mVCC = new MVCC_FLOATVALUE();
                int val = m_MyCamera.MV_CC_GetExposureTime_NET(ref mVCC);
                if (val == 0)
                {
                    double differ = Math.Abs(mVCC.fCurValue - ScrollBar.Value);
                    if (differ > 5)
                    {
                        LogHelper.Instance.Write("设置和实际值差异过大："+differ);
                        if(ScrollBar.Value < mVCC.fMin)
                        {
                            ScrollBar.Value = mVCC.fMin;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            Project.cfg.ExposureTime = (uint)ScrollBar.Value;
        }

        private void ScrollBar_OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Project.SaveConfig("Config.xml");
        }
    }
    public class Prop : ViewBase
    {
        private int _Row;

        public int Row
        {
            get { return _Row; }
            set { _Row = value; OnPropertyChanged(); }
        }

        private int _Colu1;

        public int Colu1
        {
            get { return _Colu1; }
            set { _Colu1 = value; }
        }


    }
}
