using System.Windows;
using LCD.Data;
using System.Collections.ObjectModel;

namespace LCD.View
{
    /// <summary>
    /// 设置显示那些结果
    /// </summary>
    public partial class CritView : Window
    {

        ObservableCollection<CrtModel> lstInfos { get; set; }
        //= new ObservableCollection<CrtModel>();
        public CritView()
        {
            InitializeComponent();

            Data2UI();
        }

        //选择所有
        private void OnBnClickedSelectALL(object sender, RoutedEventArgs e)
        {
            foreach (CrtModel cmo in lstInfos)
            {
                cmo.IsCHK = true;
            }
            mylist.ItemsSource = null;
            mylist.ItemsSource = lstInfos;

            //foreach(WrapPanel wrap in mylist.)
            //{
            //    CheckBox ch = (CheckBox)wrap.Children[0];
            //    ch.IsChecked = true;

            //}


        }

        //取消所有
        private void OnBnClickedUnSelectALL(object sender, RoutedEventArgs e)
        {

            foreach (CrtModel cmo in lstInfos)
            {
                cmo.IsCHK = false;
            }
            mylist.ItemsSource = null;
            mylist.ItemsSource = lstInfos;
        }


        public void Data2UI()
        {
            lstInfos = new ObservableCollection<CrtModel>();
            if (Project.cfg == null) { return; }
            if (Project.cfg.lstCrts == null) { return; }
            for (int i = 0; i < Project.cfg.lstCrts.Count; i++)
            {
                lstInfos.Add(new CrtModel()
                {
                    IsCHK = Project.cfg.lstCrts[i].IsCHK,
                    Name = Project.cfg.lstCrts[i].Name
                });
            }
            mylist.ItemsSource = lstInfos;
            mylist.DataContext = lstInfos;
            //this.DataContext = lstInfos;
        }

        /// <summary>
        /// 点击Min_Max选择1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedMimMax_1(object sender, RoutedEventArgs e)
        {

        }

        //点击Min_Max选择2
        private void OnBnClickedMinMax_2(object sender, RoutedEventArgs e)
        {

        }

        //点击Min_Max选择2
        private void OnBnClickedMinMax_3(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 执行确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedEnsure(object sender, RoutedEventArgs e)
        {
            //设置UI数据
            //if (Project.cfg != null)
            //{
            //    Project.cfg.lstCrts = new List<CritInfo>();
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "X(坐标:mm)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "Y(坐标:mm)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "Z(坐标:mm)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "U(坐标:mm)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "V(坐标:mm)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "Ball(坐标:mm)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "L(亮度)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "cx(色坐标)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "cy(色坐标)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "Tc(色温)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "u" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "v'" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "X(三刺激值)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "Y(三刺激值)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "Z(三刺激值)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "Voltage(电压)" });
            //    Project.cfg.lstCrts.Add(new Data.CritInfo() { IsCHK = true, Name = "波长(光谱)" });
            //    Project.SaveConfig("Config.xml");
            //}

            //临时保存数据
            this.Close();//执行关闭
        }
    }


    public class CrtModel
    {
        public bool IsCHK { get; set; }
        public string Name { get; set; }
    }

}
