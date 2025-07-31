using Awaitick.Core.ViewModels;
using Awaitick.Services.Navigation;
using Awaitick.Services.Store;

namespace Awaitick.ViewModels;

public partial class GetProViewModel : PageViewModel
{
	private readonly IStoreService _storeService;

	[ObservableProperty]
	public partial string? CurrentPrice { get; set; }
	
	public GetProViewModel(INavigationService navigationService, IStoreService storeService) : base(navigationService)
	{
		_storeService = storeService;
	}

	protected override async void ViewNavigatedTo(object? parameter)
	{
		var price = await _storeService.GetPriceAsync();
		if (price is not null)
		{
			CurrentPrice = price;
		}
	}

	[RelayCommand]
	public async Task GetProAsync()
	{
		var result = await _storeService.TryPurchaseProAsync();

		if (result)
		{
			GoBack();
		}
	}
}
