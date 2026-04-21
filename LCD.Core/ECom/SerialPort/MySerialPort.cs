using System;
using System.Text;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using VisionCore;
using System.Diagnostics;

namespace PCComm
{
    public delegate string SerialPortDataReceivedFunction(SerialPort serialPort);//解析 串口数据的规则

    class MySerialPort
    {
        public event ReceiveString OnReceiveString;//接受数据事件

        public SerialPortDataReceivedFunction DataReceivedFunction { get; set; } = null;//数据接收委托


        #region Manager Enums


        /// <summary>
        /// 消息类型
        /// </summary>
        public enum MessageType
        {
            /// <summary> 呼入 </summary>
            Incoming,
            /// <summary> 呼出 </summary>
            Outgoing,
            /// <summary> 常规 </summary>
            Normal,
            /// <summary> 警告 </summary>
            Warning,
            /// <summary> 错误 </summary>
            Error
        };
        #endregion

        #region 变量(私有)
        //property variables
        private string _baudRate = string.Empty;
        private string _parity = string.Empty;
        private string _stopBits = string.Empty;
        private string _dataBits = string.Empty;
        private string _portName = string.Empty;
        private bool _rtsEnable = true;
        private bool _dtrEnable = true;
        //global manager variables
        //private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        private SerialPort comPort = new SerialPort();
        #endregion

        #region 属性(公有)



        /// <summary>
        /// Return port status
        /// </summary>
        public bool isPortOpen
        {
            get { return comPort.IsOpen; }
        }

        /// <summary>
        /// Property to hold the BaudRate
        /// of our manager class
        /// </summary>
        public string BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        /// <summary>
        /// property to hold the Parity
        /// of our manager class
        /// </summary>
        public string Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        /// <summary>
        /// property to hold the StopBits
        /// of our manager class
        /// </summary>
        public string StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        /// <summary>
        /// property to hold the DataBits
        /// of our manager class
        /// </summary>
        public string DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        /// <summary>
        /// property to hold the PortName
        /// of our manager class
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }


        #endregion

        #region Manager Constructors

        /// <summary>
        /// Comstructor to set the properties of our
        /// serial port communicator to nothing
        /// </summary>
        public MySerialPort()
        {
            _baudRate = string.Empty;
            _parity = string.Empty;
            _stopBits = string.Empty;
            _dataBits = string.Empty;
            _portName = "COM1";
            comPort.Encoding = Encoding.Default;
            //add event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);

        }
        #endregion

        #region WriteData
        public bool WriteData(string msg, bool isSendByHex, bool IsAutoNewLine=true)
        {
            try
            {
                if (!(comPort.IsOpen == true))
                {
                    DisplayData(MessageType.Error, $"[{PortName}] 串口未打开!");
                    return false;
                }
                if (IsAutoNewLine) msg += Environment.NewLine;
                if (isSendByHex == true)
                {
                    byte[] bytes = HexTool.HexToByte(HexTool.StrToHexStr(msg));
                    comPort.Write(bytes, 0, bytes.Length);
                }
                else
                {
                    comPort.Write(msg);
                }

                //send the message to the port
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }

        }
        #endregion

        #region DisplayData
        /// <summary>
        /// 展示数据(界面上以及ShowMessageBox)
        /// on the screen
        /// </summary>
        /// <param name="type">MessageType of the message</param>
        /// <param name="msg">Message to display</param>
        [STAThread]
        private void DisplayData(MessageType type, string msg)
        {
            switch (type)
            {
                case MessageType.Incoming:
                    Log.Debug(msg);
                    break;
                case MessageType.Outgoing:
                    Log.Debug(msg);
                    break;
                case MessageType.Normal:
                    Log.Info(msg);
                    break;
                case MessageType.Warning:
                    Log.Warn(msg);
                    break;
                case MessageType.Error:
                    Log.Error(msg);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region OpenPort
        public bool OpenPort()
        {
            try
            {
                ClosePort();

                //set the properties of our SerialPort Object
                comPort.BaudRate = int.Parse(_baudRate);    //BaudRate
                comPort.DataBits = int.Parse(_dataBits);    //DataBits
                comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits);    //StopBits
                comPort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity);    //Parity
                comPort.PortName = _portName;   //PortName
                comPort.RtsEnable = _rtsEnable;   //PortName
                comPort.DtrEnable = _dtrEnable;   //PortName
                //now open the port
                comPort.Open();
                //display message
                DisplayData(MessageType.Normal, "Port opened at " + DateTime.Now);
                //return true
                return true;
            }
            catch (Exception ex)
            {
                DisplayData(MessageType.Error, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 如果是打开的就关闭
        /// </summary>
        public void ClosePort()
        {
            if (comPort.IsOpen == true) comPort.Close();
        }

        #endregion

        #region comPort_DataReceived
        /// <summary>
        /// method that will be called when theres data waiting in the buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string msg = "";
            //Console.WriteLine(comPort.ReadExisting().Trim());
            

            if (DataReceivedFunction == null)
            {
                Thread.Sleep(300);//一定要延迟, 才能直接去读.因为流还没有写完,就会触发事件进入 magical 2019-3-1 20:21:43
                msg = comPort.ReadExisting().Trim();
            }
            else
            {
                msg = DataReceivedFunction.Invoke(comPort);
            }

            if (msg.Length < 6)//长度小于6额外处理？
            {
                ;
                // string s = comPort.ReadExisting().Trim();
            }

            //display the data to the user
            //if (msg.Length > 0)
            //    DisplayData(MessageType.Incoming, msg + "\n");

            OnReceiveString?.Invoke(msg);
        }
        #endregion
    }
}
