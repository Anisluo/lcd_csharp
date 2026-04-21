using System;

namespace LCD.Ctrl
{
    public static class LightMeterFactory
    {
        public static TestMachine Create(ENUMMACHINE machine)
        {
            switch (machine)
            {
                case ENUMMACHINE.BMA7: return BM7A.GetInstance();
                case ENUMMACHINE.CS2000: return CS2000.GetInstance();
                case ENUMMACHINE.SR3A:
                case ENUMMACHINE.SR5A: return SR3A.GetInstance();
                case ENUMMACHINE.MS01: return MS01.GetInstance();
                case ENUMMACHINE.Admesy: return Admesy.GetInstance();
                case ENUMMACHINE.Demo: return new DemoMachine();
                default: return null;
            }
        }
    }
}
