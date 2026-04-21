using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using VisionCore;

namespace LCD
{
    public class EComTypeToVisibilityConverter : IValueConverter
    {
        private CommunicationModel ShowTYpe;
        public EComTypeToVisibilityConverter(CommunicationModel ShowType)
        {
            ShowTYpe = ShowType;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ShowTYpe == (CommunicationModel)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
