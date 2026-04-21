using System;
using System.Windows.Controls;

namespace VisionCore
{
    /// <summary>
    /// NetBase.xaml 的交互逻辑
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class ComControl : UserControl
    {
        public ComControl()
        {
            InitializeComponent();
        }

        /// <summary>通信控件</summary>
        internal NetBaseModel mECom { get; set; }
    }
}
