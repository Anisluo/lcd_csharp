using System.Windows.Documents;
using LCD.View;

namespace LCD.Data
{
    /// <summary>
    /// 主窗口启动时注入的若干 UserControl/Window 实例。
    /// 历史上挂在 Project 全局静态上供跨组件使用；
    /// 这里集中持有，Project 通过转发属性访问（已按组拆分的第二轮）。
    /// </summary>
    public static class UiRegistry
    {
        public static ResutView Results { get; set; }
        public static ResutView SpectrumResults { get; set; }
        public static ResutView ResponseResults { get; set; }
        public static ResutView CrossTalkResults { get; set; }
        public static ResutView WarmupResult { get; set; }
        public static ResutView PowerResult { get; set; }

        public static DeviceView DeviceView { get; set; }
        public static PGDebug PGDebug { get; set; }
        public static CamView Cam { get; set; }
        public static V110 V110 { get; set; }

        /// <summary>MainWindow 的 RichTextBox 文档，用于 Project.WriteLog 追加日志行。</summary>
        public static FlowDocument LogDocument { get; set; }
    }
}
