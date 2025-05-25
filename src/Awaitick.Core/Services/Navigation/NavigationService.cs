using System.Reflection;
using CommunityToolkit.Mvvm.Messaging;
using Windows.UI.Core;
using Awaitick.Core.Messages;
using System.Diagnostics.CodeAnalysis;
using Awaitick.Core.Services.Navigation;

namespace Awaitick.Services.Navigation;

public class NavigationService : INavigationService
{
	private readonly IFrameProvider _frameProvider;
	private readonly IMessenger _messenger;
	private readonly IViewProvider _viewProvider;

	public NavigationService(IFrameProvider frameProvider, IMessenger messenger, IViewProvider viewProvider)
	{
		_frameProvider = frameProvider ?? throw new ArgumentNullException(nameof(frameProvider));
		_messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
		_viewProvider = viewProvider ?? throw new ArgumentNullException(nameof(viewProvider));
		_frameProvider.GetForCurrentView().Navigated += OnNavigated;
	}

	private void OnNavigated(object sender, NavigationEventArgs e) => _messenger.Send(new NavigatedMessage());

	private Frame Frame => _frameProvider.GetForCurrentView();

	public bool CanGoBack => Frame.CanGoBack;

	public bool GoBack()
	{
		if (Frame.CanGoBack)
		{
			Frame.GoBack();
			return true;
		}
		return false;
	}

	public void Navigate<TViewModel>() => Navigate<TViewModel>(null);

	public async void Navigate<TViewModel>(object? parameter)
	{
		// This is needed, as Frame would not navigate in case another navigation is currently in progress.
		await Task.Yield();

		if (!_viewProvider.TryFindViewForViewModel(typeof(TViewModel), out var viewType))
		{
			throw new InvalidOperationException($"ViewModel type {typeof(TViewModel).Name} is not registered for navigation.");
		}

		Frame.Navigate(viewType, parameter);
	}

	public void Initialize() => SystemNavigationManager.GetForCurrentView().BackRequested += NavigationManagerBackRequested;

	private void NavigationManagerBackRequested(object? sender, BackRequestedEventArgs? e) => GoBack();

	public void ClearBackStack() => Frame.BackStack.Clear();
}
