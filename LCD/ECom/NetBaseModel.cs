using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    internal class NetBaseModel
    {
        public NetBaseModel(ECom ecom)
        {
            mECom = ecom;
        }

        /// <summary>通信控件</summary>
        protected ECom mECom { get; set; }

        /// <summary>启用通信</summary>
        public bool Enable
        {
            get { return mECom != null || mECom.IsConnected; }
            set
            {
                if (value)
                    mECom.Connect();
                else
                    mECom?.DisConnect();
            }
        }

        /// <summary>备注</summary>
        public string Remarks { get { return mECom.Remarks; } set { mECom.Remarks = value; } }

        /// <summary>通信名字</summary>
        public string Key { get { return mECom.Key; } set { mECom.Key = value; } }
    }
}
