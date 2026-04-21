using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.ViewMode
{
    public class PowerViewMode:ViewBase
    {
        /// <summary>
        /// 启用
        /// </summary>
        private bool ischeckbox;

        public bool IsCheckBox
        {
            get { return ischeckbox; }
            set { ischeckbox = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Com口
        /// </summary>
        private int  comame;

        public int comName
        {
            get { return comame; }
            set { comame = value;
                OnPropertyChanged();
            }   
        }

        private string comnametext;

        public string comNameText
        {
            get { return comnametext; }
            set { comnametext = value; OnPropertyChanged(); }
        }


        /// <summary>
        /// 波特率选项
        /// </summary>
        private int bardrate;

        public int bardRate
        {
            get { return bardrate; }
            set { bardrate = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 波特率
        /// </summary>
        private string bardratetext;

        public string bardRateText
        {
            get { return bardratetext; }
            set { bardratetext = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 数据位选项
        /// </summary>
        private int databit;

        public int dataBit
        {
            get { return databit; }
            set { databit = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 数据为
        /// </summary>
        private string  databittext;

        public string dataBitText
        {
            get { return databittext; }
            set { databittext = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 停止位选项
        /// </summary>
        private int stopbit;

        public int stopBit
        {
            get { return stopbit; }
            set { stopbit = value; OnPropertyChanged(); }
        }


        /// <summary>
        /// 停止位
        /// </summary>
        private string stopbittext;

        public string stopBitText
        {
            get { return stopbittext; }
            set { stopbittext = value; OnPropertyChanged(); }
        }


        private int parity;

        public int Parity
        {
            get { return parity; }
            set { parity = value; OnPropertyChanged(); }
        }

        private string paritytext;

        public string ParityText
        {
            get { return paritytext; }
            set { paritytext = value; OnPropertyChanged(); }
        }

        private string powerType;

        public string PowerType
        {
            get { return powerType; }
            set { powerType = value; OnPropertyChanged(); }
        }

        private string powerTypetext;

        public string PowerTypeText
        {
            get { return powerTypetext; }
            set { powerTypetext = value; OnPropertyChanged(); }
        }

        public bool enablePowerControl;
        public bool EnablePowerControl
        {
            get { return enablePowerControl; }
            set
            {
                enablePowerControl = value; OnPropertyChanged();
            }
        }

        public string powerSerialName;
        public string PowerSerialName
        {
            get { return powerSerialName; }
            set
            {
                powerSerialName = value; OnPropertyChanged();
            }
        }

    }
}
