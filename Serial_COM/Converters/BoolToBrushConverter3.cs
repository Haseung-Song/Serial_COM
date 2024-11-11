using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BoolToBrushConverter3
{
    public class BoolToBrushConverter3 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if (boolean == 1) ? (색깔: AliceBlue) : (boolean == 0) ? (색깔: Black)
            if (value is bool boolValue)
            {
                return boolValue ? new SolidColorBrush(Colors.AliceBlue) : new SolidColorBrush(Colors.DarkGray);
            }
            return new SolidColorBrush(Colors.DarkGray);  // 기본값: (색깔: DarkGray)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
