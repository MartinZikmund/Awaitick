using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.Services.Countdowns;

public interface ICountdownsManager
{
	Task ShareAsync(CountdownViewModel countdown);

	void GoToEdit(CountdownViewModel countdown);

	Task<bool> DeleteAsync(CountdownViewModel countdown);

	void GoToDetail(CountdownViewModel countdown);
}
