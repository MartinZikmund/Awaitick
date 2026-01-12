#if __IOS__
using Awaitick.Core.Infrastructure;
using Awaitick.Core.Services.DeepLink;
using Awaitick.Services.ScheduledNotification;
using Foundation;
using UserNotifications;

namespace Awaitick.iOS;

/// <summary>
/// Handles notification responses and presentation on iOS.
/// </summary>
public class NotificationDelegate : UNUserNotificationCenterDelegate
{
	/// <summary>
	/// Called when user interacts with a notification (tap, action button).
	/// </summary>
	public override void DidReceiveNotificationResponse(
		UNUserNotificationCenter center,
		UNNotificationResponse response,
		Action completionHandler)
	{
		try
		{
			var userInfo = response.Notification.Request.Content.UserInfo;
			var countdownIdValue = userInfo.ObjectForKey(new NSString(NotificationConstants.CountdownIdKey));
			var countdownId = countdownIdValue?.ToString();

			if (response.ActionIdentifier == NotificationConstants.SnoozeActionIdentifier)
			{
				// Handle snooze - reschedule notification for 10 minutes later
				HandleSnooze(countdownId, response.Notification.Request.Content);
			}
			else if (response.ActionIdentifier == NotificationConstants.DismissActionIdentifier)
			{
				// Dismiss - nothing to do, notification is already dismissed
			}
			else if (!string.IsNullOrEmpty(countdownId))
			{
				// Default tap action - navigate to countdown
				try
				{
					var deepLinkService = IoC.GetService<IDeepLinkService>();
					deepLinkService?.SetPendingNavigation(countdownId);
				}
				catch
				{
					// IoC may not be initialized during cold start
					// Store in static field to be picked up later
					PendingCountdownId = countdownId;
				}
			}
		}
		finally
		{
			completionHandler();
		}
	}

	/// <summary>
	/// Called when a notification is about to be presented while the app is in foreground.
	/// </summary>
	public override void WillPresentNotification(
		UNUserNotificationCenter center,
		UNNotification notification,
		Action<UNNotificationPresentationOptions> completionHandler)
	{
		// Show notification banner and play sound even when app is in foreground
		completionHandler(UNNotificationPresentationOptions.Banner |
						 UNNotificationPresentationOptions.Sound |
						 UNNotificationPresentationOptions.List);
	}

	/// <summary>
	/// Static property to hold pending countdown ID during cold start.
	/// </summary>
	public static string? PendingCountdownId { get; set; }

	private void HandleSnooze(string? countdownId, UNNotificationContent originalContent)
	{
		if (string.IsNullOrEmpty(countdownId))
		{
			return;
		}

		var snoozeTime = DateTimeOffset.Now.AddMinutes(NotificationConstants.DefaultSnoozeDurationMinutes);

		var content = new UNMutableNotificationContent
		{
			Title = originalContent.Title,
			Body = originalContent.Body,
			Sound = UNNotificationSound.Default,
			CategoryIdentifier = NotificationConstants.iOSCategoryIdentifier,
			UserInfo = new NSDictionary(
				new NSString(NotificationConstants.CountdownIdKey),
				new NSString(countdownId))
		};

		var dateComponents = new NSDateComponents
		{
			Year = snoozeTime.Year,
			Month = snoozeTime.Month,
			Day = snoozeTime.Day,
			Hour = snoozeTime.Hour,
			Minute = snoozeTime.Minute,
			Second = snoozeTime.Second
		};

		var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponents, false);

		// Use a unique ID for the snooze notification
		var snoozeId = $"{countdownId}_snooze_{Guid.NewGuid():N}";
		var request = UNNotificationRequest.FromIdentifier(snoozeId, content, trigger);

		UNUserNotificationCenter.Current.AddNotificationRequest(request, error =>
		{
			if (error != null)
			{
				// TODO: Log error
				System.Diagnostics.Debug.WriteLine($"Failed to schedule snooze notification: {error}");
			}
		});
	}
}
#endif
