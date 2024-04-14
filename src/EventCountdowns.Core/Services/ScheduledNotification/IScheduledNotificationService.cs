using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.Services.ScheduledNotification;

public interface IScheduledNotificationService
{
	void ScheduleCountdownNotification(EventCountdown eventCountdown);

	void UnscheduleCountdownNotification(EventCountdown eventCountdown);

	void SuppressCountdownNotification(EventCountdown eventCountdown);

	void UnSuppressAllCountdownNotifications();
}
