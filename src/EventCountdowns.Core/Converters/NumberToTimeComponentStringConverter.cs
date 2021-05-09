using System;
using Windows.UI.Xaml.Data;

namespace EventCountdowns.Core.Converters
{
    public class IntToTimeComponentStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as int?)?.ToString("00") ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
