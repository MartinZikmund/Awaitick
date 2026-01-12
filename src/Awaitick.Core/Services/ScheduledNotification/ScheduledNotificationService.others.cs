using Awaitick.Core.Models;

namespace Awaitick.Core.Services.ScheduledNotification;

/// <summary>
/// Fallback stub implementation for platforms without notification support.
/// </summary>
public class ScheduledNotificationService : IScheduledNotificationService
{
	public bool HasPermission => false;

	public void ScheduleCountdownNotification(EventCountdown eventCountdown)
	{
	}

	public void SuppressCountdownNotification(EventCountdown eventCountdown)
	{
	}

	public void UnscheduleCountdownNotification(EventCountdown eventCountdown)
	{
	}

	public void UnSuppressAllCountdownNotifications()
	{
	}

	public Task<bool> RequestPermissionAsync() => Task.FromResult(false);

	public Task RescheduleAllNotificationsAsync(IEnumerable<EventCountdown> countdowns) => Task.CompletedTask;
}
