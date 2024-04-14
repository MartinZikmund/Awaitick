using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Core.Converters
{
	public class EnumValueToVisibilityConverter : IValueConverter
After:
namespace EventCountdowns.Core.Converters;

	public class EnumValueToVisibilityConverter : IValueConverter
*/
namespace EventCountdowns.Core.Converters;

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
