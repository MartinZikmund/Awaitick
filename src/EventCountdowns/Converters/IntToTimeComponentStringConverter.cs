using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace EventCountdowns.Converters;

public class IntToTimeComponentStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return (value as int?)?.ToString("00", CultureInfo.CurrentCulture) ?? "";
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}
