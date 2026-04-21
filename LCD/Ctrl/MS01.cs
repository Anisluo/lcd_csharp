using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCD.Data;
using Microsoft.DwayneNeed.Win32.User32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OxyPlot;
using SciChart.Core.Extensions;
using SharpDX.Direct3D11;
using VisionCore;

namespace LCD.Ctrl
{
    internal class MS01 : TestMachine
    {
        private static MS01 m_ms01;
        private string RecStr { get; set; } = "";
        public IData m_Result { get; set; }
        public bool SelfInspectionMark { get; set; } = false;
        public bool AnCalibration { get; set; } = false;
        public List<List<string>> ExcelData { get; set; }
        //自检
        string self_inspection = "BD00000100BE";//new Byte[6] {0xBD, 0x00, 0x00, 0x01, 0x00, 0xBE };
        string GetSpectrumArr = "BD10000100BE";//new Byte[6] { 0xBD, 0x03, 0x00, 0x01, 0x00, 0xBE };
        string Calibration = "BD02000101BE";//暗场校准/new Byte[6] { 0xBD, 0x02, 0x00, 0x01, 0x00, 0xBE };

        private SerialPort SerialPort;

        public override bool IsOpen { get => SerialPort == null ? false : SerialPort.IsOpen; }

        public override void Init()
        {
            ExcelExop();

            if (SerialPort == null)
            {
                SerialPort = new SerialPort();
            }

            SerialPort.PortName = Project.cfg.MS01.comName;
            SerialPort.BaudRate = int.Parse(Project.cfg.MS01.bardRateText);
            SerialPort.DataBits = int.Parse(Project.cfg.MS01.dataBitText);
            //SerialPort.Parity = Project.cfg.MS01.parityText;

            Project.WriteLog("MS01串口参数:"+$"[{SerialPort.PortName}][{SerialPort.BaudRate}][{SerialPort.DataBits}][{SerialPort.Parity}][{Project.cfg.MS01.stopBit}][{Project.cfg.MS01.parity}]");

            switch (Project.cfg.MS01.stopBit)
            {
                case 0: SerialPort.StopBits = StopBits.None; break;
                case 1: SerialPort.StopBits = StopBits.One; break;
                case 2: SerialPort.StopBits = StopBits.Two; break;
            }
            switch (Project.cfg.MS01.parity)
            {
                case 0: SerialPort.Parity = Parity.None; break;
                case 1: SerialPort.Parity = Parity.Odd; break;
                case 2: SerialPort.Parity = Parity.Even; break;
            }

            SerialPort.DataReceived += SerialPort_DataReceived; 

            if (SerialPort.IsOpen) { 
                SerialPort.Close(); 
            }

            try
            {
                SerialPort.Open();
            }
            catch (Exception ex)
            {
                Project.WriteLog("MS01串口打开失败："+ex.Message);
            }

            if (SerialPort.IsOpen == false)
            {
                Project.WriteLog("错误：MS01串口打开失败");
                return;
            }
            Project.WriteLog("MS01串口打开OK");
        }

       

        public override void AutoCheck()
        {
            if (IsOpen==false)
            {
                Project.WriteLog("串口没有打开");
                return;
            }
            if(SelfInspectionMark && AnCalibration)
            {
                //已经校正过了，就不需要校正了啊
                return;
            }
            //打开串口后，执行自检和暗场校正
            bool st = AutoTest();
            if (st)
            {
                //第一个成功了才执行第二个啊
                CalibrationTest();
            }
        }

        //解析excel文件，提取参数
        private void ExcelExop()
        {
            IWorkbook workbook = null;
            ISheet sheet = null;
            string Path = System.Environment.CurrentDirectory + @"\data\光谱转化.xlsx";
            workbook = new XSSFWorkbook(Path);
            if (workbook != null)
            {
                //读取excel中第一个sheet
                sheet = workbook.GetSheetAt(0);
                ExcelData = new List<List<string>>();

                for (int row = 0; row <= sheet.LastRowNum; row++)
                {
                    IRow currentRow = sheet.GetRow(row);
                    List<string> list = new List<string>();
                    for (int col = 0; col < currentRow.LastCellNum; col++)
                    {
                        ICell currentCell = currentRow.GetCell(col);
                        if (currentCell != null)
                        {
                            string str = currentCell.ToString();

                            list.Add(currentCell.ToString());
                        }
                    }
                    ExcelData.Add(list);
                }
            }

        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string str = SerialPort.ReadExisting();
            Console.WriteLine(str);
            RecStr += str; 
        }

        

        private string waitString(string sign, int TimeOut)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalSeconds < TimeOut)
            {
                if (RecStr.Contains(sign))
                {
                    Console.WriteLine(RecStr.Contains(sign));
                    break;
                }
            }
            //Console.WriteLine("DQWC-->"+ RecStr);
            return RecStr;
        }

        //自检
        private bool AutoTest()
        {
            if (IsOpen == false)
            {
                Project.WriteLog("串口没有打开");
                return false;
            }
            Project.WriteLog("开始自检");
            Byte[] SendBuffer = HexTool.HexToByte(self_inspection);
            RecStr = "";
            SerialPort.Write(SendBuffer,0,SendBuffer.Length); 
            var Buffer = waitString("activate succeed", 50);
            if (Buffer.IndexOf("activate succeed") == -1)
            {
                SelfInspectionMark = false;
                Project.WriteLog($"自检指令返回超时超时时间:[50s]");
            }
            else
            {
                SelfInspectionMark = true;
                Project.WriteLog($"自检指令返回成功{Buffer}");
            }
            return SelfInspectionMark;
        }

        //暗场校正
        public bool CalibrationTest()
        {
            if (IsOpen == false)
            {
                Project.WriteLog("串口没有打开");
                return false;
            }
            Project.WriteLog("开始暗场校正");
            RecStr = "";
            Byte[] SendBuffer = HexTool.HexToByte(Calibration);
            SerialPort.Write(SendBuffer, 0, SendBuffer.Length);
            var BufferCalibration = waitString("dark field data saved", 30);
            if (BufferCalibration.IndexOf("dark field data saved") == -1)
            {
                AnCalibration = false;
                Project.WriteLog($"暗场校准指令返回超时超时时间:[30000ms]");           
            }
            else
            {
                AnCalibration = true;
                Project.WriteLog($"暗场校准指令返回成功{BufferCalibration}");
            }
            return AnCalibration;
        }

        public static double CalculateCCT(double x, double y)
        {
            // 根据McCamy公式计算色温
            double n = (x - 0.3320) / (0.1858 - y);
            double CCT = 437 * Math.Pow(n, 3) + 3601 * Math.Pow(n, 2) + 6861 * n + 5514.31;
            return CCT;
        }

        /// <summary>
        /// 光谱测量
        /// </summary>
        public override IData MeasureSpectrum()
        {
            if (IsOpen == false) 
            {
                Project.WriteLog("串口没有打开");
                return null; 
            }
            RecStr = "";
            Byte[] SendBuffer = HexTool.HexToByte(GetSpectrumArr);
            SerialPort.Write(SendBuffer, 0, SendBuffer.Length);
            Project.WriteLog("已经发送光谱测量命令");
            var Buffer = waitString( ",}", 60);
            if (Buffer.IndexOf(",}") == -1)
            {
                Project.WriteLog($"获取光谱指令返回超时,超时时间:[60s]");
                return null;
            }
            else
            {
                Project.WriteLog("光谱数据："+Buffer);
                string str = Buffer;             //Write($"获取光谱指令返回成功{str}");
                str = str.Substring(str.IndexOf("{") + 1, (str.Length - str.IndexOf("{") - 4));
                str = str.Trim();
                var Strs = str.Split(',');

                double XValue = 0;
                double YValue = 0;
                double ZValue = 0;
                int i = 0;

                string sub = Buffer;
                int index1= sub.IndexOf("exposure");
                int index2 = sub.IndexOf("correct spectrum", index1);
                if((index2>index1) && (index1>0))
                {
                    sub = sub.Substring(index1,index2-index1);
                    index1 = sub.IndexOf(":");
                    if(index1<0)
                    {
                        Project.WriteLog($"光谱指令返回错误数据");
                        return null;
                    }
                    index2 = sub.IndexOf("dev[0]",index1);
                    if (index2 < 0)
                    {
                        Project.WriteLog($"光谱指令返回错误数据");
                        return null;
                    }
                    sub = sub.Substring(index1+1,index2-index1-1).Trim();
                }
                else
                {
                    Project.WriteLog($"光谱指令返回错误数据");
                    return null;
                }
                string[] ary = sub.Split(',');
                if(ary.Length < 9)
                {
                    Project.WriteLog($"光谱指令返回错误光谱数据");
                    return null; 
                }

                //添加到数据窗口啊
                IData Result = new IData();
                Result.L = double.Parse(ary[3]);
                Result.X = double.Parse(ary[0]);
                Result.Y = double.Parse(ary[1]);
                Result.Z = double.Parse(ary[2]);
                Result.Cx = double.Parse(ary[4]);
                Result.Cy = double.Parse(ary[5]);
                Result.u = double.Parse(ary[6]);
                Result.v = double.Parse(ary[7]);
                Result.CCT = CalculateCCT(Result.Cx, Result.Cy);
                //这个cct不知道怎么赋值啊
                //Result.CCT = Convert.ToDouble(datastrs[11]);

                i = 0;
                foreach (var item in Strs)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    double Data = 0;
                    try
                    {
                        Data = Double.Parse(item);
                    }
                    catch (Exception ex)
                    {
                        Project.WriteLog($"数据转化异常:【{ex.Message}】,数据源:【{str}】，异常数据【{item}】");
                    }
                    Result.SpectrumData[i] = Data;
                    i++;
                }

                return Result;
            }
        }

        public double SignificantDigits(double Data, int Cont)
        {
            return Math.Round(Data, Cont);
        }

        //色坐标测量
        //lxy色坐标测量
        public override IData MeasureLxy()
        {
            return MeasureSpectrum();
        }


        //关闭仪器
        public override void Close()
        {
            if(IsOpen)
            {
                SerialPort.Close();
            }
        }

        internal static TestMachine GetInstance()
        {
            if (m_ms01 == null) m_ms01 = new MS01();
            return m_ms01;
        }
    }
}
