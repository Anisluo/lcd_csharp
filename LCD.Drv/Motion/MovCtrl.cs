/*********************************************
 * 运动控制类，负责系统中所有运动控制轴的初始化 
 * 测试过程中XY两轴要有同时运动的效果
 ***/
using LCD.Core.Runtime;
using LCD.Data;
using LCD.Dll;
using System;
using System.ComponentModel;
using System.Threading;
using VisionCore;

namespace LCD.Ctrl
{
    /// <summary>
    /// MPC08板卡
    /// </summary>
    [Category("运动控制"), Description("MPC08板卡"), DisplayName("MPC08板卡")]
    public class MovCtrl
    {
        private int board_cnt = 0;
        public int board_cnt1 = 1;
        long limitValue = 100000000;
        long maxVal = 4294967296;

        // 六个运动轴的配置（由调用方在使用前注入，取代对 Project.cfg.ax_X 的全局静态访问）
        public Axies AxX { get; set; }
        public Axies AxY { get; set; }
        public Axies AxZ { get; set; }
        public Axies AxU { get; set; }
        public Axies AxV { get; set; }
        public Axies AxBall { get; set; }

        // 其它从 Project.cfg 注入的运动相关配置
        public bool IsFlipped { get; set; }
        public bool LightScreenAlarmEnable { get; set; }
        public int LightScreenSignal { get; set; }

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
                    int XSpeed = (int)(AxX.SpeedMedium * AxX.StepsPerMM);
                    MPC08EDLL.set_profile(AxX.value, 0.2 * XSpeed, XSpeed, AxX.acSpeed);//设置速度
                    MPC08EDLL.decel_stop(AxX.value);
                    break;
                case MovCtrol.Y:
                    int YSpeed = (int)(AxY.SpeedMedium * AxY.StepsPerMM);
                    MPC08EDLL.set_profile(AxY.value, 0.2 * YSpeed, YSpeed, AxY.acSpeed);//设置速度
                    MPC08EDLL.decel_stop(AxY.value);
                    break;
                case MovCtrol.Z:
                    int nSpeed = (int)(AxZ.SpeedMedium * AxZ.StepsPerMM);
                    MPC08EDLL.set_profile(AxZ.value, 0.2* nSpeed, nSpeed, AxZ.acSpeed);//设置速度
                    MPC08EDLL.decel_stop(AxZ.value);
                    break;
                case MovCtrol.U:
                    int nSpeed1 = (int)(AxU.SpeedMedium * AxU.StepsPerMM);
                    MPC08EDLL.set_profile(AxU.value, 0.2 * nSpeed1, nSpeed1, AxU.acSpeed);//设置速度
                    MPC08EDLL.decel_stop(AxU.value);
                    break;
                case MovCtrol.V:
                    int VSpeed = (int)(AxV.SpeedMedium * AxV.StepsPerMM);
                    MPC08EDLL.set_profile(AxV.value, 0.2 * VSpeed, VSpeed, AxV.acSpeed);//设置速度
                    MPC08EDLL.decel_stop(AxV.value);
                    break;
            }
            
        }
        public void StopAll()
        {
            if (AxX.IsEnable)
            {
                Axies axe = AxX;
                if (axe.IsSecondValue)
                {
                    MPC08EDLL.decel_stop2(axe.value, axe.secondvalue);
                }
                else
                {
                    MPC08EDLL.decel_stop(axe.value);
                }
            }

            if (AxY.IsEnable)
            {
                Axies axe = AxY;
                if (axe.IsSecondValue)
                {
                    MPC08EDLL.decel_stop2(axe.value, axe.secondvalue);
                }
                else
                {
                    MPC08EDLL.decel_stop(axe.value);
                }
            }
            if (AxZ.IsEnable)
            {
                Axies axe = AxZ;
                if (axe.IsSecondValue)
                {
                    MPC08EDLL.decel_stop2(axe.value, axe.secondvalue);
                }
                else
                {
                    MPC08EDLL.decel_stop(axe.value);
                }
            }


            if (AxU.IsEnable)
            {
                Axies axe = AxU;
                MPC08EDLL.decel_stop(axe.value);
            }
            if (AxV.IsEnable)
            {
                Axies axe = AxV;
                MPC08EDLL.decel_stop(axe.value);
            }
            if (AxBall.IsEnable)
            {
                Axies axe = AxBall;
                MPC08EDLL.decel_stop(axe.value);
            }
        }

        private MovCtrl()
        {
            //int ret = MPC08EDLL.auto_set();
            //ret = MPC08EDLL.init_board();//
            //Log.Info("auto_set=" + ret.ToString());
            //Log.Info("init_board=" + ret.ToString());
            Init();
        }

        public void Init()
        {
            //初始化
            int axe_cnt = MPC08EDLL.auto_set();
            if (axe_cnt < 0)
            {
                Log.Info("MPC08初始化失败:未识别到板卡");
                return;
            }

            if (axe_cnt == 0)
            {
                Log.Info("MPC08初始化失败:未识别到轴");
                return;
            }
           
            board_cnt = MPC08EDLL.init_board();
            if (board_cnt < 0)
            {
                Log.Info("MPC08初始化失败:板卡初始化失败");
                return;
            }

            //设置速度
            //SetSpeed();

            //归零使能
            //SetHomeSignal();

            Log.Info("运动控制卡初始化完毕！");
        }
        private void SetSpeed()
        {
            if (AxX.IsEnable)
            {
                Axies axe = AxX;
                MPC08EDLL.set_profile(axe.value,
                    axe.SpeedLow * axe.StepsPerMM,
                    axe.HomeSpend * axe.StepsPerMM,
                    axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }

            if (AxY.IsEnable)
            {
                Axies axe = AxY;
                MPC08EDLL.set_profile(axe.value,
                    axe.SpeedLow * axe.StepsPerMM,
                    axe.HomeSpend * axe.StepsPerMM,
                    axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }

            if (AxZ.IsEnable)
            {
                Axies axe = AxZ;
                MPC08EDLL.set_profile(axe.value,
                    axe.SpeedLow * axe.StepsPerMM,
                    axe.HomeSpend * axe.StepsPerMM,
                    axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }

            if (AxU.IsEnable)
            {
                Axies axe = AxU;
                MPC08EDLL.set_profile(axe.value,
                    axe.SpeedLow * axe.StepsPerMM,
                    axe.HomeSpend * axe.StepsPerMM,
                    axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }


            if (AxV.IsEnable)
            {
                Axies axe = AxV;
                MPC08EDLL.set_profile(axe.value,
                          axe.SpeedLow * axe.StepsPerMM,
                          axe.HomeSpend * axe.StepsPerMM,
                          axe.acSpeed * axe.StepsPerMM);
                MPC08EDLL.set_vector_conspeed(axe.SpeedLow * axe.StepsPerMM);
                MPC08EDLL.set_conspeed(axe.value, axe.HomeSpend * axe.StepsPerMM);
            }


            if (AxBall.IsEnable)
            {
                Axies axe = AxBall;
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

            if (AxX.IsEnable)
            {
                MPC08EDLL.enable_org(AxX.value, 1);//
                MPC08EDLL.set_home_mode(AxX.value, 0);//设置回零方式
            }

            if (AxY.IsEnable)
            {
                MPC08EDLL.enable_org(AxY.value, 1);//
                MPC08EDLL.set_home_mode(AxY.value, 0);//设置回零方式
            }

            if (AxZ.IsEnable)
            {
                MPC08EDLL.enable_org(AxZ.value, 1);//
                MPC08EDLL.set_home_mode(AxZ.value, 0);//设置回零方式
            }

            if (AxU.IsEnable)
            {
                MPC08EDLL.enable_org(AxU.value, 1);//
                MPC08EDLL.set_home_mode(AxU.value, 0);//设置回零方式
            }

            if (AxV.IsEnable)
            {
                MPC08EDLL.enable_org(AxV.value, 1);//
                MPC08EDLL.set_home_mode(AxV.value, 0);//设置回零方式
            }

            if (AxBall.IsEnable)
            {
                MPC08EDLL.enable_org(AxBall.value, 1);//
                MPC08EDLL.set_home_mode(AxBall.value, 0);//设置回零方式
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
            Log.Info($"回零点：轴【{ch}】{A}方向【{dir}】速度【{Speed}】");
        }

        public void MoveHome()
        {
            SetHomeSignal();

            if (MotionRuntime.FstStop)
            {
                Log.Info("已经停止运行");
                return;
            }

            Log.Info("复位Z轴");
            if (AxZ.IsEnable)
            {

                if (AxZ.BackLash)
                {
                    Axies axe = AxZ; MoveHome(axe.value, axe.direction, axe);
                }
            }
            if (MotionRuntime.FstStop)
            {
                Log.Info("已经停止运行");
                return;
            }
            Log.Info("等待Z轴复位");
            WaitFiveAxeMoveFinish();

            Log.Info("Z轴完成,开始其他轴复位");
            if (MotionRuntime.FstStop)
            {
                Log.Info("已经停止运行");
                return;
            }
            if (AxX.IsEnable)
            {
                if (AxX.BackLash)
                {
                    Axies axe = AxX;
                    if (AxX.IsSecondValue) { MoveHomeByVector(axe); }
                    else { MoveHome(axe.value, axe.direction, axe); }
                }
                
            }
            if (MotionRuntime.FstStop)
            {
                Log.Info("已经停止运行");
                return;
            }
            if (AxY.IsEnable)
            {
                if (AxY.BackLash)
                {
                    Axies axe = AxY;
                    if (AxY.IsSecondValue) { MoveHomeByVector(axe); }
                    else { MoveHome(axe.value, axe.direction, axe); }
                }
            }
            if (MotionRuntime.FstStop)
            {
                Log.Info("已经停止运行");
                return;
            }
            if (AxU.IsEnable)
            {
                if (AxU.BackLash)
                {
                    Axies axe = AxU; MoveHome(axe.value, axe.direction, axe);
                }
            }
            if (MotionRuntime.FstStop)
            {
                Log.Info("已经停止运行");
                return;
            }
            if (AxV.IsEnable)
            {
                if (AxV.BackLash)//==EquipmentType.Type_A
                {
                    Axies axe = AxV; MoveHome(axe.value, axe.direction, axe);
                }
                else
                {
                    
                }

            }
            if (MotionRuntime.FstStop)
            {
                Log.Info("已经停止运行");
                return;
            }
            if (AxBall.IsEnable)
            {
                if (AxBall.BackLash)
                {
                    Axies axe = AxBall; MoveHome(axe.value, axe.direction, axe);
                }
            }
            if (MotionRuntime.FstStop)
            {
                Log.Info("已经停止运行");
                return;
            }
            if (MotionRuntime.Stop == true)
            {
                MotionRuntime.Stop = false;
                //return;
                
            }
            Log.Info("返回起始点");
            WaitFiveAxeMoveFinish();
            Log.Info("复位点坐标");
            ResetPos();
        }

        public void Move2Points(double dx, double dy, double dz, double du, double dv, double dball,bool Home)
        {
            double dx1 = 0, dy1 = 0, dz1 = 0, du1 = 0, dv1 = 0, dball1 = 0;
            UpdateCurAbsPos(ref dx1, ref dy1, ref dz1, ref du1, ref dv1, ref dball1);//更新五轴位置
            Log.Info("运动到指定点:"+dx+","+dy+","+dz+","+du+","+dv);
            if (AxX.IsEnable)
            { Axies axe = AxX; MoveAbsolute(axe, dx, Home, dx1); }

            if (AxY.IsEnable)
            { Axies axe = AxY;
                if (axe.IsSecondValue)
                {
                    MoveAbsoluteByVector(axe, dy,Home,dy1);//通过插补方式运动到指定点位
                }
                else
                {
                    MoveAbsolute(axe, dy, Home, dy1);
                }
                
            }

            if (AxZ.IsEnable)
            { Axies axe = AxZ; MoveAbsolute(axe, dz, Home, dz1); }

            if (AxU.IsEnable)
            { Axies axe = AxU; MoveAbsolute(axe, du, Home, du1); }

            if (AxV.IsEnable)
            { Axies axe = AxV; MoveAbsolute(axe, dv, Home, dv1); }

            if (AxBall.IsEnable)
            { Axies axe = AxBall; MoveAbsolute(axe, dball, Home, dball1); }
        }

        private void ResetPos()
        {
            if (AxX.IsEnable)
            { Axies axe = AxX; ResetPos(axe.value); }

            if (AxY.IsEnable)
            { Axies axe = AxY; ResetPos(axe.value); }

            if (AxZ.IsEnable)
            { Axies axe = AxZ; ResetPos(axe.value); }


            if (AxU.IsEnable)
            { Axies axe = AxU; ResetPos(axe.value); }


            if (AxV.IsEnable)
            { Axies axe = AxV; ResetPos(axe.value); }


            if (AxBall.IsEnable)
            { Axies axe = AxBall; ResetPos(axe.value); }
        }

        private void ResetPos(int ch)
        {
            if (MPC08EDLL.check_done(ch) != 0) { return; } 
            MPC08EDLL.reset_pos(ch);
            //Log.Info("");
        }

        /*****************************************************/
        /// <summary>
        /// 控制x轴左向移动
        /// </summary>
        /// <param name="speed"></param>
        public void MoveXAxisUp(EnumMoveSpeed speed)
        {
            Axies axe = AxX;
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
            Axies axe = AxX;
            if (!axe.IsEnable) { return; }
            int dir = IsFlipped ? -axe.direction : axe.direction;
            if (axe.IsSecondValue)
            {
                if (MPC08EDLL.check_done(axe.value) != 0) { return; }
                MoveAxisNoWaitByVector(axe, speed, dir);
            }
            else
            {
                if (MPC08EDLL.check_done(axe.value) != 0) { Log.Info("轴状态异常"); return; }
                MoveAxisNoWait(axe, speed, dir);
            }
        }

        //Down-> 数据减少  Up->数据增加
        public void MoveYAxiesUp(EnumMoveSpeed speed)
        {
            Axies axe = AxY;
            if (!axe.IsEnable) { return; }
            if (axe.IsSecondValue)
            {
                //if (MPC08EDLL.check_done(axe.value) != 0) { /*Log.Info("轴状态异常");*/ return; }
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
            Axies axe = AxY;
            if (!axe.IsEnable) { return; }
            int dir = IsFlipped ? -axe.direction : axe.direction;
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
            if (!AxZ.IsEnable) { return; }
            if (MPC08EDLL.check_done(AxZ.value) != 0) { return; }
            MoveAxisNoWait(AxZ, speed, -AxZ.direction);
        }
        public void MoveZAxiesDown(EnumMoveSpeed speed)
        {
            if (!AxZ.IsEnable) { return; }
            if (MPC08EDLL.check_done(AxZ.value) != 0) { return; }
            MoveAxisNoWait(AxZ, speed, AxZ.direction);
        }
        public void MoveUAxiesUp(EnumMoveSpeed speed)
        {
            if (!AxU.IsEnable) { return; }
            if (MPC08EDLL.check_done(AxU.value) != 0) { return; }
            MoveAxisNoWait(AxU, speed, -AxU.direction);
        }
        public void MoveUAxiesDown(EnumMoveSpeed speed)
        {
            if (!AxU.IsEnable) { return; }
            if (MPC08EDLL.check_done(AxU.value) != 0) { return; }
            MoveAxisNoWait(AxU, speed, AxU.direction);
        }
        public void MoveVAxiesUp(EnumMoveSpeed speed)
        {
            if (!AxV.IsEnable) { return; }
            if (MPC08EDLL.check_done(AxV.value) != 0) { return; }
            MoveAxisNoWait(AxV, speed, -AxV.direction);
        }
        public void MoveVAxiesDown(EnumMoveSpeed speed)
        {
            if (!AxV.IsEnable) { return; }
            if (MPC08EDLL.check_done(AxV.value) != 0) { return; }
            MoveAxisNoWait(AxV, speed, AxV.direction);
        }
        public void MoveBallAxiesUp(EnumMoveSpeed speed)
        {
            if (!AxBall.IsEnable) { return; }
            if (MPC08EDLL.check_done(AxBall.value) != 0) { return; }
            MoveAxisNoWait(AxBall, speed, -AxBall.direction);
        }
        public void MoveBallAxiesDown(EnumMoveSpeed speed)
        {
            if (!AxBall.IsEnable) { return; }
            if (MPC08EDLL.check_done(AxBall.value) != 0) { return; }
            MoveAxisNoWait(AxBall, speed, -AxBall.direction);
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
            if (AxX.IsEnable)
            {
                MPC08EDLL.reset_pos(AxX.value);
            }

            if (AxY.IsEnable)
            {
                MPC08EDLL.reset_pos(AxY.value);
            }

            if (AxZ.IsEnable)
            {
                MPC08EDLL.reset_pos(AxZ.value);
            }

            if (AxU.IsEnable)
            {
                MPC08EDLL.reset_pos(AxU.value);
            }

            if (AxV.IsEnable)
            {
                MPC08EDLL.reset_pos(AxV.value);
            }

            if (AxBall.IsEnable)
            {
                MPC08EDLL.reset_pos(AxBall.value);
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
            //Log.Info("轴代号：" + axe.value.ToString() + "当前速度位：" + (mvspeed * axe.StepsPerMM).ToString() + "当前加速度：" + (axe.acSpeed * axe.StepsPerMM).ToString() + "当前位移：" + steps.ToString());
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
            //Log.Info("轴代号：" + axe.value.ToString()+","+axe.secondvalue.ToString() + "    当前速度位：" + (mvspeed * axe.StepsPerMM).ToString() + "当前加速度：" + (axe.acSpeed * axe.StepsPerMM).ToString() + "当前位移：" + steps.ToString());
        }

        public void UpdataIO(ref int x, ref int y, ref int z, ref int u, ref int v, ref int ball,ref int TestSignal1, ref int TestSignal2,ref int lightscreen)
        {

            if (AxX.IsEnable)
                x = MPC08EDLL.checkin_bit(board_cnt1, AxX.value);

            if (AxY.IsEnable)
                y = MPC08EDLL.checkin_bit(board_cnt1, AxY.value);

            if (AxZ.IsEnable)
                z = MPC08EDLL.checkin_bit(board_cnt1, AxZ.value);

            if (AxU.IsEnable)
                u = MPC08EDLL.checkin_bit(board_cnt1, AxU.value);

            if (AxV.IsEnable)
                v = MPC08EDLL.checkin_bit(board_cnt1, AxV.value);

            if (AxBall.IsEnable)
                ball = MPC08EDLL.checkin_bit(board_cnt1, AxBall.value);

            if (LightScreenAlarmEnable)
            {
                //int6是光幕检测信号，如果检测到了就要停止5个轴，防止伤人
                //lightscreen = MPC08EDLL.checkin_bit(board_cnt1, 6);
                lightscreen = MPC08EDLL.checkin_bit(board_cnt1, LightScreenSignal);
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
            if (AxX == null | AxY == null | AxZ == null| AxV == null| AxBall==null)
            {
                return;
            }

            int ret = 0;
            long curX = 0;
            if (AxX.IsEnable)
                ret = MPC08EDLL.get_abs_pos(AxX.value, ref curX);

            long curY = 0;
            if (AxY.IsEnable)
                ret = MPC08EDLL.get_abs_pos(AxY.value, ref curY);

            //if (AxY.IsSecondValue)
            //{
            //    long curY2 = 0;
            //    ret = MPC08EDLL.get_abs_pos(AxY.secondvalue, ref curY2);
            //}
            

            long curZ = 0;
            if (AxZ.IsEnable)
                ret = MPC08EDLL.get_abs_pos(AxZ.value, ref curZ);

            long curU = 0;
            if (AxU.IsEnable)
                ret = MPC08EDLL.get_abs_pos(AxU.value, ref curU);

            long curV = 0;
            if (AxV.IsEnable)
                ret = MPC08EDLL.get_abs_pos(AxV.value, ref curV);

            long curBall = 0;
            if (AxBall.IsEnable)
                ret = MPC08EDLL.get_abs_pos(AxBall.value, ref curBall);

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


            if (AxX.StepsPerMM != 0 && AxX.IsEnable)
            { dx = ((double)curX / AxX.StepsPerMM) * (-AxX.direction); }

            if (AxY.StepsPerMM != 0 && AxY.IsEnable)
                dy = ((double)curY / AxY.StepsPerMM) * (-AxY.direction);


            if (AxZ.StepsPerMM != 0 && AxZ.IsEnable)
                dz = ((double)curZ / AxZ.StepsPerMM) * (-AxZ.direction);


            if (AxU.StepsPerMM != 0 && AxU.IsEnable)
                du = ((double)curU / AxU.StepsPerMM) * (-AxU.direction);

            if (AxV.StepsPerMM != 0 && AxV.IsEnable)
                dv = ((double)curV / AxV.StepsPerMM) * (-AxV.direction);

            if (AxBall.StepsPerMM != 0 && AxBall.IsEnable)
                dball = ((double)curBall / AxBall.StepsPerMM) * (-AxBall.direction);
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
            if (abs_pos > curAxe.LowerLimit) { Log.Info($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】"); return; }
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
                        Console.WriteLine($"当前中心点位【{(double)MotionRuntime.Xorg}】");
                        disX = disX+(double)MotionRuntime.Xorg;//((double)curAxe.center / curAxe.StepsPerMM * (-curAxe.direction));
                        break;
                    case AXiesName.Z轴:
                        Console.WriteLine($"当前中心点位【{(double)MotionRuntime.Zorg}】");
                        disX = disX + (double)MotionRuntime.Zorg;
                        break;
                    case AXiesName.Y轴:
                        Console.WriteLine($"当前中心点位【{(double)MotionRuntime.Yorg}】");
                        disX = disX + (double)MotionRuntime.Yorg;
                        break;
                    case AXiesName.V轴:
                        Console.WriteLine($"当前中心点位【{(double)MotionRuntime.Vorg}】");
                        disX = disX + (double)MotionRuntime.Vorg;
                        break;
                    case AXiesName.U轴:
                        Console.WriteLine($"当前中心点位【{(double)MotionRuntime.Uorg}】");
                        disX = disX + (double)MotionRuntime.Uorg;
                        break;
                    default:
                        disX = disX + (double)MotionRuntime.Xorg;
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
            if (abs_pos > curAxe.LowerLimit) { Log.Info($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】"); return; }
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
                Log.Info($"当前点位:【{dm}】");


                switch (curAxe.Name)
                {
                    case AXiesName.X轴:
                        Console.WriteLine($"当前中心点位【{(double)MotionRuntime.Xorg}】");
                        disX = disX + (double)MotionRuntime.Xorg;//((double)curAxe.center / curAxe.StepsPerMM * (-curAxe.direction));
                        break;
                    case AXiesName.Z轴:
                        Console.WriteLine($"当前中心点位【{(double)MotionRuntime.Zorg}】");
                        disX = disX + (double)MotionRuntime.Zorg;
                        break;
                    case AXiesName.Y轴:
                        Log.Info($"当前中心点位【{(double)MotionRuntime.Yorg}】");
                        disX = disX + (double)MotionRuntime.Yorg;
                        break;
                    case AXiesName.V轴:
                        Console.WriteLine($"当前中心点位【{(double)MotionRuntime.Vorg}】");
                        disX = disX + (double)MotionRuntime.Vorg;
                        break;
                    case AXiesName.U轴:
                        Console.WriteLine($"当前中心点位【{(double)MotionRuntime.Uorg}】");
                        disX = disX + (double)MotionRuntime.Uorg;
                        break;
                    default:
                        disX = disX + (double)MotionRuntime.Xorg;
                        break;
                }
                //disX = dm + (double) curAxe.center;//((double)curAxe.center / curAxe.StepsPerMM * (-curAxe.direction));
                Log.Info($"相对点位【{disX}】");
            }



            //通过距离求得脉冲量
            int xSteps = (int)(disX * curAxe.StepsPerMM) * (-curAxe.direction);
            Log.Info($"轴：【{curAxe.value}】当前点位【{abs_pos}】脉冲/mm【{curAxe.StepsPerMM}】相差距离【{disX}】运动脉冲【{xSteps}】输入点位【{dm}】");
            //MPC08EDLL.fast_pmove(curAxe.value, xSteps);
            Log.Info($"轴1：【{curAxe.value}】 轴2：【{{curAxe.secondvalue}}】");
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
            Log.Info($"轴【{curAxe.value}】读取点位【{curPosX}】");
            if (curPosX > curAxe.LowerLimit)
            { 
                curPosX -= maxVal;
                Log.Info($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】");
            }
            //{ Log.Info($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】"); return; }
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
            Log.Info($"轴【{curAxe.value}】读取点位【{curPosX}】");
            if (curPosX > curAxe.LowerLimit)
            {
                curPosX -= maxVal;
                Log.Info($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】");
            }
            //{ Log.Info($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】"); return; }
            //计算目标点与当前点的距离
            double disX = (dm - abs_pos);//*(-curAxe.direction);
            //通过距离求得脉冲量
            int xSteps = (int)(disX * curAxe.StepsPerMM) * (curAxe.direction);
            MPC08EDLL.fast_line2(curAxe.value, xSteps, curAxe.secondvalue, xSteps);

        }
        //等待所有运动轴停止
        public void WaitFiveAxeMoveFinish()
        {
            while ((AxX.IsEnable && MPC08EDLL.check_done(AxX.value) != 0) ||
                   (AxY.IsEnable && MPC08EDLL.check_done(AxY.value) != 0) ||
                   (AxZ.IsEnable && MPC08EDLL.check_done(AxZ.value) != 0) ||
                   (AxU.IsEnable && MPC08EDLL.check_done(AxU.value) != 0) ||
                   (AxV.IsEnable && MPC08EDLL.check_done(AxV.value) != 0) ||
                   (AxBall.IsEnable && MPC08EDLL.check_done(AxBall.value) != 0))
            {
                Thread.Sleep(50);
            }
        }

        public bool CheckFiveAxeMoveFinish()
        {
            if ((AxX.IsEnable && MPC08EDLL.check_done(AxX.value) != 0) ||
                   (AxY.IsEnable && MPC08EDLL.check_done(AxY.value) != 0) ||
                   (AxZ.IsEnable && MPC08EDLL.check_done(AxZ.value) != 0) ||
                   (AxU.IsEnable && MPC08EDLL.check_done(AxU.value) != 0) ||
                   (AxV.IsEnable && MPC08EDLL.check_done(AxV.value) != 0) ||
                   (AxBall.IsEnable && MPC08EDLL.check_done(AxBall.value) != 0))
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
          //  if (curPosX > curAxe.LowerLimit) { Log.Info($"轴【{curAxe.value}】超出软极限【{curAxe.LowerLimit}】"); return; }
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
