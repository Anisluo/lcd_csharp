using System.Collections.Generic;
using LCD.dataBase;

namespace LCD.Core.Runtime
{
    /// <summary>
    /// 一次扫码/测试会话的运行时状态。
    /// 旧代码通过 Project.listBarCode / TestDataModes / ... 访问；
    /// Project 现以转发属性的形式保留向后兼容。
    /// </summary>
    public static class SessionState
    {
        public static List<UserIdMode> ListBarCode { get; set; } = new List<UserIdMode>();
        public static List<TestDataMode> TestDataModes { get; set; } = new List<TestDataMode>();
        public static List<SpectrumDataMode> ListSpectrumData { get; set; } = new List<SpectrumDataMode>();
        public static List<UserIdMode> SaveTestData { get; set; } = new List<UserIdMode>();

        public static int BarCodeID { get; set; }
        public static int ProjectID { get; set; }
    }
}
