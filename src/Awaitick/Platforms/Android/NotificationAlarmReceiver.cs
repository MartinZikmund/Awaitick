#if __ANDROID__
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Awaitick.Core.Infrastructure;
using Awaitick.Core.Services.DeepLink;
using Awaitick.Core.Services.ScheduledNotification;

namespace Awaitick.Droid;

/// <summary>
/// BroadcastReceiver that handles scheduled alarm broadcasts and shows notifications.
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = false)]
public class NotificationAlarmReceiver : BroadcastReceiver
{
	public const string ExtraCountdownId = "countdown_id";
	public const string ExtraCountdownName = "countdown_name";
	public const string ExtraCountdownMessage = "countdown_message";
	public const string ActionSnooze = "dev.mzikmund.awaitick.SNOOZE";
	public const string ActionDismiss = "dev.mzikmund.awaitick.DISMISS";

	public override void OnReceive(Context? context, Intent? intent)
	{
		if (context == null || intent == null)
		{
			return;
		}

		var action = intent.Action;
		var countdownId = intent.GetStringExtra(ExtraCountdownId);

		if (string.IsNullOrEmpty(countdownId))
		{
			return;
		}

		if (action == ActionSnooze)
		{
			HandleSnooze(context, countdownId, intent);
			DismissNotification(context, countdownId);
			return;
		}

		if (action == ActionDismiss)
		{
			DismissNotification(context, countdownId);
			return;
		}

		// Show the notification
		ShowNotification(context, intent);
	}

	private void ShowNotification(Context context, Intent intent)
	{
		var countdownId = intent.GetStringExtra(ExtraCountdownId) ?? "";
		var name = intent.GetStringExtra(ExtraCountdownName) ?? "Countdown Complete";
		var message = intent.GetStringExtra(ExtraCountdownMessage) ?? "";
		var notificationId = NotificationConstants.GetStableId(countdownId);

		// Create tap intent to open the app and navigate to the countdown
		var tapIntent = new Intent(context, typeof(MainActivity));
		tapIntent.SetAction(Intent.ActionView);
		tapIntent.PutExtra(ExtraCountdownId, countdownId);
		tapIntent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

		var tapPendingIntent = PendingIntent.GetActivity(
			context,
			notificationId,
			tapIntent,
			PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

		// Create snooze intent
		var snoozeIntent = new Intent(context, typeof(NotificationAlarmReceiver));
		snoozeIntent.SetAction(ActionSnooze);
		snoozeIntent.PutExtra(ExtraCountdownId, countdownId);
		snoozeIntent.PutExtra(ExtraCountdownName, name);
		snoozeIntent.PutExtra(ExtraCountdownMessage, message);

		var snoozePendingIntent = PendingIntent.GetBroadcast(
			context,
			NotificationConstants.GetStableId(countdownId + "_action_snooze"),
			snoozeIntent,
			PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

		// Create dismiss intent
		var dismissIntent = new Intent(context, typeof(NotificationAlarmReceiver));
		dismissIntent.SetAction(ActionDismiss);
		dismissIntent.PutExtra(ExtraCountdownId, countdownId);

		var dismissPendingIntent = PendingIntent.GetBroadcast(
			context,
			NotificationConstants.GetStableId(countdownId + "_action_dismiss"),
			dismissIntent,
			PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

		// Build the notification
		var builder = new NotificationCompat.Builder(context, NotificationConstants.AndroidChannelId)
			.SetSmallIcon(Resource.Mipmap.icon_foreground)
			.SetContentTitle(name)
			.SetContentText(message)
			.SetPriority(NotificationCompat.PriorityHigh)
			.SetCategory(NotificationCompat.CategoryReminder)
			.SetAutoCancel(true)
			.SetContentIntent(tapPendingIntent)
			.AddAction(0, "Snooze", snoozePendingIntent)
			.AddAction(0, "Dismiss", dismissPendingIntent)
			.SetDefaults((int)NotificationDefaults.All);

		var notificationManager = NotificationManagerCompat.From(context);
		try
		{
			notificationManager.Notify(notificationId, builder.Build());
		}
		catch (Java.Lang.SecurityException)
		{
			// POST_NOTIFICATIONS permission not granted on Android 13+
		}
	}

	private void HandleSnooze(Context context, string countdownId, Intent originalIntent)
	{
		var name = originalIntent.GetStringExtra(ExtraCountdownName) ?? "Countdown";
		var message = originalIntent.GetStringExtra(ExtraCountdownMessage) ?? "";

		// Schedule a new alarm for snooze duration from now
		var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);
		if (alarmManager == null)
		{
			return;
		}

		var snoozeTime = DateTimeOffset.Now.AddMinutes(NotificationConstants.DefaultSnoozeDurationMinutes);
		var triggerTimeMillis = snoozeTime.ToUnixTimeMilliseconds();

		var intent = new Intent(context, typeof(NotificationAlarmReceiver));
		intent.PutExtra(ExtraCountdownId, countdownId);
		intent.PutExtra(ExtraCountdownName, name);
		intent.PutExtra(ExtraCountdownMessage, message);

		// Use a different request code for snooze to avoid conflicts
		var snoozeNotificationId = NotificationConstants.GetStableId($"{countdownId}_snooze");

		var pendingIntent = PendingIntent.GetBroadcast(
			context,
			snoozeNotificationId,
			intent,
			PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

		if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
		{
			alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTimeMillis, pendingIntent);
		}
		else
		{
			alarmManager.SetExact(AlarmType.RtcWakeup, triggerTimeMillis, pendingIntent);
		}
	}

	private void DismissNotification(Context context, string countdownId)
	{
		var notificationManager = NotificationManagerCompat.From(context);
		notificationManager.Cancel(NotificationConstants.GetStableId(countdownId));
	}
}
#endif
