using Awaitick.Core.Models;

namespace Awaitick.Core.Services.ScheduledNotification;

public interface IScheduledNotificationService
{
	void ScheduleCountdownNotification(EventCountdown eventCountdown);

	void UnscheduleCountdownNotification(EventCountdown eventCountdown);

	void SuppressCountdownNotification(EventCountdown eventCountdown);

	void UnSuppressAllCountdownNotifications();
}
