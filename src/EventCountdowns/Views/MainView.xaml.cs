using EventCountdowns.Core.ViewModels;

namespace EventCountdowns.Views;

public sealed partial class MainView : MainViewBase
{
	private DispatcherTimer _timer;

	public MainView()
	{
		InitializeComponent();
		_timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 1000) };
		_timer.Tick += _timer_Tick;
	}

	private void _timer_Tick(object sender, object e)
	{
		//update data on view model
		ViewModel.UpdateCountdowns();
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

public partial class MainViewBase : PageBase<MainViewModel>
{
}
