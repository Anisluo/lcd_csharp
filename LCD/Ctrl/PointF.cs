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
            double x0 = X + Project.Xorg - ptcenter.X;
            double y0 = Y +Project.Yorg - ptcenter.Y;
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

           double total_h = 750;

            PointF pt = new PointF(0, 0, 0);
            switch (type)
            {
                case EquipmentType.Type_A://中表
                    //表盘
                    double X2 = x0 * Math.Cos(V * Math.PI / 180) - y0 * Math.Sin(V * Math.PI / 180);
                    double Y2 = x0 * Math.Sin(V * Math.PI / 180) + y0 * Math.Cos(V * Math.PI / 180);
                    double Z2 = z0;
                    double u2 = U;
                    double V2= V;

                    Project.WriteLog($"计算表盘:X--》{X2} Y--》{Y2} Z--》{Z2} U--》{u2} V--》{V2} ");

                    //U = U * -1;

                    X6 = X2 * Math.Cos(U * Math.PI / 180) - (h * Math.Sin(U * Math.PI / 180));
                    Z6 = X2 * Math.Sin(U * Math.PI / 180) + (h * Math.Cos(U * Math.PI / 180));
                    //Z6 = Z2 * Math.Cos(U * Math.PI / 180) - X2 * Math.Sin(U * Math.PI / 180);
                    //X6 = X2 * Math.Sin(U * Math.PI / 180) + X2 * Math.Cos(U * Math.PI / 180);
                    Y6 = Y2;


                    Z6 = Z - Z6 ;


                    pt.X = (X6 - Project.Xorg + ptcenter.X);
                    pt.Y = (Y6 - Project.Yorg + ptcenter.Y);
                    pt.Z = (Z6);
                    pt.U = U;
                    pt.V = V;

                    Project.WriteLog($"计算0-90:X--》{X6} Y--》{Y6} Z--》{Z6} U--》{U6} V--》{V6} ");

                    break;
                case EquipmentType.Type_B:         
                     X2 = x0; //x0 * Math.Cos(V * Math.PI / 180) - y0 * Math.Sin(V * Math.PI / 180);//ZZ
                     Y2 = h * Math.Sin(V * Math.PI / 180) + y0 * Math.Cos(V * Math.PI / 180);//
                     Z2 = h * Math.Cos(V * Math.PI / 180) - y0 * Math.Sin(V * Math.PI / 180);
                     u2 = U;
                     V2 = V;

                    Project.WriteLog($"计算V轴:X--》{X2} Y--》{Y2} Z--》{Z2} U--》{u2} V--》{V2} ");
                    //U = U * -1;
                    X6 = X2 * Math.Cos(U * Math.PI / 180) - (h * Math.Sin(U * Math.PI / 180));
                    Z6 = -X2 * Math.Sin(U * Math.PI / 180) + (h * Math.Cos(U * Math.PI / 180));
                    //Z6 = Z2 * Math.Cos(U * Math.PI / 180) - X2 * Math.Sin(U * Math.PI / 180);
                    //X6 = X2 * Math.Sin(U * Math.PI / 180) + X2 * Math.Cos(U * Math.PI / 180);
                    Y6 = Y2;
                    Z6 = Z6 - Z2;

                    pt.X = (X6 - Project.Xorg + ptcenter.X);
                    pt.Y = (Y6 - Project.Yorg + ptcenter.Y);
                    pt.Z = (Z6);
                    pt.U = U;
                    pt.V = V;
                    Project.WriteLog($"计算0-90:X--》{X6} Y--》{Y6} Z--》{Z6} U--》{U6} V--》{V6} ");
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
        Type_D
    }
}
