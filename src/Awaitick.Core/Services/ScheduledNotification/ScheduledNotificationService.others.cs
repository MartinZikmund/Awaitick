using Awaitick.Core.Models;

namespace Awaitick.Core.Services.ScheduledNotification;

#if !__WASM__
public partial class ScheduledNotificationService : IScheduledNotificationService
{
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
}
#endif
