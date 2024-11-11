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
            // if (boolean == 1) ? (색깔: PaleVioletRed) : (boolean == 0) ? (색깔: Black)
            if (value is bool boolValue)
            {
                return boolValue ? new SolidColorBrush(Colors.PaleVioletRed) : new SolidColorBrush(Colors.DarkGray);
            }
            return new SolidColorBrush(Colors.PaleVioletRed); // 기본값: (색깔: PaleVioletRed)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
