using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BoolToBrushConverter4
{
    public class BoolToBrushConverter4 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if (boolean == 1) ? (색깔: White) : (boolean == 0) ? (색깔: DarkGray)
            if (value is bool boolValue)
            {
                return boolValue ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.DarkGray);
            }
            return new SolidColorBrush(Colors.DarkGray); // 기본값: (색깔: DarkGray)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
