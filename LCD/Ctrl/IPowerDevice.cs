using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.Ctrl
{
    internal interface IPowerDevice
    {
        bool start(string portname);
        void stop();
        bool output(bool on_off);
        bool voltage_set(double val);
        bool current_set(double val);
        Result query();//查询当前电压和电流
    }
}
