using Microsoft.UI.Xaml.Data;

namespace EventCountdowns.Converters;

public class EnumValueToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		if (parameter == null) throw new ArgumentNullException(nameof(parameter));
		string enumValue = value.ToString().ToLowerInvariant();
		string compareValue = parameter.ToString().ToLowerInvariant();
		return enumValue == compareValue ? Visibility.Visible : Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}
