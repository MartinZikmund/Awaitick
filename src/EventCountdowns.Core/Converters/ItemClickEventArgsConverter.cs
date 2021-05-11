using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace EventCountdowns.Core.Converters
{
    public class ItemClickEventArgsConverter : IValueConverter
    {
        public object? Convert( object value, Type targetType, object parameter, string language )
        {
            var itemClickEventArgs = value as ItemClickEventArgs;
            return itemClickEventArgs?.ClickedItem;
        }

        public object? ConvertBack( object value, Type targetType, object parameter, string language )
        {
            throw new NotImplementedException();
        }
    }
}
