using CommunityToolkit.Mvvm.Messaging;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Services.Localization;
using EventCountdowns.Services.Navigation;
using EventCountdowns.Core.Messages;
using Microsoft.UI.Dispatching;
using Uno.Disposables;

namespace EventCountdowns.ViewModels;

public partial class WindowShellViewModel : ViewModelBase
{
	private readonly IWindowShellProvider _provider;
	private readonly INavigationService _navigationService;
	private readonly IMessenger _messenger;
	private RefCountDisposable? _refCountDisposable;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private string _loadingStatusMessage = "";

	public WindowShellViewModel(IWindowShellProvider provider, INavigationService navigationService, IMessenger messenger)
	{
		_provider = provider ?? throw new ArgumentNullException(nameof(provider));
		_navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
		_messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
		_messenger.Register<NavigatedMessage>(this, OnNavigated);
	}

	public string Title { get; set; } = Localizer.Instance.GetString("AppName");

	public bool CanGoBack => _navigationService.CanGoBack;

	[RelayCommand]
	public void GoBack() => _navigationService.GoBack();

	public IDisposable BeginLoading()
	{
		LoadingStatusMessage = "";
		if (_refCountDisposable != null && !_refCountDisposable.IsDisposed)
		{
			return _refCountDisposable.GetDisposable();
		}

		IsLoading = true;
		_refCountDisposable = new RefCountDisposable(Disposable.Create(
			() => // TODO: Await TryEnequeAsync
			{
#if __WASM__
				IsLoading = false;
				return;
#else
				if (_provider.DispatcherQueue.HasThreadAccess)
				{
					IsLoading = false;
				}
				else
				{
					_provider.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
					{
						if (_refCountDisposable == null || _refCountDisposable.IsDisposed)
						{
							IsLoading = false;
						}
					});
				}
#endif
			}));
		return _refCountDisposable;
	}

	private void OnNavigated(object recipient, NavigatedMessage message)
	{
		OnPropertyChanged(nameof(CanGoBack));
	}
}
