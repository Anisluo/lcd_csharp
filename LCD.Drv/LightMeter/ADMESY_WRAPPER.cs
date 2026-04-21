using Admesy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static LCD.Ctrl.Admesy;

//using System.Numerics;
namespace LCD.Ctrl
{
    public class ADMESY_WRAPPER
    {
        //初始化设备
        private uint DeviceHandle;//设备句柄

        private string errorstring = "";
        public string get_errorstring()
        {
            return errorstring;
        }

        private  void AdmesyLibError(int status)
        {
            switch (status)
            {
                case -20:
                    errorstring = "NO Library found\n\n" +
                        "Possible Reason: NI-VISA or LibUSB-WIN32 not installed!";
                    break;

                case -21:
                    errorstring = "NO Library Opened\n\n" +
                        "Possible Reason: usbtmc_find_devices is not performed!";
                    break;

                case -22:
                    errorstring = "Vendorid or productid emty!" +
                        "Possible Reason: Open string must look like:\n" +
                        "USBBUS::Vendorid::productid::SERIAL::INSTR\n" +
                        "example: USB0::0x23cf::0x0ea0::00223::INSTR";
                    break;

                case -24:
                    errorstring = "LibUSB device to open not found!";
                    break;

                default:
                    if (status < -24)
                    {
                        errorstring = "NI-VISA Interface specific Error code.\n\n"
                            + "Error code: " + status.ToString();
                    }
                    else
                    {
                        errorstring = "LIBUSB Interface specific Error code.\n\n"
                            + "Error code: " + status.ToString();
                    }
                    break;
            }
        }
        public bool check_dll_status()
        {
            if (libusbtmc.usbtmc_check_dll() < 0)
            {
                errorstring = "libusbtmc.dll not found!\n";
                return false;
            }
            else if (colour.colour_check_dll() < 0)
            {
                errorstring = "Admesy_colour.dll not found!\n";
                return false;
            }
            return true;
        }

        public bool close_device()
        {




            return true;

        }

        /// <summary>
        /// 读取亮度数据
        /// </summary>
        /// <param name="samples_cnt">采样点数</param>
        /// <param name="delay">采样延时时间 ms</param>
        /// <param name="value">采集到的luminance数据</param>
        /// <param name="timeout">超时时间ms</param>
        /// <returns></returns>
        public bool read_sample(uint samples_cnt, uint delay, ref double[] value, int timeout)
        {
            Byte[] ByteArray = new byte[samples_cnt * 4 + 20];
            StringBuilder command = new StringBuilder();
            /* write :sample:y samples,delay to the device */
            int status = libusbtmc.usbtmc_write(ref DeviceHandle, command.Append(":sample:y " +
                 samples_cnt + "," + delay), timeout);

            if (status < 0)
            {
                AdmesyLibError(status);
                return false;
            }
            /* read data from the device */
            status = libusbtmc.usbtmc_read(ref DeviceHandle, ByteArray, (samples_cnt * 4 + 12), timeout);
            if (status < 0)
            {
                AdmesyLibError(status);
                return false;
            }
            double dt = 0;
            int k = 0;

            double[] ResultArray = new double[status / 4];       // converted double bytearray 
            if (BitConverter.IsLittleEndian)
            {
                // reverse the array 
                ByteArray = ByteArray.Reverse().ToArray();

                for (int i = ((ByteArray.Length) - 12); i >= 12; i += -4)
                {
                    // data is send back as y (4 bytes)
                    ResultArray[k] = (double)BitConverter.ToSingle(ByteArray, i - 4);
                    k++;
                }
                dt = BitConverter.ToSingle(ByteArray, ByteArray.Length - 4);
            }
            else
            {
                for (int i = 12; i <= ((ByteArray.Length) - 12); i += 4)
                {
                    // data is send back as y (4 bytes)
                    ResultArray[k] = (double)BitConverter.ToSingle(ByteArray, i);
                    k++;
                }
                dt = (double)BitConverter.ToSingle(ByteArray, ByteArray.Length - 4);
            }
            for (int i = 0; i < samples_cnt; i++)
            {
                value[i] = ResultArray[i];
            }
            errorstring = "no error";
            return true;
        }


        public bool open_device()
        {
            StringBuilder usbtmcdevices = new StringBuilder(256 * 127);
            StringBuilder SelectedDevice = new StringBuilder(256);
            string[] AdmesyDevices = new string[127];
            int status = 0;
            status = libusbtmc.usbtmc_find_devices(usbtmcdevices);
            if (status < 0)
            {
                AdmesyLibError(status);
                return false;
            }
            /* check if any USBTMC device is found */
            else if (usbtmcdevices.Length == 0)//no devices found
            {
                return false;
            }
            int AdmesyDevicesFound = 0;


            /* find if an attached device is an Admesy MSE(+) device. */
            string[] FoundDevice = usbtmcdevices.ToString().Split(new Char[] { '\n' }, 127);
            foreach (string admesy_device in FoundDevice)
            {
                /* Admesy ProductID and Vendor ID
                 * 0x23CF::0x1060	: Asteria
                 * Check For Capital and non-Capital
                 */
                //Console.WriteLine("本机设备:" + admesy_device);
                if (admesy_device.Contains("0x23CF::0x1060") || admesy_device.Contains("0X23CF::0X1060"))
                {
                    AdmesyDevices[AdmesyDevicesFound] = admesy_device;
                    AdmesyDevicesFound++;
                }
            }

            if (AdmesyDevicesFound > 0)
            {
                status = libusbtmc.usbtmc_open(SelectedDevice.Append(AdmesyDevices[0]), ref DeviceHandle);
                if (status < 0)
                {
                    AdmesyLibError(status);
                }
            }
            errorstring = "no error";
            return true;
        }
        private int Bound(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value >= max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public void calculate_response(double[] datas, int samplecnt, double freq, double levelLo, double levelHi, ref double riseTime, ref double fallTime, ref RiseFallPositon positon)
        {
            //string json = JsonConvert.SerializeObject(datas);
            //File.WriteAllText("1.txt", json);
            //uint cnt = sy->size();
            double minValue = datas.Min(); ;
            double maxValue = datas.Max();
            //    double maxValue = *std::avg(sy->begin(), sy->end());
            //double sum = datas.Sum();
            int count = samplecnt;
            //double meanValue = sum / count;//计算均值
            int cnt = samplecnt;

            int numBins = 100; // 分箱数
            int[] histogram=new int[numBins];
            double binWidth = (maxValue - minValue) / numBins;

            double min_th = (maxValue - minValue) * levelLo+ minValue; //低阈值
            double max_th = (maxValue - minValue) * levelHi+minValue;//高阈值

            List<DataItem> valid_data = new List<DataItem>();//符合的数据啊

            //查找所有在高低阈值范围内的数据啊
            for (int i = 0; i < datas.Length; i++)
            {
                if ((datas[i] >= min_th) &&(datas[i] <= max_th))
                    valid_data.Add(new DataItem(i, datas[i]));
            }

            //List<double> vals = valid_data.Select(P => P.value).ToList();
            //string str = JsonConvert.SerializeObject(vals, Formatting.Indented);

            List<DataItem> peaks = new List<DataItem>(); // 存放上升沿索引
            List<DataItem> valleys = new List<DataItem>(); // 存放下降沿索引

            for (int i = 1; i < valid_data.Count - 1; i++)
            {
                if (((valid_data[i].value > valid_data[i - 1].value)) && (valid_data[i - 1].x == (valid_data[i].x - 1)))
                {
                    if (peaks.Count > 0)
                    {
                        if ((peaks[peaks.Count - 1].x + 1) ==  valid_data[i-1].x) //滤掉第二个上升沿
                        {
                            peaks.Add(valid_data[i-1]);
                        }
                        else
                        {
                            //第二个上升了啊
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        //上升沿的开始应该和最小值比较接近
                        if (Math.Abs(valid_data[i - 1].value - min_th) < 10)
                        {
                            peaks.Add(valid_data[i - 1]);
                        }
                    }
                }

                else if ((valid_data[i].value < valid_data[i - 1].value) && (valid_data[i - 1].x == (valid_data[i].x - 1)))
                {
                    if (valleys.Count > 0)
                    {
                        if ((valleys[valleys.Count - 1].x + 1) == valid_data[i - 1].x)
                        {
                            valleys.Add(valid_data[i - 1]);
                        }
                        else
                        {
                            //第二个上升了啊
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        //下降沿的开始应该是和最大值比较接近啊
                        if (Math.Abs(valid_data[i - 1].value - max_th) < 20)
                        {
                            valleys.Add(valid_data[i - 1]);
                        }
                    }
                }
            }

            if (peaks.Count > 1)
            {
                riseTime = (peaks[peaks.Count-1].x - peaks[0].x) * 1000 / freq;
                positon.rise_start = peaks[0].x;
                positon.rise_end = peaks[peaks.Count - 1].x;
            }
            if (valleys.Count > 1)
            {
                fallTime = (valleys[valleys.Count-1].x - valleys[0].x) * 1000 / freq;
                positon.fall_start = valleys[0].x;
                positon.fall_end = valleys[valleys.Count - 1].x;
            }
            //overshoot = (maxValue - topValue) / topValue * 100;
            //undershoot = (baseValue - minValue) / baseValue;
        }



    }

    class DataItem
    {
        public DataItem(int x, double value)
        {
            this.x = x;
            this.value = value;
        }

        public int x { get; set; }
        public double value { get; set; }
    }

    public class RiseFallPositon
    {
        public int rise_start { get; set; }
        public int rise_end { get; set; }
        public int fall_start { get; set; }
        public int fall_end { get; set; }
    }
}
