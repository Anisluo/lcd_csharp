using System;

namespace LCD.Core.Abstractions
{
    public enum CameraTrigMode
    {
        Internal,
        External,
        Software,
    }

    /// <summary>一帧图像的回调；data 为原始字节（格式由实现决定，例如 Mono8/BGR24）。</summary>
    public delegate void CameraImageHandler(byte[] data, int width, int height);

    /// <summary>
    /// 工业相机抽象。
    /// 对应 LCD.Drv 里的实现：基于 MvCameraControl 的海康相机适配器。
    /// 参考现有基类 LCD.Core.Cameras.CamerasBase（将在 Phase 3 里把它改为 implements ICamera）。
    /// </summary>
    public interface ICamera : IDisposable
    {
        string CameraNo { get; }
        string SerialNo { get; }
        string CameraIp { get; }

        bool Connected { get; }

        int WidthMax { get; }
        int HeightMax { get; }

        CameraTrigMode TrigMode { get; set; }

        bool Connect();
        void Disconnect();

        /// <summary>开始连续采集；每帧通过 <see cref="ImageGrabbed"/> 回调。</summary>
        void StartGrab();
        void StopGrab();

        /// <summary>同步抓一帧（软触发）。</summary>
        byte[] GrabOne();

        event CameraImageHandler ImageGrabbed;
    }
}
