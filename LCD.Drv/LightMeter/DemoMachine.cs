using System;
using System.ComponentModel;
using LCD.Data;
using VisionCore;

namespace LCD.Ctrl
{
    [Category("仪器"), Description("DemoMachine"), DisplayName("DemoMachine")]
    public class DemoMachine : TestMachine
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

            Log.Info("设备未连接");
            return null;
        }
    }
}
