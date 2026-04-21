using Microsoft.DwayneNeed.Win32.User32;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;

namespace LCD.View
{
    /// <summary>
    /// 机台调试
    /// </summary>
    public partial class MoveDebugView : Window
    {
        private MainWindow mainWindow;
        private Thread thread;
        private bool is_running=false;
        public MoveDebugView(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private bool check_input(System.Windows.Controls.TextBox text,string name)
        {
            if(string.IsNullOrEmpty(text.Text))
            {
                return true;
            }
            double val = 0;
            try
            {
                val = double.Parse(text.Text);
            }
            catch {
                System.Windows.MessageBox.Show("请输入正确的" + name);
                text.Focus();
                return false;
            }
            if(val <0)
            {
                System.Windows.MessageBox.Show("请输入正确的" + name);
                text.Focus();
                return false;
            }
            return true;
        }

        private bool check_uv(System.Windows.Controls.TextBox text, string name)
        {
            if (string.IsNullOrEmpty(text.Text))
            {
                return true;
            }
            double val = 0;
            try
            {
                val = double.Parse(text.Text);
            }
            catch
            {
                System.Windows.MessageBox.Show("请输入正确的" + name);
                text.Focus();
                return false;
            }           
            return true;
        }

        //运动到指定位置
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.Flag == false)
            {
                System.Windows.MessageBox.Show("请设置相对原点");
                return;
            }
            if (mainWindow.IszERO == false)
            {
                mainWindow.show_motor_zero_msg();
                return;
            }
            //判断一下输入参数
            if(check_input(xval,"X轴")==false)
            {
                return;
            }
            if (check_input(yval, "Y轴") == false)
            {
                return;
            }
            if (check_input(zval, "Z轴") == false)
            {
                return;
            }
            if (check_uv(uval, "U轴") == false)
            {
                return;
            }
            if (check_uv(vval, "V轴") == false)
            {
                return;
            }

            double dx = double.TryParse(xval.Text, out dx) ? dx : 0.0;
            double dy = double.TryParse(yval.Text, out dy) ? dy : 0.0;
            double dz = double.TryParse(zval.Text, out dz) ? dz : 0.0;
            double du = double.TryParse(uval.Text, out du) ? du : 0.0;
            double dv = double.TryParse(vval.Text, out dv) ? dv : 0.0;
            dxyzuv dxyzuv = new dxyzuv();
            dxyzuv.dx = dx;
            dxyzuv.dy = dy;
            dxyzuv.dz = dz;
            dxyzuv.du = du;
            dxyzuv.dv = dv;
            //执行运动
            this.IsEnabled = false;
            thread = new Thread(work);
            thread.Start(dxyzuv);
        }

        private void work(Object obj)
        {
            dxyzuv dxyzuv = (dxyzuv)obj;           
            double dball =  0.0;
            Project.WriteLog("开始运行到X："+dxyzuv.dx+",Y:"+dxyzuv.dy+",Z:"+dxyzuv.dz+",U:"+dxyzuv.du+",V:"+dxyzuv.dv);
            mainWindow.pctrl.OnMove2Point(dxyzuv.dx, dxyzuv.dy+Project.Yorg, dxyzuv.dz, dxyzuv.du, dxyzuv.dv, dball,false);
            //double dx = 0, dy = 0, dz = 0, du = 0, dv = 0;
            //mainWindow.mvctrl.UpdateCurAbsPos(ref dx, ref dy, ref dz, ref du, ref dv, ref dball);//更新五轴位置
            //double angle = Project.cfg.Angle;
            ////走绝对定位
            //mainWindow.mvctrl.Move2Points(
            //    dxyzuv.dx - (double)Project.Xorg,
            //    dxyzuv.dy - (double)Project.Yorg,
            //    dz - (double)Project.Zorg,
            //    angle - (double)Project.Uorg,
            //     dv,
            //    dball - (double)Project.Ballorg, false);
            //Project.WriteLog("已经运行到指定位置");
            this.Dispatcher.BeginInvoke(new Action(() => { this.IsEnabled = true; }));
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if(this.IsEnabled == false)
            {
                MessageBoxResult dr = System.Windows.MessageBox.Show("运动轴正在运行，确定要退出吗","提示",MessageBoxButton.YesNo);
                if (dr == MessageBoxResult.Yes)
                {
                    try
                    {
                        thread?.Abort();
                    }
                    catch
                    {

                    }
                    this.Close();
                }                
            }
            else
            {
                this.Close();
            }
        }
    }

    public class dxyzuv
    {
        public double dx { get; set; }
        public double dy { get; set; }
        public double dz { get; set; }
        public double du { get; set; }
        public double dv { get; set; }
    }
}
