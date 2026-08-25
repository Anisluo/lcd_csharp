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
    }
}
