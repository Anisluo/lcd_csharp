using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.ViewMode
{
    public class PGViewsModel: ViewBase
    {
        private string nema1;

        public string Name1
        {
            get { return nema1; }
            set { nema1 = value; OnPropertyChanged(); }
        }
        private string nema2;

        public string Name2
        {
            get { return nema2; }
            set { nema2 = value; OnPropertyChanged(); }
        }
    }
}
