using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace EventCountdowns.Core.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public bool Inverted { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (!Inverted)
        {
            return value is bool boolValue && boolValue ?
                Visibility.Visible : Visibility.Collapsed;
        }
        else
        {
            return value is bool boolValue && boolValue ?
                Visibility.Collapsed : Visibility.Visible;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (!Inverted)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
        else
        {
            return value is Visibility visibility && visibility == Visibility.Collapsed;
        }
    }
}
