using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LCD
{
    public class Converters
    {
        //bool转红绿
        public static BoolToGreenRedConverter BoolToBackground = new BoolToGreenRedConverter();
        public static EComTypeToVisibilityConverter ShowCOM = new EComTypeToVisibilityConverter(VisionCore.CommunicationModel.COM);
        public static EComTypeToVisibilityConverter ShowUDP = new EComTypeToVisibilityConverter(VisionCore.CommunicationModel.UDP);
        public static EComTypeToVisibilityConverter ShowTcpServ = new EComTypeToVisibilityConverter(VisionCore.CommunicationModel.TcpServer);
        public static EComTypeToVisibilityConverter ShowTcpClnt = new EComTypeToVisibilityConverter(VisionCore.CommunicationModel.TcpClient);

        public static LogLevelToColor LogLevelToColor { get; set; } = new LogLevelToColor();
    }
}
