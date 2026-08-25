using LCD.Data;
using LCD.Dll;
using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// CamView.xaml 的交互逻辑
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class CamView : UserControl
    {
        public static int[] DeviceHandle;
        //MV public variables
        public static int CurDevice;

        public Thickness TopMargin { get { return new Thickness(0, Top, 0, 0); } }
        public Thickness LeftMargin { get { return new Thickness(Left, 0, 0, 0); } }

        public double Top { get; set; } = 0;
        public double Left { get; set; } = 0;

        public CamView()
        {
            InitCam();
            InitializeComponent();
            ScrollBar.Value = Project.cfg.ExposureTime;



            this.DataContext = this;
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

                            var CamRes = MyCamera.GetDeviceCount();
                            if (CamRes.Count > 0)
                            {
                                cbImage = new MyCamera.cbOutputExdelegate(ImageCallBack1);

                                MyCamera.MV_CC_DEVICE_INFO device =
                                    (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(CamRes[0],
                                                                                  typeof(MyCamera.MV_CC_DEVICE_INFO));

                                if (null == m_pMyCamera)
                                {
                                    m_pMyCamera = new MyCamera();
                                    if (null == m_pMyCamera)
                                    {
                                        return;
                                    }
                                }

                                var nRet = MyCamera.MV_OK;


                                nRet = m_pMyCamera.MV_CC_CreateDevice_NET(ref device);
                                if (MyCamera.MV_OK != nRet)
                                {
                                    return;
                                }

                                nRet = m_pMyCamera.MV_CC_OpenDevice_NET();
                                int Error = m_pMyCamera.MV_CC_SetEnumValue_NET("PixelFormat", (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed);
                                //int Error= m_pMyCamera.MV_CC_SetPixelFormat_NET(0x02180014);//mono8=0x1080001
                                m_pMyCamera.MV_CC_SetAutoExposureTimeLower_NET(Project.cfg.ExposureTime);
                                m_pMyCamera.MV_CC_SetAutoExposureTimeUpper_NET(Project.cfg.ExposureTime);
                                m_pMyCamera.MV_CC_SetExposureTime_NET(Project.cfg.ExposureTime);

                                if (MyCamera.MV_OK != nRet)
                                {
                                    m_pMyCamera.MV_CC_DestroyDevice_NET();
                                    return;
                                }
                                //ExposureTime
                                m_pMyCamera.MV_CC_GetOptimalPacketSize_NET();

                                nRet = m_pMyCamera.MV_CC_RegisterImageCallBackEx_NET(cbImage, IntPtr.Zero);

                                m_pMyCamera.MV_CC_StartGrabbing_NET();

                                m_pMyCamera.MV_CC_SetExposureAutoMode_NET((uint)MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_CONTINUOUS);
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
            }

        }

        public void CloseCam()
        {
            m_pMyCamera?.MV_CC_StopGrabbing_NET();
            m_pMyCamera?.MV_CC_CloseDevice_NET();
            //m_pMyCamera?.MV_CC_DestroyDevice_NET();
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
                return;

            }
            catch// (Exception ex)
            {
            }
            finally
            {
            }
        }

        private void SetMainImagePtrRGB24(int _Width, int _Height, IntPtr buffer)
        {
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
            double height = this.ActualHeight;
            double width = this.ActualHeight;

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
            m_pMyCamera.MV_CC_SetExposureTime_NET((uint)ScrollBar.Value);
            Project.cfg.ExposureTime = (uint)ScrollBar.Value;
        }

        private void ScrollBar_OnMouseLeave(object sender, MouseEventArgs e)
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
