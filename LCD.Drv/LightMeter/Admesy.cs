using System;
using System.Collections.Generic;
using LCD.Data;
using VisionCore;

namespace LCD.Ctrl
{
    public class Admesy : TestMachine
    {
        private static Admesy admesy = null;

        private ADMESY_WRAPPER admes;

        public double[] value;
        public List<double> vlist;
        public RiseFallPositon positon;

        private uint sample_count = 100000;

        private Admesy()
        {
            admes = new ADMESY_WRAPPER();
            if (!admes.open_device())//开启admes设备
            {
                //mess.Show(admes.get_errorstring());
                Log.Error("Admesy打开失败："+ admes.get_errorstring());
            }
            else
            {
                Log.Info("Admesy打开成功");
            }
            if (!admes.check_dll_status())
            {
                //MessageBox.Show(admes.get_errorstring());
                Log.Error("Admesy库文件检查失败：" + admes.get_errorstring());
            }

            value = new double[sample_count];
            positon = new RiseFallPositon();
        }
        public static Admesy GetInstance()
        {
            if (admesy == null)
            {
                admesy = new Admesy();
            }
            return admesy;
        }

        public override void Init()
        {

        }

        public override void AutoCheck()
        {

        }

        //这里是测量啊
        public override IData Measure()
        {          
            //测量
            bool st = admes.read_sample(sample_count, 0, ref value, 4000);
            if(st==false)
            {
                Log.Error("读取数据失败:"+ admes.get_errorstring());
                return null;
            }
            //2次拟合
            List<double> slist = Smooth(value, 186); //Smooth(value, 50);            
            slist = Smooth(slist.ToArray(), 3000);
            vlist = slist;
            //接下来是求上升沿和下降沿时间啊
            double riseTime = 0.0;
            double fallTime = 0.0;
            positon.rise_start = -1;
            positon.fall_start = -1;
            admes.calculate_response(slist.ToArray(),slist.Count, 196000, 0.1, 0.9, ref riseTime, ref fallTime, ref positon);
            //绘图怎么搞啊
            //返回数据啊
            IData Result = new IData();
            /*
                ResultDatatemp.Columns.Add("Low");
                ResultDatatemp.Columns.Add("High");
                ResultDatatemp.Columns.Add("RiseTime");
                ResultDatatemp.Columns.Add("FallTime");
             */
            //Result.Low = Convert.ToInt32(datastrs[3]);
            //Result.High = Convert.ToInt32(datastrs[4]);
            Result.RiseTime =riseTime;
            Result.FallTime = fallTime;
            return Result;
        }

        public static List<double> Smooth(double[] input, int windowSize)
        {

            List<double> output = new List<double>();

            for (int i = 0; i <= input.Length - windowSize; i++)
            {
                double sum = 0;
                for (int j = i; j < i + windowSize; j++)
                    sum += input[j];

                double average = sum / windowSize;
                output.Add(average);
            }

            return output;
        }

        public override void Close()
        {
            //不需要关闭啊
        }

    }
}
