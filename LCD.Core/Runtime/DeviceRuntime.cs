using LCD.Core.Abstractions;

namespace LCD.Core.Runtime
{
    /// <summary>
    /// 硬件设备运行时单例槽位（按接口持有，不直接依赖驱动实现）。
    ///
    /// 目前只持有 Pattern Generator；
    /// TestMachine 仍由 UiRegistry.DeviceView.TestDevice 持有（与 DeviceView 面板同生命周期），
    /// Power 仍挂在 Project.power（等 LCD.Ctrl.Power 退役、迁到 IPowerSupply 之后再并入）。
    /// </summary>
    public static class DeviceRuntime
    {
        public static IPatternGenerator PG { get; set; }
    }
}
