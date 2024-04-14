namespace EventCountdowns.Core.Services.StoreLauncher;

public interface IStoreLauncherService
{
	Task RateAppAsync();

	Task MoreAppsByPublisherAsync();

	Task ShowAppListingAsync();
}
