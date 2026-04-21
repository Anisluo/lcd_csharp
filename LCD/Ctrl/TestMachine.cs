//作为全局测试类
using LCD.Data;
using System;
using VisionCore;

namespace LCD.Ctrl
{

    //注意此处设置为接口类
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class TestMachine
    {
        protected ECom serialPort = null;
        /// <summary>
        /// 仪器命名-一般默认
        /// </summary>
        public virtual string Key { get; set; }

        public virtual string sLineBreak { get; set; } = "\r\n";

        public virtual bool IsOpen { get; set; } = false;


        //机器自检
        public virtual void AutoCheck()
        {


        }

        //初始化仪器
        public virtual void Init()
        {

        }

        //光谱测量
        public virtual IData MeasureSpectrum()
        {
            return null;
        }

        //lxy色坐标测量
        public virtual IData MeasureLxy()
        {
            return null;
        }

        //关闭仪器
        public virtual void Close()
        {

        }
        public virtual void StopTest()
        {

        }
        public virtual IData Measure()
        {
            return null;
        }

        public virtual bool Set(string key, string value)
        {
            return false;
        }

        internal ECom GetECom()
        {
            return serialPort;
        }
    }

    public enum ENUMMACHINE
    {
        BMA7,
        BM5A,
        PR655,
        CS2000,
        SR3A,
        BM5AS,
        USB2000,
        Demo,
        Admesy,
        SR5A,
		MS01
    }
}
