using LCD.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LCD.Ctrl;
using static LCD.View.CustomTemplate;
using Microsoft.Win32;
using static System.Net.WebRequestMethods;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Diagnostics.Contracts;
using NPOI.SS.Formula.Functions;
using System.Windows.Interop;
using static LCD.View.CustomView;
using SciChart.Charting.Common.Extensions;

namespace LCD.View
{
    /// <summary>
    /// 自定义模板界面
    /// </summary>
    public partial class CustomView : Window
    {
        private MovCtrl.EnumMoveSpeed RotateSpeed = MovCtrl.EnumMoveSpeed.SLOW;
        private MovCtrl.EnumMoveSpeed MovSpeed = MovCtrl.EnumMoveSpeed.SLOW;
        public static bool IsOK;
        //public List<Info> lst = new List<Info>();
        //DataTable table = new DataTable();
        Info info = new Info();//用于指示当前坐标


        List<InfoList> list = new List<InfoList>();// InfoList[] infoList = new InfoList[Project.PG.PatternList.Size];

        ENUMMESSTYLE MESTYPE = ENUMMESSTYLE._01_POINT;
        ObservableCollection<Info> lst = new ObservableCollection<Info>();
        Info info1 = new Info();

        public bool IsSelected { get; set; } = false;
        public CustomView()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();


            AddAxiesRoute();

            //this.DataContext = lst;

            Data2Info();
            list.Clear();

            info1.IsSelected = IsSelected;
            info1.Name = "Power";
            info1.MESTYPE = ENUMMESSTYLE.Power;
            info1.id = -1;

            //心跳函数
            System.Timers.Timer timer = new System.Timers.Timer();//声明timer对象
            timer.Interval = 150;//100ms刷新一次
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            timer.Start();
            if (Project.PG!=null)
            {
                for (int i = 0; i < Project.PG.PatternList.Count; i++)
                {
                    InfoList infoList = new InfoList();
                    infoList.Name2 = i.ToString();
                    infoList.Name1 = Project.PG.PatternList[i];
                    list.Add(infoList);
                    mylist1.ItemsSource = list;
                }
            }
           
           
            mylist.ItemsSource = lst;
            IsOK = false;
        }
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher?.Invoke(new Action(() =>
            {
                MovCtrl mvctrl = MovCtrl.GetInstance();
                if (mvctrl != null)
                {
                    double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
                    mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
                    Data2UI(dx, dy, dz, du, dv, dball);
                }
                else
                {
                    ShowWar();
                }
            }));
        }
        private void ShowWar()
        {
            XPos.Text = "Error: No Data";
            YPos.Text = "Error: No Data";
            ZPos.Text = "Error: No Data";
            UPos.Text = "Error: No Data";
            VPos.Text = "Error: No Data";
            BallPos.Text = "Error: No Data";
        }
        private void Data2UI(double dx, double dy, double dz, double du, double dv, double dball)
        {
            if (Project.cfg.ax_x == null | Project.cfg.ax_y == null | Project.cfg.ax_z == null | Project.cfg.ax_v == null | Project.cfg.ax_ball == null)
            {
                Project.WriteLog("参数加载失败");
                return;
            }
            if (Project.cfg.ax_x.IsEnable)
                XPos.Text = (dx - Project.Xorg).ToString("0.00") + " mm";
            else { XPos.Text = "----"; }

            if (Project.cfg.ax_y.IsEnable)
                YPos.Text = (dy - Project.Yorg).ToString("0.00") + " mm";
            else { YPos.Text = "----"; }

            if (Project.cfg.ax_z.IsEnable)
                ZPos.Text = (dz - Project.Zorg).ToString("0.00") + " mm";
            else { ZPos.Text = "----"; }

            if (Project.cfg.ax_u.IsEnable)
                UPos.Text = (du - Project.Uorg).ToString("0.00") + " °";
            else { UPos.Text = "----"; }

            if (Project.cfg.ax_v.IsEnable)
                VPos.Text = (dv - Project.Vorg).ToString("0.00") + " °";
            else { VPos.Text = "----"; }

            if (Project.cfg.ax_ball.IsEnable)
                BallPos.Text = (dball - Project.Ballorg).ToString("0.00") + " mm";
            else { BallPos.Text = "----"; }

        }
        //显示内容
        private void OnBnClickedSelectIndex(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// 轴路由事件
        /// </summary>
        private void AddAxiesRoute()
        {
            btnUP.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Up_MouseDown), true);
            btnUP.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Up_MouseUp), true);

            btnLeft.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Left_MouseDown), true);
            btnLeft.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Left_MouseUp), true);

            btnRight.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Right_MouseDown), true);
            btnRight.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Right_MouseUp), true);

            btnDown.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Down_MouseDown), true);
            btnDown.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Down_MouseUp), true);


            btnIn.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(In_MouseDown), true);
            btnIn.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(In_MouseUp), true);

            btnOut.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Out_MouseDown), true);
            btnOut.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Out_MouseUp), true);

            btnULeft.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(URotatLeft_MouseDown), true);
            btnULeft.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(URotatLeft_MouseUp), true);


            btnURight.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(URotatRight_MouseDown), true);
            btnURight.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(URotatRight_MouseUp), true);


            btnVLeft.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(VRotatLeft_MouseDown), true);
            btnVLeft.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(VRotatLeft_MouseUp), true);

            btnVRight.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(VRotatRight_MouseDown), true);
            btnVRight.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(VRotatRight_MouseUp), true);
            /***********************************************************************************************/
        }

        /// <summary>
        /// 设置Left_MouseUp位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VRotatRight_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveVAxiesUp(RotateSpeed);
        }

        private void VRotatRight_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_v.value);
        }
        private void VRotatLeft_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveVAxiesDown(RotateSpeed);
        }
        private void VRotatLeft_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_v.value);
        }
        private void URotatRight_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveUAxiesUp(RotateSpeed);
        }

        private void URotatRight_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_u.value);
        }

        private void URotatLeft_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveUAxiesDown(RotateSpeed);
        }

        private void URotatLeft_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_u.value);
        }
        private void Out_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveZAxiesUp(MovSpeed);
        }
        private void Out_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_z.value);
        }
        private void In_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveZAxiesDown(MovSpeed);
        }

        private void In_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_z.value);
        }

        /// <summary>
        /// Y轴向下运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Down_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveYAxiesUp(MovSpeed);
        }

        /// <summary>
        /// Y轴停止运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Down_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            if (Project.cfg.ax_y.IsSecondValue)
            {
                mvctrl.MoveStop2(Project.cfg.ax_y.value, Project.cfg.ax_y.secondvalue);
            }
            else
            {
                mvctrl.MoveStop(Project.cfg.ax_y.value);
            }
        }

        //向左
        private void Right_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveXAxisUp(MovSpeed);
        }

        private void Right_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_x.value);
        }

        //向右
        private void Left_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveXAxisDown(MovSpeed);
        }

        //停止
        private void Left_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveStop(Project.cfg.ax_x.value);
        }

        /// <summary>
        /// Y轴向上运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Up_MouseDown(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            mvctrl.MoveYAxiesDown(MovSpeed);

        }

        /// <summary>
        /// Y轴停止运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Up_MouseUp(object sender, RoutedEventArgs e)
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            if (Project.cfg.ax_y.IsSecondValue)
            {
                mvctrl.MoveStop2(Project.cfg.ax_y.value, Project.cfg.ax_y.secondvalue);
            }
            else
            {
                mvctrl.MoveStop(Project.cfg.ax_y.value);
            }
        }

        //设置Info转为dataa数据
        private List<InfoData> Info2Data()
        {
            //这里要改
            List<InfoData> lstDatas = new List<InfoData>();

            for (int i = 0; i < lst.Count; i++)
            {
                //名字也有可能变化啊，
                InfoData infoData = Project.lstInfos.FirstOrDefault(p => p.id == lst[i].id);
                if(infoData == null)
                {
                    //为null是新建的
                    //if (lst[i].id <0)
                    //{
                    //    //小于0的话是系统创建的，临时的，不添加
                    //    continue;
                    //}                    
                }
                InfoData ifdata = new InfoData();
                //模版的高度可能是不同的，不能这样设置成一样的
                ifdata.height = lst[i].height;
                //ifdata.height = Double.Parse(ProductHeight.Text.Trim());
                ifdata.IsSelected = lst[i].IsSelected;
                ifdata.Name = lst[i].Name;
                ifdata.MESTYPE=lst[i].MESTYPE;                
                ifdata.lstdata = new List<string>();
                if (lst[i].table != null)
                {
                    string strheader = "";
                    for (int k = 0; k < lst[i].table.Columns.Count; k++)
                    {
                        strheader += lst[i].table.Columns[k].ColumnName + ",";
                    }
                    ifdata.lstdata.Add(strheader);
                    
                    for (int j = 0; j < lst[i].table.Rows.Count; j++)
                    {
                        DataRow dr = lst[i].table.Rows[j];

                        string str = "";
                        for (int k = 0; k < dr.ItemArray.Length; k++)
                        {
                            str += (dr.ItemArray[k].ToString()) + ',';
                        }
                        ifdata.lstdata.Add(str);                        
                    }
                    
                }

                //刚添加的没有infoData啊
                if (infoData != null)
                {
                    //填充数据
                    ifdata.id = infoData.id;//id一定要拷贝过来啊
                    ifdata.Isxchk = infoData.Isxchk;
                    ifdata.Isychk = infoData.Isychk;
                    ifdata.IsLchk = infoData.IsLchk;
                    ifdata.Lmax = infoData.Lmax;
                    ifdata.Lmin = infoData.Lmin;
                    ifdata.xmax = infoData.xmax;
                    ifdata.xmin = infoData.xmin;
                    ifdata.ymax = infoData.ymax;
                    ifdata.ymin = infoData.ymin;
                    ifdata.IsBalancechk = infoData.IsBalancechk;
                    ifdata.balancemin = infoData.balancemin;
                    ifdata.warnR = infoData.warnR;
                    ifdata.warnG = infoData.warnG;
                    ifdata.warnB = infoData.warnB;

                    ifdata.productLength = infoData.productLength;
                    ifdata.productWidth = infoData.productWidth ;
                    ifdata.IsMeter = infoData.IsMeter ;
                    ifdata.Ameter =  infoData.Ameter ;
                    ifdata.Bmeter = infoData.Bmeter ;
                    ifdata.Apercent =  infoData.Apercent;
                    ifdata.Bpercent = infoData.Bpercent ;
                    ifdata.Cmeter = infoData.Cmeter ;
                    ifdata.Dmeter =infoData.Dmeter ;
                    ifdata.Cpercent = infoData.Cpercent ;
                    ifdata.Dpercent = infoData.Dpercent;                    
                }
                else
                {
                    //更新id序号
                    int idmax = 0;
                    idmax = lst.Select(p => p.id).Max();
                    ifdata.id = idmax + 1;
                    //其他参数是默认值啊
                }
                lstDatas.Add(ifdata);
            }
            return lstDatas;
        }

        private void CheckBox_MouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            CustomMessage cstom = new CustomMessage("修改名称");
            cstom.ShowDialog();
        }

        //数据转为Infos
        private void Data2Info()
        {
            lst.Clear();
            //lst.Add(info1);
            double Height = 0;
            try
            {
                Height = Project.lstInfos[0].height;
            }
            catch (Exception e)
            {
                Project.WriteLog(e.Message);
            }
            

            ProductHeight.Text= Height.ToString();
            for (int i = 0; i < Project.lstInfos.Count; i++)
            {
                if (Project.lstInfos[i].MESTYPE == ENUMMESSTYLE.Power)
                {
                    IsSelected = Project.lstInfos[i].IsSelected;
                    //continue;
                }

                Info inf = new Info();
                inf.height = Project.lstInfos[i].height;
                inf.IsSelected = Project.lstInfos[i].IsSelected;
                inf.Name = Project.lstInfos[i].Name;
                inf.MESTYPE = Project.lstInfos[i].MESTYPE;
                //序号更新一下,防止一直增加
                inf.id = i + 1;
                Project.lstInfos[i].id = i + 1;
                update_type(inf);
                Lst2Table(Project.lstInfos[i].lstdata, ref inf.table);
                lst.Add(inf);

            }
        }

        private void update_type(Info inf)
        {
            if (inf.MESTYPE == ENUMMESSTYLE._01_POINT)
            {
                inf.Type = "点位";
            }
            else if (inf.MESTYPE == ENUMMESSTYLE._02_RESPONSE)
            {
                inf.Type = "响应";
            }
            else if (inf.MESTYPE == ENUMMESSTYLE._03_SPECTRUM)
            {
                inf.Type = "光谱";
            }
            else if (inf.MESTYPE == ENUMMESSTYLE._04_FLICKER)
            {
                inf.Type = "FLICKER";
            }
            else if (inf.MESTYPE == ENUMMESSTYLE._05_CROSSTALK)
            {
                inf.Type = "串扰";
            }
            else if (inf.MESTYPE == ENUMMESSTYLE._06_ACR)
            {
                inf.Type = "ACR";
            }
            else if (inf.MESTYPE == ENUMMESSTYLE._07_warmup)
            {
                inf.Type = "Warmup";
            }
            else if (inf.MESTYPE == ENUMMESSTYLE.Power)
            {
                inf.Type = "Power";
            }
            else
            {
                inf.Type = "None";
            }
        }

        private void Lst2Table(List<string> lstdata, ref DataTable dt)
        {
            if(lstdata == null)
            {
                return;
            }
            if (lstdata.Count != 0)
            {
                //if (dt == null)
                //{
                //    return;
                //}
                if (dt != null) { dt.Dispose(); }
                dt = new DataTable();

                string[] strheader = lstdata[0].Split(',');
                for (int i = 0; i < strheader.Length; i++)
                {
                    if (strheader[i] != "") { dt.Columns.Add(strheader[i]); }
                }
                DataView dv = new DataView(dt);
                ///定义数据表
                mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
                mydata.VerticalGridLinesBrush = Brushes.Gray;
                //mydata.HorizontalAlignment = HorizontalAlignment.Center;
                mydata.MinColumnWidth = 70;

                mydata.CanUserAddRows = false;
                mydata.CanUserDeleteRows = false;
                mydata.ItemsSource = dv;

                for (int i = 1; i < lstdata.Count; i++)
                {
                    string[] strs = lstdata[i].Split(',');
                    DataRow dr = dt.NewRow();

                    for (int k = 0; k < strheader.Length; k++)
                    {
                        if (strs[k] != "") { dr[strheader[k]] = strs[k]; }
                    }
                    dt.Rows.Add(dr);
                }
                //初始化三维坐标空间
            }
        }

        //重新命名
        private void OnBnClickedReName(object sender, RoutedEventArgs e)
        {
            int n = mylist.SelectedIndex;
            if(n < 0)
            {
                return;
            }
            Info info = (Info)mylist.SelectedItem;
            string text = info.Name;
            string info1 = "请输入新名称:";
            if (Project.cfg.Lang != 0)
            {
                info1 = "Please input new name:";
            }
            pop_up barCode = new pop_up(text, info1);
            //需要自动开始测试啊
            barCode.ShowDialog();
            if (barCode.is_ok == false)
            {
                return;
            }
            //获取新的名称
            string new_name = barCode.Time;
            if(new_name == text)
            {
                //没有变化啊
                return;
            }
            //更新一下
            lst[n].Name= new_name;
            mylist.ItemsSource = null;
            mylist.ItemsSource = lst;
        }

        private void OnBnClickedDelete(object sender, RoutedEventArgs e)
        {           
            int count = lst.Count(p => p.IsSelected == true);
            if(count >1)
            {
                MessageBox.Show("一次只能删除一个模版");
                return;
            }
            if(count <= 0)
            {
                MessageBox.Show("模版没有选中，不能删除");
                return;
            }
            //查找选中的元素
            var selected = lst.FirstOrDefault(p =>p.IsSelected==true);
            int n = lst.IndexOf(selected);

            //在这里确认一下
            string msg = "确定要删除吗？";
            string title = "提示";
            if (Project.cfg.Lang != 0)
            {
                msg = "Confirm to delete ?";
                title = "Notice";
            }
            MessageBoxResult result = MessageBox.Show(msg, title, MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            if ((n >= 0)&&(n<lst.Count))
            {
                lst.RemoveAt(n);
                mylist.ItemsSource = null;
                mylist.ItemsSource = lst;
                mylist.InvalidateVisual();
            }
        }


        /// <summary>
        /// 自动增加模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedAddTemplate(object sender, RoutedEventArgs e)
        {
            string filename = comboBoxType.Text + DateTime.Now.ToString("yyyyMMddHHmmss");

            string info = "请输入名称：";
            if (Project.cfg.Lang != 0)
            {
                info = "Please input template name:";
            }
            pop_up pop_Up = new pop_up(filename,info);
            pop_Up.ShowDialog();

            String Temp = pop_Up.Time;
        

            ENUMMESSTYLE MESTYPE = ENUMMESSTYLE._01_POINT;

            if (this.comboBoxType.Text.IndexOf("01")!=-1){MESTYPE = ENUMMESSTYLE._01_POINT;}
            else if (this.comboBoxType.Text.IndexOf("02") != -1) { MESTYPE = ENUMMESSTYLE._02_RESPONSE; }
            else if (this.comboBoxType.Text.IndexOf("03")!=-1){ MESTYPE = ENUMMESSTYLE._03_SPECTRUM; }
            else if(this.comboBoxType.Text.IndexOf("04")!=-1) { MESTYPE = ENUMMESSTYLE._04_FLICKER; }
            else if (this.comboBoxType.Text.IndexOf("05") != -1) { MESTYPE = ENUMMESSTYLE._05_CROSSTALK; }
            else if (this.comboBoxType.Text.IndexOf("06") != -1){ MESTYPE = ENUMMESSTYLE._06_ACR; }
            else if (this.comboBoxType.Text.IndexOf("7")!=-1) { MESTYPE = ENUMMESSTYLE._07_warmup; }

            int id = 0;
            if(Project.lstInfos.Count ==0)
            {
                id = 1;
            }
            else
            {
                id = Project.lstInfos.Max(p => p.id);
                id++;
            }
            Info ifo = new Info();
            {
                ifo.IsSelected = true;
                ifo.Name = Temp;
                ifo.MESTYPE = MESTYPE;
                ifo.id = id;
            }
            update_type(ifo);

            DataTemplate sDataTemplate= mylist.ItemTemplate;
            ItemCollection sCollection= mylist.Items;


            lst.Add(ifo);
            int n = lst.Count-1;

            

            //DataTemplate zDataTemplate = mylist.ItemTemplate;

            //mylist.SelectedIndex = n;
            this.mylist.SelectedIndex = n;
           
            //mydata.Columns[0].IsReadOnly = true;
            //mydata.CanUserAddRows = false;
            //}
            //private void    OnBnClickedDeleIndex(){
            //        IsSelected = true,
            //        Name = filename,
            //        MESTYPE = ENUMMESSTYLE._01_POINT
            //    };
            //    int n = lst.Count;
            //    mylist.SelectedIndex = n;
            //    lst.Add(ifo);//太你家info 
            //    mydata.Columns[0].IsReadOnly = true;
            //    mydata.CanUserAddRows = false;
        }

        //添加index指示
        private void OnBnClickedAddIndex(object sender, RoutedEventArgs e)
        {
            int n = mylist.SelectedIndex;
            if (n >= lst.Count) { return; }
            if(n <0) { return; }
            Info info = lst[n];

            int cnt = mydata.SelectedIndex;
            if (cnt < 0)
            {
                if (info.table != null) { cnt = info.table.Rows.Count; }
                else { cnt = 0; }
            }

            if (info.table == null || cnt >= info.table.Rows.Count)
            {
                info.table.Rows.Add(new object[] { });
            }
            else if (cnt < info.table.Rows.Count)
            {
                DataRow dr = info.table.NewRow();
                info.table.Rows.InsertAt(dr, cnt + 1);

            }
            else { }
        }





        //另存为
        private void OnBnClickedSaveAs(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Filter = "XML files (*.xml)|*.xml|Text files (*.txt)|*.txt";//(*.txt)|*.txt|
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //保存当前数据
                List<InfoData> lstInfos = Info2Data();
                Project.SaveTemplateGroup(lstInfos, saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// 显示模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedShowTemplate(object sender, RoutedEventArgs e)
        {

        }

        public class InfoList : ViewBase
        {
            private string nema1;

            public string Name1
            {
                get { return nema1; }
                set { nema1 = value;OnPropertyChanged(); }
            }
            private string nema2;

            public string Name2
            {
                get { return nema2; }
                set { nema2 = value; OnPropertyChanged(); }
            }
        }
        public class Info: ViewBase
        {
            public double height { get; set; }
            public int id { get; set; }

            private bool isselected;
            public bool IsSelected {
                get { return isselected; }
                set
                {
                    isselected = value;
                    OnPropertyChanged();
                } }


            private string name;
            public string Name
            {
                get { return name;}
                set
                {
                    name = value;
                    OnPropertyChanged();
                }
            }

            //加一列类型
            private string type;
            public string Type
            {
                get { return type; }
                set
                {
                    type = value;
                    OnPropertyChanged();
                }
            }

            public ENUMMESSTYLE MESTYPE;
            

           

            public DataTable table;


        }

        private void mylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Info info = (Info)mylist.SelectedItem;
            InitDataGrid(info);           

            if (info != null)
            {
                ProductHeight.Text = info.height.ToString();// infoData.height.ToString();
                //显示厚度数据
                InfoData infoData = Project.lstInfos.FirstOrDefault(p => p.id == info.id);
                if (infoData != null)
                {
                    //右邊show測量標準
                    if (infoData.Isxchk)
                    {
                        Xcheck.IsChecked = true;
                        xmax.Text = infoData.xmax.ToString();
                        xmin.Text = infoData.xmin.ToString();
                    }
                    else
                    {
                        Xcheck.IsChecked = false;
                        xmax.Text = "";
                        xmin.Text = "";
                    }
                    if (infoData.Isychk)
                    {
                        Ycheck.IsChecked = true;
                        ymax.Text = infoData.ymax.ToString();
                        ymin.Text = infoData.ymin.ToString();
                    }
                    else
                    {
                        Ycheck.IsChecked = false;
                        ymax.Text = "";
                        ymin.Text = "";
                    }
                    if (infoData.IsLchk)
                    {
                        Lcheck.IsChecked = true;
                        Lmax.Text = infoData.Lmax.ToString();
                        Lmin.Text = infoData.Lmin.ToString();
                    }
                    else
                    {
                        Lcheck.IsChecked = false;
                        Lmax.Text = "";
                        Lmin.Text = "";
                    }
                    if (infoData.IsBalancechk)
                    {
                        Balancechk.IsChecked = true;
                        balancemin.Text = infoData.balancemin.ToString();
                    }
                    else
                    {
                        Balancechk.IsChecked = false;
                        balancemin.Text = "";
                    }
                    //设置颜色
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(infoData.warnR, infoData.warnG, infoData.warnB);
                    update_warn_color(color);
                }
            }
            else
            {
                //没有使用默认的
                Xcheck.IsChecked = false;
                xmax.Text = "";
                xmin.Text = "";
                Ycheck.IsChecked = false;
                ymax.Text = "";
                ymin.Text = "";
                Lcheck.IsChecked = false;
                Lmax.Text = "";
                Lmin.Text = "";
                Balancechk.IsChecked = false;
                balancemin.Text = "";
                System.Drawing.Color color = System.Drawing.Color.FromArgb(0, 0, 0);
                update_warn_color(color);
            }
        }

        private void update_warn_color(System.Drawing.Color selectedColor)
        {
            // 将所选颜色转换为WPF的颜色结构
            System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);
            // 将颜色应用到WPF控件上
            // 例如，可以将颜色应用到一个Rectangle控件的Fill属性
            RecColor.Fill = new System.Windows.Media.SolidColorBrush(wpfColor);
        }
        /// <summary>
        /// 初始化控件列表
        /// </summary>
        private void InitDataGrid(Info info)
        {
            switch (MESTYPE)
            {
                case ENUMMESSTYLE._01_POINT: InitDataGrid_POINT(info); break;
                case ENUMMESSTYLE._02_RESPONSE: InitDataGrid_RESPONSE(info); break;
                case ENUMMESSTYLE._03_SPECTRUM: InitDataGrid_SPECTRUM(info); break;
                case ENUMMESSTYLE._04_FLICKER: InitDataGrid_FLICKER(info); break;
                case ENUMMESSTYLE._05_CROSSTALK: InitDataGrid_CROSSTALK(info); break;
                case ENUMMESSTYLE._06_ACR: InitDataGrid_ACR(info); break;
                case ENUMMESSTYLE._07_warmup: InitDataGrid_warmup(info); break;
            }
        }

        // 动态添加下拉列到DataGrid
        public void AddComboBoxColumn(string header, List<string> itemsSource)
        {
            var comboBoxColumn = new DataGridTemplateColumn
            {
                Header = header
            };

            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(ComboBox));
            factory.SetValue(ComboBox.ItemsSourceProperty, itemsSource);
            //factory.SetValue(ComboBox.SelectedItemProperty, BindingOperations.NewDataBinding(factory, ComboBox.SelectedItemProperty, comboBoxColumn, DataGridTemplateColumn.CellEditingBindingProperty));
            //factory.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(ComboBox_SelectionChanged));

            DataTemplate dataTemplate = new DataTemplate();
            dataTemplate.VisualTree = factory;

            comboBoxColumn.CellTemplate = dataTemplate;
            comboBoxColumn.CellEditingTemplate = dataTemplate;

            mydata.Columns.Add(comboBoxColumn);
        }

        private void InitDataGrid_POINT(Info info)
        {
            if (info == null) { return; }
            if (info.table == null)
            {
                info.table = new DataTable();
                info.table.Columns.Add("ID");
                info.table.Columns.Add("X(mm)");
                info.table.Columns.Add("Y(mm)");
                info.table.Columns.Add("Z(mm)");
                info.table.Columns.Add("U(°)");
                info.table.Columns.Add("V(°)");
                info.table.Columns.Add("PG(序号)");
                info.table.Columns.Add("PG(提示信息)");
            }
            DataView dv = new DataView(info.table);
            ///定义数据表
            mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            mydata.VerticalGridLinesBrush = Brushes.Gray;
            mydata.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly =true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            mydata.MinColumnWidth = 70;
            mydata.ItemsSource = dv;
            //测试添加下拉框列，确实可以添加下拉框的啊
            //List<string> itemsSource = new List<string> { "是", "否" };
            //AddComboBoxColumn("暂停",itemsSource);
        }
        private void InitDataGrid_warmup(Info info)
        {
            if (info == null) { return; }
            if (info.table == null)
            {
                info.table = new DataTable();
                info.table.Columns.Add("ID");
                info.table.Columns.Add("X(mm)");
                info.table.Columns.Add("Y(mm)");
                info.table.Columns.Add("Z(mm)");
                info.table.Columns.Add("U(°)");
                info.table.Columns.Add("V(°)");
                info.table.Columns.Add("PG(序号)");
                info.table.Columns.Add("PG(提示信息)");
                info.table.Columns.Add("时长(s)");
                info.table.Columns.Add("间隔(s)");
            }
            DataView dv = new DataView(info.table);
            ///定义数据表
            mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            mydata.VerticalGridLinesBrush = Brushes.Gray;
            mydata.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly =true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            mydata.MinColumnWidth = 70;
            mydata.ItemsSource = dv;
        }
        private void InitDataGrid_RESPONSE(Info info)
        {
            if(info==null)
            {
                return;
            }
            if (info.table == null)
            {
                info.table = new DataTable();
                info.table.Columns.Add("ID");
                info.table.Columns.Add("X(mm)");
                info.table.Columns.Add("Y(mm)");
                info.table.Columns.Add("Z(mm)");
                info.table.Columns.Add("Low(灰阶)");
                info.table.Columns.Add("High(灰阶)");
            }
            DataView dv = new DataView(info.table);
            ///定义数据表
            mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            mydata.VerticalGridLinesBrush = Brushes.Gray;
            mydata.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly = true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            mydata.MinColumnWidth = 70;
            mydata.ItemsSource = dv;
        }


        private void InitDataGrid_SPECTRUM(Info info)
        {
            if(info ==null)
            {
                return;
            }
            if (info.table == null)
            {
                info.table = new DataTable();
                info.table.Columns.Add("ID");
                info.table.Columns.Add("X(mm)");
                info.table.Columns.Add("Y(mm)");
                info.table.Columns.Add("Z(mm)");
                info.table.Columns.Add("U(°)");
                info.table.Columns.Add("V(°)");
                info.table.Columns.Add("PG(序号)");
                info.table.Columns.Add("PG(提示信息)");
            }
            DataView dv = new DataView(info.table);
            ///定义数据表
            mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            mydata.VerticalGridLinesBrush = Brushes.Gray;
            mydata.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly = true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            mydata.MinColumnWidth = 70;
            mydata.ItemsSource = dv;
        }


        private void InitDataGrid_ACR(Info info)
        {
            if (info.table == null)
            {
                info.table = new DataTable();
                info.table.Columns.Add("ID");
                info.table.Columns.Add("X(mm)");
                info.table.Columns.Add("Y(mm)");
                info.table.Columns.Add("Z(mm)");
                info.table.Columns.Add("U(°)");
                info.table.Columns.Add("V(°)");
                info.table.Columns.Add("PG(序号)");
                info.table.Columns.Add("PG(提示信息)");
            }
            DataView dv = new DataView(info.table);
            ///定义数据表
            mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            mydata.VerticalGridLinesBrush = Brushes.Gray;
            mydata.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly = true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            mydata.MinColumnWidth = 70;
            mydata.ItemsSource = dv;
        }

        private void InitDataGrid_FLICKER(Info info)
        {
            if (info.table == null)
            {
                info.table = new DataTable();
                info.table.Columns.Add("ID");
                info.table.Columns.Add("X(mm)");
                info.table.Columns.Add("Y(mm)");
                info.table.Columns.Add("Z(mm)");
                info.table.Columns.Add("U(°)");
                info.table.Columns.Add("V(°)");
                info.table.Columns.Add("PG(序号)");
                info.table.Columns.Add("PG(提示信息)");
            }
            DataView dv = new DataView(info.table);
            ///定义数据表
            mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            mydata.VerticalGridLinesBrush = Brushes.Gray;
            mydata.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly = true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            mydata.MinColumnWidth = 70;
            mydata.ItemsSource = dv;
        }


        private void InitDataGrid_CROSSTALK(Info info)
        {
            if (info.table == null)
            {
                info.table = new DataTable();
                info.table.Columns.Add("ID");
                info.table.Columns.Add("X(mm)");
                info.table.Columns.Add("Y(mm)");
                info.table.Columns.Add("Z(mm)");
                info.table.Columns.Add("U(°)");
                info.table.Columns.Add("V(°)");
                info.table.Columns.Add("PG(序号)");
                info.table.Columns.Add("PG(提示信息)");
            }
            DataView dv = new DataView(info.table);
            ///定义数据表
            mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            mydata.VerticalGridLinesBrush = Brushes.Gray;
            mydata.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly = true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            mydata.MinColumnWidth = 70;
            mydata.ItemsSource = dv;
        }

        private void OnBnClickedEnsure(object sender, RoutedEventArgs e)
        {
            string title = "是否使用当前模板组进行测试？";
            if (Project.cfg.Lang != 0)
            {
                title = "Do you want to use the current template group for testing?";
            }
            CustomMessage cstom = new CustomMessage(title);
            cstom.ShowDialog();
            if (CustomMessage.IsOK)
            {
                Project.WriteLog("载入当前模板组");
                Project.lstInfos = Info2Data();//获取当前模板组内容
                Project.SaveTemplate("Template.xml");
                IsOK = true;
                this.Close();
            }
        }

        private void OnBnClickedAutoCreate(object sender, RoutedEventArgs e)
        {
            int n = mylist.SelectedIndex;
            if (n >= lst.Count || n < 0) { return; }
            Info info = lst[n];

            //这里要获取原来的信息，根据名字来搜索
            InfoData infoData = Project.lstInfos.FirstOrDefault(p => p.id == info.id);
            if(infoData == null)
            {
                //为null表示新建的，这个时候也需要新建一个InfoData对象
                infoData = new InfoData();
                infoData.id = info.id;
                Project.lstInfos.Add(infoData);//加到队列里面啊，后面才能搜索到
            }
            if (infoData.MESTYPE == ENUMMESSTYLE._05_CROSSTALK)
            {
                //CustomTemplateCrosstalk cstm = new CustomTemplateCrosstalk(info, infoData);
                //cstm.ptmodel.tempName = info.Name;
                //CustomTemplate.dt = info.table;
                ////定义默认参数
                //cstm.ShowDialog();
                //info.table = !CustomTemplate.IsEnsure ? info.table : CustomTemplate.dt;
            }
           
            CustomTemplate cstm = new CustomTemplate(info, infoData);
            cstm.ptmodel.tempName = info.Name;
            CustomTemplate.dt = info.table;
            //定义默认参数
            cstm.ShowDialog();
            info.table = !CustomTemplate.IsEnsure ? info.table : CustomTemplate.dt;
            
            if(CustomTemplate.IsEnsure)
            {
                //更新判断数据啊
                infoData.Isxchk = cstm.ptmodel.Isxchk;
                infoData.Isychk =cstm.ptmodel.Isychk;
                infoData.IsLchk =cstm.ptmodel.IsLchk ;
                infoData.Lmax = cstm.ptmodel.Lmax;
                infoData.Lmin = cstm.ptmodel.Lmin;
                infoData.xmax = cstm.ptmodel.xmax;
                infoData.xmin = cstm.ptmodel.xmin;
                infoData.ymax = cstm.ptmodel.ymax;
                infoData.ymin = cstm.ptmodel.ymin;
                infoData.IsBalancechk = cstm.ptmodel.IsBalancechk ;
                infoData.balancemin = cstm.ptmodel.balancemin;
                infoData.warnR = cstm.ptmodel.color.R;
                infoData.warnG = cstm.ptmodel.color.G;
                infoData.warnB = cstm.ptmodel.color.B;
                //更新產品數據
                infoData.productLength = cstm.ptmodel.productLength;
                infoData.productWidth = cstm.ptmodel.productWidth;
                infoData.IsMeter = cstm.ptmodel.IsMeter ;
                infoData.Ameter = cstm.ptmodel.Ameter ;
                infoData.Bmeter = cstm.ptmodel.Bmeter ;
                infoData.Apercent = cstm.ptmodel.Apercent ;
                infoData.Bpercent = cstm.ptmodel.Bpercent ;
                infoData.Cmeter = cstm.ptmodel.Cmeter ;
                infoData.Dmeter = cstm.ptmodel.Dmeter ;
                infoData.Cpercent = cstm.ptmodel.Cpercent ;
                infoData.Dpercent = cstm.ptmodel.Dpercent ;
            }

            DataView dv = new DataView(info.table);
            ///定义数据表
            mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            mydata.VerticalGridLinesBrush = Brushes.Gray;
            mydata.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly = true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            mydata.MinColumnWidth = 70;
            mydata.ItemsSource = dv;
        }

        private void ShowData(double x, double px, double y, double py)
        {


        }

        private void comboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBoxType.SelectedIndex)
            {
                case 0: MESTYPE = ENUMMESSTYLE._01_POINT; break;
                case 1: MESTYPE = ENUMMESSTYLE._02_RESPONSE; break;
                case 2: MESTYPE = ENUMMESSTYLE._03_SPECTRUM; break;
                case 3: MESTYPE = ENUMMESSTYLE._05_CROSSTALK; break;
                case 4: MESTYPE = ENUMMESSTYLE._07_warmup; break;
                //case 5: MESTYPE = ENUMMESSTYLE._06_ACR; break;
                //case 6: MESTYPE = ENUMMESSTYLE._07_warmup; break;
            }

            if (AutoCreate != null)
            {
                if (MESTYPE == ENUMMESSTYLE._01_POINT || MESTYPE == ENUMMESSTYLE._07_warmup||
                    MESTYPE == ENUMMESSTYLE._03_SPECTRUM || MESTYPE == ENUMMESSTYLE._05_CROSSTALK)
                {
                    AutoCreate.Visibility = Visibility.Visible;
                }
                else
                {

                    AutoCreate.Visibility = Visibility.Hidden;
                }


            }
        }

        private void CheckBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void mydata_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            int index = e.Row.GetIndex();

            int n = mylist.SelectedIndex;
            if (n >= lst.Count) { return; }
            n = n != -1 ? n : 0;
            Info info = lst[n];

            if (info.table != null)
            {
                for (int i = index; i < info.table.Rows.Count; i++)
                {
                    info.table.Rows[i]["ID"] = i + 1;
                }
            }
        }


        private void OnBnClickedDeleIndex(object sender, RoutedEventArgs e)
        {
            int n = mylist.SelectedIndex;
            if (n >= lst.Count) { return; }
            Info info = lst[n];
            int index = mydata.SelectedIndex;
            int index_cnt = mydata.SelectedItems.Count;

            if(index <0)
            {
                return;
            }

            if (info.table != null)
            {
                List<DataRow> lstdr = new List<DataRow>();

                //统计删除数据项
                for (int i = index; i < index + index_cnt; i++)
                {
                    lstdr.Add(info.table.Rows[i]);
                }

                //删除数据
                for (int i = 0; i < lstdr.Count; i++)
                {
                    info.table.Rows.Remove(lstdr[i]);
                }

                //重排列
                for (int i = index; i < info.table.Rows.Count; i++)
                {
                    info.table.Rows[i]["ID"] = i + 1;
                }

            }
        }

        private void LoadConfiger(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FileDialog fileDialog=new System.Windows.Forms.OpenFileDialog();
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Project.LoadTempLate(fileDialog.FileName);
            }

            Data2Info();
        }

        private int T =0;
        DateTime date=DateTime.Now;
        private string Name = "-1";
        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            T++; 
            if (T==1)
            {
                Name = (e.OriginalSource as TextBlock).Text;
                date = DateTime.Now;
            }
            if (T>=2)
            {
                if (Name != (e.OriginalSource as TextBlock).Text)
                {
                    T = 0;
                    return;
                }

                Double A=(DateTime.Now - date).TotalMilliseconds;
                if (A>500)
                {
                    T = 0;
                    return;
                }

                int n = mylist.SelectedIndex;
                if (n >= lst.Count)
                {
                    return;
                }
                //lst.RemoveAt(n);

                Info info = (Info)mylist.SelectedItem;
                string info1 = "请输入要修改的名称:";
                if (Project.cfg.Lang != 0)
                {
                    info1 = "Please enter the name you want to modify:";
                }
                pop_up pop_Up = new pop_up(info.Name,info1);
                pop_Up.ShowDialog();

                info.Name = pop_Up.Time;

                lst[n] = info;

                T = 0;
            }
        }


        private void BtnOk_OnClick(object sender, RoutedEventArgs e)//mydata
        {
            MovCtrl mvctrl = MovCtrl.GetInstance();
            //lst

            int ListIndex = mylist.SelectedIndex;


            int dataColl = mydata.SelectedIndex;

            if (dataColl==-1)
            {
                dataColl = mydata.Items.Count-1;
            }
            if (ListIndex==-1)
            {
                ListIndex = mylist.Items.Count-1;
            }

            Info info= lst[ListIndex];

            double dx = 0, dy = 0, dz = 0, du = 0, dv = 0, dball = 0;
            mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
            //Project.cfg.Xorg
            info.table.Rows[dataColl][info.table.Columns[1].ColumnName] = dx - Project.Xorg;
            info.table.Rows[dataColl][info.table.Columns[2].ColumnName] = dy - Project.Yorg;
            info.table.Rows[dataColl][info.table.Columns[3].ColumnName] = dz - Project.Zorg;
            info.table.Rows[dataColl][info.table.Columns[4].ColumnName] = du - Project.Uorg;
            info.table.Rows[dataColl][info.table.Columns[5].ColumnName] = dv - Project.Vorg;
            //info.table.Rows[dataColl][info.table.Columns[6].ColumnName] = dball;

            //ItemCollection data = mydata.Items;
        }

        private void Btn_select_all(object sender, RoutedEventArgs e)
        {
            //全选
            for(int i=0;i< lst.Count;i++)
            {
                lst[i].IsSelected = true;       
            }            
            mylist.ItemsSource = null;
            mylist.ItemsSource = lst;
        }

        private void Btn_select_reverse(object sender, RoutedEventArgs e)
        {
            //反选
            for (int i = 0; i < lst.Count; i++)
            {
                //取反
                lst[i].IsSelected = !lst[i].IsSelected;
            }
            mylist.ItemsSource = null;
            mylist.ItemsSource = lst;
        }

        private void OnBnClickedImport(object sender, RoutedEventArgs e)
        {
            //導入的是點數據啊啊
            string filename = "01-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string info = "请输入名称：";
            if (Project.cfg.Lang != 0)
            {
                info = "Please input template name:";
            }
            pop_up pop_Up = new pop_up(filename, info);
            pop_Up.ShowDialog();
            String Temp = pop_Up.Time;

            //create info
            ENUMMESSTYLE MESTYPE = ENUMMESSTYLE._01_POINT;
            int id = 0;
            if (Project.lstInfos.Count == 0)
            {
                id = 1;
            }
            else
            {
                id = Project.lstInfos.Max(p => p.id);
                id++;
            }
            Info ifo = new Info();
            {
                ifo.IsSelected = true;
                ifo.Name = Temp;
                ifo.MESTYPE = MESTYPE;
                ifo.id = id;
            }            
            DataTemplate sDataTemplate = mylist.ItemTemplate;
            ItemCollection sCollection = mylist.Items;

            //select the creadted one
            lst.Add(ifo);
            int n = lst.Count - 1;
            this.mylist.SelectedIndex = n;

            //从excel里面导入模版啊
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel|*.xls|Excel|*.xlsx"; // 设置文件过滤器，这里是所有文件
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false; 
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "Select excel file";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                // 用户选择了文件，可以处理文件
                string filePath = openFileDialog.FileName;
                // 例如，可以在这里将文件路径显示在文本框中
                // filePathTextBox.Text = filePath;

                //解析excel文件啊
                if (System.IO.File.Exists(filePath) == false)
                {
                    MessageBox.Show("Could not find file:" + filePath, "Error");
                    return;
                }
                parse_excel(filePath,ifo);
            }
        }

        private bool parse_excel(string xls_file, Info info)
        {
            IWorkbook wk = null;
            string extension = System.IO.Path.GetExtension(xls_file);
            try
            {
                FileStream fs = System.IO.File.OpenRead(xls_file);
                if (extension.Equals(".xls"))
                {
                    //把xls文件中的数据写入wk中
                    wk = new HSSFWorkbook(fs);
                }
                else
                {
                    //把xlsx文件中的数据写入wk中
                    wk = new XSSFWorkbook(fs);
                }
                fs.Close();
                //读取当前表数据
                ISheet sheet = wk.GetSheetAt(0);

                int rows = sheet.LastRowNum;
                if (rows < 4)
                {
                    LogHelper.Instance.Write("错误：Excel文件行数太少:" + rows);
                    return false;
                }
                IRow row = sheet.GetRow(2);  //读取第3行的数据
                //首先判断一下厚度数据有没有啊
                int thick_row = -1;
                for(int i=0;i<4;i++) //在前面4行里面找厚度数据
                {
                    row = sheet.GetRow(i);
                    string value = row.GetCell(0).ToString();
                    if(value == "产品厚度"|| value == "產品厚度")
                    {
                        thick_row = i;
                        break;
                    }
                }
                if(thick_row<0)
                {
                    LogHelper.Instance.Write("没有找到产品厚度行");
                    MessageBox.Show("没有找到产品厚度行");
                    return false;
                }
                if(sheet.GetRow(thick_row).GetCell(1)== null)
                {
                    LogHelper.Instance.Write("没有找到产品厚度数据");
                    MessageBox.Show("没有找到产品厚度数据");
                    return false;
                }
                InfoData ifdata = new InfoData();
                try
                {
                    ifdata.height = Double.Parse(sheet.GetRow(thick_row).GetCell(1).ToString().Trim());
                }
                catch(Exception ex)
                {
                    LogHelper.Instance.Write("产品厚度数据转换失败");
                    return false;
                }
                ifdata.IsSelected = true;
                ifdata.Name = comboBoxType.Text + DateTime.Now.ToString("yyyyMMddHHmmss"); ;
                ifdata.MESTYPE = ENUMMESSTYLE._01_POINT; 
                ifdata.lstdata = new List<string>();
                int idmax = 0;
                idmax = Project.lstInfos.Select(p => p.id).Max();
                ifdata.id = idmax+1;

                int id_col = -1;
                int x_col = -1;
                int y_col = -1;
                int z_col = -1;
                int u_col = -1;
                int v_col = -1;
                int pg_no_col = -1;
                int pg_info_col = -1;

                //第三行是各种数据啊
                row = sheet.GetRow(2);
                for (int j = 0; j < row.LastCellNum; j++)
                {
                    string value = row.GetCell(j).ToString();
                    if (value.Contains("ID"))
                    {
                        id_col = j;
                        LogHelper.Instance.Write("id列为：" + id_col);                        
                    }
                    else if (value.Contains("X"))
                    {
                        x_col = j;
                        LogHelper.Instance.Write("x列号为：" + x_col);                        
                    }
                    else if (value.Contains("Y"))
                    {
                        y_col = j;
                        LogHelper.Instance.Write("y列号为：" + y_col);                        
                    }
                    else if (value.Contains("Z"))
                    {
                        z_col = j;
                        LogHelper.Instance.Write("z列号为：" + z_col);                        
                    }
                    else if (value.Contains("U"))
                    {
                        u_col = j;
                        LogHelper.Instance.Write("u列号为：" + u_col);                        
                    }
                    else if (value.Contains("V"))
                    {
                        v_col = j;
                        LogHelper.Instance.Write("v列号为：" + v_col);                        
                    }                    
                    else if (value.Contains("PG") && (value.Contains("序号")||
                         value.Contains("序號")))
                    {
                        pg_no_col = j;
                        LogHelper.Instance.Write("PG(序号)列号为：" + pg_no_col);                        
                    }
                    else if (value.Contains("PG") && value.Contains("提示"))
                    {
                        pg_info_col = j;
                        LogHelper.Instance.Write("PG（提示信息）列号为：" + pg_info_col);                        
                    }
                }
                if ((x_col < 0) || (y_col < 0) || (z_col <0)||
                    (u_col <0) || (v_col <0) ||
                    (pg_no_col<0) || (pg_info_col<0))
                {
                    LogHelper.Instance.Write("Excel文件里面有信息列缺失");
                    MessageBox.Show("Excel文件里面有信息列缺失");
                    return false;
                }

                string strheader = "";
                List<string> datalist = new List<string>();
                datalist.Add("ID");
                datalist.Add("X(mm)");
                datalist.Add("Y(mm)");
                datalist.Add("Z(mm)");
                datalist.Add("U(°)");
                datalist.Add("V(°)");
                datalist.Add("PG(序号)");
                datalist.Add("PG(提示信息)");
                strheader = string.Join(",",datalist);//先添加的是表头
                ifdata.lstdata.Add(strheader);

                //添加表頭
                InitDataGrid_POINT(info);
                //LastRowNum 是当前表的总行数-1（注意）
                int id = 1;
                //数据是从第4行开始的
                for (int i = 3; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);  //读取当前行数据
                    if (row != null)
                    {
                        if (row.GetCell(x_col) == null)
                        {
                            LogHelper.Instance.Write("第" + (i + 1) + "行x数据为空");
                            break;
                        }
                        if (row.GetCell(y_col) == null)
                        {
                            LogHelper.Instance.Write("第" + (i + 1) + "行y数据为空");
                            break;
                        }
                        //有了x和y就可以建立数据了啊
                        datalist.Clear();
                        datalist.Add(id.ToString());
                        datalist.Add(row.GetCell(x_col).ToString());
                        datalist.Add(row.GetCell(y_col).ToString());
                        if (row.GetCell(z_col) != null)
                        {
                            datalist.Add(row.GetCell(z_col).ToString());
                        }
                        else
                        {
                            datalist.Add("");
                        }

                        if (row.GetCell(u_col) != null)
                        {
                            datalist.Add(row.GetCell(u_col).ToString());
                        }
                        else
                        {
                            datalist.Add("");
                        }

                        if (row.GetCell(v_col) != null)
                        {
                            datalist.Add(row.GetCell(v_col).ToString());
                        }
                        else
                        {
                            datalist.Add("");
                        }

                        if (row.GetCell(pg_no_col) != null)
                        {
                            datalist.Add(row.GetCell(pg_no_col).ToString());
                        }
                        else
                        {
                            datalist.Add("");
                        }

                        if (row.GetCell(pg_info_col) != null)
                        {
                            datalist.Add(row.GetCell(pg_info_col).ToString());
                        }
                        else
                        {
                            datalist.Add("");
                        }
                        string data = string.Join(",", datalist);//添加的是数据
                        ifdata.lstdata.Add(data);
                        id++;

                        //加到表格裡面啊
                        info.table.Rows.Add(datalist.ToArray());
                    }
                    else
                    {
                        //空数据了啊
                        break;
                    }
                }


                ifdata.id = info.id;
                DataView dv = new DataView(info.table);
                ///定义数据表
                mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
                mydata.VerticalGridLinesBrush = Brushes.Gray;
                mydata.CanUserSortColumns = false;
                mydata.MinColumnWidth = 70;
                mydata.ItemsSource = dv;

                //展示厚度
                ProductHeight.Text = ifdata.height.ToString();
                Project.lstInfos.Add(ifdata);//加到队列里面啊，后面才能搜索到

                //可能lstInfos有更新，首先获取当前模板组内容
                //Project.lstInfos = Info2Data();
                ////再增加
                //Project.lstInfos.Add(ifdata);//当前模板组内容增加一项啊
                //Project.SaveTemplate("Template.xml");

                ////刷新界面啊
                //MessageBox.Show("导入成功","提示");
                //this.Close();
            }
            catch (Exception e)
            {
                LogHelper.Instance.Write("读取Excel文件异常：" + e.Message);
                MessageBox.Show("读取Excel文件异常：" + e.Message);
            }

            return false;
        }

        private void Color_select(object sender, MouseButtonEventArgs e)
        {
            // 创建一个ColorDialog实例
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            // 显示颜色选择面板
            var result = colorDialog.ShowDialog();
            // 检查用户是否选择了颜色
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // 获取所选颜色的RGB值
                System.Drawing.Color selectedColor = colorDialog.Color;
                //update_warn_color(selectedColor);
                //保存颜色
                //ptmodel.color = selectedColor;
                System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);
                RecColor.Fill = new System.Windows.Media.SolidColorBrush(wpfColor);
            }
        }

        private void BtnOnSave_click(object sender, RoutedEventArgs e)
        {
            Info info = (Info)mylist.SelectedItem;
            if(info == null)
            {
                return;
            }
            InfoData infoData = Project.lstInfos.FirstOrDefault(p => p.id == info.id);
            if(infoData == null)
            {
                return;
            }
            if (Xcheck.IsChecked == true)
            {
                if (CustomTemplate.check_input(xmax, "X最大值") == false)
                {
                    return;
                }
                if (CustomTemplate.check_input(xmin, "X最小值") == false)
                {
                    return;
                }
                if (double.Parse(xmax.Text) < double.Parse(xmin.Text))
                {
                    MessageBox.Show("错误：测量标准中的X最大值比最小值小");
                    return;
                }
            }

            if (Ycheck.IsChecked == true)
            {
                if (CustomTemplate.check_input(ymax, "Y最大值") == false)
                {
                    return;
                }
                if (CustomTemplate.check_input(ymin, "Y最小值") == false)
                {
                    return;
                }
                if (double.Parse(ymax.Text) < double.Parse(ymin.Text))
                {
                    MessageBox.Show("错误：测量标准中的Y最大值比最小值小");
                    return;
                }
            }

            if (Lcheck.IsChecked == true)
            {
                if (CustomTemplate.check_input(Lmax, "L最大值") == false)
                {
                    return;
                }
                if (CustomTemplate.check_input(Lmin, "L最小值") == false)
                {
                    return;
                }
                if (double.Parse(Lmax.Text) < double.Parse(Lmin.Text))
                {
                    MessageBox.Show("错误：测量标准中的L最大值比最小值小");
                    return;
                }
            }

            if (Balancechk.IsChecked == true)
            {
                if (CustomTemplate.check_input(balancemin, "一致性") == false)
                {
                    return;
                }
            }
            if (Lcheck.IsChecked == true)
            {
                infoData.IsLchk = true;
                infoData.Lmax = double.Parse(Lmax.Text);
                infoData.Lmin = double.Parse(Lmin.Text);
            }
            else
            {
                infoData.IsLchk = false;
            }
            if (Xcheck.IsChecked == true)
            {
                infoData.Isxchk = true;
                infoData.xmax = double.Parse(xmax.Text);
                infoData.xmin = double.Parse(xmin.Text);
            }
            else
            {
                infoData.Isxchk = false;
            }
            if (Ycheck.IsChecked == true)
            {
                infoData.Isychk = true;
                infoData.ymax = double.Parse(ymax.Text);
                infoData.ymin = double.Parse(ymin.Text);
            }
            else
            {
                infoData.Isychk = false;
            }
            if (Balancechk.IsChecked == true)
            {
                infoData.balancemin = double.Parse(balancemin.Text);
                infoData.IsBalancechk = true;
            }
            else
            {
                infoData.IsBalancechk = false;
            }
            SolidColorBrush solidColorBrush = RecColor.Fill as SolidColorBrush;
            System.Windows.Media.Color color = solidColorBrush.Color;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            infoData.warnR = r;
            infoData.warnG = g;
            infoData.warnB = b;
            //保存到文件
            Project.SaveTemplate("Template.xml");
            string msg = "设置已经保存";
            if (Project.cfg.Lang != 0)
            {
                msg = "Saved";
            }
            MessageBox.Show(msg);
        }

        private void OnBnClickedChange(object sender, RoutedEventArgs e)
        {
            int count = mylist.SelectedItems.Count;
            if(count!=1)
            {
                MessageBox.Show("请勾选单个模版后，再进行修改");
                return;
            }
            //修正按钮
            Info info = (Info)mylist.SelectedItem;
            if(info == null)
            {
                MessageBox.Show("修改失败");
                return;
            }
            double height = 0;
            try
            {
                height = double.Parse(ProductHeight.Text.Trim());
            }
            catch
            {
                MessageBox.Show("高度输入错误，请重新修改");
                ProductHeight.Focus();
                return;
            }
            info.height = height;
            MessageBox.Show("修改成功");
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            int count = mylist.SelectedItems.Count;
            if (count != 1)
            {
                MessageBox.Show("请选中单个模版后，再进行复制");
                return;
            }
            Info info = (Info)mylist.SelectedItem;
            if (info == null)
            {
                MessageBox.Show("复制失败");
                return;
            }

            int idmax = 0;
            idmax = lst.Select(p => p.id).Max();           
            Info ifo = new Info();
            {
                ifo.IsSelected = true;
                ifo.Name = info.Name+"_副本";
                ifo.MESTYPE = info.MESTYPE;
                ifo.Type = info.Type;
                ifo.height = info.height;
                ifo.id = idmax+1;                
            }

            DataTemplate sDataTemplate = mylist.ItemTemplate;
            ItemCollection sCollection = mylist.Items;
            int n = mylist.SelectedIndex;
            lst.Insert(n+1, ifo);
            //int n = lst.Count - 1;
            //this.mylist.SelectedIndex = n;
           

            //接下来复制数据啊
            InfoData infoData = Project.lstInfos.FirstOrDefault(p => p.id == info.id);
            if(infoData != null)
            {
                InfoData ifdata = new InfoData();
                ifdata.height = infoData.height;
                //ifdata.height = Double.Parse(ProductHeight.Text.Trim());
                ifdata.IsSelected = infoData.IsSelected;
                ifdata.Name = ifo.Name;
                ifdata.MESTYPE = infoData.MESTYPE;
                ifdata.height = info.height;
                ifdata.lstdata = new List<string>();
                //使用新增的id
                ifdata.id = ifo.id;

                //把点位数据拷贝过来啊
                ifdata.lstdata = new List<string>(); 
                infoData.lstdata.ForEach(p => ifdata.lstdata.Add(p));

                ifdata.Isxchk = infoData.Isxchk;
                ifdata.Isychk = infoData.Isychk;
                ifdata.IsLchk = infoData.IsLchk;
                ifdata.Lmax = infoData.Lmax;
                ifdata.Lmin = infoData.Lmin;
                ifdata.xmax = infoData.xmax;
                ifdata.xmin = infoData.xmin;
                ifdata.ymax = infoData.ymax;
                ifdata.ymin = infoData.ymin;
                ifdata.IsBalancechk = infoData.IsBalancechk;
                ifdata.balancemin = infoData.balancemin;
                ifdata.warnR = infoData.warnR;
                ifdata.warnG = infoData.warnG;
                ifdata.warnB = infoData.warnB;

                ifdata.productLength = infoData.productLength;
                ifdata.productWidth = infoData.productWidth;
                ifdata.IsMeter = infoData.IsMeter;
                ifdata.Ameter = infoData.Ameter;
                ifdata.Bmeter = infoData.Bmeter;
                ifdata.Apercent = infoData.Apercent;
                ifdata.Bpercent = infoData.Bpercent;
                ifdata.Cmeter = infoData.Cmeter;
                ifdata.Dmeter = infoData.Dmeter;
                ifdata.Cpercent = infoData.Cpercent;
                ifdata.Dpercent = infoData.Dpercent;

                Lst2Table(ifdata.lstdata, ref ifo.table);
                Project.lstInfos.Add(ifdata);
            }


            //最后修改选中
            mylist.SelectedIndex = n + 1;
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            int count = mylist.SelectedItems.Count;
            if (count != 1)
            {
                MessageBox.Show("请选中单个模版后，再进行重命名");
                return;
            }
            //修正按钮
            OnBnClickedReName(null,null);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int count = lst.Count(p => p.IsSelected == true);
            if (count > 1)
            {
                MessageBox.Show("一次只能删除一个模版");
                return;
            }
            if (count <= 0)
            {
                MessageBox.Show("模版没有选中，不能删除");
                return;
            }
            
            OnBnClickedDelete(null, null);
        }
    }

    //测试数据
    public class InfoData
    {
        public int id;//唯一的id，每次增加,通过id来区分
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
        //告警颜色rgb
        public int warnR;
        public int warnG;
        public int warnB;
    }


    //public enum ENUMMESSTYLE
    //{
    //    _01_POINT,
    //    _02_RESPONSE,
    //    _03_SPECTRUM,
    //    _04_FLICKER,
    //    _05_CROSSTALK,
    //    _06_ACR
    //}

}
