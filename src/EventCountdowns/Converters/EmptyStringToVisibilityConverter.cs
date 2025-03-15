using Microsoft.UI.Xaml.Data;

namespace EventCountdowns.Converters;

public class EmptyStringToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) => string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
