using CommunityToolkit.Mvvm.Input;
using EventCountdowns.Core.Services.StoreLauncher;
using EventCountdowns.Services.Navigation;
using EventCountdowns.ViewModels;

namespace EventCountdowns.Core.ViewModels;

public partial class AboutViewModel : PageViewModel
{
	private readonly IStoreLauncherService _storeLauncherService;

	public AboutViewModel(IStoreLauncherService storeLauncherService, INavigationService navigationService) : base(navigationService)
	{
		_storeLauncherService = storeLauncherService;
	}

	[RelayCommand]
	private async Task MoreAppsAsync()
	{
		IsWorking = true;
		await _storeLauncherService.MoreAppsByPublisherAsync();
		IsWorking = false;
	}
}
