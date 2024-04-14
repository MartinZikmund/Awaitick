using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.StoreLauncher;

public interface IStoreLauncherService
{
	Task RateAppAsync();

	Task MoreAppsByPublisherAsync();

	Task ShowAppListingAsync();
}
