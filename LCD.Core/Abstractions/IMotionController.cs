namespace LCD.Core.Abstractions
{
    public enum MoveSpeed
    {
        Slow,
        Medium,
        Fast,
    }

    /// <summary>运动轴，对应 6 轴测试机台。</summary>
    public enum MotionAxis
    {
        X,
        Y,
        Z,
        U,
        V,
        Ball,
    }

    public enum MoveDirection
    {
        Positive,
        Negative,
    }

    /// <summary>
    /// 多轴运动控制抽象。
    /// 对应 LCD.Drv 里的实现：MPC08 板卡适配器（当前在 LCD.Ctrl.MovCtrl 里）。
    /// </summary>
    public interface IMotionController
    {
        void Init();

        void StopAll();

        /// <summary>回原点（所有轴）。</summary>
        void MoveHome();

        /// <summary>Jog 某一轴（点动）。</summary>
        void MoveAxis(MotionAxis axis, MoveDirection direction, MoveSpeed speed);

        /// <summary>停止单个轴。</summary>
        void MoveAxisStop(MotionAxis axis);

        /// <summary>绝对定位到指定坐标（mm 或 角度）。</summary>
        void MoveAbsolute(MotionAxis axis, double target, bool isHomeReturn);

        /// <summary>多轴联合到指定点（典型用法：测试点阵第 N 个点）。</summary>
        void MoveToPoint(double dx, double dy, double dz, double du, double dv, double dball, bool isHomeReturn);

        /// <summary>轮询当前轴状态是否就绪。</summary>
        bool IsMotionFinished();

        /// <summary>读取各轴当前绝对位置。</summary>
        void ReadCurrentPosition(out double x, out double y, out double z, out double u, out double v, out double ball);
    }
}
