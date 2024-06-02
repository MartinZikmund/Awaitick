using System.Reflection;
using CommunityToolkit.Mvvm.Messaging;
using Windows.UI.Core;
using EventCountdowns.Core.Messages;

namespace EventCountdowns.Services.Navigation;

public class NavigationService : INavigationService
{
	private readonly Dictionary<string, Type> _views = new();
	private readonly IFrameProvider _frameProvider;
	private readonly IMessenger _messenger;

	public NavigationService(IFrameProvider frameProvider, IMessenger messenger)
	{
		_frameProvider = frameProvider ?? throw new ArgumentNullException(nameof(frameProvider));
		_messenger = messenger;
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

	public void Navigate<TViewModel>(object? parameter)
	{
		if (!TryFindViewForViewModel(typeof(TViewModel), out var viewType))
		{
			throw new InvalidOperationException($"ViewModel type {typeof(TViewModel).Name} is not registered for navigation.");
		}

		Frame.Navigate(viewType, parameter);
	}

	private bool TryFindViewForViewModel(Type viewModelType, out Type? viewType)
	{
		if (!viewModelType.Name.EndsWith("ViewModel", StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidOperationException("ViewModel name must end with 'ViewModel' by convention.");
		}

		var viewModelName = viewModelType.Name;
		return _views.TryGetValue(viewModelName.Substring(0, viewModelName.Length - "Model".Length), out viewType);
	}

	public void RegisterViewsFromAssembly(Assembly sourceAssembly)
	{
		// TODO: Avoid reflection
		var pageType = typeof(Page);
		var pages = sourceAssembly.GetTypes().Where(t => pageType.IsAssignableFrom(t) && t.Name.EndsWith("View", StringComparison.OrdinalIgnoreCase)).ToArray();
		foreach (var viewType in pages)
		{
			_views.Add(viewType.Name, viewType);
		}
	}

	public void Initialize() =>
		SystemNavigationManager.GetForCurrentView().BackRequested += NavigationManagerBackRequested;

	private void NavigationManagerBackRequested(object? sender, BackRequestedEventArgs? e) => GoBack();

	public void ClearBackStack() => Frame.BackStack.Clear();
}
