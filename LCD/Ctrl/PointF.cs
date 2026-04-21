using SciChart.Charting3D.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCD.Ctrl
{
    public class PointF
    {
        private EquipmentType type;
        //使用控件XYZUV坐标系计算
        public double X;
        public double Y;
        public double Z;
        public double U;//U
        public double V;//V
        public double M;
        public PointF(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public PointF(double x, double y, double z, double u, double v, EquipmentType type)
        {
            this.type = type;
            X = x;
            Y = y;
            Z = z;
            U = u;
            V = v;
        }
        public void ResetPos()
        {
            X = 0;
            Y = 0;
            Z = 0;
            U = 0;
            M = 0;
            V = 0;
        }

        public static (double newX, double newY, double newH) RotatePoint(
    double X, double Y, double h,double degreesU, double degreesV, PointF ptcenter)
        {
            X = X + Project.Xorg - ptcenter.X;
            Y= Y + Project.Yorg - ptcenter.Y;
            // 将角度转换为弧度（逆时针为正）
            double radiansV = degreesV * Math.PI / 180.0;
            double radiansU = degreesU * Math.PI / 180.0;

            // 1. 绕X轴旋转V度
            double cosV = Math.Cos(radiansV);
            double sinV = Math.Sin(radiansV);
            double yAfterX = Y * cosV + h * sinV;    // Y分量更新
            double hAfterX = -Y * sinV + h * cosV;   // h分量更新
            double xAfterX = X;                      // X保持不变

            // 2. 绕Y轴旋转U度
            double cosU = Math.Cos(radiansU);
            double sinU = Math.Sin(radiansU);
            double newX = xAfterX * cosU - hAfterX * sinU;  // X分量更新
            double newH = xAfterX * sinU + hAfterX * cosU;  // h分量更新
            double newY = yAfterX;                          // Y保持不变

            return (Math.Round(newX, 2), Math.Round(newY, 2), Math.Round(newH, 2));
        }


        //以作为坐标参考系
        //在X坐标系下不是0,0
        //使用算法1更新点位坐标
        /// <summary>
        /// 该算法使用场景，需要知道中点坐标
        /// </summary>
        /// <param name="r"></param>
        /// <param name="ptcenter"></param>
        public PointF UpdateByAlgorithm1(double h, PointF ptcenter)//终点坐标
        {
            if (type==null)
            {
                throw new Exception("请确认设备类型");
            }
            //XY向量详减少 (h-Z)=r-r*M
            //换算成相对于ptcetner的坐标值
            Project.WriteLog($"中心:X--》{ptcenter.X} Y--》{ptcenter.Y} Z--》{ptcenter.Z} U--》{ptcenter.U} V--》{ptcenter.V} ");
            Project.WriteLog($"相对:X--》{Project.Xorg} Y--》{Project.Yorg} Z--》{Project.Zorg} U--》{Project.Uorg} V--》{Project.Vorg} ");
            double x0 = X + Project.Xorg - ptcenter.X;//step 1:把测试表中的坐标点（x,y）转换到实际的五轴坐标系中
            double y0 = Y +Project.Yorg - ptcenter.Y;//
            //double r = Math.Sqrt(x0 * x0 + y0 * y0 + h * h);//计算测量半径
            double z0 = Z ;//+ h
            double u0 = U;
            double v0 = V;

            double X6 = 0;
            double Y6 = 0;
            double Z6 = 0;
            double U6 = U;
            double V6 = V;

            Project.WriteLog($"补偿前:X--》{x0} Y--》{y0} Z--》{z0} U--》{u0} V--》{v0} ");

            //double total_h = 1030;//z轴上旋转中心到测试平面的距离（要实际测量）
            double total_h = Project.cfg.UProductH;
            PointF pt = new PointF(0, 0, 0);
            switch (type)
            {
                case EquipmentType.Type_A://中表
                    //表盘

                    // step2-先单独计算x轴的偏移（测试平台圆盘旋转后的X,Y坐标计算）
                    double X2 = x0 * Math.Cos(V * Math.PI / 180) - y0 * Math.Sin(V * Math.PI / 180);
                    double Y2 = x0 * Math.Sin(V * Math.PI / 180) + y0 * Math.Cos(V * Math.PI / 180);
                    double Z2 = z0;
                    double u2 = U;
                    double V2= V;

                    Project.WriteLog($"计算表盘:X--》{X2} Y--》{Y2} Z--》{Z2} U--》{u2} V--》{V2} ");

                    //U = U * -1;
                    // step3- 根据垂直旋转角计算，X偏移后的位置，Z坐标偏移后的位置
                    X6 = X2 * Math.Cos(U * Math.PI / 180) - (h * Math.Sin(U * Math.PI / 180));
                    Z6 = X2 * Math.Sin(U * Math.PI / 180) + (h * Math.Cos(U * Math.PI / 180));
                    //Z6 = Z2 * Math.Cos(U * Math.PI / 180) - X2 * Math.Sin(U * Math.PI / 180);
                    //X6 = X2 * Math.Sin(U * Math.PI / 180) + X2 * Math.Cos(U * Math.PI / 180);
                    Y6 = Y2;

                    Z6 = Z - Z6 ;

                    //把计算后的点坐标换回计算前的坐标系
                    pt.X = (X6 - Project.Xorg + ptcenter.X);
                    pt.Y = (Y6 - Project.Yorg + ptcenter.Y);
                    pt.Z = (Z6);
                    pt.U = U;
                    pt.V = V;

                    Project.WriteLog($"计算0-90:X--》{X6} Y--》{Y6} Z--》{Z6} U--》{U6} V--》{V6} ");

                    break;
                case EquipmentType.Type_B:                 
                    X2 = x0; //x0 * Math.Cos(V * Math.PI / 180) - y0 * Math.Sin(V * Math.PI / 180);//ZZ
                    Y2 = y0 + y0 * Math.Cos(V * Math.PI / 180);//               
                    Z2 = h + y0 * Math.Sin(V * Math.PI / 180);
                    u2 = U;
                    V2 = V;
                    Project.WriteLog($"计算V轴:X--》{X2} Y--》{Y2} Z--》{Z2} U--》{u2} V--》{V2} ");
                    //U = U * -1;
                    //更新h,X6和Z6使用更新后的h 
                    double h_v = h + y0 * Math.Sin(V * Math.PI / 180);
                    X6 = X2 * Math.Cos(U * Math.PI / 180) + (h_v * Math.Sin(U * Math.PI / 180));
                    Z6 = X2 * Math.Sin(U * Math.PI / 180) + (h_v * Math.Cos(U * Math.PI / 180));
                    //Z6 = Z2 * Math.Cos(U * Math.PI / 180) - X2 * Math.Sin(U * Math.PI / 180);
                    //X6 = X2 * Math.Sin(U * Math.PI / 180) + X2 * Math.Cos(U * Math.PI / 180);
                    Y6 = Y2;
                    Z6 = Z6 - Z2;

                    pt.X = (X6 - Project.Xorg + ptcenter.X);
                    pt.Y = (Y6 - Project.Yorg + ptcenter.Y);
                    pt.Z = (Z6);
                    pt.U = U;
                    pt.V = V;

                    //double radV = V * Math.PI / 180;
                    //double rotatedV_Y = h * Math.Sin(radV) + y0 * Math.Cos(radV);
                    //double rotatedV_Z = h * Math.Cos(radV) - y0 * Math.Sin(radV);
                    ////第二步:绕U轴旋转(X-Z平面)
                    //double radU = U * Math.PI / 180;
                    //double rotatedU_X = x0 * Math.Cos(radU) - h * Math.Sin(radU);
                    //double rotatedU_Z = -x0 * Math.Sin(radU) + h * Math.Cos(radU);
                    ////坐标系补偿
                    //pt.X = rotatedU_X - Project.Xorg + ptcenter.X;
                    //pt.Y = rotatedV_Y - Project.Yorg + ptcenter.Y;
                    //pt.Z = rotatedU_Z - rotatedU_Z;//需确认此补偿逻辑是否正确
                    //pt.U = U;
                    //pt.V = V;
                    Project.WriteLog($"计算0-90:X--》{pt.X} Y--》{pt.Y} Z--》{pt.Z} U--》{pt.U} V--》{pt.V} ");
                    break;
                case EquipmentType.Type_C:

                 X2 = x0*Math.Cos(V*Math.PI/180)+y0*Math.Sin(V*Math.PI/180);
                 Y2 = y0*Math.Cos(V*Math.PI/180)-x0*Math.Sin(V*Math.PI/180);
                 Z2 = z0+(total_h-Project.Zorg-z0-h)*(1-Math.Cos(U*Math.PI/180));
                  
                 
                //Z2
                 u2 = U;
                 V2 = V;

                 double X3 = X2 +(total_h-Project.Zorg-z0-h) * (Math.Sin(U * Math.PI / 180));
                    double value = Project.Xorg;
                 pt.X = (X3 - Project.Xorg + ptcenter.X);
                 //pt.X = X2;
                 pt.Y = (Y2+ptcenter.Y);
                 pt.Z = (Z2);
                 pt.U = U;
                 pt.V = V;

                  break;
                case EquipmentType.Type_E://type_e跟type_c一样
                    //表盘
                    //X2 = x0 * Math.Cos(V * Math.PI / 180) + y0 * Math.Sin(V * Math.PI / 180);
                    //Y2 = y0 * Math.Cos(V * Math.PI / 180) - x0 * Math.Sin(V * Math.PI / 180);

                    X2= x0 * Math.Cos(V * Math.PI / 180) - y0 * Math.Sin(V * Math.PI / 180); ;
                   Y2= y0 * Math.Cos(V * Math.PI / 180) + x0 * Math.Sin(V * Math.PI / 180);
                    Z2 = z0 + (total_h - Project.Zorg - z0 - h) * (1 - Math.Cos(U * Math.PI / 180));
                    



                    //Z2
                    u2 = U;
                    V2 = V;

                    X3 = X2 + (total_h - Project.Zorg - z0 - h) * (Math.Sin(U * Math.PI / 180));
                    value = Project.Xorg;
                    pt.X = (X3 - Project.Xorg + ptcenter.X);
                    //pt.X = X2;
                    //pt.Y = (Y2 -Project.Yorg+ ptcenter.Y);
                    pt.Y=(Y2-ptcenter.Y);
                    pt.Z = (Z2);
                    pt.U = U;
                    pt.V = V;

                    Project.WriteLog($"E计算0-90:X--》{X6} Y--》{Y6} Z--》{Z6} U--》{U6} V--》{V6} ");                    
                    break;
            }


           


            ////计算水平偏移方向
            //double x2 = x1 * Math.Cos(u1 * Math.PI / 180) + y1 * Math.Sin(u1 * Math.PI / 180);
            //double y2 = y1 * Math.Cos(u1 * Math.PI / 180) - x1 * Math.Sin(u1 * Math.PI / 180);
            //double z2 = z1;

           // PointF pt = new PointF(0, 0, 0);
            ////从相对于中点替换为原来坐标系
            //pt.X = (x2 + ptcenter.X);
            //pt.Y = (y2 + ptcenter.Y);
            //pt.Z = (z2+ptcenter.Z) ;
            //pt.U = U ;
            //pt.V = V;
            //pt.X = (X6-Project.Xorg + ptcenter.X );
            //pt.Y = (Y6-Project.Yorg + ptcenter.Y );
            //pt.Z = (Z6 );
            //pt.U = U;
            //pt.V = V;
            return pt;
        }
    }
    public enum EquipmentType
    {
        Type_A,
        Type_B,
        Type_C,//五轴，Y轴插补运动圆盘旋转，仪器垂直方向摆动
        Type_D,
        Type_E  //20250708新增
    }
}
