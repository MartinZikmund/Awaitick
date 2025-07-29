using Awaitick.Core.Models;

namespace Awaitick.Core.Services.EventCountdownManager;

public interface ICountdownsDataService
{
	Task AddCountdownsAsync(params EventCountdown[] eventCountdowns);
	Task AddCountdownAsync(EventCountdown eventCountdown);
	Task UpdateCountdownAsync(EventCountdown eventCountdown);
	Task DeleteCountdownAsync(EventCountdown eventCountdown);
}
