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
		NavigationCacheMode = NavigationCacheMode.Required;
		_timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
		_timer.Interval = TimeSpan.FromMilliseconds(1000);
		_timer.Tick += _timer_Tick;

		Unloaded += MainView_Unloaded;
	}

	private void MainView_Unloaded(object sender, RoutedEventArgs e) => _timer.Stop();

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

	private bool _isListLayout;

	private readonly UniformGridLayout _gridLayout = new()
	{
		ItemsJustification = UniformGridLayoutItemsJustification.Center,
		ItemsStretch = UniformGridLayoutItemsStretch.Fill,
		MaximumRowsOrColumns = 3,
		MinColumnSpacing = 20,
		MinItemHeight = 320,
		MinItemWidth = 480,
		MinRowSpacing = 20,
		Orientation = Orientation.Horizontal,
	};

	private readonly StackLayout _listLayout = new()
	{
		Spacing = 20,
	};

	private void EventsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		var useList = e.NewSize.Width < 520;
		if (useList != _isListLayout)
		{
			_isListLayout = useList;
			CountdownItemsView.Layout = useList ? _listLayout : _gridLayout;
		}
	}
}

public abstract partial class MainViewBase : PageBase<MainViewModel>
{
}
