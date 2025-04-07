using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.ScheduledNotification;
using EventCountdowns.Core.Services.Tiles;

namespace EventCountdowns.Core.Services.EventCountdownManager;

public class CountdownsDataService : ICountdownsDataService
{
	private readonly IDataService _dataService;
	private readonly ITileService _tileService;
	private readonly IScheduledNotificationService _scheduledNotificationService;

	public CountdownsDataService(IDataService dataService, ITileService tileService,
		IScheduledNotificationService scheduledNotificationService)
	{
		_dataService = dataService;
		_tileService = tileService;
		_scheduledNotificationService = scheduledNotificationService;
	}

	public async Task AddCountdownAsync(EventCountdown eventCountdown)
	{
		eventCountdown.Id = Guid.NewGuid().ToString().ToLowerInvariant();
		await _dataService.AddCountdownAsync(eventCountdown);
		_tileService.UpdateMainTile((await _dataService.GetCountdownsAsync()).ToArray());
		_scheduledNotificationService.ScheduleCountdownNotification(eventCountdown);
		_tileService.ScheduleCountdownNotification(eventCountdown);
	}

	public async Task UpdateCountdownAsync(EventCountdown eventCountdown)
	{
		await _dataService.UpdateCountdownAsync(eventCountdown);
		_tileService.UpdateMainTile((await _dataService.GetCountdownsAsync()).ToArray());
		_scheduledNotificationService.UnscheduleCountdownNotification(eventCountdown);
		_scheduledNotificationService.ScheduleCountdownNotification(eventCountdown);
		_tileService.UnscheduleCountdownNotification(eventCountdown);
		_tileService.ScheduleCountdownNotification(eventCountdown);
		_tileService.UpdateCountdownTile(eventCountdown);
	}

	public async Task DeleteCountdownAsync(EventCountdown eventCountdown)
	{
		await _dataService.DeleteCountdownAsync(eventCountdown.Id);
		_tileService.UpdateMainTile((await _dataService.GetCountdownsAsync()).ToArray());
		_scheduledNotificationService.UnscheduleCountdownNotification(eventCountdown);
		_tileService.UnscheduleCountdownNotification(eventCountdown);
		await _tileService.UnpinCountdownAsync(eventCountdown);
	}
}
