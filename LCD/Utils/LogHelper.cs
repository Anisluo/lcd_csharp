using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Collections.Concurrent;
using System.Text;

namespace LCD
{
    /// <summary>
    /// Structure of each log entry
    /// </summary>
    public class LogItem
    {
        public string time;
        public string module;
        public string content;
    }

    /// <summary>
    /// 
    /// summary:
    ///     Log consumer - A simple thread-safe log module that uses producer-consumer mode.
    /// 
    /// usage:
    ///     HuiHut.LogConsumer.LogConsumer.Instance.Write("your log content");
    ///     
    /// </summary>
    public class LogHelper
    {
        // Log file path and file name such as: @"D:\Log.txt", @"../Log.txt", @"Log.txt"
        private string dir = "log";
        private string logFileName = @"log/log";
        private FileStream logFileStream;
        private StreamWriter logStreamWriter;
        private int curent_size;
        private bool is_run;

        // Log cache queue
        private BlockingCollection<LogItem> queue = new BlockingCollection<LogItem>();
        private int _maxFileSize = 16*1024*1024;//单个日志文件的大小
        private int _maxFileCount = 9; //10个文件循环


        // Consumer thread: write log
        private Thread consumerThread;

        // Singleton
        private static LogHelper instance = new LogHelper();
        public static LogHelper Instance
        {
            get { return instance; }
            private set { }
        }

        private LogHelper()
        {
            // Open file stream
            OpenFileStream();
            is_run = true;
            // Log consumer
            consumerThread = new Thread(Consumer);
            consumerThread.Start();
        }

        /// <summary>
        /// 字节数组转16进制字符串：空格分隔
        /// </summary>
        /// <param name="byteDatas"></param>
        /// <returns></returns>
        public static string ToHexStrFromByte(byte[] byteDatas)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2} ", byteDatas[i]));
            }
            return builder.ToString().Trim();
        }

        ~LogHelper()
        {
            is_run = false;
            try
            {
                consumerThread.Abort();
            }
            catch
            {

            }
            CloseFileStream();
        }

        public void dispose()
        {
            is_run = false;
            try
            {
                consumerThread.Abort();
            }
            catch
            {

            }
            CloseFileStream();
        }

        /// <summary>
        /// Producer: Write the log. 
        /// Other modules write logs by calling this method.
        /// </summary>
        /// <param name="content">Log content</param>
        public void Write(string content)
        {
            Console.WriteLine(content);//首先打印出来

            // Time of each log entry
            string time = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss:fff");

            //下面是获取调用日志的类和方法信息
            // Get the module that calls Write ( namespace:class.method )
            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(1);
            MethodBase method = frame.GetMethod();
            string namespaceName = method.ReflectedType.Namespace;
            string className = method.ReflectedType.Name;
            string methodName = method.Name;
            string module = namespaceName + ":" + className + "." + methodName;

            // Production log entry
            var item = ProduceItem(time, module, content);

            queue.Add(item);
        }

        // Open file stream
        private void OpenFileStream()
        {
            string file1, file2;
            if (Directory.Exists(dir) == false)
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch
                {

                }
            }
            file1 = logFileName + "0.txt";//0文件永远是最新的啊
            if (File.Exists(file1))
            {
                FileInfo info = new FileInfo(file1);
                if (info.Length >= _maxFileSize)
                {
                    file1 = logFileName + _maxFileCount +".txt";
                    if (File.Exists(file1))
                    {
                        try
                        {
                            File.Delete(file1);//删除最大的
                        }
                        catch
                        {

                        }
                    }
                    //从后面往前面操作，删除最旧的日志文件
                    for (int i = _maxFileCount; i > 0; i--)
                    {
                        file1 = logFileName + (i - 1) + ".txt";
                        file2 = logFileName + i + ".txt";                        
                        if (File.Exists(file1))
                        {
                            try
                            {
                                File.Move(file1, file2);
                            }
                            catch
                            { 
                            }
                        }
                    }
                    curent_size = 0;
                }
                else
                {
                    curent_size = (int)info.Length;
                }
            }
            else
            {
                curent_size = 0;
            }
            file1 = logFileName + "0.txt";//0文件永远是最新的啊
            if (logFileStream == null)
            {
                try
                {
                    logFileStream = new FileStream(file1, FileMode.Append);
                }
                catch
                { }
            }
            if (logStreamWriter == null)
            {
                try
                {
                    logStreamWriter = new StreamWriter(logFileStream);
                }
                catch
                {

                }
            }
        }

        // Close file stream
        private void CloseFileStream()
        {
            if (logStreamWriter != null)
            {
                try
                {                    
                    logStreamWriter.Close();                    
                }
                catch { }
                logStreamWriter = null;
            }
            if (logFileStream != null)
            {
                try
                {
                    logFileStream.Close();
                }
                catch { }
                logFileStream = null;
            }
        }
        
        // Consumer
        private void Consumer()
        {
            while (is_run)
            {
                if (queue.Count > 0)
                {
                    foreach (LogItem _item in queue.GetConsumingEnumerable())
                    {
                        WriteLog(_item);
                    }                       
                }                
            }
        }

        // Produce Item
        private LogItem ProduceItem(string logTime, string logModule, string logContent)
        {
            LogItem item = new LogItem() { time = logTime, module = logModule, content = logContent };
            return item;
        }

        // Write log to file
        private void WriteLog(LogItem logItem)
        {
            if(!is_run)
            {
                return;
            }
            if (logStreamWriter == null)
            {
                OpenFileStream();
            }
            //如果文件到了最大值，则重新打开新的日志文件
            if(curent_size >= _maxFileSize)
            {
                CloseFileStream();
                OpenFileStream();
            }
            string content = logItem.time + "  " + logItem.module + "  " + logItem.content;
            try
            {
                logStreamWriter?.WriteLine(content);
                logStreamWriter?.Flush();
            }
            catch { }
            curent_size = curent_size + content.Length;
        }
    }
}
