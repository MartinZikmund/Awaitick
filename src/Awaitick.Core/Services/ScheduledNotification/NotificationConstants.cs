namespace Awaitick.Core.Services.ScheduledNotification;

/// <summary>
/// Shared constants for scheduled notifications across all platforms.
/// </summary>
public static class NotificationConstants
{
	/// <summary>
	/// Default snooze duration in minutes.
	/// </summary>
	public const int DefaultSnoozeDurationMinutes = 10;

	/// <summary>
	/// Minimum seconds in the future for scheduling a notification.
	/// Notifications closer than this are not scheduled.
	/// </summary>
	public const int MinimumFutureSecondsForScheduling = 3;

	/// <summary>
	/// Maximum length for notification IDs (Windows limitation).
	/// </summary>
	public const int MaxNotificationIdLength = 15;

	/// <summary>
	/// Notification channel ID for Android.
	/// </summary>
	public const string AndroidChannelId = "countdown_notifications";

	/// <summary>
	/// Notification channel name for Android.
	/// </summary>
	public const string AndroidChannelName = "Countdown Notifications";

	/// <summary>
	/// Notification channel description for Android.
	/// </summary>
	public const string AndroidChannelDescription = "Notifications for countdown completions";

	/// <summary>
	/// Category identifier for iOS notifications.
	/// </summary>
	public const string iOSCategoryIdentifier = "COUNTDOWN_CATEGORY";

	/// <summary>
	/// Snooze action identifier.
	/// </summary>
	public const string SnoozeActionIdentifier = "SNOOZE_ACTION";

	/// <summary>
	/// Dismiss action identifier.
	/// </summary>
	public const string DismissActionIdentifier = "DISMISS_ACTION";

	/// <summary>
	/// Key for countdown ID in notification data.
	/// </summary>
	public const string CountdownIdKey = "countdownId";

	/// <summary>
	/// Key for countdown name in notification data.
	/// </summary>
	public const string CountdownNameKey = "countdownName";

	/// <summary>
	/// Key for countdown message in notification data.
	/// </summary>
	public const string CountdownMessageKey = "countdownMessage";

	/// <summary>
	/// Gets a stable hash code for a countdown ID that is consistent across process restarts.
	/// String.GetHashCode() is randomized per process in .NET, making it unsuitable for
	/// PendingIntent request codes and notification IDs that must persist across restarts.
	/// </summary>
	public static int GetStableId(string countdownId)
	{
		if (Guid.TryParse(countdownId, out var guid))
		{
			var bytes = guid.ToByteArray();
			return BitConverter.ToInt32(bytes, 0) ^ BitConverter.ToInt32(bytes, 4)
				 ^ BitConverter.ToInt32(bytes, 8) ^ BitConverter.ToInt32(bytes, 12);
		}

		// Fallback: stable hash for non-GUID IDs
		unchecked
		{
			int hash = 17;
			foreach (char c in countdownId)
			{
				hash = hash * 31 + c;
			}
			return hash;
		}
	}
}
