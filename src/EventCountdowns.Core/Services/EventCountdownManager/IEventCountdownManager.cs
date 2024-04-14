using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.Services.EventCountdownManager;

public interface IEventCountdownManager
{
	Task AddCountdownAsync(EventCountdown eventCountdown);
	Task UpdateCountdownAsync(EventCountdown eventCountdown);
	Task DeleteCountdownAsync(EventCountdown eventCountdown);
}
