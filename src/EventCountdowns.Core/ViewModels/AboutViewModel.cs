using EventCountdowns.Core.Services.StoreLauncher;

namespace EventCountdowns.Core.ViewModels;

public class AboutViewModel : ViewModelBase
{
	private readonly IStoreLauncherService _storeLauncherService;

	public AboutViewModel(IStoreLauncherService storeLauncherService)
	{
		_storeLauncherService = storeLauncherService;
	}

	public ICommand MoreAppsCommand => GetOrCreateCommand(MoreApps);

	private async void MoreApps()
	{
		IsWorking = true;
		await _storeLauncherService.MoreAppsByPublisherAsync();
		IsWorking = false;
	}
}
