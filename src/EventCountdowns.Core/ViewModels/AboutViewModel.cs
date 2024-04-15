using EventCountdowns.Core.Services.StoreLauncher;
using EventCountdowns.ViewModels;

namespace EventCountdowns.Core.ViewModels;

public partial class AboutViewModel : PageViewModel
{
	private readonly IStoreLauncherService _storeLauncherService;

	public AboutViewModel(IStoreLauncherService storeLauncherService)
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
