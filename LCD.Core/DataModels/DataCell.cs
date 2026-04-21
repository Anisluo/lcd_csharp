using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VisionCore
{
    /// <summary>
    /// 变量数据单元
    /// </summary>
    [Serializable]
    public struct DataCell : ICloneable
    {
        /// <summary>模块名称</summary>
        public string mModName { set; get; }
        /// <summary>单元编号</summary>
        public int mModIndex { set; get; }
        /// <summary>变量分类</summary>
        public DataType mDataType { set; get; }
        /// <summary>变量类型</summary>
        public DataMode mDataMode { set; get; }
        /// <summary>变量名称</summary>
        public string mDataName { set; get; }
        /// <summary>变量注释</summary>
        public string mDataTip { set; get; }
        /// <summary>变量的值</summary>
        public object mDataValue { set; get; }
        /// <summary>变量初值</summary>
        private string mDataInit;
        /// <summary>变量个数</summary>
        private int mDataNum;
        /// <summary>变量属性</summary>
        public DataAtrr mDataAtrr;
        /// <summary>
        /// 数据单元
        /// </summary>
        /// <param name="_ModName">模块名称</param>
        /// <param name="_CellID">单元编号</param>
        /// <param name="_Group">变量类型</param>
        /// <param name="_type">数据类型</param>
        /// <param name="_Name">名称</param>
        /// <param name="_Tip">注释</param>
        /// <param name="_InitValue">初始值</param>
        /// <param name="_Num">数量</param>
        /// <param name="_Value">值</param>
        /// <param name="_Atrr">变量归属</param>
        public DataCell(string _ModName, int _Index, DataType _Type, DataMode _Mode, string _Name, string _Tip, string _InitValue, int _Num, object _Value, DataAtrr _Atrr)
        {
            mModName = _ModName;
            mModIndex = _Index;
            mDataType = _Type;
            mDataMode = _Mode;
            mDataName = _Name;
            mDataTip = _Tip;
            mDataInit = _InitValue;
            mDataNum = _Num;
            mDataValue = _Value;
            mDataAtrr = _Atrr;
        }
        public void InitValue(string mModName, DataMode _DataType, string _InitValue)
        {
            try
            {
                if (mModName.Equals("全局变量")) return;
                mDataMode = _DataType;
                mDataInit = _InitValue;
                switch (mDataMode)
                {
                    case DataMode.Int:
                        mDataValue = int.Parse(_InitValue);
                        break;
                    case DataMode.Bool:
                        mDataValue = Convert.ToBoolean(_InitValue.ToUpper());
                        break;
                    case DataMode.Double:
                        mDataValue = (double.Parse(_InitValue));
                        break;
                    case DataMode.String:
                        mDataValue = _InitValue;
                        break;
                    case DataMode.点点:
                        //mDataValue = new PtoP_Info();
                        break;
                    case DataMode.点2D:
                        //mDataValue = new RPoint();
                        break;
                    case DataMode.点3D:
                        //mDataValue = new Point3DF();
                        break;
                    case DataMode.矩形阵列:
                        //mDataValue = new Rect_Info();
                        break;
                    case DataMode.旋转矩形:
                        //mDataValue = new Rect2_Info();
                        break;
                    case DataMode.图像:
                        //mDataValue = new RImage();
                        break;
                    case DataMode.椭圆:
                        //mDataValue = new Ellipse_Info();
                        break;
                    case DataMode.位置转换2D:
                        //mDataValue = new HHomMat2D();
                        break;
                    case DataMode.圆:
                        //mDataValue = new Circle_Info();
                        break;
                    case DataMode.直线:
                        //mDataValue = new Line_Info();
                        break;
                    case DataMode.坐标系:
                        //mDataValue = new Coord_Info();
                        break;
                    case DataMode.平面:
                        //mDataValue = new Plane_Info();
                        break;
                    case DataMode.标定信息:
                        //mDataValue = new Cal_Info();
                        break;
                    case DataMode.自定义:
                        mDataValue = _InitValue;
                        break;
                    default:
                        //RLog.Instance.VTLogError("未处理数据类型 " + mDataMode.ToString() + " 数据信息 ");
                        break;
                }
            }
            catch { }
        }
        /// <summary>
        /// 初始化值
        /// </summary>
        /// <param name="_Mod_Name">模块名称</param>
        /// <param name="_CellID">单元编号</param>
        /// <param name="_Name">变量名称</param>
        /// <param name="_DataType">数据类型</param>
        /// <param name="_InitValue">初始值</param>
        public void InitValue(string _Mod_Name, int _CellID, string _Name, DataMode _DataType, string _InitValue)
        {
            mModIndex = _CellID;
            mDataName = _Name;
            mModName = _Mod_Name;
            InitValue(mModName, _DataType, _InitValue);
        }

        public object GetValue()
        {
            object _value = null;
            if (mDataType == DataType.单量)
            {
                switch (mDataMode)
                {
                    case DataMode.Int:
                        _value = ((List<double>)mDataValue)[0];
                        break;
                    case DataMode.Bool:
                        _value = ((List<bool>)mDataValue)[0];
                        break;
                    case DataMode.Double:
                        mDataValue = new List<double>();
                        break;
                    case DataMode.String:
                        _value = ((List<string>)mDataValue)[0];
                        break;
                    case DataMode.点2D:
                        //_value = ((List<RPoint>)mDataValue)[0];
                        break;
                    case DataMode.点3D:
                        //_value = ((List<Point3DF>)mDataValue)[0];
                        break;
                    case DataMode.矩形阵列:
                        //_value = ((List<Rect_Info>)mDataValue)[0];
                        break;
                    case DataMode.旋转矩形:
                        //_value = ((List<Rect2_Info>)mDataValue)[0];
                        break;
                    case DataMode.图像:
                        //_value = ((List<RImage>)mDataValue)[0];
                        break;
                    case DataMode.椭圆:
                        //_value = ((List<Ellipse_Info>)mDataValue)[0];
                        break;
                    case DataMode.位置转换2D:
                        //_value = ((List<HHomMat2D>)mDataValue)[0];
                        break;
                    case DataMode.圆:
                        //_value = ((List<Circle_Info>)mDataValue)[0];
                        break;
                    case DataMode.直线:
                        //_value = ((List<Line_Info>)mDataValue)[0];
                        break;
                    case DataMode.坐标系:
                        //_value = ((List<Coord_Info>)mDataValue)[0];
                        break;
                    case DataMode.平面:
                        //_value = ((List<Plane_Info>)mDataValue)[0];
                        break;
                    case DataMode.标定信息:
                        //_value = ((List<Cal_Info>)mDataValue)[0];
                        break;
                    default:
                        //RLog.Instance.VTLogError("未处理数据类型 " + mDataMode.ToString());
                        break;
                }
            }
            else
            {
                _value = mDataValue;
            }
            return _value;
        }

        public void SetNull(DataMode _DataType)
        {
            switch (mDataMode)
            {
                case DataMode.Int:
                    mDataValue = new List<int>();
                    break;
                case DataMode.Bool:
                    mDataValue = new List<bool>();
                    break;
                case DataMode.Double:
                    mDataValue = new List<double>();
                    break;
                case DataMode.String:
                    mDataValue = new List<string>();
                    break;
                case DataMode.点2D:
                    //mDataValue = new List<RPoint>();
                    break;
                case DataMode.点3D:
                    //mDataValue = new List<Point3DF>();
                    break;
                case DataMode.矩形阵列:
                    //mDataValue = new List<Rect_Info>();
                    break;
                case DataMode.旋转矩形:
                    //mDataValue = new List<Rect2_Info>();
                    break;
                case DataMode.图像:
                    //mDataValue = new List<RImage>();
                    break;
                case DataMode.椭圆:
                    //mDataValue = new List<Ellipse_Info>();
                    break;
                case DataMode.位置转换2D:
                    // mDataValue = new List<HHomMat2D>();
                    break;
                case DataMode.圆:
                    //mDataValue = new List<Circle_Info>();
                    break;
                case DataMode.直线:
                    //mDataValue = new List<Line_Info>();
                    break;
                case DataMode.坐标系:
                    //mDataValue = new List<Coord_Info>();
                    break;
                case DataMode.平面:
                    //mDataValue = new List<Plane_Info>();
                    break;
                case DataMode.标定信息:
                    //mDataValue = new List<Cal_Info>();
                    break;
                default:
                    //RLog.Instance.VTLogError("未处理数据类型 " + mDataMode.ToString());
                    break;
            }
        }
        /// <summary>
        /// 根据类型设置值
        /// </summary>
        /// <param name="_DataType">类型</param>
        /// <param name="_value">值不为list</param>
        public void SetValue(DataMode _DataType, object _value)
        {
            if (_value.GetType().Name.Contains("List"))
            {
                mDataValue = _value;
            }
            else
            {
                switch (mDataMode)
                {
                    case DataMode.Int:
                        mDataValue = new List<Double>() { (int)_value };
                        break;
                    case DataMode.Bool:
                        mDataValue = new List<bool>() { (bool)_value };
                        break;
                    case DataMode.Double:
                        mDataValue = new List<Double>() { (double)_value };
                        break;
                    case DataMode.String:
                        mDataValue = new List<string>() { (string)_value };
                        break;
                    case DataMode.点2D:
                        //mDataValue = new List<RPoint>() { (RPoint)_value };
                        break;
                    case DataMode.点3D:
                        //mDataValue = new List<Point3DF>() { (Point3DF)_value };
                        break;
                    case DataMode.矩形阵列:
                        //mDataValue = new List<Rect_Info>() { (Rect_Info)_value };
                        break;
                    case DataMode.旋转矩形:
                        //mDataValue = new List<Rect2_Info>() { (Rect2_Info)_value };
                        break;
                    case DataMode.图像:
                        //mDataValue = new List<RImage>() { (RImage)_value };
                        break;
                    case DataMode.椭圆:
                        //mDataValue = new List<Ellipse_Info>() { (Ellipse_Info)_value };
                        break;
                    case DataMode.位置转换2D:
                        //mDataValue = new List<HHomMat2D>() { (HHomMat2D)_value };
                        break;
                    case DataMode.圆:
                        //mDataValue = new List<Circle_Info>() { (Circle_Info)_value };
                        break;
                    case DataMode.直线:
                        //mDataValue = new List<Line_Info>() { (Line_Info)_value };
                        break;
                    case DataMode.坐标系:
                        //mDataValue = new List<Coord_Info>() { (Coord_Info)_value };
                        break;
                    case DataMode.平面:
                        //mDataValue = new List<Plane_Info>() { (Plane_Info)_value };
                        break;
                    case DataMode.标定信息:
                        //mDataValue = new List<Cal_Info>() { (Cal_Info)_value };
                        break;
                    default:
                        //RLog.Instance.VTLogError("未处理数据类型 " + mDataMode.ToString() + " 数据信息 " + _value.GetType().ToString());
                        break;
                }
            }
        }

        [OnDeserializing()]
        internal void OnDeSerializingMethod(StreamingContext context)
        {
            mDataInit = "0";
        }

        [OnDeserialized()]
        internal void OnDeSerializedMethod(StreamingContext context)
        {
            if (mModName != null)
            {
                InitValue(mModName, mDataMode, mDataInit);
            }
        }

        public object Clone()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream);
        }
    }
}
