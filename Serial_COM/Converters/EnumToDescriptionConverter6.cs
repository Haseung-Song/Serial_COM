using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace EnumToDescriptionConverter6
{
    public class EnumToDescriptionConverter6 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 1) [value]가 [Enum] 타입인지 확인, 맞으면, GetEnumDescription(enumValue)를 호출
            // 2) [value]가 [Enum] 타입인지 확인, 아니면, value.ToString()을 호출, 문자열 반환
            return value is Enum enumValue ? GetEnumDescription(enumValue) : value.ToString();
        }

        private string GetEnumDescription(Enum enumValue)
        {
            // [Enum] 타입에서 해당 이름의 필드 정보를 객체로 가져옴.
            FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());

            // 해당 필드에 [DescriptionAttribute] 특성의 존재 여부 확인 후, 해당 특성을 가져옴.
            DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();

            // 1) [DescriptionAttribute] 특성이 있으면, 해당된 [Description] 속성 값을 가져 옴.
            // 2) [DescriptionAttribute] 특성이 없으면, [Enum] 타입의 기본 문자열 값을 반환 함.
            return attribute?.Description ?? enumValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
