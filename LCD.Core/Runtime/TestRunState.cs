using System.Collections.Generic;
using LCD.Data;
using LCD.View;

namespace LCD.Core.Runtime
{
    /// <summary>
    /// 一次点阵测试执行的运行时状态：选中的模板、结果缓存、暂停/结束标志等。
    /// 旧代码通过 Project.TstPause / TestFlag / lstDatas / lstInfos / resultFormat 访问；
    /// Project 现以转发属性的形式保留向后兼容。
    /// </summary>
    public static class TestRunState
    {
        public static bool TstPause { get; set; } = false;
        public static bool TestFlag { get; set; } = false;

        public static List<ResultData> Results { get; set; } = new List<ResultData>();
        public static List<InfoData> Infos { get; set; } = new List<InfoData>();

        public static SortedDictionary<string, bool> ResultFormat { get; set; } =
            new SortedDictionary<string, bool>();
    }
}
