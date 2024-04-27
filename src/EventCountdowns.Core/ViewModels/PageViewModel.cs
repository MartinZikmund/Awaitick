
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Services.Navigation;

namespace EventCountdowns.ViewModels;

public abstract partial class PageViewModel : ViewModelBase
{
	private readonly INavigationService _navigationService;

	protected PageViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public bool CanGoBack => _navigationService.CanGoBack;

	[RelayCommand]
	public void GoBack() => _navigationService.GoBack();

	[ObservableProperty]
	private string _title = "";

	public virtual void ViewCreated() { }

	public virtual void ViewLoading() { }

	public virtual void ViewLoaded() { }

	public virtual void ViewUnloaded() { }

	public virtual void ViewNavigatedTo(object? parameter) { }
}
