/*********************************************
 * 运动控制类，负责系统中所有运动控制轴的初始化 
 * 测试过程中XY两轴要有同时运动的效果
 ***/
using LCD.Data;
using LCD.Dll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LCD.Ctrl
{
    /// <summary>
    /// MPC08板卡
    /// </summary>
    [Category("运动控制"), Description("MPC08板卡"), DisplayName("MPC08板卡")]
    public class MovCtrl
    {
        private int board_cnt=0;
        public int board_cnt1 = 1;
        long limitValue = 100000000;
        long maxVal = 4294967296;
        public static MovCtrl m_movecontrol = null;
        public static MovCtrl GetInstance()
        {
            if (m_movecontrol == null) { m_movecontrol = new MovCtrl(); }
            return m_movecontrol;
        }
        public void Stop(MovCtrol movCtrol)
        {
            switch (movCtrol)
            {
                case MovCtrol.X:
                    int XSpeed = (int)(Project.cfg.ax_x.SpeedMedium * Project.cfg.ax_x.StepsPerMM);
                    MPC08EDLL.set_profile(Project.cfg.ax_x.value, 0.2 * XSpeed, XSpeed, Project.cfg.ax_x.acSpeed);//设置速度
                    MPC08EDLL.decel_stop(Project.cfg.ax_x.value);
                    break;
                case MovCtrol.Y:
                    int YSpeed = (int)(Project.cfg.ax_y.SpeedMedium * Project.cfg.ax_y.StepsPerMM);
                    MPC08EDLL.set_profile(Project.cfg.ax_y.value, 0.2 * YSpeed, YSpeed, Project.cfg.ax_y.acSpeed);//设置速度
                    MPC08EDLL.decel_stop(Project.cfg.ax_y.value);
                    break;
                case MovCtrol.Z:
                    int nSpeed = (int)(Project.cfg.ax_z.SpeedMedium * Project.cfg.ax_z.StepsPerMM);
                    MPC08EDLL.set_profile(Project.cfg.ax_z.value, 0.2* nSpeed, nSpeed, Project.cfg.ax_z.acSpeed);//设置速度
                    MPC08EDLL.decel_stop(Project.cfg.ax_z.value);
                    break;
                case MovCtrol.U:
                    int nSpeed1 = (int)(Project.cfg.ax_u.SpeedMedium * Project.cfg.ax_u.StepsPerMM);
                    MPC08EDLL.set_profile(Project.cfg.ax_u.value, 0.2 * nSpeed1, nSpeed1, Project.cfg.ax_u.acSpeed);//设置速度
                    MPC08EDLL.decel_stop(Project.cfg.ax_u.value);
                    break;
                case MovCtrol.V:
                    int VSpeed = (int)(Project.cfg.ax_v.SpeedMedium * Project.cfg.ax_v.StepsPerMM);
                    MPC08EDLL.set_profile(Project.cfg.ax_v.value, 0.2 * VSpeed, VSpeed, Project.cfg.ax_v.acSpeed);//设置速度
                    MPC08EDLL.decel_stop(Project.cfg.ax_v.value);
                    break;
            }
            
        }
        public void StopAll()
        {
            if (Project.cfg.ax_x.IsEnable)
            {
                Axies axe = Project.cfg.ax_x;
                if (axe.IsSecondValue)
                {
                    MPC08EDLL.decel_stop2(axe.value, axe.secondvalue);
                }
                else
                {
                    MPC08EDLL.decel_stop(axe.value);
                }
            }

            if (Project.cfg.ax_y.IsEnable)
            {
                Axies axe = Project.cfg.ax_y;
                if (axe.IsSecondValue)
                {
                    MPC08EDLL.decel_stop2(axe.value, axe.secondvalue);
                }
                else
                {
                    MPC08EDLL.decel_stop(axe.value);
                }
            }
            if (Project.cfg.ax_z.IsEnable)
            {
                Axies axe = Project.cfg.ax_z;
                if (axe.IsSecondValue)
                {
                    MPC08EDLL.decel_stop2(axe.value, axe.secondvalue);
                }
                else
                {
                    MPC08EDLL.decel_stop(axe.value);
                }
            }


            if (Project.cfg.ax_u.IsEnable)
            {
                Axies axe = Project.cfg.ax_u;
                MPC08EDLL.decel_stop(axe.value);
            }
            if (Project.cfg.ax_v.IsEnable)
            {
                Axies axe = Project.cfg.ax_v;
                MPC08EDLL.decel_stop(axe.value);
            }
            if (Project.cfg.ax_ball.IsEnable)
            {
                Axies axe = Project.cfg.ax_ball;
                MPC08EDLL.decel_stop(axe.value);
            }
        }

        private MovCtrl()
        {
            //int ret = MPC08EDLL.auto_set();
            //ret = MPC08EDLL.init_board();//
            //Project.WriteLog("auto_set=" + ret.ToString());
            //Project.WriteLog("init_board=" + ret.ToString());
            Init();
        }

        public void Init()
        {
            //初始化
            int axe_cnt = MPC08EDLL.auto_set();
            if (axe_cnt < 0)
            {
                Project.WriteLog("MPC08初始化失败:未识别到板卡");
                return;
            }

            if (axe_cnt == 0)
            {
                Project.WriteLog("MPC08初始化失败:未识别到轴");
                return;
            }
           
            board_cnt = MPC08EDLL.init_board();
            if (board_cnt < 0)
            {
                Project.WriteLog("MPC08初始化失败:板卡初始化失败");
                return;
            }

            //设置速度
            //SetSpeed();

            //归零使能
            //SetHomeSignal();

            Project.WriteLog("运动控制卡初始化完毕！");
        }
        private void SetSpeed()
        {
            if (Project.cfg.ax_x.IsEnable)
            {
                Axies axe = Project.cfg.ax_x;
                MPC08EDLL.set_profile(axe.value,
                    axe.SpeedLow * axe.StepsPerMM,
                    axe.HomeSpend * axe.StepsPerMM,
                    axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }

            if (Project.cfg.ax_y.IsEnable)
            {
                Axies axe = Project.cfg.ax_y;
                MPC08EDLL.set_profile(axe.value,
                    axe.SpeedLow * axe.StepsPerMM,
                    axe.HomeSpend * axe.StepsPerMM,
                    axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }

            if (Project.cfg.ax_z.IsEnable)
            {
                Axies axe = Project.cfg.ax_z;
                MPC08EDLL.set_profile(axe.value,
                    axe.SpeedLow * axe.StepsPerMM,
                    axe.HomeSpend * axe.StepsPerMM,
                    axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }

            if (Project.cfg.ax_u.IsEnable)
            {
                Axies axe = Project.cfg.ax_u;
                MPC08EDLL.set_profile(axe.value,
                    axe.SpeedLow * axe.StepsPerMM,
                    axe.HomeSpend * axe.StepsPerMM,
                    axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }


            if (Project.cfg.ax_v.IsEnable)
            {
                Axies axe = Project.cfg.ax_v;
                MPC08EDLL.set_profile(axe.value,
                          axe.SpeedLow * axe.StepsPerMM,
                          axe.HomeSpend * axe.StepsPerMM,
                          axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }


            if (Project.cfg.ax_ball.IsEnable)
            {
                Axies axe = Project.cfg.ax_ball;
                MPC08EDLL.set_profile(axe.value,
                     axe.SpeedLow * axe.StepsPerMM,
                     axe.HomeSpend * axe.StepsPerMM,
                     axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }

        }
        /// <summary>
        /// 设置归零信号
        /// </summary>
        private void SetHomeSignal()
        {

            if (Project.cfg.ax_x.IsEnable)
            {
                MPC08EDLL.enable_org(Project.cfg.ax_x.value, 1);//
                MPC08EDLL.set_home_mode(Project.cfg.ax_x.value, 0);//设置回零方式
            }

            if (Project.cfg.ax_y.IsEnable)
            {
                MPC08EDLL.enable_org(Project.cfg.ax_y.value, 1);//
                MPC08EDLL.set_home_mode(Project.cfg.ax_y.value, 0);//设置回零方式
            }

            if (Project.cfg.ax_z.IsEnable)
            {
                MPC08EDLL.enable_org(Project.cfg.ax_z.value, 1);//
                MPC08EDLL.set_home_mode(Project.cfg.ax_z.value, 0);//设置回零方式
            }

            if (Project.cfg.ax_u.IsEnable)
            {
                MPC08EDLL.enable_org(Project.cfg.ax_u.value, 1);//
                MPC08EDLL.set_home_mode(Project.cfg.ax_u.value, 0);//设置回零方式
            }

            if (Project.cfg.ax_v.IsEnable)
            {
                MPC08EDLL.enable_org(Project.cfg.ax_v.value, 1);//
                MPC08EDLL.set_home_mode(Project.cfg.ax_v.value, 0);//设置回零方式
            }

            if (Project.cfg.ax_ball.IsEnable)
            {
                MPC08EDLL.enable_org(Project.cfg.ax_ball.value, 1);//
                MPC08EDLL.set_home_mode(Project.cfg.ax_ball.value, 0);//设置回零方式
            }

        }

        /// <summary>
        /// 快速返回机械中心
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="dir"></param>
        private void MoveHome(int ch, int dir, Axies axies)
        {
            if (MPC08EDLL.check_done(ch) != 0)
            {
                return;
            }

            int Speed = (int)(axies.HomeSpend * axies.StepsPerMM);
            int SpendL=(int)(axies.SpeedLow * axies.StepsPerMM);
            MPC08EDLL.set_profile(axies.value, SpendL, Speed, axies.acSpeed);//设置速度
            //MPC08EDLL.set_conspeed(axies.value,Speed);
            //MPC08EDLL.set_conspeed(ch, axies.HomeSpend * axies.StepsPerMM);
            ////SetSpeed(ch, speedDict[strHomeSpeed]);
            int A= MPC08EDLL.fast_hmove(ch, dir);//set_conspeed
            Project.WriteLog($"回零点：轴【{ch}】{A}方向【{dir}】速度【{Speed}】");
        }

        public void MoveHome()
        {
            SetHomeSignal();

            if (Project.FstStop)
            {
                Project.WriteLog("已经停止运行");
                return;
            }

            Project.WriteLog("复位Z轴");
            if (Project.cfg.ax_z.IsEnable)
            {

                if (Project.cfg.ax_z.BackLash)
                {
                    Axies axe = Project.cfg.ax_z; MoveHome(axe.value, axe.direction, axe);
                }
            }
            if (Project.FstStop)
            {
                Project.WriteLog("已经停止运行");
                return;
            }
            Project.WriteLog("等待Z轴复位");
            WaitFiveAxeMoveFinish();

            Project.WriteLog("Z轴完成,开始其他轴复位");
            if (Project.FstStop)
            {
                Project.WriteLog("已经停止运行");
                return;
            }
            if (Project.cfg.ax_x.IsEnable)
            {
                if (Project.cfg.ax_x.BackLash)
                {
                    Axies axe = Project.cfg.ax_x;
                    if (Project.cfg.ax_x.IsSecondValue) { MoveHomeByVector(axe); }
                    else { MoveHome(axe.value, axe.direction, axe); }
                }
                
            }
            if (Project.FstStop)
            {
                Project.WriteLog("已经停止运行");
                return;
            }
            if (Project.cfg.ax_y.IsEnable)
            {
                if (Project.cfg.ax_y.BackLash)
                {
                    Axies axe = Project.cfg.ax_y;
                    if (Project.cfg.ax_y.IsSecondValue) { MoveHomeByVector(axe); }
                    else { MoveHome(axe.value, axe.direction, axe); }
                }
            }
            if (Project.FstStop)
            {
                Project.WriteLog("已经停止运行");
                return;
            }
            if (Project.cfg.ax_u.IsEnable)
            {
                if (Project.cfg.ax_u.BackLash)
                {
                    Axies axe = Project.cfg.ax_u; MoveHome(axe.value, axe.direction, axe);
                }
            }
            if (Project.FstStop)
            {
                Project.WriteLog("已经停止运行");
                return;
            }
            if (Project.cfg.ax_v.IsEnable)
            {
                if (Project.cfg.ax_v.BackLash)//==EquipmentType.Type_A
                {
                    Axies axe = Project.cfg.ax_v; MoveHome(axe.value, axe.direction, axe);
                }
                else
                {
                    
                }

            }
            if (Project.FstStop)
            {
                Project.WriteLog("已经停止运行");
                return;
            }
            if (Project.cfg.ax_ball.IsEnable)
            {
                if (Project.cfg.ax_ball.BackLash)
                {
                    Axies axe = Project.cfg.ax_ball; MoveHome(axe.value, axe.direction, axe);
                }
            }
            if (Project.FstStop)
            {
                Project.WriteLog("已经停止运行");
                return;
            }
            if (Project.Stop == true)
            {
                Project.Stop = false;
                //return;
                
            }
            Project.WriteLog("返回起始点");
            WaitFiveAxeMoveFinish();
            Project.WriteLog("复位点坐标");
            ResetPos();
        }

        public void Move2Points(double dx, double dy, double dz, double du, double dv, double dball,bool Home)
        {
            double dx1 = 0, dy1 = 0, dz1 = 0, du1 = 0, dv1 = 0, dball1 = 0;
            UpdateCurAbsPos(ref dx1, ref dy1, ref dz1, ref du1, ref dv1, ref dball1);//更新五轴位置
            Project.WriteLog("运动到指定点:"+dx+","+dy+","+dz+","+du+","+dv);
            if (Project.cfg.ax_x.IsEnable)
            { Axies axe = Project.cfg.ax_x; MoveAbsolute(axe, dx, Home, dx1); }

            if (Project.cfg.ax_y.IsEnable)
            { Axies axe = Project.cfg.ax_y;
                if (axe.IsSecondValue)
                {
                    MoveAbsoluteByVector(axe, dy,Home,dy1);//通过插补方式运动到指定点位
                }
                else
                {
                    MoveAbsolute(axe, dy, Home, dy1);
                }
                
            }

            if (Project.cfg.ax_z.IsEnable)
            { Axies axe = Project.cfg.ax_z; MoveAbsolute(axe, dz, Home, dz1); }

            if (Project.cfg.ax_u.IsEnable)
            { Axies axe = Project.cfg.ax_u; MoveAbsolute(axe, du, Home, du1); }

            if (Project.cfg.ax_v.IsEnable)
            { Axies axe = Project.cfg.ax_v; MoveAbsolute(axe, dv, Home, dv1); }

            if (Project.cfg.ax_ball.IsEnable)
            { Axies axe = Project.cfg.ax_ball; MoveAbsolute(axe, dball, Home, dball1); }
        }

        private void ResetPos()
        {
            if (Project.cfg.ax_x.IsEnable)
            { Axies axe = Project.cfg.ax_x; ResetPos(axe.value); }

            if (Project.cfg.ax_y.IsEnable)
            { Axies axe = Project.cfg.ax_y; ResetPos(axe.value); }

            if (Project.cfg.ax_z.IsEnable)
            { Axies axe = Project.cfg.ax_z; ResetPos(axe.value); }


            if (Project.cfg.ax_u.IsEnable)
            { Axies axe = Project.cfg.ax_u; ResetPos(axe.value); }


            if (Project.cfg.ax_v.IsEnable)
            { Axies axe = Project.cfg.ax_v; ResetPos(axe.value); }


            if (Project.cfg.ax_ball.IsEnable)
            { Axies axe = Project.cfg.ax_ball; ResetPos(axe.value); }
        }

        private void ResetPos(int ch)
        {
            if (MPC08EDLL.check_done(ch) != 0) { return; } 
            MPC08EDLL.reset_pos(ch);
            //Project.WriteLog("");
        }

        /*****************************************************/
        /// <summary>
        /// 控制x轴左向移动
        /// </summary>
        /// <param name="speed"></param>
        public void MoveXAxisUp(EnumMoveSpeed speed)
        {
            Axies axe = Project.cfg.ax_x;
            if (!axe.IsEnable) { return; }
            if (axe.IsSecondValue)
            {
                if (MPC08EDLL.check_done(axe.value) != 0) { return; }
                MoveAxisNoWaitByVector(axe, speed, -axe.direction);
            }
            else
            {
                if (MPC08EDLL.check_done(axe.value) != 0) { return; }
                MoveAxisNoWait(axe, speed, -axe.direction);
            }
        }

        public void MoveXAxisDown(EnumMoveSpeed speed)
        {
            Axies axe = Project.cfg.ax_x;
            if (!axe.IsEnable) { return; }
            int dir = Project.cfg.IsFlipped ? -axe.direction : axe.direction;
            if (axe.IsSecondValue)
            {
                if (MPC08EDLL.check_done(axe.value) != 0) { return; }
                MoveAxisNoWaitByVector(axe, speed, dir);
            }
            else
            {
                if (MPC08EDLL.check_done(axe.value) != 0) { Project.WriteLog("轴状态异常"); return; }
                MoveAxisNoWait(axe, speed, dir);
            }
        }

        //Down-> 数据减少  Up->数据增加
        public void MoveYAxiesUp(EnumMoveSpeed speed)
        {
            Axies axe = Project.cfg.ax_y;
            if (!axe.IsEnable) { return; }
            if (axe.IsSecondValue)
            {
                //if (MPC08EDLL.check_done(axe.value) != 0) { /*Project.WriteLog("轴状态异常");*/ return; }
                MoveAxisNoWaitByVector(axe, speed, -axe.direction);
            }
            else
            {
                if (MPC08EDLL.check_done(axe.value) != 0) { return; }
                MoveAxisNoWait(axe, speed, -axe.direction);
            }
        }
        public void MoveYAxiesDown(EnumMoveSpeed speed)
        {
            Axies axe = Project.cfg.ax_y;
            if (!axe.IsEnable) { return; }
            int dir = Project.cfg.IsFlipped ? -axe.direction : axe.direction;
            if (axe.IsSecondValue)
            {
                //if (MPC08EDLL.check_done(axe.value) != 0) { return; }
                MoveAxisNoWaitByVector(axe, speed, dir);
            }
            else
            {
                if (MPC08EDLL.check_done(axe.value) != 0) { return; }
                MoveAxisNoWait(axe, speed, dir);
            }
        }
        public void MoveZAxiesUp(EnumMoveSpeed speed)
        {
            if (!Project.cfg.ax_z.IsEnable) { return; }
            if (MPC08EDLL.check_done(Project.cfg.ax_z.value) != 0) { return; }
            MoveAxisNoWait(Project.cfg.ax_z, speed, -Project.cfg.ax_z.direction);
        }
        public void MoveZAxiesDown(EnumMoveSpeed speed)
        {
            if (!Project.cfg.ax_z.IsEnable) { return; }
            if (MPC08EDLL.check_done(Project.cfg.ax_z.value) != 0) { return; }
            MoveAxisNoWait(Project.cfg.ax_z, speed, Project.cfg.ax_z.direction);
        }
        public void MoveUAxiesUp(EnumMoveSpeed speed)
        {
            if (!Project.cfg.ax_u.IsEnable) { return; }
            if (MPC08EDLL.check_done(Project.cfg.ax_u.value) != 0) { return; }
            MoveAxisNoWait(Project.cfg.ax_u, speed, -Project.cfg.ax_u.direction);
        }
        public void MoveUAxiesDown(EnumMoveSpeed speed)
        {
            if (!Project.cfg.ax_u.IsEnable) { return; }
            if (MPC08EDLL.check_done(Project.cfg.ax_u.value) != 0) { return; }
            MoveAxisNoWait(Project.cfg.ax_u, speed, Project.cfg.ax_u.direction);
        }
        public void MoveVAxiesUp(EnumMoveSpeed speed)
        {
            if (!Project.cfg.ax_v.IsEnable) { return; }
            if (MPC08EDLL.check_done(Project.cfg.ax_v.value) != 0) { return; }
            MoveAxisNoWait(Project.cfg.ax_v, speed, -Project.cfg.ax_v.direction);
        }
        public void MoveVAxiesDown(EnumMoveSpeed speed)
        {
            if (!Project.cfg.ax_v.IsEnable) { return; }
            if (MPC08EDLL.check_done(Project.cfg.ax_v.value) != 0) { return; }
            MoveAxisNoWait(Project.cfg.ax_v, speed, Project.cfg.ax_v.direction);
        }
        public void MoveBallAxiesUp(EnumMoveSpeed speed)
        {
            if (!Project.cfg.ax_ball.IsEnable) { return; }
            if (MPC08EDLL.check_done(Project.cfg.ax_ball.value) != 0) { return; }
            MoveAxisNoWait(Project.cfg.ax_ball, speed, -Project.cfg.ax_ball.direction);
        }
        public void MoveBallAxiesDown(EnumMoveSpeed speed)
        {
            if (!Project.cfg.ax_ball.IsEnable) { return; }
            if (MPC08EDLL.check_done(Project.cfg.ax_ball.value) != 0) { return; }
            MoveAxisNoWait(Project.cfg.ax_ball, speed, -Project.cfg.ax_ball.direction);
        }

        public void MoveStop(int ch)
        {
            MPC08EDLL.decel_stop(ch);
        }

        public void MoveStop2(int ch, int ch2)
        {
            MPC08EDLL.decel_stop2(ch, ch2);
        }

        //设置为0点位置 
        public void Reset()
        {
            if (Project.cfg.ax_x.IsEnable)
            {
                MPC08EDLL.reset_pos(Project.cfg.ax_x.value);
            }

            if (Project.cfg.ax_y.IsEnable)
            {
                MPC08EDLL.reset_pos(Project.cfg.ax_y.value);
            }

            if (Project.cfg.ax_z.IsEnable)
            {
                MPC08EDLL.reset_pos(Project.cfg.ax_z.value);
            }

            if (Project.cfg.ax_u.IsEnable)
            {
                MPC08EDLL.reset_pos(Project.cfg.ax_u.value);
            }

            if (Project.cfg.ax_v.IsEnable)
            {
                MPC08EDLL.reset_pos(Project.cfg.ax_v.value);
            }

            if (Project.cfg.ax_ball.IsEnable)
            {
                MPC08EDLL.reset_pos(Project.cfg.ax_ball.value);
            }
        }

        /*****************************************************/
        /// <summary>
        /// 单轴以某一速度移动
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="speed"></param>
        /// <param name="steps"></param>
        public void MoveAxisNoWait(Axies axe, EnumMoveSpeed speed, int steps)
        {
            double mvspeed = 0;
            switch (speed)
            {
                case EnumMoveSpeed.SLOW: mvspeed = axe.SpeedLow; break;
                case EnumMoveSpeed.MEDIUM: mvspeed = axe.SpeedMedium; break;
                case EnumMoveSpeed.HIGH: mvspeed = axe.SpeedFast; break;
            }
            MPC08EDLL.set_profile(axe.value, 0, (int)mvspeed * axe.StepsPerMM, axe.acSpeed * axe.StepsPerMM);
            //Project.WriteLog("轴代号：" + axe.value.ToString() + "当前速度位：" + (mvspeed * axe.StepsPerMM).ToString() + "当前加速度：" + (axe.acSpeed * axe.StepsPerMM).ToString() + "当前位移：" + steps.ToString());
            int ret = MPC08EDLL.fast_vmove(axe.value, steps);//移动
        }

        private void MoveAxisNoWaitByVector(Axies axe, EnumMoveSpeed speed, int steps)
        {
            double mvspeed = 0;
            switch (speed)
            {
                case EnumMoveSpeed.SLOW: mvspeed = axe.SpeedLow; break;
                case EnumMoveSpeed.MEDIUM: mvspeed = axe.SpeedMedium; break;
                case EnumMoveSpeed.HIGH: mvspeed = axe.SpeedFast; break;
            }

            //MPC08EDLL.set_profile(axe.value, 0, axe.SpeedFast, axe.acSpeed);
            //运动到指定位置再进行运动,执行测试策略
            int ret = MPC08EDLL.set_vector_profile(mvspeed * axe.StepsPerMM,
                mvspeed * axe.StepsPerMM, axe.acSpeed * axe.StepsPerMM);
            long curPosX = 0;
            MPC08EDLL.get_abs_pos(axe.value, ref curPosX);
            if (curPosX > limitValue) { curPosX -= maxVal; }
            //计算目标点与当前点的距离
            double disX = 1000+((double)curPosX / axe.StepsPerMM * (-axe.direction));
            int xsteps = (int)(disX * axe.StepsPerMM) * (steps);

            ret = MPC08EDLL.fast_line2(axe.value, xsteps,
                axe.secondvalue, xsteps);//移动
            //Project.WriteLog("轴代号：" + axe.value.ToString()+","+axe.secondvalue.ToString() + "    当前速度位：" + (mvspeed * axe.StepsPerMM).ToString() + "当前加速度：" + (axe.acSpeed * axe.StepsPerMM).ToString() + "当前位移：" + steps.ToString());
        }

        public void UpdataIO(ref int x, ref int y, ref int z, ref int u, ref int v, ref int ball,ref int TestSignal1, ref int TestSignal2,ref int lightscreen)
        {

            if (Project.cfg.ax_x.IsEnable)
                x = MPC08EDLL.checkin_bit(board_cnt1, Project.cfg.ax_x.value);

            if (Project.cfg.ax_y.IsEnable)
                y = MPC08EDLL.checkin_bit(board_cnt1, Project.cfg.ax_y.value);

            if (Project.cfg.ax_z.IsEnable)
                z = MPC08EDLL.checkin_bit(board_cnt1, Project.cfg.ax_z.value);

            if (Project.cfg.ax_u.IsEnable)
                u = MPC08EDLL.checkin_bit(board_cnt1, Project.cfg.ax_u.value);

            if (Project.cfg.ax_v.IsEnable)
                v = MPC08EDLL.checkin_bit(board_cnt1, Project.cfg.ax_v.value);

            if (Project.cfg.ax_ball.IsEnable)
                ball = MPC08EDLL.checkin_bit(board_cnt1, Project.cfg.ax_ball.value);

            if (Project.cfg.LightScreenAlarmEnable)
            {
                //int6是光幕检测信号，如果检测到了就要停止5个轴，防止伤人
                //lightscreen = MPC08EDLL.checkin_bit(board_cnt1, 6);
                lightscreen = MPC08EDLL.checkin_bit(board_cnt1, Project.cfg.LightScreenSignal);
            }               

            TestSignal1 = MPC08EDLL.checkin_bit(board_cnt1, 10);
            TestSignal2 = MPC08EDLL.checkin_bit(board_cnt1, 11);

           // Console.WriteLine(DateTime.Now+"信号1-->" + TestSignal1+"信号2-->"+TestSignal2);
        }
        /// <summary>
        /// 获取当前坐标参数
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        public void UpdateCurAbsPos(ref double dx, ref double dy, ref double dz, ref double du, ref double dv, ref double dball)
        {
            Config config = Project.cfg;

            if (Project.cfg.ax_x == null | Project.cfg.ax_y == null | Project.cfg.ax_z == null| Project.cfg.ax_v == null| Project.cfg.ax_ball==null)
            {
                return;
            }

            int ret = 0;
            long curX = 0;
            if (Project.cfg.ax_x.IsEnable)
                ret = MPC08EDLL.get_abs_pos(Project.cfg.ax_x.value, ref curX);

            long curY = 0;
            if (Project.cfg.ax_y.IsEnable)
                ret = MPC08EDLL.get_abs_pos(Project.cfg.ax_y.value, ref curY);

            //if (Project.cfg.ax_y.IsSecondValue)
            //{
            //    long curY2 = 0;
            //    ret = MPC08EDLL.get_abs_pos(Project.cfg.ax_y.secondvalue, ref curY2);
            //}
            

            long curZ = 0;
            if (Project.cfg.ax_z.IsEnable)
                ret = MPC08EDLL.get_abs_pos(Project.cfg.ax_z.value, ref curZ);

            long curU = 0;
            if (Project.cfg.ax_u.IsEnable)
                ret = MPC08EDLL.get_abs_pos(Project.cfg.ax_u.value, ref curU);

            long curV = 0;
            if (Project.cfg.ax_v.IsEnable)
                ret = MPC08EDLL.get_abs_pos(Project.cfg.ax_v.value, ref curV);

            long curBall = 0;
            if (Project.cfg.ax_ball.IsEnable)
                ret = MPC08EDLL.get_abs_pos(Project.cfg.ax_ball.value, ref curBall);

            if (curX > limitValue)
            {
                curX -= maxVal;
            }

            if (curY > limitValue)
            {
                curY -= maxVal;
            }

            if (curZ > limitValue)
            {
                curZ -= maxVal;
            }

            if (curU > limitValue)
            {
                curU -= maxVal;
            }

            if (curV > limitValue)
            {
                curV -= maxVal;
            }


            if (curBall > limitValue)
            {
                curBall -= maxVal;
            }


            if (Project.cfg.ax_x.StepsPerMM != 0 && Project.cfg.ax_x.IsEnable)
            { dx = ((double)curX / Project.cfg.ax_x.StepsPerMM) * (-Project.cfg.ax_x.direction); }

            if (Project.cfg.ax_y.StepsPerMM != 0 && Project.cfg.ax_y.IsEnable)
                dy = ((double)curY / Project.cfg.ax_y.StepsPerMM) * (-Project.cfg.ax_y.direction);


            if (Project.cfg.ax_z.StepsPerMM != 0 && Project.cfg.ax_z.IsEnable)
                dz = ((double)curZ / Project.cfg.ax_z.StepsPerMM) * (-Project.cfg.ax_z.direction);


            if (Project.cfg.ax_u.StepsPerMM != 0 && Project.cfg.ax_u.IsEnable)
                du = ((double)curU / Project.cfg.ax_u.StepsPerMM) * (-Project.cfg.ax_u.direction);

            if (Project.cfg.ax_v.StepsPerMM != 0 && Project.cfg.ax_v.IsEnable)
                dv = ((double)curV / Project.cfg.ax_v.StepsPerMM) * (-Project.cfg.ax_v.direction);

            if (Project.cfg.ax_ball.StepsPerMM != 0 && Project.cfg.ax_ball.IsEnable)
                dball = ((double)curBall / Project.cfg.ax_ball.StepsPerMM) * (-Project.cfg.ax_ball.direction);
        }
        private void SetVectorSpeed(int nSpeed, double acSpeed)
        {
            MPC08EDLL.set_vector_profile(0, nSpeed, acSpeed);
        }
        //移动指定运动轴
        public void MoveAbsolute(Axies curAxe, double dm,bool Home,double abs_pos)
        {
           
            if (MPC08EDLL.check_done(curAxe.value) != 0) { return; }
            int nSpeed = (int)(curAxe.SpeedMedium * curAxe.StepsPerMM);
            MPC08EDLL.set_profile(curAxe.value, 0, nSpeed, curAxe.acSpeed);//设置速度
            //获取当前位置
            //long curPosX = 0;
            //MPC08EDLL.get_abs_pos(curAxe.value, ref curPosX);
            //MPC08EDLL.get_abs_pos
            if (abs_pos > curAxe.LowerLimit) { Project.WriteLog($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】"); return; }
            double disX = 0;
            //double aaa = (double)curPosX / curAxe.StepsPerMM * (-curAxe.direction);
            //disX = (dm - ((double)curPosX / curAxe.StepsPerMM * (-curAxe.direction)));//*(-curAxe.direction)
            disX = (dm - abs_pos);//*(-curAxe.direction)

            if (Home == true)
            {

                if (!curAxe.BackLash)
                {
                    return;
                }
                #region
                //if ((EquipmentType)Project.cfg.EQType == EquipmentType.Type_A)
                //{
                //    if (curAxe.Name == AXiesName.U轴)
                //    {
                //        return;
                //    }
                //}
                //else
                //{

                //}
                #endregion


            }
            else
            {
                //计算目标点与当前点的距离
                Console.WriteLine($"当前点位【{dm}】");


                switch (curAxe.Name)
                {
                    case AXiesName.X轴:
                        Console.WriteLine($"当前中心点位【{(double)Project.Xorg}】");
                        disX = disX+(double)Project.Xorg;//((double)curAxe.center / curAxe.StepsPerMM * (-curAxe.direction));
                        break;
                    case AXiesName.Z轴:
                        Console.WriteLine($"当前中心点位【{(double)Project.Zorg}】");
                        disX = disX + (double)Project.Zorg;
                        break;
                    case AXiesName.Y轴:
                        Console.WriteLine($"当前中心点位【{(double)Project.Yorg}】");
                        disX = disX + (double)Project.Yorg;
                        break;
                    case AXiesName.V轴:
                        Console.WriteLine($"当前中心点位【{(double)Project.Vorg}】");
                        disX = disX + (double)Project.Vorg;
                        break;
                    case AXiesName.U轴:
                        Console.WriteLine($"当前中心点位【{(double)Project.Uorg}】");
                        disX = disX + (double)Project.Uorg;
                        break;
                    default:
                        disX = disX + (double)Project.Xorg;
                        break;
                }
                //disX = dm + (double) curAxe.center;//((double)curAxe.center / curAxe.StepsPerMM * (-curAxe.direction));
                Console.WriteLine($"相对点位【{disX}】");
            }

            #region 测试




            //if (Home==true)
            //{
            //    disX = dm;//((double)curAxe.center / curAxe.StepsPerMM * (-curAxe.direction));
            //    Console.WriteLine($"dm:【{disX}】Center【{(double)curAxe.center}】");
            //}
            //else
            //{
            //    //计算目标点与当前点的距离
            //    Console.WriteLine($"当前点位【{dm}】");


            //    switch (curAxe.value)
            //    {
            //        case 1:
            //            Console.WriteLine($"当前中心点位【{(double)Project.cfg.Xorg}】");
            //            disX = dm + (double)Project.cfg.Xorg;//((double)curAxe.center / curAxe.StepsPerMM * (-curAxe.direction));
            //            break;
            //        case 2:
            //            Console.WriteLine($"当前中心点位【{(double)Project.cfg.Zorg}】");
            //            disX = dm + (double)Project.cfg.Zorg;
            //            break;
            //        case 3:
            //            Console.WriteLine($"当前中心点位【{(double)Project.cfg.Yorg}】");
            //            disX = dm + (double)Project.cfg.Yorg;
            //            break;
            //        case 4:
            //            Console.WriteLine($"当前中心点位【{(double)Project.cfg.Vorg}】");
            //            disX = dm + (double)Project.cfg.Vorg;
            //            break;
            //        case 5:
            //            Console.WriteLine($"当前中心点位【{(double)Project.cfg.Uorg}】");
            //            disX = dm + (double)Project.cfg.Uorg;
            //            break;
            //        default:
            //            disX = dm + (double)Project.cfg.Xorg;
            //            break;
            //    }
            //    //disX = dm + (double) curAxe.center;//((double)curAxe.center / curAxe.StepsPerMM * (-curAxe.direction));
            //    Console.WriteLine($"相对点位【{disX}】");
            //}

            #endregion

            //通过距离求得脉冲量
            int xSteps = (int) (disX * curAxe.StepsPerMM) * (-curAxe.direction);
            Console.WriteLine($"轴：【{curAxe.value}】当前点位【{abs_pos}】脉冲/mm【{curAxe.StepsPerMM}】相差距离【{disX}】运动脉冲【{xSteps}】输入点位【{dm}】");
            //MPC08EDLL.fast_pmove(curAxe.value, xSteps);
          
            MPC08EDLL.fast_pmove(curAxe.value, xSteps);
        }

        public void MoveAbsoluteByVector(Axies curAxe, double dm, bool Home, double abs_pos)
        {

            if (MPC08EDLL.check_done(curAxe.value) != 0) { return; }
            int nSpeed = (int)(curAxe.SpeedMedium * curAxe.StepsPerMM);
           // MPC08EDLL.set_profile(curAxe.value, 0, nSpeed, curAxe.acSpeed);//设置速度
            MPC08EDLL.set_vector_profile(nSpeed, nSpeed, curAxe.acSpeed * curAxe.StepsPerMM);
            //获取当前位置

            //获取当前位置
            long curPosX = 0;
            MPC08EDLL.get_abs_pos(curAxe.value, ref curPosX);

            //MPC08EDLL.get_abs_pos
            if (abs_pos > curAxe.LowerLimit) { Project.WriteLog($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】"); return; }
            double disX = 0;
            //double aaa = (double)curPosX / curAxe.StepsPerMM * (-curAxe.direction);
            disX = (dm - ((double)curPosX / curAxe.StepsPerMM * (-curAxe.direction)));//*(-curAxe.direction)
            //disX = (dm - abs_pos);//*(-curAxe.direction)

            if (Home == true)
            {

                if (!curAxe.BackLash)
                {
                    return;
                }
            }
            else
            {
                //计算目标点与当前点的距离
                Project.WriteLog($"当前点位:【{dm}】");


                switch (curAxe.Name)
                {
                    case AXiesName.X轴:
                        Console.WriteLine($"当前中心点位【{(double)Project.Xorg}】");
                        disX = disX + (double)Project.Xorg;//((double)curAxe.center / curAxe.StepsPerMM * (-curAxe.direction));
                        break;
                    case AXiesName.Z轴:
                        Console.WriteLine($"当前中心点位【{(double)Project.Zorg}】");
                        disX = disX + (double)Project.Zorg;
                        break;
                    case AXiesName.Y轴:
                        Project.WriteLog($"当前中心点位【{(double)Project.Yorg}】");
                        disX = disX + (double)Project.Yorg;
                        break;
                    case AXiesName.V轴:
                        Console.WriteLine($"当前中心点位【{(double)Project.Vorg}】");
                        disX = disX + (double)Project.Vorg;
                        break;
                    case AXiesName.U轴:
                        Console.WriteLine($"当前中心点位【{(double)Project.Uorg}】");
                        disX = disX + (double)Project.Uorg;
                        break;
                    default:
                        disX = disX + (double)Project.Xorg;
                        break;
                }
                //disX = dm + (double) curAxe.center;//((double)curAxe.center / curAxe.StepsPerMM * (-curAxe.direction));
                Project.WriteLog($"相对点位【{disX}】");
            }



            //通过距离求得脉冲量
            int xSteps = (int)(disX * curAxe.StepsPerMM) * (-curAxe.direction);
            Project.WriteLog($"轴：【{curAxe.value}】当前点位【{abs_pos}】脉冲/mm【{curAxe.StepsPerMM}】相差距离【{disX}】运动脉冲【{xSteps}】输入点位【{dm}】");
            //MPC08EDLL.fast_pmove(curAxe.value, xSteps);
            Project.WriteLog($"轴1：【{curAxe.value}】 轴2：【{{curAxe.secondvalue}}】");
            MPC08EDLL.fast_line2(curAxe.value, xSteps, curAxe.secondvalue, xSteps);
            //MPC08EDLL.fast_pmove(curAxe.value, xSteps);
        }



        public void MoveAbsoluteByVector(Axies curAxe, double dm)
        {
            if (MPC08EDLL.check_done(curAxe.value) != 0) { return; }
            int nSpeed = (int)(curAxe.SpeedMedium * curAxe.StepsPerMM);
            //MPC08EDLL.set_profile(curAxe.value, 0, nSpeed, curAxe.acSpeed);//设置速度
            MPC08EDLL.set_vector_profile(nSpeed, nSpeed, curAxe.acSpeed * curAxe.StepsPerMM);
            //获取当前位置
            long curPosX = 0,curPosX2=0;
            MPC08EDLL.get_abs_pos(curAxe.value, ref curPosX);
            MPC08EDLL.get_abs_pos(curAxe.secondvalue, ref curPosX2);
            Project.WriteLog($"轴【{curAxe.value}】读取点位【{curPosX}】");
            if (curPosX > curAxe.LowerLimit)
            { 
                curPosX -= maxVal;
                Project.WriteLog($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】");
            }
            //{ Project.WriteLog($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】"); return; }
            //计算目标点与当前点的距离
            double disX = dm - ((double)curPosX / curAxe.StepsPerMM * (-curAxe.direction));
            //通过距离求得脉冲量
            int xSteps = (int)(disX * curAxe.StepsPerMM) * (-curAxe.direction);
            MPC08EDLL.fast_line2(curAxe.value, xSteps, curAxe.secondvalue, xSteps);

        }

        public void MoveAbsoluteByVector2(Axies curAxe, double dm, bool Home, double abs_pos)
        {
            if (MPC08EDLL.check_done(curAxe.value) != 0) { return; }
            int nSpeed = (int)(curAxe.SpeedMedium * curAxe.StepsPerMM);
            //MPC08EDLL.set_profile(curAxe.value, 0, nSpeed, curAxe.acSpeed);//设置速度
            MPC08EDLL.set_vector_profile(nSpeed, nSpeed, curAxe.acSpeed * curAxe.StepsPerMM);
            //获取当前位置
            long curPosX = 0;
            MPC08EDLL.get_abs_pos(curAxe.value, ref curPosX);
            Project.WriteLog($"轴【{curAxe.value}】读取点位【{curPosX}】");
            if (curPosX > curAxe.LowerLimit)
            {
                curPosX -= maxVal;
                Project.WriteLog($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】");
            }
            //{ Project.WriteLog($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】"); return; }
            //计算目标点与当前点的距离
            double disX = (dm - abs_pos);//*(-curAxe.direction);
            //通过距离求得脉冲量
            int xSteps = (int)(disX * curAxe.StepsPerMM) * (curAxe.direction);
            MPC08EDLL.fast_line2(curAxe.value, xSteps, curAxe.secondvalue, xSteps);

        }
        //等待所有运动轴停止
        public void WaitFiveAxeMoveFinish()
        {
            while ((Project.cfg.ax_x.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_x.value) != 0) ||
                   (Project.cfg.ax_y.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_y.value) != 0) ||
                   (Project.cfg.ax_z.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_z.value) != 0) ||
                   (Project.cfg.ax_u.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_u.value) != 0) ||
                   (Project.cfg.ax_v.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_v.value) != 0) ||
                   (Project.cfg.ax_ball.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_ball.value) != 0))
            {
                Thread.Sleep(50);
            }
        }

        public bool CheckFiveAxeMoveFinish()
        {
            if ((Project.cfg.ax_x.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_x.value) != 0) ||
                   (Project.cfg.ax_y.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_y.value) != 0) ||
                   (Project.cfg.ax_z.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_z.value) != 0) ||
                   (Project.cfg.ax_u.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_u.value) != 0) ||
                   (Project.cfg.ax_v.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_v.value) != 0) ||
                   (Project.cfg.ax_ball.IsEnable && MPC08EDLL.check_done(Project.cfg.ax_ball.value) != 0))
            {
                return false;
            }
            return true;
        }
        //双轴以
        public void MoveHomeByVector(Axies curAxe)
        {
            if (MPC08EDLL.check_done(curAxe.value) != 0) { return; }
            int nSpeed = (int)(curAxe.SpeedMedium * curAxe.StepsPerMM);
            //MPC08EDLL.set_profile(curAxe.value, 0, nSpeed, curAxe.acSpeed);//设置速度
            MPC08EDLL.set_vector_profile(nSpeed, nSpeed, curAxe.acSpeed * curAxe.StepsPerMM);
            //获取当前位置
            long curPosX = 0;
            MPC08EDLL.get_abs_pos(curAxe.value, ref curPosX);
            Console.WriteLine($"轴【{curAxe.value}】读取点位【{curPosX}】");
          //  if (curPosX > curAxe.LowerLimit) { Project.WriteLog($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】"); return; }
            //计算目标点与当前点的距离
            //double disX = dm - ((double)curPosX / curAxe.StepsPerMM * (-curAxe.direction));
            double disX = -500000;
            //通过距离求得脉冲量
            int xSteps = (int)(disX * curAxe.StepsPerMM) * (-curAxe.direction);
            MPC08EDLL.fast_line2(curAxe.value, xSteps, curAxe.secondvalue, xSteps);
        }
        //运动速度
        public enum EnumMoveSpeed
        {
            SLOW,
            MEDIUM,
            HIGH
        }
    }

    public enum MovCtrol
    {
        X,
        Y,Z,U,V,Ball
    }

    /**********************************************************************/
}
