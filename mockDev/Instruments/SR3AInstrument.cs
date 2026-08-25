using System;
using System.Globalization;
using System.Text;

namespace MockDev.Instruments
{
    /// <summary>
    /// SR-3A 分光辐射亮度计模拟。
    /// LCD 端协议（见 LCD/Ctrl/SR3A.cs）：
    ///   RM       → 进入远程/复位，回 "OK"
    ///   D1 ST    → 测色坐标，回 头3行 + X/Y/Z/Cx/Cy/u/v/CCT(第4~11行) + END
    ///   D0 ST    → 测光谱，  回 上述头 + "380 值".."780 值" + END
    /// 注意：上位机在 RM 之后未清接收缓冲，"OK" 会占到解析后的第 0 行，
    ///       因此测量响应需再补 3 行占位(第1~3行)，使 X 落在第 4 行(datastrs[4])。
    /// </summary>
    public class SR3AInstrument : InstrumentBase
    {
        public override string Name => "SR3A";
        public override int BaudRate => 9600;

        private readonly Random _rand = new Random();

        public SR3AInstrument()
        {
            On("D0 ST", () => MeasureResponse(withSpectrum: true));
            On("D1 ST", () => MeasureResponse(withSpectrum: false));
            On("RM", () => "OK\r\n");
        }

        private static string F(double v) => v.ToString("F4", CultureInfo.InvariantCulture);

        private string MeasureResponse(bool withSpectrum)
        {
            // 模拟一组色度值（峰在 ~555nm 的类光谱曲线）
            double X = 100 + _rand.NextDouble() * 20;
            double Y = 200 + _rand.NextDouble() * 20;
            double Z = 150 + _rand.NextDouble() * 20;
            double sum = X + Y + Z;
            double Cx = X / sum;
            double Cy = Y / sum;
            double u = 4 * X / (X + 15 * Y + 3 * Z);
            double v = 9 * Y / (X + 15 * Y + 3 * Z);
            double cct = 6500 + _rand.NextDouble() * 50;

            var sb = new StringBuilder();
            // 头 3 行占位（对应解析后的 datastrs[1..3]，内容会被上位机忽略）
            sb.Append("SR-3A\r\n");
            sb.Append("OK\r\n");
            sb.Append(withSpectrum ? "D0\r\n" : "D1\r\n");
            // datastrs[4..11]：X / Y(=L) / Z / Cx / Cy / u / v / CCT
            sb.Append(F(X)).Append("\r\n");
            sb.Append(F(Y)).Append("\r\n");
            sb.Append(F(Z)).Append("\r\n");
            sb.Append(F(Cx)).Append("\r\n");
            sb.Append(F(Cy)).Append("\r\n");
            sb.Append(F(u)).Append("\r\n");
            sb.Append(F(v)).Append("\r\n");
            sb.Append(F(cct)).Append("\r\n");

            if (withSpectrum)
            {
                // "波长 值"，380~780nm 共 401 行
                for (int wl = 380; wl <= 780; wl++)
                {
                    double val = 100.0 * Math.Exp(-Math.Pow((wl - 555) / 80.0, 2)) + _rand.NextDouble();
                    sb.Append(wl).Append(' ').Append(F(val)).Append("\r\n");
                }
            }
            sb.Append("END\r\n");
            return sb.ToString();
        }
    }
}
