using Awaitick.Core.Models;
using Awaitick.Core.Services.Data;
using Awaitick.Core.Services.ScheduledNotification;
using Awaitick.Core.Services.Settings;
using Awaitick.Core.Services.Tiles;

namespace Awaitick.Core.Services.EventCountdownManager;

public class CountdownsDataService : ICountdownsDataService
{
	private readonly IDataService _dataService;
	private readonly ITileService _tileService;
	private readonly IScheduledNotificationService _scheduledNotificationService;
	private readonly IAppPreferences _appPreferences;

	public CountdownsDataService(IDataService dataService, ITileService tileService,
		IScheduledNotificationService scheduledNotificationService, IAppPreferences appPreferences)
	{
		_dataService = dataService;
		_tileService = tileService;
		_scheduledNotificationService = scheduledNotificationService;
		_appPreferences = appPreferences;
	}

	public async Task AddCountdownAsync(EventCountdown eventCountdown)
	{
		eventCountdown.Id = Guid.NewGuid().ToString().ToLowerInvariant();
		await _dataService.AddCountdownAsync(eventCountdown);
		_tileService.UpdateMainTile((await _dataService.GetCountdownsAsync()).ToArray());
		if (_appPreferences.NotificationsEnabled && _scheduledNotificationService.HasPermission)
		{
			_scheduledNotificationService.ScheduleCountdownNotification(eventCountdown);
		}
		_tileService.ScheduleCountdownNotification(eventCountdown);
	}

	public async Task UpdateCountdownAsync(EventCountdown eventCountdown)
	{
		await _dataService.UpdateCountdownAsync(eventCountdown);
		_tileService.UpdateMainTile((await _dataService.GetCountdownsAsync()).ToArray());
		_scheduledNotificationService.UnscheduleCountdownNotification(eventCountdown);
		if (_appPreferences.NotificationsEnabled && _scheduledNotificationService.HasPermission)
		{
			_scheduledNotificationService.ScheduleCountdownNotification(eventCountdown);
		}
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

	public async Task AddCountdownsAsync(params EventCountdown[] eventCountdowns)
	{
		foreach (var countdown in eventCountdowns)
		{
			countdown.Id = Guid.NewGuid().ToString().ToLowerInvariant();
		}

		await _dataService.AddCountdownsAsync(eventCountdowns);
		// TODO: Schedule notifications for all added countdowns
	}
}
