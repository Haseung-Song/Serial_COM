using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BoolToBrushConverter1
{
    public class BoolToBrushConverter1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if (boolean == 1) ? (색깔: LightGreen) : (boolean == 0) ? (색깔: LightGray)
            if (value is bool boolValue)
            {
                return boolValue ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            }
            return new SolidColorBrush(Colors.LightGray);     // 기본값: (색깔: LightGray)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
