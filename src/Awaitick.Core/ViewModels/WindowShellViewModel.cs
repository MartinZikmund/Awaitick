using Awaitick.Core.Messages;
using Awaitick.Core.ViewModels;
using Awaitick.Services.Localization;
using Awaitick.Services.Navigation;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;
using Uno.Disposables;

namespace Awaitick.ViewModels;

public partial class WindowShellViewModel : ViewModelBase
{
	private readonly IWindowShellProvider _provider;
	private readonly INavigationService _navigationService;
	private readonly IMessenger _messenger;
	private RefCountDisposable? _refCountDisposable;

	public WindowShellViewModel(IWindowShellProvider provider, INavigationService navigationService, IMessenger messenger)
	{
		_provider = provider ?? throw new ArgumentNullException(nameof(provider));
		_navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
		_messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
		_messenger.Register<NavigationChangedMessage>(this, OnNavigated);
	}

	public string Title { get; set; } = Localizer.Instance.GetString("AppName");

	[ObservableProperty]
	public partial string LoadingStatusMessage { get; set; } = "";

	[RelayCommand]
	public void GoBack() => _navigationService.GoBack();

	public bool CanGoBack => _navigationService.CanGoBack;

	public IDisposable BeginLoading()
	{
		LoadingStatusMessage = "";
		if (_refCountDisposable != null && !_refCountDisposable.IsDisposed)
		{
			return _refCountDisposable.GetDisposable();
		}

		IsWorking = true;
		_refCountDisposable = new RefCountDisposable(Disposable.Create(
			() => // TODO: Await TryEnequeAsync
			{
#if __WASM__
				IsWorking = false;
				return;
#else
				if (_provider.DispatcherQueue.HasThreadAccess)
				{
					IsWorking = false;
				}
				else
				{
					_provider.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
					{
						if (_refCountDisposable == null || _refCountDisposable.IsDisposed)
						{
							IsWorking = false;
						}
					});
				}
#endif
			}));
		return _refCountDisposable;
	}

	private void OnNavigated(object recipient, NavigationChangedMessage message) => OnPropertyChanged(nameof(CanGoBack));
}
