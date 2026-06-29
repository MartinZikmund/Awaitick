using Awaitick.Core.Models;

namespace Awaitick.Core.Services.Countdowns;

public interface ICountdownsManager
{
	Task ShareAsync(CountdownViewModel countdown);

	void GoToEdit(CountdownViewModel countdown);

	Task CloneAsync(CountdownViewModel countdown);

	Task<bool> DeleteAsync(CountdownViewModel countdown);

	void GoToDetail(CountdownViewModel countdown);
}
