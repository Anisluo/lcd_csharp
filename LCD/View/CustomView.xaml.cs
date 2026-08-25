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

            //心跳函数
            System.Timers.Timer timer = new System.Timers.Timer();//声明timer对象
            timer.Interval = 150;//100ms刷新一次
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            timer.Start();
            if (Project.PG!=null)
            {
                for (int i = 0; i < Project.PG?.PatternList.Size; i++)
                {
                    InfoList infoList = new InfoList();
                    infoList.Name2 = i.ToString();
                    infoList.Name1 = Project.PG.PatternList.ItemStrings[i].name;
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
            List<InfoData> lstDatas = new List<InfoData>();

            for (int i = 0; i < lst.Count; i++)
            {
                InfoData ifdata = new InfoData();
                ifdata.height = Double.Parse(ProductHeight.Text.Trim());
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
            lst.Add(info1);
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
                    continue;
                }

                Info inf = new Info();
                inf.IsSelected = Project.lstInfos[i].IsSelected;
                inf.Name = Project.lstInfos[i].Name;
                inf.MESTYPE = Project.lstInfos[i].MESTYPE;

                Lst2Table(Project.lstInfos[i].lstdata, ref inf.table);
                lst.Add(inf);

            }
        }

        private void Lst2Table(List<string> lstdata, ref DataTable dt)
        {
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
            
            //Info info = (Info)mylist.SelectedItem;
            //string text = info.Name;
            //int n = mylist.SelectedIndex;
            //if (n != -1)
            //{
            //    lst.RemoveAt(n);
            //}

        }

        private void OnBnClickedDelete(object sender, RoutedEventArgs e)
        {
            int n = mylist.SelectedIndex;
            if (n >= lst.Count)
            {
                return;
            }
            lst.RemoveAt(n);
            mylist.ItemsSource = null;
            mylist.ItemsSource = lst;
        }


        /// <summary>
        /// 自动增加模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBnClickedAddTemplate(object sender, RoutedEventArgs e)
        {
            string filename = comboBoxType.Text + DateTime.Now.ToString("yyyyMMddHHmmss");

           
            pop_up pop_Up = new pop_up(filename,"请输入名称:");
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

            Info ifo = new Info();
            {
                ifo.IsSelected = true;
                ifo.Name = Temp;
                ifo.MESTYPE = MESTYPE;
            }

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

            public ENUMMESSTYLE MESTYPE;
            

           

            public DataTable table;


        }

        private void mylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Info info = (Info)mylist.SelectedItem;
            InitDataGrid(info);
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

            CustomMessage cstom = new CustomMessage("是否使用当前模板组进行测试？");
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
            CustomTemplate cstm = new CustomTemplate();
            cstm.ptmodel.tempName = info.Name;
            CustomTemplate.dt = info.table;
            //定义默认参数
            cstm.ShowDialog();
            info.table = !CustomTemplate.IsEnsure ? info.table : CustomTemplate.dt;

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
                case 3: MESTYPE = ENUMMESSTYLE._04_FLICKER; break;
                case 4: MESTYPE = ENUMMESSTYLE._05_CROSSTALK; break;
                case 5: MESTYPE = ENUMMESSTYLE._06_ACR; break;
                case 6: MESTYPE = ENUMMESSTYLE._07_warmup; break;
            }

            if (AutoCreate != null)
            {
                if (MESTYPE == ENUMMESSTYLE._01_POINT| MESTYPE == ENUMMESSTYLE._07_warmup)
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

                pop_up pop_Up = new pop_up(info.Name,"请输入要修改的名称：");
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
    }

    //测试数据
    public class InfoData
    {
        public double height;
        
        public bool IsSelected;
        public ENUMMESSTYLE MESTYPE;
        public string Name;
        public string Name1;
        public List<string> lstdata;
        public string productLength;
        public double productWidth;
        public bool IsMeter;
        public double Ameter;
        public double Bmeter;
        public double Apercent;
        public double Bpercent;
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
