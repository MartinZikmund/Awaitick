using Microsoft.UI.Xaml.Data;

namespace Awaitick.Converters;

public class EnumValueToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		if (parameter == null) throw new ArgumentNullException(nameof(parameter));
		var enumValue = value?.ToString()?.ToLowerInvariant();
		var compareValue = parameter?.ToString()?.ToLowerInvariant();
		return string.Equals(enumValue, compareValue, StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
