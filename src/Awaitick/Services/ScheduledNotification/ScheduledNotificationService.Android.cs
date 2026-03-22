#if __ANDROID__
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Awaitick.Core.Models;
using Awaitick.Core.Services.ScheduledNotification;
using Awaitick.Droid;

namespace Awaitick.Services.ScheduledNotification;

/// <summary>
/// Android implementation of scheduled notifications using AlarmManager.
/// </summary>
public class ScheduledNotificationService : IScheduledNotificationService
{
	private readonly Context _context;
	private readonly AlarmManager _alarmManager;
	private readonly HashSet<string> _suppressedNotifications = [];
	private bool _hasPermission;

	public ScheduledNotificationService()
	{
		_context = Android.App.Application.Context;
		_alarmManager = (AlarmManager?)_context.GetSystemService(Context.AlarmService)
			?? throw new InvalidOperationException("AlarmManager not available");
		CreateNotificationChannel();
		CheckPermission();
	}

	public bool HasPermission => _hasPermission;

	private void CreateNotificationChannel()
	{
		if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
		{
			var channel = new NotificationChannel(
				NotificationConstants.AndroidChannelId,
				NotificationConstants.AndroidChannelName,
				NotificationImportance.High)
			{
				Description = NotificationConstants.AndroidChannelDescription
			};

			channel.EnableVibration(true);
			channel.EnableLights(true);

			var notificationManager = (NotificationManager?)_context.GetSystemService(Context.NotificationService);
			notificationManager?.CreateNotificationChannel(channel);
		}
	}

	private void CheckPermission()
	{
		if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
		{
			// Android 13+ requires POST_NOTIFICATIONS permission
			_hasPermission = _context.CheckSelfPermission(Android.Manifest.Permission.PostNotifications)
				== Android.Content.PM.Permission.Granted;
		}
		else
		{
			// Older Android versions don't require runtime permission
			_hasPermission = true;
		}
	}

	public void ScheduleCountdownNotification(EventCountdown eventCountdown)
	{
		var triggerTime = eventCountdown.TargetDateTime.ToUnixTimeMilliseconds();
		var currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

		if (triggerTime <= currentTime + (NotificationConstants.MinimumFutureSecondsForScheduling * 1000))
		{
			return;
		}

		try
		{
			// Remove any existing notification first
			UnscheduleCountdownNotification(eventCountdown);

			var intent = new Intent(_context, typeof(NotificationAlarmReceiver));
			intent.PutExtra(NotificationAlarmReceiver.ExtraCountdownId, eventCountdown.Id);
			intent.PutExtra(NotificationAlarmReceiver.ExtraCountdownName, eventCountdown.Name);
			intent.PutExtra(NotificationAlarmReceiver.ExtraCountdownMessage, eventCountdown.CelebrationMessage ?? string.Empty);

			var pendingIntent = PendingIntent.GetBroadcast(
				_context,
				NotificationConstants.GetStableId(eventCountdown.Id),
				intent,
				PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

			if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
			{
				_alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTime, pendingIntent);
			}
			else
			{
				_alarmManager.SetExact(AlarmType.RtcWakeup, triggerTime, pendingIntent);
			}
		}
		catch (Exception)
		{
			// TODO: Log error
		}
	}

	public void UnscheduleCountdownNotification(EventCountdown eventCountdown)
	{
		try
		{
			var intent = new Intent(_context, typeof(NotificationAlarmReceiver));
			var pendingIntent = PendingIntent.GetBroadcast(
				_context,
				NotificationConstants.GetStableId(eventCountdown.Id),
				intent,
				PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

			_alarmManager.Cancel(pendingIntent);

			// Also cancel any showing notification
			var notificationManager = NotificationManagerCompat.From(_context);
			notificationManager.Cancel(NotificationConstants.GetStableId(eventCountdown.Id));
		}
		catch (Exception)
		{
			// TODO: Log error
		}
	}

	public void SuppressCountdownNotification(EventCountdown eventCountdown)
	{
		_suppressedNotifications.Add(eventCountdown.Id);
		// On Android, we can't suppress a scheduled alarm, but we can cancel any showing notification
		var notificationManager = NotificationManagerCompat.From(_context);
		notificationManager.Cancel(NotificationConstants.GetStableId(eventCountdown.Id));
	}

	public void UnSuppressAllCountdownNotifications()
	{
		_suppressedNotifications.Clear();
	}

	public async Task<bool> RequestPermissionAsync()
	{
		if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
		{
			CheckPermission();
			if (_hasPermission)
			{
				return true;
			}

			var activity = global::Uno.UI.ContextHelper.Current as Android.App.Activity;
			if (activity != null)
			{
				ActivityCompat.RequestPermissions(activity,
					new[] { Android.Manifest.Permission.PostNotifications }, 0);
				// Brief delay for permission dialog result
				await Task.Delay(500);
				CheckPermission();
			}

			return _hasPermission;
		}

		// For older versions, permission is always granted
		_hasPermission = true;
		return true;
	}

	public Task OpenNotificationSettingsAsync()
	{
		try
		{
			var intent = new Intent(Android.Provider.Settings.ActionAppNotificationSettings);
			intent.PutExtra(Android.Provider.Settings.ExtraAppPackage, _context.PackageName);
			intent.AddFlags(ActivityFlags.NewTask);
			_context.StartActivity(intent);
		}
		catch (Exception)
		{
			// Fallback to general app settings
			var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
			intent.SetData(Android.Net.Uri.FromParts("package", _context.PackageName, null));
			intent.AddFlags(ActivityFlags.NewTask);
			_context.StartActivity(intent);
		}
		return Task.CompletedTask;
	}

	public Task RescheduleAllNotificationsAsync(IEnumerable<EventCountdown> countdowns)
	{
		// Cancel all existing countdown alarms
		// Note: Android doesn't provide a way to enumerate scheduled alarms,
		// so we just schedule new ones (they will replace existing ones with same request code)

		var now = DateTimeOffset.Now;
		foreach (var countdown in countdowns)
		{
			if (countdown.TargetDateTime > now)
			{
				ScheduleCountdownNotification(countdown);
			}
		}

		return Task.CompletedTask;
	}
}
#endif
