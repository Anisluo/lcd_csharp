namespace LCD.Core.Runtime
{
    /// <summary>
    /// 运动/测试过程中跨模块共享的运行时状态（6 轴原点、停止标志）。
    /// 由 LCD/Data/Project.cs 里的 Stop/FstStop/Xorg/Yorg/Zorg/Uorg/Vorg/Ballorg 迁移而来，
    /// 使 LCD.Drv 的 MovCtrl / PointF 等不再上行依赖 Project。
    /// Project 中同名字段改为转发到此类，保持 LCD 侧消费者零改动。
    /// </summary>
    public static class MotionRuntime
    {
        /// <summary>用户点击"立即停止"后置 true；驱动在主循环中检测到后退出。</summary>
        public static bool Stop { get; set; } = false;

        /// <summary>"首次停止"标志，用于单次测试流程中途中止。</summary>
        public static bool FstStop { get; set; } = false;

        public static double Xorg { get; set; }
        public static double Yorg { get; set; }
        public static double Zorg { get; set; }
        public static double Uorg { get; set; }
        public static double Vorg { get; set; }
        public static double Ballorg { get; set; }
    }
}
