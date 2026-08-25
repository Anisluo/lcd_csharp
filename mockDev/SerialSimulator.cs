using System;
using System.IO.Ports;
using System.Text;
using MockDev.Instruments;

namespace MockDev
{
    /// <summary>
    /// 打开 mockDev 一端的串口，接收上位机指令 → 交给仪器生成响应 → 写回。
    /// </summary>
    public class SerialSimulator
    {
        private SerialPort _port;
        private readonly IInstrument _instrument;
        private readonly StringBuilder _rx = new StringBuilder();

        /// <summary>日志：(方向, 文本)，方向为 RX / TX / INFO。</summary>
        public event Action<string, string> Log;

        public SerialSimulator(IInstrument instrument) { _instrument = instrument; }

        public bool IsOpen => _port != null && _port.IsOpen;

        public void Start(string portName, int baud)
        {
            _port = new SerialPort(portName, baud, Parity.None, 8, StopBits.One)
            {
                Encoding = Encoding.ASCII,
                ReadTimeout = 500,
                WriteTimeout = 500,
                DtrEnable = true,
                RtsEnable = true
            };
            _port.DataReceived += OnData;
            _port.Open();
            Log?.Invoke("INFO", $"已打开 {portName} @ {baud}，模拟仪器：{_instrument.Name}");
        }

        private void OnData(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string chunk = _port.ReadExisting();
                if (string.IsNullOrEmpty(chunk)) return;
                _rx.Append(chunk);
                Log?.Invoke("RX", chunk);

                int consumed;
                string resp = _instrument.Process(_rx.ToString(), out consumed);
                if (resp != null)
                {
                    if (consumed >= _rx.Length) _rx.Clear();
                    else if (consumed > 0) _rx.Remove(0, consumed);
                    _port.Write(resp);
                    Log?.Invoke("TX", resp);
                }
            }
            catch (Exception ex)
            {
                Log?.Invoke("INFO", "接收处理异常: " + ex.Message);
            }
        }

        public void Stop()
        {
            try
            {
                if (_port != null)
                {
                    _port.DataReceived -= OnData;
                    if (_port.IsOpen) _port.Close();
                    _port.Dispose();
                    _port = null;
                    Log?.Invoke("INFO", "已关闭串口");
                }
            }
            catch { }
        }
    }
}
