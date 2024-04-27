#nullable enable

using System.Runtime.CompilerServices;
using EventCountdowns.Core.Services.Navigation;
using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.Services.InAppPurchases;
using EventCountdowns.Services.Navigation;

namespace EventCountdowns.Core.ViewModels;

public abstract partial class ViewModelBase : ObservableRecipient
{
	private readonly INavigationService _navigationService;
	[ObservableProperty]
	private bool _isWorking;

	[ObservableProperty]
	private string _title = "";

	protected ViewModelBase(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public bool CanGoBack => _navigationService.CanGoBack;

	public bool UserPremium => IoC.GetRequiredService<IInAppPurchaseService>().HasUserAnyProduct();

	[RelayCommand]
	public void GoBack() => _navigationService.GoBack();
}
