using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace Awaitick.Converters;

public class FullScreenIconConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) =>
		value is true ? Symbol.BackToWindow : Symbol.FullScreen;

	public object ConvertBack(object value, Type targetType, object parameter, string language) =>
		throw new NotSupportedException();
}
