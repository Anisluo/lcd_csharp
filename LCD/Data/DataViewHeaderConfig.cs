using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.Data
{
    /// <summary>
    /// 应当属于UI部分-UI部分的保存
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class DataViewHeaderConfig
    {
        public bool ShowX { get { return Xwidth != 0; } set { if (value) { XWidth = XWidthLast; } else { XWidthLast = XWidth; Xwidth = 0; } } }
        public bool ShowY { get { return YWidth != 0; } set { if (value) { YWidth = YWidthLast; } else { YWidthLast = YWidth; Ywidth = 0; } } }
        public bool ShowZ { get { return ZWidth != 0; } set { if (value) { ZWidth = ZWidthLast; } else { ZWidthLast = ZWidth; Zwidth = 0; } } }
        public bool ShowCx { get { return CxWidth != 0; } set { if (value) { CxWidth = CxWidthLast; } else { CxWidthLast = CxWidth; Cxwidth = 0; } } }
        public bool ShowCy { get { return CyWidth != 0; } set { if (value) { CyWidth = CyWidthLast; } else { CyWidthLast = CyWidth; Cywidth = 0; } } }
        public bool Showu { get { return uWidth != 0; } set { if (value) { uWidth = uWidthLast; } else { uWidthLast = uWidth; uwidth = 0; } } }
        public bool Showv { get { return vWidth != 0; } set { if (value) { vWidth = vWidthLast; } else { vWidthLast = vWidth; vwidth = 0; } } }
        public bool ShowCCT { get { return CCTWidth != 0; } set { if (value) { CCTWidth = CCTWidthLast; } else { CCTWidthLast = CCTWidth; CCTwidth = 0; } } }
        public bool ShowG { get { return GWidth != 0; } set { if (value) { GWidth = GWidthLast; } else { GWidthLast = GWidth; Gwidth = 0; } } }

        public double Xwidth { get; set; } = 50;
        public double XWidth
        {
            get
            { return Xwidth; }
            set
            {
                if (value >= 20)
                    Xwidth = value;
                else Xwidth = 20;
            }
        }

        public double Ywidth { get; set; } = 50;
        public double YWidth { get { return Ywidth; } set { if (value >= 20) Ywidth = value; else Ywidth = 20; } }

        public double Zwidth { get; set; } = 50;
        public double ZWidth { get { return Zwidth; } set { if (value >= 20) Zwidth = value; else Zwidth = 20; } }

        public double Cxwidth { get; set; } = 50;
        public double CxWidth { get { return Cxwidth; } set { if (value >= 20) Cxwidth = value; else Cxwidth = 20; } }

        public double Cywidth { get; set; } = 50;
        public double CyWidth { get { return Cywidth; } set { if (value >= 20) Cywidth = value; else Cywidth = 20; } }

        public double uwidth { get; set; } = 50;
        public double uWidth { get { return uwidth; } set { if (value >= 20) uwidth = value; else uwidth = 20; } }

        public double vwidth { get; set; } = 50;
        public double vWidth { get { return vwidth; } set { if (value >= 20) vwidth = value; else vwidth = 20; } }

        public double CCTwidth { get; set; } = 50;
        public double CCTWidth { get { return CCTwidth; } set { if (value >= 20) CCTwidth = value; else CCTwidth = 20; } }

        public double Gwidth { get; set; } = 50;
        public double GWidth { get { return Gwidth; } set { if (value >= 20) Gwidth = value; else Gwidth = 20; } }

        public double Lwidth { get; set; } = 50;
        public double LWidth { get { return Lwidth; } set { if (value >= 20) Lwidth = value; else Lwidth = 20; } }

        public double Awidth { get; set; } = 50;
        public double AWidth { get { return Awidth; } set { if (value >= 20) Awidth = value; else Awidth = 20; } }

        public double Bwidth { get; set; } = 50;
        public double BWidth { get { return Bwidth; } set { if (value >= 20) Bwidth = value; else Bwidth = 20; } }


        public double XWidthLast { get; set; } = 20;
        public double YWidthLast { get; set; } = 20;
        public double ZWidthLast { get; set; } = 20;
        public double CxWidthLast { get; set; } = 20;
        public double CyWidthLast { get; set; } = 20;
        public double uWidthLast { get; set; } = 20;
        public double vWidthLast { get; set; } = 20;
        public double CCTWidthLast { get; set; } = 20;
        public double GWidthLast { get; set; } = 20;
        public double LWidthLast { get; set; } = 20;
        public double AWidthLast { get; set; } = 20;
        public double BWidthLast { get; set; } = 20;
    }
}
