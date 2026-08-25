using LCD.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.Ctrl
{
    [Category("仪器"), Description("DemoMachine"), DisplayName("DemoMachine")]
    internal class DemoMachine : TestMachine
    {
        public DemoMachine()
        {
        }

        public override void Init()
        {
            base.Init();
            IsOpen = true;
        }

        public override IData Measure()
        {
            if (IsOpen)
            {
                Random random = new Random();

                var X = 450 + random.NextDouble() * 10;
                var Y = 450 + random.NextDouble() * 10;
                var Z = 450 + random.NextDouble() * 10;
                return IData.CreateNew((float)X, (float)Y, (float)Z);
            }

            Project.ShowMessage(VisionCore.LogLevel.Info, "设备未连接");
            return null;
        }

        //色坐标测量（模拟）
        public override IData MeasureLxy()
        {
            var d = new IData();
            Random random = new Random();
            d.X = 100 + random.NextDouble() * 10;
            d.Y = 200 + random.NextDouble() * 10;
            d.Z = 150 + random.NextDouble() * 10;
            d.L = d.Y;
            d.Cx = d.X / (d.X + d.Y + d.Z);
            d.Cy = d.Y / (d.X + d.Y + d.Z);
            d.u = 4 * d.X / (d.X + 15 * d.Y + 3 * d.Z);
            d.v = 9 * d.Y / (d.X + 15 * d.Y + 3 * d.Z);
            d.CCT = 6500;
            return d;
        }

        //光谱测量（模拟 401 点，峰在 ~555nm）
        public override IData MeasureSpectrum()
        {
            var d = MeasureLxy();
            Random random = new Random();
            for (int i = 0; i < 401; i++)
            {
                double wl = 380 + i;
                d.SpectrumData[i] = 100.0 * Math.Exp(-Math.Pow((wl - 555) / 80.0, 2)) + random.NextDouble();
            }
            return d;
        }
    }
}
