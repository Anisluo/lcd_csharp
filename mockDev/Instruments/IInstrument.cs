using System;
using System.Collections.Generic;

namespace MockDev.Instruments
{
    /// <summary>
    /// 一台被模拟的仪器：解析 LCD 上位机发来的指令，产生该仪器真实的响应。
    /// 新增仪器只需实现本接口（或继承 InstrumentBase），在 InstrumentBase.Registry 里注册即可。
    /// </summary>
    public interface IInstrument
    {
        /// <summary>仪器名（UI 下拉显示，也用于分类）</summary>
        string Name { get; }

        /// <summary>建议波特率（虚拟串口其实不敏感，仅作提示 / 对齐配置）</summary>
        int BaudRate { get; }

        /// <summary>
        /// 处理已累积的接收缓冲。若识别出一条完整指令，返回要回给上位机的响应文本，
        /// 并通过 <paramref name="consumed"/> 告知本次消耗掉的字符数；
        /// 若还不足以构成完整指令，返回 null 且 consumed=0（继续等待后续数据）。
        /// </summary>
        string Process(string buffer, out int consumed);
    }

    /// <summary>仪器基类：提供"指令→响应"注册表 + 通用匹配逻辑。</summary>
    public abstract class InstrumentBase : IInstrument
    {
        public abstract string Name { get; }
        public virtual int BaudRate => 9600;

        /// <summary>指令处理器：key=指令文本，value=生成响应的委托。子类在构造里注册。</summary>
        protected readonly List<KeyValuePair<string, Func<string>>> Handlers
            = new List<KeyValuePair<string, Func<string>>>();

        /// <summary>注册一条指令及其响应生成器。先注册的优先匹配（长指令请先注册）。</summary>
        protected void On(string command, Func<string> responder)
        {
            Handlers.Add(new KeyValuePair<string, Func<string>>(command, responder));
        }

        public virtual string Process(string buffer, out int consumed)
        {
            consumed = 0;
            string trimmed = buffer.Trim();
            if (trimmed.Length == 0) return null;
            foreach (var h in Handlers)
            {
                // 上位机多为一次一条指令、发完等回复，故用"整条(去空白)相等或结尾匹配"即可稳妥识别
                if (trimmed == h.Key || trimmed.EndsWith(h.Key))
                {
                    consumed = buffer.Length; // 消耗整个缓冲
                    return h.Value();
                }
            }
            return null; // 未识别到完整指令，继续等
        }
    }
}
