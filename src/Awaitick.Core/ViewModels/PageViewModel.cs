using Awaitick.Core.ViewModels;
using Awaitick.Services.Navigation;
using CommunityToolkit.Mvvm.Messaging;

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
	private string _title = "";

	public virtual void ViewCreated() { }

	public virtual void ViewLoading() { }

	public virtual void ViewLoaded() { }

	public virtual void ViewUnloaded() { }

	public void ViewNavigatedToInternal(object? parameter)
	{
		OnPropertyChanged(nameof(CanGoBack));
		ViewNavigatedTo(parameter);
	}

	public virtual void ViewNavigatedTo(object? parameter) { }

	/// <summary>
	/// Called when the page is navigated away from.
	/// </summary>
	public virtual void ViewNavigatedFrom() { }
}
