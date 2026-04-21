using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.Data
{
    /// <summary>
    /// 包含基础数据
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class IData
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
        public string TestName { get; set; }

        public int ID { get; set; }
        public double L { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Cx { get; set; }
        public double Cy { get; set; }
        public double u { get; set; }
        public double v { get; set; }
        public double Tc { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public double CCT { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double RiseTime { get; set; }
        public double FallTime { get; set; }
        public double CoordX { get; set; }
        public double CoordY { get; set; }
        public double CoordZ { get; set; }
        public double CoordU { get; set; }
        public double CoordV { get; set; }
        public double Lcolor { get; set; }
        public double Acolor { get; set; }
        public double Bcolor { get; set; }
        public double La { get; set; }
        public double Lb { get; set; }
        public double CT { get; set; }
        public int Point_Count { get; set; } //测量点数量
        public bool CT_done { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;

        public double[] SpectrumData { get; set; } = new double[401];

        internal static IData CreateNew(float X_, float Y_, float Z_)
        {
            IData result = new IData();
            result.X = X_;
            result.Y = Y_;
            result.Z = Z_;

            float Total = (X_ + Y_ + Z_);
            float Total2 = (X_ + 15 * Y_ + 3 * Z_);
            if (Total == 0)
            {
                return result;
            }
            result.Cx = X_ / Total;
            result.Cy = Y_ / Total;
            result.u = 4 * X_ / Total2;
            result.v = 9 * Y_ / Total2;

            var n = (result.Cx - 0.3320) / (0.1858 - result.Cy);
            result.CCT = (float)(437 * Math.Pow(n, 3) + 3601 * Math.Pow(n, 2) + n * 6831 + 5517);
            //var Optics = Helper_CIE.GetMainWaveLength(result.Cx, result.Cy, Manager_Config.p_ProjectConfig.Mcx, Manager_Config.p_ProjectConfig.Mcy);
            //result.MainWave = Optics.MainWaveLength;
            //result.CPurity = Optics.CPurity;

            //var LabData = new Luv() { L = Y_, U = result.u, V = result.v }.To<Lab>();
            //result.L = (float)LabData.L;
            //result.a = (float)LabData.A;
            //result.b = (float)LabData.B;
            return result;
        }
    }
}
