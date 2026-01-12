using Awaitick.Core.Models;

namespace Awaitick.Core.Services.ScheduledNotification;

public interface IScheduledNotificationService
{
	void ScheduleCountdownNotification(EventCountdown eventCountdown);

	void UnscheduleCountdownNotification(EventCountdown eventCountdown);

	void SuppressCountdownNotification(EventCountdown eventCountdown);

	void UnSuppressAllCountdownNotifications();

	/// <summary>
	/// Requests notification permission from the user.
	/// </summary>
	/// <returns>True if permission was granted, false otherwise.</returns>
	Task<bool> RequestPermissionAsync();

	/// <summary>
	/// Gets whether the app has notification permission.
	/// </summary>
	bool HasPermission { get; }

	/// <summary>
	/// Reschedules notifications for all provided countdowns.
	/// Used on app startup to ensure notifications persist after device restart or app updates.
	/// </summary>
	Task RescheduleAllNotificationsAsync(IEnumerable<EventCountdown> countdowns);
}
