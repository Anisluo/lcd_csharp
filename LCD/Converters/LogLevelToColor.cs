using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using VisionCore;

namespace LCD
{
    public class LogLevelToColor
    {
        public Brush Convert(LogLevel value)
        {
            switch (value)
            {
                case LogLevel.Debug:
                    return Brushes.DarkOrange;
                case LogLevel.Info:
                    break;
                case LogLevel.Warn:
                    break;
                case LogLevel.Error:
                    return Brushes.DarkRed;
                case LogLevel.Fatal:
                    return Brushes.DarkRed;
                case LogLevel.Comm:
                    return Brushes.DarkBlue;
                default:
                    break;
            }
            return Brushes.Black;
        }
    }
}