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

	private const double ListLayoutThreshold = 520;
	private const double ScrollViewerEdgePadding = 12;
	private const double GridMinItemWidth = 480;
	private const double GridMinItemHeight = 320;
	private const double GridSpacing = 20;

	private readonly UniformGridLayout _gridLayout = new()
	{
		ItemsJustification = UniformGridLayoutItemsJustification.Center,
		ItemsStretch = UniformGridLayoutItemsStretch.Fill,
		MaximumRowsOrColumns = 3,
		MinColumnSpacing = GridSpacing,
		MinItemHeight = GridMinItemHeight,
		MinItemWidth = GridMinItemWidth,
		MinRowSpacing = GridSpacing,
		Orientation = Orientation.Horizontal,
	};

	private readonly StackLayout _listLayout = new()
	{
		Spacing = 20,
	};

	private void EventsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (e.NewSize.Width < ListLayoutThreshold)
		{
			if (CountdownItemsView.Layout != _listLayout)
			{
				CountdownItemsView.Layout = _listLayout;
			}

			return;
		}

		// Compute how many columns fit the available width (minus ScrollViewer padding),
		// allowing for the inter-column spacing. No upper cap, so wide/ultra-wide displays
		// can show more than three columns.
		var availableWidth = e.NewSize.Width - (ScrollViewerEdgePadding * 2);
		var columns = Math.Max(1, (int)Math.Floor((availableWidth + GridSpacing) / (GridMinItemWidth + GridSpacing)));

		_gridLayout.MaximumRowsOrColumns = columns;

		// Reassign the layout when transitioning from the list (or the XAML-seeded layout)
		// to force the ItemsView to pick up the responsive grid; the column change above
		// re-lays out subsequent resizes in place.
		if (CountdownItemsView.Layout != _gridLayout)
		{
			CountdownItemsView.Layout = _gridLayout;
		}
	}
}

public abstract partial class MainViewBase : PageBase<MainViewModel>
{
}
