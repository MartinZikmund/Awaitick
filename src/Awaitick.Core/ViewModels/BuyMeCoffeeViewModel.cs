#nullable enable

using Awaitick.Core.Services.InAppPurchases;
using Awaitick.Services.Navigation;
using Awaitick.ViewModels;
using MZikmund.Services.Dialogs;

namespace Awaitick.Core.ViewModels;

public partial class BuyMeCoffeeViewModel : PageViewModel
{
	private readonly IInAppPurchaseService _inAppPurchaseService;
	private readonly IDialogService _dialogService;
	private readonly IStringLocalizer _localizationService;

	public BuyMeCoffeeViewModel(
		INavigationService navigationService,
		IInAppPurchaseService inAppPurchaseService,
		IDialogService messageDialogService,
		IStringLocalizer localizationService) :
		base(navigationService)
	{
		_inAppPurchaseService = inAppPurchaseService ?? throw new ArgumentNullException(nameof(inAppPurchaseService));
		_dialogService = messageDialogService ?? throw new ArgumentNullException(nameof(messageDialogService));
		_localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
	}

	[RelayCommand]
	private async Task DonateAsync(string? coffeeSize)
	{
		InAppProducts product = InAppProducts.LargeCoffee;
		switch (coffeeSize?.ToLowerInvariant())
		{
			case "small":
				{
					product = InAppProducts.SmallCoffee;
					break;
				}
			case "medium":
				{
					product = InAppProducts.MediumCoffee;
					break;
				}
			case "large":
				{
					product = InAppProducts.LargeCoffee;
					break;
				}
			case "giga":
				{
					product = InAppProducts.GigaCoffee;
					break;
				}
		}
		IsWorking = true;
		var result = await _inAppPurchaseService.PurchaseAsync(product);
		if (result)
		{
			//show dialog
			await _dialogService.ShowAsync(_localizationService.GetString("AppName"), _localizationService.GetString("CoffeeThankYou"));
		}
		IsWorking = false;
	}
}
