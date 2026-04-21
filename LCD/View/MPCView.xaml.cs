using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using LCD.Data;
using VisionCore;

namespace LCD.View
{
    /// <summary>
    /// MPC机台运动设置
    /// </summary>
    public partial class MPCView : Window
    {
        listMPCInfo MPCInfo=new listMPCInfo();
        //ObservableCollection<MPCInfo> mPCInfos = new ObservableCollection<MPCInfo>();

        //MPCInfo mpc = new MPCInfo();
        public MPCView()
        {
            InitializeComponent();

            if (Project.cfg.Lang != 0)
            {
                uaxis.Items.Clear();
                uaxis.Items.Add("Not reverse");
                uaxis.Items.Add("Reverse");
                uaxis.SelectedIndex = 0;

                vaxis.Items.Clear();
                vaxis.Items.Add("Not reverse");
                vaxis.Items.Add("Reverse");
                vaxis.SelectedIndex = 0;
            }

            Data2UI();//初始化mpc
            this.DataContext = MPCInfo;

            dataGrid.ItemsSource = MPCInfo.mPCInfos;
        }

        public void Data2UI()
        {
            LightDectPort.Text = "IN" + Project.cfg.LightScreenSignal;
            MPCInfo.Signal1Enable = Project.cfg.Signal1Enable;
            MPCInfo.Signal2Enable = Project.cfg.Signal2Enable;

            MPCInfo.Angle=Project.cfg.Angle ;
            MPCInfo.EQType = Project.cfg.EQType;
            MPCInfo.ZSeft= Project.cfg.ZSeft ;
            MPCInfo.USeftMax = Project.cfg.USeftMax;
            MPCInfo.UProductH = Project.cfg.UProductH;
            MPCInfo.USeftMin = Project.cfg.USeftMin;
            MPCInfo.MoveEndDelay = Project.cfg.AxiesDoneDelay;
			MPCInfo.UMaxAngle = Project.cfg.UMaxAngle ;
            MPCInfo.VMaxAngle = Project.cfg.VMaxAngle ;
            MPCInfo.UReverse = Project.cfg.UReverse ;
            MPCInfo.VReverse = Project.cfg.VReverse ;
            bool Tempx=false;
            if (Project.cfg.ax_x.direction == -1)
            {
                Tempx = true;
            }
            bool Tempy = false;
            if (Project.cfg.ax_y.direction == -1)
            {
                Tempy = true;
            }
            bool Tempz = false;
            if (Project.cfg.ax_z.direction == -1)
            {
                Tempz = true;
            }
            bool Tempu = false;
            if (Project.cfg.ax_u.direction == -1)
            {
                Tempu = true;
            }
            bool Tempv = false;
            if (Project.cfg.ax_v.direction == -1)
            {
                Tempv = true;
            }
            bool Tempball = false;
            if (Project.cfg.ax_ball.direction == -1)
            {
                Tempball = true;
            }
            MPCInfo.mPCInfos.Clear();
            MPCInfo.mPCInfos.Add(new MPCInfo
            {
                Name = AXiesName.X轴.ToString(),
                IsChecked = Project.cfg.ax_x.IsEnable,
                HomeSpend = Project.cfg.ax_x.HomeSpend,
                Speed = Project.cfg.ax_x.StepsPerMM,
                ID = Project.cfg.ax_x.value,
                Isnegative = Tempx,
                IsinterpolateCheck = Project.cfg.ax_x.IsSecondValue,
                interpolated = Project.cfg.ax_x.secondvalue,
                IsXinterpolate = Project.cfg.ax_x.IsSecondValue,
                center = Project.cfg.XCenter,
                SendIndex = Project.cfg.ax_x.SpendIndex,
                SpeedLow = Project.cfg.ax_x.SpeedLow,
                SpeedHigh = Project.cfg.ax_x.SpeedFast,
                SpeedAc = Project.cfg.ax_x.acSpeed,
                LowerLimit = Project.cfg.ax_x.LowerLimit / Project.cfg.ax_x.StepsPerMM,
                UpperLimit = Project.cfg.ax_x.UpperLimit / (long)Project.cfg.ax_x.StepsPerMM,
                AlarmEnable = Project.cfg.ax_x.AlarmEnable,
                BackLash = Project.cfg.ax_x.BackLash,
                CompensationDirection = Project.cfg.ax_x.CompensationDirection
            }) ;// 
            MPCInfo.mPCInfos.Add(new MPCInfo
            {
                Name = AXiesName.Y轴.ToString(),
                IsChecked = Project.cfg.ax_y.IsEnable,
                HomeSpend = Project.cfg.ax_y.HomeSpend,
                Speed = Project.cfg.ax_y.StepsPerMM,
                ID = Project.cfg.ax_y.value,
                Isnegative = Tempy,
                IsinterpolateCheck = Project.cfg.ax_y.IsSecondValue,
                interpolated = Project.cfg.ax_y.secondvalue,
                IsXinterpolate = Project.cfg.ax_y.IsSecondValue,
                center = Project.cfg.YCenter,
                SendIndex = Project.cfg.ax_y.SpendIndex,
                SpeedLow = Project.cfg.ax_y.SpeedLow,
                SpeedHigh = Project.cfg.ax_y.SpeedFast,
                SpeedAc = Project.cfg.ax_y.acSpeed,
                LowerLimit = Project.cfg.ax_y.LowerLimit / Project.cfg.ax_y.StepsPerMM,
                UpperLimit = Project.cfg.ax_y.UpperLimit      / (long)Project.cfg.ax_y.StepsPerMM,
                AlarmEnable = Project.cfg.ax_y.AlarmEnable,
                BackLash = Project.cfg.ax_y.BackLash,
                CompensationDirection = Project.cfg.ax_y.CompensationDirection
            });//
            MPCInfo.mPCInfos.Add(new MPCInfo
            {
                Name = AXiesName.Z轴.ToString(),
                IsChecked = Project.cfg.ax_z.IsEnable,
                HomeSpend = Project.cfg.ax_z.HomeSpend,
                Speed = Project.cfg.ax_z.StepsPerMM,
                ID = Project.cfg.ax_z.value,
                Isnegative = Tempz,
                IsinterpolateCheck = Project.cfg.ax_z.IsSecondValue,
                interpolated = Project.cfg.ax_z.secondvalue,
                IsXinterpolate = Project.cfg.ax_z.IsSecondValue,
                center = Project.cfg.ZCenter,
                SendIndex = Project.cfg.ax_z.SpendIndex,
                SpeedLow = Project.cfg.ax_z.SpeedLow,
                SpeedHigh = Project.cfg.ax_z.SpeedFast,
                SpeedAc = Project.cfg.ax_z.acSpeed,
                LowerLimit = Project.cfg.ax_z.LowerLimit / Project.cfg.ax_z.StepsPerMM,
                UpperLimit = Project.cfg.ax_z.UpperLimit / (long)Project.cfg.ax_z.StepsPerMM,
                AlarmEnable = Project.cfg.ax_z.AlarmEnable,
                BackLash = Project.cfg.ax_z.BackLash,
                CompensationDirection = Project.cfg.ax_z.CompensationDirection
            });
            MPCInfo.mPCInfos.Add(new MPCInfo
            {
                Name = AXiesName.U轴.ToString(),
                IsChecked = Project.cfg.ax_u.IsEnable,
                HomeSpend = Project.cfg.ax_u.HomeSpend,
                Speed = Project.cfg.ax_u.StepsPerMM,
                ID = Project.cfg.ax_u.value,
                Isnegative = Tempu,
                IsinterpolateCheck = Project.cfg.ax_u.IsSecondValue,
                interpolated = Project.cfg.ax_u.secondvalue,
                IsXinterpolate = Project.cfg.ax_u.IsSecondValue,
                center = Project.cfg.UCenter,
                SendIndex = Project.cfg.ax_u.SpendIndex,
                SpeedLow = Project.cfg.ax_u.SpeedLow,
                SpeedHigh = Project.cfg.ax_u.SpeedFast,
                SpeedAc = Project.cfg.ax_u.acSpeed,
                LowerLimit = Project.cfg.ax_u.LowerLimit / Project.cfg.ax_u.StepsPerMM,
                UpperLimit = Project.cfg.ax_u.UpperLimit    / (long)Project.cfg.ax_u.StepsPerMM,
                AlarmEnable = Project.cfg.ax_u.AlarmEnable,
                BackLash = Project.cfg.ax_u.BackLash,
                CompensationDirection = Project.cfg.ax_u.CompensationDirection
            });
            MPCInfo.mPCInfos.Add(new MPCInfo
            {
                Name = AXiesName.V轴.ToString(),
                IsChecked = Project.cfg.ax_v.IsEnable,
                Speed = Project.cfg.ax_v.StepsPerMM,
                HomeSpend = Project.cfg.ax_v.HomeSpend,
                ID = Project.cfg.ax_v.value,
                Isnegative = Tempv,
                IsinterpolateCheck = Project.cfg.ax_v.IsSecondValue,
                interpolated = Project.cfg.ax_v.secondvalue,
                IsXinterpolate = Project.cfg.ax_v.IsSecondValue,
                center = Project.cfg.VCenter,
                SendIndex = Project.cfg.ax_v.SpendIndex,
                SpeedLow = Project.cfg.ax_v.SpeedLow,
                SpeedHigh = Project.cfg.ax_v.SpeedFast,
                SpeedAc = Project.cfg.ax_v.acSpeed,
                LowerLimit = Project.cfg.ax_v.LowerLimit / Project.cfg.ax_v.StepsPerMM,
                UpperLimit = Project.cfg.ax_v.UpperLimit  / (long)Project.cfg.ax_v.StepsPerMM,
                AlarmEnable = Project.cfg.ax_v.AlarmEnable,
                BackLash = Project.cfg.ax_v.BackLash,
                CompensationDirection = Project.cfg.ax_v.CompensationDirection
            });
            MPCInfo.mPCInfos.Add(new MPCInfo
            {
                Name = AXiesName.Ball轴.ToString(),
                IsChecked = Project.cfg.ax_ball.IsEnable,
                Speed = Project.cfg.ax_ball.StepsPerMM,
                HomeSpend = Project.cfg.ax_ball.HomeSpend,
                ID = Project.cfg.ax_ball.value,
                Isnegative = Tempball,
                IsinterpolateCheck = Project.cfg.ax_ball.IsSecondValue,
                interpolated = Project.cfg.ax_ball.secondvalue,
                IsXinterpolate = Project.cfg.ax_ball.IsSecondValue,
                center = Project.cfg.BallCenter,
                SendIndex = Project.cfg.ax_ball.SpendIndex,
                SpeedLow = Project.cfg.ax_ball.SpeedLow,
                SpeedHigh = Project.cfg.ax_ball.SpeedFast,
                SpeedAc = Project.cfg.ax_ball.acSpeed,
                LowerLimit = Project.cfg.ax_ball.LowerLimit/ Project.cfg.ax_ball.StepsPerMM,
                UpperLimit = Project.cfg.ax_ball.UpperLimit/ (long)Project.cfg.ax_ball.StepsPerMM,
                AlarmEnable = Project.cfg.ax_ball.AlarmEnable,
                BackLash = Project.cfg.ax_ball.BackLash,
                CompensationDirection = Project.cfg.ax_ball.CompensationDirection
            });

            if(Project.cfg.ShowLab)
            {
                ShowLab.IsChecked = true;
            }
            else
            {
                ShowLab.IsChecked= false;
            }
        }
        /// <summary>
        /// UI转Data数据
        /// </summary>
        private void UI2Data()
        {


            Project.cfg.Signal1Enable = MPCInfo.Signal1Enable;
            Project.cfg.Signal2Enable = MPCInfo.Signal2Enable;

            Project.cfg.ax_x.Name = AXiesName.X轴;
            Project.cfg.ax_y.Name = AXiesName.Y轴;
            Project.cfg.ax_z.Name = AXiesName.Z轴;
            Project.cfg.ax_u.Name = AXiesName.U轴;
            Project.cfg.ax_v.Name = AXiesName.V轴;
            Project.cfg.ax_ball.Name = AXiesName.Ball轴;

            if (ShowLab.IsChecked == true)
            {
                Project.cfg.ShowLab = true;
            }
            else
            {
                Project.cfg.ShowLab = false;
            }

            Project.cfg.ax_x.IsEnable = MPCInfo.mPCInfos[0].IsChecked;

            if (MPCInfo.mPCInfos[0].IsChecked)
            {
                Project.cfg.ax_x.BackLash = MPCInfo.mPCInfos[0].BackLash;
                Project.cfg.ax_x.CompensationDirection = MPCInfo.mPCInfos[0].CompensationDirection;
                Project.cfg.ax_x.AlarmEnable = MPCInfo.mPCInfos[0].AlarmEnable;
                Project.cfg.Angle = MPCInfo.Angle;
                Project.cfg.ax_x.value = MPCInfo.mPCInfos[0].ID;
                Project.cfg.ax_x.StepsPerMM = MPCInfo.mPCInfos[0].Speed;                

                if (MPCInfo.mPCInfos[0].Isnegative) { Project.cfg.ax_x.direction = -1; }
                else { Project.cfg.ax_x.direction = 1; }

                Project.cfg.ax_x.IsSecondValue = MPCInfo.mPCInfos[0].IsinterpolateCheck; ;//MPCInfo.mPCInfos[0].IsXinterpolate;
                Project.cfg.ax_x.secondvalue = MPCInfo.mPCInfos[0].interpolated;

                Project.cfg.ax_x.SpendIndex = MPCInfo.mPCInfos[0].SendIndex;
                Project.cfg.ax_x.HomeSpend = MPCInfo.mPCInfos[0].HomeSpend;
                Project.cfg.ax_x.LowerLimit = MPCInfo.mPCInfos[0].LowerLimit * MPCInfo.mPCInfos[0].Speed;
                Project.cfg.XCenter = MPCInfo.mPCInfos[0].center;
                Project.cfg.ax_x.UpperLimit = MPCInfo.mPCInfos[0].UpperLimit * (long)MPCInfo.mPCInfos[0].Speed;

                Project.cfg.ax_x.SpeedLow = MPCInfo.mPCInfos[0].SpeedLow;
                Project.cfg.ax_x.homespeed = MPCInfo.mPCInfos[0].SpeedLow;
                Project.cfg.ax_x.SpeedFast = MPCInfo.mPCInfos[0].SpeedHigh;
                Project.cfg.ax_x.SpeedMedium = (MPCInfo.mPCInfos[0].SpeedHigh + MPCInfo.mPCInfos[0].SpeedLow) / 2;
                Project.cfg.ax_x.acSpeed = MPCInfo.mPCInfos[0].SpeedAc;
                Project.cfg.XCenter = MPCInfo.mPCInfos[0].center;
            }

            Project.cfg.ax_z.IsEnable = MPCInfo.mPCInfos[2].IsChecked;
            if (Project.cfg.ax_z.IsEnable)
            {
                Project.cfg.ax_z.BackLash = MPCInfo.mPCInfos[2].BackLash;
                Project.cfg.ax_z.CompensationDirection = MPCInfo.mPCInfos[2].CompensationDirection;
                Project.cfg.ax_z.AlarmEnable = MPCInfo.mPCInfos[2].AlarmEnable;
                Project.cfg.ax_z.SpendIndex = MPCInfo.mPCInfos[2].SendIndex;
                Project.cfg.ax_z.HomeSpend = MPCInfo.mPCInfos[2].HomeSpend;
                Project.cfg.ax_z.LowerLimit = MPCInfo.mPCInfos[2].LowerLimit * MPCInfo.mPCInfos[2].Speed;
                Project.cfg.ax_z.UpperLimit = MPCInfo.mPCInfos[2].UpperLimit * (long)MPCInfo.mPCInfos[2].Speed;
                Project.cfg.ZCenter = MPCInfo.mPCInfos[2].center;
                Project.cfg.ZCenter = MPCInfo.mPCInfos[2].center;
                Project.cfg.ax_z.value = MPCInfo.mPCInfos[2].ID;
                Project.cfg.ax_z.StepsPerMM = MPCInfo.mPCInfos[2].Speed;
                Project.cfg.ax_z.homespeed = MPCInfo.mPCInfos[2].SpeedLow;
                Project.cfg.ax_z.SpeedLow = MPCInfo.mPCInfos[2].SpeedLow;
                Project.cfg.ax_z.SpeedFast = MPCInfo.mPCInfos[2].SpeedHigh;
                Project.cfg.ax_z.SpeedMedium = (MPCInfo.mPCInfos[2].SpeedHigh + MPCInfo.mPCInfos[2].SpeedLow) / 2;
                Project.cfg.ax_z.acSpeed = MPCInfo.mPCInfos[2].SpeedAc;
                if (MPCInfo.mPCInfos[2].Isnegative) { Project.cfg.ax_z.direction = -1; }
                else { Project.cfg.ax_z.direction = 1; }
            }

            Project.cfg.ax_u.IsEnable = MPCInfo.mPCInfos[3].IsChecked;
            if (Project.cfg.ax_u.IsEnable)
            {
                Project.cfg.ax_u.BackLash = MPCInfo.mPCInfos[3].BackLash;
                Project.cfg.ax_u.CompensationDirection = MPCInfo.mPCInfos[3].CompensationDirection;
                Project.cfg.ax_u.AlarmEnable = MPCInfo.mPCInfos[3].AlarmEnable;
                Project.cfg.ax_u.SpendIndex = MPCInfo.mPCInfos[3].SendIndex;
                Project.cfg.ax_u.HomeSpend = MPCInfo.mPCInfos[3].HomeSpend;
                Project.cfg.ax_u.LowerLimit = MPCInfo.mPCInfos[3].LowerLimit * MPCInfo.mPCInfos[3].Speed;
                Project.cfg.ax_u.UpperLimit = MPCInfo.mPCInfos[3].UpperLimit * (long)MPCInfo.mPCInfos[3].Speed;
                Project.cfg.UCenter = MPCInfo.mPCInfos[3].center;
                Project.cfg.ax_u.value = MPCInfo.mPCInfos[3].ID;
                Project.cfg.ax_u.StepsPerMM = MPCInfo.mPCInfos[3].Speed;
                Project.cfg.ax_u.homespeed = MPCInfo.mPCInfos[3].SpeedLow;
                Project.cfg.ax_u.SpeedLow = MPCInfo.mPCInfos[3].SpeedLow;
                Project.cfg.ax_u.SpeedFast = MPCInfo.mPCInfos[3].SpeedHigh;
                Project.cfg.ax_u.SpeedMedium = (MPCInfo.mPCInfos[3].SpeedHigh + MPCInfo.mPCInfos[3].SpeedLow) / 2;
                Project.cfg.ax_u.acSpeed = MPCInfo.mPCInfos[3].SpeedAc;
                if (MPCInfo.mPCInfos[3].Isnegative) { Project.cfg.ax_u.direction = -1; }
                else { Project.cfg.ax_u.direction = 1; }
            }

            Project.cfg.ax_v.IsEnable = MPCInfo.mPCInfos[4].IsChecked;
            if (Project.cfg.ax_v.IsEnable)
            {
                Project.cfg.ax_v.BackLash = MPCInfo.mPCInfos[4].BackLash;
                Project.cfg.ax_v.CompensationDirection = MPCInfo.mPCInfos[4].CompensationDirection;
                Project.cfg.ax_v.AlarmEnable = MPCInfo.mPCInfos[4].AlarmEnable;
                Project.cfg.ax_v.SpendIndex = MPCInfo.mPCInfos[4].SendIndex;
                Project.cfg.ax_v.HomeSpend = MPCInfo.mPCInfos[4].HomeSpend;
                Project.cfg.ax_v.LowerLimit = MPCInfo.mPCInfos[4].LowerLimit * MPCInfo.mPCInfos[4].Speed;
                Project.cfg.ax_v.UpperLimit = MPCInfo.mPCInfos[4].UpperLimit * (long)MPCInfo.mPCInfos[4].Speed;
                Project.cfg.VCenter = MPCInfo.mPCInfos[4].center;
                Project.cfg.ax_v.value = MPCInfo.mPCInfos[4].ID;
                Project.cfg.ax_v.StepsPerMM = MPCInfo.mPCInfos[4].Speed;
                Project.cfg.ax_v.homespeed = MPCInfo.mPCInfos[4].SpeedLow;

                Project.cfg.ax_v.SpeedLow = MPCInfo.mPCInfos[4].SpeedLow;
                Project.cfg.ax_v.SpeedFast = MPCInfo.mPCInfos[4].SpeedHigh;
                Project.cfg.ax_v.SpeedMedium = (MPCInfo.mPCInfos[4].SpeedHigh + MPCInfo.mPCInfos[4].SpeedLow) / 2;
                Project.cfg.ax_v.acSpeed = MPCInfo.mPCInfos[4].SpeedAc;

                if (MPCInfo.mPCInfos[4].Isnegative) { Project.cfg.ax_v.direction = -1; }
                else { Project.cfg.ax_v.direction = 1; }
            }

            Project.cfg.ax_ball.IsEnable = MPCInfo.mPCInfos[5].IsChecked;
            if (Project.cfg.ax_ball.IsEnable)
            {
                Project.cfg.ax_ball.BackLash = MPCInfo.mPCInfos[5].BackLash;
                Project.cfg.ax_ball.CompensationDirection = MPCInfo.mPCInfos[5].CompensationDirection;
                Project.cfg.ax_ball.AlarmEnable = MPCInfo.mPCInfos[5].AlarmEnable;
                Project.cfg.ax_ball.SpendIndex = MPCInfo.mPCInfos[5].SendIndex;
                Project.cfg.ax_ball.HomeSpend = MPCInfo.mPCInfos[5].HomeSpend;
                Project.cfg.ax_ball.LowerLimit = MPCInfo.mPCInfos[5].LowerLimit * MPCInfo.mPCInfos[5].Speed;
                Project.cfg.ax_ball.UpperLimit = MPCInfo.mPCInfos[5].UpperLimit * (long)MPCInfo.mPCInfos[5].Speed;
                Project.cfg.BallCenter = MPCInfo.mPCInfos[5].center;
                Project.cfg.ax_ball.value = MPCInfo.mPCInfos[5].ID;
                Project.cfg.ax_ball.StepsPerMM = MPCInfo.mPCInfos[5].Speed;

                Project.cfg.ax_ball.homespeed = MPCInfo.mPCInfos[5].SpeedAc;
                Project.cfg.ax_ball.SpeedLow = MPCInfo.mPCInfos[5].SpeedLow;
                Project.cfg.ax_ball.SpeedFast = MPCInfo.mPCInfos[5].SpeedHigh;
                Project.cfg.ax_ball.SpeedMedium = (MPCInfo.mPCInfos[5].SpeedHigh + MPCInfo.mPCInfos[5].SpeedLow) / 2;
                Project.cfg.ax_ball.acSpeed = MPCInfo.mPCInfos[5].SpeedAc;

                if (MPCInfo.mPCInfos[5].Isnegative) { Project.cfg.ax_ball.direction = -1; }
                else { Project.cfg.ax_ball.direction = 1; }
            }


            Project.cfg.ax_y.IsEnable = MPCInfo.mPCInfos[1].IsChecked;
            if (Project.cfg.ax_y.IsEnable)
            {
                Project.cfg.ax_y.BackLash = MPCInfo.mPCInfos[1].BackLash;
                Project.cfg.ax_y.CompensationDirection = MPCInfo.mPCInfos[1].CompensationDirection;
                Project.cfg.ax_y.AlarmEnable = MPCInfo.mPCInfos[1].AlarmEnable;
                Project.cfg.ax_y.SpendIndex = MPCInfo.mPCInfos[1].SendIndex;
                Project.cfg.ax_y.HomeSpend = MPCInfo.mPCInfos[1].HomeSpend;
                Project.cfg.ax_y.LowerLimit = MPCInfo.mPCInfos[1].LowerLimit * MPCInfo.mPCInfos[1].Speed;
                Project.cfg.ax_y.UpperLimit = MPCInfo.mPCInfos[1].UpperLimit * (long)MPCInfo.mPCInfos[1].Speed;
                Project.cfg.YCenter = MPCInfo.mPCInfos[1].center;
                Project.cfg.ax_y.IsSecondValue = MPCInfo.mPCInfos[1].IsinterpolateCheck;//MPCInfo.mPCInfos[1].IsXinterpolate;
                Project.cfg.ax_y.secondvalue = MPCInfo.mPCInfos[1].interpolated;
                Project.cfg.ax_y.value = MPCInfo.mPCInfos[1].ID;
                Project.cfg.ax_y.StepsPerMM = MPCInfo.mPCInfos[1].Speed;
                Project.cfg.ax_y.SpeedLow = MPCInfo.mPCInfos[1].SpeedLow;
                Project.cfg.ax_y.homespeed = MPCInfo.mPCInfos[1].SpeedLow;
                Project.cfg.ax_y.SpeedFast = MPCInfo.mPCInfos[1].SpeedHigh;
                Project.cfg.ax_y.SpeedMedium = (MPCInfo.mPCInfos[1].SpeedHigh + MPCInfo.mPCInfos[1].SpeedLow) / 2;
                Project.cfg.ax_y.acSpeed = MPCInfo.mPCInfos[1].SpeedAc;
                //Project.cfg.ax_y.IsSecondValue = MPCInfo.mPCInfos[1].IsXinterpolate;
                Project.cfg.ax_y.secondvalue = MPCInfo.mPCInfos[1].interpolated;
                if (MPCInfo.mPCInfos[1].Isnegative) { Project.cfg.ax_y.direction = -1; }
                else { Project.cfg.ax_y.direction = 1; }
                Project.cfg.YCenter = MPCInfo.mPCInfos[1].center;
            }


            Project.cfg.EQType = MPCInfo.EQType;

            Project.cfg.ZSeft = MPCInfo.ZSeft;
            Project.cfg.USeftMax = MPCInfo.USeftMax;
            Project.cfg.UProductH = MPCInfo.UProductH;
            
            Project.cfg.USeftMin = MPCInfo.USeftMin;
            Project.cfg.UMaxAngle = MPCInfo.UMaxAngle;
            Project.cfg.VMaxAngle = MPCInfo.VMaxAngle;
            Project.cfg.UReverse = MPCInfo.UReverse;
            Project.cfg.VReverse = MPCInfo.VReverse;


            //速度选择
            if (MPCInfo.mPCInfos[0].IsChecked == true)
            { Project.cfg.movSpeed = Ctrl.MovCtrl.EnumMoveSpeed.SLOW; }
            else if (MPCInfo.mPCInfos[0].IsChecked == true)
            { Project.cfg.movSpeed = Ctrl.MovCtrl.EnumMoveSpeed.MEDIUM; }
            else if (MPCInfo.mPCInfos[0].IsChecked == true)
            { Project.cfg.movSpeed = Ctrl.MovCtrl.EnumMoveSpeed.HIGH; }
            else { }

            //速度选择Y
            if (MPCInfo.mPCInfos[1].IsChecked == true)
            { Project.cfg.movSpeed_Y = Ctrl.MovCtrl.EnumMoveSpeed.SLOW; }
            else if (MPCInfo.mPCInfos[1].IsChecked == true)
            { Project.cfg.movSpeed_Y = Ctrl.MovCtrl.EnumMoveSpeed.MEDIUM; }
            else if (MPCInfo.mPCInfos[1].IsChecked == true)
            { Project.cfg.movSpeed_Y = Ctrl.MovCtrl.EnumMoveSpeed.HIGH; }
            else { }


            //速度选择Z
            if (MPCInfo.mPCInfos[2].IsChecked == true)
            { Project.cfg.movSpeed_Z = Ctrl.MovCtrl.EnumMoveSpeed.SLOW; }
            else if (MPCInfo.mPCInfos[2].IsChecked == true)
            { Project.cfg.movSpeed_Z = Ctrl.MovCtrl.EnumMoveSpeed.MEDIUM; }
            else if (MPCInfo.mPCInfos[2].IsChecked == true)
            { Project.cfg.movSpeed_Z = Ctrl.MovCtrl.EnumMoveSpeed.HIGH; }
            else { }

            //速度选择
            if (MPCInfo.mPCInfos[3].IsChecked == true)
            { Project.cfg.movSpeed_U = Ctrl.MovCtrl.EnumMoveSpeed.SLOW; }
            else if (MPCInfo.mPCInfos[3].IsChecked == true)
            { Project.cfg.movSpeed_U = Ctrl.MovCtrl.EnumMoveSpeed.MEDIUM; }
            else if (MPCInfo.mPCInfos[3].IsChecked == true)
            { Project.cfg.movSpeed_U = Ctrl.MovCtrl.EnumMoveSpeed.HIGH; }
            else { }

            //速度选择
            if (MPCInfo.mPCInfos[4].IsChecked == true)
            { Project.cfg.movSpeed_V = Ctrl.MovCtrl.EnumMoveSpeed.SLOW; }
            else if (MPCInfo.mPCInfos[4].IsChecked == true)
            { Project.cfg.movSpeed_V = Ctrl.MovCtrl.EnumMoveSpeed.MEDIUM; }
            else if (MPCInfo.mPCInfos[4].IsChecked == true)
            { Project.cfg.movSpeed_V = Ctrl.MovCtrl.EnumMoveSpeed.HIGH; }
            else { }

            //速度选择
            if (MPCInfo.mPCInfos[5].IsChecked == true)
            { Project.cfg.movSpeed_Ball = Ctrl.MovCtrl.EnumMoveSpeed.SLOW; }
            else if (MPCInfo.mPCInfos[5].IsChecked == true)
            { Project.cfg.movSpeed_Ball = Ctrl.MovCtrl.EnumMoveSpeed.MEDIUM; }
            else if (MPCInfo.mPCInfos[5].IsChecked == true)
            { Project.cfg.movSpeed_Ball = Ctrl.MovCtrl.EnumMoveSpeed.HIGH; }
            else { }

            Project.cfg.AxiesDoneDelay = MPCInfo.MoveEndDelay;
        }

        //X选择
        private void OnBnClickedCHK_X(object sender, RoutedEventArgs e)
        {
            //mpc.IsXnegative=

        }

        //X选择
        private void OnBnClickedCHK_Y(object sender, RoutedEventArgs e)
        {

        }

        //X选择
        private void OnBnClickedCHK_Z(object sender, RoutedEventArgs e)
        {

        }

        //X选择
        private void OnBnClickedCHK_U(object sender, RoutedEventArgs e)
        {

        }


        //X选择
        private void OnBnClickedCHK_V(object sender, RoutedEventArgs e)
        {

        }

        private void OnBnClickedCHK_Ball(object sender, RoutedEventArgs e)
        {

        }

     

        //选择CHK执行选择事件
        

        private void OnBnClickedEnsure(object sender, RoutedEventArgs e)
        {
            //数据检查
            for(int i=0;i< MPCInfo.mPCInfos.Count; i++)
            {
                if (MPCInfo.mPCInfos[i].IsChecked)
                {
                    if(MPCInfo.mPCInfos[i].Speed <=0)
                    {
                        MessageBox.Show(MPCInfo.mPCInfos[i].Name+"轴设置错误，速度不能为0");
                        return;
                    }
                }
            }
            if(LightDectPort.Text.Length ==0)
            {
                MessageBox.Show("安全光栅端口不能为空");
                LightDectPort.Focus();
                return;
            }
            if (LightDectPort.Text.StartsWith("IN")==false)
            {
                MessageBox.Show("安全光栅端口配置应该以IN开头");
                LightDectPort.Focus();
                return;
            }
            int port = 0;
            try
            {
                port = int.Parse(LightDectPort.Text.Substring(2));
            }
            catch
            {
                MessageBox.Show("安全光栅端口配置格式是IN+数字");
                LightDectPort.Focus();
                return;
            }
            if(port <= 0)
            {
                MessageBox.Show("安全光栅端口不能小于等于0");
                LightDectPort.Focus();
                return;
            }
            Project.cfg.LightScreenSignal = port;
            //填充数据
            UI2Data();
            Project.SaveConfig("Config.xml");
            this.Close();
        }
    }
    public class listMPCInfo: ViewBase
    {

        private bool _Signal1Enable;
        /// <summary>
        /// 到位信号1
        /// </summary>
        public bool Signal1Enable
        {
            get { return _Signal1Enable; }
            set { _Signal1Enable = value; }
        }
        private bool _Signal2Enable;
        /// <summary>
        /// 到位信号2
        /// </summary>
        public bool Signal2Enable
        {
            get { return _Signal2Enable; }
            set { _Signal2Enable = value; }
        }



        private double _Angle;

        public double Angle
        {
            get { return _Angle; }
            set { _Angle = value;
                OnPropertyChanged();
            }
        }

        private uint _MoveEndDelay;
        public uint MoveEndDelay
        {
            get { return _MoveEndDelay; }
            set
            {
                _MoveEndDelay = value;
                OnPropertyChanged();
            }
        }


        private int _EQType;
        public int EQType { get { return _EQType; } set { _EQType = value;OnPropertyChanged(); } }

        private int _ZSeft;

        public int ZSeft
        {
            get { return _ZSeft; }
            set { _ZSeft = value;OnPropertyChanged(); }
        }
        private int _USeftMin;

        public int USeftMin
        {
            get { return _USeftMin; }
            set { _USeftMin = value;OnPropertyChanged(); }
        }

        private int _USeftMax;

        public int USeftMax
        {
            get { return _USeftMax; }
            set { _USeftMax = value; OnPropertyChanged(); }
        }

        private double _UMaxAngle;


        public double UMaxAngle
        {
            get { return _UMaxAngle; }
            set { _UMaxAngle = value; OnPropertyChanged(); }
        }


        private double _UProductH;


        public double UProductH
        {
            get { return _UProductH; }
            set { _UProductH = value; OnPropertyChanged(); }
        }



        private double _VMaxAngle;

        public double VMaxAngle
        {
            get { return _VMaxAngle; }
            set { _VMaxAngle = value; OnPropertyChanged(); }
        }

        private int _UReverse;

        public int UReverse
        {
            get { return _UReverse; }
            set { _UReverse = value; OnPropertyChanged(); }
        }

        private int _VReverse;

        public int VReverse
        {
            get { return _VReverse; }
            set { _VReverse = value; OnPropertyChanged(); }
        }
        //public int USeftMin;
        //public int USeftMax;

        //public ObservableCollection<MPCInfo> mPCInfos = new ObservableCollection<MPCInfo>();
        private ObservableCollection<MPCInfo> _mPCInfos=new ObservableCollection<MPCInfo>();

        public ObservableCollection<MPCInfo> mPCInfos
        {
            get { return _mPCInfos; }
            set { _mPCInfos = value;OnPropertyChanged(); }
        }

    }

    public class MPCInfo:ViewBase
    {
        //电机轴号
        private String  name;

        public String Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }



        private bool _IsChecked;
        /// <summary>
        /// 电机选中
        /// </summary>
        public bool IsChecked {
            get { return _IsChecked; } set { _IsChecked = value;OnPropertyChanged(); } }

        //脉冲当量


        private double _Speed;
        /// <summary>
        /// 脉冲/mm
        /// </summary>
        public double Speed
        {
            get { return _Speed; }
            set
            {
                _Speed = value; OnPropertyChanged();
            }
        }

        private bool backlash;
        /// <summary>
        /// 回零
        /// </summary>
        public bool BackLash
        {
            get { return backlash; }
            set { backlash = value; }
        }

        private bool compensationDirection;
        /// <summary>
        /// 回零
        /// </summary>
        public bool CompensationDirection
        {
            get { return compensationDirection; }
            set { compensationDirection = value; }
        }




        private bool _AlarmEnable;



        public bool AlarmEnable
        {
            get { return _AlarmEnable; }
            set { _AlarmEnable = value; OnPropertyChanged(); }
        }


        //轴号

        private int _ID;
        public int ID { get { return _ID; } set
            {
                _ID = value; OnPropertyChanged();
            } }


        //反向归零
        private bool _Isnegative;
        public bool Isnegative { get
            {
                return _Isnegative; 
            } set { _Isnegative = value; OnPropertyChanged(); } }

        private bool _IsinterpolateCheck;
        public bool IsinterpolateCheck { get {return _IsinterpolateCheck; } set {
                _IsinterpolateCheck = value;OnPropertyChanged();
            } }

        private int _interpolated;
        /// <summary>
        /// 差补
        /// </summary>
        public int interpolated { get { return _interpolated; } set { _interpolated = value;OnPropertyChanged(); } }


        private bool _IsXinterpolate;
        public bool IsXinterpolate { get { return _IsXinterpolate; } set { _IsXinterpolate = value;OnPropertyChanged(); } }


        private double _HomeSpend;

        public double HomeSpend
        {
            get { return _HomeSpend; }
            set { _HomeSpend = value;OnPropertyChanged(); }
        }


        private double _LowerLimit;

        public double LowerLimit
        {
            get { return _LowerLimit; }
            set { _LowerLimit = value;OnPropertyChanged(); }
        }

        private long _UpperLimit;

        public long UpperLimit
        {
            get { return _UpperLimit; }
            set { _UpperLimit = value;OnPropertyChanged(); }
        }


        /// <summary>
        /// 中心点
        /// </summary>
        public double center { get; set; }




        //速度


        public int SendIndex { get; set; }




        //初始速度


        public double SpeedLow { get; set; }
        public double SpeedHigh { get; set; }
        public double SpeedAc { get; set; }
    


    }
}
