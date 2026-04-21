using System;
using LCD.Data;

namespace LCD.Core.Services
{
    public static class ColorService
    {
        private const double Xn = 475.228;
        private const double Yn = 500;
        private const double Zn = 544.529;

        public static IData ToLab(double x, double y, double z)
        {
            IData data = new IData();
            data.Lcolor = 116 * F(y / Yn) - 16;
            data.Acolor = 500 * (F(x / Xn) - F(y / Yn));
            data.Bcolor = 200 * (F(y / Yn) - F(z / Zn));
            return data;
        }

        private static double F(double ratio)
        {
            if (ratio > Math.Pow(6 / 29.0, 3))
            {
                return Math.Pow(ratio, 1 / 3.0);
            }
            return 7.787037 * ratio + 0.137931;
        }
    }
}
