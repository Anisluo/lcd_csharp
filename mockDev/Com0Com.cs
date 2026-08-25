using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace MockDev
{
    /// <summary>
    /// 调用 com0com 的 setupc.exe 创建/删除虚拟串口对。需管理员权限（本程序清单已声明）。
    /// </summary>
    public class Com0Com
    {
        public string SetupcPath { get; set; }

        /// <summary>在常见安装路径自动查找 setupc.exe。</summary>
        public static string AutoFindSetupc()
        {
            string[] candidates =
            {
                @"C:\Program Files (x86)\com0com\setupc.exe",
                @"C:\Program Files\com0com\setupc.exe",
                @"C:\Program Files (x86)\com0com\setup.exe",
            };
            foreach (var c in candidates)
                if (File.Exists(c)) return c;
            return null;
        }

        private string Run(string args, out int exit)
        {
            var psi = new ProcessStartInfo(SetupcPath, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(SetupcPath)
            };
            using (var p = Process.Start(psi))
            {
                string o = p.StandardOutput.ReadToEnd();
                string er = p.StandardError.ReadToEnd();
                p.WaitForExit(20000);
                exit = p.HasExited ? p.ExitCode : -1;
                return (o + er).Trim();
            }
        }

        /// <summary>
        /// 创建一对：<paramref name="portLcd"/>（LCD 连）↔ <paramref name="portMock"/>（mockDev 连）。
        /// 返回 com0com 分配的 pair 号（用于之后删除）；失败返回 -1。
        /// </summary>
        public int CreatePair(string portLcd, string portMock, out string output)
        {
            output = Run($"--silent install PortName={portLcd} PortName={portMock}", out int exit);
            var m = Regex.Match(output, @"CNC[AB](\d+)");
            return m.Success ? int.Parse(m.Groups[1].Value) : -1;
        }

        /// <summary>删除指定 pair。</summary>
        public string RemovePair(int pair, out int exit)
        {
            if (pair < 0) { exit = 0; return "(无有效 pair，跳过)"; }
            return Run($"--silent remove {pair}", out exit);
        }

        public string List()
        {
            return Run("--silent list", out int exit);
        }
    }
}
