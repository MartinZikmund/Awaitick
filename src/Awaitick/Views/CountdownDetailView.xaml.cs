using System.ComponentModel;
using Awaitick.Core.ViewModels;
using Microsoft.UI.Dispatching;

namespace Awaitick.Views;

public sealed partial class CountdownDetailView : CountdownDetailViewBase
{
	private readonly DispatcherQueueTimer _timer;
	private readonly DispatcherQueueTimer _fullScreenHideTimer;
	private bool _overlayVisible = true;
	private bool _subscribedToViewModel;

	public CountdownDetailView()
	{
		this.InitializeComponent();
		_timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
		_timer.Interval = TimeSpan.FromMilliseconds(1000);
		_timer.Tick += _timer_Tick;

		_fullScreenHideTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
		_fullScreenHideTimer.Interval = TimeSpan.FromSeconds(3);
		_fullScreenHideTimer.IsRepeating = false;
		_fullScreenHideTimer.Tick += FullScreenHideTimer_Tick;

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

	private void FullScreenHideTimer_Tick(DispatcherQueueTimer sender, object args)
	{
		if (ViewModel?.IsFullScreen == true && _overlayVisible)
		{
			FadeOutOverlay();
		}
	}

	protected override void OnNavigatedTo(NavigationEventArgs e)
	{
		base.OnNavigatedTo(e);
		_timer.Start();
		SubscribeToViewModel();
	}

	protected override void OnNavigatedFrom(NavigationEventArgs e)
	{
		base.OnNavigatedFrom(e);
		_timer.Stop();
		_fullScreenHideTimer.Stop();
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
		if (ViewModel?.IsFullScreen != true)
		{
			return;
		}

		if (!_overlayVisible)
		{
			FadeInOverlay();
		}

		// Reset the auto-hide timer
		_fullScreenHideTimer.Stop();
		_fullScreenHideTimer.Start();
	}

	private void OnFullScreenChanged(bool isFullScreen)
	{
		if (isFullScreen)
		{
			// Start the auto-hide timer
			_fullScreenHideTimer.Start();
		}
		else
		{
			// Show overlay and stop the timer
			_fullScreenHideTimer.Stop();
			FadeInOverlay();
		}
	}

	private void FadeInOverlay()
	{
		FadeOutStoryboard.Stop();
		FadeInStoryboard.Begin();
		OverlayContainer.IsHitTestVisible = true;
		_overlayVisible = true;
	}

	private void FadeOutOverlay()
	{
		FadeInStoryboard.Stop();
		FadeOutStoryboard.Begin();
		OverlayContainer.IsHitTestVisible = false;
		_overlayVisible = false;
	}
}

public abstract partial class CountdownDetailViewBase : PageBase<CountdownDetailViewModel>
{
}
