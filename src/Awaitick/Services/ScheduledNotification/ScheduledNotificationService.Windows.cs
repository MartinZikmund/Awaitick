#if WINDOWS
using Awaitick.Core.Models;
using Awaitick.Core.Services.ScheduledNotification;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Awaitick.Services.ScheduledNotification;

/// <summary>
/// Windows implementation of scheduled notifications using toast notifications.
/// Uses native Windows.UI.Notifications APIs for WinUI 3 compatibility.
/// </summary>
public class ScheduledNotificationService : IScheduledNotificationService
{
	private readonly HashSet<string> _suppressedNotifications = [];

	public bool HasPermission => true; // Windows always has permission for toast notifications

	public void ScheduleCountdownNotification(EventCountdown eventCountdown)
	{
		var timeUntilTarget = eventCountdown.TargetDateTime - DateTimeOffset.Now;
		if (timeUntilTarget.TotalSeconds <= NotificationConstants.MinimumFutureSecondsForScheduling)
		{
			return;
		}

		try
		{
			// Remove any existing notification first
			UnscheduleCountdownNotification(eventCountdown);

			var isSuppressed = _suppressedNotifications.Contains(eventCountdown.Id);

			// Build toast XML manually for WinUI 3
			var toastXml = BuildToastXml(eventCountdown);

			var notification = new ScheduledToastNotification(toastXml, eventCountdown.TargetDateTime)
			{
				Id = GenerateNotificationId(eventCountdown.Id),
				Tag = "Countdown",
				Group = eventCountdown.Id,
				SuppressPopup = isSuppressed
			};

			var notifier = ToastNotificationManager.CreateToastNotifier();
			notifier.AddToSchedule(notification);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to schedule countdown notification: {ex}");
		}
	}

	public void UnscheduleCountdownNotification(EventCountdown eventCountdown)
	{
		try
		{
			var notifier = ToastNotificationManager.CreateToastNotifier();
			var scheduledNotifications = notifier.GetScheduledToastNotifications();
			var notificationId = GenerateNotificationId(eventCountdown.Id);

			foreach (var notification in scheduledNotifications)
			{
				if (notification.Id == notificationId || notification.Group == eventCountdown.Id)
				{
					notifier.RemoveFromSchedule(notification);
				}
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to unschedule countdown notification: {ex}");
		}
	}

	public void SuppressCountdownNotification(EventCountdown eventCountdown)
	{
		_suppressedNotifications.Add(eventCountdown.Id);

		// Reschedule with suppression
		ScheduleCountdownNotification(eventCountdown);
	}

	public void UnSuppressAllCountdownNotifications()
	{
		_suppressedNotifications.Clear();

		// Reschedule any toasts that were created with SuppressPopup = true
		try
		{
			var notifier = ToastNotificationManager.CreateToastNotifier();
			var scheduledNotifications = notifier.GetScheduledToastNotifications();

			foreach (var notification in scheduledNotifications)
			{
				if (notification.Tag == "Countdown" && notification.SuppressPopup)
				{
					notifier.RemoveFromSchedule(notification);

					var rescheduled = new ScheduledToastNotification(notification.Content, notification.DeliveryTime)
					{
						Id = notification.Id,
						Tag = notification.Tag,
						Group = notification.Group,
						SuppressPopup = false
					};

					notifier.AddToSchedule(rescheduled);
				}
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to unsuppress countdown notifications: {ex}");
		}
	}

	public Task<bool> RequestPermissionAsync()
	{
		// Windows doesn't require runtime permission for toast notifications
		return Task.FromResult(true);
	}

	public Task OpenNotificationSettingsAsync() => Task.CompletedTask;

	public Task RescheduleAllNotificationsAsync(IEnumerable<EventCountdown> countdowns)
	{
		// Clear all existing scheduled notifications
		try
		{
			var notifier = ToastNotificationManager.CreateToastNotifier();
			var scheduledNotifications = notifier.GetScheduledToastNotifications();

			foreach (var notification in scheduledNotifications)
			{
				if (notification.Tag == "Countdown")
				{
					notifier.RemoveFromSchedule(notification);
				}
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to clear existing scheduled notifications: {ex}");
		}

		// Schedule all future countdowns
		foreach (var countdown in countdowns)
		{
			if (countdown.TargetDateTime > DateTimeOffset.Now)
			{
				ScheduleCountdownNotification(countdown);
			}
		}

		return Task.CompletedTask;
	}

	/// <summary>
	/// Builds the toast notification XML manually.
	/// </summary>
	private static XmlDocument BuildToastXml(EventCountdown eventCountdown)
	{
		// Escape XML special characters
		var name = System.Security.SecurityElement.Escape(eventCountdown.Name ?? string.Empty);
		var message = System.Security.SecurityElement.Escape(eventCountdown.CelebrationMessage ?? string.Empty);
		var countdownId = System.Security.SecurityElement.Escape(eventCountdown.Id);

		var toastXmlString = $@"
<toast launch=""action=viewCountdown&amp;{NotificationConstants.CountdownIdKey}={countdownId}"" scenario=""reminder"">
    <visual>
        <binding template=""ToastGeneric"">
            <text>{name}</text>
            <text>{message}</text>
        </binding>
    </visual>
    <audio src=""ms-winsoundevent:Notification.Reminder""/>
    <actions>
        <input id=""snoozeTime"" type=""selection"" defaultInput=""10"">
            <selection id=""5"" content=""5 minutes""/>
            <selection id=""10"" content=""10 minutes""/>
            <selection id=""15"" content=""15 minutes""/>
            <selection id=""30"" content=""30 minutes""/>
            <selection id=""60"" content=""1 hour""/>
        </input>
        <action activationType=""system"" arguments=""snooze"" hint-inputId=""snoozeTime"" content=""Snooze""/>
        <action activationType=""system"" arguments=""dismiss"" content=""Dismiss""/>
    </actions>
</toast>";

		var toastXml = new XmlDocument();
		toastXml.LoadXml(toastXmlString);
		return toastXml;
	}

	/// <summary>
	/// Generates a notification ID from a countdown GUID.
	/// Windows has a 15-character limit for notification IDs.
	/// </summary>
	private static string GenerateNotificationId(string countdownId)
	{
		try
		{
			var bytes = new Guid(countdownId).ToByteArray();
			var base64 = Convert.ToBase64String(bytes)
				.Replace("/", "-")
				.Replace("+", "_")
				.Replace("=", "");

			return base64.Length > NotificationConstants.MaxNotificationIdLength
				? base64[..NotificationConstants.MaxNotificationIdLength]
				: base64;
		}
		catch
		{
			// Fallback for non-GUID IDs
			return countdownId.Length > NotificationConstants.MaxNotificationIdLength
				? countdownId[..NotificationConstants.MaxNotificationIdLength]
				: countdownId;
		}
	}
}
#endif
