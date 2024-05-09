using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.Services;

public interface IEventSharingService
{
	Task ShareAsync(EventCountdownObservable eventInfo);
}
