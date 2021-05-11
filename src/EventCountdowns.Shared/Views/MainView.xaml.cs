using System;
using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EventCountdowns.Views
{
    public sealed partial class MainView : Page
    {
        private DispatcherTimer _timer = null;

        public MainView()
        {
            InitializeComponent();
            Model = IoC.GetRequiredService<MainViewModel>();
            DataContext = Model;
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 1000) };
            _timer.Tick += _timer_Tick;
        }
        
        public MainViewModel Model { get; private set; }

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
}
