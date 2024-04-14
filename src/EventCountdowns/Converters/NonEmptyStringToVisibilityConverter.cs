using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Core.Converters
{
	public class NonEmptyStringToVisibilityConverter : IValueConverter
After:
namespace EventCountdowns.Core.Converters;

	public class NonEmptyStringToVisibilityConverter : IValueConverter
*/
namespace EventCountdowns.Core.Converters;

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
