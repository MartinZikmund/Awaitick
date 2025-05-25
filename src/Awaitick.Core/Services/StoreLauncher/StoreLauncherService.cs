using Windows.System;
using Awaitick.Core.Services.StoreLauncher;

namespace Awaitick.Core.Services;

public class StoreLauncherService : IStoreLauncherService
{
	public async Task RateAppAsync() => await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?PFN={Package.Current.Id.FamilyName}"));

	public async Task MoreAppsByPublisherAsync() => await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://publisher/?name=Sphereline"));

	public async Task ShowAppListingAsync() => await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?PFN={Package.Current.Id.FamilyName}"));
}
