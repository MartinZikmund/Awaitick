using Microsoft.UI.Xaml.Data;

namespace EventCountdowns.Converters;

public class NonEmptyStringToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}
