using System;
using Microsoft.UI.Xaml.Data;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Core.Converters
{
	public class IntToTimeComponentStringConverter : IValueConverter
After:
namespace EventCountdowns.Core.Converters;

	public class IntToTimeComponentStringConverter : IValueConverter
*/
namespace EventCountdowns.Core.Converters;

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
