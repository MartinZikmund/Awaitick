using System.ComponentModel;
using Awaitick.Core.Infrastructure;
using Awaitick.Core.Messages;
using Awaitick.Core.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;

namespace Awaitick.Views;

public sealed partial class CountdownDetailView : CountdownDetailViewBase
{
	private readonly DispatcherQueueTimer _timer;
	private readonly DispatcherQueueTimer _autoHideTimer;
	private bool _overlayVisible = true;
	private bool _subscribedToViewModel;

	public CountdownDetailView()
	{
		this.InitializeComponent();
		_timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
		_timer.Interval = TimeSpan.FromMilliseconds(1000);
		_timer.Tick += _timer_Tick;

		_autoHideTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
		_autoHideTimer.Interval = TimeSpan.FromSeconds(3);
		_autoHideTimer.IsRepeating = false;
		_autoHideTimer.Tick += AutoHideTimer_Tick;

		FadeOutStoryboard.Completed += FadeOutStoryboard_Completed;

		Loaded += CountdownDetailView_Loaded;
		Unloaded += CountdownDetailView_Unloaded;
	}

	public override bool BlendsInTitleBar => true;

	private void CountdownDetailView_Loaded(object sender, RoutedEventArgs e)
	{
		SubscribeToViewModel();
	}

	private void CountdownDetailView_Unloaded(object sender, RoutedEventArgs e)
	{
		UnsubscribeFromViewModel();
	}

	private void SubscribeToViewModel()
	{
		if (!_subscribedToViewModel && ViewModel != null)
		{
			ViewModel.PropertyChanged += ViewModel_PropertyChanged;
			_subscribedToViewModel = true;
		}
	}

	private void UnsubscribeFromViewModel()
	{
		if (_subscribedToViewModel && ViewModel != null)
		{
			ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
			_subscribedToViewModel = false;
		}
	}

	private void _timer_Tick(DispatcherQueueTimer sender, object args)
	{
		if (ViewModel != null)
		{
			//update data on view model
			ViewModel.UpdateCountdowns();
		}
	}

	private void AutoHideTimer_Tick(DispatcherQueueTimer sender, object args)
	{
		if (_overlayVisible)
		{
			FadeOutOverlay();
		}
	}

	protected override void OnNavigatedTo(NavigationEventArgs e)
	{
		base.OnNavigatedTo(e);
		_timer.Start();
		_autoHideTimer.Start();
		SubscribeToViewModel();
	}

	protected override void OnNavigatedFrom(NavigationEventArgs e)
	{
		base.OnNavigatedFrom(e);
		_timer.Stop();
		_autoHideTimer.Stop();

		// Restore overlay and notify WindowShell to show title bar
		FadeInStoryboard.Stop();
		FadeOutStoryboard.Stop();
		OverlayContainer.Visibility = Visibility.Visible;
		OverlayContainer.Opacity = 1;
		OverlayContainer.IsHitTestVisible = true;
		_overlayVisible = true;
		IoC.GetRequiredService<IMessenger>().Send(new OverlayVisibilityChangedMessage(true));

		UnsubscribeFromViewModel();
	}

	private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(CountdownDetailViewModel.IsFullScreen))
		{
			OnFullScreenChanged(ViewModel!.IsFullScreen);
		}
	}

	private void RootGrid_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
	{
		HandleUserInteraction();
	}

	private void RootGrid_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
	{
		HandleUserInteraction();
	}

	private void HandleUserInteraction()
	{
		if (!_overlayVisible)
		{
			FadeInOverlay();
		}

		// Reset the auto-hide timer
		_autoHideTimer.Stop();
		_autoHideTimer.Start();
	}

	private void OnFullScreenChanged(bool isFullScreen)
	{
		if (!isFullScreen)
		{
			// Exiting full screen - show overlay and reset timer
			FadeInOverlay();
			_autoHideTimer.Stop();
			_autoHideTimer.Start();
		}
	}

	private void FadeInOverlay()
	{
		FadeOutStoryboard.Stop();
		OverlayContainer.Visibility = Visibility.Visible;
		FadeInStoryboard.Begin();
		OverlayContainer.IsHitTestVisible = true;
		_overlayVisible = true;
		IoC.GetRequiredService<IMessenger>().Send(new OverlayVisibilityChangedMessage(true));
	}

	private void FadeOutOverlay()
	{
		FadeInStoryboard.Stop();
		FadeOutStoryboard.Begin();
		OverlayContainer.IsHitTestVisible = false;
		_overlayVisible = false;
		IoC.GetRequiredService<IMessenger>().Send(new OverlayVisibilityChangedMessage(false));
	}

	private void FadeOutStoryboard_Completed(object? sender, object e)
	{
		OverlayContainer.Visibility = Visibility.Collapsed;
	}
}

public abstract partial class CountdownDetailViewBase : PageBase<CountdownDetailViewModel>
{
}
