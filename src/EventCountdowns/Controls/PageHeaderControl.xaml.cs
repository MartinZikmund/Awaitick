using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Controls
{
    public sealed partial class PageHeaderControl : UserControl
After:
namespace EventCountdowns.Controls;

public sealed partial class PageHeaderControl : UserControl
*/
namespace EventCountdowns.Controls;

    public sealed partial class PageHeaderControl : UserControl
{
    public PageHeaderControl()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static DependencyProperty TitleProperty { get; } =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(PageHeaderControl),
            new PropertyMetadata(""));

/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
    }
}
After:
}
*/
    }
