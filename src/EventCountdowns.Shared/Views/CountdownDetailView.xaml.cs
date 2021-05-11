using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using EventCountdowns.Core.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace EventCountdowns.Views
{
    public sealed partial class CountdownDetailView : CountdownDetailViewBase
    {
        private readonly DispatcherTimer _timer = null;

        public CountdownDetailView()
        {
            this.InitializeComponent();
            this.DataContextChanged += CountdownDetailView_DataContextChanged;
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

        private void CountdownDetailView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Model = args.NewValue as CountdownDetailViewModel;
        }

        public CountdownDetailViewModel Model { get; private set; }
    }

    public partial class CountdownDetailViewBase : ViewBase<CountdownDetailViewModel>
    {
    }
}
