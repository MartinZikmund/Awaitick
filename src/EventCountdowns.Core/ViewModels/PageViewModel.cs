
using System.Reflection;
using CommunityToolkit.Mvvm.Messaging;
using EventCountdowns.Core.Attributes;
using EventCountdowns.Core.Messages;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Services.Navigation;

namespace EventCountdowns.ViewModels;

public abstract partial class PageViewModel : ViewModelBase
{
	private readonly INavigationService _navigationService;
	private readonly IMessenger _messenger;

	protected PageViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
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

	public void ViewNavigatedToInternal(object? parameter)
	{
		OnPropertyChanged(nameof(CanGoBack));
		ViewNavigatedTo(parameter);
	}

	public virtual void ViewNavigatedTo(object? parameter) { }
}
