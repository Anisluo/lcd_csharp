namespace LCD.Data
{
    /// <summary>
    /// 单轴运动参数（脉冲、速度、使能、极限等）。
    /// 由 LCD/Data/Project.cs 抽出，下沉到 LCD.Core 以便
    /// LCD.Drv 的运动驱动直接访问，不再穿透 Project 全局静态。
    /// </summary>
    public class Axies
    {
        public bool BackLash;
        /// <summary>报警启用</summary>
        public bool AlarmEnable;
        public AXiesName Name;
        public double HomeSpend;
        public int SpendIndex;
        public bool IsEnable;
        public int value;
        public int secondvalue;
        public bool IsSecondValue; // 判断是否有使能轴号
        public int direction;
        public double center;
        public double homespeed;
        public double SpeedFast;
        /// <summary>平均速度</summary>
        public double SpeedMedium;
        public double SpeedLow;
        /// <summary>脉冲/mm — 单位毫米对应的脉冲量</summary>
        public double StepsPerMM;
        public double acSpeed;
        /// <summary>软下限</summary>
        public double LowerLimit;
        /// <summary>最大范围</summary>
        public long UpperLimit;
        /// <summary>是否使能补偿方向</summary>
        public bool CompensationDirection;
    }

    public enum AXiesName
    {
        X轴,
        Y轴,
        Z轴,
        U轴,
        V轴,
        Ball轴,
    }
}
