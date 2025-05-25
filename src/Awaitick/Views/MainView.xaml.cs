using Awaitick.Core.Models;
using Awaitick.Core.ViewModels;
using Microsoft.UI.Dispatching;

namespace Awaitick.Views;

public sealed partial class MainView : MainViewBase
{
	private DispatcherQueueTimer _timer;

	public MainView()
	{
		InitializeComponent();
		_timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
		_timer.Interval = TimeSpan.FromMilliseconds(1000);
		_timer.Tick += _timer_Tick;
	}

	private void _timer_Tick(DispatcherQueueTimer sender, object args)
	{
		if (ViewModel is not null)
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

	private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
	{
		if (args.InvokedItem is CountdownViewModel vm)
		{
			vm.GoToDetail();
		}
    }
}

public abstract partial class MainViewBase : PageBase<MainViewModel>
{
}
