using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LCD.Data;
using LCD.dataBase;

namespace LCD.View
{
    /// <summary>
    /// 历史记录控件
    /// </summary>
    public partial class HistoryView : Window
    {
        private user_id user = null;

        private MainWindowVM mainWindow = null;
        //设置
        DataTable dt = new DataTable();
        private DataTable ResultDatatemp = null;
        private level level = level.BarCode;
        public HistoryView()
        {
            InitializeComponent();
            //Init();
            mainWindow = new LCD.View.MainWindowVM();
            this.DataContext = mainWindow;
            //Init();
        }

        public void Init()
        {

            user = new user_id();
            for (int i = 0; i < Project.listBarCode.Count; i++)
            {
                
            }
            level = level.BarCode;
            //mylist.ItemsSource = lstdatas;

            ResultDatatemp = new DataTable();
            DataView dv = new DataView(ResultDatatemp);
            mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            mydata.VerticalGridLinesBrush = Brushes.Gray;
            mydata.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly =true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            //ResultData.MinColumnWidth = 70;
            mydata.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            mydata.ItemsSource = dv;
        }

       




        //导出测试结果
        private void OnBnClickedOutput(object sender, RoutedEventArgs e)
        {
            DateTime dateTime=DateTime.Now;;
            //this.Cursor=System.Windows.Forms.Cursor.
            SaveDate();

            System.Windows.Forms.MessageBox.Show("导出成功");

            //Project.SaveTestData

            double ss= (DateTime.Now - dateTime).Seconds;
        }

       
        public void SaveDate()
        {
            Project.SaveTestData.Clear();
           //Project.SaveTestData

            for (int i = 0; i < mainWindow.Items.Count; i++)//条码
            {
                if (mainWindow.Items[i].IsChecked == true)//条码是否勾选
                {
                    UserIdMode userIdMode = new UserIdMode();

                    userIdMode.ID=mainWindow.Items[i].ID;
                    userIdMode.BarCode = mainWindow.Items[i].Header;
                    

                    List< ProjectModeClass> projectModeClasses = new List< ProjectModeClass>();
                    for (int j = 0; j < mainWindow.Items[i].Children.Count; j++)//项目
                    {
                        ProjectModeClass projectModeClass=new ProjectModeClass();

                       
                        if (mainWindow.Items[i].Children[j].IsChecked == true)//项目是否勾选
                        {
                            projectModeClass.ID = mainWindow.Items[i].Children[j].ID;
                            projectModeClass.projectName = mainWindow.Items[i].Children[j].Header;
                            projectModeClass.ModeType= mainWindow.Items[i].Children[j].TestType;
                            ENUMMESSTYLE eNUMMESSTYLE = (ENUMMESSTYLE)mainWindow.Items[i].Children[j].TestType;
                            List<TestDataMode> testData = null;
                            List<SpectrumDataMode> spectrum = null;
                            if (eNUMMESSTYLE == ENUMMESSTYLE._03_SPECTRUM)
                            {
                                testData=TestData.RederList(mainWindow.Items[i].Children[j].ID);
                                spectrum = SpectrumData.RederList(mainWindow.Items[i].Children[j].ID);
                            }
                            else
                            {
                                testData= TestData.RederList(mainWindow.Items[i].Children[j].ID);
                            }
                            projectModeClass.TestDataModes = testData;
                            projectModeClass.SpectrumDataModes = spectrum;
                        }
                        projectModeClasses.Add(projectModeClass);
                    }
                    userIdMode.ProjectModes = projectModeClasses;

                    Project.SaveTestData.Add(userIdMode);
                }
            }

            List<dataBase.UserIdMode> list = Project.SaveTestData;

            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "|*.xlsx";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string Name = saveFileDialog1.FileName;
                //try
                //{
                    ExcelHelper.Save2ExcelMultyWithTitle(Name, Project.SaveTestData);
                //}
                //catch (Exception e)
                //{
                //    System.Windows.Forms.MessageBox.Show(e.Message);
                //}
                
            }
        }




        public void AddSingleData(TestDataMode objs, ENUMMESSTYLE enummesstyle)
        {

            if (objs == null) { Project.WriteLog("加载数据失败！"); return; }
            DataRow dataRow = null;
            if (enummesstyle == ENUMMESSTYLE._01_POINT)
            {
                
                dataRow = ResultDatatemp.NewRow();
                dataRow["Num"] = objs.Num;
                dataRow["L"] = objs.L;
                //dataRow["X"] = objs.X;
                //dataRow["Y"] = objs.Y;
                //dataRow["Z"] = objs.Z;
                dataRow["Cx"] = objs.Cx;
                dataRow["Cy"] = objs.Cy;
                dataRow["u"] = objs.u;
                dataRow["v"] = objs.v;
                dataRow["CCT"] = objs.CCT;
                dataRow["完成时间"] = objs.Time;
                dataRow["备注"] = objs.Remark;

            }
            else if (enummesstyle == ENUMMESSTYLE._02_RESPONSE)
            {
                dataRow = ResultDatatemp.NewRow();
                dataRow["Num"] = objs.Num;
                dataRow["X"] = objs.X;
                dataRow["Y"] = objs.Y;
                dataRow["Z"] = objs.Z;
                dataRow["Low"] = objs.Low;
                dataRow["High"] = objs.High;
                dataRow["RiseTime"] = objs.RiseTime;
                dataRow["FallTime"] = objs.FallTime;
                dataRow["完成时间"] = objs.Time;
            }
            else if (enummesstyle == ENUMMESSTYLE._03_SPECTRUM)
            {
                
                dataRow = ResultDatatemp.NewRow();
                dataRow["Num"] = objs.Num;
                dataRow["L"] = objs.L;
                //dataRow["X"] = objs.X;
                //dataRow["Y"] = objs.Y;
                //dataRow["Z"] = objs.Z;
                dataRow["Cx"] = objs.Cx;
                dataRow["Cy"] = objs.Cy;
                dataRow["u"] = objs.u;
                dataRow["v"] = objs.v;
                dataRow["CCT"] = objs.CCT;
                dataRow["完成时间"] = objs.Time;
                dataRow["备注"] = objs.Remark;
                for (int i = 0; i < 400; i++)
                {
                    //SpectrumData.Insert(new SpectrumDataMode()
                    //{
                    //    Project_id = Project.ProjectID,
                    //    DataName = $"{i + 380}",
                    //    dataValue = objs.SpectrumData[i].ToString()
                    //});
                    //dataRow[$"{i + 380}"] = objs.SpectrumData[i];
                }

            }
            else if (enummesstyle == ENUMMESSTYLE._04_FLICKER)
            {

            }
            else if (enummesstyle == ENUMMESSTYLE._05_CROSSTALK)
            {
                dataRow = ResultDatatemp.NewRow();
                dataRow["Num"] = objs.Num;
                dataRow["La"] = objs.La;
                dataRow["Lb"] = objs.Lb;
                dataRow["CT"] = objs.CT;
                dataRow["完成时间"] = objs.Time;
            }
            else if (enummesstyle==ENUMMESSTYLE._07_warmup)
            {
                dataRow = ResultDatatemp.NewRow();
                dataRow["Num"] = objs.Num;
                dataRow["L"] = objs.L;
                //dataRow["X"] = objs.X;
                //dataRow["Y"] = objs.Y;
                //dataRow["Z"] = objs.Z;
                dataRow["Cx"] = objs.Cx;
                dataRow["Cy"] = objs.Cy;
                dataRow["u"] = objs.u;
                dataRow["v"] = objs.v;
                dataRow["CCT"] = objs.CCT;
                dataRow["完成时间"] = objs.Time;
                dataRow["备注"] = objs.Remark;
            }
            else if ((enummesstyle == ENUMMESSTYLE.Power))
            {

                dataRow = ResultDatatemp.NewRow();
                dataRow["Num"] = objs.Num;
                dataRow["电压"] = objs.Voltage;
                dataRow["电流"] = objs.ElectricCurrent;
                dataRow["功率"] = objs.Power;             
            }


            ResultDatatemp.Rows.InsertAt(dataRow, ResultDatatemp.Rows.Count + 1);

            DataView dv = new DataView(ResultDatatemp);



            this.Dispatcher.Invoke(new Action(() =>
            {
                this.mydata.ItemsSource = dv;
            }));

            //dataRow.Delete();
            //ResultDatatemp.Rows.Add() = testname;


            //if (ResutType != objs.GetType())
            //{
            //    Init(objs);
            //}
            //objs.TestName = testname;
            //objs.ID = Datas.Count;
            //Datas.Add(objs);
            //Datas = new List<IData>(Datas);
        }

        public void AddSingleData(TestDataMode objs,int index)
        {
            if (objs == null) { Project.WriteLog("加载数据失败！"); return; }
            DataRow dataRow = null;
            dataRow = ResultDatatemp.NewRow();
            dataRow["Num"] = objs.Num;
            dataRow["L"] = objs.L;
            //dataRow["X"] = objs.X;
            //dataRow["Y"] = objs.Y;
            //dataRow["Z"] = objs.Z;
            dataRow["Cx"] = objs.Cx;
            dataRow["Cy"] = objs.Cy;
            dataRow["u"] = objs.u;
            dataRow["v"] = objs.v;
            dataRow["CCT"] = objs.CCT;
            dataRow["完成时间"] = objs.Time;
            dataRow["备注"] = objs.Remark;

            List<SpectrumDataMode> ListSpectrumData = Project.ListSpectrumData;

            index = index * 400;

            for (int i = index; i < index+401; i++)
            {
                dataRow[$"{Project.ListSpectrumData[i].DataName}"] = Project.ListSpectrumData[i].dataValue;
            }

            ResultDatatemp.Rows.InsertAt(dataRow, ResultDatatemp.Rows.Count + 1);

            DataView dv = new DataView(ResultDatatemp);



            this.Dispatcher.Invoke(new Action(() =>
            {
                this.mydata.ItemsSource = dv;
            }));
        }


        private void DataMode(ENUMMESSTYLE enummesstyle)
        {
            if (enummesstyle == ENUMMESSTYLE._01_POINT)
            {
                ResultDatatemp=new DataTable();

                //ResultDatatemp.Columns.Add("测试项");
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("L");
                //ResultDatatemp.Columns.Add("X");
                //ResultDatatemp.Columns.Add("Y");
                //ResultDatatemp.Columns.Add("Z");
                ResultDatatemp.Columns.Add("Cx");
                ResultDatatemp.Columns.Add("Cy");
                ResultDatatemp.Columns.Add("u");
                ResultDatatemp.Columns.Add("v");
                ResultDatatemp.Columns.Add("CCT");
                ResultDatatemp.Columns.Add("完成时间");
                ResultDatatemp.Columns.Add("备注");
            }
            else if (enummesstyle == ENUMMESSTYLE._02_RESPONSE)
            {
                ResultDatatemp = new DataTable();
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("X");
                ResultDatatemp.Columns.Add("Y");
                ResultDatatemp.Columns.Add("Z");
                ResultDatatemp.Columns.Add("Low");
                ResultDatatemp.Columns.Add("High");
                ResultDatatemp.Columns.Add("RiseTime");
                ResultDatatemp.Columns.Add("FallTime");
                ResultDatatemp.Columns.Add("完成时间");
            }
            else if (enummesstyle == ENUMMESSTYLE._03_SPECTRUM)
            {
                ResultDatatemp = new DataTable();
                //ResultDatatemp.Columns.Add("测试项");
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("L");
                //ResultDatatemp.Columns.Add("X");
                //ResultDatatemp.Columns.Add("Y");
                //ResultDatatemp.Columns.Add("Z");
                ResultDatatemp.Columns.Add("Cx");
                ResultDatatemp.Columns.Add("Cy");
                ResultDatatemp.Columns.Add("u");
                ResultDatatemp.Columns.Add("v");
                ResultDatatemp.Columns.Add("CCT");
                ResultDatatemp.Columns.Add("完成时间");
                ResultDatatemp.Columns.Add("备注");
                for (int i = 0; i < 401; i++)
                {
                    ResultDatatemp.Columns.Add($"{i + 380}");
                }
            }
            else if (enummesstyle == ENUMMESSTYLE._04_FLICKER)
            {

            }
            else if (enummesstyle == ENUMMESSTYLE._05_CROSSTALK)
            {
                ResultDatatemp = new DataTable();
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("La");
                ResultDatatemp.Columns.Add("Lb");
                ResultDatatemp.Columns.Add("CT");
                ResultDatatemp.Columns.Add("完成时间");
            }
            else if ((enummesstyle == ENUMMESSTYLE._07_warmup))
            {
                ResultDatatemp = new DataTable();
                //ResultDatatemp.Columns.Add("测试项");
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("L");
                //ResultDatatemp.Columns.Add("X");
                //ResultDatatemp.Columns.Add("Y");
                //ResultDatatemp.Columns.Add("Z");
                ResultDatatemp.Columns.Add("Cx");
                ResultDatatemp.Columns.Add("Cy");
                ResultDatatemp.Columns.Add("u");
                ResultDatatemp.Columns.Add("v");
                ResultDatatemp.Columns.Add("CCT");
                ResultDatatemp.Columns.Add("完成时间");
                ResultDatatemp.Columns.Add("备注");
            }
            else if ((enummesstyle == ENUMMESSTYLE.Power))
            {
                ResultDatatemp = new DataTable();
                ResultDatatemp.Columns.Add("测试项");
                ResultDatatemp.Columns.Add("Num");
                ResultDatatemp.Columns.Add("电压");
                //ResultDatatemp.Columns.Add("");
                //ResultDatatemp.Columns.Add("");
                //ResultDatatemp.Columns.Add("");
                ResultDatatemp.Columns.Add("电流");
                ResultDatatemp.Columns.Add("功率");
            }

            DataView dv = new DataView(ResultDatatemp);
            mydata.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            mydata.VerticalGridLinesBrush = Brushes.Gray;
            mydata.CanUserSortColumns = false;
            //mydata.Columns[0].IsReadOnly =true;
            //mydata.HorizontalAlignment = HorizontalAlignment.Center;
            //ResultData.MinColumnWidth = 70;
            mydata.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            mydata.ItemsSource = dv;
        }


        private void Control_OnMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //MyView mainWindow
            TextBlock text = (TextBlock) sender;
            CheckBoxTreeViewModel checkBoxTreeView = (CheckBoxTreeViewModel)text.DataContext;
            if (checkBoxTreeView.Parent == null)
            {
                return;
            }
            int ID = 0;
            for (int i = 0; i < Project.listBarCode.Count; i++)
            {
                if (checkBoxTreeView.Parent.Header==Project.listBarCode[i].BarCode)
                {
                    for (int j = 0; j < Project.listBarCode[i].ProjectModes.Count; j++)
                    {
                        if (checkBoxTreeView.Header== Project.listBarCode[i].ProjectModes[j].projectName)
                        {
                            ID = Project.listBarCode[i].ProjectModes[j].ID;

                            ENUMMESSTYLE enummesstyle= (ENUMMESSTYLE)Project.listBarCode[i].ProjectModes[j].ModeType;
                            if (enummesstyle==ENUMMESSTYLE._03_SPECTRUM)
                            {
                                SpectrumData.RederList(ID);
                            }
                            DataMode(enummesstyle);
                            TestData.RederList(ID);
                            if (enummesstyle == ENUMMESSTYLE._03_SPECTRUM)
                            {
                                for (int k = 0; k < Project.TestDataModes.Count; k++)
                                {
                                    AddSingleData(Project.TestDataModes[k],k);
                                }
                            }
                            else
                            {
                                for (int k = 0; k < Project.TestDataModes.Count; k++)
                                {
                                    AddSingleData(Project.TestDataModes[k], enummesstyle);
                                }
                            }
                            
                            return;
                        }
                    }
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            user_id user_Id = new user_id();
            for (int i = 0; i < mainWindow.Items.Count; i++)//条码
            {
                if (mainWindow.Items[i].IsChecked == true)//条码是否勾选
                {
                    UserIdMode userIdMode = new UserIdMode();

                    userIdMode.ID = mainWindow.Items[i].ID;
                    userIdMode.BarCode = mainWindow.Items[i].Header;

                    user_Id.DeleteAll(userIdMode.ID, userIdMode.BarCode);
                }
            }
            mainWindow.Init();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            //if (((LCD.View.CheckBoxTreeViewModel)((System.Windows.Controls.CheckBox)(sender)).DataContext).Parent==null)
            //{
            //    return;
            //}
            //((LCD.View.CheckBoxTreeViewModel)((System.Windows.Controls.CheckBox)(sender)).DataContext).Parent.IsChecked = true;
            ////((MainWindowVM)this.DataContext).Items[0].IsChecked=true;
            LCD.View.CheckBoxTreeViewModel treeView = ((LCD.View.CheckBoxTreeViewModel)((System.Windows.Controls.CheckBox)(sender)).DataContext);
            var treeViewItem = treeView.Children;
            if (treeViewItem != null)
            {
                bool st = (bool)treeView.IsChecked;
                set_child(treeViewItem, st);
            }
        }

        private void set_child(List<CheckBoxTreeViewModel> currNode, bool selected)
        {
            if (currNode != null)
            {
                if (currNode.Count > 0)
                {
                    for (int i = 0; i < currNode.Count; i++)
                    {
                        currNode[i].IsChecked = selected;
                    }
                }
            }
        }
    }

    public class CheckBoxTreeViewModel : ViewBase
    {
        private bool? _IsChecked = false;
        private int _ID;
        private string _Header = string.Empty;
        private List<CheckBoxTreeViewModel> _Children = null;
        private CheckBoxTreeViewModel _Parent = null;

        public bool? IsChecked { get => _IsChecked; set
        {
            _IsChecked = value; /*OnPropertyChanged();*/ }
        }//this.SetIsChecked(value, true, true);
        public string Header { get => _Header;
            set { _Header = value;/*OnPropertyChanged();*/ }
        }//set => _Header = value;
        public List<CheckBoxTreeViewModel> Children { get => _Children; set { _Children = value; SetParentValue(); /*OnPropertyChanged();*/ } }//
        public int ID { get => _ID; set => _ID = value; }
        public CheckBoxTreeViewModel Parent { get => _Parent; private set => _Parent = value; }

        private int testtype;
            
        public int TestType
        {
            get { return testtype; }
            set { testtype = value; }
        }


        /// <summary>
        /// 设置节点IsChecked的值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="updateChildren"></param>
        /// <param name="updateParent"></param>
        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _IsChecked)
                return;
            _IsChecked = value;
            if (updateChildren  && Children != null)//&& _IsChecked.HasValue
            {
                this.Children.ForEach(c => c.SetIsChecked(_IsChecked, true, false));
            }
            if (updateParent && Parent != null)
            {
                Parent.VerifyCheckState();
            }
            this.OnPropertyChanged();
        }
        /// <summary>
        /// 验证并设置父级节点的IsChecked的值
        /// </summary>
        private void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }
        /// <summary>
        /// 数据初始化时设置父节点的值
        /// </summary>
        private void SetParentValue()
        {
            if (this.Children != null)
            {
                this.Children.ForEach(ch => ch.Parent = this);
            }
        }
    }


    public class MainWindowVM : ViewBase
    {
        private user_id user = null;
        public MainWindowVM()
        {
            Init();
        }
        public void Init()
        {
            user = new user_id();
            InitData();
        }

        private ObservableCollection<CheckBoxTreeViewModel> _Items = null;

        public ObservableCollection<CheckBoxTreeViewModel> Items
        {
            get => _Items;
            set { _Items = value; OnPropertyChanged(); }
        }


        private void InitData()
        {
            Items=new ObservableCollection<CheckBoxTreeViewModel>();
           
            for (int i = 0; i < Project.listBarCode.Count; i++)
            {
                var items1 = new List<CheckBoxTreeViewModel>();
                for (int j = 0; j < Project.listBarCode[i].ProjectModes.Count; j++)
                {
                    items1.Add(new CheckBoxTreeViewModel()
                    {
                        TestType = Project.listBarCode[i].ProjectModes[j].ModeType,
                        ID = Project.listBarCode[i].ProjectModes[j].ID,
                        Header = Project.listBarCode[i].ProjectModes[j].projectName
                    }) ;
                }

                Items.Add(new CheckBoxTreeViewModel()
                {
                    ID=Project.listBarCode[i].ID,
                    Header = Project.listBarCode[i].BarCode,Children = items1
                });

            }


            //var items3 = new List<CheckBoxTreeViewModel> { new CheckBoxTreeViewModel{ID=6,Header="item1.1.1", Children=null },
            //    new CheckBoxTreeViewModel{ID=7,Header="item1.1.2", Children=null },
            //    new CheckBoxTreeViewModel{ID=8,Header="item1.1.3", Children=null },
            //    new CheckBoxTreeViewModel{ID=9,Header="item1.1.4", Children=null }};

            //var items4 = new List<CheckBoxTreeViewModel> { new CheckBoxTreeViewModel{ID=101, Header="item2.1.1", Children=null },
            //    new CheckBoxTreeViewModel{ID=102, Header="item2.1.2", Children=null }};

            //var items1 = new List<CheckBoxTreeViewModel> { new CheckBoxTreeViewModel{ID=3,Header="item1.1", Children=items3 },
            //    new CheckBoxTreeViewModel{ID=4,Header="item1.2", Children=null }};

            //var items2 = new List<CheckBoxTreeViewModel> { new CheckBoxTreeViewModel{ID=5,Header="item2.1", Children=items4 },
            //    new CheckBoxTreeViewModel{ID=5,Header="item2.2", Children=null }};

            //Items = new ObservableCollection<CheckBoxTreeViewModel> { new CheckBoxTreeViewModel{ID=1,Header="item1", Children=items1 },
            //    new CheckBoxTreeViewModel{ID=2,Header="item2", Children=items2 } };
        }
    }


    enum level
    {
        BarCode,
        Project
    }
}
