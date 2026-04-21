using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.Data
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class LogConfigModel
    {
        public bool ShowDebug { get; set; } = true;
        public bool ShowComm { get; set; } = true;
    }
}
