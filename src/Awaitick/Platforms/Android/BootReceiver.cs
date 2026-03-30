#if __ANDROID__
using Android.App;
using Android.Content;
using Android.OS;
using Awaitick.Core.Models;
using Awaitick.Core.Services.ScheduledNotification;
using System.Text.Json;

namespace Awaitick.Droid;

/// <summary>
/// Reschedules all countdown notification alarms after device reboot.
/// AlarmManager alarms are cleared on reboot, so this receiver restores them.
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { Intent.ActionBootCompleted })]
public class BootReceiver : BroadcastReceiver
{
	private const string DataFileName = "events.data";

	public override void OnReceive(Context? context, Intent? intent)
	{
		if (context == null || intent?.Action != Intent.ActionBootCompleted)
		{
			return;
		}

		try
		{
			RescheduleNotifications(context);
		}
		catch
		{
			// Silently fail - notifications will be rescheduled when app is next opened
		}
	}

	private static void RescheduleNotifications(Context context)
	{
		var localAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
		var dataFilePath = Path.Combine(localAppData, DataFileName);

		if (!File.Exists(dataFilePath))
		{
			return;
		}

		var json = File.ReadAllText(dataFilePath);
		if (string.IsNullOrEmpty(json))
		{
			return;
		}

		var countdowns = JsonSerializer.Deserialize(json, EventCountdownSerializerContext.Default.ListEventCountdown);
		if (countdowns == null || countdowns.Count == 0)
		{
			return;
		}

		var alarmManager = (AlarmManager?)context.GetSystemService(Context.AlarmService);
		if (alarmManager == null)
		{
			return;
		}

		var now = DateTimeOffset.Now;
		foreach (var countdown in countdowns)
		{
			if (countdown.TargetDateTime > now)
			{
				ScheduleAlarm(context, alarmManager, countdown);
			}
		}
	}

	private static void ScheduleAlarm(Context context, AlarmManager alarmManager, EventCountdown countdown)
	{
		var triggerTime = countdown.TargetDateTime.ToUnixTimeMilliseconds();

		var intent = new Intent(context, typeof(NotificationAlarmReceiver));
		intent.PutExtra(NotificationAlarmReceiver.ExtraCountdownId, countdown.Id);
		intent.PutExtra(NotificationAlarmReceiver.ExtraCountdownName, countdown.Name);
		intent.PutExtra(NotificationAlarmReceiver.ExtraCountdownMessage, countdown.CelebrationMessage ?? string.Empty);

		var pendingIntent = PendingIntent.GetBroadcast(
			context,
			NotificationConstants.GetStableId(countdown.Id),
			intent,
			PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

		if (Build.VERSION.SdkInt >= BuildVersionCodes.S &&
			!alarmManager.CanScheduleExactAlarms())
		{
			alarmManager.SetAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTime, pendingIntent);
		}
		else if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
		{
			alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTime, pendingIntent);
		}
		else
		{
			alarmManager.SetExact(AlarmType.RtcWakeup, triggerTime, pendingIntent);
		}
	}
}
#endif
