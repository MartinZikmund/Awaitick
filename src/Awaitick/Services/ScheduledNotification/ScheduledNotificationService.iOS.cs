#if __IOS__
using Awaitick.Core.Models;
using Awaitick.Core.Services.ScheduledNotification;
using Foundation;
using UserNotifications;

namespace Awaitick.Services.ScheduledNotification;

/// <summary>
/// iOS implementation of scheduled notifications using UNUserNotificationCenter.
/// </summary>
public class ScheduledNotificationService : IScheduledNotificationService
{
	private readonly HashSet<string> _suppressedNotifications = [];
	private bool _hasPermission;

	public ScheduledNotificationService()
	{
		RegisterNotificationCategories();
		CheckPermission();
	}

	public bool HasPermission => _hasPermission;

	private void RegisterNotificationCategories()
	{
		var snoozeAction = UNNotificationAction.FromIdentifier(
			NotificationConstants.SnoozeActionIdentifier,
			"Snooze",
			UNNotificationActionOptions.None);

		var dismissAction = UNNotificationAction.FromIdentifier(
			NotificationConstants.DismissActionIdentifier,
			"Dismiss",
			UNNotificationActionOptions.Destructive);

		var category = UNNotificationCategory.FromIdentifier(
			NotificationConstants.iOSCategoryIdentifier,
			new[] { snoozeAction, dismissAction },
			Array.Empty<string>(),
			UNNotificationCategoryOptions.CustomDismissAction);

		UNUserNotificationCenter.Current.SetNotificationCategories(
			new NSSet<UNNotificationCategory>(category));
	}

	private void CheckPermission()
	{
		UNUserNotificationCenter.Current.GetNotificationSettings(settings =>
		{
			_hasPermission =
				settings.AuthorizationStatus == UNAuthorizationStatus.Authorized ||
				settings.AuthorizationStatus == UNAuthorizationStatus.Provisional ||
				settings.AuthorizationStatus == UNAuthorizationStatus.Ephemeral;
		});
	}

	public void ScheduleCountdownNotification(EventCountdown eventCountdown)
	{
		var targetDate = eventCountdown.TargetDateTime;
		if (targetDate <= DateTimeOffset.Now.AddSeconds(NotificationConstants.MinimumFutureSecondsForScheduling))
		{
			return;
		}

		if (_suppressedNotifications.Contains(eventCountdown.Id))
		{
			return;
		}

		// Remove any existing notification first
		UnscheduleCountdownNotification(eventCountdown);

		var content = new UNMutableNotificationContent
		{
			Title = eventCountdown.Name ?? "Countdown Complete",
			Body = eventCountdown.CelebrationMessage ?? string.Empty,
			Sound = UNNotificationSound.Default,
			CategoryIdentifier = NotificationConstants.iOSCategoryIdentifier,
			UserInfo = new NSDictionary(
				new NSString(NotificationConstants.CountdownIdKey),
				new NSString(eventCountdown.Id))
		};

		// Convert to local time since UNCalendarNotificationTrigger interprets
		// date components in the device's local calendar/timezone
		var localDate = targetDate.LocalDateTime;
		var dateComponents = new NSDateComponents
		{
			Year = localDate.Year,
			Month = localDate.Month,
			Day = localDate.Day,
			Hour = localDate.Hour,
			Minute = localDate.Minute,
			Second = localDate.Second
		};

		var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponents, false);
		var request = UNNotificationRequest.FromIdentifier(eventCountdown.Id, content, trigger);

		UNUserNotificationCenter.Current.AddNotificationRequest(request, error =>
		{
			if (error != null)
			{
				// TODO: Log error
				System.Diagnostics.Debug.WriteLine($"Failed to schedule notification: {error}");
			}
		});
	}

	public void UnscheduleCountdownNotification(EventCountdown eventCountdown)
	{
		UNUserNotificationCenter.Current.RemovePendingNotificationRequests(
			new[] { eventCountdown.Id });

		// Also remove any delivered notifications
		UNUserNotificationCenter.Current.RemoveDeliveredNotifications(
			new[] { eventCountdown.Id });
	}

	public void SuppressCountdownNotification(EventCountdown eventCountdown)
	{
		_suppressedNotifications.Add(eventCountdown.Id);

		// Remove both pending and delivered notifications so suppressed countdowns don't fire later
		UNUserNotificationCenter.Current.RemovePendingNotificationRequests(
			new[] { eventCountdown.Id });
		UNUserNotificationCenter.Current.RemoveDeliveredNotifications(
			new[] { eventCountdown.Id });
	}

	public void UnSuppressAllCountdownNotifications()
	{
		_suppressedNotifications.Clear();
	}

	public async Task<bool> RequestPermissionAsync()
	{
		try
		{
			var (granted, error) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(
				UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge);

			_hasPermission = granted;
			return granted;
		}
		catch (Exception)
		{
			_hasPermission = false;
			return false;
		}
	}

	public Task OpenNotificationSettingsAsync()
	{
		var url = new NSUrl(UIKit.UIApplication.OpenSettingsUrlString);
		UIKit.UIApplication.SharedApplication.OpenUrl(url, new UIKit.UIApplicationOpenUrlOptions(), null);
		return Task.CompletedTask;
	}

	public async Task RescheduleAllNotificationsAsync(IEnumerable<EventCountdown> countdowns)
	{
		// Remove all pending countdown notifications
		var pendingRequests = await UNUserNotificationCenter.Current.GetPendingNotificationRequestsAsync();

		var countdownIds = pendingRequests
			.Where(r => r.Content.CategoryIdentifier == NotificationConstants.iOSCategoryIdentifier)
			.Select(r => r.Identifier)
			.ToArray();

		if (countdownIds.Length > 0)
		{
			UNUserNotificationCenter.Current.RemovePendingNotificationRequests(countdownIds);
		}

		// Schedule all future countdowns
		var now = DateTimeOffset.Now;
		foreach (var countdown in countdowns)
		{
			if (countdown.TargetDateTime > now)
			{
				ScheduleCountdownNotification(countdown);
			}
		}
	}
}
#endif
