using System;
using System.Globalization;
using System.Windows.Data;

namespace EnumToBoolConverter5
{
    public class EnumToBoolConverter5 : IValueConverter
    {
        /// <summary>
        /// [Convert]
        /// 즉, [Enum]과 [parameter]가 같은지 여부 확인
        /// [Enum] == [parameter] ?
        /// [equal] Yes => true:
        /// [equal] No => false;
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || parameter == null ? false : (object)value.Equals(parameter);
        }

        /// <summary>
        /// [ConvertBack]
        /// [RadioButton]의 [IsChecked] 속성
        /// 즉, [RadioButton]이 선택된 상태? [XAML]에서 [ConverterParameter]로 전달된 [Enum] 값을 반환!
        /// 선택 (X) ? 바인딩에 변경은 없음.
        /// [Enum] == [parameter] ?
        /// [true]: => [parameter] 반환
        /// [false] => [DoNothing] 반환
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isChecked && isChecked ? parameter : Binding.DoNothing;
        }

    }

}
