using CommunityToolkit.Mvvm.Messaging;
using Awaitick.Core.ViewModels;
using Awaitick.Services.Navigation;

namespace Awaitick.Core.ViewModels;

public abstract partial class PageViewModel : ViewModelBase
{
	private readonly INavigationService _navigationService;
	private readonly IMessenger _messenger;

	protected PageViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
	}

	public bool CanGoBack => NavigationService.CanGoBack;

	public INavigationService NavigationService => _navigationService;

	[RelayCommand]
	public virtual void GoBack() => NavigationService.GoBack();

	[ObservableProperty]
	public partial string Title { get; set; } = "";

	public virtual void ViewCreated() { }

	public virtual void ViewLoading() { }

	public virtual void ViewLoaded() { }

	public virtual void ViewUnloaded() { }

	internal void ViewNavigatedToInternal(object? parameter)
	{
		OnPropertyChanged(nameof(CanGoBack));
		ViewNavigatedTo(parameter);
	}

	internal void ViewNavigatedFromInternal(object? parameter)
	{
		ViewNavigatedFrom(parameter);
	}

	protected virtual void ViewNavigatedTo(object? parameter) { }

	protected virtual void ViewNavigatedFrom(object? parameter) { }
}
