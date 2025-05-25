using Awaitick.Core.Models;

namespace Awaitick.Core.Services.Tiles;

public class TileService : ITileService
{
	public bool IsCountdownPinned(string id) => false;

	public Task<bool> PinCountdownAsync(EventCountdown eventCountdown) => Task.FromResult(false);

	public void ScheduleCountdownNotification(EventCountdown eventCountdown)
	{
	}

	public Task<bool> UnpinCountdownAsync(EventCountdown eventCountdown) => Task.FromResult(false);

	public void UnscheduleCountdownNotification(EventCountdown eventCountdown)
	{
	}

	public void UpdateCountdownTile(EventCountdown eventCountdown)
	{
	}

	public void UpdateMainTile(params EventCountdown[] countdowns)
	{
	}
}
