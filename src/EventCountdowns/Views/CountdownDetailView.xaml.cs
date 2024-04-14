using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using EventCountdowns.Core.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace EventCountdowns.Views;

public sealed partial class CountdownDetailView : CountdownDetailViewBase
{
    private readonly DispatcherTimer _timer;

	public CountdownDetailView()
    {
        this.InitializeComponent();
        _timer = new DispatcherTimer();
        _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        _timer.Tick += _timer_Tick;
    }

    private void _timer_Tick(object sender, object e)
    {
        //update data on view model
        Model.UpdateCountdowns();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _timer.Start();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        _timer.Stop();
    }
}

public partial class CountdownDetailViewBase : ViewBase<CountdownDetailViewModel>
{
}
