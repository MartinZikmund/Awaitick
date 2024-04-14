using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Core.Converters
{
	public class ItemClickEventArgsConverter : IValueConverter
After:
namespace EventCountdowns.Core.Converters;

	public class ItemClickEventArgsConverter : IValueConverter
*/
namespace EventCountdowns.Core.Converters;

public class ItemClickEventArgsConverter : IValueConverter
{
	public object? Convert(object value, Type targetType, object parameter, string language)
	{
		var itemClickEventArgs = value as ItemClickEventArgs;
		return itemClickEventArgs?.ClickedItem;
	}

	public object? ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}
