using Microsoft.UI.Xaml.Data;

namespace Awaitick.Converters;

public class NonNullToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) => value is null ? Visibility.Collapsed : Visibility.Visible;
	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
