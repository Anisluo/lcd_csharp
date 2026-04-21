using log4net.Appender;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace VisionCore
{
    public class TextBoxBaseAppender : AppenderSkeleton
    {
        public Action<LogActionEnum, string> RichTextBoxLog;
        public Func<bool> ShouldClear;
        private bool m_Flag = false;//现场开启
        private object m_LockObj = new object();
        private List<string> m_LogList = new List<string>();
        static TextBoxBaseAppender()
        {
        }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            if (RichTextBoxLog == null)
                return;
            PatternLayout patternLayout = (PatternLayout)this.Layout;
            string str = string.Empty;
            if (patternLayout != null)
            {
                str = patternLayout.Format(loggingEvent);
                if (loggingEvent.ExceptionObject != null)
                {
                    str += loggingEvent.ExceptionObject.ToString() + Environment.NewLine;
                }
            }
            else
            {
                str = loggingEvent.LoggerName + "-" + loggingEvent.RenderedMessage + Environment.NewLine;
            }
            Printf(str);
        }
        private void Printf(string str)
        {
            lock (m_LockObj)
            {
                m_LogList.Add(str);
            }
            if (!m_Flag)
            {
                m_Flag = true;
                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(300);//日志刷新周期是200ms 避免 native刷新太块 抢占了主线程
                        if (m_LogList.Count == 0) continue;
                        List<string> tempList = null;
                        lock (m_LockObj)
                        {
                            tempList = CloneObject.DeepCopy(m_LogList);
                            m_LogList.Clear();
                        }
                        if (ShouldClear != null && ShouldClear.Invoke())//(RichTextBoxLog.Lines.Length > 1000)
                        {
                            RichTextBoxLog?.Invoke(LogActionEnum.Clear, "");
                        }
                        try
                        {
                            string logstr = string.Join("", tempList);
                            RichTextBoxLog?.Invoke(LogActionEnum.Append, logstr);//.AppendText(logstr);
                            RichTextBoxLog?.Invoke(LogActionEnum.Bottom, logstr);
                        }
                        catch (Exception ex)
                        {
                            Debug.Write(ex.Message);
                            throw;
                        }
                    }
                });
            }
        }
    }
    public enum LogActionEnum
    {
        Clear,
        Append,
        Bottom,
    }
}
