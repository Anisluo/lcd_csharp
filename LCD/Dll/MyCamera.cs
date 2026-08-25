using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace MvCamCtrl.NET
{
    /// <summary>
    /// 海康相机类库
    /// </summary>
    public class MyCamera
    {

        /// <summary>
        /// 获取设备
        /// </summary>
        /// <returns></returns>
        public static List<IntPtr> GetDeviceCount(string Name = null)
        {
            var Result = new List<IntPtr>();
            int nRet;
            try
            {
                MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
                nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
                // ch:在窗体列表中显示设备名 | en:Display device name in the form list
               
                //临时调整相机顺序
                for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
                {
                    MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                    if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                        MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                        var CameraType = gigeInfo.chUserDefinedName.ToLower();
                        if (Name == null || CameraType.StartsWith(Name))
                        {
                            Result.Add(m_pDeviceList.pDeviceInfo[i]);
                            var access = MyCamera.MV_CC_IsDeviceAccessible_NET(ref device, 1);//设备是否可用
                        }
                    }
                   // break;
                }
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText("camerror.txt", e.Message + e.StackTrace + Environment.NewLine);
            }

            return Result;
        }

        #region  Method
        /// <summary>
        /// 获取图片深度
        /// </summary>
        /// <param name="enPixelType"></param>
        /// <returns></returns>
        public static int GetDepth(MyCamera.MvGvspPixelType enPixelType)
        {
            switch (enPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono1p:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono2p:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono4p:
                    return 4;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                    return 8;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8_Signed:
                    return 8;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                    return 10;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                    return 10;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                    return 12;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return 12;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono14:
                    return 14;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono16:
                    return 16;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR16:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG16:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB16:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG16:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGBA8_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGRA8_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB10_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR10_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB12_Packed:
                    return 12;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR12_Packed:
                    return 12;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB16_Packed:
                    return 16;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB10V1_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB10V2_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB12V1_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB565_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR565_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV411_Packed:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                    return 12;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                    return 12;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV444_Packed:
                    return 16;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR8_CBYCR:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR422_8:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR422_8_CBYCRY:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR601_8_CBYCR:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR601_422_8:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR601_422_8_CBYCRY:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR601_411_8_CBYYCRYY:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR709_8_CBYCR:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR709_422_8:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR709_422_8_CBYCRY:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR709_411_8_CBYYCRYY:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Planar:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB10_Planar:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB12_Planar:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB16_Planar:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Jpeg:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Coord3D_ABC32f:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Coord3D_ABC32f_Planar:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Coord3D_AC32f:
                    break;
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_COORD3D_DEPTH_PLUS_MASK:
                    break;
                default:
                    break;
            }
            return 8;
        }

        /// <summary>
        /// Get SDK Version
        /// </summary>
        /// <returns>Always return 4 Bytes of version number |Main  |Sub   |Rev   |Test|
        ///                                                   8bits  8bits  8bits  8bits 
        /// </returns>
        public static uint MV_CC_GetSDKVersion_NET()
        {
            return MyCamera.MV_CC_GetSDKVersion();
        }

        /// <summary>
        /// Get supported Transport Layer
        /// </summary>
        /// <returns>Supported Transport Layer number</returns>
        public static int MV_CC_EnumerateTls_NET()
        {
            return MyCamera.MV_CC_EnumerateTls();
        }

        /// <summary>
        /// Enumerate Device
        /// </summary>
        /// <param name="nTLayerType">Enumerate TLs</param>
        /// <param name="stDevList">Device List</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public static int MV_CC_EnumDevices_NET(uint nTLayerType, ref MyCamera.MV_CC_DEVICE_INFO_LIST stDevList)
        {
            return MyCamera.MV_CC_EnumDevices(nTLayerType, ref stDevList);
        }

        /// <summary>
        /// Enumerate device according to manufacture name
        /// </summary>
        /// <param name="nTLayerType">Enumerate TLs</param>
        /// <param name="stDevList">Device List</param>
        /// <param name="pManufacturerName">Manufacture Name</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public static int MV_CC_EnumDevicesEx_NET(uint nTLayerType, ref MyCamera.MV_CC_DEVICE_INFO_LIST stDevList, string pManufacturerName)
        {
            return MyCamera.MV_CC_EnumDevicesEx(nTLayerType, ref stDevList, pManufacturerName);
        }

        /// <summary>
        /// Is the device accessible 
        /// MV_ACCESS_Exclusive  1  独占权限，其他APP只允许读CCP寄存器  
        ///  MV_ACCESS_ExclusiveWithSwitch  2  可以从5模式下抢占权限，然后以独占权限打开
        ///  MV_ACCESS_Control  3  控制权限，其他APP允许读所有寄存器
        ///  MV_ACCESS_ControlWithSwitch  4  可以从5的模式下抢占权限，然后以控制权限打开
        ///  MV_ACCESS_ControlSwitchEnable  5  以可被抢占的控制权限打开
        ///  MV_ACCESS_ControlSwitchEnableWithKey  6  可以从5的模式下抢占权限，然后以可被抢占的控制权限打开
        ///  MV_ACCESS_Monitor  7  读模式打开设备，适用于控制权限下
        /// </summary>
        /// <param name="stDevInfo">Device Information</param>
        /// <param name="nAccessMode">MV_ACCESS_</param>
        /// <returns>Access, return true. Not access, return false</returns>
        public static bool MV_CC_IsDeviceAccessible_NET(ref MyCamera.MV_CC_DEVICE_INFO stDevInfo, uint nAccessMode)
        {
            return MyCamera.MV_CC_IsDeviceAccessible(ref stDevInfo, nAccessMode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MyCamera()
        {
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MyCamera()
        {
        }

        /// <summary>
        /// Create Device
        /// </summary>
        /// <param name="stDevInfo">Device Information</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_CreateDevice_NET(ref MyCamera.MV_CC_DEVICE_INFO stDevInfo)
        {
            if (IntPtr.Zero != this.handle)
            {
                MyCamera.MV_CC_DestroyHandle(this.handle);
                this.handle = IntPtr.Zero;
            }
            return MyCamera.MV_CC_CreateHandle(ref this.handle, ref stDevInfo);
        }

        /// <summary>
        /// Create Device without log
        /// </summary>
        /// <param name="stDevInfo">Device Information</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_CreateDeviceWithoutLog_NET(ref MyCamera.MV_CC_DEVICE_INFO stDevInfo)
        {
            if (IntPtr.Zero != this.handle)
            {
                MyCamera.MV_CC_DestroyHandle(this.handle);
                this.handle = IntPtr.Zero;
            }
            return MyCamera.MV_CC_CreateHandleWithoutLog(ref this.handle, ref stDevInfo);
        } 

        /// <summary>
        /// Destroy Device
        /// </summary>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_DestroyDevice_NET()
        {
            Thread.Sleep(500);
            int result = MyCamera.MV_CC_DestroyHandle(this.handle);
            this.handle = IntPtr.Zero;
            return result;
        }

        /// <summary>
        /// Open Device
        /// </summary>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_OpenDevice_NET()
        {
            return MyCamera.MV_CC_OpenDevice(this.handle, 1U, 0);
        }

        /// <summary>
        /// Open Device
        /// </summary>
        /// <param name="nAccessMode">Access Right</param>
        /// <param name="nSwitchoverKey">Switch key of access right</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_OpenDevice_NET(uint nAccessMode, ushort nSwitchoverKey)
        {
            return MyCamera.MV_CC_OpenDevice(this.handle, nAccessMode, nSwitchoverKey);
        }

        /// <summary>
        /// Close Device
        /// </summary>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_CloseDevice_NET()
        {
            return MyCamera.MV_CC_CloseDevice(this.handle);
        }

        /// <summary>
        /// Is the device connected
        /// </summary>
        /// <returns>Connected, return true. Not Connected or DIsconnected, return false</returns>
        public bool MV_CC_IsDeviceConnected_NET()
        {
            return MyCamera.MV_CC_IsDeviceConnected(this.handle);
        }

        /// <summary>
        /// Register the image callback function
        /// </summary>
        /// <param name="cbOutput">Callback function pointer</param>
        /// <param name="pUser">User defined variable</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_RegisterImageCallBackEx_NET(MyCamera.cbOutputExdelegate cbOutput, IntPtr pUser)
        {
            return MyCamera.MV_CC_RegisterImageCallBackEx(this.handle, cbOutput, pUser);
        }

        /// <summary>
        /// Register the RGB image callback function
        /// </summary>
        /// <param name="cbOutput">Callback function pointer</param>
        /// <param name="pUser">User defined variable</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_RegisterImageCallBackForRGB_NET(MyCamera.cbOutputExdelegate cbOutput, IntPtr pUser)
        {
            return MyCamera.MV_CC_RegisterImageCallBackForRGB(this.handle, cbOutput, pUser);
        }

        /// <summary>
        /// Register the BGR image callback function
        /// </summary>
        /// <param name="cbOutput">Callback function pointer</param>
        /// <param name="pUser">User defined variable</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_RegisterImageCallBackForBGR_NET(MyCamera.cbOutputExdelegate cbOutput, IntPtr pUser)
        {
            return MyCamera.MV_CC_RegisterImageCallBackForBGR(this.handle, cbOutput, pUser);
        }

        /// <summary>
        /// Start Grabbing
        /// </summary>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_StartGrabbing_NET()
        {
            return MyCamera.MV_CC_StartGrabbing(this.handle);
        }

        /// <summary>
        /// Stop Grabbing
        /// </summary>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_StopGrabbing_NET()
        {
            return MyCamera.MV_CC_StopGrabbing(this.handle);
        }

        /// <summary>
        /// Get one frame of RGB image, this function is using query to get data
        /// query whether the internal cache has data, get data if there has, return error code if no data
        /// </summary>
        /// <param name="pData">Image data receiving buffer</param>
        /// <param name="nDataSize">Buffer size</param>
        /// <param name="pFrameInfo">Image information</param>
        /// <param name="nMsec">Waiting timeout</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetImageForRGB_NET(IntPtr pData, uint nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, int nMsec)
        {
            return MyCamera.MV_CC_GetImageForRGB(this.handle, pData, nDataSize, ref pFrameInfo, nMsec);
        }

        /// <summary>
        /// Get one frame of BGR image, this function is using query to get data
        /// query whether the internal cache has data, get data if there has, return error code if no data
        /// </summary>
        /// <param name="pData">Image data receiving buffer</param>
        /// <param name="nDataSize">Buffer size</param>
        /// <param name="pFrameInfo">Image information</param>
        /// <param name="nMsec">Waiting timeout</param>
        /// <returns>Success, return MV_OK. Failure, return error cod</returns>
        public int MV_CC_GetImageForBGR_NET(IntPtr pData, uint nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, int nMsec)
        {
            return MyCamera.MV_CC_GetImageForBGR(this.handle, pData, nDataSize, ref pFrameInfo, nMsec);
        }

        /// <summary>
        /// Get a frame of an image using an internal cache
        /// </summary>
        /// <param name="pFrame">Image data and image information</param>
        /// <param name="nMsec">Waiting timeout</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetImageBuffer_NET(ref MyCamera.MV_FRAME_OUT pFrame, int nMsec)
        {
            return MyCamera.MV_CC_GetImageBuffer(this.handle, ref pFrame, nMsec);
        }

        /// <summary>
        /// Free image buffer（used with MV_CC_GetImageBuffer）
        /// </summary>
        /// <param name="pFrame">Image data and image information</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_FreeImageBuffer_NET(ref MyCamera.MV_FRAME_OUT pFrame)
        {
            return MyCamera.MV_CC_FreeImageBuffer(this.handle, ref pFrame);
        }

        /// <summary>
        /// Get a frame of an image
        /// </summary>
        /// <param name="pData">Image data receiving buffer</param>
        /// <param name="nDataSize">Buffer size</param>
        /// <param name="pFrameInfo">Image information</param>
        /// <param name="nMsec">Waiting timeout</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetOneFrameTimeout_NET(IntPtr pData, uint nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, int nMsec)
        {
            return MyCamera.MV_CC_GetOneFrameTimeout(this.handle, pData, nDataSize, ref pFrameInfo, nMsec);
        }

        /// <summary>
        /// Clear image Buffers to clear old data
        /// </summary>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_ClearImageBuffer_NET()
        {
            return MyCamera.MV_CC_ClearImageBuffer(this.handle);
        }

        /// <summary>
        /// Display one frame image
        /// </summary>
        /// <param name="pDisplayInfo">Image information</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_DisplayOneFrame_NET(ref MyCamera.MV_DISPLAY_FRAME_INFO pDisplayInfo)
        {
            return MyCamera.MV_CC_DisplayOneFrame(this.handle, ref pDisplayInfo);
        }

        /// <summary>
        /// Set the number of the internal image cache nodes in SDK(Greater than or equal to 1, to be called before the capture)
        /// </summary>
        /// <param name="nNum">Number of cache nodes</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetImageNodeNum_NET(uint nNum)
        {
            return MyCamera.MV_CC_SetImageNodeNum(this.handle, nNum);
        }

        /// <summary>
        /// Set Grab Strategy
        /// </summary>
        /// <param name="enGrabStrategy">The value of grab strategy</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetGrabStrategy_NET(MyCamera.MV_GRAB_STRATEGY enGrabStrategy)
        {
            return MyCamera.MV_CC_SetGrabStrategy(this.handle, enGrabStrategy);
        }

        /// <summary>
        /// Set The Size of Output Queue(Only work under the strategy of MV_GrabStrategy_LatestImages，rang：1-ImageNodeNum)
        /// </summary>
        /// <param name="nOutputQueueSize">The Size of Output Queue</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetOutputQueueSize_NET(uint nOutputQueueSize)
        {
            return MyCamera.MV_CC_SetOutputQueueSize(this.handle, nOutputQueueSize);
        }

        /// <summary>
        /// Get device information(Called before start grabbing)
        /// </summary>
        /// <param name="pstDevInfo">device information</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetDeviceInfo_NET(ref MyCamera.MV_CC_DEVICE_INFO pstDevInfo)
        {
            return MyCamera.MV_CC_GetDeviceInfo(this.handle, ref pstDevInfo);
        }

        /// <summary>
        /// Get various type of information
        /// </summary>
        /// <param name="pstInfo">Various type of information</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetAllMatchInfo_NET(ref MyCamera.MV_ALL_MATCH_INFO pstInfo)
        {
            return MyCamera.MV_CC_GetAllMatchInfo(this.handle, ref pstInfo);
        }

        /// <summary>
        /// Get Integer value
        /// </summary>
        /// <param name="strKey">Key value, for example, using "Width" to get width</param>
        /// <param name="pstValue">Value of device features</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetIntValueEx_NET(string strKey, ref MyCamera.MVCC_INTVALUE_EX pstValue)
        {
            return MyCamera.MV_CC_GetIntValueEx(this.handle, strKey, ref pstValue);
        }

        /// <summary>
        /// Set Integer value
        /// </summary>
        /// <param name="strKey">Key value, for example, using "Width" to set width</param>
        /// <param name="nValue">Feature value to set</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetIntValueEx_NET(string strKey, long nValue)
        {
            return MyCamera.MV_CC_SetIntValueEx(this.handle, strKey, nValue);
        }

        /// <summary>
        /// Get Enum value
        /// </summary>
        /// <param name="strKey">Key value, for example, using "PixelFormat" to get pixel format</param>
        /// <param name="pstValue">Value of device features</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetEnumValue_NET(string strKey, ref MyCamera.MVCC_ENUMVALUE pstValue)
        {
            return MyCamera.MV_CC_GetEnumValue(this.handle, strKey, ref pstValue);
        }

        /// <summary>
        /// Set Enum value
        /// </summary>
        /// <param name="strKey">Key value, for example, using "PixelFormat" to set pixel format</param>
        /// <param name="nValue">Feature value to set</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetEnumValue_NET(string strKey, uint nValue)
        {
            return MyCamera.MV_CC_SetEnumValue(this.handle, strKey, nValue);
        }

        /// <summary>
        /// Set Enum value
        /// </summary>
        /// <param name="strKey">Key value, for example, using "PixelFormat" to set pixel format</param>
        /// <param name="sValue">Feature String to set</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetEnumValueByString_NET(string strKey, string sValue)
        {
            return MyCamera.MV_CC_SetEnumValueByString(this.handle, strKey, sValue);
        }

        /// <summary>
        /// Get Float value
        /// </summary>
        /// <param name="strKey">Key value</param>
        /// <param name="pstValue">Value of device features</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetFloatValue_NET(string strKey, ref MyCamera.MVCC_FLOATVALUE pstValue)
        {
            return MyCamera.MV_CC_GetFloatValue(this.handle, strKey, ref pstValue);
        }

        /// <summary>
        /// Set float value
        /// </summary>
        /// <param name="strKey">Key value</param>
        /// <param name="fValue">Feature value to set</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetFloatValue_NET(string strKey, float fValue)
        {
            return MyCamera.MV_CC_SetFloatValue(this.handle, strKey, fValue);
        }

        /// <summary>
        /// Get Boolean value
        /// </summary>
        /// <param name="strKey">Key value</param>
        /// <param name="pbValue">Value of device features</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetBoolValue_NET(string strKey, ref bool pbValue)
        {
            return MyCamera.MV_CC_GetBoolValue(this.handle, strKey, ref pbValue);
        }

        /// <summary>
        /// Set Boolean value
        /// </summary>
        /// <param name="strKey">Key value</param>
        /// <param name="bValue">Feature value to set</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetBoolValue_NET(string strKey, bool bValue)
        {
            return MyCamera.MV_CC_SetBoolValue(this.handle, strKey, bValue);
        }

        /// <summary>
        /// Get String value
        /// </summary>
        /// <param name="strKey">Key value</param>
        /// <param name="pstValue">Value of device features</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetStringValue_NET(string strKey, ref MyCamera.MVCC_STRINGVALUE pstValue)
        {
            return MyCamera.MV_CC_GetStringValue(this.handle, strKey, ref pstValue);
        }

        /// <summary>
        /// Set String value
        /// </summary>
        /// <param name="strKey">Key value</param>
        /// <param name="strValue">Feature value to set</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetStringValue_NET(string strKey, string strValue)
        {
            return MyCamera.MV_CC_SetStringValue(this.handle, strKey, strValue);
        }

        /// <summary>
        /// Send Command
        /// </summary>
        /// <param name="strKey">Key value</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetCommandValue_NET(string strKey)
        {
            return MyCamera.MV_CC_SetCommandValue(this.handle, strKey);
        }

        /// <summary>
        /// Invalidate GenICam Nodes
        /// </summary>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_InvalidateNodes_NET()
        {
            return MyCamera.MV_CC_InvalidateNodes(this.handle);
        }

        /// <summary>
        /// Device Local Upgrade
        /// </summary>
        /// <param name="pFilePathName">File path and name</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_LocalUpgrade_NET(string pFilePathName)
        {
            return MyCamera.MV_CC_LocalUpgrade(this.handle, pFilePathName);
        }

        /// <summary>
        /// Get Upgrade Progress
        /// </summary>
        /// <param name="pnProcess">Value of Progress</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_GetUpgradeProcess_NET(ref uint pnProcess)
        {
            return MyCamera.MV_CC_GetUpgradeProcess(this.handle, ref pnProcess);
        }

        /// <summary>
        /// Read Memory
        /// </summary>
        /// <param name="pBuffer">Used as a return value, save the read-in memory value(Memory value is stored in accordance with the big end model)</param>
        /// <param name="nAddress">Memory address to be read, which can be obtained from the Camera.xml file of the device, the form xml node value of xxx_RegAddr</param>
        /// <param name="nLength">Length of the memory to be read</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_ReadMemory_NET(IntPtr pBuffer, long nAddress, long nLength)
        {
            return MyCamera.MV_CC_ReadMemory(this.handle, pBuffer, nAddress, nLength);
        }

        /// <summary>
        /// Write Memory
        /// </summary>
        /// <param name="pBuffer">Memory value to be written ( Note the memory value to be stored in accordance with the big end model)</param>
        /// <param name="nAddress">Memory address to be written, which can be obtained from the Camera.xml file of the device, the form xml node value of xxx_RegAddr</param>
        /// <param name="nLength">Length of the memory to be written</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_WriteMemory_NET(IntPtr pBuffer, long nAddress, long nLength)
        {
            return MyCamera.MV_CC_WriteMemory(this.handle, pBuffer, nAddress, nLength);
        }

        /// <summary>
        /// Register Exception Message CallBack, call after open device
        /// </summary>
        /// <param name="cbException">Exception Message CallBack Function</param>
        /// <param name="pUser">User defined variable</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_RegisterExceptionCallBack_NET(MyCamera.cbExceptiondelegate cbException, IntPtr pUser)
        {
            return MyCamera.MV_CC_RegisterExceptionCallBack(this.handle, cbException, pUser);
        }

        /// <summary>
        /// Register event callback, which is called after the device is opened
        /// </summary>
        /// <param name="cbEvent">Event CallBack Function</param>
        /// <param name="pUser">User defined variable</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_RegisterAllEventCallBack_NET(MyCamera.cbEventdelegateEx cbEvent, IntPtr pUser)
        {
            return MyCamera.MV_CC_RegisterAllEventCallBack(this.handle, cbEvent, pUser);
        }

        /// <summary>
        /// Register single event callback, which is called after the device is opened
        /// </summary>
        /// <param name="pEventName">Event name</param>
        /// <param name="cbEvent">Event CallBack Function</param>
        /// <param name="pUser">User defined variable</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_RegisterEventCallBackEx_NET(string pEventName, MyCamera.cbEventdelegateEx cbEvent, IntPtr pUser)
        {
            return MyCamera.MV_CC_RegisterEventCallBackEx(this.handle, pEventName, cbEvent, pUser);
        }

        /// <summary>
        /// Force IP
        /// </summary>
        /// <param name="nIP">IP to set</param>
        /// <param name="nSubNetMask">Subnet mask</param>
        /// <param name="nDefaultGateWay">Default gateway</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_ForceIpEx_NET(uint nIP, uint nSubNetMask, uint nDefaultGateWay)
        {
            return MyCamera.MV_GIGE_ForceIpEx(this.handle, nIP, nSubNetMask, nDefaultGateWay);
        }

        /// <summary>
        /// IP configuration method
        /// </summary>
        /// <param name="nType">IP type, refer to MV_IP_CFG_x</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_SetIpConfig_NET(uint nType)
        {
            return MyCamera.MV_GIGE_SetIpConfig(this.handle, nType);
        }

        /// <summary>
        /// Set to use only one mode,type: MV_NET_TRANS_x. When do not set, priority is to use driver by default
        /// </summary>
        /// <param name="nType">Net transmission mode, refer to MV_NET_TRANS_x</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_SetNetTransMode_NET(uint nType)
        {
            return MyCamera.MV_GIGE_SetNetTransMode(this.handle, nType);
        }

        /// <summary>
        /// Get net transmission information
        /// </summary>
        /// <param name="pstInfo">Transmission information</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_GetNetTransInfo_NET(ref MyCamera.MV_NETTRANS_INFO pstInfo)
        {
            return MyCamera.MV_GIGE_GetNetTransInfo(this.handle, ref pstInfo);
        }

        /// <summary>
        /// Setting the ACK mode of devices Discovery
        /// </summary>
        /// <param name="nMode">ACK mode（Default-Broadcast）,0-Unicast,1-Broadcast</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_SetDiscoveryMode_NET(uint nMode)
        {
            return MyCamera.MV_GIGE_SetDiscoveryMode(nMode);
        }

        /// <summary>
        /// Set GVSP streaming timeout
        /// </summary>
        /// <param name="nMillisec">Timeout, default 300ms, range: &gt;10ms</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_SetGvspTimeout_NET(uint nMillisec)
        {
            return MyCamera.MV_GIGE_SetGvspTimeout(this.handle, nMillisec);
        }

        /// <summary>
        /// Get GVSP streaming timeout
        /// </summary>
        /// <param name="pMillisec">Timeout, ms as unit</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_GetGvspTimeout_NET(ref uint pMillisec)
        {
            return MyCamera.MV_GIGE_GetGvspTimeout(this.handle, ref pMillisec);
        }

        /// <summary>
        /// Set GVCP cammand timeout
        /// </summary>
        /// <param name="nMillisec">Timeout, ms as unit, range: 0-10000</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_SetGvcpTimeout_NET(uint nMillisec)
        {
            return MyCamera.MV_GIGE_SetGvcpTimeout(this.handle, nMillisec);
        }

        /// <summary>
        /// Get GVCP cammand timeout
        /// </summary>
        /// <param name="pMillisec">Timeout, ms as unit</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_GetGvcpTimeout_NET(ref uint pMillisec)
        {
            return MyCamera.MV_GIGE_GetGvcpTimeout(this.handle, ref pMillisec);
        }

        /// <summary>
        /// Set the number of retry GVCP cammand
        /// </summary>
        /// <param name="nRetryGvcpTimes">The number of retries，rang：0-100</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_SetRetryGvcpTimes_NET(uint nRetryGvcpTimes)
        {
            return MyCamera.MV_GIGE_SetRetryGvcpTimes(this.handle, nRetryGvcpTimes);
        }

        /// <summary>
        /// Get the number of retry GVCP cammand
        /// </summary>
        /// <param name="pRetryGvcpTimes">The number of retries</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_GetRetryGvcpTimes_NET(ref uint pRetryGvcpTimes)
        {
            return MyCamera.MV_GIGE_GetRetryGvcpTimes(this.handle, ref pRetryGvcpTimes);
        }

        /// <summary>
        /// Get the optimal Packet Size, Only support GigE Camera
        /// </summary>
        /// <returns>Optimal packet size</returns>
        public int MV_CC_GetOptimalPacketSize_NET()
        {
            return MyCamera.MV_CC_GetOptimalPacketSize(this.handle);
        }

        /// <summary>
        /// Set whethe to enable resend, and set resend
        /// </summary>
        /// <param name="bEnable">Enable resend</param>
        /// <param name="nMaxResendPercent">Max resend persent</param>
        /// <param name="nResendTimeout">Resend timeout</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_SetResend_NET(uint bEnable, uint nMaxResendPercent, uint nResendTimeout)
        {
            return MyCamera.MV_GIGE_SetResend(this.handle, bEnable, nMaxResendPercent, nResendTimeout);
        }

        /// <summary>
        /// Set the max resend retry times
        /// </summary>
        /// <param name="nRetryTimes">The max times to retry resending lost packets，default 20</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_SetResendMaxRetryTimes_NET(uint nRetryTimes)
        {
            return MyCamera.MV_GIGE_SetResendMaxRetryTimes(this.handle, nRetryTimes);
        }

        /// <summary>
        /// Get the max resend retry times
        /// </summary>
        /// <param name="pnRetryTimes">the max times to retry resending lost packets</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_GetResendMaxRetryTimes_NET(ref uint pnRetryTimes)
        {
            return MyCamera.MV_GIGE_GetResendMaxRetryTimes(this.handle, ref pnRetryTimes);
        }

        /// <summary>
        /// Set time interval between same resend requests
        /// </summary>
        /// <param name="nMillisec">The time interval between same resend requests,default 10ms</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_SetResendTimeInterval_NET(uint nMillisec)
        {
            return MyCamera.MV_GIGE_SetResendTimeInterval(this.handle, nMillisec);
        }

        /// <summary>
        /// Get time interval between same resend requests
        /// </summary>
        /// <param name="pnMillisec">The time interval between same resend requests</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_GetResendTimeInterval_NET(ref uint pnMillisec)
        {
            return MyCamera.MV_GIGE_GetResendTimeInterval(this.handle, ref pnMillisec);
        }

        /// <summary>
        /// Set transmission type,Unicast or Multicast
        /// </summary>
        /// <param name="pstTransmissionType">Struct of transmission type</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_GIGE_SetTransmissionType_NET(ref MyCamera.MV_CC_TRANSMISSION_TYPE pstTransmissionType)
        {
            return MyCamera.MV_GIGE_SetTransmissionType(this.handle, ref pstTransmissionType);
        }

        /// <summary>
        /// Issue Action Command
        /// </summary>
        /// <param name="pstActionCmdInfo">Action Command info</param>
        /// <param name="pstActionCmdResults">Action Command Result List</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_GIGE_IssueActionCommand_NET(ref MyCamera.MV_ACTION_CMD_INFO pstActionCmdInfo, ref MyCamera.MV_ACTION_CMD_RESULT_LIST pstActionCmdResults)
        {
            return MyCamera.MV_GIGE_IssueActionCommand(ref pstActionCmdInfo, ref pstActionCmdResults);
        }

        /// <summary>
        /// Get Multicast Status
        /// </summary>
        /// <param name="pstDevInfo">Device Information</param>
        /// <param name="pStatus">Status of Multicast</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public static int MV_GIGE_GetMulticastStatus_NET(ref MyCamera.MV_CC_DEVICE_INFO pstDevInfo, ref bool pStatus)
        {
            return MyCamera.MV_GIGE_GetMulticastStatus(ref pstDevInfo, ref pStatus);
        }

        /// <summary>
        /// Set device baudrate using one of the CL_BAUDRATE_XXXX value
        /// </summary>
        /// <param name="nBaudrate">Baudrate to set. Refer to the 'CameraParams.h' for parameter definitions, for example, #define MV_CAML_BAUDRATE_9600  0x00000001</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CAML_SetDeviceBaudrate_NET(uint nBaudrate)
        {
            return MyCamera.MV_CAML_SetDeviceBaudrate(this.handle, nBaudrate);
        }

        public int MV_CAML_SetDeviceBauderate_NET(uint nBaudrate)
        {
            return MyCamera.MV_CAML_SetDeviceBaudrate(this.handle, nBaudrate);
        }

        /// <summary>
        /// Get device baudrate, using one of the CL_BAUDRATE_XXXX value
        /// </summary>
        /// <param name="pnCurrentBaudrate">Return pointer of baud rate to user. 
        ///                                 Refer to the 'CameraParams.h' for parameter definitions, for example, #define MV_CAML_BAUDRATE_9600  0x00000001</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CAML_GetDeviceBaudrate_NET(ref uint pnCurrentBaudrate)
        {
            return MyCamera.MV_CAML_GetDeviceBaudrate(this.handle, ref pnCurrentBaudrate);
        }

        public int MV_CAML_GetDeviceBauderate_NET(ref uint pnCurrentBaudrate)
        {
            return MyCamera.MV_CAML_GetDeviceBaudrate(this.handle, ref pnCurrentBaudrate);
        }

        /// <summary>
        /// Get supported baudrates of the combined device and host interface
        /// </summary>
        /// <param name="pnBaudrateAblity">Return pointer of the supported baudrates to user. 'OR' operation results of the supported baudrates. 
        ///                                Refer to the 'CameraParams.h' for single value definitions, for example, #define MV_CAML_BAUDRATE_9600  0x00000001</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CAML_GetSupportBaudrates_NET(ref uint pnBaudrateAblity)
        {
            return MyCamera.MV_CAML_GetSupportBaudrates(this.handle, ref pnBaudrateAblity);
        }

        public int MV_CAML_GetSupportBauderates_NET(ref uint pnBaudrateAblity)
        {
            return MyCamera.MV_CAML_GetSupportBaudrates(this.handle, ref pnBaudrateAblity);
        }

        /// <summary>
        /// Sets the timeout for operations on the serial port
        /// </summary>
        /// <param name="nMillisec">Timeout in [ms] for operations on the serial port.</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CAML_SetGenCPTimeOut_NET(uint nMillisec)
        {
            return MyCamera.MV_CAML_SetGenCPTimeOut(this.handle, nMillisec);
        }

        /// <summary>
        /// Set transfer size of U3V device
        /// </summary>
        /// <param name="nTransferSize">Transfer size，Byte，default：1M，rang：&gt;=0x10000</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_USB_SetTransferSize_NET(uint nTransferSize)
        {
            return MyCamera.MV_USB_SetTransferSize(this.handle, nTransferSize);
        }

        /// <summary>
        /// Get transfer size of U3V device
        /// </summary>
        /// <param name="pTransferSize">Transfer size，Byte</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_USB_GetTransferSize_NET(ref uint pTransferSize)
        {
            return MyCamera.MV_USB_GetTransferSize(this.handle, ref pTransferSize);
        }

        /// <summary>
        /// Set transfer ways of U3V device
        /// </summary>
        /// <param name="nTransferWays">Transfer ways，rang：1-10</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_USB_SetTransferWays_NET(uint nTransferWays)
        {
            return MyCamera.MV_USB_SetTransferWays(this.handle, nTransferWays);
        }

        /// <summary>
        /// Get transfer ways of U3V device
        /// </summary>
        /// <param name="pTransferWays">Transfer ways</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_USB_GetTransferWays_NET(ref uint pTransferWays)
        {
            return MyCamera.MV_USB_GetTransferWays(this.handle, ref pTransferWays);
        }

        /// <summary>
        /// Enumerate interfaces by GenTL
        /// </summary>
        /// <param name="stIFInfoList"> Interface information list</param>
        /// <param name="pGenTLPath">Path of GenTL's cti file</param>
        /// <returns></returns>
        public static int MV_CC_EnumInterfacesByGenTL_NET(ref MyCamera.MV_GENTL_IF_INFO_LIST stIFInfoList, string pGenTLPath)
        {
            return MyCamera.MV_CC_EnumInterfacesByGenTL(ref stIFInfoList, pGenTLPath);
        }

        /// <summary>
        /// Enumerate Device Based On GenTL
        /// </summary>
        /// <param name="stIFInfo">Interface information</param>
        /// <param name="stDevList">Device List</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public static int MV_CC_EnumDevicesByGenTL_NET(ref MyCamera.MV_GENTL_IF_INFO stIFInfo, ref MyCamera.MV_GENTL_DEV_INFO_LIST stDevList)
        {
            return MyCamera.MV_CC_EnumDevicesByGenTL(ref stIFInfo, ref stDevList);
        }

        /// <summary>
        /// Create Device Handle Based On GenTL Device Info
        /// </summary>
        /// <param name="stDevInfo">Device Information Structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_CreateDeviceByGenTL_NET(ref MyCamera.MV_GENTL_DEV_INFO stDevInfo)
        {
            if (IntPtr.Zero != this.handle)
            {
                MyCamera.MV_CC_DestroyHandle(this.handle);
                this.handle = IntPtr.Zero;
            }
            return MyCamera.MV_CC_CreateHandleByGenTL(ref this.handle, ref stDevInfo);
        }

        /// <summary>
        /// Get camera feature tree XML
        /// </summary>
        /// <param name="pData">XML data receiving buffer</param>
        /// <param name="nDataSize">Buffer size</param>
        /// <param name="pnDataLen">Actual data length</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_XML_GetGenICamXML_NET(IntPtr pData, uint nDataSize, ref uint pnDataLen)
        {
            return MyCamera.MV_XML_GetGenICamXML(this.handle, pData, nDataSize, ref pnDataLen);
        }

        /// <summary>
        /// Get Access mode of cur node
        /// </summary>
        /// <param name="pstrName">Name of node</param>
        /// <param name="pAccessMode">Access mode of the node</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_XML_GetNodeAccessMode_NET(string pstrName, ref MyCamera.MV_XML_AccessMode pAccessMode)
        {
            return MyCamera.MV_XML_GetNodeAccessMode(this.handle, pstrName, ref pAccessMode);
        }

        /// <summary>
        /// Get Interface Type of cur node
        /// </summary>
        /// <param name="pstrName">Name of node</param>
        /// <param name="pInterfaceType">Interface Type of the node</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_XML_GetNodeInterfaceType_NET(string pstrName, ref MyCamera.MV_XML_InterfaceType pInterfaceType)
        {
            return MyCamera.MV_XML_GetNodeInterfaceType(this.handle, pstrName, ref pInterfaceType);
        }

        /// <summary>
        /// Save image, support Bmp and Jpeg. Encoding quality(50-99]
        /// </summary>
        /// <param name="stSaveParam">Save image parameters structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_SaveImageEx_NET(ref MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam)
        {
            return MyCamera.MV_CC_SaveImageEx2(this.handle, ref stSaveParam);
        }

        /// <summary>
        /// Save the image file, support Bmp、 Jpeg、Png and Tiff. Encoding quality(50-99]
        /// </summary>
        /// <param name="pstSaveFileParam">Save the image file parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_SaveImageToFile_NET(ref MyCamera.MV_SAVE_IMG_TO_FILE_PARAM pstSaveFileParam)
        {
            return MyCamera.MV_CC_SaveImageToFile(this.handle, ref pstSaveFileParam);
        }

        /// <summary>
        /// Save 3D point data, support PLY、CSV and OBJ
        /// </summary>
        /// <param name="pstPointDataParam">Save 3D point data parameters structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SavePointCloudData_NET(ref MyCamera.MV_SAVE_POINT_CLOUD_PARAM pstPointDataParam)
        {
            return MyCamera.MV_CC_SavePointCloudData(this.handle, ref pstPointDataParam);
        }

        /// <summary>
        /// Rotate Image
        /// </summary>
        /// <param name="pstRotateParam">Rotate image parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_RotateImage_NET(ref MyCamera.MV_CC_ROTATE_IMAGE_PARAM pstRotateParam)
        {
            return MyCamera.MV_CC_RotateImage(this.handle, ref pstRotateParam);
        }

        /// <summary>
        /// Flip Image
        /// </summary>
        /// <param name="pstFlipParam">Flip image parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_FlipImage_NET(ref MyCamera.MV_CC_FLIP_IMAGE_PARAM pstFlipParam)
        {
            return MyCamera.MV_CC_FlipImage(this.handle, ref pstFlipParam);
        }

        /// <summary>
        /// Pixel format conversion
        /// </summary>
        /// <param name="pstCvtParam">Convert Pixel Type parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_ConvertPixelType_NET(ref MyCamera.MV_PIXEL_CONVERT_PARAM pstCvtParam)
        {
            return MyCamera.MV_CC_ConvertPixelType(this.handle, ref pstCvtParam);
        }

        /// <summary>
        /// Interpolation algorithm type setting
        /// </summary>
        /// <param name="BayerCvtQuality">Bayer interpolation method  0-Fast 1-Equilibrium 2-Optimal</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_SetBayerCvtQuality_NET(uint BayerCvtQuality)
        {
            return MyCamera.MV_CC_SetBayerCvtQuality(this.handle, BayerCvtQuality);
        }

        /// <summary>
        /// Set Gamma value
        /// </summary>
        /// <param name="fBayerGammaValue">Gamma value[0.1,4.0]</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_SetBayerGammaValue_NET(float fBayerGammaValue)
        {
            return MyCamera.MV_CC_SetBayerGammaValue(this.handle, fBayerGammaValue);
        }

        /// <summary>
        /// Set Gamma param
        /// </summary>
        /// <param name="pstGammaParam">Gamma parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetBayerGammaParam_NET(ref MyCamera.MV_CC_GAMMA_PARAM pstGammaParam)
        {
            return MyCamera.MV_CC_SetBayerGammaParam(this.handle, ref pstGammaParam);
        }

        /// <summary>
        /// Set CCM param
        /// </summary>
        /// <param name="pstCCMParam">CCM parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetBayerCCMParam_NET(ref MyCamera.MV_CC_CCM_PARAM pstCCMParam)
        {
            return MyCamera.MV_CC_SetBayerCCMParam(this.handle, ref pstCCMParam);
        }

        /// <summary>
        /// Set CCM param
        /// </summary>
        /// <param name="pstCCMParam">CCM parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetBayerCCMParamEx_NET(ref MyCamera.MV_CC_CCM_PARAM_EX pstCCMParam)
        {
            return MyCamera.MV_CC_SetBayerCCMParamEx(this.handle, ref pstCCMParam);
        }

        /// <summary>
        /// Set CLUT param
        /// </summary>
        /// <param name="pstCLUTParam">CLUT parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SetBayerCLUTParam_NET(ref MyCamera.MV_CC_CLUT_PARAM pstCLUTParam)
        {
            return MyCamera.MV_CC_SetBayerCLUTParam(this.handle, ref pstCLUTParam);
        }

        /// <summary>
        /// Adjust image contrast
        /// </summary>
        /// <param name="pstContrastParam">Contrast parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_ImageContrast_NET(ref MyCamera.MV_CC_CONTRAST_PARAM pstContrastParam)
        {
            return MyCamera.MV_CC_ImageContrast(this.handle, ref pstContrastParam);
        }

        /// <summary>
        /// Image sharpen
        /// </summary>
        /// <param name="pstSharpenParam">Sharpen parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_ImageSharpen_NET(ref MyCamera.MV_CC_SHARPEN_PARAM pstSharpenParam)
        {
            return MyCamera.MV_CC_ImageSharpen(this.handle, ref pstSharpenParam);
        }

        /// <summary>
        /// Color Correct(include CCM and CLUT)
        /// </summary>
        /// <param name="pstColorCorrectParam">Color Correct parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_ColorCorrect_NET(ref MyCamera.MV_CC_COLOR_CORRECT_PARAM pstColorCorrectParam)
        {
            return MyCamera.MV_CC_ColorCorrect(this.handle, ref pstColorCorrectParam);
        }

        /// <summary>
        /// Noise Estimate
        /// </summary>
        /// <param name="pstNoiseEstimateParam">Noise Estimate parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_NoiseEstimate_NET(ref MyCamera.MV_CC_NOISE_ESTIMATE_PARAM pstNoiseEstimateParam)
        {
            return MyCamera.MV_CC_NoiseEstimate(this.handle, ref pstNoiseEstimateParam);
        }

        /// <summary>
        /// Spatial Denoise
        /// </summary>
        /// <param name="pstSpatialDenoiseParam">Spatial Denoise parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_SpatialDenoise_NET(ref MyCamera.MV_CC_SPATIAL_DENOISE_PARAM pstSpatialDenoiseParam)
        {
            return MyCamera.MV_CC_SpatialDenoise(this.handle, ref pstSpatialDenoiseParam);
        }

        /// <summary>
        /// LSC Calib
        /// </summary>
        /// <param name="pstLSCCalibParam">LSC Calib parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_LSCCalib_NET(ref MyCamera.MV_CC_LSC_CALIB_PARAM pstLSCCalibParam)
        {
            return MyCamera.MV_CC_LSCCalib(this.handle, ref pstLSCCalibParam);
        }

        /// <summary>
        /// LSC Correct
        /// </summary>
        /// <param name="pstLSCCorrectParam">LSC Correct parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_LSCCorrect_NET(ref MyCamera.MV_CC_LSC_CORRECT_PARAM pstLSCCorrectParam)
        {
            return MyCamera.MV_CC_LSCCorrect(this.handle, ref pstLSCCorrectParam);
        }

        /// <summary>
        /// High Bandwidth Decode
        /// </summary>
        /// <param name="pstDecodeParam">High Bandwidth Decode parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_HB_Decode_NET(ref MyCamera.MV_CC_HB_DECODE_PARAM pstDecodeParam)
        {
            return MyCamera.MV_CC_HB_Decode(this.handle, ref pstDecodeParam);
        }

        /// <summary>
        /// Noise estimate of Bayer format
        /// </summary>
        /// <param name="pstNoiseEstimateParam">Noise estimate parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_BayerNoiseEstimate_NET(ref MyCamera.MV_CC_BAYER_NOISE_ESTIMATE_PARAM pstNoiseEstimateParam)
        {
            return MyCamera.MV_CC_BayerNoiseEstimate(this.handle, ref pstNoiseEstimateParam);
        }

        /// <summary>
        /// Spatial Denoise of Bayer format
        /// </summary>
        /// <param name="pstSpatialDenoiseParam">Spatial Denoise parameter structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code</returns>
        public int MV_CC_BayerSpatialDenoise_NET(ref MyCamera.MV_CC_BAYER_SPATIAL_DENOISE_PARAM pstSpatialDenoiseParam)
        {
            return MyCamera.MV_CC_BayerSpatialDenoise(this.handle, ref pstSpatialDenoiseParam);
        }

        /// <summary>
        /// Save camera feature
        /// </summary>
        /// <param name="pFileName">File name</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_FeatureSave_NET(string pFileName)
        {
            return MyCamera.MV_CC_FeatureSave(this.handle, pFileName);
        }

        /// <summary>
        /// Load camera feature
        /// </summary>
        /// <param name="pFileName">File name</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_FeatureLoad_NET(string pFileName)
        {
            return MyCamera.MV_CC_FeatureLoad(this.handle, pFileName);
        }

        /// <summary>
        /// Read the file from the camera
        /// </summary>
        /// <param name="pstFileAccess">File access structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_FileAccessRead_NET(ref MyCamera.MV_CC_FILE_ACCESS pstFileAccess)
        {
            return MyCamera.MV_CC_FileAccessRead(this.handle, ref pstFileAccess);
        }

        /// <summary>
        /// Write the file to camera
        /// </summary>
        /// <param name="pstFileAccess">File access structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_FileAccessWrite_NET(ref MyCamera.MV_CC_FILE_ACCESS pstFileAccess)
        {
            return MyCamera.MV_CC_FileAccessWrite(this.handle, ref pstFileAccess);
        }

        /// <summary>
        /// Get File Access Progress 
        /// </summary>
        /// <param name="pstFileAccessProgress">File access Progress</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_GetFileAccessProgress_NET(ref MyCamera.MV_CC_FILE_ACCESS_PROGRESS pstFileAccessProgress)
        {
            return MyCamera.MV_CC_GetFileAccessProgress(this.handle, ref pstFileAccessProgress);
        }

        /// <summary>
        /// Start Record
        /// </summary>
        /// <param name="pstRecordParam">Record param structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_StartRecord_NET(ref MyCamera.MV_CC_RECORD_PARAM pstRecordParam)
        {
            return MyCamera.MV_CC_StartRecord(this.handle, ref pstRecordParam);
        }

        /// <summary>
        /// Input RAW data to Record
        /// </summary>
        /// <param name="pstInputFrameInfo">Record data structure</param>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_InputOneFrame_NET(ref MyCamera.MV_CC_INPUT_FRAME_INFO pstInputFrameInfo)
        {
            return MyCamera.MV_CC_InputOneFrame(this.handle, ref pstInputFrameInfo);
        }

        /// <summary>
        /// Stop Record
        /// </summary>
        /// <returns>Success, return MV_OK. Failure, return error code </returns>
        public int MV_CC_StopRecord_NET()
        {
            return MyCamera.MV_CC_StopRecord(this.handle);
        }

        /// <summary>
        /// Set SDK log path (Interfaces not recommended)
        /// If the logging service MvLogServer is enabled, the interface is invalid and The logging service is enabled by default
        /// </summary>
        /// <param name="pSDKLogPath"></param>
        /// <returns></returns>
        public static int MV_CC_SetSDKLogPath_NET(string pSDKLogPath)
        {
            return MyCamera.MV_CC_SetSDKLogPath(pSDKLogPath);
        }

        /// <summary>
        /// Get basic information of image (Interfaces not recommended)
        /// </summary>
        /// <param name="pstInfo"></param>
        /// <returns></returns>
        public int MV_CC_GetImageInfo_NET(ref MyCamera.MV_IMAGE_BASIC_INFO pstInfo)
        {
            return MyCamera.MV_CC_GetImageInfo(this.handle, ref pstInfo);
        }

        /// <summary>
        /// Get GenICam proxy (Interfaces not recommended)
        /// </summary>
        /// <returns></returns>
        public IntPtr MV_CC_GetTlProxy_NET()
        {
            return MyCamera.MV_CC_GetTlProxy(this.handle);
        }

        /// <summary>
        /// Get root node (Interfaces not recommended)
        /// </summary>
        /// <param name="pstNode"></param>
        /// <returns></returns>
        public int MV_XML_GetRootNode_NET(ref MyCamera.MV_XML_NODE_FEATURE pstNode)
        {
            return MyCamera.MV_XML_GetRootNode(this.handle, ref pstNode);
        }

        /// <summary>
        /// Get all children node of specific node from xml, root node is Root (Interfaces not recommended)
        /// </summary>
        /// <param name="pstNode"></param>
        /// <param name="pstNodesList"></param>
        /// <returns></returns>
        public int MV_XML_GetChildren_NET(ref MyCamera.MV_XML_NODE_FEATURE pstNode, IntPtr pstNodesList)
        {
            return MyCamera.MV_XML_GetChildren(this.handle, ref pstNode, pstNodesList);
        }

        /// <summary>
        /// Get all children node of specific node from xml, root node is Root (Interfaces not recommended)
        /// </summary>
        /// <param name="pstNode"></param>
        /// <param name="pstNodesList"></param>
        /// <returns></returns>
        public int MV_XML_GetChildren_NET(ref MyCamera.MV_XML_NODE_FEATURE pstNode, ref MyCamera.MV_XML_NODES_LIST pstNodesList)
        {
            return MyCamera.MV_XML_GetChildren(this.handle, ref pstNode, ref pstNodesList);
        }

        /// <summary>
        /// Get current node feature (Interfaces not recommended)
        /// </summary>
        /// <param name="pstNode"></param>
        /// <param name="pstFeature"></param>
        /// <returns></returns>
        public int MV_XML_GetNodeFeature_NET(ref MyCamera.MV_XML_NODE_FEATURE pstNode, IntPtr pstFeature)
        {
            return MyCamera.MV_XML_GetNodeFeature(this.handle, ref pstNode, pstFeature);
        }

        /// <summary>
        /// Update node (Interfaces not recommended)
        /// </summary>
        /// <param name="enType"></param>
        /// <param name="pstFeature"></param>
        /// <returns></returns>
        public int MV_XML_UpdateNodeFeature_NET(MyCamera.MV_XML_InterfaceType enType, IntPtr pstFeature)
        {
            return MyCamera.MV_XML_UpdateNodeFeature(this.handle, enType, pstFeature);
        }

        /// <summary>
        /// Register update callback (Interfaces not recommended)
        /// </summary>
        /// <param name="cbXmlUpdate"></param>
        /// <param name="pUser"></param>
        /// <returns></returns>
        public int MV_XML_RegisterUpdateCallBack_NET(MyCamera.cbXmlUpdatedelegate cbXmlUpdate, IntPtr pUser)
        {
            return MyCamera.MV_XML_RegisterUpdateCallBack(this.handle, cbXmlUpdate, pUser);
        }

        /// <summary>
        /// This interface is abandoned, it is recommended to use the MV_CC_GetOneFrameTimeOut
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="nDataSize"></param>
        /// <param name="pFrameInfo"></param>
        /// <returns></returns>
        public int MV_CC_GetOneFrame_NET(IntPtr pData, uint nDataSize, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo)
        {
            return MyCamera.MV_CC_GetOneFrame(this.handle, pData, nDataSize, ref pFrameInfo);
        }

        /// <summary>
        /// This interface is abandoned, it is recommended to use the MV_CC_GetOneFrameTimeOut
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="nDataSize"></param>
        /// <param name="pFrameInfo"></param>
        /// <returns></returns>
        public int MV_CC_GetOneFrameEx_NET(IntPtr pData, uint nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo)
        {
            return MyCamera.MV_CC_GetOneFrameEx(this.handle, pData, nDataSize, ref pFrameInfo);
        }

        /// <summary>
        /// This interface is abandoned, it is recommended to use the MV_CC_RegisterImageCallBackEx
        /// </summary>
        /// <param name="cbOutput"></param>
        /// <param name="pUser"></param>
        /// <returns></returns>
        public int MV_CC_RegisterImageCallBack_NET(MyCamera.cbOutputdelegate cbOutput, IntPtr pUser)
        {
            return MyCamera.MV_CC_RegisterImageCallBack(this.handle, cbOutput, pUser);
        }

        /// <summary>
        /// This interface is abandoned, it is recommended to use the MV_CC_SaveImageEx
        /// </summary>
        /// <param name="stSaveParam"></param>
        /// <returns></returns>
        public int MV_CC_SaveImage_NET(ref MyCamera.MV_SAVE_IMAGE_PARAM stSaveParam)
        {
            return MyCamera.MV_CC_SaveImage(ref stSaveParam);
        }

        /// <summary>
        /// This interface is abandoned, it is recommended to use the MV_GIGE_ForceIpEx
        /// </summary>
        /// <param name="nIP"></param>
        /// <returns></returns>
        public int MV_GIGE_ForceIp_NET(uint nIP)
        {
            return MyCamera.MV_GIGE_ForceIp(this.handle, nIP);
        }

        /// <summary>
        /// This interface is abandoned, it is recommended to use the MV_CC_RegisterEventCallBackEx
        /// </summary>
        /// <param name="cbEvent"></param>
        /// <param name="pUser"></param>
        /// <returns></returns>
        public int MV_CC_RegisterEventCallBack_NET(MyCamera.cbEventdelegate cbEvent, IntPtr pUser)
        {
            return MyCamera.MV_CC_RegisterEventCallBack(this.handle, cbEvent, pUser);
        }

        /// <summary>
        /// This interface is abandoned, it is recommended to use the MV_CC_DisplayOneFrame
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public int MV_CC_Display_NET(IntPtr hWnd)
        {
            return MyCamera.MV_CC_Display(this.handle, hWnd);
        }

        /// <summary>
        /// This interface is abandoned, it is recommended to use the MV_CC_GetIntValueEx
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetIntValue_NET(string strKey, ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetIntValue(this.handle, strKey, ref pstValue);
        }

        /// <summary>
        /// This interface is abandoned, it is recommended to use the MV_CC_SetIntValueEx
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetIntValue_NET(string strKey, uint nValue)
        {
            return MyCamera.MV_CC_SetIntValue(this.handle, strKey, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetWidth_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetWidth(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetWidth_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetWidth(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetHeight_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetHeight(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetHeight_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetHeight(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetAOIoffsetX_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetAOIoffsetX(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetAOIoffsetX_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetAOIoffsetX(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetAOIoffsetY_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetAOIoffsetY(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetAOIoffsetY_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetAOIoffsetY(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetAutoExposureTimeLower_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetAutoExposureTimeLower(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetAutoExposureTimeLower_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetAutoExposureTimeLower(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetAutoExposureTimeUpper_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetAutoExposureTimeUpper(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetAutoExposureTimeUpper_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetAutoExposureTimeUpper(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetBrightness_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetBrightness(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetBrightness_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetBrightness(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetFrameRate_NET(ref MyCamera.MVCC_FLOATVALUE pstValue)
        {
            return MyCamera.MV_CC_GetFrameRate(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="fValue"></param>
        /// <returns></returns>
        public int MV_CC_SetFrameRate_NET(float fValue)
        {
            return MyCamera.MV_CC_SetFrameRate(this.handle, fValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetGain_NET(ref MyCamera.MVCC_FLOATVALUE pstValue)
        {
            return MyCamera.MV_CC_GetGain(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="fValue"></param>
        /// <returns></returns>
        public int MV_CC_SetGain_NET(float fValue)
        {
            return MyCamera.MV_CC_SetGain(this.handle, fValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetExposureTime_NET(ref MyCamera.MVCC_FLOATVALUE pstValue)
        {
            return MyCamera.MV_CC_GetExposureTime(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="fValue"></param>
        /// <returns></returns>
        public int MV_CC_SetExposureTime_NET(float fValue)
        {
            return MyCamera.MV_CC_SetExposureTime(this.handle, fValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetPixelFormat_NET(ref MyCamera.MVCC_ENUMVALUE pstValue)
        {
            return MyCamera.MV_CC_GetPixelFormat(this.handle, ref pstValue);
        }

        public int MV_CC_SetPixelFormat_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetPixelFormat(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetAcquisitionMode_NET(ref MyCamera.MVCC_ENUMVALUE pstValue)
        {
            return MyCamera.MV_CC_GetAcquisitionMode(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetAcquisitionMode_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetAcquisitionMode(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetGainMode_NET(ref MyCamera.MVCC_ENUMVALUE pstValue)
        {
            return MyCamera.MV_CC_GetGainMode(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetGainMode_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetGainMode(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetExposureAutoMode_NET(ref MyCamera.MVCC_ENUMVALUE pstValue)
        {
            return MyCamera.MV_CC_GetExposureAutoMode(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetExposureAutoMode_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetExposureAutoMode(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetTriggerMode_NET(ref MyCamera.MVCC_ENUMVALUE pstValue)
        {
            return MyCamera.MV_CC_GetTriggerMode(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetTriggerMode_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetTriggerMode(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetTriggerDelay_NET(ref MyCamera.MVCC_FLOATVALUE pstValue)
        {
            return MyCamera.MV_CC_GetTriggerDelay(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="fValue"></param>
        /// <returns></returns>
        public int MV_CC_SetTriggerDelay_NET(float fValue)
        {
            return MyCamera.MV_CC_SetTriggerDelay(this.handle, fValue);
        }

        public int MV_CC_GetTriggerSource_NET(ref MyCamera.MVCC_ENUMVALUE pstValue)
        {
            return MyCamera.MV_CC_GetTriggerSource(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetTriggerSource_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetTriggerSource(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <returns></returns>
        public int MV_CC_TriggerSoftwareExecute_NET()
        {
            return MyCamera.MV_CC_TriggerSoftwareExecute(this.handle);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetGammaSelector_NET(ref MyCamera.MVCC_ENUMVALUE pstValue)
        {
            return MyCamera.MV_CC_GetGammaSelector(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetGammaSelector_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetGammaSelector(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetGamma_NET(ref MyCamera.MVCC_FLOATVALUE pstValue)
        {
            return MyCamera.MV_CC_GetGamma(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="fValue"></param>
        /// <returns></returns>
        public int MV_CC_SetGamma_NET(float fValue)
        {
            return MyCamera.MV_CC_SetGamma(this.handle, fValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetSharpness_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetSharpness(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetSharpness_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetSharpness(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetHue_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetHue(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetHue_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetHue(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetSaturation_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetSaturation(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetSaturation_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetSaturation(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetBalanceWhiteAuto_NET(ref MyCamera.MVCC_ENUMVALUE pstValue)
        {
            return MyCamera.MV_CC_GetBalanceWhiteAuto(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetBalanceWhiteAuto_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetBalanceWhiteAuto(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetBalanceRatioRed_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetBalanceRatioRed(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetBalanceRatioRed_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetBalanceRatioRed(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetBalanceRatioGreen_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetBalanceRatioGreen(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetBalanceRatioGreen_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetBalanceRatioGreen(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetBalanceRatioBlue_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetBalanceRatioBlue(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetBalanceRatioBlue_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetBalanceRatioBlue(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetDeviceUserID_NET(ref MyCamera.MVCC_STRINGVALUE pstValue)
        {
            return MyCamera.MV_CC_GetDeviceUserID(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="chValue"></param>
        /// <returns></returns>
        public int MV_CC_SetDeviceUserID_NET(string chValue)
        {
            return MyCamera.MV_CC_SetDeviceUserID(this.handle, chValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetBurstFrameCount_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetBurstFrameCount(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetBurstFrameCount_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetBurstFrameCount(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetAcquisitionLineRate_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetAcquisitionLineRate(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetAcquisitionLineRate_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetAcquisitionLineRate(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_CC_GetHeartBeatTimeout_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_CC_GetHeartBeatTimeout(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_CC_SetHeartBeatTimeout_NET(uint nValue)
        {
            return MyCamera.MV_CC_SetHeartBeatTimeout(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_GIGE_GetGevSCPSPacketSize_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_GIGE_GetGevSCPSPacketSize(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_GIGE_SetGevSCPSPacketSize_NET(uint nValue)
        {
            return MyCamera.MV_GIGE_SetGevSCPSPacketSize(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pstValue"></param>
        /// <returns></returns>
        public int MV_GIGE_GetGevSCPD_NET(ref MyCamera.MVCC_INTVALUE pstValue)
        {
            return MyCamera.MV_GIGE_GetGevSCPD(this.handle, ref pstValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public int MV_GIGE_SetGevSCPD_NET(uint nValue)
        {
            return MyCamera.MV_GIGE_SetGevSCPD(this.handle, nValue);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pnIP"></param>
        /// <returns></returns>
        public int MV_GIGE_GetGevSCDA_NET(ref uint pnIP)
        {
            return MyCamera.MV_GIGE_GetGevSCDA(this.handle, ref pnIP);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nIP"></param>
        /// <returns></returns>
        public int MV_GIGE_SetGevSCDA_NET(uint nIP)
        {
            return MyCamera.MV_GIGE_SetGevSCDA(this.handle, nIP);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="pnPort"></param>
        /// <returns></returns>
        public int MV_GIGE_GetGevSCSP_NET(ref uint pnPort)
        {
            return MyCamera.MV_GIGE_GetGevSCSP(this.handle, ref pnPort);
        }

        /// <summary>
        /// This interface is replaced by general interface
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        public int MV_GIGE_SetGevSCSP_NET(uint nPort)
        {
            return MyCamera.MV_GIGE_SetGevSCSP(this.handle, nPort);
        }

        /// <summary>
        /// Get Camera Handle
        /// </summary>
        /// <returns></returns>
        public IntPtr GetCameraHandle()
        {
            return this.handle;
        }

        /// <summary>
        /// Byte array to struct
        /// </summary>
        /// <param name="bytes">Byte array</param>
        /// <param name="type">Struct type</param>
        /// <returns>Struct object</returns>
        public static object ByteToStruct(byte[] bytes, Type type)
        {
            int num = Marshal.SizeOf(type);
            if (num > bytes.Length)
            {
                return null;
            }
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.Copy(bytes, 0, intPtr, num);
            object result = Marshal.PtrToStructure(intPtr, type);
            Marshal.FreeHGlobal(intPtr);
            return result;
        }

        [DllImport("MvCameraControl.dll")]
        private static extern uint MV_CC_GetSDKVersion();

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_EnumerateTls();

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_EnumDevices(uint nTLayerType, ref MyCamera.MV_CC_DEVICE_INFO_LIST stDevList);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_EnumDevicesEx(uint nTLayerType, ref MyCamera.MV_CC_DEVICE_INFO_LIST stDevList, string pManufacturerName);

        [DllImport("MvCameraControl.dll")]
        private static extern bool MV_CC_IsDeviceAccessible(ref MyCamera.MV_CC_DEVICE_INFO stDevInfo, uint nAccessMode);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetSDKLogPath(string pSDKLogPath);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_CreateHandle(ref IntPtr handle, ref MyCamera.MV_CC_DEVICE_INFO stDevInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_CreateHandleWithoutLog(ref IntPtr handle, ref MyCamera.MV_CC_DEVICE_INFO stDevInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_DestroyHandle(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_OpenDevice(IntPtr handle, uint nAccessMode, ushort nSwitchoverKey);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_CloseDevice(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern bool MV_CC_IsDeviceConnected(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_RegisterImageCallBackEx(IntPtr handle, MyCamera.cbOutputExdelegate cbOutput, IntPtr pUser);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_RegisterImageCallBackForRGB(IntPtr handle, MyCamera.cbOutputExdelegate cbOutput, IntPtr pUser);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_RegisterImageCallBackForBGR(IntPtr handle, MyCamera.cbOutputExdelegate cbOutput, IntPtr pUser);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_StartGrabbing(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_StopGrabbing(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetImageForRGB(IntPtr handle, IntPtr pData, uint nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, int nMsec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetImageForBGR(IntPtr handle, IntPtr pData, uint nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, int nMsec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetImageBuffer(IntPtr handle, ref MyCamera.MV_FRAME_OUT pFrame, int nMsec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_FreeImageBuffer(IntPtr handle, ref MyCamera.MV_FRAME_OUT pFrame);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetOneFrameTimeout(IntPtr handle, IntPtr pData, uint nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, int nMsec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_ClearImageBuffer(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_Display(IntPtr handle, IntPtr hWnd);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_DisplayOneFrame(IntPtr handle, ref MyCamera.MV_DISPLAY_FRAME_INFO pDisplayInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetImageNodeNum(IntPtr handle, uint nNum);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetGrabStrategy(IntPtr handle, MyCamera.MV_GRAB_STRATEGY enGrabStrategy);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetOutputQueueSize(IntPtr handle, uint nOutputQueueSize);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetImageInfo(IntPtr handle, ref MyCamera.MV_IMAGE_BASIC_INFO pstInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetDeviceInfo(IntPtr handle, ref MyCamera.MV_CC_DEVICE_INFO pstDevInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetAllMatchInfo(IntPtr handle, ref MyCamera.MV_ALL_MATCH_INFO pstInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetIntValue(IntPtr handle, string strValue, ref MyCamera.MVCC_INTVALUE pIntValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetIntValueEx(IntPtr handle, string strValue, ref MyCamera.MVCC_INTVALUE_EX pIntValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetIntValue(IntPtr handle, string strValue, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetIntValueEx(IntPtr handle, string strValue, long nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetEnumValue(IntPtr handle, string strValue, ref MyCamera.MVCC_ENUMVALUE pEnumValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetEnumValue(IntPtr handle, string strValue, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetEnumValueByString(IntPtr handle, string strValue, string sValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetFloatValue(IntPtr handle, string strValue, ref MyCamera.MVCC_FLOATVALUE pFloatValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetFloatValue(IntPtr handle, string strValue, float fValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetBoolValue(IntPtr handle, string strValue, ref bool pBoolValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBoolValue(IntPtr handle, string strValue, bool bValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetStringValue(IntPtr handle, string strKey, ref MyCamera.MVCC_STRINGVALUE pStringValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetStringValue(IntPtr handle, string strKey, string sValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetCommandValue(IntPtr handle, string strValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_InvalidateNodes(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetWidth(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetWidth(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetHeight(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetHeight(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetAOIoffsetX(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetAOIoffsetX(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetAOIoffsetY(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetAOIoffsetY(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetAutoExposureTimeLower(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetAutoExposureTimeLower(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetAutoExposureTimeUpper(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetAutoExposureTimeUpper(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetBrightness(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBrightness(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetFrameRate(IntPtr handle, ref MyCamera.MVCC_FLOATVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetFrameRate(IntPtr handle, float fValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetGain(IntPtr handle, ref MyCamera.MVCC_FLOATVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetGain(IntPtr handle, float fValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetExposureTime(IntPtr handle, ref MyCamera.MVCC_FLOATVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetExposureTime(IntPtr handle, float fValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetPixelFormat(IntPtr handle, ref MyCamera.MVCC_ENUMVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetPixelFormat(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetAcquisitionMode(IntPtr handle, ref MyCamera.MVCC_ENUMVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetAcquisitionMode(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetGainMode(IntPtr handle, ref MyCamera.MVCC_ENUMVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetGainMode(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetExposureAutoMode(IntPtr handle, ref MyCamera.MVCC_ENUMVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetExposureAutoMode(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetTriggerMode(IntPtr handle, ref MyCamera.MVCC_ENUMVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetTriggerMode(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetTriggerDelay(IntPtr handle, ref MyCamera.MVCC_FLOATVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetTriggerDelay(IntPtr handle, float fValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetTriggerSource(IntPtr handle, ref MyCamera.MVCC_ENUMVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetTriggerSource(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_TriggerSoftwareExecute(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetGammaSelector(IntPtr handle, ref MyCamera.MVCC_ENUMVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetGammaSelector(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetGamma(IntPtr handle, ref MyCamera.MVCC_FLOATVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetGamma(IntPtr handle, float fValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetSharpness(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetSharpness(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetHue(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetHue(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetSaturation(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetSaturation(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetBalanceWhiteAuto(IntPtr handle, ref MyCamera.MVCC_ENUMVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBalanceWhiteAuto(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetBalanceRatioRed(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBalanceRatioRed(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetBalanceRatioGreen(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBalanceRatioGreen(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetBalanceRatioBlue(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBalanceRatioBlue(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetDeviceUserID(IntPtr handle, ref MyCamera.MVCC_STRINGVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetDeviceUserID(IntPtr handle, string chValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetBurstFrameCount(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBurstFrameCount(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetAcquisitionLineRate(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetAcquisitionLineRate(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetHeartBeatTimeout(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetHeartBeatTimeout(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_LocalUpgrade(IntPtr handle, string pFilePathName);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetUpgradeProcess(IntPtr handle, ref uint pnProcess);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetOptimalPacketSize(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_ReadMemory(IntPtr handle, IntPtr pBuffer, long nAddress, long nLength);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_WriteMemory(IntPtr handle, IntPtr pBuffer, long nAddress, long nLength);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_RegisterExceptionCallBack(IntPtr handle, MyCamera.cbExceptiondelegate cbException, IntPtr pUser);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_RegisterEventCallBack(IntPtr handle, MyCamera.cbEventdelegate cbEvent, IntPtr pUser);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_RegisterAllEventCallBack(IntPtr handle, MyCamera.cbEventdelegateEx cbEvent, IntPtr pUser);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_RegisterEventCallBackEx(IntPtr handle, string pEventName, MyCamera.cbEventdelegateEx cbEvent, IntPtr pUser);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_ForceIpEx(IntPtr handle, uint nIP, uint nSubNetMask, uint nDefaultGateWay);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetIpConfig(IntPtr handle, uint nType);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetNetTransMode(IntPtr handle, uint nType);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetNetTransInfo(IntPtr handle, ref MyCamera.MV_NETTRANS_INFO pstInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetDiscoveryMode(uint nMode);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetGvspTimeout(IntPtr handle, uint nMillisec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetGvspTimeout(IntPtr handle, ref uint pMillisec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetGvcpTimeout(IntPtr handle, uint nMillisec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetGvcpTimeout(IntPtr handle, ref uint pMillisec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetRetryGvcpTimes(IntPtr handle, uint nRetryGvcpTimes);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetRetryGvcpTimes(IntPtr handle, ref uint pRetryGvcpTimes);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetResend(IntPtr handle, uint bEnable, uint nMaxResendPercent, uint nResendTimeout);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetResendMaxRetryTimes(IntPtr handle, uint nRetryTimes);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetResendMaxRetryTimes(IntPtr handle, ref uint pnRetryTimes);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetResendTimeInterval(IntPtr handle, uint nMillisec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetResendTimeInterval(IntPtr handle, ref uint pnMillisec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetGevSCPSPacketSize(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetGevSCPSPacketSize(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetGevSCPD(IntPtr handle, ref MyCamera.MVCC_INTVALUE pstValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetGevSCPD(IntPtr handle, uint nValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetGevSCDA(IntPtr handle, ref uint pnIP);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetGevSCDA(IntPtr handle, uint nIP);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetGevSCSP(IntPtr handle, ref uint pnPort);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetGevSCSP(IntPtr handle, uint nPort);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_SetTransmissionType(IntPtr handle, ref MyCamera.MV_CC_TRANSMISSION_TYPE pstTransmissionType);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_IssueActionCommand(ref MyCamera.MV_ACTION_CMD_INFO pstActionCmdInfo, ref MyCamera.MV_ACTION_CMD_RESULT_LIST pstActionCmdResults);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_GetMulticastStatus(ref MyCamera.MV_CC_DEVICE_INFO pstDevInfo, ref bool pStatus);

        [DllImport("MvCameraControl.dll", EntryPoint = "MV_CAML_SetDeviceBauderate")]
        private static extern int MV_CAML_SetDeviceBaudrate(IntPtr handle, uint nBaudrate);

        [DllImport("MvCameraControl.dll", EntryPoint = "MV_CAML_GetDeviceBauderate")]
        private static extern int MV_CAML_GetDeviceBaudrate(IntPtr handle, ref uint pnCurrentBaudrate);

        [DllImport("MvCameraControl.dll", EntryPoint = "MV_CAML_GetSupportBauderates")]
        private static extern int MV_CAML_GetSupportBaudrates(IntPtr handle, ref uint pnBaudrateAblity);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CAML_SetGenCPTimeOut(IntPtr handle, uint nMillisec);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_USB_SetTransferSize(IntPtr handle, uint nTransferSize);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_USB_GetTransferSize(IntPtr handle, ref uint pTransferSize);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_USB_SetTransferWays(IntPtr handle, uint nTransferWays);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_USB_GetTransferWays(IntPtr handle, ref uint pTransferWays);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_EnumInterfacesByGenTL(ref MyCamera.MV_GENTL_IF_INFO_LIST pstIFInfoList, string sGenTLPath);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_EnumDevicesByGenTL(ref MyCamera.MV_GENTL_IF_INFO stIFInfo, ref MyCamera.MV_GENTL_DEV_INFO_LIST pstDevList);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_CreateHandleByGenTL(ref IntPtr handle, ref MyCamera.MV_GENTL_DEV_INFO stDevInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_XML_GetGenICamXML(IntPtr handle, IntPtr pData, uint nDataSize, ref uint pnDataLen);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_XML_GetNodeAccessMode(IntPtr handle, string pstrName, ref MyCamera.MV_XML_AccessMode pAccessMode);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_XML_GetNodeInterfaceType(IntPtr handle, string pstrName, ref MyCamera.MV_XML_InterfaceType pInterfaceType);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_XML_GetRootNode(IntPtr handle, ref MyCamera.MV_XML_NODE_FEATURE pstNode);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_XML_GetChildren(IntPtr handle, ref MyCamera.MV_XML_NODE_FEATURE pstNode, IntPtr pstNodesList);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_XML_GetChildren(IntPtr handle, ref MyCamera.MV_XML_NODE_FEATURE pstNode, ref MyCamera.MV_XML_NODES_LIST pstNodesList);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_XML_GetNodeFeature(IntPtr handle, ref MyCamera.MV_XML_NODE_FEATURE pstNode, IntPtr pstFeature);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_XML_UpdateNodeFeature(IntPtr handle, MyCamera.MV_XML_InterfaceType enType, IntPtr pstFeature);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_XML_RegisterUpdateCallBack(IntPtr handle, MyCamera.cbXmlUpdatedelegate cbXmlUpdate, IntPtr pUser);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SaveImageEx2(IntPtr handle, ref MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_ConvertPixelType(IntPtr handle, ref MyCamera.MV_PIXEL_CONVERT_PARAM pstCvtParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBayerCvtQuality(IntPtr handle, uint BayerCvtQuality);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBayerGammaValue(IntPtr handle, float fBayerGammaValue);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBayerGammaParam(IntPtr handle, ref MyCamera.MV_CC_GAMMA_PARAM pstGammaParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBayerCCMParam(IntPtr handle, ref MyCamera.MV_CC_CCM_PARAM pstCCMParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBayerCCMParamEx(IntPtr handle, ref MyCamera.MV_CC_CCM_PARAM_EX pstCCMParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SetBayerCLUTParam(IntPtr handle, ref MyCamera.MV_CC_CLUT_PARAM pstCLUTParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_ImageContrast(IntPtr handle, ref MyCamera.MV_CC_CONTRAST_PARAM pstContrastParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_ImageSharpen(IntPtr handle, ref MyCamera.MV_CC_SHARPEN_PARAM pstSharpenParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_ColorCorrect(IntPtr handle, ref MyCamera.MV_CC_COLOR_CORRECT_PARAM pstColorCorrectParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_NoiseEstimate(IntPtr handle, ref MyCamera.MV_CC_NOISE_ESTIMATE_PARAM pstNoiseEstimateParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SpatialDenoise(IntPtr handle, ref MyCamera.MV_CC_SPATIAL_DENOISE_PARAM pstSpatialDenoiseParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_LSCCalib(IntPtr handle, ref MyCamera.MV_CC_LSC_CALIB_PARAM pstLSCCalibParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_LSCCorrect(IntPtr handle, ref MyCamera.MV_CC_LSC_CORRECT_PARAM pstLSCCorrectParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_HB_Decode(IntPtr handle, ref MyCamera.MV_CC_HB_DECODE_PARAM pstDecodeParam);

        [DllImport("MvCameraControl.dll")]
        private static extern IntPtr MV_CC_GetTlProxy(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_FeatureSave(IntPtr handle, string pFileName);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_FeatureLoad(IntPtr handle, string pFileName);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_FileAccessRead(IntPtr handle, ref MyCamera.MV_CC_FILE_ACCESS pstFileAccess);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_FileAccessWrite(IntPtr handle, ref MyCamera.MV_CC_FILE_ACCESS pstFileAccess);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetFileAccessProgress(IntPtr handle, ref MyCamera.MV_CC_FILE_ACCESS_PROGRESS pstFileAccessProgress);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_StartRecord(IntPtr handle, ref MyCamera.MV_CC_RECORD_PARAM pstRecordParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_InputOneFrame(IntPtr handle, ref MyCamera.MV_CC_INPUT_FRAME_INFO pstInputFrameInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_StopRecord(IntPtr handle);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SaveImageToFile(IntPtr handle, ref MyCamera.MV_SAVE_IMG_TO_FILE_PARAM pstSaveFileParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SavePointCloudData(IntPtr handle, ref MyCamera.MV_SAVE_POINT_CLOUD_PARAM pstPointDataParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_RotateImage(IntPtr handle, ref MyCamera.MV_CC_ROTATE_IMAGE_PARAM pstRotateParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_FlipImage(IntPtr handle, ref MyCamera.MV_CC_FLIP_IMAGE_PARAM pstFlipParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetOneFrame(IntPtr handle, IntPtr pData, uint nDataSize, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_GetOneFrameEx(IntPtr handle, IntPtr pData, uint nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_RegisterImageCallBack(IntPtr handle, MyCamera.cbOutputdelegate cbOutput, IntPtr pUser);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_SaveImage(ref MyCamera.MV_SAVE_IMAGE_PARAM stSaveParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_GIGE_ForceIp(IntPtr handle, uint nIP);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_BayerNoiseEstimate(IntPtr handle, ref MyCamera.MV_CC_BAYER_NOISE_ESTIMATE_PARAM pstNoiseEstimateParam);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CC_BayerSpatialDenoise(IntPtr handle, ref MyCamera.MV_CC_BAYER_SPATIAL_DENOISE_PARAM pstSpatialDenoiseParam);
        #endregion

        #region ConstParams
        /// <summary>Unknown Device Type, Reserved</summary>
        public const int MV_UNKNOW_DEVICE = 0;

        /// <summary>GigE Device</summary>
        public const int MV_GIGE_DEVICE = 1;

        /// <summary>1394-a/b Device</summary>
        public const int MV_1394_DEVICE = 2;

        /// <summary>USB3.0 Device</summary>
        public const int MV_USB_DEVICE = 4;

        /// <summary>CameraLink Device</summary>
        public const int MV_CAMERALINK_DEVICE = 8;

        /// <summary>Successed, no error</summary>
        public const int MV_OK = 0;

        /// <summary>Error or invalid handle</summary>
        public const int MV_E_HANDLE = -0x80000000;

        /// <summary>Not supported function</summary>
        public const int MV_E_SUPPORT = -0x7FFFFFFF;

        /// <summary>Buffer overflow</summary>
        public const int MV_E_BUFOVER = -0x7FFFFFFE;

        /// <summary>Function calling order error</summary>
        public const int MV_E_CALLORDER = -0x7FFFFFFD;

        /// <summary>Incorrect parameter</summary>
        public const int MV_E_PARAMETER = -0x7FFFFFFC;

        /// <summary>Applying resource failed</summary>
        public const int MV_E_RESOURCE = -0x7FFFFFFA;

        /// <summary>No data</summary>
        public const int MV_E_NODATA = -0x7FFFFFF9;

        /// <summary>Precondition error, or running environment changed</summary>
        public const int MV_E_PRECONDITION = -0x7FFFFFF8;

        /// <summary>Version mismatches</summary>
        public const int MV_E_VERSION = -0x7FFFFFF7;

        /// <summary>Insufficient memory</summary>
        public const int MV_E_NOENOUGH_BUF = -0x7FFFFFF6;

        /// <summary>Abnormal image, maybe incomplete image because of lost packet</summary>
        public const int MV_E_ABNORMAL_IMAGE = -0x7FFFFFF5;

        /// <summary>Load library failed</summary>
        public const int MV_E_LOAD_LIBRARY = -0x7FFFFFF4;

        /// <summary>No Avaliable Buffer</summary>
        public const int MV_E_NOOUTBUF = -0x7FFFFFF3;

        /// <summary>Encryption error</summary>
        public const int MV_E_ENCRYPT = -0x7FFFFFF2;

        /// <summary>Unknown error</summary>
        public const int MV_E_UNKNOW = -0x7FFFFF01;

        /// <summary>General error</summary>
        public const int MV_E_GC_GENERIC = -0x7FFFFF00;

        /// <summary>Illegal parameters</summary>
        public const int MV_E_GC_ARGUMENT = -0x7FFFFEFF;

        /// <summary>The value is out of range</summary>
        public const int MV_E_GC_RANGE = -0x7FFFFEFE;

        /// <summary>Property</summary>
        public const int MV_E_GC_PROPERTY = -0x7FFFFEFD;

        /// <summary>Running environment error</summary>
        public const int MV_E_GC_RUNTIME = -0x7FFFFEFC;

        /// <summary>Logical error</summary>
        public const int MV_E_GC_LOGICAL = -0x7FFFFEFB;

        /// <summary>Node accessing condition error</summary>
        public const int MV_E_GC_ACCESS = -0x7FFFFEFA;

        /// <summary>Timeout</summary>
        public const int MV_E_GC_TIMEOUT = -0x7FFFFEF9;

        /// <summary>Transformation exception</summary>
        public const int MV_E_GC_DYNAMICCAST = -0x7FFFFEF8;

        /// <summary>GenICam unknown error</summary>
        public const int MV_E_GC_UNKNOW = -0x7FFFFE01;

        /// <summary>The command is not supported by device</summary>
        public const int MV_E_NOT_IMPLEMENTED = -0x7FFFFE00;

        /// <summary>The target address being accessed does not exist</summary>
        public const int MV_E_INVALID_ADDRESS = -0x7FFFFDFF;

        /// <summary>The target address is not writable</summary>
        public const int MV_E_WRITE_PROTECT = -0x7FFFFDFE;

        /// <summary>No permission</summary>
        public const int MV_E_ACCESS_DENIED = -0x7FFFFDFD;

        /// <summary>Device is busy, or network disconnected</summary>
        public const int MV_E_BUSY = -0x7FFFFDFC;

        /// <summary>Network data packet error</summary>
        public const int MV_E_PACKET = -0x7FFFFDFB;

        /// <summary>Network error</summary>
        public const int MV_E_NETER = -0x7FFFFDFA;

        /// <summary>Device IP conflict</summary>
        public const int MV_E_IP_CONFLICT = -0x7FFFFDDF;

        /// <summary>Reading USB error</summary>
        public const int MV_E_USB_READ = -0x7FFFFD00;

        /// <summary>Writing USB error</summary>
        public const int MV_E_USB_WRITE = -0x7FFFFCFF;

        /// <summary>Device exception</summary>
        public const int MV_E_USB_DEVICE = -0x7FFFFCFE;

        /// <summary>GenICam error</summary>
        public const int MV_E_USB_GENICAM = -0x7FFFFCFD;

        /// <summary>Insufficient bandwidth, this error code is newly added</summary>
        public const int MV_E_USB_BANDWIDTH = -0x7FFFFCFC;

        /// <summary>Driver mismatch or unmounted drive</summary>
        public const int MV_E_USB_DRIVER = -0x7FFFFCFB;

        /// <summary>USB unknown error</summary>
        public const int MV_E_USB_UNKNOW = -0x7FFFFC01;

        /// <summary>Firmware mismatches</summary>
        public const int MV_E_UPG_FILE_MISMATCH = -0x7FFFFC00;

        /// <summary>Firmware language mismatches</summary>
        public const int MV_E_UPG_LANGUSGE_MISMATCH = -0x7FFFFBFF;

        /// <summary>Upgrading conflicted (repeated upgrading requests during device upgrade)</summary>
        public const int MV_E_UPG_CONFLICT = -0x7FFFFBFE;

        /// <summary>Camera internal error during upgrade</summary>
        public const int MV_E_UPG_INNER_ERR = -0x7FFFFBFD;

        /// <summary>Unknown error during upgrade</summary>
        public const int MV_E_UPG_UNKNOW = -0x7FFFFB01;

        /// <summary>处理正确</summary>
        public const int MV_ALG_OK = 0;

        /// <summary>不确定类型错误</summary>
        public const int MV_ALG_ERR = 0x10000000;

        /// <summary>能力集中存在无效参数</summary>
        public const int MV_ALG_E_ABILITY_ARG = 0x10000001;

        /// <summary>内存地址为空</summary>
        public const int MV_ALG_E_MEM_NULL = 0x10000002;

        /// <summary>内存对齐不满足要求</summary>
        public const int MV_ALG_E_MEM_ALIGN = 0x10000003;

        /// <summary>内存空间大小不够</summary>
        public const int MV_ALG_E_MEM_LACK = 0x10000004;

        /// <summary>内存空间大小不满足对齐要求</summary>
        public const int MV_ALG_E_MEM_SIZE_ALIGN = 0x10000005;

        /// <summary>内存地址不满足对齐要求</summary>
        public const int MV_ALG_E_MEM_ADDR_ALIGN = 0x10000006;

        /// <summary>图像格式不正确或者不支持</summary>
        public const int MV_ALG_E_IMG_FORMAT = 0x10000007;

        /// <summary>图像宽高不正确或者超出范围</summary>
        public const int MV_ALG_E_IMG_SIZE = 0x10000008;

        /// <summary>图像宽高与step参数不匹配</summary>
        public const int MV_ALG_E_IMG_STEP = 0x10000009;

        /// <summary>图像数据存储地址为空</summary>
        public const int MV_ALG_E_IMG_DATA_NULL = 0x1000000A;

        /// <summary>设置或者获取参数类型不正确</summary>
        public const int MV_ALG_E_CFG_TYPE = 0x1000000B;

        /// <summary>设置或者获取参数的输入、输出结构体大小不正确</summary>
        public const int MV_ALG_E_CFG_SIZE = 0x1000000C;

        /// <summary>处理类型不正确</summary>
        public const int MV_ALG_E_PRC_TYPE = 0x1000000D;

        /// <summary>处理时输入、输出参数大小不正确</summary>
        public const int MV_ALG_E_PRC_SIZE = 0x1000000E;

        /// <summary>子处理类型不正确</summary>
        public const int MV_ALG_E_FUNC_TYPE = 0x1000000F;

        /// <summary>子处理时输入、输出参数大小不正确</summary>
        public const int MV_ALG_E_FUNC_SIZE = 0x10000010;

        /// <summary>index参数不正确</summary>
        public const int MV_ALG_E_PARAM_INDEX = 0x10000011;

        /// <summary>value参数不正确或者超出范围</summary>
        public const int MV_ALG_E_PARAM_VALUE = 0x10000012;

        /// <summary>param_num参数不正确</summary>
        public const int MV_ALG_E_PARAM_NUM = 0x10000013;

        /// <summary>函数参数指针为空</summary>
        public const int MV_ALG_E_NULL_PTR = 0x10000014;

        /// <summary>超过限定的最大内存</summary>
        public const int MV_ALG_E_OVER_MAX_MEM = 0x10000015;

        /// <summary>回调函数出错</summary>
        public const int MV_ALG_E_CALL_BACK = 0x10000016;

        /// <summary>加密错误</summary>
        public const int MV_ALG_E_ENCRYPT = 0x10000017;

        /// <summary>算法库使用期限错误</summary>
        public const int MV_ALG_E_EXPIRE = 0x10000018;

        /// <summary>参数范围不正确</summary>
        public const int MV_ALG_E_BAD_ARG = 0x10000019;

        /// <summary>数据大小不正确</summary>
        public const int MV_ALG_E_DATA_SIZE = 0x1000001A;

        /// <summary>数据step不正确</summary>
        public const int MV_ALG_E_STEP = 0x1000001B;

        /// <summary>cpu不支持优化代码中的指令集</summary>
        public const int MV_ALG_E_CPUID = 0x1000001C;

        /// <summary>警告</summary>
        public const int MV_ALG_WARNING = 0x1000001D;

        /// <summary>算法库超时</summary>
        public const int MV_ALG_E_TIME_OUT = 0x1000001E;

        /// <summary>算法版本号出错</summary>
        public const int MV_ALG_E_LIB_VERSION = 0x1000001F;

        /// <summary>模型版本号出错</summary>
        public const int MV_ALG_E_MODEL_VERSION = 0x10000020;

        /// <summary>GPU内存分配错误</summary>
        public const int MV_ALG_E_GPU_MEM_ALLOC = 0x10000021;

        /// <summary>文件不存在</summary>
        public const int MV_ALG_E_FILE_NON_EXIST = 0x10000022;

        /// <summary>字符串为空</summary>
        public const int MV_ALG_E_NONE_STRING = 0x10000023;

        /// <summary>图像解码器错误</summary>
        public const int MV_ALG_E_IMAGE_CODEC = 0x10000024;

        /// <summary>打开文件错误</summary>
        public const int MV_ALG_E_FILE_OPEN = 0x10000025;

        /// <summary>文件读取错误</summary>
        public const int MV_ALG_E_FILE_READ = 0x10000026;

        /// <summary>文件写错误</summary>
        public const int MV_ALG_E_FILE_WRITE = 0x10000027;

        /// <summary>文件读取大小错误</summary>
        public const int MV_ALG_E_FILE_READ_SIZE = 0x10000028;

        /// <summary>文件类型错误</summary>
        public const int MV_ALG_E_FILE_TYPE = 0x10000029;

        /// <summary>模型类型错误</summary>
        public const int MV_ALG_E_MODEL_TYPE = 0x1000002A;

        /// <summary>分配内存错误</summary>
        public const int MV_ALG_E_MALLOC_MEM = 0x1000002B;

        /// <summary>线程绑核失败</summary>
        public const int MV_ALG_E_BIND_CORE_FAILED = 0x1000002C;

        /// <summary>噪声特性图像格式错误</summary>
        public const int MV_ALG_E_DENOISE_NE_IMG_FORMAT = 0x10402001;

        /// <summary>噪声特性类型错误</summary>
        public const int MV_ALG_E_DENOISE_NE_FEATURE_TYPE = 0x10402002;

        /// <summary>噪声特性个数错误</summary>
        public const int MV_ALG_E_DENOISE_NE_PROFILE_NUM = 0x10402003;

        /// <summary>噪声特性增益个数错误</summary>
        public const int MV_ALG_E_DENOISE_NE_GAIN_NUM = 0x10402004;

        /// <summary>噪声曲线增益值输入错误</summary>
        public const int MV_ALG_E_DENOISE_NE_GAIN_VAL = 0x10402005;

        /// <summary>噪声曲线柱数错误</summary>
        public const int MV_ALG_E_DENOISE_NE_BIN_NUM = 0x10402006;

        /// <summary>噪声估计初始化增益设置错误</summary>
        public const int MV_ALG_E_DENOISE_NE_INIT_GAIN = 0x10402007;

        /// <summary>噪声估计未初始化</summary>
        public const int MV_ALG_E_DENOISE_NE_NOT_INIT = 0x10402008;

        /// <summary>颜色空间模式错误</summary>
        public const int MV_ALG_E_DENOISE_COLOR_MODE = 0x10402009;

        /// <summary>图像ROI个数错误</summary>
        public const int MV_ALG_E_DENOISE_ROI_NUM = 0x1040200A;

        /// <summary>图像ROI原点错误</summary>
        public const int MV_ALG_E_DENOISE_ROI_ORI_PT = 0x1040200B;

        /// <summary>图像ROI大小错误</summary>
        public const int MV_ALG_E_DENOISE_ROI_SIZE = 0x1040200C;

        /// <summary>输入的相机增益不存在(增益个数已达上限)</summary>
        public const int MV_ALG_E_DENOISE_GAIN_NOT_EXIST = 0x1040200D;

        /// <summary>输入的相机增益不在范围内</summary>
        public const int MV_ALG_E_DENOISE_GAIN_BEYOND_RANGE = 0x1040200E;

        /// <summary>输入的噪声特性内存大小错误</summary>
        public const int MV_ALG_E_DENOISE_NP_BUF_SIZE = 0x1040200F;

        /// <summary>
        /// ch:信息结构体的最大缓存 | en: Max buffer size of information structs
        /// </summary>
        public const int INFO_MAX_BUFFER_SIZE = 0x40;

        public const int MV_MAX_DEVICE_NUM = 0x100;

        /// <summary>
        /// ch:最大Interface数量 | en:Max num of interfaces
        /// </summary>
        public const int MV_MAX_GENTL_IF_NUM = 0x100;

        /// <summary>
        /// ch:最大GenTL设备数量 | en:Max num of GenTL devices
        /// </summary>
        public const int MV_MAX_GENTL_DEV_NUM = 0x100;

        public const int MV_IP_CFG_STATIC = 0x5000000;

        public const int MV_IP_CFG_DHCP = 0x6000000;

        public const int MV_IP_CFG_LLA = 0x4000000;

        public const int MV_NET_TRANS_DRIVER = 1;

        public const int MV_NET_TRANS_SOCKET = 2;

        public const int MV_CAML_BAUDRATE_9600 = 1;

        public const int MV_CAML_BAUDRATE_19200 = 2;

        public const int MV_CAML_BAUDRATE_38400 = 4;

        public const int MV_CAML_BAUDRATE_57600 = 8;

        public const int MV_CAML_BAUDRATE_115200 = 0x10;

        public const int MV_CAML_BAUDRATE_230400 = 0x20;

        public const int MV_CAML_BAUDRATE_460800 = 0x40;

        public const int MV_CAML_BAUDRATE_921600 = 0x80;

        public const int MV_CAML_BAUDRATE_AUTOMAX = 0x40000000;

        public const int MV_MATCH_TYPE_NET_DETECT = 1;

        public const int MV_MATCH_TYPE_USB_DETECT = 2;

        public const int MV_MAX_XML_DISC_STRLEN_C = 0x200;

        public const int MV_MAX_XML_NODE_STRLEN_C = 0x40;

        public const int MV_MAX_XML_NODE_NUM_C = 0x80;

        public const int MV_MAX_XML_SYMBOLIC_NUM = 0x40;

        public const int MV_MAX_XML_STRVALUE_STRLEN_C = 0x40;

        public const int MV_MAX_XML_PARENTS_NUM = 8;

        public const int MV_MAX_XML_SYMBOLIC_STRLEN_C = 0x40;

        public const int MV_EXCEPTION_DEV_DISCONNECT = 0x8001;

        public const int MV_EXCEPTION_VERSION_CHECK = 0x8002;

        public const int MV_ACCESS_Exclusive = 1;

        public const int MV_ACCESS_ExclusiveWithSwitch = 2;

        public const int MV_ACCESS_Control = 3;

        public const int MV_ACCESS_ControlWithSwitch = 4;

        public const int MV_ACCESS_ControlSwitchEnable = 5;

        public const int MV_ACCESS_ControlSwitchEnableWithKey = 6;

        public const int MV_ACCESS_Monitor = 7;

        public const int MAX_EVENT_NAME_SIZE = 0x80;

        private IntPtr handle;
        #endregion

        #region delegate
        /// <summary>
        /// Grab callback
        /// </summary>
        /// <param name="pData">Image data</param>
        /// <param name="pFrameInfo">Frame info</param>
        /// <param name="pUser">User defined variable</param>
        public delegate void cbOutputdelegate(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser);

        /// <summary>
        /// Grab callback
        /// </summary>
        /// <param name="pData">Image data</param>
        /// <param name="pFrameInfo">Frame info</param>
        /// <param name="pUser">User defined variable</param>
        public delegate void cbOutputExdelegate(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser);

        /// <summary>
        /// Xml Update callback(Interfaces not recommended)
        /// </summary>
        /// <param name="enType">Node type</param>
        /// <param name="pstFeature">Current node feature structure</param>
        /// <param name="pstNodesList">Nodes list</param>
        /// <param name="pUser">User defined variable</param>
        public delegate void cbXmlUpdatedelegate(MyCamera.MV_XML_InterfaceType enType, IntPtr pstFeature, ref MyCamera.MV_XML_NODES_LIST pstNodesList, IntPtr pUser);

        /// <summary>
        /// Exception callback
        /// </summary>
        /// <param name="nMsgType">Msg type</param>
        /// <param name="pUser">User defined variable</param>
        public delegate void cbExceptiondelegate(uint nMsgType, IntPtr pUser);

        /// <summary>
        /// Event callback (Interfaces not recommended)
        /// </summary>
        /// <param name="nUserDefinedId">User defined ID</param>
        /// <param name="pUser">User defined variable</param>
        public delegate void cbEventdelegate(uint nUserDefinedId, IntPtr pUser);

        /// <summary>
        /// Event callback
        /// </summary>
        /// <param name="pEventInfo">Event Info</param>
        /// <param name="pUser">User defined variable</param>
        public delegate void cbEventdelegateEx(ref MyCamera.MV_EVENT_OUT_INFO pEventInfo, IntPtr pUser);
        #endregion

        public enum MvGvspPixelType
        {
            PixelType_Gvsp_Undefined = -1,
            PixelType_Gvsp_Mono1p = 0x1010037,
            PixelType_Gvsp_Mono2p = 0x1020038,
            PixelType_Gvsp_Mono4p = 0x1040039,
            PixelType_Gvsp_Mono8 = 0x1080001,
            PixelType_Gvsp_Mono8_Signed,
            PixelType_Gvsp_Mono10 = 0x1100003,
            PixelType_Gvsp_Mono10_Packed = 0x10C0004,
            PixelType_Gvsp_Mono12 = 0x1100005,
            PixelType_Gvsp_Mono12_Packed = 0x10C0006,
            PixelType_Gvsp_Mono14 = 0x1100025,
            PixelType_Gvsp_Mono16 = 0x1100007,
            PixelType_Gvsp_BayerGR8 = 0x1080008,
            PixelType_Gvsp_BayerRG8,
            PixelType_Gvsp_BayerGB8,
            PixelType_Gvsp_BayerBG8,
            PixelType_Gvsp_BayerGR10 = 0x110000C,
            PixelType_Gvsp_BayerRG10,
            PixelType_Gvsp_BayerGB10,
            PixelType_Gvsp_BayerBG10,
            PixelType_Gvsp_BayerGR12,
            PixelType_Gvsp_BayerRG12,
            PixelType_Gvsp_BayerGB12,
            PixelType_Gvsp_BayerBG12,
            PixelType_Gvsp_BayerGR10_Packed = 0x10C0026,
            PixelType_Gvsp_BayerRG10_Packed,
            PixelType_Gvsp_BayerGB10_Packed,
            PixelType_Gvsp_BayerBG10_Packed,
            PixelType_Gvsp_BayerGR12_Packed,
            PixelType_Gvsp_BayerRG12_Packed,
            PixelType_Gvsp_BayerGB12_Packed,
            PixelType_Gvsp_BayerBG12_Packed,
            PixelType_Gvsp_BayerGR16 = 0x110002E,
            PixelType_Gvsp_BayerRG16,
            PixelType_Gvsp_BayerGB16,
            PixelType_Gvsp_BayerBG16,
            PixelType_Gvsp_RGB8_Packed = 0x2180014,
            PixelType_Gvsp_BGR8_Packed,
            PixelType_Gvsp_RGBA8_Packed = 0x2200016,
            PixelType_Gvsp_BGRA8_Packed,
            PixelType_Gvsp_RGB10_Packed = 0x2300018,
            PixelType_Gvsp_BGR10_Packed,
            PixelType_Gvsp_RGB12_Packed,
            PixelType_Gvsp_BGR12_Packed,
            PixelType_Gvsp_RGB16_Packed = 0x2300033,
            PixelType_Gvsp_RGB10V1_Packed = 0x220001C,
            PixelType_Gvsp_RGB10V2_Packed,
            PixelType_Gvsp_RGB12V1_Packed = 0x2240034,
            PixelType_Gvsp_RGB565_Packed = 0x2100035,
            PixelType_Gvsp_BGR565_Packed,
            PixelType_Gvsp_YUV411_Packed = 0x20C001E,
            PixelType_Gvsp_YUV422_Packed = 0x210001F,
            PixelType_Gvsp_YUV422_YUYV_Packed = 0x2100032,
            PixelType_Gvsp_YUV444_Packed = 0x2180020,
            PixelType_Gvsp_YCBCR8_CBYCR = 0x218003A,
            PixelType_Gvsp_YCBCR422_8 = 0x210003B,
            PixelType_Gvsp_YCBCR422_8_CBYCRY = 0x2100043,
            PixelType_Gvsp_YCBCR411_8_CBYYCRYY = 0x20C003C,
            PixelType_Gvsp_YCBCR601_8_CBYCR = 0x218003D,
            PixelType_Gvsp_YCBCR601_422_8 = 0x210003E,
            PixelType_Gvsp_YCBCR601_422_8_CBYCRY = 0x2100044,
            PixelType_Gvsp_YCBCR601_411_8_CBYYCRYY = 0x20C003F,
            PixelType_Gvsp_YCBCR709_8_CBYCR = 0x2180040,
            PixelType_Gvsp_YCBCR709_422_8 = 0x2100041,
            PixelType_Gvsp_YCBCR709_422_8_CBYCRY = 0x2100045,
            PixelType_Gvsp_YCBCR709_411_8_CBYYCRYY = 0x20C0042,
            PixelType_Gvsp_RGB8_Planar = 0x2180021,
            PixelType_Gvsp_RGB10_Planar = 0x2300022,
            PixelType_Gvsp_RGB12_Planar,
            PixelType_Gvsp_RGB16_Planar,
            PixelType_Gvsp_Jpeg = -0x7FE7FFFF,
            PixelType_Gvsp_Coord3D_ABC32f = 0x26000C0,
            PixelType_Gvsp_Coord3D_ABC32f_Planar,
            PixelType_Gvsp_Coord3D_AC32f = 0x24000C2,
            PixelType_Gvsp_COORD3D_DEPTH_PLUS_MASK = -0x7DE3FFFF,
            PixelType_Gvsp_Coord3D_ABC32 = -0x7D9FCFFF,
            PixelType_Gvsp_Coord3D_AB32f = -0x7DBFCFFE,
            PixelType_Gvsp_Coord3D_AB32,
            PixelType_Gvsp_Coord3D_AC32f_Planar = 0x24000C3,
            PixelType_Gvsp_Coord3D_AC32 = -0x7DBFCFFC,
            PixelType_Gvsp_Coord3D_A32f = 0x12000BD,
            PixelType_Gvsp_Coord3D_A32 = -0x7EDFCFFB,
            PixelType_Gvsp_Coord3D_C32f = 0x12000BF,
            PixelType_Gvsp_Coord3D_C32 = -0x7EDFCFFA,
            PixelType_Gvsp_Coord3D_ABC16 = 0x23000B9,
            PixelType_Gvsp_Coord3D_C16 = 0x11000B8,
            PixelType_Gvsp_HB_Mono8 = -0x7EF7FFFF,
            PixelType_Gvsp_HB_Mono10 = -0x7EEFFFFD,
            PixelType_Gvsp_HB_Mono10_Packed = -0x7EF3FFFC,
            PixelType_Gvsp_HB_Mono12 = -0x7EEFFFFB,
            PixelType_Gvsp_HB_Mono12_Packed = -0x7EF3FFFA,
            PixelType_Gvsp_HB_Mono16 = -0x7EEFFFF9,
            PixelType_Gvsp_HB_BayerGR8 = -0x7EF7FFF8,
            PixelType_Gvsp_HB_BayerRG8,
            PixelType_Gvsp_HB_BayerGB8,
            PixelType_Gvsp_HB_BayerBG8,
            PixelType_Gvsp_HB_BayerGR10 = -0x7EEFFFF4,
            PixelType_Gvsp_HB_BayerRG10,
            PixelType_Gvsp_HB_BayerGB10,
            PixelType_Gvsp_HB_BayerBG10,
            PixelType_Gvsp_HB_BayerGR12,
            PixelType_Gvsp_HB_BayerRG12,
            PixelType_Gvsp_HB_BayerGB12,
            PixelType_Gvsp_HB_BayerBG12,
            PixelType_Gvsp_HB_BayerGR10_Packed = -0x7EF3FFDA,
            PixelType_Gvsp_HB_BayerRG10_Packed,
            PixelType_Gvsp_HB_BayerGB10_Packed,
            PixelType_Gvsp_HB_BayerBG10_Packed,
            PixelType_Gvsp_HB_BayerGR12_Packed,
            PixelType_Gvsp_HB_BayerRG12_Packed,
            PixelType_Gvsp_HB_BayerGB12_Packed,
            PixelType_Gvsp_HB_BayerBG12_Packed,
            PixelType_Gvsp_HB_YUV422_Packed = -0x7DEFFFE1,
            PixelType_Gvsp_HB_YUV422_YUYV_Packed = -0x7DEFFFCE,
            PixelType_Gvsp_HB_RGB8_Packed = -0x7DE7FFEC,
            PixelType_Gvsp_HB_BGR8_Packed,
            PixelType_Gvsp_HB_RGBA8_Packed = -0x7DDFFFEA,
            PixelType_Gvsp_HB_BGRA8_Packed
        }
        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CAML_SetDeviceBauderate(IntPtr handle, uint nBaudrate);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CAML_GetDeviceBauderate(IntPtr handle, ref uint pnCurrentBaudrate);

        [DllImport("MvCameraControl.dll")]
        private static extern int MV_CAML_GetSupportBauderates(IntPtr handle, ref uint pnBaudrateAblity);

        #region Struct

        //未使用
        /// <summary>
        /// ch: GigE设备信息 | en: GigE device information
        /// </summary>
        internal struct MV_GIGE_DEVICE_INFO_EX
        {
            public uint nIpCfgOption;

            public uint nIpCfgCurrent;

            public uint nCurrentIp;

            public uint nCurrentSubNetMask;

            public uint nDefultGateWay;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string chManufacturerName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string chModelName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string chDeviceVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x30)]
            public string chManufacturerSpecificInfo;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
            public string chSerialNumber;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public byte[] chUserDefinedName;

            public uint nNetExport;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_GIGE_DEVICE_INFO_EX(uint nIpCfgOption, uint nIpCfgCurrent, uint nCurrentIp, uint nCurrentSubNetMask, uint nDefultGateWay, string chManufacturerName, string chModelName, string chDeviceVersion, string chManufacturerSpecificInfo, string chSerialNumber, byte[] chUserDefinedName, uint nNetExport, uint[] nReserved)
            {
                this.nIpCfgOption = nIpCfgOption;
                this.nIpCfgCurrent = nIpCfgCurrent;
                this.nCurrentIp = nCurrentIp;
                this.nCurrentSubNetMask = nCurrentSubNetMask;
                this.nDefultGateWay = nDefultGateWay;
                this.chManufacturerName = chManufacturerName;
                this.chModelName = chModelName;
                this.chDeviceVersion = chDeviceVersion;
                this.chManufacturerSpecificInfo = chManufacturerSpecificInfo;
                this.chSerialNumber = chSerialNumber;
                this.chUserDefinedName = chUserDefinedName;
                this.nNetExport = nNetExport;
                this.nReserved = nReserved;
            }
        }

        /// <summary>
        /// Gige设备信息
        /// </summary>
        public struct MV_GIGE_DEVICE_INFO
        {
            public uint nIpCfgOption;

            public uint nIpCfgCurrent;

            public uint nCurrentIp;

            public uint nCurrentSubNetMask;

            public uint nDefultGateWay;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string chManufacturerName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string chModelName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string chDeviceVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x30)]
            public string chManufacturerSpecificInfo;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
            public string chSerialNumber;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
            public string chUserDefinedName;

            public uint nNetExport;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_GIGE_DEVICE_INFO(uint nIpCfgOption, uint nIpCfgCurrent, uint nCurrentIp, uint nCurrentSubNetMask, uint nDefultGateWay, string chManufacturerName, string chModelName, string chDeviceVersion, string chManufacturerSpecificInfo, string chSerialNumber, string chUserDefinedName, uint nNetExport, uint[] nReserved)
            {
                this.nIpCfgOption = nIpCfgOption;
                this.nIpCfgCurrent = nIpCfgCurrent;
                this.nCurrentIp = nCurrentIp;
                this.nCurrentSubNetMask = nCurrentSubNetMask;
                this.nDefultGateWay = nDefultGateWay;
                this.chManufacturerName = chManufacturerName;
                this.chModelName = chModelName;
                this.chDeviceVersion = chDeviceVersion;
                this.chManufacturerSpecificInfo = chManufacturerSpecificInfo;
                this.chSerialNumber = chSerialNumber;
                this.chUserDefinedName = chUserDefinedName;
                this.nNetExport = nNetExport;
                this.nReserved = nReserved;
            }
        }

        /// <summary>
        /// ch:USB3 设备信息 | en:USB3 device information
        /// </summary>
        public struct MV_USB3_DEVICE_INFO_EX
        {
            public byte CrtlInEndPoint;

            public byte CrtlOutEndPoint;

            public byte StreamEndPoint;

            public byte EventEndPoint;

            public ushort idVendor;

            public ushort idProduct;

            public uint nDeviceNumber;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chDeviceGUID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chVendorName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chModelName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chFamilyName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chDeviceVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chManufacturerName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chSerialNumber;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
            public byte[] chUserDefinedName;

            public uint nbcdUSB;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public uint[] nReserved;

            public MV_USB3_DEVICE_INFO_EX(byte crtlInEndPoint, byte crtlOutEndPoint, byte streamEndPoint, byte eventEndPoint, ushort idVendor, ushort idProduct, uint nDeviceNumber, string chDeviceGUID, string chVendorName, string chModelName, string chFamilyName, string chDeviceVersion, string chManufacturerName, string chSerialNumber, byte[] chUserDefinedName, uint nbcdUSB, uint[] nReserved)
            {
                CrtlInEndPoint = crtlInEndPoint;
                CrtlOutEndPoint = crtlOutEndPoint;
                StreamEndPoint = streamEndPoint;
                EventEndPoint = eventEndPoint;
                this.idVendor = idVendor;
                this.idProduct = idProduct;
                this.nDeviceNumber = nDeviceNumber;
                this.chDeviceGUID = chDeviceGUID;
                this.chVendorName = chVendorName;
                this.chModelName = chModelName;
                this.chFamilyName = chFamilyName;
                this.chDeviceVersion = chDeviceVersion;
                this.chManufacturerName = chManufacturerName;
                this.chSerialNumber = chSerialNumber;
                this.chUserDefinedName = chUserDefinedName;
                this.nbcdUSB = nbcdUSB;
                this.nReserved = nReserved;
            }
        }

        public struct MV_USB3_DEVICE_INFO
        {
            public byte CrtlInEndPoint;

            public byte CrtlOutEndPoint;

            public byte StreamEndPoint;

            public byte EventEndPoint;

            public ushort idVendor;

            public ushort idProduct;

            public uint nDeviceNumber;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chDeviceGUID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chVendorName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chModelName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chFamilyName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chDeviceVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chManufacturerName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chSerialNumber;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chUserDefinedName;

            public uint nbcdUSB;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public uint[] nReserved;

            public MV_USB3_DEVICE_INFO(byte crtlInEndPoint, byte crtlOutEndPoint, byte streamEndPoint, byte eventEndPoint, ushort idVendor, ushort idProduct, uint nDeviceNumber, string chDeviceGUID, string chVendorName, string chModelName, string chFamilyName, string chDeviceVersion, string chManufacturerName, string chSerialNumber, string chUserDefinedName, uint nbcdUSB, uint[] nReserved)
            {
                CrtlInEndPoint = crtlInEndPoint;
                CrtlOutEndPoint = crtlOutEndPoint;
                StreamEndPoint = streamEndPoint;
                EventEndPoint = eventEndPoint;
                this.idVendor = idVendor;
                this.idProduct = idProduct;
                this.nDeviceNumber = nDeviceNumber;
                this.chDeviceGUID = chDeviceGUID;
                this.chVendorName = chVendorName;
                this.chModelName = chModelName;
                this.chFamilyName = chFamilyName;
                this.chDeviceVersion = chDeviceVersion;
                this.chManufacturerName = chManufacturerName;
                this.chSerialNumber = chSerialNumber;
                this.chUserDefinedName = chUserDefinedName;
                this.nbcdUSB = nbcdUSB;
                this.nReserved = nReserved;
            }
        }

        /// <summary>
        /// ch:CamLink设备信息 | en:CamLink device information
        /// </summary>
        public struct MV_CamL_DEV_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chPortID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chModelName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chFamilyName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chDeviceVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chManufacturerName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chSerialNumber;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x26)]
            public uint[] nReserved;

            public MV_CamL_DEV_INFO(string chPortID, string chModelName, string chFamilyName, string chDeviceVersion, string chManufacturerName, string chSerialNumber, uint[] nReserved)
            {
                this.chPortID = chPortID;
                this.chModelName = chModelName;
                this.chFamilyName = chFamilyName;
                this.chDeviceVersion = chDeviceVersion;
                this.chManufacturerName = chManufacturerName;
                this.chSerialNumber = chSerialNumber;
                this.nReserved = nReserved;
            }
        }

        /// <summary>
        /// ch:设备信息 | en:Device information
        /// </summary>
        public struct MV_CC_DEVICE_INFO
        {
            public ushort nMajorVer;

            public ushort nMinorVer;

            public uint nMacAddrHigh;

            /// MAC 地址
            public uint nMacAddrLow;

            public uint nTLayerType;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MyCamera.MV_CC_DEVICE_INFO.SPECIAL_INFO SpecialInfo;

            public MV_CC_DEVICE_INFO(ushort nMajorVer, ushort nMinorVer, uint nMacAddrHigh, uint nMacAddrLow, uint nTLayerType, uint[] nReserved, SPECIAL_INFO specialInfo)
            {
                this.nMajorVer = nMajorVer;
                this.nMinorVer = nMinorVer;
                this.nMacAddrHigh = nMacAddrHigh;
                this.nMacAddrLow = nMacAddrLow;
                this.nTLayerType = nTLayerType;
                this.nReserved = nReserved;
                SpecialInfo = specialInfo;
            }

            /// <summary>
            /// ch:特定类型的设备信息 | en:Special devcie information
            /// </summary>
            [StructLayout(LayoutKind.Explicit, Size = 0x21C)]
            public struct SPECIAL_INFO
            {
                [FieldOffset(0)]
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xD8)]
                public byte[] stGigEInfo;

                [FieldOffset(0)]
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x218)]
                public byte[] stCamLInfo;

                [FieldOffset(0)]
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x21C)]
                public byte[] stUsb3VInfo;
            }
        }

        public struct MV_CC_DEVICE_INFO_LIST
        {
            public uint nDeviceNum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
            public IntPtr[] pDeviceInfo;

            public MV_CC_DEVICE_INFO_LIST(uint nDeviceNum, IntPtr[] pDeviceInfo)
            {
                this.nDeviceNum = nDeviceNum;
                this.pDeviceInfo = pDeviceInfo;
            }
        }

        /// <summary>
        /// ch:通过GenTL枚举到的Interface信息 | en:Interface Information with GenTL
        /// </summary>
        public struct MV_GENTL_IF_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chInterfaceID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chTLType;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chDisplayName;

            public uint nCtiIndex;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nReserved;

            public MV_GENTL_IF_INFO(string chInterfaceID, string chTLType, string chDisplayName, uint nCtiIndex, uint[] nReserved)
            {
                this.chInterfaceID = chInterfaceID;
                this.chTLType = chTLType;
                this.chDisplayName = chDisplayName;
                this.nCtiIndex = nCtiIndex;
                this.nReserved = nReserved;
            }
        }

        /// <summary>
        /// ch:通过GenTL枚举到的设备信息列表 | en:Interface Information List with GenTL
        /// </summary>
        public struct MV_GENTL_IF_INFO_LIST
        {
            public uint nInterfaceNum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
            public IntPtr[] pIFInfo;

            public MV_GENTL_IF_INFO_LIST(uint nInterfaceNum, IntPtr[] pIFInfo)
            {
                this.nInterfaceNum = nInterfaceNum;
                this.pIFInfo = pIFInfo;
            }
        }

        /// <summary>
        /// ch:通过GenTL枚举到的设备信息 | en:Device Information discovered by with GenTL
        /// </summary>
        public struct MV_GENTL_DEV_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chInterfaceID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chDeviceID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chVendorName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chModelName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chTLType;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chUserDefinedName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chSerialNumber;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string chDeviceVersion;

            public uint nCtiIndex;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nReserved;

            public MV_GENTL_DEV_INFO(string chInterfaceID, string chDeviceID, string chVendorName, string chModelName, string chTLType, string chDisplayName, string chUserDefinedName, string chSerialNumber, string chDeviceVersion, uint nCtiIndex, uint[] nReserved)
            {
                this.chInterfaceID = chInterfaceID;
                this.chDeviceID = chDeviceID;
                this.chVendorName = chVendorName;
                this.chModelName = chModelName;
                this.chTLType = chTLType;
                this.chDisplayName = chDisplayName;
                this.chUserDefinedName = chUserDefinedName;
                this.chSerialNumber = chSerialNumber;
                this.chDeviceVersion = chDeviceVersion;
                this.nCtiIndex = nCtiIndex;
                this.nReserved = nReserved;
            }
        }

        /// <summary>
        /// ch:GenTL设备列表 | en:GenTL devices list
        /// </summary>
        public struct MV_GENTL_DEV_INFO_LIST
        {
            public uint nDeviceNum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
            public IntPtr[] pDeviceInfo;

            public MV_GENTL_DEV_INFO_LIST(uint nDeviceNum, IntPtr[] pDeviceInfo)
            {
                this.nDeviceNum = nDeviceNum;
                this.pDeviceInfo = pDeviceInfo;
            }
        }

        public struct MV_NETTRANS_INFO
        {
            public long nReviceDataSize;

            public int nThrowFrameCount;

            public uint nNetRecvFrameCount;

            public long nRequestResendPacketCount;

            public long nResendPacketCount;

            public MV_NETTRANS_INFO(long nReviceDataSize, int nThrowFrameCount, uint nNetRecvFrameCount, long nRequestResendPacketCount, long nResendPacketCount)
            {
                this.nReviceDataSize = nReviceDataSize;
                this.nThrowFrameCount = nThrowFrameCount;
                this.nNetRecvFrameCount = nNetRecvFrameCount;
                this.nRequestResendPacketCount = nRequestResendPacketCount;
                this.nResendPacketCount = nResendPacketCount;
            }
        }

        public struct MV_FRAME_OUT_INFO
        {
            public ushort nWidth;

            public ushort nHeight;

            public MyCamera.MvGvspPixelType enPixelType;

            public uint nFrameNum;

            public uint nDevTimeStampHigh;

            public uint nDevTimeStampLow;

            public uint nReserved0;

            public long nHostTimeStamp;

            public uint nFrameLen;

            public uint nLostPacket;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] nReserved;

            public MV_FRAME_OUT_INFO(ushort nWidth, ushort nHeight, MvGvspPixelType enPixelType, uint nFrameNum, uint nDevTimeStampHigh, uint nDevTimeStampLow, uint nReserved0, long nHostTimeStamp, uint nFrameLen, uint nLostPacket, uint[] nReserved)
            {
                this.nWidth = nWidth;
                this.nHeight = nHeight;
                this.enPixelType = enPixelType;
                this.nFrameNum = nFrameNum;
                this.nDevTimeStampHigh = nDevTimeStampHigh;
                this.nDevTimeStampLow = nDevTimeStampLow;
                this.nReserved0 = nReserved0;
                this.nHostTimeStamp = nHostTimeStamp;
                this.nFrameLen = nFrameLen;
                this.nLostPacket = nLostPacket;
                this.nReserved = nReserved;
            }
        }

        public struct MV_CHUNK_DATA_CONTENT
        {
            public IntPtr pChunkData;

            public uint nChunkID;

            public uint nChunkLen;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nReserved;

            public MV_CHUNK_DATA_CONTENT(IntPtr pChunkData, uint nChunkID, uint nChunkLen, uint[] nReserved)
            {
                this.pChunkData = pChunkData;
                this.nChunkID = nChunkID;
                this.nChunkLen = nChunkLen;
                this.nReserved = nReserved;
            }
        }

        public struct MV_FRAME_OUT_INFO_EX
        {
            public ushort nWidth;

            public ushort nHeight;

            public MyCamera.MvGvspPixelType enPixelType;

            public uint nFrameNum;

            public uint nDevTimeStampHigh;

            public uint nDevTimeStampLow;

            public uint nReserved0;

            public long nHostTimeStamp;

            public uint nFrameLen;

            public uint nSecondCount;

            public uint nCycleCount;

            public uint nCycleOffset;

            public float fGain;

            public float fExposureTime;

            public uint nAverageBrightness;

            public uint nRed;

            public uint nGreen;

            public uint nBlue;

            public uint nFrameCounter;

            public uint nTriggerIndex;

            public uint nInput;

            public uint nOutput;

            public ushort nOffsetX;

            public ushort nOffsetY;

            public ushort nChunkWidth;

            public ushort nChunkHeight;

            public uint nLostPacket;

            public uint nUnparsedChunkNum;

            public MyCamera.MV_FRAME_OUT_INFO_EX.UNPARSED_CHUNK_LIST UnparsedChunkList;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x24)]
            public uint[] nReserved;

            public MV_FRAME_OUT_INFO_EX(ushort nWidth, ushort nHeight, MvGvspPixelType enPixelType, uint nFrameNum, uint nDevTimeStampHigh, uint nDevTimeStampLow, uint nReserved0, long nHostTimeStamp, uint nFrameLen, uint nSecondCount, uint nCycleCount, uint nCycleOffset, float fGain, float fExposureTime, uint nAverageBrightness, uint nRed, uint nGreen, uint nBlue, uint nFrameCounter, uint nTriggerIndex, uint nInput, uint nOutput, ushort nOffsetX, ushort nOffsetY, ushort nChunkWidth, ushort nChunkHeight, uint nLostPacket, uint nUnparsedChunkNum, UNPARSED_CHUNK_LIST unparsedChunkList, uint[] nReserved)
            {
                this.nWidth = nWidth;
                this.nHeight = nHeight;
                this.enPixelType = enPixelType;
                this.nFrameNum = nFrameNum;
                this.nDevTimeStampHigh = nDevTimeStampHigh;
                this.nDevTimeStampLow = nDevTimeStampLow;
                this.nReserved0 = nReserved0;
                this.nHostTimeStamp = nHostTimeStamp;
                this.nFrameLen = nFrameLen;
                this.nSecondCount = nSecondCount;
                this.nCycleCount = nCycleCount;
                this.nCycleOffset = nCycleOffset;
                this.fGain = fGain;
                this.fExposureTime = fExposureTime;
                this.nAverageBrightness = nAverageBrightness;
                this.nRed = nRed;
                this.nGreen = nGreen;
                this.nBlue = nBlue;
                this.nFrameCounter = nFrameCounter;
                this.nTriggerIndex = nTriggerIndex;
                this.nInput = nInput;
                this.nOutput = nOutput;
                this.nOffsetX = nOffsetX;
                this.nOffsetY = nOffsetY;
                this.nChunkWidth = nChunkWidth;
                this.nChunkHeight = nChunkHeight;
                this.nLostPacket = nLostPacket;
                this.nUnparsedChunkNum = nUnparsedChunkNum;
                UnparsedChunkList = unparsedChunkList;
                this.nReserved = nReserved;
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct UNPARSED_CHUNK_LIST
            {
                [FieldOffset(0)]
                public IntPtr pUnparsedChunkContent;

                [FieldOffset(0)]
                public long nAligning;
            }
        }

        public struct MV_FRAME_OUT
        {
            public IntPtr pBufAddr;

            public MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public uint[] nReserved;

            public MV_FRAME_OUT(IntPtr pBufAddr, MV_FRAME_OUT_INFO_EX stFrameInfo, uint[] nReserved)
            {
                this.pBufAddr = pBufAddr;
                this.stFrameInfo = stFrameInfo;
                this.nReserved = nReserved;
            }
        }

        public enum MV_GRAB_STRATEGY
        {
            MV_GrabStrategy_OneByOne,
            MV_GrabStrategy_LatestImagesOnly,
            MV_GrabStrategy_LatestImages,
            MV_GrabStrategy_UpcomingImage
        }

        public struct MV_DISPLAY_FRAME_INFO
        {
            public IntPtr hWnd;

            public IntPtr pData;

            public uint nDataLen;

            public ushort nWidth;

            public ushort nHeight;

            public MyCamera.MvGvspPixelType enPixelType;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_DISPLAY_FRAME_INFO(IntPtr hWnd, IntPtr pData, uint nDataLen, ushort nWidth, ushort nHeight, MvGvspPixelType enPixelType, uint[] nReserved)
            {
                this.hWnd = hWnd;
                this.pData = pData;
                this.nDataLen = nDataLen;
                this.nWidth = nWidth;
                this.nHeight = nHeight;
                this.enPixelType = enPixelType;
                this.nReserved = nReserved;
            }
        }

        public enum MV_SAVE_IAMGE_TYPE
        {
            MV_Image_Undefined,
            MV_Image_Bmp,
            MV_Image_Jpeg,
            MV_Image_Png,
            MV_Image_Tif
        }

        public struct MV_SAVE_POINT_CLOUD_PARAM
        {
            public uint nLinePntNum;

            public uint nLineNum;

            public MyCamera.MvGvspPixelType enSrcPixelType;

            public IntPtr pSrcData;

            public uint nSrcDataLen;

            public IntPtr pDstBuf;

            public uint nDstBufSize;

            public uint nDstBufLen;

            public MyCamera.MV_SAVE_POINT_CLOUD_FILE_TYPE enPointCloudFileType;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;

            public MV_SAVE_POINT_CLOUD_PARAM(uint nLinePntNum, uint nLineNum, MvGvspPixelType enSrcPixelType, IntPtr pSrcData, uint nSrcDataLen, IntPtr pDstBuf, uint nDstBufSize, uint nDstBufLen, MV_SAVE_POINT_CLOUD_FILE_TYPE enPointCloudFileType, uint[] nRes)
            {
                this.nLinePntNum = nLinePntNum;
                this.nLineNum = nLineNum;
                this.enSrcPixelType = enSrcPixelType;
                this.pSrcData = pSrcData;
                this.nSrcDataLen = nSrcDataLen;
                this.pDstBuf = pDstBuf;
                this.nDstBufSize = nDstBufSize;
                this.nDstBufLen = nDstBufLen;
                this.enPointCloudFileType = enPointCloudFileType;
                this.nRes = nRes;
            }
        }

        public struct MV_SAVE_IMAGE_PARAM
        {
            public IntPtr pData;

            public uint nDataLen;

            public MyCamera.MvGvspPixelType enPixelType;

            public ushort nWidth;

            public ushort nHeight;

            public IntPtr pImageBuffer;

            public uint nImageLen;

            public uint nBufferSize;

            public MyCamera.MV_SAVE_IAMGE_TYPE enImageType;

            public MV_SAVE_IMAGE_PARAM(IntPtr pData, uint nDataLen, MvGvspPixelType enPixelType, ushort nWidth, ushort nHeight, IntPtr pImageBuffer, uint nImageLen, uint nBufferSize, MV_SAVE_IAMGE_TYPE enImageType)
            {
                this.pData = pData;
                this.nDataLen = nDataLen;
                this.enPixelType = enPixelType;
                this.nWidth = nWidth;
                this.nHeight = nHeight;
                this.pImageBuffer = pImageBuffer;
                this.nImageLen = nImageLen;
                this.nBufferSize = nBufferSize;
                this.enImageType = enImageType;
            }
        }

        public struct MV_SAVE_IMAGE_PARAM_EX
        {
            public IntPtr pData;

            public uint nDataLen;

            public MyCamera.MvGvspPixelType enPixelType;

            public ushort nWidth;

            public ushort nHeight;

            public IntPtr pImageBuffer;

            public uint nImageLen;

            public uint nBufferSize;

            public MyCamera.MV_SAVE_IAMGE_TYPE enImageType;

            public uint nJpgQuality;

            public uint iMethodValue;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public uint[] nReserved;

            public MV_SAVE_IMAGE_PARAM_EX(IntPtr pData, uint nDataLen, MvGvspPixelType enPixelType, ushort nWidth, ushort nHeight, IntPtr pImageBuffer, uint nImageLen, uint nBufferSize, MV_SAVE_IAMGE_TYPE enImageType, uint nJpgQuality, uint iMethodValue, uint[] nReserved)
            {
                this.pData = pData;
                this.nDataLen = nDataLen;
                this.enPixelType = enPixelType;
                this.nWidth = nWidth;
                this.nHeight = nHeight;
                this.pImageBuffer = pImageBuffer;
                this.nImageLen = nImageLen;
                this.nBufferSize = nBufferSize;
                this.enImageType = enImageType;
                this.nJpgQuality = nJpgQuality;
                this.iMethodValue = iMethodValue;
                this.nReserved = nReserved;
            }
        }

        public struct MV_SAVE_IMG_TO_FILE_PARAM
        {
            public MyCamera.MvGvspPixelType enPixelType;

            public IntPtr pData;

            public uint nDataLen;

            public ushort nWidth;

            public ushort nHeight;

            public MyCamera.MV_SAVE_IAMGE_TYPE enImageType;

            public uint nQuality;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
            public string pImagePath;

            public uint iMethodValue;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;

            public MV_SAVE_IMG_TO_FILE_PARAM(MvGvspPixelType enPixelType, IntPtr pData, uint nDataLen, ushort nWidth, ushort nHeight, MV_SAVE_IAMGE_TYPE enImageType, uint nQuality, string pImagePath, uint iMethodValue, uint[] nRes)
            {
                this.enPixelType = enPixelType;
                this.pData = pData;
                this.nDataLen = nDataLen;
                this.nWidth = nWidth;
                this.nHeight = nHeight;
                this.enImageType = enImageType;
                this.nQuality = nQuality;
                this.pImagePath = pImagePath;
                this.iMethodValue = iMethodValue;
                this.nRes = nRes;
            }
        }

        public enum MV_IMG_ROTATION_ANGLE
        {
            MV_IMAGE_ROTATE_90 = 1,
            MV_IMAGE_ROTATE_180,
            MV_IMAGE_ROTATE_270
        }

        public struct MV_CC_ROTATE_IMAGE_PARAM
        {
            public MyCamera.MvGvspPixelType enPixelType;

            public uint nWidth;

            public uint nHeight;

            public IntPtr pSrcData;

            public uint nSrcDataLen;

            public IntPtr pDstBuf;

            public uint nDstBufLen;

            public uint nDstBufSize;

            public MyCamera.MV_IMG_ROTATION_ANGLE enRotationAngle;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;

            public MV_CC_ROTATE_IMAGE_PARAM(MvGvspPixelType enPixelType, uint nWidth, uint nHeight, IntPtr pSrcData, uint nSrcDataLen, IntPtr pDstBuf, uint nDstBufLen, uint nDstBufSize, MV_IMG_ROTATION_ANGLE enRotationAngle, uint[] nRes)
            {
                this.enPixelType = enPixelType;
                this.nWidth = nWidth;
                this.nHeight = nHeight;
                this.pSrcData = pSrcData;
                this.nSrcDataLen = nSrcDataLen;
                this.pDstBuf = pDstBuf;
                this.nDstBufLen = nDstBufLen;
                this.nDstBufSize = nDstBufSize;
                this.enRotationAngle = enRotationAngle;
                this.nRes = nRes;
            }
        }

        public enum MV_IMG_FLIP_TYPE
        {
            MV_FLIP_VERTICAL = 1,
            MV_FLIP_HORIZONTAL
        }

        public struct MV_CC_FLIP_IMAGE_PARAM
        {
            public MyCamera.MvGvspPixelType enPixelType;

            public uint nWidth;

            public uint nHeight;

            public IntPtr pSrcData;

            public uint nSrcDataLen;

            public IntPtr pDstBuf;

            public uint nDstBufLen;

            public uint nDstBufSize;

            public MyCamera.MV_IMG_FLIP_TYPE enFlipType;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;

            public MV_CC_FLIP_IMAGE_PARAM(MvGvspPixelType enPixelType, uint nWidth, uint nHeight, IntPtr pSrcData, uint nSrcDataLen, IntPtr pDstBuf, uint nDstBufLen, uint nDstBufSize, MV_IMG_FLIP_TYPE enFlipType, uint[] nRes)
            {
                this.enPixelType = enPixelType;
                this.nWidth = nWidth;
                this.nHeight = nHeight;
                this.pSrcData = pSrcData;
                this.nSrcDataLen = nSrcDataLen;
                this.pDstBuf = pDstBuf;
                this.nDstBufLen = nDstBufLen;
                this.nDstBufSize = nDstBufSize;
                this.enFlipType = enFlipType;
                this.nRes = nRes;
            }
        }

        public struct MV_PIXEL_CONVERT_PARAM
        {
            public ushort nWidth;

            public ushort nHeight;

            public MyCamera.MvGvspPixelType enSrcPixelType;

            public IntPtr pSrcData;

            public uint nSrcDataLen;

            public MyCamera.MvGvspPixelType enDstPixelType;

            public IntPtr pDstBuffer;

            public uint nDstLen;

            public uint nDstBufferSize;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nRes;

            public MV_PIXEL_CONVERT_PARAM(ushort nWidth, ushort nHeight, MvGvspPixelType enSrcPixelType, IntPtr pSrcData, uint nSrcDataLen, MvGvspPixelType enDstPixelType, IntPtr pDstBuffer, uint nDstLen, uint nDstBufferSize, uint[] nRes)
            {
                this.nWidth = nWidth;
                this.nHeight = nHeight;
                this.enSrcPixelType = enSrcPixelType;
                this.pSrcData = pSrcData;
                this.nSrcDataLen = nSrcDataLen;
                this.enDstPixelType = enDstPixelType;
                this.pDstBuffer = pDstBuffer;
                this.nDstLen = nDstLen;
                this.nDstBufferSize = nDstBufferSize;
                this.nRes = nRes;
            }
        }

        public enum MV_CC_GAMMA_TYPE
        {
            MV_CC_GAMMA_TYPE_NONE,
            MV_CC_GAMMA_TYPE_VALUE,
            MV_CC_GAMMA_TYPE_USER_CURVE,
            MV_CC_GAMMA_TYPE_LRGB2SRGB,
            MV_CC_GAMMA_TYPE_SRGB2LRGB
        }

        public struct MV_CC_GAMMA_PARAM
        {
            public MyCamera.MV_CC_GAMMA_TYPE enGammaType;

            public float fGammaValue;

            public IntPtr pGammaCurveBuf;

            public uint nGammaCurveBufLen;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;

            public MV_CC_GAMMA_PARAM(MV_CC_GAMMA_TYPE enGammaType, float fGammaValue, IntPtr pGammaCurveBuf, uint nGammaCurveBufLen, uint[] nRes)
            {
                this.enGammaType = enGammaType;
                this.fGammaValue = fGammaValue;
                this.pGammaCurveBuf = pGammaCurveBuf;
                this.nGammaCurveBufLen = nGammaCurveBufLen;
                this.nRes = nRes;
            }
        }

        public struct MV_CC_CCM_PARAM
        {
            public bool bCCMEnable;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public int[] nCCMat;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;

            public MV_CC_CCM_PARAM(bool bCCMEnable, int[] nCCMat, uint[] nRes)
            {
                this.bCCMEnable = bCCMEnable;
                this.nCCMat = nCCMat;
                this.nRes = nRes;
            }
        }

        public struct MV_CC_CCM_PARAM_EX
        {
            public bool bCCMEnable;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public int[] nCCMat;

            public uint nCCMScale;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;

            public MV_CC_CCM_PARAM_EX(bool bCCMEnable, int[] nCCMat, uint nCCMScale, uint[] nRes)
            {
                this.bCCMEnable = bCCMEnable;
                this.nCCMat = nCCMat;
                this.nCCMScale = nCCMScale;
                this.nRes = nRes;
            }
        }

        public struct MV_CC_CLUT_PARAM
        {
            public bool bCLUTEnable;

            public uint nCLUTScale;

            public uint nCLUTSize;

            public IntPtr pCLUTBuf;

            public uint nCLUTBufLen;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;

            public MV_CC_CLUT_PARAM(bool bCLUTEnable, uint nCLUTScale, uint nCLUTSize, IntPtr pCLUTBuf, uint nCLUTBufLen, uint[] nRes)
            {
                this.bCLUTEnable = bCLUTEnable;
                this.nCLUTScale = nCLUTScale;
                this.nCLUTSize = nCLUTSize;
                this.pCLUTBuf = pCLUTBuf;
                this.nCLUTBufLen = nCLUTBufLen;
                this.nRes = nRes;
            }
        }

        public struct MV_CC_CONTRAST_PARAM
        {
            public uint nWidth;

            public uint nHeight;

            public IntPtr pSrcBuf;

            public uint nSrcBufLen;

            public MyCamera.MvGvspPixelType enPixelType;

            public IntPtr pDstBuf;

            public uint nDstBufSize;

            public uint nDstBufLen;

            public uint nContrastFactor;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public struct MV_CC_SHARPEN_PARAM
        {
            public uint nWidth;

            public uint nHeight;

            public IntPtr pSrcBuf;

            public uint nSrcBufLen;

            public MyCamera.MvGvspPixelType enPixelType;

            public IntPtr pDstBuf;

            public uint nDstBufSize;

            public uint nDstBufLen;

            public uint nSharpenAmount;

            public uint nSharpenRadius;

            public uint nSharpenThreshold;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public struct MV_CC_COLOR_CORRECT_PARAM
        {
            public uint nWidth;

            public uint nHeight;

            public IntPtr pSrcBuf;

            public uint nSrcBufLen;

            public MyCamera.MvGvspPixelType enPixelType;

            public IntPtr pDstBuf;

            public uint nDstBufSize;

            public uint nDstBufLen;

            public uint nImageBit;

            public MyCamera.MV_CC_GAMMA_PARAM stGammaParam;

            public MyCamera.MV_CC_CCM_PARAM_EX stCCMParam;

            public MyCamera.MV_CC_CLUT_PARAM stCLUTParam;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public struct MV_CC_RECT_I
        {
            public uint nX;

            public uint nY;

            public uint nWidth;

            public uint nHeight;

            public MV_CC_RECT_I(uint nX, uint nY, uint nWidth, uint nHeight)
            {
                this.nX = nX;
                this.nY = nY;
                this.nWidth = nWidth;
                this.nHeight = nHeight;
            }
        }

        public struct MV_CC_NOISE_ESTIMATE_PARAM
        {
            public uint nWidth;

            public uint nHeight;

            public MyCamera.MvGvspPixelType enPixelType;

            public IntPtr pSrcBuf;

            public uint nSrcBufLen;

            public IntPtr pstROIRect;

            public uint nROINum;

            public uint nNoiseThreshold;

            public IntPtr pNoiseProfile;

            public uint nNoiseProfileSize;

            public uint nNoiseProfileLen;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public struct MV_CC_SPATIAL_DENOISE_PARAM
        {
            public uint nWidth;

            public uint nHeight;

            public MyCamera.MvGvspPixelType enPixelType;

            public IntPtr pSrcBuf;

            public uint nSrcBufLen;

            public IntPtr pDstBuf;

            public uint nDstBufSize;

            public uint nDstBufLen;

            public IntPtr pNoiseProfile;

            public uint nNoiseProfileLen;

            public uint nBayerDenoiseStrength;

            public uint nBayerSharpenStrength;

            public uint nBayerNoiseCorrect;

            public uint nNoiseCorrectLum;

            public uint nNoiseCorrectChrom;

            public uint nStrengthLum;

            public uint nStrengthChrom;

            public uint nStrengthSharpen;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public struct MV_CC_LSC_CALIB_PARAM
        {
            public uint nWidth;

            public uint nHeight;

            public MyCamera.MvGvspPixelType enPixelType;

            public IntPtr pSrcBuf;

            public uint nSrcBufLen;

            public IntPtr pCalibBuf;

            public uint nCalibBufSize;

            public uint nCalibBufLen;

            public uint nSecNumW;

            public uint nSecNumH;

            public uint nPadCoef;

            public uint nCalibMethod;

            public uint nTargetGray;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public struct MV_CC_LSC_CORRECT_PARAM
        {
            public uint nWidth;

            public uint nHeight;

            public MyCamera.MvGvspPixelType enPixelType;

            public IntPtr pSrcBuf;

            public uint nSrcBufLen;

            public IntPtr pDstBuf;

            public uint nDstBufSize;

            public uint nDstBufLen;

            public IntPtr pCalibBuf;

            public uint nCalibBufLen;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public enum MV_CC_BAYER_NOISE_FEATURE_TYPE
        {
            MV_CC_BAYER_NOISE_FEATURE_TYPE_INVALID,
            MV_CC_BAYER_NOISE_FEATURE_TYPE_PROFILE,
            MV_CC_BAYER_NOISE_FEATURE_TYPE_LEVEL,
            MV_CC_BAYER_NOISE_FEATURE_TYPE_DEFAULT = 2
        }

        public struct MV_CC_BAYER_NOISE_PROFILE_INFO
        {
            public uint nVersion;

            public MyCamera.MV_CC_BAYER_NOISE_FEATURE_TYPE enNoiseFeatureType;

            public MyCamera.MvGvspPixelType enPixelType;

            public int nNoiseLevel;

            public uint nCurvePointNum;

            public IntPtr nNoiseCurve;

            public IntPtr nLumCurve;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public struct MV_CC_BAYER_NOISE_ESTIMATE_PARAM
        {
            public uint nWidth;

            public uint nHeight;

            public MyCamera.MvGvspPixelType enPixelType;

            public IntPtr pSrcData;

            public uint nSrcDataLen;

            public uint nNoiseThreshold;

            public IntPtr pCurveBuf;

            public MyCamera.MV_CC_BAYER_NOISE_PROFILE_INFO stNoiseProfile;

            public uint nThreadNum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public struct MV_CC_BAYER_SPATIAL_DENOISE_PARAM
        {
            public uint nWidth;

            public uint nHeight;

            public MyCamera.MvGvspPixelType enPixelType;

            public IntPtr pSrcData;

            public uint nSrcDataLen;

            public IntPtr pDstBuf;

            public uint nDstBufSize;

            public uint nDstBufLen;

            public MyCamera.MV_CC_BAYER_NOISE_PROFILE_INFO stNoiseProfile;

            public uint nDenoiseStrength;

            public uint nSharpenStrength;

            public uint nNoiseCorrect;

            public uint nThreadNum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public struct MV_CC_FRAME_SPEC_INFO
        {
            public uint nSecondCount;

            public uint nCycleCount;

            public uint nCycleOffset;

            public float fGain;

            public float fExposureTime;

            public uint nAverageBrightness;

            public uint nRed;

            public uint nGreen;

            public uint nBlue;

            public uint nFrameCounter;

            public uint nTriggerIndex;

            public uint nInput;

            public uint nOutput;

            public ushort nOffsetX;

            public ushort nOffsetY;

            public ushort nFrameWidth;

            public ushort nFrameHeight;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public uint[] nRes;
        }

        public struct MV_CC_HB_DECODE_PARAM
        {
            public IntPtr pSrcBuf;

            public uint nSrcLen;

            public uint nWidth;

            public uint nHeight;

            public IntPtr pDstBuf;

            public uint nDstBufSize;

            public uint nDstBufLen;

            public MyCamera.MvGvspPixelType enDstPixelType;

            public MyCamera.MV_CC_FRAME_SPEC_INFO stFrameSpecInfo;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public enum MV_RECORD_FORMAT_TYPE
        {
            MV_FormatType_Undefined,
            MV_FormatType_AVI
        }

        public enum MV_SAVE_POINT_CLOUD_FILE_TYPE
        {
            MV_PointCloudFile_Undefined,
            MV_PointCloudFile_PLY,
            MV_PointCloudFile_CSV,
            MV_PointCloudFile_OBJ
        }

        public struct MV_CC_RECORD_PARAM
        {
            public MyCamera.MvGvspPixelType enPixelType;

            public ushort nWidth;

            public ushort nHeight;

            public float fFrameRate;

            public uint nBitRate;

            public MyCamera.MV_RECORD_FORMAT_TYPE enRecordFmtType;

            public string strFilePath;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public struct MV_CC_INPUT_FRAME_INFO
        {
            public IntPtr pData;

            public uint nDataLen;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nRes;
        }

        public enum MV_CAM_ACQUISITION_MODE
        {
            MV_ACQ_MODE_SINGLE,
            MV_ACQ_MODE_MUTLI,
            MV_ACQ_MODE_CONTINUOUS
        }

        public enum MV_CAM_GAIN_MODE
        {
            MV_GAIN_MODE_OFF,
            MV_GAIN_MODE_ONCE,
            MV_GAIN_MODE_CONTINUOUS
        }

        public enum MV_CAM_EXPOSURE_MODE
        {
            MV_EXPOSURE_MODE_TIMED,
            MV_EXPOSURE_MODE_TRIGGER_WIDTH
        }

        public enum MV_CAM_EXPOSURE_AUTO_MODE
        {
            MV_EXPOSURE_AUTO_MODE_OFF,
            MV_EXPOSURE_AUTO_MODE_ONCE,
            MV_EXPOSURE_AUTO_MODE_CONTINUOUS
        }

        public enum MV_CAM_TRIGGER_MODE
        {
            MV_TRIGGER_MODE_OFF,
            MV_TRIGGER_MODE_ON
        }

        public enum MV_CAM_GAMMA_SELECTOR
        {
            MV_GAMMA_SELECTOR_USER = 1,
            MV_GAMMA_SELECTOR_SRGB
        }

        public enum MV_CAM_BALANCEWHITE_AUTO
        {
            MV_BALANCEWHITE_AUTO_OFF,
            MV_BALANCEWHITE_AUTO_ONCE = 2,
            MV_BALANCEWHITE_AUTO_CONTINUOUS = 1
        }

        public enum MV_CAM_TRIGGER_SOURCE
        {
            MV_TRIGGER_SOURCE_LINE0,
            MV_TRIGGER_SOURCE_LINE1,
            MV_TRIGGER_SOURCE_LINE2,
            MV_TRIGGER_SOURCE_LINE3,
            MV_TRIGGER_SOURCE_COUNTER0,
            MV_TRIGGER_SOURCE_SOFTWARE = 7,
            MV_TRIGGER_SOURCE_FrequencyConverter
        }

        public enum MV_GIGE_TRANSMISSION_TYPE
        {
            MV_GIGE_TRANSTYPE_UNICAST,
            MV_GIGE_TRANSTYPE_MULTICAST,
            MV_GIGE_TRANSTYPE_LIMITEDBROADCAST,
            MV_GIGE_TRANSTYPE_SUBNETBROADCAST,
            MV_GIGE_TRANSTYPE_CAMERADEFINED,
            MV_GIGE_TRANSTYPE_UNICAST_DEFINED_PORT,
            MV_GIGE_TRANSTYPE_UNICAST_WITHOUT_RECV = 0x10000,
            MV_GIGE_TRANSTYPE_MULTICAST_WITHOUT_RECV
        }

        public struct MV_ALL_MATCH_INFO
        {
            public uint nType;

            public IntPtr pInfo;

            public uint nInfoSize;
        }

        public struct MV_MATCH_INFO_NET_DETECT
        {
            public long nReviceDataSize;

            public long nLostPacketCount;

            public uint nLostFrameCount;

            public uint nNetRecvFrameCount;

            public long nRequestResendPacketCount;

            public long nResendPacketCount;

            public MV_MATCH_INFO_NET_DETECT(long nReviceDataSize, long nLostPacketCount, uint nLostFrameCount, uint nNetRecvFrameCount, long nRequestResendPacketCount, long nResendPacketCount)
            {
                this.nReviceDataSize = nReviceDataSize;
                this.nLostPacketCount = nLostPacketCount;
                this.nLostFrameCount = nLostFrameCount;
                this.nNetRecvFrameCount = nNetRecvFrameCount;
                this.nRequestResendPacketCount = nRequestResendPacketCount;
                this.nResendPacketCount = nResendPacketCount;
            }
        }

        //未使用
        public struct MV_MATCH_INFO_USB_DETECT
        {
            public long nReviceDataSize;

            public uint nRevicedFrameCount;

            public uint nErrorFrameCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] nReserved;

            public MV_MATCH_INFO_USB_DETECT(long nReviceDataSize, uint nRevicedFrameCount, uint nErrorFrameCount, uint[] nReserved)
            {
                this.nReviceDataSize = nReviceDataSize;
                this.nRevicedFrameCount = nRevicedFrameCount;
                this.nErrorFrameCount = nErrorFrameCount;
                this.nReserved = nReserved;
            }
        }

        public struct MV_IMAGE_BASIC_INFO
        {
            public ushort nWidthValue;

            public ushort nWidthMin;

            public uint nWidthMax;

            public uint nWidthInc;

            public uint nHeightValue;

            public uint nHeightMin;

            public uint nHeightMax;

            public uint nHeightInc;

            public float fFrameRateValue;

            public float fFrameRateMin;

            public float fFrameRateMax;

            public uint enPixelType;

            public uint nSupportedPixelFmtNum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
            public uint[] enPixelList;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nReserved;
        }

        public enum MV_XML_InterfaceType
        {
            IFT_IValue,
            IFT_IBase,
            IFT_IInteger,
            IFT_IBoolean,
            IFT_ICommand,
            IFT_IFloat,
            IFT_IString,
            IFT_IRegister,
            IFT_ICategory,
            IFT_IEnumeration,
            IFT_IEnumEntry,
            IFT_IPort
        }

        public enum MV_XML_AccessMode
        {
            AM_NI,
            AM_NA,
            AM_WO,
            AM_RO,
            AM_RW,
            AM_Undefined,
            AM_CycleDetect
        }

        public enum MV_XML_Visibility
        {
            V_Beginner,
            V_Expert,
            V_Guru,
            V_Invisible,
            V_Undefined = 0x63
        }

        public struct MV_EVENT_OUT_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
            public string EventName;

            public ushort nEventID;

            public ushort nStreamChannel;

            public uint nBlockIdHigh;

            public uint nBlockIdLow;

            public uint nTimestampHigh;

            public uint nTimestampLow;

            public IntPtr pEventData;

            public uint nEventDataSize;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public uint[] nReserved;

            public MV_EVENT_OUT_INFO(string eventName, ushort nEventID, ushort nStreamChannel, uint nBlockIdHigh, uint nBlockIdLow, uint nTimestampHigh, uint nTimestampLow, IntPtr pEventData, uint nEventDataSize, uint[] nReserved)
            {
                EventName = eventName;
                this.nEventID = nEventID;
                this.nStreamChannel = nStreamChannel;
                this.nBlockIdHigh = nBlockIdHigh;
                this.nBlockIdLow = nBlockIdLow;
                this.nTimestampHigh = nTimestampHigh;
                this.nTimestampLow = nTimestampLow;
                this.pEventData = pEventData;
                this.nEventDataSize = nEventDataSize;
                this.nReserved = nReserved;
            }
        }

        public struct MV_CC_FILE_ACCESS
        {
            public string pUserFileName;

            public string pDevFileName;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public uint[] nReserved;

            public MV_CC_FILE_ACCESS(string pUserFileName, string pDevFileName, uint[] nReserved)
            {
                this.pUserFileName = pUserFileName;
                this.pDevFileName = pDevFileName;
                this.nReserved = nReserved;
            }
        }

        public struct MV_CC_FILE_ACCESS_PROGRESS
        {
            public long nCompleted;

            public long nTotal;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nReserved;

            public MV_CC_FILE_ACCESS_PROGRESS(long nCompleted, long nTotal, uint[] nReserved)
            {
                this.nCompleted = nCompleted;
                this.nTotal = nTotal;
                this.nReserved = nReserved;
            }
        }

        public struct MV_CC_TRANSMISSION_TYPE
        {
            public MyCamera.MV_GIGE_TRANSMISSION_TYPE enTransmissionType;

            public uint nDestIp;

            public ushort nDestPort;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public uint[] nReserved;

            public MV_CC_TRANSMISSION_TYPE(MV_GIGE_TRANSMISSION_TYPE enTransmissionType, uint nDestIp, ushort nDestPort, uint[] nReserved)
            {
                this.enTransmissionType = enTransmissionType;
                this.nDestIp = nDestIp;
                this.nDestPort = nDestPort;
                this.nReserved = nReserved;
            }
        }

        public struct MV_ACTION_CMD_INFO
        {
            public uint nDeviceKey;

            public uint nGroupKey;

            public uint nGroupMask;

            public uint bActionTimeEnable;

            public long nActionTime;

            public string pBroadcastAddress;

            public uint nTimeOut;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public uint[] nReserved;

            public MV_ACTION_CMD_INFO(uint nDeviceKey, uint nGroupKey, uint nGroupMask, uint bActionTimeEnable, long nActionTime, string pBroadcastAddress, uint nTimeOut, uint[] nReserved)
            {
                this.nDeviceKey = nDeviceKey;
                this.nGroupKey = nGroupKey;
                this.nGroupMask = nGroupMask;
                this.bActionTimeEnable = bActionTimeEnable;
                this.nActionTime = nActionTime;
                this.pBroadcastAddress = pBroadcastAddress;
                this.nTimeOut = nTimeOut;
                this.nReserved = nReserved;
            }
        }

        public struct MV_ACTION_CMD_RESULT
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
            public string strDeviceAddress;

            public int nStatus;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_ACTION_CMD_RESULT(string strDeviceAddress, int nStatus, uint[] nReserved)
            {
                this.strDeviceAddress = strDeviceAddress;
                this.nStatus = nStatus;
                this.nReserved = nReserved;
            }
        }

        public struct MV_ACTION_CMD_RESULT_LIST
        {
            public uint nNumResults;

            public IntPtr pResults;

            public MV_ACTION_CMD_RESULT_LIST(uint nNumResults, IntPtr pResults)
            {
                this.nNumResults = nNumResults;
                this.pResults = pResults;
            }
        }

        public struct MV_XML_NODE_FEATURE
        {
            public MyCamera.MV_XML_InterfaceType enType;

            public MyCamera.MV_XML_Visibility enVisivility;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_XML_NODE_FEATURE(MV_XML_InterfaceType enType, MV_XML_Visibility enVisivility, string strDescription, string strDisplayName, string strName, string strToolTip, uint[] nReserved)
            {
                this.enType = enType;
                this.enVisivility = enVisivility;
                this.strDescription = strDescription;
                this.strDisplayName = strDisplayName;
                this.strName = strName;
                this.strToolTip = strToolTip;
                this.nReserved = nReserved;
            }
        }

        public struct MV_XML_NODES_LIST
        {
            public uint nNodeNum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
            public MyCamera.MV_XML_NODE_FEATURE[] stNodes;

            public MV_XML_NODES_LIST(uint nNodeNum, MV_XML_NODE_FEATURE[] stNodes)
            {
                this.nNodeNum = nNodeNum;
                this.stNodes = stNodes;
            }
        }

        public struct MVCC_INTVALUE
        {
            public uint nCurValue;

            public uint nMax;

            public uint nMin;

            public uint nInc;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MVCC_INTVALUE(uint nCurValue, uint nMax, uint nMin, uint nInc, uint[] nReserved)
            {
                this.nCurValue = nCurValue;
                this.nMax = nMax;
                this.nMin = nMin;
                this.nInc = nInc;
                this.nReserved = nReserved;
            }
        }

        public struct MVCC_INTVALUE_EX
        {
            public long nCurValue;

            public long nMax;

            public long nMin;

            public long nInc;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public uint[] nReserved;

            public MVCC_INTVALUE_EX(long nCurValue, long nMax, long nMin, long nInc, uint[] nReserved)
            {
                this.nCurValue = nCurValue;
                this.nMax = nMax;
                this.nMin = nMin;
                this.nInc = nInc;
                this.nReserved = nReserved;
            }
        }

        public struct MVCC_FLOATVALUE
        {
            public float fCurValue;

            public float fMax;

            public float fMin;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MVCC_FLOATVALUE(float fCurValue, float fMax, float fMin, uint[] nReserved)
            {
                this.fCurValue = fCurValue;
                this.fMax = fMax;
                this.fMin = fMin;
                this.nReserved = nReserved;
            }
        }

        public struct MVCC_ENUMVALUE
        {
            public uint nCurValue;

            public uint nSupportedNum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
            public uint[] nSupportValue;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MVCC_ENUMVALUE(uint nCurValue, uint nSupportedNum, uint[] nSupportValue, uint[] nReserved)
            {
                this.nCurValue = nCurValue;
                this.nSupportedNum = nSupportedNum;
                this.nSupportValue = nSupportValue;
                this.nReserved = nReserved;
            }
        }

        public struct MVCC_STRINGVALUE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
            public string chCurValue;

            public long nMaxLength;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] nReserved;

            public MVCC_STRINGVALUE(string chCurValue, long nMaxLength, uint[] nReserved)
            {
                this.chCurValue = chCurValue;
                this.nMaxLength = nMaxLength;
                this.nReserved = nReserved;
            }
        }

        //未使用
        public struct MV_XML_FEATURE_Integer
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            public MyCamera.MV_XML_Visibility enVisivility;

            public MyCamera.MV_XML_AccessMode enAccessMode;

            public int bIsLocked;

            public long nValue;

            public long nMinValue;

            public long nMaxValue;

            public long nIncrement;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_XML_FEATURE_Integer(string strName, string strDisplayName, string strDescription, string strToolTip, MV_XML_Visibility enVisivility, MV_XML_AccessMode enAccessMode, int bIsLocked, long nValue, long nMinValue, long nMaxValue, long nIncrement, uint[] nReserved)
            {
                this.strName = strName;
                this.strDisplayName = strDisplayName;
                this.strDescription = strDescription;
                this.strToolTip = strToolTip;
                this.enVisivility = enVisivility;
                this.enAccessMode = enAccessMode;
                this.bIsLocked = bIsLocked;
                this.nValue = nValue;
                this.nMinValue = nMinValue;
                this.nMaxValue = nMaxValue;
                this.nIncrement = nIncrement;
                this.nReserved = nReserved;
            }
        }

        /// <summary>
        /// 不使用的
        /// </summary>
        public struct MV_XML_FEATURE_Boolean
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            public MyCamera.MV_XML_Visibility enVisivility;

            public MyCamera.MV_XML_AccessMode enAccessMode;

            public int bIsLocked;

            public bool bValue;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_XML_FEATURE_Boolean(string strName, string strDisplayName, string strDescription, string strToolTip, MV_XML_Visibility enVisivility, MV_XML_AccessMode enAccessMode, int bIsLocked, bool bValue, uint[] nReserved)
            {
                this.strName = strName;
                this.strDisplayName = strDisplayName;
                this.strDescription = strDescription;
                this.strToolTip = strToolTip;
                this.enVisivility = enVisivility;
                this.enAccessMode = enAccessMode;
                this.bIsLocked = bIsLocked;
                this.bValue = bValue;
                this.nReserved = nReserved;
            }
        }

        public struct MV_XML_FEATURE_Command
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            public MyCamera.MV_XML_Visibility enVisivility;

            public MyCamera.MV_XML_AccessMode enAccessMode;

            public int bIsLocked;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_XML_FEATURE_Command(string strName, string strDisplayName, string strDescription, string strToolTip, MV_XML_Visibility enVisivility, MV_XML_AccessMode enAccessMode, int bIsLocked, uint[] nReserved)
            {
                this.strName = strName;
                this.strDisplayName = strDisplayName;
                this.strDescription = strDescription;
                this.strToolTip = strToolTip;
                this.enVisivility = enVisivility;
                this.enAccessMode = enAccessMode;
                this.bIsLocked = bIsLocked;
                this.nReserved = nReserved;
            }
        }

        public struct MV_XML_FEATURE_Float
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            public MyCamera.MV_XML_Visibility enVisivility;

            public MyCamera.MV_XML_AccessMode enAccessMode;

            public int bIsLocked;

            public double dfValue;

            public double dfMinValue;

            public double dfMaxValue;

            public double dfIncrement;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] vnReserved;

            public MV_XML_FEATURE_Float(string strName, string strDisplayName, string strDescription, string strToolTip, MV_XML_Visibility enVisivility, MV_XML_AccessMode enAccessMode, int bIsLocked, double dfValue, double dfMinValue, double dfMaxValue, double dfIncrement, uint[] vnReserved)
            {
                this.strName = strName;
                this.strDisplayName = strDisplayName;
                this.strDescription = strDescription;
                this.strToolTip = strToolTip;
                this.enVisivility = enVisivility;
                this.enAccessMode = enAccessMode;
                this.bIsLocked = bIsLocked;
                this.dfValue = dfValue;
                this.dfMinValue = dfMinValue;
                this.dfMaxValue = dfMaxValue;
                this.dfIncrement = dfIncrement;
                this.vnReserved = vnReserved;
            }
        }

        public struct MV_XML_FEATURE_String
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            public MyCamera.MV_XML_Visibility enVisivility;

            public MyCamera.MV_XML_AccessMode enAccessMode;

            public int bIsLocked;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strValue;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_XML_FEATURE_String(string strName, string strDisplayName, string strDescription, string strToolTip, MV_XML_Visibility enVisivility, MV_XML_AccessMode enAccessMode, int bIsLocked, string strValue, uint[] nReserved)
            {
                this.strName = strName;
                this.strDisplayName = strDisplayName;
                this.strDescription = strDescription;
                this.strToolTip = strToolTip;
                this.enVisivility = enVisivility;
                this.enAccessMode = enAccessMode;
                this.bIsLocked = bIsLocked;
                this.strValue = strValue;
                this.nReserved = nReserved;
            }
        }

        internal struct MV_XML_FEATURE_Register
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            public MyCamera.MV_XML_Visibility enVisivility;

            public MyCamera.MV_XML_AccessMode enAccessMode;

            public int bIsLocked;

            public long nAddrValue;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_XML_FEATURE_Register(string strName, string strDisplayName, string strDescription, string strToolTip, MV_XML_Visibility enVisivility, MV_XML_AccessMode enAccessMode, int bIsLocked, long nAddrValue, uint[] nReserved)
            {
                this.strName = strName;
                this.strDisplayName = strDisplayName;
                this.strDescription = strDescription;
                this.strToolTip = strToolTip;
                this.enVisivility = enVisivility;
                this.enAccessMode = enAccessMode;
                this.bIsLocked = bIsLocked;
                this.nAddrValue = nAddrValue;
                this.nReserved = nReserved;
            }
        }

        public struct MV_XML_FEATURE_Category
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            public MyCamera.MV_XML_Visibility enVisivility;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_XML_FEATURE_Category(string strDescription, string strDisplayName, string strName, string strToolTip, MV_XML_Visibility enVisivility, uint[] nReserved)
            {
                this.strDescription = strDescription;
                this.strDisplayName = strDisplayName;
                this.strName = strName;
                this.strToolTip = strToolTip;
                this.enVisivility = enVisivility;
                this.nReserved = nReserved;
            }
        }

        internal struct MV_XML_FEATURE_EnumEntry
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            public int bIsImplemented;

            public int nParentsNum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public MyCamera.MV_XML_NODE_FEATURE[] stParentsList;

            public MyCamera.MV_XML_Visibility enVisivility;

            public long nValue;

            public MyCamera.MV_XML_AccessMode enAccessMode;

            public int bIsLocked;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] nReserved;

            public MV_XML_FEATURE_EnumEntry(string strName, string strDisplayName, string strDescription, string strToolTip, int bIsImplemented, int nParentsNum, MV_XML_NODE_FEATURE[] stParentsList, MV_XML_Visibility enVisivility, long nValue, MV_XML_AccessMode enAccessMode, int bIsLocked, uint[] nReserved)
            {
                this.strName = strName;
                this.strDisplayName = strDisplayName;
                this.strDescription = strDescription;
                this.strToolTip = strToolTip;
                this.bIsImplemented = bIsImplemented;
                this.nParentsNum = nParentsNum;
                this.stParentsList = stParentsList;
                this.enVisivility = enVisivility;
                this.nValue = nValue;
                this.enAccessMode = enAccessMode;
                this.bIsLocked = bIsLocked;
                this.nReserved = nReserved;
            }
        }

        internal struct StrSymbolic
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string str;

            public StrSymbolic(string str)
            {
                this.str = str;
            }
        }

        internal struct MV_XML_FEATURE_Enumeration
        {
            public MyCamera.MV_XML_Visibility enVisivility;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            public int nSymbolicNum;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strCurrentSymbolic;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
            public MyCamera.StrSymbolic[] strSymbolic;

            public MyCamera.MV_XML_AccessMode enAccessMode;

            public int bIsLocked;

            public long nValue;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_XML_FEATURE_Enumeration(MV_XML_Visibility enVisivility, string strDescription, string strDisplayName, string strName, string strToolTip, int nSymbolicNum, string strCurrentSymbolic, StrSymbolic[] strSymbolic, MV_XML_AccessMode enAccessMode, int bIsLocked, long nValue, uint[] nReserved)
            {
                this.enVisivility = enVisivility;
                this.strDescription = strDescription;
                this.strDisplayName = strDisplayName;
                this.strName = strName;
                this.strToolTip = strToolTip;
                this.nSymbolicNum = nSymbolicNum;
                this.strCurrentSymbolic = strCurrentSymbolic;
                this.strSymbolic = strSymbolic;
                this.enAccessMode = enAccessMode;
                this.bIsLocked = bIsLocked;
                this.nValue = nValue;
                this.nReserved = nReserved;
            }
        }

        public struct MV_XML_FEATURE_Port
        {
            public MyCamera.MV_XML_Visibility enVisivility;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strDescription;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string strName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string strToolTip;

            public MyCamera.MV_XML_AccessMode enAccessMode;

            public int bIsLocked;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] nReserved;

            public MV_XML_FEATURE_Port(MV_XML_Visibility enVisivility, string strDescription, string strDisplayName, string strName, string strToolTip, MV_XML_AccessMode enAccessMode, int bIsLocked, uint[] nReserved)
            {
                this.enVisivility = enVisivility;
                this.strDescription = strDescription;
                this.strDisplayName = strDisplayName;
                this.strName = strName;
                this.strToolTip = strToolTip;
                this.enAccessMode = enAccessMode;
                this.bIsLocked = bIsLocked;
                this.nReserved = nReserved;
            }
        }
        #endregion
    }
}