using Awaitick.Core.ViewModels;
using Microsoft.UI.Dispatching;

namespace Awaitick.Views;

public sealed partial class CountdownDetailView : CountdownDetailViewBase
{
	private readonly DispatcherQueueTimer _timer;

	public CountdownDetailView()
	{
		this.InitializeComponent();
		_timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
		_timer.Interval = TimeSpan.FromMilliseconds(1000);
		_timer.Tick += _timer_Tick;
	}

	public override bool BlendsInTitleBar => true;

	private void _timer_Tick(DispatcherQueueTimer sender, object args)
	{
		if (ViewModel != null)
		{
			//update data on view model
			ViewModel.UpdateCountdowns();
		}
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

public abstract partial class CountdownDetailViewBase : PageBase<CountdownDetailViewModel>
{
}
