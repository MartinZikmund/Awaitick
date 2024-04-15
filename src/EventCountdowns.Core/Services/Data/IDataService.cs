using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.Services.Data;

public interface IDataService
{
	Task InitializeAsync();

	Task<List<EventCountdown>> GetCountdownsAsync();

	Task UpdateCountdownAsync(EventCountdown eventCountdown);

	Task UpdateCountdownsAsync(params EventCountdown[] eventCountdowns);

	Task DeleteCountdownAsync(string id);

	Task AddCountdownAsync(EventCountdown eventCountdown);

	Task<EventCountdown> GetCountdownAsync(string id);

	Task AddCountdownsAsync(params EventCountdown[] sampleEvents);
}
