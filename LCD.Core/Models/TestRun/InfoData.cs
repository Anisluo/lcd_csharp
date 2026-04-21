using System.Collections.Generic;
using LCD.Data;

namespace LCD.View
{
    /// <summary>
    /// 单个测试模板条目（点阵项、响应项等）的数据模型。
    /// 迁自 LCD/View/CustomView.xaml.cs（保留命名空间 LCD.View）。
    /// </summary>
    public class InfoData
    {
        public int id;
        public double height;

        public bool IsSelected;
        public ENUMMESSTYLE MESTYPE;
        public string Name;
        public string Name1;
        public List<string> lstdata;
        public double productLength;
        public double productWidth;
        public bool IsMeter;
        public double Ameter;
        public double Bmeter;
        public double Apercent;
        public double Bpercent;
        public double Cmeter;
        public double Dmeter;
        public double Cpercent;
        public double Dpercent;
        public int SerNo;
        public double Xmeter;
        public double Xpercent;
        public double Ymeter;
        public double Ypercent;
        public bool IsLchk;
        public double Lmin;
        public double Lmax;
        public bool Isxchk;
        public double xmin;
        public double xmax;
        public bool Isychk;
        public double ymin;
        public double ymax;
        public bool IsBalancechk;
        public double balancemin;
        public int warnR;
        public int warnG;
        public int warnB;
    }
}
