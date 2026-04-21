using System;
using System.Collections.Generic;

namespace LCD.Services
{
    public enum PointLayoutType
    {
        CrossTalk4,
        Point5,
        Point9,
        Point13,
        Point13Diag,
        Point15,
        Point17,
        Point17Array,
        Point25,
        Custom,
    }

    public sealed class PointLayoutInput
    {
        public double H;
        public double V;

        public double Amm, Bmm, Cmm, Dmm;
        public double Apct, Bpct, Cpct, Dpct;

        public bool UseMeter;

        public int Rows;
        public int Columns;
    }

    public sealed class PointPos
    {
        public int Id;
        public double XMm;
        public double YMm;
        public double XPct;
        public double YPct;
    }

    public static class PointLayoutService
    {
        public static IReadOnlyList<PointPos> Generate(PointLayoutType type, PointLayoutInput p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            switch (type)
            {
                case PointLayoutType.CrossTalk4:   return CrossTalk4(p);
                case PointLayoutType.Point5:       return Point5(p);
                case PointLayoutType.Point9:       return Point9(p);
                case PointLayoutType.Point13:      return Point13(p);
                case PointLayoutType.Point13Diag:  return Point13Diag(p);
                case PointLayoutType.Point15:      return Point15(p);
                case PointLayoutType.Point17:      return Point17(p);
                case PointLayoutType.Point17Array: return Point17Array(p);
                case PointLayoutType.Point25:      return Point25(p);
                case PointLayoutType.Custom:       return CustomGrid(p);
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        private static PointPos Make(int id, double xm, double ym, double xp, double yp, PointLayoutInput p)
        {
            if (!p.UseMeter)
            {
                xm = p.H * xp / 100.0;
                ym = p.V * yp / 100.0;
            }
            return new PointPos { Id = id, XMm = xm, YMm = ym, XPct = xp, YPct = yp };
        }

        private static List<PointPos> CrossTalk4(PointLayoutInput p)
        {
            var r = new List<PointPos>(4);
            r.Add(Make(1, p.Amm,       p.V / 2.0,           p.Apct,         50,             p));
            r.Add(Make(2, p.H / 2.0,   p.Bmm,               50,             p.Bpct,         p));
            r.Add(Make(3, p.H - p.Amm, p.V / 2.0,           100 - p.Apct,   50,             p));
            r.Add(Make(4, p.H / 2.0,   p.V - p.Bmm,         50,             100 - p.Bpct,   p));
            return r;
        }

        private static List<PointPos> Point5(PointLayoutInput p)
        {
            var r = new List<PointPos>(5);
            r.Add(Make(1, p.Amm,       p.Bmm,       p.Apct,       p.Bpct,       p));
            r.Add(Make(2, p.H - p.Amm, p.Bmm,       100 - p.Apct, p.Bpct,       p));
            r.Add(Make(3, p.H / 2.0,   p.V / 2.0,   50,           50,           p));
            r.Add(Make(4, p.Amm,       p.V - p.Bmm, p.Apct,       100 - p.Bpct, p));
            r.Add(Make(5, p.H - p.Amm, p.V - p.Bmm, 100 - p.Apct, 100 - p.Bpct, p));
            return r;
        }

        private static List<PointPos> Point9(PointLayoutInput p)
        {
            var r = new List<PointPos>(9);
            double[] xMm  = { p.Amm,       p.H / 2.0, p.H - p.Amm };
            double[] yMm  = { p.Bmm,       p.V / 2.0, p.V - p.Bmm };
            double[] xPct = { p.Apct,      50,        100 - p.Apct };
            double[] yPct = { p.Bpct,      50,        100 - p.Bpct };
            int id = 1;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    r.Add(Make(id++, xMm[j], yMm[i], xPct[j], yPct[i], p));
            return r;
        }

        private static List<PointPos> Point13(PointLayoutInput p)
        {
            // Same order as legacy recalculate_point13: 3 top + 2 mid-top + 3 mid + 2 mid-bottom + 3 bottom
            double H = p.H, V = p.V;
            var src = new (double xm, double ym, double xp, double yp)[]
            {
                (p.Amm,       p.Bmm,       p.Apct,         p.Bpct),         // 1
                (H / 2.0,     p.Bmm,       50,             p.Bpct),         // 2
                (H - p.Amm,   p.Bmm,       100 - p.Apct,   p.Bpct),         // 3
                (p.Cmm,       p.Dmm,       p.Cpct,         p.Dpct),         // 4
                (H - p.Cmm,   p.Dmm,       100 - p.Cpct,   p.Dpct),         // 5
                (p.Amm,       V / 2.0,     p.Apct,         50),             // 6
                (H / 2.0,     V / 2.0,     50,             50),             // 7
                (H - p.Amm,   V / 2.0,     100 - p.Apct,   50),             // 8
                (p.Cmm,       V - p.Dmm,   p.Cpct,         100 - p.Dpct),   // 9
                (H - p.Cmm,   V - p.Dmm,   100 - p.Cpct,   100 - p.Dpct),   // 10
                (p.Amm,       V - p.Bmm,   p.Apct,         100 - p.Bpct),   // 11
                (H / 2.0,     V - p.Bmm,   50,             100 - p.Bpct),   // 12
                (H - p.Amm,   V - p.Bmm,   100 - p.Apct,   100 - p.Bpct),   // 13
            };
            var r = new List<PointPos>(13);
            for (int i = 0; i < 13; i++)
                r.Add(Make(i + 1, src[i].xm, src[i].ym, src[i].xp, src[i].yp, p));
            return r;
        }

        /// <summary>
        /// 13-point diagonal ("X" + inner diamond): fixed 10% grid, independent of A/B/C/D margins.
        /// #1 center, #2-5 inner diamond, #6-11 main diagonal outer, #12-13 anti-diagonal outer.
        /// </summary>
        private static List<PointPos> Point13Diag(PointLayoutInput p)
        {
            double[] pct = new double[9];
            double[] mmX = new double[9];
            double[] mmY = new double[9];
            for (int k = 0; k < 9; k++)
            {
                pct[k] = (k + 1) * 10.0;
                mmX[k] = p.H * pct[k] / 100.0;
                mmY[k] = p.V * pct[k] / 100.0;
            }
            int[,] pts =
            {
                { 4, 4 }, { 2, 2 }, { 6, 2 }, { 2, 6 }, { 6, 6 },
                { 0, 0 }, { 1, 1 }, { 3, 3 }, { 5, 5 }, { 7, 7 },
                { 8, 8 }, { 8, 0 }, { 0, 8 },
            };
            var r = new List<PointPos>(13);
            for (int i = 0; i < 13; i++)
            {
                int c = pts[i, 0], rr = pts[i, 1];
                r.Add(new PointPos
                {
                    Id = i + 1,
                    XMm = mmX[c], YMm = mmY[rr],
                    XPct = pct[c], YPct = pct[rr],
                });
            }
            return r;
        }

        private static List<PointPos> Point15(PointLayoutInput p)
        {
            double H = p.H, V = p.V;
            var src = new (double xm, double ym, double xp, double yp)[]
            {
                (p.Amm,       p.Bmm,       p.Apct,       p.Bpct),       // 1
                (H - p.Amm,   p.Bmm,       100 - p.Apct, p.Bpct),       // 2
                (p.Cmm,       p.Dmm,       p.Cpct,       p.Dpct),       // 3
                (H / 2.0,     p.Dmm,       50,           p.Dpct),       // 4
                (H - p.Cmm,   p.Dmm,       100 - p.Cpct, p.Dpct),       // 5
                (p.Amm,       V / 2.0,     p.Apct,       50),           // 6
                (p.Cmm,       V / 2.0,     p.Cpct,       50),           // 7
                (H / 2.0,     V / 2.0,     50,           50),           // 8
                (H - p.Cmm,   V / 2.0,     100 - p.Cpct, 50),           // 9
                (H - p.Amm,   V / 2.0,     100 - p.Apct, 50),           // 10
                (p.Cmm,       V - p.Dmm,   p.Cpct,       100 - p.Dpct), // 11
                (H / 2.0,     V - p.Dmm,   50,           100 - p.Dpct), // 12
                (H - p.Cmm,   V - p.Dmm,   100 - p.Cpct, 100 - p.Dpct), // 13
                (p.Amm,       V - p.Bmm,   p.Apct,       100 - p.Bpct), // 14
                (H - p.Amm,   V - p.Bmm,   100 - p.Apct, 100 - p.Bpct), // 15
            };
            var r = new List<PointPos>(15);
            for (int i = 0; i < 15; i++)
                r.Add(Make(i + 1, src[i].xm, src[i].ym, src[i].xp, src[i].yp, p));
            return r;
        }

        private static List<PointPos> Point17(PointLayoutInput p)
        {
            double H = p.H, V = p.V;
            var src = new (double xm, double ym, double xp, double yp)[]
            {
                (p.Amm,       p.Bmm,       p.Apct,       p.Bpct),       // 1
                (H / 2.0,     p.Bmm,       50,           p.Bpct),       // 2
                (H - p.Amm,   p.Bmm,       100 - p.Apct, p.Bpct),       // 3
                (p.Cmm,       p.Dmm,       p.Cpct,       p.Dpct),       // 4
                (H / 2.0,     p.Dmm,       50,           p.Dpct),       // 5
                (H - p.Cmm,   p.Dmm,       100 - p.Cpct, p.Dpct),       // 6
                (p.Amm,       V / 2.0,     p.Apct,       50),           // 7
                (p.Cmm,       V / 2.0,     p.Cpct,       50),           // 8
                (H / 2.0,     V / 2.0,     50,           50),           // 9
                (H - p.Cmm,   V / 2.0,     100 - p.Cpct, 50),           // 10
                (H - p.Amm,   V / 2.0,     100 - p.Apct, 50),           // 11
                (p.Cmm,       V - p.Dmm,   p.Cpct,       100 - p.Dpct), // 12
                (H / 2.0,     V - p.Dmm,   50,           100 - p.Dpct), // 13
                (H - p.Cmm,   V - p.Dmm,   100 - p.Cpct, 100 - p.Dpct), // 14
                (p.Amm,       V - p.Bmm,   p.Apct,       100 - p.Bpct), // 15
                (H / 2.0,     V - p.Bmm,   50,           100 - p.Bpct), // 16
                (H - p.Amm,   V - p.Bmm,   100 - p.Apct, 100 - p.Bpct), // 17
            };
            var r = new List<PointPos>(17);
            for (int i = 0; i < 17; i++)
                r.Add(Make(i + 1, src[i].xm, src[i].ym, src[i].xp, src[i].yp, p));
            return r;
        }

        private static List<PointPos> Point17Array(PointLayoutInput p)
        {
            double H = p.H, V = p.V;
            // Helper: ith third-division between A and (H - A): i=1,2 → 1/3, 2/3 of (H - 2A)
            double XThird(int i) => i * (H - 2 * p.Amm) / 3.0 + p.Amm;
            double YThird(int i) => i * (V - 2 * p.Bmm) / 3.0 + p.Bmm;
            double XThirdPct(int i) => i * (100 - 2 * p.Apct) / 3.0 + p.Apct;
            double YThirdPct(int i) => i * (100 - 2 * p.Bpct) / 3.0 + p.Bpct;

            var src = new (double xm, double ym, double xp, double yp)[]
            {
                (p.Amm,         p.Bmm,         p.Apct,         p.Bpct),         // 1
                (XThird(1),     p.Bmm,         XThirdPct(1),   p.Bpct),         // 2
                (XThird(2),     p.Bmm,         XThirdPct(2),   p.Bpct),         // 3
                (H - p.Amm,     p.Bmm,         100 - p.Apct,   p.Bpct),         // 4
                (H - p.Amm,     YThird(1),     100 - p.Apct,   YThirdPct(1)),   // 5
                (XThird(2),     YThird(1),     XThirdPct(2),   YThirdPct(1)),   // 6
                (XThird(1),     YThird(1),     XThirdPct(1),   YThirdPct(1)),   // 7
                (p.Amm,         YThird(1),     p.Apct,         YThirdPct(1)),   // 8
                (H / 2.0,       V / 2.0,       50,             50),             // 9
                (p.Amm,         YThird(2),     p.Apct,         YThirdPct(2)),   // 10
                (XThird(1),     YThird(2),     XThirdPct(1),   YThirdPct(2)),   // 11
                (XThird(2),     YThird(2),     XThirdPct(2),   YThirdPct(2)),   // 12
                (H - p.Amm,     YThird(2),     100 - p.Apct,   YThirdPct(2)),   // 13
                (H - p.Amm,     V - p.Bmm,     100 - p.Apct,   100 - p.Bpct),   // 14
                (XThird(2),     V - p.Bmm,     XThirdPct(2),   100 - p.Bpct),   // 15
                (XThird(1),     V - p.Bmm,     XThirdPct(1),   100 - p.Bpct),   // 16
                (p.Amm,         V - p.Bmm,     p.Apct,         100 - p.Bpct),   // 17
            };
            var r = new List<PointPos>(17);
            for (int i = 0; i < 17; i++)
                r.Add(Make(i + 1, src[i].xm, src[i].ym, src[i].xp, src[i].yp, p));
            return r;
        }

        private static List<PointPos> Point25(PointLayoutInput p)
        {
            double H = p.H, V = p.V;
            double[] yMm = {
                p.Bmm,
                (V / 2.0 - p.Bmm) / 2.0 + p.Bmm,
                V / 2.0,
                (V / 2.0 - p.Bmm) / 2.0 + V / 2.0,
                V - p.Bmm,
            };
            double[] yPct = {
                p.Bpct,
                (50 - p.Bpct) / 2.0 + p.Bpct,
                50,
                (50 - p.Bpct) / 2.0 + 50,
                100 - p.Bpct,
            };
            double[] xMm = {
                p.Amm,
                (H / 2.0 - p.Amm) / 2.0 + p.Amm,
                H / 2.0,
                (H / 2.0 - p.Amm) / 2.0 + H / 2.0,
                H - p.Amm,
            };
            double[] xPct = {
                p.Apct,
                (50 - p.Apct) / 2.0 + p.Apct,
                50,
                (50 - p.Apct) / 2.0 + 50,
                100 - p.Apct,
            };

            var r = new List<PointPos>(25);
            int id = 1;
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    r.Add(Make(id++, xMm[j], yMm[i], xPct[j], yPct[i], p));
            return r;
        }

        private static List<PointPos> CustomGrid(PointLayoutInput p)
        {
            if (p.Rows <= 1 || p.Columns <= 1)
                return new List<PointPos>();

            double xOff = (p.H - p.Amm * 2) / (p.Columns - 1);
            double yOff = (p.V - p.Bmm * 2) / (p.Rows - 1);
            double xOffPct = (100 - p.Apct * 2) / (p.Columns - 1);
            double yOffPct = (100 - p.Bpct * 2) / (p.Rows - 1);

            var r = new List<PointPos>(p.Rows * p.Columns);
            int id = 1;
            for (int i = 0; i < p.Rows; i++)
                for (int j = 0; j < p.Columns; j++)
                {
                    double xm = j * xOff + p.Amm;
                    double ym = i * yOff + p.Bmm;
                    double xp = j * xOffPct + p.Apct;
                    double yp = i * yOffPct + p.Bpct;
                    r.Add(Make(id++, xm, ym, xp, yp, p));
                }
            return r;
        }
    }
}
