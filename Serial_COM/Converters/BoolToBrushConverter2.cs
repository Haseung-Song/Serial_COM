using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BoolToBrushConverter2
{
    public class BoolToBrushConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if (boolean == 1) ? (색상: OrangeRed) : (boolean == 0) ? (색상: LightGray)
            if (value is bool boolValue)
            {
                return boolValue ? new SolidColorBrush(Colors.OrangeRed) : new SolidColorBrush(Colors.LightGray);
            }
            return new SolidColorBrush(Colors.LightGray);    // 기본값: (색깔: LightGray)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
